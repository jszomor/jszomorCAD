using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using EquipmentPosition;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrganiCAD.AutoCAD;
using JsonFindKey;

namespace jszomorCAD
{
  public class InsertBlockTable
  {
    private Database _db;

    public InsertBlockTable(Database db)
    {
      _db = db;
    }

    #region
    //public void InsertVfdPump(Database db, PromptIntegerResult numberOfItem, PromptIntegerResult distance, string blockName, string layerName, int visibilitystateIndex) => 
    //  InsertBlockTableMethod(db, numberOfItem, distance, blockName, layerName, "Centrifugal Pump", visibilitystateIndex); // todo: magic numberOfItem

    //public void InsertBlockTableMethodAsTable(Database db, InsertBlockBase insertData)
    //  => InsertBlockTableMethod(db, insertData);

    //public void InsertBlockTableMethodAsVisibility(Database db, InsertBlockBase insertData)
    //  => InsertBlockTableMethod(db, insertData);

    //public void InsertBlockTableMethodAsPipe(Database db, InsertBlockBase insertData)
    //  => InsertBlockTableMethod(db, insertData);



    #endregion
    public ObjectId GetBlockTable(string blockName)
    {
      var blockIds = new List<ObjectId>();

      using (var tr = _db.TransactionManager.StartTransaction())
      {
        var bt = _db.BlockTableId.GetObject<BlockTable>(OpenMode.ForRead);

        foreach (var btrId in bt)
        {
          using (var btr = tr.GetObject(btrId, OpenMode.ForRead, false) as BlockTableRecord)
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == blockName)
              blockIds.Add(btrId);
          }
        }
      }

      if (blockIds.Count > 1) throw new Exception($"More than one block record found with the name {blockName}");

      else if (blockIds.Count == 0) throw new Exception($"No block record found with the name {blockName}");

      else return blockIds.First();
    }

    private void PlaceBlock(ObjectId blockId, InsertBlockBase insertBlock, double offsetX = 0.0d, double offsetY = 0.0d)
    {
      //var defultLayers = new LayerCreator();
      using (var tr = _db.TransactionManager.StartTransaction())
      {
        var currentSpaceId = tr.GetObject(_db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        using (var blockDefinition = (BlockTableRecord)tr.GetObject(blockId, OpenMode.ForRead, false))
        {
          using (var acBlkRef = new BlockReference(
            new Point3d(insertBlock.Position.X + offsetX, insertBlock.Position.Y + offsetY, 0), blockId))
          {

            //InsertBlockBase insertData;
            currentSpaceId.AppendEntity(acBlkRef);
            tr.AddNewlyCreatedDBObject(acBlkRef, true);

            SetBlockReferenceLayer(acBlkRef, insertBlock.LayerName);
            SetRotate(acBlkRef, insertBlock.Rotation);
            CreateBlockRefenceAttributes(acBlkRef, blockDefinition, tr);
            SetVisibilityIndex(acBlkRef, insertBlock.StateProperty);
            SetBlockRefenceAttributesValues(acBlkRef, insertBlock.ActionToExecuteOnAttRef);
            SetDynamicBlockReferenceValues(acBlkRef, insertBlock.ActionToExecuteOnDynPropAfter);
            SetHostName(acBlkRef, insertBlock.HostName);
          }
        }
        tr.Commit();
      }
    }

    private void SetBlockReferenceLayer(BlockReference acBlkRef, string layerName)
    {
      try
      {
        acBlkRef.Layer = layerName;
      }
      catch (Autodesk.AutoCAD.Runtime.Exception ex)
      {
        if (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.KeyNotFound) throw new Exception($"Layer name not found: {layerName}");

        else throw;
      }
    }

    private void SetRotate(BlockReference acBlkRef, double rotation)
    {
      try
      {
        acBlkRef.Rotation = rotation;
      }
      catch (Autodesk.AutoCAD.Runtime.Exception ex)
      {
        if (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.KeyNotFound) throw new Exception($"Invalid number");

        else throw;
      }
    }

    private void SetHostName(BlockReference acBlkRef, string hostName)
    {
      foreach (ObjectId objectId in acBlkRef.AttributeCollection)
      {
        var ar = objectId.GetObject<AttributeReference>();
        if (ar == null) continue;

        if (ar.Tag == "HOSTNAME")
          ar.TextString = hostName;
      }
    }

    private void CreateBlockRefenceAttributes(BlockReference acBlkRef, BlockTableRecord blockDefinition, Transaction tr)
    {
      // copy/create attribute references
      foreach (var bdEntityObjectId in blockDefinition)
      {
        var ad = tr.GetObject(bdEntityObjectId, OpenMode.ForRead) as AttributeDefinition;
        if (ad == null) continue;

        using (var ar = new AttributeReference())
        {
          ar.SetDatabaseDefaults(_db);
          ar.SetAttributeFromBlock(ad, acBlkRef.BlockTransform);
          ar.TextString = ad.TextString; // set default value, copied from AttributeDefinition
          ar.AdjustAlignment(HostApplicationServices.WorkingDatabase);

          acBlkRef.AttributeCollection.AppendAttribute(ar);
          tr.AddNewlyCreatedDBObject(ar, true);
        }
      }
    }
    private void SetBlockRefenceAttributesValues(BlockReference acBlkRef,
      IEnumerable<Action<AttributeReference>> actionToExecuteOnAttRef)
    {
      if (actionToExecuteOnAttRef == null) return;

      foreach (ObjectId objectId in acBlkRef.AttributeCollection)
      {
        var ar = objectId.GetObject<AttributeReference>();
        if (ar == null) continue;

        foreach (var action in actionToExecuteOnAttRef)
        {
          action.Invoke(ar);
        }
      }
    }

    private void SetDynamicBlockReferenceValues(BlockReference acBlkRef,
      IEnumerable<Action<DynamicBlockReferenceProperty>> actionToExecuteOnDynProp)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
          foreach (var a in actionToExecuteOnDynProp)
          {
            a.Invoke(dbrProp);
          }
        }
      }
    }

    private void SetVisibilityIndex(BlockReference acBlkRef, EquipmentStateProperty stateProperty)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
          if (dbrProp.PropertyName == stateProperty.PropertyName)
            dbrProp.Value = stateProperty.Value;
        }
      }
    }

    public bool InsertBlock(InsertBlockBase insertData)
    {
      // 1. which block to insert? insertData.BlockName
      // get the block to insert
      var blockId = GetBlockTable(insertData.BlockName);

      var offsetX = 0.0d;
      var offsetY = 0.0d;
      // 2. insert block
      for (var i = 0; i < insertData.NumberOfItem; i++)
      {
        PlaceBlock(blockId, insertData, offsetX, offsetY);
        offsetX += insertData.OffsetX;
        offsetY += insertData.OffsetY;
      }
      return true;
    }

    public void ReadBlockTableRecord()
    {
      List<string> list = new List<string>();

      using (var tr = _db.TransactionManager.StartTransaction())
      {
        var bt = _db.BlockTableId.GetObject<BlockTable>(OpenMode.ForRead);

        foreach (var btrId in bt)
        {
          using (var btr = tr.GetObject(btrId, OpenMode.ForRead, false) as BlockTableRecord)
          {
            foreach (var objectId in btr)
            {
              using (var acBlkRef = tr.GetObject(objectId, OpenMode.ForRead) as Entity)
              {
                //var X = insertBlock.Position.X;
                //var Y = insertBlock.Position.Y;

                //var jsonseri = new JsonSerializer();
                if (!btr.IsAnonymous && !btr.IsLayout)
                  list.Add(acBlkRef.Layer);

                //SetBlockReferenceLayer(acBlkRef, insertBlock.LayerName);
                //SetRotate(acBlkRef, insertBlock.Rotation);
                //CreateBlockRefenceAttributes(acBlkRef, btr, tr);
                //SetVisibilityIndex(acBlkRef, insertBlock.StateProperty);
                //SetBlockRefenceAttributesValues(acBlkRef, insertBlock.ActionToExecuteOnAttRef);
                //SetDynamicBlockReferenceValues(acBlkRef, insertBlock.ActionToExecuteOnDynPropAfter);
                //SetHostName(acBlkRef, insertBlock.HostName);
              }
            }
            // Only add named & non-layout blocks to the copy list
            //if (!btr.IsAnonymous && !btr.IsLayout)
            //  list.Add(btr.Name);
          }
        }

        var jsonseri = new JsonSerializer();
        jsonseri.JsonSeri(list);
      }
    }

    #region OLD CODE
    public void InsertBlockTableMethod(InsertBlockBase insertData)
    {
      var sizeProperty = new PositionProperty();
      sizeProperty.FreeSpace = 60;

      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;     
      var aw = new AutoCadWrapper();
      BlockTableRecord btr;

      var blockDefinitions = new List<ObjectId>();
      var positionProperty = new PositionProperty();

      //setup default layers
      var defultLayers = new LayerCreator();
      defultLayers.Layers();
    
      //var shortvisibilitystateIndex = Convert.ToInt16(visibilitystateIndex);

      // Start transaction to insert equipment
      aw.ExecuteActionOnBlockTable(_db, (tr, bt) =>
      {        
        foreach (ObjectId btrId in bt)
        {
          using (btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead, false))
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == insertData.BlockName)
            {  
              blockDefinitions.Add(btrId);             
            }
          }
        }        

        var currentSpaceId = tr.GetObject(_db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        for (int i = 0; i < insertData.NumberOfItem; i++)
        {
          foreach (var objectId in blockDefinitions)
          {
            using (var blockDefinition = (BlockTableRecord)tr.GetObject(objectId, OpenMode.ForRead, false))
            {
              using (var acBlkRef = new BlockReference(new Point3d(insertData.X, insertData.Y, positionProperty.Z), objectId))
              {
                currentSpaceId.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);

                acBlkRef.Layer = insertData.LayerName;                

                // copy/create attribute references
                foreach (var bdEntityObjectId in blockDefinition)
                {
                  var ad = tr.GetObject(bdEntityObjectId, OpenMode.ForRead) as AttributeDefinition;
                  if (ad == null) continue;

                  var ar = new AttributeReference();
                  ar.SetDatabaseDefaults(_db);
                  ar.SetAttributeFromBlock(ad, acBlkRef.BlockTransform);
                  ar.TextString = ad.TextString;
                  ar.AdjustAlignment(HostApplicationServices.WorkingDatabase);

                  acBlkRef.AttributeCollection.AppendAttribute(ar);
                  tr.AddNewlyCreatedDBObject(ar, true);
                
                  System.Diagnostics.Debug.Print($"Tag={ar.Tag} TextString={ar.TextString}");

                  if (acBlkRef.IsDynamicBlock)
                  {
                    foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection) // this loop must be here
                                                                                                                        //else tag rotation for pump will be wrong!
                    {
                      // for jet pump rotate
                      if (ar.TextString == "Air Jet Pump")                      
                        acBlkRef.Rotation = DegreeHelper.DegreeToRadian(270); // this command must be here else tag rotation will be wrong!
                    }
                  }

                  //text for EQ tank - Attributes
                  if (ar.Tag == "NAME1" && insertData.BlockName == "chamber")
                    ar.TextString = "EQUALIZATION";
                  if (ar.Tag == "NAME2" && insertData.BlockName == "chamber")
                    ar.TextString = "TANK";

                  //valve setup
                  if (insertData.BlockName == "valve")
                  {
                    //ar.Rotation = DegreeHelper.DegreeToRadian(90);
                    acBlkRef.Rotation = DegreeHelper.DegreeToRadian(90);
                  }
                }

                // setup item by index
                #region
                //if (acBlkRef.IsDynamicBlock)
                //{
                //  foreach (DynamicBl ockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
                //  {       
                //    if (dbrProp.PropertyName == PropertyName)                                    
                //      dbrProp.Value = visibilitystateIndex; // SHORT !!!!!!!!!!!!                                                           
                //  }
                //}
                #endregion

                // udpate attribute reference values after setting the visibility state or block table index
                foreach (ObjectId arObjectId in acBlkRef.AttributeCollection)
                {
                  foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
                  {
                    var ar = arObjectId.GetObject<AttributeReference>();
                    if (ar == null) continue;
                    
                    //pipe setup
                    if (dbrProp.PropertyName == "PipeLength")
                      dbrProp.Value = insertData.PipeLength;
                    if (dbrProp.PropertyName == "ArrowPosition Y")
                      dbrProp.Value = insertData.PipeLength;

                    // for jet pump tag position
                    if (ar.Tag == "NOTE" && ar.TextString == "Air Jet Pump" && insertData.BlockName == "pump")
                    {
                      //acBlkRef.Rotation = DegreeHelper.DegreeToRadian(270); // this command has wrong result that is why should be in the upper loop.

                      if (acBlkRef.IsDynamicBlock)
                      {
                        // tag horizontal positioning
                        if (dbrProp.PropertyName == "Angle")
                          dbrProp.Value = DegreeHelper.DegreeToRadian(90);
                        if (dbrProp.PropertyName == "Position X")
                          dbrProp.Value = (double)6;
                        if (dbrProp.PropertyName == "Position Y")
                          dbrProp.Value = (double)0;
                      }
                    }
                    //for pumps VFD rotate
                    if (dbrProp.PropertyName == "Angle1" && ar.TextString == "Equalization Tank Pump")
                      dbrProp.Value = DegreeHelper.DegreeToRadian(90);

                    // pumps VFD rotate
                    if (dbrProp.PropertyName == "Angle2" && ar.TextString == "Equalization Tank Pump")
                      dbrProp.Value = DegreeHelper.DegreeToRadian(270);

                    //setup chamber width
                    //if (dbrProp.PropertyName == "Distance" && insertData.BlockName == "chamber")
                    //  dbrProp.Value = PositionProperty.NumberOfPump * PositionProperty.DistanceOfPump + sizeProperty.FreeSpace; //last value is the free space for other items
                    ////text position for chamber
                    //if (dbrProp.PropertyName == "Position X" && insertData.BlockName == "chamber")
                    //  dbrProp.Value = ((PositionProperty.NumberOfPump * PositionProperty.DistanceOfPump + sizeProperty.FreeSpace) / (double)2); //place text middle of chamber horizontaly 
                  }
                }
              }
            }
          }
          //insertData.X += insertData.X;
          //insertData.X += insertData.Distance;
        }
        currentSpaceId.UpdateAnonymousBlocks();
      });
      #endregion
    }
  } 
}
using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using EquipmentPosition;
using OrganiCAD.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jszomorCAD
{
  public class InsertBlockTable
  {
    //public void InsertVfdPump(Database db, PromptIntegerResult numberOfItem, PromptIntegerResult distance, string blockName, string layerName, int eqIndex) => 
    //  InsertBlockTableMethod(db, numberOfItem, distance, blockName, layerName, "Centrifugal Pump", eqIndex); // todo: magic numberOfItem

    public void InsertBlockTableMethodAsTable(Database db, double numberOfItem, double distance, string blockName, string layerName, string propertyName,
      short eqIndex, double X, double Y, double NA)
      => InsertBlockTableMethod(db, numberOfItem, distance, blockName, layerName, propertyName, eqIndex, X, Y, NA);

    public void InsertBlockTableMethodAsVisibility(Database db, double numberOfItem, double distance, string blockName, string layerName, string propertyName,
      string visibilityStateName, double X, double Y, double NA, string NAm)
      => InsertBlockTableMethod(db, numberOfItem, distance, blockName, layerName, propertyName, visibilityStateName, X, Y, NA, NAm);

    public void InsertBlockTableMethodAsPipe(Database db, double numberOfItem, double distance, string blockName, string layerName, string propertyName,
      string visibilityStateName, double X, double Y, double pipeLength, string master)
      => InsertBlockTableMethod(db, numberOfItem, distance, blockName, layerName, propertyName, visibilityStateName, X, Y, pipeLength, master);    


    public void InsertBlockTableBaseMethod(Database db, InsertBlockBase insertData)
    {
      // 1. which block to inster? insertData.BlockName
      // get the block to insert


    }


    public void InsertBlockTableMethod(Database db, InsertBlockProperty insertBlockProperty)
    {
      var sizeProperty = new PositionProperty();
      sizeProperty.FreeSpace = 60;

      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;     
      var aw = new AutoCadWrapper();
      BlockTableRecord btr;
      var blockDefinitions = new List<ObjectId>();
      var positionProperty = new PositionProperty();

      //setup default layers
      var layerCreator = new LayerCreator();
      layerCreator.LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.25);      
      layerCreator.LayerCreatorMethod("unit", Color.FromRgb(255, 0, 0), 0.25);
      layerCreator.LayerCreatorMethod("valve", Color.FromRgb(255, 255, 255), 0.25);
      layerCreator.LayerCreatorMethod("valve2", Color.FromRgb(255, 255, 255), 0.25);
      layerCreator.LayerCreatorMethod("instrumentation", Color.FromRgb(0, 255, 255), 0.25);
      layerCreator.LayerCreatorMethod("text", Color.FromRgb(255, 255, 255), 0.25);
      layerCreator.LayerCreatorMethod("sewer", Color.FromRgb(28, 38, 0), 0.25);
      layerCreator.LayerCreatorMethod("sludge", Color.FromRgb(38, 19, 19), 0.25);
      layerCreator.LayerCreatorMethod("chemical", Color.FromRgb(0, 255, 255), 0.25);
      layerCreator.LayerCreatorMethod("water", Color.FromRgb(0, 0, 255), 0.25);
      layerCreator.LayerCreatorMethod("treated_water", Color.FromRgb(0, 127, 255), 0.25);
      layerCreator.LayerCreatorMethod("air", Color.FromRgb(63, 255, 0), 0.25);
      layerCreator.LayerCreatorMethod("recycle_flow", Color.FromRgb(145, 165, 82), 0.25);

      //var shortEqIndex = Convert.ToInt16(eqIndex);

      // Start transaction to insert equipment
      aw.ExecuteActionOnBlockTable(db, (tr, bt) =>
      {        
        foreach (ObjectId btrId in bt)
        {
          using (btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead, false))
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == insertBlockProperty.BlockName)
            {  
              blockDefinitions.Add(btrId);             
            }
          }
        }        

        var currentSpaceId = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        for (int i = 0; i < insertBlockProperty.NumberOfItem; i++)
        {
          foreach (var objectId in blockDefinitions)
          {
            using (var blockDefinition = (BlockTableRecord)tr.GetObject(objectId, OpenMode.ForRead, false))
            {
              using (var acBlkRef = new BlockReference(new Point3d(insertBlockProperty.X, insertBlockProperty.Y, positionProperty.Z), objectId))
              {
                currentSpaceId.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);

                acBlkRef.Layer = insertBlockProperty.LayerName;                

                // copy/create attribute references
                foreach (var bdEntityObjectId in blockDefinition)
                {
                  var ad = tr.GetObject(bdEntityObjectId, OpenMode.ForRead) as AttributeDefinition;
                  if (ad == null) continue;

                  var ar = new AttributeReference();
                  ar.SetDatabaseDefaults(db);
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
                      if (dbrProp.PropertyName == insertBlockProperty.PropertyName)
                        dbrProp.Value = insertBlockProperty.EqIndex; // SHORT !!!!!!!!!!!!

                      // for jet pump rotate
                      if (ar.TextString == "Air Jet Pump")                      
                        acBlkRef.Rotation = DegreeHelper.DegreeToRadian(270); // this command must be here else tag rotation will be wrong!
                    }
                  }

                  //text for EQ tank - Attributes
                  if (ar.Tag == "NAME1" && insertBlockProperty.BlockName == "chamber")
                    ar.TextString = "EQUALIZATION";
                  if (ar.Tag == "NAME2" && insertBlockProperty.BlockName == "chamber")
                    ar.TextString = "TANK";

                  //valve setup
                  if (insertBlockProperty.BlockName == "valve")
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
                //    if (dbrProp.PropertyName == propertyName)                                    
                //      dbrProp.Value = eqIndex; // SHORT !!!!!!!!!!!!                                                           
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

                    if (dbrProp.PropertyName == "PipeLength")
                      dbrProp.Value = insertBlockProperty.PipeLength;

                    // for jet pump tag position
                    if (ar.Tag == "NOTE" && ar.TextString == "Air Jet Pump" && insertBlockProperty.BlockName == "pump")
                    {
                      //acBlkRef.Rotation = DegreeHelper.DegreeToRadian(270); // this command has a wrong result that is why should be in the upper loop.

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
                    if (dbrProp.PropertyName == "Distance" && insertBlockProperty.BlockName == "chamber")
                      dbrProp.Value = PositionProperty.NumberOfPump * PositionProperty.DistanceOfPump + sizeProperty.FreeSpace; //last value is the free space for other items
                    //text position for chamber
                    if (dbrProp.PropertyName == "Position X" && insertBlockProperty.BlockName == "chamber")
                      dbrProp.Value = ((PositionProperty.NumberOfPump * PositionProperty.DistanceOfPump + sizeProperty.FreeSpace) / (double)2); //place text middle of chamber horizontaly 
                  }
                }
              }
            }
          }
          insertBlockProperty.X += insertBlockProperty.Distance;
        }
        currentSpaceId.UpdateAnonymousBlocks();
      });
    }
  } 
}
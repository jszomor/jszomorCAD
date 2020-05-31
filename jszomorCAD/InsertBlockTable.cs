﻿using Autodesk.AutoCAD.ApplicationServices.Core;
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
using JsonParse;
using System.Linq.Expressions;
using jCAD.PID_Builder;

namespace jszomorCAD
{
  public class InsertBlockTable
  {

    //public void ExecuteActionOnModelSpace(Database database, Action<Transaction, BlockTableRecord> action)
    //{
    //  ExecuteActionInTransaction(database, (db, tr) =>
    //    ExecuteActionOnBlockTable(db, bt =>
    //    {
    //      using (var ms = bt[BlockTableRecord.ModelSpace].GetObject<BlockTableRecord>())
    //      {
    //        action.Invoke(tr, ms);
    //      }
    //    }
    //    ));
    //}

    //public void ExecuteActionInTransaction(Database db, Action<Database, Transaction> action)
    //{
    //  using (var tr = db.TransactionManager.StartTransaction())
    //  {
    //    action.Invoke(db, tr);
    //    tr.Commit();
    //  }
    //}

    //private void ExecuteActionOnTable<T>(Database db,
    //  Expression<Func<Database, ObjectId>> tableIdProperty, Action<T> action) where T : class, IDisposable
    //{
    //  var c = tableIdProperty.Compile();
    //  using (var t = c.Invoke(db).GetObject<T>())
    //  {
    //    action.Invoke(t);
    //  }
    //}

    //public void ExecuteActionOnBlockTable(Database db, Action<BlockTable> action) =>
    //  ExecuteActionOnTable(db, x => x.BlockTableId, action);

    //public void ExecuteActionOnLayerTable(Database db, Action<LayerTable> action) =>
    //  ExecuteActionOnTable(db, x => x.LayerTableId, action);

    /// <summary>
    /// wrapper from OrganiCad
    /// </summary>
    /// 

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
        BlockTable bt = _db.BlockTableId.GetObject(OpenMode.ForRead) as BlockTable;

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
        AttributeReference ar = objectId.GetObject(OpenMode.ForRead) as AttributeReference;
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
    private void SetBlockRefenceAttributesValues(BlockReference acBlkRef, IEnumerable<Action<AttributeReference>> actionToExecuteOnAttRef)
    {
      if (actionToExecuteOnAttRef == null) return;

      foreach (ObjectId objectId in acBlkRef.AttributeCollection)
      {
        AttributeReference ar = objectId.GetObject(OpenMode.ForRead) as AttributeReference;
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

    public void ReadBlockTableRecord(Database db)
    {
      ExtensionMethod.ExecuteActionOnModelSpace(db, (tr, btrModelSpace) =>
      {
        foreach (ObjectId objectId in btrModelSpace)
        {
          using (var blockReference = tr.GetObject(objectId, OpenMode.ForRead) as BlockReference)
          {
            if (blockReference == null) continue;

            var btrObjectId = blockReference.DynamicBlockTableRecord; //must be Dynamic to find every blocks
            using (var blockDefinition = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord)
            {
              //System.Diagnostics.Debug.Print(blockDefinition.Name);

              //if (blockDefinition.Name == "RefPIDDenit$0$reactor")
              //{
              //  System.Diagnostics.Debug.Print("STOP! Hammertime!");
              //}

              var jsonBlockProperty = new JsonBlockProperty();
              if (!blockDefinition.IsAnonymous && !blockDefinition.IsLayout)
              {
                //jsonBlockProperty.Misc = new Misc(blockName: blockDefinition.Name, rotation: 0);
              }

              if (blockReference.IsDynamicBlock)
              {
                foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
                {
                }
              }

              foreach (ObjectId BlockObjectId in blockDefinition)
              {
                var entity = tr.GetObject(BlockObjectId, OpenMode.ForWrite) as Autodesk.AutoCAD.DatabaseServices.Entity;

                if (entity == null) continue;

              }
            }
          }
        }
      });
    }
    public void IterateBTRForSetupBlockVisibility(string blockName, short visiblityIndex, string Note, Database db)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead, false) as BlockTableRecord;
        foreach (ObjectId objectId in btr)
        {
          BlockReference blockReference;
          //ObjectId blockId = insertBlockTable.GetBlockTable(blockName, db, tr);
          using (blockReference = tr.GetObject(objectId, OpenMode.ForRead) as BlockReference)
          {
            if (blockReference == null) continue;

            var btrObjectId = blockReference.DynamicBlockTableRecord;
            var blockDefinition = tr.GetObject(btrObjectId, OpenMode.ForWrite, false) as BlockTableRecord;
            if (blockDefinition.Name.EndsWith(blockName))
            {
              //string validBlockName = //RealNameFinder(btr.Name);
              AttributeCollection attributeCollection = blockReference.AttributeCollection;
              foreach (ObjectId objIdAtt in attributeCollection)
              {
                using (AttributeReference attRef = tr.GetObject(objIdAtt, OpenMode.ForWrite) as AttributeReference)
                {
                  if (attRef.Tag == "NOTE" && attRef.TextString == Note)
                  {
                    if (blockReference.IsDynamicBlock)
                    {
                      foreach (DynamicBlockReferenceProperty dbProp in blockReference.DynamicBlockReferencePropertyCollection)
                      {
                        if (dbProp.PropertyName == "Centrifugal Pump")
                        {
                          dbProp.Value = visiblityIndex;
                        }
                        else if (dbProp.PropertyName == "Block Table1")
                        {
                          dbProp.Value = visiblityIndex;
                        }
                      }
                    }
                  }
                }
              }
            }
          }
        }
        tr.Commit();
      }
    }
    public void Numbering(Database db, bool isLeftToDownSelection)
    {
      using (Transaction tr = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;
        BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead, false) as BlockTableRecord;
        int nextNumber = 1;

        IEnumerable<BlockReference> blkRefList = GetBlockRef(btr, tr, isLeftToDownSelection);
        
        foreach (var blockReference in blkRefList)
        {
          if (blockReference == null) continue;

          AttributeCollection attributeCollection = blockReference.AttributeCollection;
          foreach (ObjectId objIdAtt in attributeCollection)
          {
            using (AttributeReference attRef = tr.GetObject(objIdAtt, OpenMode.ForWrite) as AttributeReference)
            {
              if (attRef.Tag == "TAG")
              {
                attRef.TextString = nextNumber.ToString("D3");
              }
            }
          }
          ++nextNumber;
        }
        tr.Commit();
      }
    }
    public IEnumerable<BlockReference> GetBlockRef(BlockTableRecord btr, Transaction tr, bool isLeftToDownSelection)
    {
      List<BlockReference> blkRefList = new List<BlockReference>();
      blkRefList
            .OrderBy(x => isLeftToDownSelection ? x.Position.Y : -(x.Position.X))
            .ThenBy(x => isLeftToDownSelection ? -(x.Position.X) : x.Position.Y)
            .ToList();

      foreach (ObjectId objectId in btr)
      {
        var br = tr.GetObject(objectId, OpenMode.ForRead) as BlockReference;
        blkRefList.Add(br);
      }

      return blkRefList;
    }
  }
}
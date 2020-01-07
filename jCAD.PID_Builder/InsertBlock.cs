using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using OrganiCAD.AutoCAD;

namespace jCAD.PID_Builder
{
  public class InsertBlock
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

    private Database _db;
    public InsertBlock(Database db)
    {
      _db = db;
    }

    public ObjectId GetBlockTable(string blockName)
    {
      var blockIds = new List<ObjectId>();

      using (var tr = _db.TransactionManager.StartTransaction())
      {
        BlockTable bt = _db.BlockTableId.GetObject<BlockTable>(OpenMode.ForRead);

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

    public void PlaceBlock()
    {
      var blockDeserialize = new BlockDeserialize();
      ObjectId blockId = GetBlockTable(Convert.ToString(blockDeserialize.BlockSearch("Name")));

      //var defultLayers = new LayerCreator();
      using (var tr = _db.TransactionManager.StartTransaction())
      {
        var btr = tr.GetObject(_db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        using (var blockDefinition = (BlockTableRecord)tr.GetObject(blockId, OpenMode.ForRead, false))
        {
          using (var acBlkRef = new BlockReference(
            new Point3d(Convert.ToInt64(blockDeserialize.BlockSearch("Position X")),
                        Convert.ToInt64(blockDeserialize.BlockSearch("Position Y")), 0), blockId))
          {
            
            btr.AppendEntity(acBlkRef);
            tr.AddNewlyCreatedDBObject(acBlkRef, true);

            SetBlockReferenceLayer(acBlkRef, Convert.ToString(blockDeserialize.BlockSearch("Layer")));
            SetRotate(acBlkRef, Convert.ToInt64(blockDeserialize.BlockSearch("Rotation")));
            CreateBlockRefenceAttributes(acBlkRef, blockDefinition, tr);
            SetDynamicReference(acBlkRef, blockDeserialize);
            //SetBlockRefenceAttributesValues(acBlkRef, insertBlock.ActionToExecuteOnAttRef);
            //SetDynamicBlockReferenceValues(acBlkRef, insertBlock.ActionToExecuteOnDynPropAfter);
            //SetHostName(acBlkRef, insertBlock.HostName);
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

    private void SetDynamicReference(BlockReference acBlkRef, BlockDeserialize blockDeserialize)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
          if (dbrProp.PropertyName == "Centrifugal Pump" && acBlkRef.Name == "pump")
            dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Centrifugal Pump"));

          else if (dbrProp.PropertyName == "Visibility" && acBlkRef.Name == "chamber")
            dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Visibility"));

          else if (dbrProp.PropertyName == "Block Table1")
            dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Block Table1"));

          if (dbrProp.PropertyName == "Position X") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("TAG X")); continue; }
          if (dbrProp.PropertyName == "Position Y") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("TAG Y")); continue; }
          if (dbrProp.PropertyName == "Position1 X") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("TAG1 X")); continue; }
          if (dbrProp.PropertyName == "Position1 Y") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("TAG1 Y")); continue; }
          if (dbrProp.PropertyName == "Angle") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Angle")); continue; }
          if (dbrProp.PropertyName == "Angle1") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Angle1")); continue; }
          if (dbrProp.PropertyName == "Angle2") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Angle2")); continue; }
          if (dbrProp.PropertyName == "Distance") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Distance")); continue; }
          if (dbrProp.PropertyName == "Distance1") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Distance1")); continue; }
          if (dbrProp.PropertyName == "Distance2") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Distance2")); continue; }
          if (dbrProp.PropertyName == "Distance3") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Distance3")); continue; }
          if (dbrProp.PropertyName == "Distance4") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Distance4")); continue; }
          if (dbrProp.PropertyName == "Distance5") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Distance5")); continue; }
          if (dbrProp.PropertyName == "Flip state") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Flip state")); continue; }
          if (dbrProp.PropertyName == "Flip state1") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Flip state1")); continue; }
          if (dbrProp.PropertyName == "Try1") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Try1")); continue; }

          if (blockDeserialize.BlockSearch("Try") != null)
          {
            if (dbrProp.PropertyName == "Try")
            {
            
              dbrProp.Value = Convert.ToString(blockDeserialize.BlockSearch("Try")); continue;
            }
          }

          if (dbrProp.PropertyName == "Housing") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Housing")); continue; }
          if (dbrProp.PropertyName == "TTRY") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("TTRY")); continue; }
        }
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

    public double? DoubleConverter(object value)
    {
      if (value.GetType() != typeof(string))
      {
        double doubleValue = Convert.ToDouble(value);

        return doubleValue;
      }
      return null;
    }
  }
}

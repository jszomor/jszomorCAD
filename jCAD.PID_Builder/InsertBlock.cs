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
            SetVisibilityIndex(acBlkRef, blockDeserialize);
            SetDynamicReference(acBlkRef, blockDeserialize);
            //SetBlockRefenceAttributesValues(acBlkRef, insertBlock.ActionToExecuteOnAttRef);
            //SetDynamicBlockReferenceValues(acBlkRef, insertBlock.ActionToExecuteOnDynPropAfter);
            //SetHostName(acBlkRef, insertBlock.HostName);
          }
        }
        tr.Commit();
      }
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
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == blockName && !string.IsNullOrEmpty(blockName))
              blockIds.Add(btrId);
          }
        }
      }

      if (blockIds.Count > 1) throw new Exception($"More than one block record found with the name {blockName}");

      else if (blockIds.Count == 0) throw new Exception($"No block record found with the name {blockName}");

      else return blockIds.First();
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

    private void SetVisibilityIndex(BlockReference acBlkRef, BlockDeserialize blockDeserialize)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
          if (dbrProp.PropertyName == "Centrifugal Pump" && acBlkRef.Name == "pump")
            dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Centrifugal Pump"));

          else if (dbrProp.PropertyName == "Visibility" && acBlkRef.Name == "chamber")
            dbrProp.Value = Convert.ToString(blockDeserialize.BlockSearch("Visibility"));

          else if (dbrProp.PropertyName == "Block Table1")
            dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Block Table1"));
        }
      }
    }

    public void SetDynamicReference(BlockReference acBlkRef, BlockDeserialize blockDeserialize)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
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
          if (dbrProp.PropertyName == "Flip state") { dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Flip state")); continue; }
          if (dbrProp.PropertyName == "Flip state1") { dbrProp.Value = Convert.ToInt16(blockDeserialize.BlockSearch("Flip state1")); continue; }
          if (dbrProp.PropertyName == "Try1") { dbrProp.Value = DoubleConverter(blockDeserialize.BlockSearch("Try1")); continue; }
          if (dbrProp.PropertyName == "Try") { dbrProp.Value = Convert.ToString(blockDeserialize.BlockSearch("Try")); continue; }
          if (dbrProp.PropertyName == "Housing") { dbrProp.Value = Convert.ToString(blockDeserialize.BlockSearch("Housing")); continue; }
          if (dbrProp.PropertyName == "TTRY") { dbrProp.Value = Convert.ToString(blockDeserialize.BlockSearch("TTRY")); continue; }
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

    //public void SetupAttributeProperty(Transaction tr, BlockReference blockReference, BlockDeserialize blockDeserialize)
    //{
    //  AttributeCollection attCol = blockReference.AttributeCollection;
    //  foreach (ObjectId attId in attCol)
    //  {
    //    AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);

    //    if (attRef.Tag == "NOTE") {attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("NOTE")); continue; }
    //    if (attRef.Tag == "NOTE_CHINESE") { jsonBlockProperty.Attributes.NoteChinese = attRef.TextString; continue; }
    //    if (attRef.Tag == "NAME1") { jsonBlockProperty.Attributes.Name1 = attRef.TextString; continue; }
    //    if (attRef.Tag == "NAME2") { jsonBlockProperty.Attributes.Name2 = attRef.TextString; continue; }
    //    if (attRef.Tag == "TAG") { jsonBlockProperty.Attributes.Tag = attRef.TextString; continue; }
    //    if (attRef.Tag == "TAG_ID") { jsonBlockProperty.Attributes.TagId = attRef.TextString; continue; }
    //    if (attRef.Tag == "AREA_CODE") { jsonBlockProperty.Attributes.AreaCode = attRef.TextString; continue; }
    //    if (attRef.Tag == "MANUFACTURER") { jsonBlockProperty.Attributes.Manufacturer = attRef.TextString; continue; }
    //    if (attRef.Tag == "MODEL") { jsonBlockProperty.Attributes.Model = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_COVER") { jsonBlockProperty.Attributes.MaterialCover = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_BARS") { jsonBlockProperty.Attributes.MaterialBars = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_FIXED") { jsonBlockProperty.Attributes.MaterialFixed = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_FRAME") { jsonBlockProperty.Attributes.MaterialFrame = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_HOUSING") { jsonBlockProperty.Attributes.MaterialHousing = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL") { jsonBlockProperty.Attributes.Material = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_SCREW_LINER") { jsonBlockProperty.Attributes.MaterialScrewLiner = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_CARPENTRY") { jsonBlockProperty.Attributes.MaterialCarpentry = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_BODY") { jsonBlockProperty.Attributes.MaterialBody = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_GEAR") { jsonBlockProperty.Attributes.MaterialGear = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_SHAFT") { jsonBlockProperty.Attributes.MaterialShaft = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_ROTOR") { jsonBlockProperty.Attributes.MaterialRotor = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_SUBSURFACE") { jsonBlockProperty.Attributes.MaterialSubsurface = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_ABOVE_WATER") { jsonBlockProperty.Attributes.MaterialAboveWater = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_SEALING") { jsonBlockProperty.Attributes.MaterialSealing = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_STEM") { jsonBlockProperty.Attributes.MaterialStem = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_BLADE") { jsonBlockProperty.Attributes.MaterialBlade = attRef.TextString; continue; }
    //    if (attRef.Tag == "MATERIAL_ABOVE") { jsonBlockProperty.Attributes.MaterialAbove = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_FLOW") { jsonBlockProperty.Attributes.SpFlow = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_FLUID") { jsonBlockProperty.Attributes.SpFluid = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_SPACING") { jsonBlockProperty.Attributes.SpSpacing = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_ACTUATED") { jsonBlockProperty.Attributes.SpActuated = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_CAPACITY") { jsonBlockProperty.Attributes.SpCapacity = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_HEAD") { jsonBlockProperty.Attributes.SpHead = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_TSS_INLET") { jsonBlockProperty.Attributes.SpTssInlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_TSS_OUTLET") { jsonBlockProperty.Attributes.SpTssOutlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_DIAMETER") { jsonBlockProperty.Attributes.SpDiameter = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_VOLUME") { jsonBlockProperty.Attributes.SpVolume = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_PRESSURE") { jsonBlockProperty.Attributes.SpPressure = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_WIDTH") { jsonBlockProperty.Attributes.SpWidth = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_LEVEL") { jsonBlockProperty.Attributes.SpLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_BOARD") { jsonBlockProperty.Attributes.SpBoard = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_LENGTH") { jsonBlockProperty.Attributes.SpLength = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_INLET") { jsonBlockProperty.Attributes.SpInlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_OUTLET") { jsonBlockProperty.Attributes.SpOutlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_CHANNELH") { jsonBlockProperty.Attributes.SpChannelH = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_CHANNELW") { jsonBlockProperty.Attributes.SpChannelW = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_WATER_LEVEL") { jsonBlockProperty.Attributes.SpWaterLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_FLOWMAX") { jsonBlockProperty.Attributes.SpFlowMax = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_FLOWMIN") { jsonBlockProperty.Attributes.SpFlowMin = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_TANKW") { jsonBlockProperty.Attributes.SpTankW = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_TANKL") { jsonBlockProperty.Attributes.SpTankL = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_TANKD") { jsonBlockProperty.Attributes.SpTankD = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_TANKV") { jsonBlockProperty.Attributes.SpTankV = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_CLARIFIER_DIA") { jsonBlockProperty.Attributes.SpClarifierDia = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_OPERATION_LEVEL") { jsonBlockProperty.Attributes.SpOperationLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "SP_FREE_BOARD") { jsonBlockProperty.Attributes.SpFreeboard = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_SPACING") { jsonBlockProperty.Attributes.UnitSpacing = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_FLOW") { jsonBlockProperty.Attributes.UnitFlow = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_HEAD") { jsonBlockProperty.Attributes.UnitHead = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_CAPACITY") { jsonBlockProperty.Attributes.UnitCapacity = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_TSS_INLET") { jsonBlockProperty.Attributes.UnitTssInlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_TSS_OUTLET") { jsonBlockProperty.Attributes.UnitTssOutlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_DIAMETER") { jsonBlockProperty.Attributes.UnitDiameter = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_PRESSURE") { jsonBlockProperty.Attributes.UnitPressure = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_WIDTH") { jsonBlockProperty.Attributes.UnitWidth = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_LEVEL") { jsonBlockProperty.Attributes.UnitLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_BOARD") { jsonBlockProperty.Attributes.UnitBoard = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_LENGTH") { jsonBlockProperty.Attributes.UnitLength = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_INLET") { jsonBlockProperty.Attributes.UnitInlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_OUTLET") { jsonBlockProperty.Attributes.UnitOutlet = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_CHANNELW") { jsonBlockProperty.Attributes.UnitChannelW = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_CHANNELH") { jsonBlockProperty.Attributes.UnitChannelH = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_CLARIFIER") { jsonBlockProperty.Attributes.UnitClarifier = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_OPERATIONAL_LEVEL") { jsonBlockProperty.Attributes.UnitOperationalLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_FREE_BOARD") { jsonBlockProperty.Attributes.UnitFreeboard = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_WATER_LEVEL") { jsonBlockProperty.Attributes.UnitWaterLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_FLOWMAX") { jsonBlockProperty.Attributes.UnitFlowMax = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_FLOWMIN") { jsonBlockProperty.Attributes.UnitFlowMin = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_CLARIFIER_DIA") { jsonBlockProperty.Attributes.UnitClarifierDia = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_VOLUME") { jsonBlockProperty.Attributes.UnitVolume = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_TANKD") { jsonBlockProperty.Attributes.UnitTankD = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_TANKL") { jsonBlockProperty.Attributes.UnitTankL = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_TANW") { jsonBlockProperty.Attributes.UnitTankW = attRef.TextString; continue; }
    //    if (attRef.Tag == "UNIT_TANKV") { jsonBlockProperty.Attributes.UnitTankV = attRef.TextString; continue; }
    //    if (attRef.Tag == "RUNNINGHOURS") { jsonBlockProperty.Attributes.RunningHours = attRef.TextString; continue; }
    //    if (attRef.Tag == "EQUIP_TYPE") { jsonBlockProperty.Attributes.EquipType = attRef.TextString; continue; }
    //    if (attRef.Tag == "BLOWER_TYPE") { jsonBlockProperty.Attributes.BlowerType = attRef.TextString; continue; }
    //    if (attRef.Tag == "STB_DTY") { jsonBlockProperty.Attributes.StandByDuty = attRef.TextString; continue; }
    //    if (attRef.Tag == "DI") { jsonBlockProperty.Attributes.DI = attRef.TextString; continue; }
    //    if (attRef.Tag == "DO") { jsonBlockProperty.Attributes.DO = attRef.TextString; continue; }
    //    if (attRef.Tag == "AI") { jsonBlockProperty.Attributes.AI = attRef.TextString; continue; }
    //    if (attRef.Tag == "AO") { jsonBlockProperty.Attributes.AO = attRef.TextString; continue; }
    //    if (attRef.Tag == "PB") { jsonBlockProperty.Attributes.PB = attRef.TextString; continue; }
    //    if (attRef.Tag == "PO") { jsonBlockProperty.Attributes.PO = attRef.TextString; continue; }
    //    if (attRef.Tag == "PROCESSUNITAREA") { jsonBlockProperty.Attributes.ProcessUnitArea = attRef.TextString; continue; }
    //    if (attRef.Tag == "VOLUME") { jsonBlockProperty.Attributes.Volume = attRef.TextString; continue; }
    //    if (attRef.Tag == "LIQUIDLEVEL") { jsonBlockProperty.Attributes.LiquidLevel = attRef.TextString; continue; }
    //    if (attRef.Tag == "LENGTH") { jsonBlockProperty.Attributes.Length = attRef.TextString; continue; }
    //    if (attRef.Tag == "HEIGHT") { jsonBlockProperty.Attributes.Height = attRef.TextString; continue; }
    //    if (attRef.Tag == "WIDTH") { jsonBlockProperty.Attributes.Width = attRef.TextString; continue; }
    //    if (attRef.Tag == "CHANNELWIDTH") { jsonBlockProperty.Attributes.ChannelWidth = attRef.TextString; continue; }
    //    if (attRef.Tag == "PRESSURE") { jsonBlockProperty.Attributes.Pressure = attRef.TextString; continue; }
    //    if (attRef.Tag == "SIZE") { jsonBlockProperty.Attributes.Size = attRef.TextString; continue; }
    //    if (attRef.Tag == "INSTALLATION") { jsonBlockProperty.Attributes.Installation = attRef.TextString; continue; }
    //    if (attRef.Tag == "FC_MOD") { jsonBlockProperty.Attributes.FcMod = attRef.TextString; continue; }
    //    if (attRef.Tag == "FC_MAN") { jsonBlockProperty.Attributes.FcMan = attRef.TextString; continue; }
    //    if (attRef.Tag == "PUMP_TYPE") { jsonBlockProperty.Attributes.PumpType = attRef.TextString; continue; }
    //    if (attRef.Tag == "FILTER") { jsonBlockProperty.Attributes.Filter = attRef.TextString; continue; }
    //    if (attRef.Tag == "FLOW") { jsonBlockProperty.Attributes.Flow = attRef.TextString; continue; }
    //    if (attRef.Tag == "DIAMETER") { jsonBlockProperty.Attributes.Diameter = attRef.TextString; continue; }
    //    if (attRef.Tag == "POWER") { jsonBlockProperty.Attributes.Power = attRef.TextString; continue; }
    //    if (attRef.Tag == "INSTALLED_POWER") { jsonBlockProperty.Attributes.InstalledPower = attRef.TextString; continue; }
    //    if (attRef.Tag == "POWER_INSTALLED") { jsonBlockProperty.Attributes.PowerInstalled = attRef.TextString; continue; }
    //    if (attRef.Tag == "CONSUMED_POWER") { jsonBlockProperty.Attributes.ConsumedPower = attRef.TextString; continue; }
    //    if (attRef.Tag == "POWER_CONSUMED") { jsonBlockProperty.Attributes.PowerConsumed = attRef.TextString; continue; }
    //    if (attRef.Tag == "NUMBER_OF_UNIT") { jsonBlockProperty.Attributes.NumberOfUnits = attRef.TextString; continue; }
    //    if (attRef.Tag == "TYP") { jsonBlockProperty.Attributes.Typ = attRef.TextString; continue; }
    //    if (attRef.Tag == "HOST_NAME") { jsonBlockProperty.Attributes.HostName = attRef.TextString; continue; }
    //  }

    //  return jsonBlockProperty;
    //}
  }
}

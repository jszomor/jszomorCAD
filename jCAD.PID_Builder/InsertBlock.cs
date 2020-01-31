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
using Autodesk.AutoCAD.ApplicationServices;

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

    public void PlaceBlocksByName(JsonPID jsonPID, string blockName)
    {
      var blocks = jsonPID.BlocksSearch(blockName).ToList();
      if (blocks == null || blocks.Count == 0) throw new ArgumentNullException("Block not found " + blockName);
      foreach (var block in blocks)
      {
        PlaceOneBlock(block);
      }
    }

    private void PlaceOneBlock(JsonBlockProperty block)
    {
      ObjectId blockId = GetBlockTable(block.Misc.BlockName);

      //var defultLayers = new LayerCreator();
      using (var tr = _db.TransactionManager.StartTransaction())
      {
        var btr = tr.GetObject(_db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        using (var blockDefinition = tr.GetObject(blockId, OpenMode.ForWrite, false) as BlockTableRecord)
        {
          using (var acBlkRef = new BlockReference( new Point3d(block.Geometry.X, block.Geometry.Y, 0), blockId))
          {
            btr.AppendEntity(acBlkRef);

            CreateNewAttributeDefinition(blockDefinition, tr);
            //using (AttributeDefinition acAttDef = new AttributeDefinition())
            //{
            //  acAttDef.Position = new Point3d(0, 0, 0);
            //  acAttDef.Verifiable = true;
            //  acAttDef.Prompt = "OrderId: ";
            //  acAttDef.Tag = "OrderId";
            //  acAttDef.TextString = "OrderId";
            //  acAttDef.Height = 1;
            //  acAttDef.Invisible = true;
            //  acAttDef.Justify = AttachmentPoint.MiddleCenter;
            //  blockDefinition.AppendEntity(acAttDef);
            //  tr.AddNewlyCreatedDBObject(acAttDef, true);
            //}

            tr.AddNewlyCreatedDBObject(acBlkRef, true);

            SetBlockReferenceLayer(acBlkRef, block.General.Layer);
            SetRotate(acBlkRef, block.Misc.Rotation);
            CreateBlockRefenceAttributes(acBlkRef, blockDefinition, tr);
            SetVisibilityIndex(acBlkRef, block);
            SetDynamicReference(acBlkRef, block);
            SetupAttributeProperty(tr, acBlkRef, block);
            //SetBlockRefenceAttributesValues(acBlkRef, insertBlock.ActionToExecuteOnAttRef);
            //SetDynamicBlockReferenceValues(acBlkRef, insertBlock.ActionToExecuteOnDynPropAfter);
            //SetHostName(acBlkRef, insertBlock.HostName);
          }
        }
        tr.Commit();
      }
    }

    private void CreateNewAttributeDefinition(BlockTableRecord blockDefinition, Transaction tr)
    {
      using (AttributeDefinition acAttDef = new AttributeDefinition())
      {
        if(acAttDef.Tag != "OrderId")
        {
          var position = new Point3d(-20, 10, 0);
          acAttDef.Verifiable = true;
          acAttDef.Prompt = "OrderId";
          acAttDef.Tag = "OrderId";
          acAttDef.Height = 2;
          acAttDef.Invisible = true;
          acAttDef.Justify = AttachmentPoint.MiddleCenter;
          acAttDef.AlignmentPoint = position;
          blockDefinition.AppendEntity(acAttDef);
          tr.AddNewlyCreatedDBObject(acAttDef, true);
        }
      }
    }

    public String RealNameFinder(string originalName)
    {
      if (originalName == null) return null;
      var result = originalName;

      if (originalName.Contains("$") && originalName.StartsWith("RefA"))
      {
        result = originalName.Substring(0, originalName.LastIndexOf('$') - 2);
      }

      else if (originalName.Contains("$"))
      {
        result = originalName.Substring(originalName.LastIndexOf('$') + 1);
      }

      else if (originalName == "refvalve" || originalName == "ref_valve")
      {
        result = "valve";
      }

      return result;
    }

    public ObjectId GetBlockTable(string blockName)
    {
      string validBlockName = RealNameFinder(blockName);

      var blockIds = new List<ObjectId>();

      using (var tr = _db.TransactionManager.StartTransaction())
      {
        BlockTable bt = _db.BlockTableId.GetObject<BlockTable>(OpenMode.ForRead);

        foreach (var btrId in bt)
        {
          using (var btr = tr.GetObject(btrId, OpenMode.ForRead, false) as BlockTableRecord)
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && validBlockName == btr.Name && !string.IsNullOrEmpty(validBlockName))
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
      string actualLayer = RealNameFinder(layerName);

      try
      {
        acBlkRef.Layer = actualLayer;
      }
      catch (Autodesk.AutoCAD.Runtime.Exception ex)
      {
        if (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.KeyNotFound) throw new Exception($"Layer name not found: {actualLayer}");

        else throw;
      }
    }

    private void SetVisibilityIndex(BlockReference acBlkRef, JsonBlockProperty jsonBlockProperty)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
          if (dbrProp.PropertyName == "Centrifugal Pump" && acBlkRef.Name == "pump")
          {
            dbrProp.Value = Convert.ToInt16(jsonBlockProperty.Custom.PumpTableValue);
            break;
          }

          else if (dbrProp.PropertyName == "Visibility" && acBlkRef.Name != "blower" && acBlkRef.Name != "filter")
          {
            if(acBlkRef.Name == "sst2_chmbr")
            {
              string dbValue = Convert.ToString(dbrProp.Value);
              dbValue = Convert.ToString(jsonBlockProperty.Custom.VisibilityValue);
            }
            else
            dbrProp.Value = jsonBlockProperty.Custom.VisibilityValue;

            break;
          }

          else if (dbrProp.PropertyName == "Block Table1")
          {
            dbrProp.Value = Convert.ToInt16(jsonBlockProperty.Custom.BlockTableValue);
            break;
          }
        }
      }
    }

    public void SetDynamicReference(BlockReference acBlkRef, JsonBlockProperty jsonBlockProperty)
    {
      if (acBlkRef.IsDynamicBlock)
      {
        foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
        {
          if(dbrProp.PropertyName != "Centrifugal Pump" && dbrProp.PropertyName != "Visibility" && dbrProp.PropertyName != "Block Table1")
          {
            GetDynamicValue(dbrProp, jsonBlockProperty);
          }
          //try
          //{
          //  if (jsonBlockProperty.Custom != null)
          //  {
          //    if (dbrProp.PropertyName == "Position X") { dbrProp.Value = jsonBlockProperty.Custom.TagX; continue; }
          //    if (dbrProp.PropertyName == "Position Y") { dbrProp.Value = jsonBlockProperty.Custom.TagY; continue; }
          //    if (dbrProp.PropertyName == "Position1 X") { dbrProp.Value = jsonBlockProperty.Custom.TagX1; continue; }
          //    if (dbrProp.PropertyName == "Position1 Y") { dbrProp.Value = jsonBlockProperty.Custom.TagY1; continue; }
          //    if (dbrProp.PropertyName == "Angle") { dbrProp.Value = jsonBlockProperty.Custom.Angle; continue; }
          //    if (dbrProp.PropertyName == "Angle1") { dbrProp.Value = jsonBlockProperty.Custom.Angle1; continue; }
          //    if (dbrProp.PropertyName == "Angle2") { dbrProp.Value = jsonBlockProperty.Custom.Angle2; continue; }
          //    if (dbrProp.PropertyName == "Distance") { dbrProp.Value = jsonBlockProperty.Custom.Distance; continue; }
          //    if (dbrProp.PropertyName == "Distance1") { dbrProp.Value = jsonBlockProperty.Custom.Distance1; continue; }
          //    if (dbrProp.PropertyName == "Distance2") { dbrProp.Value = jsonBlockProperty.Custom.Distance2; continue; }
          //    if (dbrProp.PropertyName == "Distance3") { dbrProp.Value = jsonBlockProperty.Custom.Distance3; continue; }
          //    if (dbrProp.PropertyName == "Distance4") { dbrProp.Value = jsonBlockProperty.Custom.Distance4; continue; }
          //    if (dbrProp.PropertyName == "Distance5") { dbrProp.Value = jsonBlockProperty.Custom.Distance5; continue; }
          //    if (dbrProp.PropertyName == "Flip state") { dbrProp.Value = jsonBlockProperty.Custom.FlipState; continue; }
          //    if (dbrProp.PropertyName == "Flip state1") { dbrProp.Value = jsonBlockProperty.Custom.FlipState1; continue; }
          //    if (dbrProp.PropertyName == "Try1") { dbrProp.Value = jsonBlockProperty.Custom.Try1; continue; }
          //    if (dbrProp.PropertyName == "Try") { dbrProp.Value = jsonBlockProperty.Custom.Try; continue; }
          //    if (dbrProp.PropertyName == "Housing") { dbrProp.Value = jsonBlockProperty.Custom.Housing; continue; }
          //    if (dbrProp.PropertyName == "TTRY") { dbrProp.Value = jsonBlockProperty.Custom.TTRY; continue; }
          //  }
          //}
          //catch (NullReferenceException)
          //{
          //  //continue;
          //  throw new NullReferenceException($"selected property: {dbrProp.PropertyName} got null value");
          //}
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

    public void SetupAttributeProperty(Transaction tr, BlockReference blockReference, JsonBlockProperty jsonBlockProperty)
    {
      AttributeCollection attCol = blockReference.AttributeCollection;
      foreach (ObjectId attId in attCol)
      {
        using (AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead))
        {
          if(attRef.TextString != "OrderId")
          {
            GetRefTextString(attRef, jsonBlockProperty);
          }
        }

        #region
        //if (attRef.Tag == "NOTE") { attRef.TextString = jsonBlockProperty.Attributes.Note; continue; }
        //if (attRef.Tag == "NOTE_CHINESE") { attRef.TextString = jsonBlockProperty.Attributes.NoteChinese; continue; }
        //if (attRef.Tag == "Name") { attRef.TextString = jsonBlockProperty.Attributes.Name; continue; }
        //if (attRef.Tag == "NAME1") { attRef.TextString = jsonBlockProperty.Attributes.Name1; continue; }
        ////if (attRef.Tag == "NAME2") { attRef.TextString = jsonBlockProperty.Attributes.Name2 == null ? "" : jsonBlockProperty.Attributes.Name2; continue; }
        ////if (attRef.Tag == "NAME2") { SetRefTextString(attRef, jsonBlockProperty, x => x.Name2); continue; }
        //if (attRef.Tag == "NAME2") { SetRefTextString(attRef, jsonBlockProperty); continue; }
        //if (attRef.Tag == "TAG") { attRef.TextString = jsonBlockProperty.Attributes.Tag; continue; }
        //    if (attRef.Tag == "TAG_ID")               { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("TAG_ID")); continue; }
        //    if (attRef.Tag == "AREA_CODE")            { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("AREA_CODE")); continue; }
        //    if (attRef.Tag == "MANUFACTURER")         { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MANUFACTURER")); continue; }
        //    if (attRef.Tag == "MODEL")                { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MODEL")); continue; }
        //    if (attRef.Tag == "MATERIAL_COVER")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_COVER")); continue; }
        //    if (attRef.Tag == "MATERIAL_BARS")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_BARS")); continue; }
        //    if (attRef.Tag == "MATERIAL_FIXED")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_FIXED")); continue; }
        //    if (attRef.Tag == "MATERIAL_FRAME")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_FRAME")); continue; }
        //    if (attRef.Tag == "MATERIAL_HOUSING")     { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_HOUSING")); continue; }
        //    if (attRef.Tag == "MATERIAL")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL")); continue; }
        //    if (attRef.Tag == "MATERIAL_SCREW_LINER") { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_SCREW_LINER")); continue; }
        //    if (attRef.Tag == "MATERIAL_CARPENTRY")   { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_CARPENTRY")); continue; }
        //    if (attRef.Tag == "MATERIAL_BODY")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_BODY")); continue; }
        //    if (attRef.Tag == "MATERIAL_GEAR")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_GEAR")); continue; }
        //    if (attRef.Tag == "MATERIAL_SHAFT")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_SHAFT")); continue; }
        //    if (attRef.Tag == "MATERIAL_ROTOR")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_ROTOR")); continue; }
        //    if (attRef.Tag == "MATERIAL_SUBSURFACE")  { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_SUBSURFACE")); continue; }
        //    if (attRef.Tag == "MATERIAL_ABOVE_WATER") { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_ABOVE_WATER")); continue; }
        //    if (attRef.Tag == "MATERIAL_SEALING")     { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_SEALING")); continue; }
        //    if (attRef.Tag == "MATERIAL_STEM")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_STEM")); continue; }
        //    if (attRef.Tag == "MATERIAL_BLADE")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_BLADE")); continue; }
        //    if (attRef.Tag == "MATERIAL_ABOVE")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("MATERIAL_ABOVE")); continue; }
        //    if (attRef.Tag == "SP_FLOW")              { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_FLOW")); continue; }
        //    if (attRef.Tag == "SP_FLUID")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_FLUID")); continue; }
        //    if (attRef.Tag == "SP_SPACING")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_SPACING")); continue; }
        //    if (attRef.Tag == "SP_ACTUATED")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_ACTUATED")); continue; }
        //    if (attRef.Tag == "SP_CAPACITY")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_CAPACITY")); continue; }
        //    if (attRef.Tag == "SP_HEAD")              { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_HEAD")); continue; }
        //    if (attRef.Tag == "SP_TSS_INLET")         { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_TSS_INLET")); continue; }
        //    if (attRef.Tag == "SP_TSS_OUTLET")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_TSS_OUTLET")); continue; }
        //    if (attRef.Tag == "SP_DIAMETER")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_DIAMETER")); continue; }
        //    if (attRef.Tag == "SP_VOLUME")            { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_VOLUME")); continue; }
        //    if (attRef.Tag == "SP_PRESSURE")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_PRESSURE")); continue; }
        //    if (attRef.Tag == "SP_WIDTH")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_WIDTH")); continue; }
        //    if (attRef.Tag == "SP_LEVEL")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_LEVEL")); continue; }
        //    if (attRef.Tag == "SP_BOARD")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_BOARD")); continue; }
        //    if (attRef.Tag == "SP_LENGTH")            { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_LENGTH")); continue; }
        //    if (attRef.Tag == "SP_INLET")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_INLET")); continue; }
        //    if (attRef.Tag == "SP_OUTLET")            { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_OUTLET")); continue; }
        //    if (attRef.Tag == "SP_CHANNELH")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_CHANNELH")); continue; }
        //    if (attRef.Tag == "SP_CHANNELW")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_CHANNELW")); continue; }
        //    if (attRef.Tag == "SP_WATER_LEVEL")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_WATER_LEVEL")); continue; }
        //    if (attRef.Tag == "SP_FLOWMAX")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_FLOWMAX")); continue; }
        //    if (attRef.Tag == "SP_FLOWMIN")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_FLOWMIN")); continue; }
        //    if (attRef.Tag == "SP_TANKW")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_TANKW")); continue; }
        //    if (attRef.Tag == "SP_TANKL")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_TANKL")); continue; }
        //    if (attRef.Tag == "SP_TANKD")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_TANKD")); continue; }
        //    if (attRef.Tag == "SP_TANKV")             { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_TANKV")); continue; }
        //    if (attRef.Tag == "SP_CLARIFIER_DIA")     { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_CLARIFIER_DIA")); continue; }
        //    if (attRef.Tag == "SP_OPERATION_LEVEL")   { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_OPERATION_LEVEL")); continue; }
        //    if (attRef.Tag == "SP_FREE_BOARD")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("SP_FREE_BOARD")); continue; }
        //    if (attRef.Tag == "UNIT_SPACING")         { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_SPACING")); continue; }
        //    if (attRef.Tag == "UNIT_FLOW")            { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_FLOW")); continue; }
        //    if (attRef.Tag == "UNIT_HEAD")            { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_HEAD")); continue; }
        //    if (attRef.Tag == "UNIT_CAPACITY")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_CAPACITY")); continue; }
        //    if (attRef.Tag == "UNIT_TSS_INLET")       { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_TSS_INLET")); continue; }
        //    if (attRef.Tag == "UNIT_TSS_OUTLET")      { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_TSS_OUTLET")); continue; }
        //    if (attRef.Tag == "UNIT_DIAMETER")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_DIAMETER")); continue; }
        //    if (attRef.Tag == "UNIT_PRESSURE")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_PRESSURE")); continue; }
        //    if (attRef.Tag == "UNIT_WIDTH")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_WIDTH")); continue; }
        //    if (attRef.Tag == "UNIT_LEVEL")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_LEVEL")); continue; }
        //    if (attRef.Tag == "UNIT_BOARD")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_BOARD")); continue; }
        //    if (attRef.Tag == "UNIT_LENGTH")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_LENGTH")); continue; }
        //    if (attRef.Tag == "UNIT_INLET")           { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_INLET")); continue; }
        //    if (attRef.Tag == "UNIT_OUTLET")          { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_OUTLET")); continue; }
        //    if (attRef.Tag == "UNIT_CHANNELW")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_CHANNELW")); continue; }
        //    if (attRef.Tag == "UNIT_CHANNELH")        { attRef.TextString = Convert.ToString(blockDeserialize.BlockSearch("UNIT_CHANNELH")); continue; }
        //    //if (attRef.Tag == "UNIT_CLARIFIER") { jsonBlockProperty.Attributes.UnitClarifier = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_OPERATIONAL_LEVEL") { jsonBlockProperty.Attributes.UnitOperationalLevel = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_FREE_BOARD") { jsonBlockProperty.Attributes.UnitFreeboard = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_WATER_LEVEL") { jsonBlockProperty.Attributes.UnitWaterLevel = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_FLOWMAX") { jsonBlockProperty.Attributes.UnitFlowMax = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_FLOWMIN") { jsonBlockProperty.Attributes.UnitFlowMin = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_CLARIFIER_DIA") { jsonBlockProperty.Attributes.UnitClarifierDia = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_VOLUME") { jsonBlockProperty.Attributes.UnitVolume = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_TANKD") { jsonBlockProperty.Attributes.UnitTankD = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_TANKL") { jsonBlockProperty.Attributes.UnitTankL = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_TANW") { jsonBlockProperty.Attributes.UnitTankW = attRef.TextString; continue; }
        //    //if (attRef.Tag == "UNIT_TANKV") { jsonBlockProperty.Attributes.UnitTankV = attRef.TextString; continue; }
        //    //if (attRef.Tag == "RUNNINGHOURS") { jsonBlockProperty.Attributes.RunningHours = attRef.TextString; continue; }
        //    //if (attRef.Tag == "EQUIP_TYPE") { jsonBlockProperty.Attributes.EquipType = attRef.TextString; continue; }
        //    //if (attRef.Tag == "BLOWER_TYPE") { jsonBlockProperty.Attributes.BlowerType = attRef.TextString; continue; }
        //    //if (attRef.Tag == "STB_DTY") { jsonBlockProperty.Attributes.StandByDuty = attRef.TextString; continue; }
        //    //if (attRef.Tag == "DI") { jsonBlockProperty.Attributes.DI = attRef.TextString; continue; }
        //    //if (attRef.Tag == "DO") { jsonBlockProperty.Attributes.DO = attRef.TextString; continue; }
        //    //if (attRef.Tag == "AI") { jsonBlockProperty.Attributes.AI = attRef.TextString; continue; }
        //    //if (attRef.Tag == "AO") { jsonBlockProperty.Attributes.AO = attRef.TextString; continue; }
        //    //if (attRef.Tag == "PB") { jsonBlockProperty.Attributes.PB = attRef.TextString; continue; }
        //    //if (attRef.Tag == "PO") { jsonBlockProperty.Attributes.PO = attRef.TextString; continue; }
        //    //if (attRef.Tag == "PROCESSUNITAREA") { jsonBlockProperty.Attributes.ProcessUnitArea = attRef.TextString; continue; }
        //    //if (attRef.Tag == "VOLUME") { jsonBlockProperty.Attributes.Volume = attRef.TextString; continue; }
        //    //if (attRef.Tag == "LIQUIDLEVEL") { jsonBlockProperty.Attributes.LiquidLevel = attRef.TextString; continue; }
        //    //if (attRef.Tag == "LENGTH") { jsonBlockProperty.Attributes.Length = attRef.TextString; continue; }
        //    //if (attRef.Tag == "HEIGHT") { jsonBlockProperty.Attributes.Height = attRef.TextString; continue; }
        //    //if (attRef.Tag == "WIDTH") { jsonBlockProperty.Attributes.Width = attRef.TextString; continue; }
        //    //if (attRef.Tag == "CHANNELWIDTH") { jsonBlockProperty.Attributes.ChannelWidth = attRef.TextString; continue; }
        //    //if (attRef.Tag == "PRESSURE") { jsonBlockProperty.Attributes.Pressure = attRef.TextString; continue; }
        //    //if (attRef.Tag == "SIZE") { jsonBlockProperty.Attributes.Size = attRef.TextString; continue; }
        //    //if (attRef.Tag == "INSTALLATION") { jsonBlockProperty.Attributes.Installation = attRef.TextString; continue; }
        //    //if (attRef.Tag == "FC_MOD") { jsonBlockProperty.Attributes.FcMod = attRef.TextString; continue; }
        //    //if (attRef.Tag == "FC_MAN") { jsonBlockProperty.Attributes.FcMan = attRef.TextString; continue; }
        //    //if (attRef.Tag == "PUMP_TYPE") { jsonBlockProperty.Attributes.PumpType = attRef.TextString; continue; }
        //    //if (attRef.Tag == "FILTER") { jsonBlockProperty.Attributes.Filter = attRef.TextString; continue; }
        //    //if (attRef.Tag == "FLOW") { jsonBlockProperty.Attributes.Flow = attRef.TextString; continue; }
        //    //if (attRef.Tag == "DIAMETER") { jsonBlockProperty.Attributes.Diameter = attRef.TextString; continue; }
        //    //if (attRef.Tag == "POWER") { jsonBlockProperty.Attributes.Power = attRef.TextString; continue; }
        //    //if (attRef.Tag == "INSTALLED_POWER") { jsonBlockProperty.Attributes.InstalledPower = attRef.TextString; continue; }
        //    //if (attRef.Tag == "POWER_INSTALLED") { jsonBlockProperty.Attributes.PowerInstalled = attRef.TextString; continue; }
        //    //if (attRef.Tag == "CONSUMED_POWER") { jsonBlockProperty.Attributes.ConsumedPower = attRef.TextString; continue; }
        //    //if (attRef.Tag == "POWER_CONSUMED") { jsonBlockProperty.Attributes.PowerConsumed = attRef.TextString; continue; }
        //    //if (attRef.Tag == "NUMBER_OF_UNIT") { jsonBlockProperty.Attributes.NumberOfUnits = attRef.TextString; continue; }
        //    //if (attRef.Tag == "TYP") { jsonBlockProperty.Attributes.Typ = attRef.TextString; continue; }
        //    //if (attRef.Tag == "HOST_NAME") { jsonBlockProperty.Attributes.HostName = attRef.TextString; continue; }
        #endregion

      }
    }

    //private void SetRefTextString(AttributeReference attRef, JsonBlockProperty block, Expression<Func<Attributes, string>> expression)
    //{
    //  var c = expression.Compile();
    //  var f = c.Invoke(block.Attributes);

    //  if (!string.IsNullOrWhiteSpace(f)) attRef.TextString = f;
    //}

    private void GetRefTextString(AttributeReference attRef, JsonBlockProperty block)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties = block.Attributes.GetType().GetProperties();
      foreach (var prop in properties)
      {
        var customAttributes = prop
          .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (attRef.Tag == jsonTagName)
          {
            var propValue = prop.GetValue(block.Attributes);
            if (propValue == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue.ToString()))
            {
              attRef.TextString = propValue.ToString();
              break;
            }

            //prop.SetValue(block.Attributes, attRef.TextString); //serialization
          }
        }
      }
    }

    private void GetDynamicValue(DynamicBlockReferenceProperty dbrProp, JsonBlockProperty block)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties = block.Custom.GetType().GetProperties();
      foreach (var prop in properties)
      {
        var customAttributes = prop
          .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (dbrProp.PropertyName == jsonTagName)
          {
            var propValue = prop.GetValue(block.Custom);
            if (propValue == null) continue;
            
              dbrProp.Value = propValue;
              break;
            

            //prop.SetValue(block.Attributes, attRef.TextString); //serialization
          }
        }
      }
    }
  }
}

using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Linq;
using System.Collections.Generic;
using EquipmentPosition;
using System.Runtime.InteropServices;
using OrganiCAD.AutoCAD;
using JsonFindKey;
using jCAD.PID_Builder;
using JsonParse;

namespace jszomorCAD
{
  public class CommandMethods
  {
    public string path = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\Autocad PID blocks work in progress.dwg";

    [CommandMethod("jcad_EqTankBuilder")]
    public void ListBlocks()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;

      // Copy blocks from sourcefile into opened file
      var copyBlockTable = new CopyBlock();
      var btrNamesToCopy = new[] { "pump", "valve", "chamber", "instrumentation tag", "channel gate", "pipe", "pipe2", "channel", "channel2" };
      copyBlockTable.CopyBlockTable(db, path, btr =>
      {
        System.Diagnostics.Debug.Print(btr.Name);
        return btrNamesToCopy.Contains(btr.Name);
      });


      //var sizeProperty = new PositionProperty();
      //"\nEnter number of equipment:"
      //var pio = new PromptIntegerOptions("\nEnter number of equipment:") { DefaultValue = 5 };
      //var number = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(pio);
      //var intNumber = Convert.ToInt32(number.Value);

      //"\nEnter distance of equipment:"
      //var dio = new PromptIntegerOptions("\nEnter distance of equipment:") { DefaultValue = 20 };
      //var distance = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(dio);
      //var intDistance = Convert.ToInt32(distance.Value);

      //"\nEnter index of equipment:"
      //var eio = new PromptIntegerOptions("\nEnter index of equipment:") { DefaultValue = 22 };
      //var eqIndex = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(eio);
      //var promptEqIndex = Convert.ToInt16(eqIndex.Value);

      var selectorProperty = new SelectorProperty();
      long numbers = JsonProcessClass.JsonProcessValue("number_of_eqPump");
      long indexOfEqPump = JsonClass.JsonEquipmentValue("Equalization Tank Pump");
      var promptEqIndex = Convert.ToInt16(indexOfEqPump);

      var eqt = new EqualizationTank(numberOfPumps: Convert.ToInt32(EquipmentSelector.EqPumpNumberSelect(selectorProperty)), distanceOfPump: 20, eqIndex: promptEqIndex);

      //var InsBlock = new BlockMapping();
      #region old code
      //var blocks = new[]
      //{
      //  new InsertBlockBase(numberOfItem: 1,
      //    blockName: "chamber",
      //    layerName: "unit",
      //    x: 0,
      //    y: 0,
      //    hostName: "Equalization Tank")
      //  {
      //    ActionToExecuteOnDynProp = new Action<DynamicBlockReferenceProperty>[] 
      //    {
      //      dbrProp => 
      //      {
      //        if (dbrProp.PropertyName == "Visibility")
      //          dbrProp.Value = "no channel";
      //      }
      //    },
      //    ActionToExecuteOnAttRef = new Action<AttributeReference>[]
      //    {
      //      ar =>
      //      {
      //        //text for EQ tank - Attributes
      //        if (ar.Tag == "NAME1")
      //          ar.TextString = "EQUALIZATION";
      //        if (ar.Tag == "NAME2")
      //          ar.TextString = "TANK";
      //      }
      //    },
      //    ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
      //    {
      //      dbrProp =>
      //      {
      //        //setup chamber width
      //        if (dbrProp.PropertyName == "Distance")
      //          dbrProp.Value = 5.0d * 20 + 50; //last value is the free space for other items
      //        //text position for chamber
      //        if (dbrProp.PropertyName == "Position X")
      //          dbrProp.Value = (5 * 20 + 50) / 2.0d; //place text middle of chamber horizontaly 
      //      }
      //    },
      //  },
      //};
      #endregion

      var layers = new[]
      {
        new LayerData("unit", Color.FromRgb(255, 0, 0), 0.25, false)
      };

      var layerCreator = new LayerCreator();
      layerCreator.LayerCreatorMethod(layers);

      DrawBlocks(db, eqt.Blocks);
      //DrawBlocks(db, InsBlock.Blocks);

      #region oldcode
      ////short shortCheckValveIndex = 2;
      //PositionProperty.NumberOfPump = number.Value;
      //PositionProperty.DistanceOfPump = distance.Value;

      //var ed = Application.DocumentManager.MdiActiveDocument.Editor;
      //var aw = new AutoCadWrapper();


      ////copyBlockTable.CopyBlockTableMethod(db, path, btr => true);
      //sizeProperty.X = 50;

      //// Call a transaction to create layer

      ////layerCreator.LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.5);

      ////layerCreator.LayerCreatorMethod("unit", Color.FromRgb(0, 0, 255), 0.5);

      //// Start transaction to write equipment

      //var insertEqTAnkPump = new InsertBlockBase(PositionProperty.NumberOfPump,               // number of item
      //  PositionProperty.DistanceOfPump,             // disctance of item
      //  "pump",                                      //block name
      //  "equipment",                                 //layer name
      //  "Centrifugal Pump",                          //dynamic property type
      //  shortEqIndex,                                //visibility of equipment ()
      //  sizeProperty.X,                              //X
      //  10,                                          //Y
      //  "Equalization Tank",                         //Host name       
      //  0);                                          //pipe length
      //insertBlockTable.InsertBlockTableMethod(db, insertEqTAnkPump);
      ////
      //var insertEqTAnk = new InsertBlockBase(
      //  1,                                          // number of item
      //  0,                                          // disctance of item
      //  "chamber",                                  //block name
      //  "unit",                                     //layer name
      //  "Visibility",                               //dynamic property type
      //  "no channel",                               //visibility of equipment
      //  0,                                          //X
      //  0,                                          //Y
      //  "Equalization Tank",                        //Host name       
      //  0);                                         //pipe length
      //insertBlockTable.InsertBlockTableMethod(db, insertEqTAnk);
      ////
      //var insertCheckValve = new InsertBlockBase(
      //  PositionProperty.NumberOfPump,              // number of item
      //  PositionProperty.DistanceOfPump,            // disctance of item
      //  "valve",                                    //block name
      //  "valve",                                    //layer name
      //  "Block Table1",                             //dynamic property type
      //  (short)5,                                          //visibility of equipment (check valve)
      //  sizeProperty.X,                             //X
      //  25,                                         //Y
      //  "Equalization Tank",                        //Host name       
      //  0);                                         //pipe length
      //insertBlockTable.InsertBlockTableMethod(db, insertCheckValve);
      ////
      //var insertGateValve = new InsertBlockBase(
      //  PositionProperty.NumberOfPump,            // number of item
      //  PositionProperty.DistanceOfPump,          // disctance of item
      //  "valve",                                  //block name
      //  "valve",                                  //layer name
      //  "Block Table1",                           //dynamic property type
      //  (short)0,                                        //visibility of equipment (gate valve)
      //  sizeProperty.X,                           //X
      //  40,                                       //Y
      //  "Equalization Tank",                      //Host name       
      //  0);                                       //pipe length
      //insertBlockTable.InsertBlockTableMethod(db, insertGateValve);
      ////
      //var insertLIT = new InsertBlockBase(
      //  1,                                        // number of item
      //  0,                                        // disctance of item
      //  "instrumentation tag",                    //block name
      //  "instrumentation",                        //layer name
      //  "Block Table1",                           //dynamic property type
      //  (short)7,                                        //visibility of equipment (LIT)
      //  PositionProperty.NumberOfPump
      //  * PositionProperty.DistanceOfPump + 50,   //X
      //  10,                                       //Y
      //  "Equalization Tank",                      //Host name       
      //  0);                                       //pipe length
      ////
      //var insertFIT = new InsertBlockBase(
      //  1,                                        // number of item
      //  0,                                        // disctance of item
      //  "instrumentation tag",                    //block name
      //  "instrumentation",                        //layer name
      //  "Block Table1",                           //dynamic property type
      //  (short)11,                                       //visibility of equipment (FIT)
      //  PositionProperty.NumberOfPump
      //  * PositionProperty.DistanceOfPump + 50,   //X
      //  50,                                       //Y
      // "Equalization Tank",                       //Host name       
      //  0);                                       //pipe length
      ////
      //var insertJetPump = new InsertBlockBase(
      //  1,                                        // number of item
      //  0,                                        // disctance of item
      //  "pump",                                   //block name
      //  "equipment",                              //layer name
      //  "Centrifugal Pump",                       //dynamic property type
      //  (short)17,                                       //visibility of jet pump
      //  20,                                       //X
      //  10,                                       //Y
      //  "Equalization Tank",                      //Host name       
      //  0);                                       //pipe length
      ////
      //var insertChannelGateSTB = new InsertBlockBase(
      //  1,                                        // number of item
      //  0,                                        // disctance of item
      //  "channel gate",                           //block name
      //  "equipment",                              //layer name
      //  "Block Table1",                           //dynamic property type
      //  (short)23,                                       //visibility of item (Channel gate (Equalization Tank))
      //  -10.5,                                    //X
      //  6,                                        //Y
      // "Equalization Tank",                       //Host name       
      //  0);                                       //pipe length
      ////

      //var insertChannelGateDTY = new InsertBlockBase(
      //  1,                                         // number of item
      //  0,                                         // disctance of item
      //  "channel gate",                            //block name
      //  "equipment",                               //layer name
      //  "Block Table1",                            //dynamic property type
      //  (short)23,                                        //visibility of item (Channel gate (Equalization Tank))
      //  -10.5,                                     //X
      //  -24,                                       //Y
      //  "Equalization Tank",                       //Host name       
      //  0);                                        //pipe length

      //var insertPipe1 = new InsertBlockBase(
      //  PositionProperty.NumberOfPump,             // number of item
      //  PositionProperty.DistanceOfPump,           // disctance of item
      //  "pipe",                                    //block name
      //  "sewer",                                   //layer name
      //  "Visibility1",                             //dynamic property type
      //  "sewer",                                   //visibility of item (sewer pipe)
      //  50,                                        //X
      //  14,                                        //Y
      //  "Equalization Tank",                       //Host name       
      //  36);

      //var blocks = new[] {
      //  insertPipe1, insertChannelGateDTY, insertChannelGateSTB
      //};

      #endregion

    }

    public void DrawBlocks(Database db, IEnumerable<InsertBlockBase> blocks)
    {
      //setup default layers
      var defultLayers = new LayerCreator();
      defultLayers.Layers();

      var insertBlockTable = new InsertBlockTable(db);

      foreach (var block in blocks)
      {
        insertBlockTable.InsertBlock(block);
      }

      MoveToBottom.SendToBackBlock();
      //MoveToBottom.SendToBackLine();
      SendClass.SendRegen();
      SendClass.SendZoomExtents();
    }

    [CommandMethod("JCAD_PIDBuilder", CommandFlags.Modal)]
    public void BlockSearch()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var blockDeserialize = new BlockDeserializer();
      string path = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\Autocad PID blocks work in progress.dwg";
      // Copy blocks from sourcefile into opened file
      var copyBlock = new CopyBlock();

      #region btrNamesToCopy
      var eqType = new[] {
        "aeration diffuzer",
        "arrow",
        "alkalinity_chamber",
        "blower",
        "break",
        "chamber",
        "chamber circular",
        "chamber description",
        "channel",
        "channel gate",
        "channel2",
        "clarifier circular",
        "Clarifier Equipment",
        "clarifier longitudial",
        "collector",
        "controll tag e",
        "digester",
        "digester cap",
        "drum filter",
        "filter",
        "filter2",
        "Gas equipments",
        "gas flame",
        "gas holder",
        "grease_coll_chmbr",
        "heating",
        "instrumentairheaderarrow",
        "instrumentation tag",
        "leachet_coll_chmbr",
        "moister trap",
        "TWT",
        "ozone unit",
        //"PID-PS-FRAME",
        "pipe",
        "pipe2",
        "poli dosing",
        "pump",
        "mixer",
        "pr_cl_chmbr",
        "reactor",
        "RefALSewage",
        "RefALPoly",
        "RefALPotable",
        "RefALRecSludge",
        "RefARRecSludge",
        "RefALTransport",
        "RefARRawSludge",
        "RefARSewage",
        "RefARNaturalGas",
        "RefALRawSludge",
        "RefPIDEfluent",
        "RefPIDInfluent",
        "RefAL1AirToBiofilter",
        "RefAL1AirfromSludge",
        "RefAR1AirfromSludge",
        "RefAL2UtilityWater",
        "RefAR2UtilityWater",
        "RefAR3UtilityWater",
        "RefAL3UtilityWater",
        "RefAR4Leachate",
        "RefAL5Leachate",
        "RefAL6WasteSludge",
        "RefAR6WasteSludge",
        "RefAL7WasteAir",
        "RefAR7WasteAir",
        "RefAR10Sludge",
        "RefAR12Gas",
        "RefAR13Sludge",
        "RefAR14Water",
        "RefAL15Water",
        "RefAR16Water",
        "RefAL17Water",
        "RefAR18Water",
        "RefAL19Water",
        "RefAL22WasteSludge",
        "RefAR22WasteSludge",
        "RefAL23Leachate",
        "RefAR24UtilityWater",
        "RefAL25UtilityWater",
        "RefAR25UtilityWater",
        "RefAR26Leachate",
        "RefAL26Leachate",
        "sand trap",
        "screen",
        "screening press",
        "sst2_chmbr",
        "sst2Dig",
        "screening press",
        "sludge dewatering",
        "tank - vessel",
        "valve",
        "Vortex_grit_chamber",
        "Vortex_sand_chamber",
        "vortexEQ"};
      #endregion
      
      string[] btrNamesToCopy = eqType.Distinct().ToArray();

      copyBlock.CopyBlockTable(db, path, btr =>
      {
        System.Diagnostics.Debug.Print(btr.Name);
        return btrNamesToCopy.Contains(btr.Name);
      });

      var defultLayers = new LayerCreator();
      defultLayers.Layers();

      var insertBlock = new InsertBlock(db);
      var filePath = @"E:\Jszomor\source\repos\jszomorCAD\jCAD.PID_Builder\JsonPIDBuild.json";
      var jsonPID = blockDeserialize.ReadJsonData(filePath);

      //var sBlockName = jsonPID.Blocks.Select(b => b.Misc.BlockName);
      var blockNames =
        from b in jsonPID.Blocks
        select b.Misc.BlockName;

      foreach (var blockName in btrNamesToCopy)
      {
        try
        {
          insertBlock.PlaceBlocksByName(jsonPID, blockName);
          //MoveToBottom.SendToBackBlock();
        }
        catch (ArgumentNullException)
        {
          // ignore
        }
      }

      var insertBlockTable = new BlockTableRead(db);
      string fileName = "JsonPIDBuildCopy.json";
      insertBlockTable.ReadBtrForSeri(db, fileName);
      //var blockDeserialize = new BlockDeserialize();
      //var eqType = blockDeserialize.BlockSearch("Name");

      //System.Diagnostics.Debug.Print($"BlockName: {eqType}");
    }

    [CommandMethod("JCAD_SerializeBlock", CommandFlags.Modal)]
    public void StringBuilderSerialize()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      //var insertBlockTable = new InsertBlockTable(db);
      //insertBlockTable.ReadBtrForSeri(db);
      var insertBlockTable = new BlockTableRead(db);
      string fileName = "JsonPIDBuild.json";
      insertBlockTable.ReadBtrForSeri(db, fileName);
    }

    [CommandMethod("JCAD_Fillet", CommandFlags.Modal)]
    public void ComCommand()
    {
      Document doc = Application.DocumentManager.MdiActiveDocument;
      Editor ed = doc.Editor;
      Database db = doc.Database;

      Line l1 = new Line(new Point3d(250, 250, 0), new Point3d(400, 400, 0));
      Line l2 = new Line(new Point3d(0, 400, 0), new Point3d(150, 300, 0));

      SendClass.ToModelSpace(l1);
      SendClass.ToModelSpace(l2);
      SendClass.FilletCommand(ed, db, l1, l2);
    }

    [CommandMethod("JCAD_Attsync", CommandFlags.Modal)]
    public void AttsyncEquipment()
    {
      Document doc = Application.DocumentManager.MdiActiveDocument;
      doc.SendStringToExecute(("_attsync" + "\n" + "n" + "\n" + "pump" + "\n"), true, false, false);
    }

    //public void SendSync(Editor ed, string blockName, Database db)
    //{
    //  using (Transaction acTrans = db.TransactionManager.StartTransaction())
    //  {
    //    var blockIds = new List<ObjectId>();

    //    using (BlockTable bt = db.BlockTableId.GetObject<BlockTable>(OpenMode.ForRead))
    //    {
    //      // Open the Block table record Model space for write
    //      using (var btrModelSpace = acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
    //      {
    //        foreach (var btrId in btrModelSpace)
    //        {
    //          var item = btrId.GetObject(OpenMode.ForRead);
    //          if (item == null) continue;
    //          if (item is BlockReference)
    //          {
    //            var attrDef = item as BlockReference;

    //            if (attrDef.IsErased || attrDef.IsDisposed)
    //            {
    //              continue;
    //            }
    //            var dynBtr = acTrans.GetObject(attrDef.DynamicBlockTableRecord, OpenMode.ForRead, false) as BlockTableRecord;

    //            System.Diagnostics.Debug.Print("DynBlockName: " + dynBtr.Name);

    //            if (!dynBtr.IsAnonymous && !dynBtr.IsLayout && dynBtr.Name == blockName)
    //              blockIds.Add(btrId);
    //          }
    //        }

    //        var first = blockIds.First();
    //        var acEnt = acTrans.GetObject(first, OpenMode.ForWrite) as DBObject;

    //        ed.Command("_attsync"); // does not work, I have no idea why.

    //        if (blockIds.Count == 0) ed.WriteMessage($"No block record found with the name {blockName}");

    //      }
    //    }
    //  }
    //}

    [CommandMethod("JCAD_Layermerge", CommandFlags.Modal)]
    public void LayerMerge()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;

      var layerCreator = new LayerCreator();
      layerCreator.SelectEntity(db);
    }

    [CommandMethod("JCAD_SelectEntity", CommandFlags.Modal)]
    public void Select()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var select = new Select();
      select.SelectBlockReference(db);
    }

    [CommandMethod("AttachRasterImage")]

    public void AttachRasterImage()

    {

      // Get the current database and start a transaction 

      Database db = Application.DocumentManager.MdiActiveDocument.Database;

      using (Transaction tr = db.TransactionManager.StartTransaction())

      {

        // Define the name and image to use 

        string strImgName = "Organica_Logo";

        string strFileName = @"E:\Munka\OrganiCad\DrawingTemplates\Images\Organica_Logo.png";

        RasterImageDef acRasterDef;

        bool bRasterDefCreated = false;

        ObjectId acImgDefId;

        // Get the image dictionary 

        ObjectId acImgDctID = RasterImageDef.GetImageDictionary(db);

        // Check to see if the dictionary does not exist, it not then create it 

        if (acImgDctID.IsNull)

        {

          acImgDctID = RasterImageDef.CreateImageDictionary(db);

        }

        // Open the image dictionary 

        DBDictionary acImgDict = tr.GetObject(acImgDctID, OpenMode.ForRead) as DBDictionary;

        // Check to see if the image definition already exists 

        if (acImgDict.Contains(strImgName))

        {

          acImgDefId = acImgDict.GetAt(strImgName);

          acRasterDef = tr.GetObject(acImgDefId,

          OpenMode.ForWrite) as RasterImageDef;

        }

        else

        {

          // Create a raster image definition 

          RasterImageDef acRasterDefNew = new RasterImageDef();

          // Set the source for the image file

          acRasterDefNew.SourceFileName = strFileName;

          // Load the image into memory

          acRasterDefNew.Load();

          // Add the image definition to the dictionary

          acImgDict.UpgradeOpen();

          acImgDefId = acImgDict.SetAt(strImgName, acRasterDefNew);

          tr.AddNewlyCreatedDBObject(acRasterDefNew, true);

          acRasterDef = acRasterDefNew;

          bRasterDefCreated = true;

        }

        // Open the Block table for read 

        BlockTable bt = tr.GetObject(db.BlockTableId, OpenMode.ForRead) as BlockTable;

        // Open the Block table record Model space for write 

        BlockTableRecord btr = tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForWrite) as BlockTableRecord;

        // Create the new image and assign it the image definition 

        using (RasterImage acRaster = new RasterImage())

        {

          acRaster.ImageDefId = acImgDefId;

          // Use ImageWidth and ImageHeight to get the size of the image in pixels (1024 x 768). 

          // Use ResolutionMMPerPixel to determine the number of millimeters in a pixel so you  

          // can convert the size of the drawing into other units or millimeters based on the  

          // drawing units used in the current drawing. 

          // Define the width and height of the image 

          Vector3d width;

          Vector3d height;

          // Check to see if the measurement is set to English (Imperial) or Metric units 

          if (db.Measurement == MeasurementValue.English)

          {

            width = new Vector3d((acRasterDef.ResolutionMMPerPixel.X * acRaster.ImageWidth) / 25.4, 0, 0);

            height = new Vector3d(0, (acRasterDef.ResolutionMMPerPixel.Y * acRaster.ImageHeight) / 25.4, 0);

          }

          else

          {

            width = new Vector3d(acRasterDef.ResolutionMMPerPixel.X * acRaster.ImageWidth, 0, 0);

            height = new Vector3d(0, acRasterDef.ResolutionMMPerPixel.Y * acRaster.ImageHeight, 0);
          }

          // Define the position for the image  

          Point3d insPt = new Point3d(12.0, 12.0, 0.0);

          // Define and assign a coordinate system for the image's orientation 

          CoordinateSystem3d coordinateSystem = new CoordinateSystem3d(insPt, width * 2, height * 2);

          acRaster.Orientation = coordinateSystem;

          // Set the rotation angle for the image

          acRaster.Rotation = 0;

          // Add the new object to the block table record and the transaction

          btr.AppendEntity(acRaster);

          tr.AddNewlyCreatedDBObject(acRaster, true);

          // Connect the raster definition and image together so the definition 

          // does not appear as "unreferenced" in the External References palette. 

          RasterImage.EnableReactors(true);

          acRaster.AssociateRasterDef(acRasterDef);

          if (bRasterDefCreated)

            acRasterDef.Dispose();

        }

        // Save the new object to the database

        tr.Commit();

        // Dispose of the transaction

      }

    }

    [CommandMethod("SetVisiblityState", CommandFlags.Modal)]
    public void SetVisiblityState()
    {
      //	var pio = new PromptStringOptions("\nEnter blockname:");
      //	var inputString = Application.DocumentManager.MdiActiveDocument.Editor.GetString(pio);
      //	string blockName = Convert.ToString(inputString);
      string blockName = "pump";

      //var index = new PromptIntegerOptions("\nEnter index of equipment:");
      //var eqIndex = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(index);
      //var promptEqIndex = Convert.ToInt16(eqIndex.Value);
      short promptEqIndex = 53;

      var doc = Application.DocumentManager.MdiActiveDocument;
      string Note = "Anoxic recycle pump";
      using (doc.LockDocument())
      {
        using (var db = doc.Database)
        {
          var insertBlockTable = new InsertBlockTable(db);
          insertBlockTable.IterateBTRForSetupBlockVisibility(blockName, promptEqIndex, Note, db);
        }
      }
    }
    [CommandMethod("TagNumbering", CommandFlags.Modal)]
    public void TagNumbering()
    {
      var doc = Application.DocumentManager.MdiActiveDocument;
      using (doc.LockDocument())
      {
        using (var db = doc.Database)
        {
          var insertBlockTable = new InsertBlockTable(db);
          insertBlockTable.Numbering(db, true);
        }
      }
    }
  }
}


using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using OrganiCAD.AutoCAD;
using System;
using System.Linq;
using System.Collections.Generic;
using EquipmentPosition;

namespace jszomorCAD
{
  public class CommandMethods
  {
    public string path = @"E:\Test\Autocad PID blocks work in progress.dwg";    

    [CommandMethod("jcad_EquipmentBuilder")]
    public void ListBlocks()
    {
      var sizeProperty = new PositionProperty();
      //"\nEnter number of equipment:"
      var pio = new PromptIntegerOptions("\nEnter number of equipment:") { DefaultValue = 5 };
      var number = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(pio);

      //"\nEnter distance of equipment:"
      var dio = new PromptIntegerOptions("\nEnter distance of equipment:") { DefaultValue = 20 };
      var distance = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(dio);

      //"\nEnter index of equipment:"
      var eio = new PromptIntegerOptions("\nEnter index of equipment:") { DefaultValue = 22 };
      var eqIndex = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(eio);         
      
      var shortEqIndex = Convert.ToInt16(eqIndex.Value);
      //short shortCheckValveIndex = 2;
      PositionProperty.NumberOfPump = number.Value;
      PositionProperty.DistanceOfPump = distance.Value;     

      var db = Application.DocumentManager.MdiActiveDocument.Database;
      //var ed = Application.DocumentManager.MdiActiveDocument.Editor;
      //var aw = new AutoCadWrapper();

      // Copy blocks from sourcefile into opened file
      var copyBlockTable = new CopyBlockTable();
      var btrNamesToCopy = new[] { "pump", "valve", "chamber", "instrumentation tag" };
      copyBlockTable.CopyBlockTableMethod(db, path, btr =>
      {
        System.Diagnostics.Debug.Print(btr.Name);
        return btrNamesToCopy.Contains(btr.Name);
      });
      //copyBlockTable.CopyBlockTableMethod(db, path, btr => true);
      sizeProperty.X = 50;

      // Call a transaction to create layer
      //var layerCreator = new LayerCreator();
      //layerCreator.LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.5);

      //layerCreator.LayerCreatorMethod("unit", Color.FromRgb(0, 0, 255), 0.5);

      // Start transaction to write equipment
      var insertBlockTable = new InsertBlockTable();

      insertBlockTable.InsertBlockTableMethodAsTable(db,
                                              PositionProperty.NumberOfPump,        // number of item
                                              PositionProperty.DistanceOfPump,      // disctance of item
                                              "pump",                               //block name
                                              "equipment",                         //layer name
                                              "Centrifugal Pump",                  //dynamic property type
                                              shortEqIndex,                         //visibility of equipment
                                              sizeProperty.X,                      //X
                                              10);                                 //Y

      insertBlockTable.InsertBlockTableMethodAsVisibility(db,
                                              1,                                 // number of item
                                              0,                                  // disctance of item
                                              "chamber",                         //block name
                                              "unit",                            //layer name
                                              "Visibility",                      //dynamic property type
                                              "no channel",                       //visibility of equipment
                                              0,                                  //X
                                              0);                                 //Y

      insertBlockTable.InsertBlockTableMethodAsTable(db,
                                              PositionProperty.NumberOfPump,       // number of item
                                              PositionProperty.DistanceOfPump,     // disctance of item
                                              "valve",                             //block name
                                              "valve",                             //layer name
                                              "Block Table1",                      //dynamic property type
                                              5,                                   //visibility of equipment
                                              sizeProperty.X,                      //X
                                              25);                                 //Y

      insertBlockTable.InsertBlockTableMethodAsTable(db,
                                              PositionProperty.NumberOfPump,       // number of item
                                              PositionProperty.DistanceOfPump,     // disctance of item
                                              "valve",                             //block name
                                              "valve",                             //layer name
                                              "Block Table1",                      //dynamic property type
                                              0,                                   //visibility of equipment
                                              sizeProperty.X,                      //X
                                              40);                                  //Y

      insertBlockTable.InsertBlockTableMethodAsTable(db,
                                              1,                                   // number of item
                                              0,                                   // disctance of item
                                              "instrumentation tag",                //block name
                                              "instrumentation",                   //layer name
                                              "Block Table1",                      //dynamic property type
                                              7,                                   //visibility of equipment
                                              PositionProperty.NumberOfPump * PositionProperty.DistanceOfPump + 50,                      //X
                                              10);                                 //Y

      insertBlockTable.InsertBlockTableMethodAsTable(db,
                                              1,                                    // number of item
                                              0,                                    // disctance of item
                                              "pump",                               //block name
                                              "equipment",                         //layer name
                                              "Centrifugal Pump",                  //dynamic property type
                                              17,                                  //visibility of jet pump
                                              25,                                  //X
                                              10);                                 //Y

      MoveToBottom.SendToBackWipeout();

    } 
  }
}


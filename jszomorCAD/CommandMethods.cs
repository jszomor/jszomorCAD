using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using OrganiCAD.AutoCAD;
using System;
using System.Collections.Generic;

namespace jszomorCAD
{
  public class CommandMethods
  {
    public string path = @"E:\Test\Autocad PID blocks work in progress.dwg";

    [CommandMethod("jcad_EquipmentBuilder")]
    public void ListBlocks()
    {

      //"\nEnter number of equipment:"
      var pio = new PromptIntegerOptions("\nEnter number of equipment:") { DefaultValue = 1 };
      var number = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(pio);

      //"\nEnter distance of equipment:"
      var dio = new PromptIntegerOptions("\nEnter distance of equipment:") { DefaultValue = 0 };
      var distance = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(dio);

      //"\nEnter index of equipment:"
      var eio = new PromptIntegerOptions("\nEnter index of equipment:") { DefaultValue = 0 };
      var eqIndex = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(eio);

      var intNumber = number.Value;
      var intDistance = distance.Value;
      var shortEqIndex = Convert.ToInt16(eqIndex.Value);

      var db = Application.DocumentManager.MdiActiveDocument.Database;
      //var ed = Application.DocumentManager.MdiActiveDocument.Editor;
      //var aw = new AutoCadWrapper();

      // Copy pump from sourcefile
      var copyBlockTable = new CopyBlockTable();
      copyBlockTable.CopyBlockTableMethod(db, "pump", path);

      // Copy chamber from sourcefile      
      copyBlockTable.CopyBlockTableMethod(db, "chamber", path);

      // Call a transaction to create layer
      var layerCreator = new LayerCreator();
      layerCreator.LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.5);

      // Start transaction to write equipment
      var insertBlockTable = new InsertBlockTable();      

      insertBlockTable.InsertBlockTableMethod(db,
                                              intNumber,                         // number
                                              intDistance,                       // disctance
                                              "pump",                         //block name
                                              "equipment",                    //layer name
                                              "Centrifugal Pump",             //equipment type
                                              shortEqIndex);                       //index of equipment

    } 
  }
}


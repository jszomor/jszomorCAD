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
    [CommandMethod("jcad_EquipmentBuilder")]
    public void ListBlocks()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var ed = Application.DocumentManager.MdiActiveDocument.Editor;
      var aw = new AutoCadWrapper();

      // Copy pump from sourcefile
      var copyBlockTable = new CopyBlockTable();
      copyBlockTable.CopyBlockTableMethod(db, "pump", @"E:\Test\Autocad PID blocks work in progress.dwg");

      // Copy pump from sourcefile      
      copyBlockTable.CopyBlockTableMethod(db, "chamber", @"E:\Test\Autocad PID blocks work in progress.dwg");

      // Call a transaction to create layer
      var layerCreator = new LayerCreator();
      layerCreator.LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.5);

      // Start transaction to write equipment
      var insertBlockTable = new InsertBlockTable();
                                                                         
      insertBlockTable.InsertBlockTableMethod("\nEnter number of equipment:", // numberQuestion
                                              "\nEnter distance of equipment:", // disctanceQuestion
                                              "pump",                         //block name
                                              "equipment",                    //layer name
                                              "Centrifugal Pump");            //equipment type

    } 
  }
}


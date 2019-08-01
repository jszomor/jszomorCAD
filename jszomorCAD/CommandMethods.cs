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
    [CommandMethod("jcad_test")]
    public void Test()
    {
      //var db = Application.DocumentManager.MdiActiveDocument.Database;
      var ed = Application.DocumentManager.MdiActiveDocument.Editor;

      ed.WriteMessage("Hello World!");
    }

    [CommandMethod("jcad_EquipmentBuilder")]
    public void ListBlocks()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var ed = Application.DocumentManager.MdiActiveDocument.Editor;
      var aw = new AutoCadWrapper();

      // Copy specific item from file
      var copyBlockTable = new CopyBlockTable();
      copyBlockTable.CopyBlockTableMethod("pump", @"E:\Test\pump.dwg");

      // Call a transaction to create layer
      var layerCreator = new LayerCreator();
      layerCreator.LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.5);

      // Start transaction to write equipment
      var insertBlockTable = new InsertBlockTable();
      insertBlockTable.InsertBlockTableMethod("\nEnter number of equipment:", "pump", "equipment", "Centrifugal Pump");
    }
    

    [CommandMethod("jcad_SetLayerColor")]
    public static void SetLayerColor()
    {
      // Get the current document and database
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database dataBase = acDoc.Database;

      // Start a transaction
      using (Transaction transaction = dataBase.TransactionManager.StartTransaction())
      {
        // Open the Layer table for read
        var layerTable = transaction.GetObject(dataBase.LayerTableId, OpenMode.ForRead) as LayerTable;

        string sLayerName = "equipment";
        
        var acColors = Color.FromRgb(0, 0, 255);        

        var layerTableRecord = new LayerTableRecord();

        if (layerTable.Has(sLayerName) == false)
        {
          // Assign the layer a name
          layerTableRecord.Name = sLayerName;

          // Upgrade the Layer table for write
          if (layerTable.IsWriteEnabled == false) layerTable.UpgradeOpen();

          // Append the new layer to the Layer table and the transaction
          layerTable.Add(layerTableRecord);
          transaction.AddNewlyCreatedDBObject(layerTableRecord, true);
        }
        else
        {
          // Open the layer if it already exists for write
          layerTableRecord = transaction.GetObject(layerTable[sLayerName], OpenMode.ForWrite) as LayerTableRecord;
        }
        // Set the color of the layer
        layerTableRecord.Color = acColors;

        // Save the changes and dispose of the transaction
        transaction.Commit();
      }
    }

  }
}


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
      const string pump = "pump";

      using (Database sourceDb = new Database(false, true))
      {
        // Read the DWG into a side database
        sourceDb.ReadDwgFile(@"E:\Test\pump.dwg", System.IO.FileShare.ReadWrite, true, "");

        // Start transaction to read equipment
        aw.ExecuteActionOnBlockTable(sourceDb, bt =>
        {
          // Create a variable to store the list of block identifiers
          ObjectIdCollection blockIds = new ObjectIdCollection();

          foreach (var objectId in bt)
          {
            using (var btr = objectId.GetObject<BlockTableRecord>())
            {
              // Only add named & non-layout blocks to the copy list
              if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == pump)
                blockIds.Add(objectId);
            }
          }

          // Copy blocks from source to destination database
          IdMapping mapping = new IdMapping();
          sourceDb.WblockCloneObjects(blockIds, db.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
        });
      }

      // Start a transaction to create layer
      aw.ExecuteActionOnLayerTable(db, (tr, lt) =>
      {
        // Open the Layer table for read
        var layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

        string sLayerName = "Equipment";
        string sLayerName1 = sLayerName.ToLower();
        string sLayerName2 = sLayerName.ToUpper();
        Color acColors = Color.FromRgb(0, 0, 255);

        // Append the new layer to the Layer table and the transaction
        var layerTableRecord = new LayerTableRecord();

        if (layerTable.Has(sLayerName1) == false || layerTable.Has(sLayerName) == false || layerTable.Has(sLayerName2) == false)
        {
          // Assign the layer a name
          layerTableRecord.Name = sLayerName1;

          // Upgrade the Layer table for write
          if (layerTable.IsWriteEnabled == false) layerTable.UpgradeOpen();

          // Append the new layer to the Layer table and the transaction
          layerTable.Add(layerTableRecord);
          tr.AddNewlyCreatedDBObject(layerTableRecord, true);
        }
        else
        {
          // Open the layer if it already exists for write
          layerTableRecord = tr.GetObject(layerTable[sLayerName], OpenMode.ForWrite) as LayerTableRecord;
        }
        // Set the color of the layer
        layerTableRecord.Color = acColors;
        db.Ltscale = 0.5;
      });

      // Start transaction to write equipment
      aw.ExecuteActionOnBlockTable(db, (tr, bt) =>
      {
        var blockDefinitions = new List<ObjectId>();
        foreach (ObjectId btrId in bt)
        {
          using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead, false))
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == "pump")
              blockDefinitions.Add(btrId);
          }
        }

        var pio = new PromptIntegerOptions("\nEnter number of equipment:") { DefaultValue = 1 };
        var pi = Application.DocumentManager.MdiActiveDocument.Editor.GetInteger(pio);

        int x = 0;

        var currentSpaceId = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        for (int i = 0; i < pi.Value; i++)
        {
          foreach (var objectId in blockDefinitions)
          {
            using (var blockDefinition = (BlockTableRecord)tr.GetObject(objectId, OpenMode.ForRead, false))
            {
              using (var acBlkRef = new BlockReference(new Point3d(x, 0, 0), objectId))
              {
                currentSpaceId.AppendEntity(acBlkRef);
                tr.AddNewlyCreatedDBObject(acBlkRef, true);

                acBlkRef.Layer = "equipment";

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
                }

                // set dynamic properties
                if (acBlkRef.IsDynamicBlock)
                {
                  foreach (DynamicBlockReferenceProperty dbrProp in acBlkRef.DynamicBlockReferencePropertyCollection)
                  {
                    if (dbrProp.PropertyName == "Centrifugal Pump")
                      dbrProp.Value = (short)45; // SHORT !!!!!!!!!!!!

                    //if (dbrProp.PropertyName == "Angle")
                    //  dbrProp.Value = 90; // SHORT !!!!!!!!!!!!aut


                  }
                }
              }
            }
          }

          x += 20;
        }
        currentSpaceId.UpdateAnonymousBlocks();        
      });

    }
    //Assign Color to a Layer

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


using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using OrganiCAD.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jszomorCAD
{
  public class LayerCreator
  {
    public void LayerCreatorMethod(string sLayerName, Color acColors, double lineTypeScale)
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var aw = new AutoCadWrapper();

      // Start a transaction to create layer
      aw.ExecuteActionOnLayerTable(db, (tr, lt) =>
      {
        // Open the Layer table for read
        var layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForRead) as LayerTable;

        //layerName definition;
        string sLayerName1 = sLayerName.ToLower();
        string sLayerName2 = sLayerName.ToUpper();        

        // Append the new layer to the Layer table and the transaction
        var layerTableRecord = new LayerTableRecord();

        if (layerTable.Has(sLayerName1) == false || layerTable.Has(sLayerName) == false || layerTable.Has(sLayerName2) == false)
        {
          // Assign the layer a name
          layerTableRecord.Name = sLayerName;

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
        db.Ltscale = lineTypeScale;
        layerTableRecord.LineWeight = LineWeight.LineWeight025;
      });
    }
    public void Layers()
    {
      //setup default layers      
      LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.25);
      LayerCreatorMethod("unit", Color.FromRgb(255, 0, 0), 0.25);
      LayerCreatorMethod("valve", Color.FromRgb(255, 255, 255), 0.25);
      LayerCreatorMethod("valve2", Color.FromRgb(255, 255, 255), 0.25);
      LayerCreatorMethod("instrumentation", Color.FromRgb(0, 255, 255), 0.25);
      LayerCreatorMethod("text", Color.FromRgb(255, 255, 255), 0.25);
      LayerCreatorMethod("sewer", Color.FromRgb(28, 38, 0), 0.25);
      LayerCreatorMethod("sludge", Color.FromRgb(38, 19, 19), 0.25);
      LayerCreatorMethod("chemical", Color.FromRgb(0, 255, 255), 0.25);
      LayerCreatorMethod("water", Color.FromRgb(0, 0, 255), 0.25);
      LayerCreatorMethod("treated_water", Color.FromRgb(0, 127, 255), 0.25);
      LayerCreatorMethod("air", Color.FromRgb(63, 255, 0), 0.25);
      LayerCreatorMethod("recycle_flow", Color.FromRgb(145, 165, 82), 0.25);
    }
  }
}

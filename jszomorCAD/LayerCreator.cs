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

  public class LayerData
  {
    public string LayerName { get; set; }
    public Color Color { get; set; }

    public bool IsOff;

    public double LineTypeScale { get; set; }

    public LayerData(string layerName, Color color, double lineTypeScale, bool isOff)
    {
      LayerName = layerName;
      Color = color;
      LineTypeScale = lineTypeScale;
      IsOff = isOff;
    }
  }

  public class LayerCreator
  {

    public void LayerCreatorMethod(IEnumerable<LayerData> layers)
    {
      foreach (var layer in layers)
      {
        LayerCreatorMethod(layer);
      }
    }

    public void LayerCreatorMethod(LayerData layerData) => LayerCreatorMethod(layerData.LayerName, layerData.Color, layerData.LineTypeScale, layerData.IsOff);


    public void LayerCreatorMethod(string sLayerName, Color acColors, double lineTypeScale, bool isOff)
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
        // Set layer
        layerTableRecord.Color = acColors;
        db.Ltscale = lineTypeScale;
        layerTableRecord.LineWeight = LineWeight.LineWeight025;
        layerTableRecord.IsOff = isOff;
      });
    }
    public void Layers()
    {
      //setup default layers      
      LayerCreatorMethod("equipment", Color.FromRgb(0, 0, 255), 0.25, false);
      LayerCreatorMethod("unit", Color.FromRgb(255, 0, 0), 0.25, false);
      LayerCreatorMethod("valve", Color.FromRgb(255, 255, 255), 0.25, false);
      LayerCreatorMethod("valve2", Color.FromRgb(255, 255, 255), 0.25, true);
      LayerCreatorMethod("instrumentation", Color.FromRgb(0, 255, 255), 0.25, false);
      LayerCreatorMethod("text", Color.FromRgb(255, 255, 255), 0.25, false);
      LayerCreatorMethod("sewer", Color.FromRgb(28, 38, 0), 0.25, false);
      LayerCreatorMethod("sludge", Color.FromRgb(38, 19, 19), 0.25, false);
      LayerCreatorMethod("chemical", Color.FromRgb(0, 255, 255), 0.25, false);
      LayerCreatorMethod("water", Color.FromRgb(0, 0, 255), 0.25, false);
      LayerCreatorMethod("treated_water", Color.FromRgb(0, 127, 255), 0.25, false);
      LayerCreatorMethod("air", Color.FromRgb(63, 255, 0), 0.25, false);
      LayerCreatorMethod("recycle_flow", Color.FromRgb(145, 165, 82), 0.25, false);
      LayerCreatorMethod("pipeTag", Color.FromRgb(255, 255, 255), 0.25, false);
    }

    public void LayerStatus()
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var aw = new AutoCadWrapper();
      
      aw.ExecuteActionOnLayerTable(db, (tr, lt) =>
      {
        var layerTable = tr.GetObject(db.LayerTableId, OpenMode.ForWrite) as LayerTable;

        // Append the new layer to the Layer table and the transaction
        var layerTableRecord = new LayerTableRecord();

           layerTableRecord.IsOff = true;
      });
    }
  }
}

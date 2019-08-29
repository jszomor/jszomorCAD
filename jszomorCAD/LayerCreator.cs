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
  }
}

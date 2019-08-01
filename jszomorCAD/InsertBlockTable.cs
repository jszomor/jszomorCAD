using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using OrganiCAD.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jszomorCAD
{
  public class InsertBlockTable
  {
    public void InsertBlockTableMethod(string question, string itemType, string layerName, string propertyName)
    {
      var db = Application.DocumentManager.MdiActiveDocument.Database;
      var aw = new AutoCadWrapper();

      // Start transaction to insert equipment
      aw.ExecuteActionOnBlockTable(db, (tr, bt) =>
      {
        var blockDefinitions = new List<ObjectId>();
        foreach (ObjectId btrId in bt)
        {
          using (BlockTableRecord btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead, false))
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == itemType)
              blockDefinitions.Add(btrId);
          }
        }
        //"\nEnter number of equipment:"
        var pio = new PromptIntegerOptions(question) { DefaultValue = 1 };
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

                acBlkRef.Layer = layerName;

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
                    if (dbrProp.PropertyName == propertyName)
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
  }
}


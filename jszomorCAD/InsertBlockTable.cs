using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using EquipmentPosition;
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
    //public void InsertVfdPump(Database db, PromptIntegerResult number, PromptIntegerResult distance, string itemType, string layerName, int eqIndex) => 
    //  InsertBlockTableMethod(db, number, distance, itemType, layerName, "Centrifugal Pump", eqIndex); // todo: magic number

    public void InsertBlockTableMethod(Database db, int number, int distance, string itemType, string layerName, string propertyName, short eqIndex)
    {
      Editor ed = Application.DocumentManager.MdiActiveDocument.Editor;     
      var aw = new AutoCadWrapper();
      BlockTableRecord btr;
      var blockDefinitions = new List<ObjectId>();
      var positionProperty = new PositionProperty();
      
      //var shortEqIndex = Convert.ToInt16(eqIndex);

      // Start transaction to insert equipment
      aw.ExecuteActionOnBlockTable(db, (tr, bt) =>
      {
        
        foreach (ObjectId btrId in bt)
        {
          using (btr = (BlockTableRecord)tr.GetObject(btrId, OpenMode.ForRead, false))
          {
            // Only add named & non-layout blocks to the copy list
            if (!btr.IsAnonymous && !btr.IsLayout && btr.Name == itemType)
            {
              blockDefinitions.Add(btrId);             
            }
          }
        }

        positionProperty.X = 0;

        var currentSpaceId = tr.GetObject(db.CurrentSpaceId, OpenMode.ForWrite) as BlockTableRecord;

        for (int i = 0; i < number; i++)
        {
          foreach (var objectId in blockDefinitions)
          {
            using (var blockDefinition = (BlockTableRecord)tr.GetObject(objectId, OpenMode.ForRead, false))
            {
              using (var acBlkRef = new BlockReference(new Point3d(positionProperty.X, positionProperty.Y, positionProperty.Z), objectId))
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
                      dbrProp.Value = eqIndex; // SHORT !!!!!!!!!!!!

                    if (dbrProp.PropertyName == "Angle1")
                      dbrProp.Value = DegreeHelper.DegreeToRadian(90);

                    if (dbrProp.PropertyName == "Angle2")
                      dbrProp.Value = DegreeHelper.DegreeToRadian(270);                    
                  }
                }
              }
            }
          }
          positionProperty.X += distance;
        }
        currentSpaceId.UpdateAnonymousBlocks();
      });
    }
  }  
}


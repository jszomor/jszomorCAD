using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrganiCAD.AutoCAD;
using System.Linq.Expressions;
using JsonParse;
using JsonFindKey;

namespace jCAD.PID_Builder
{
  public class BlockTableRead
  {
    private Database _db;
    public BlockTableRead(Database db)
    {
      _db = db;
    }

    public static int InternalCounter { get; set; }
    public void ReadBtrForSeri(Database db, string fileName)
    {
      ExtensionMethod.ExecuteActionOnModelSpace(db, (tr, btrModelSpace) =>
      {
        var jsonLineSetup = new JsonLineSetup();
        var jsonBlockSetup = new JsonBlockSetup();

        var jsonPID = new JsonPID();
        InternalCounter = 1;
        foreach (ObjectId objectId in btrModelSpace)
        {
          using (var item = objectId.GetObject(OpenMode.ForWrite))
          {
            if (item is BlockReference)
            {
              BlockReference blockReference = item as BlockReference;
              if (blockReference == null) continue;
              var btrObjectId = blockReference.DynamicBlockTableRecord; //must be Dynamic to find every blocks
              var blockDefinition = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord;
              if (blockDefinition.Name != "PID-PS-FRAME")
              {
                jsonPID.Blocks.Add(jsonBlockSetup.SetupBlockProperty(blockDefinition, tr, blockReference));
                InternalCounter++;
              }
            }
       
            if (item == null) continue;

            if (item is Line || item is Polyline || item is Polyline2d || item is Polyline3d || item is Circle || item is Arc)
            {
              //jsonLineToSerialize.Add(jsonLineSetup.SetupLineProperty(item));
              jsonPID.Lines.Add(jsonLineSetup.SetupLineProperty(item, jsonBlockSetup));
              InternalCounter++;
            }
          }
        }

        jsonPID.Blocks.Sort();
        jsonPID.Lines.Sort();

        var seralizer = new JsonStringBuilderSerialize();
        seralizer.StringBuilderSerialize(jsonPID, fileName);
      });
    }
  }
}

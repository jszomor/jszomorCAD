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

    public static int OrderCounter { get; set; }
    public void ReadBtrForSeri(Database db, string fileName)
    {
      Wrappers.ExecuteActionOnModelSpace(db, (tr, btrModelSpace) =>
      {
        var jsonLineSetup = new JsonLineSetup();
        var jsonBlockSetup = new JsonBlockSetup();

        var jsonPID = new JsonPID();
        OrderCounter = 1;
        foreach (ObjectId objectId in btrModelSpace)
        {
          using (var item = objectId.GetObject(OpenMode.ForWrite))
          {
            if (item == null) continue;

            if (item is Line || item is Polyline || item is Polyline2d || item is Polyline3d)
            {
              //jsonLineToSerialize.Add(jsonLineSetup.SetupLineProperty(item));
              jsonPID.Lines.Add(jsonLineSetup.SetupLineProperty(item));
            }

            if (item is BlockReference)
            {
              BlockReference blockReference = item as BlockReference;
              if (blockReference == null) continue;
              var btrObjectId = blockReference.DynamicBlockTableRecord; //must be Dynamic to find every blocks
              var blockDefinition = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord;
              if (blockDefinition.Name != "PID-PS-FRAME")
              {
                //jsonPID.Blocks.Sort();
                jsonPID.Blocks.Add(jsonBlockSetup.SetupBlockProperty(blockDefinition, tr, blockReference));
                OrderCounter++;
              }
            }
          }
        }
        var seralizer = new JsonStringBuilderSerialize();
        seralizer.StringBuilderSerialize(jsonPID, fileName);
      });
    }
  }
}

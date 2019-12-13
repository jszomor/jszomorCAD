using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.Colors;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrganiCAD.AutoCAD;
using JsonFindKey;
using JsonParse;
using System.Linq.Expressions;
using jszomorCAD;

namespace JsonEnumerate
{
  class Program
  {
    public void ReadBtrForSeri(Database db)
    {
      var aw = new InsertBlockTable(db);
      aw. ExecuteActionOnModelSpace(db, (tr, btrModelSpace) =>
      {
        var entitiesToSerialize = new List<JsonBlockClass>();

        foreach (ObjectId objectId in btrModelSpace)
        {
          using (var blockReference = tr.GetObject(objectId, OpenMode.ForRead) as BlockReference)
          {
            if (blockReference == null) continue;

            var btrObjectId = blockReference.DynamicBlockTableRecord; //must be Dynamic to find every blocks
            using (var blockDefinition = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord)
            {
              var JsonBlockSetup = new JsonBlockEnumerate();
              //var serializationAttributeSetup = new SerializationAttributeSetup();

              //IEnumerable<JsonBlockProperty>jsonProperty;

              var jsonProperty = new JsonBlockClass();

              entitiesToSerialize.Add(JsonBlockSetup.SetupBlockProperty(blockDefinition, blockReference, jsonProperty));
              //entitiesToSerialize.Add(serializationAttributeSetup.SetupAttributeProperty(tr, blockReference, jsonProperty));
            }
          }
        }
        var seralizer = new JsonEnumerateSerialize();
        seralizer.JsonJcadSerialize(entitiesToSerialize);
      });
    }
  }
}

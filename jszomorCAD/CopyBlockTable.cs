using Autodesk.AutoCAD.ApplicationServices.Core;
using Autodesk.AutoCAD.DatabaseServices;
using OrganiCAD.AutoCAD;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jszomorCAD
{
  public class CopyBlockTable
  {
    public void CopyBlockTableMethod(Database db, string filePath)
    {     
      var aw = new AutoCadWrapper();      

      using (Database sourceDb = new Database(false, true))
      {
        // Read the DWG into a side database
        sourceDb.ReadDwgFile(filePath, System.IO.FileShare.ReadWrite, true, "");
       
        // Start transaction to read equipment
        aw.ExecuteActionOnBlockTable(sourceDb, bt =>
        {
          // Create a variable to store the list of block identifiers
          ObjectIdCollection blockIds = new ObjectIdCollection();

          foreach (var objectId in bt)
          {
            using (var btr = objectId.GetObject<BlockTableRecord>())
            {
              // Only add named & non-layout blocks to the copy list and filter for specific item
              if (!btr.IsAnonymous && !btr.IsLayout)
                blockIds.Add(objectId);
            }
          }
          // Copy blocks from source to destination database
          IdMapping mapping = new IdMapping();
          sourceDb.WblockCloneObjects(blockIds, db.BlockTableId, mapping, DuplicateRecordCloning.Replace, false);
        });
      }
    }
  }
}

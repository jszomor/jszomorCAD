using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
//using OrganiCAD.AutoCAD;

namespace jszomorCAD
{
  public static class Extension
  {
    public static string GetRealName(this BlockReference br)
    {
      if (br == null) throw new ArgumentNullException(nameof(br));

      var returnValue = string.Empty;
      using (var dynBtr = br.DynamicBlockTableRecord.GetObject<BlockTableRecord>())
      {
        if (dynBtr != null) returnValue = dynBtr.Name; // we could get NULL here. dynBtr?.Name would propagate this NULL down the call chain, and it could cause issues
      }
      return returnValue; // should we return something else here?
    }
  }

  public class Select
  {
    public void ExecuteActionOnModelSpace(Database database, Action<Transaction, BlockTableRecord> action)
    {
      ExecuteActionInTransaction(database, (db, tr) =>
        ExecuteActionOnBlockTable(db, bt =>
        {
          using (var ms = bt[BlockTableRecord.ModelSpace].GetObject<BlockTableRecord>())
          {
            action.Invoke(tr, ms);
          }
        }
        ));
    }

    public void ExecuteActionInTransaction(Database db, Action<Database, Transaction> action)
    {
      using (var tr = db.TransactionManager.StartTransaction())
      {
        action.Invoke(db, tr);
        tr.Commit();
      }
    }

    private void ExecuteActionOnTable<T>(Database db,
      Expression<Func<Database, ObjectId>> tableIdProperty, Action<T> action) where T : class, IDisposable
    {
      var c = tableIdProperty.Compile();
      using (var t = c.Invoke(db).GetObject<T>())
      {
        action.Invoke(t);
      }
    }
    public void ExecuteActionOnBlockTable(Database db, Action<BlockTable> action) =>
      ExecuteActionOnTable(db, x => x.BlockTableId, action);

    public void SelectEntity(Database db)
    {
      ExecuteActionOnModelSpace(db, (tr, btrModelSpace) =>
      {
        foreach (ObjectId objectId in btrModelSpace)
        {
          using (var entity = tr.GetObject(objectId, OpenMode.ForWrite) as Autodesk.AutoCAD.DatabaseServices.Entity)
          {
            if (entity == null) continue;

            System.Diagnostics.Debug.Print(btrModelSpace.Name);
          }
        }
      });
    }

    public void SelectBlockReference(Database db)
    {
      ExecuteActionOnModelSpace(db, (tr, btrModelSpace) =>
      {
        foreach (ObjectId objectId in btrModelSpace)
        {
          using (var blockReference = tr.GetObject(objectId, OpenMode.ForRead) as BlockReference)
          {
            if (blockReference == null) continue;

            var btrObjectId = blockReference.DynamicBlockTableRecord; //must be Dynamic to find every blocks
            using (var blockDefinition = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord)
            {
              //System.Diagnostics.Debug.Print(blockDefinition.Name);

              //if (blockDefinition.Name == "RefPIDDenit$0$reactor")
              //{
              //  System.Diagnostics.Debug.Print("STOP! Hammertime!");
              //}

              foreach (ObjectId BlockObjectId in blockDefinition)
              {
                var entity = tr.GetObject(BlockObjectId, OpenMode.ForWrite) as Autodesk.AutoCAD.DatabaseServices.Entity;

                if (entity == null) continue;
                System.Diagnostics.Debug.Print(blockReference.GetRealName());

              }
            }
          }
        }
      });
    }
  }
}

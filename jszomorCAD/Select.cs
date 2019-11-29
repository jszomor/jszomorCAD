using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using OrganiCAD.AutoCAD;

namespace jszomorCAD
{
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

            System.Diagnostics.Debug.Print(entity.BlockName);
          }
        }
      });
    }
  }
}

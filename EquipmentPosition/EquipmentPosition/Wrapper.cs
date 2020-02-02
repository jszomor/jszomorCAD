using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OrganiCAD.AutoCAD;
using System.Linq.Expressions;

namespace EquipmentPosition
{
  public class Wrapper
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

    public void ExecuteActionOnLayerTable(Database db, Action<LayerTable> action) =>
      ExecuteActionOnTable(db, x => x.LayerTableId, action);

  }
}

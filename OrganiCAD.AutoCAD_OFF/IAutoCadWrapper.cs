using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace OrganiCAD.AutoCAD
{
  public interface IAutoCadWrapper
  {
    void ExecuteActionOnBlockReferences(string fileName, Action<BlockReference> action, bool saveFile = true);
    void ExecuteActionOnEntities<T>(string fileName, Action<T> action) where T : Entity;
    void ExecuteActionOnEntities<T>(string fileName, Action<T> action, Predicate<T> predicate) where T : Entity;
    void ExecuteActionOnModelSpace(string fileName, Action<Transaction, BlockTableRecord> action);
  }
}
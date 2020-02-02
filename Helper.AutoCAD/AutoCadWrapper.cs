using System;
using Autodesk.AutoCAD.DatabaseServices;

namespace OrganiCAD.AutoCAD
{
  public class AutoCadWrapper : IAutoCadWrapper
  {
    public void ExecuteActionOnModelSpace(string fileName, Action<Transaction, BlockTableRecord> action) =>
      Wrappers.ExecuteActionOnBlockTable(fileName, (tr, bt) =>
        Wrappers.ExecuteActionOnModelSpace(tr, bt, action.Invoke));

    public void ExecuteActionOnModelSpace(Database db, Action<BlockTableRecord> action) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, bt =>
            Wrappers.ExecuteActionOnModelSpace(bt, action)));

    public void ExecuteActionOnBlockTable(Database db, Action<BlockTable> action) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, action));

    public void ExecuteActionOnBlockTable(Database db, Action<Transaction, BlockTable> action) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, action));

    public void ExecuteActionOnLayerTable(Database db, Action<LayerTable> action) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnLayerTable(db, tr, action));

    public void ExecuteActionOnLayerTable(Database db, Action<Transaction, LayerTable> action) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnLayerTable(db, tr, action));

    public void ExecuteActionOnEntities<T>(Database db, Action<T> action) where T : Entity =>
      ExecuteActionOnEntities(db, action, btr => true);

    public void ExecuteActionOnEntities<T>(string fileName, Action<T> action) where T : Entity =>
      ExecuteActionOnEntities(fileName, action, btr => true);

    public void ExecuteActionOnEntities<T>(string fileName, Action<T> action, Predicate<T> predicate) where T : Entity =>
      Wrappers.ExecuteActionOnBlockTable(fileName, (tr, bt) =>
        Wrappers.ExecuteActionOnModelSpace(tr, bt, (tran, ms) =>
          Wrappers.ExecuteActionOnItemsInModelSpace(tr, ms, action, predicate)));

    public void ExecuteActionOnEntities<T>(Database db, Action<T> action, Predicate<T> predicate) where T : Entity =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, bt =>
          Wrappers.ExecuteActionOnModelSpace(tr, bt, (tran, ms) =>
            Wrappers.ExecuteActionOnItemsInModelSpace(tr, ms, action, predicate))));


		public void ExecuteActionReadOnlyOnBlockReferences(string fileName, Action<BlockReference> action) =>
	ExecuteActionOnBlockReferences(fileName, (_, br) => action.Invoke(br), saveFile:false);

		/// <summary>
		/// Executes an action on every block reference in a file
		/// </summary>
		/// <param name="fileName"></param>
		/// <param name="action"></param>
		public void ExecuteActionOnBlockReferences(string fileName, Action<BlockReference> action, bool saveFile = true) =>
      ExecuteActionOnBlockReferences(fileName, (_, br) => action.Invoke(br), saveFile);

    public void ExecuteActionOnBlockReferences(Database db, Action<Transaction, BlockReference> action, bool executeOnAnonymousBlocks = true) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, (tran, bt) =>
          Wrappers.ExecuteActionOnBlockReferences(tran, bt, action, executeOnAnonymousBlocks: executeOnAnonymousBlocks)));

    public void ExecuteActionOnBlockReferences(string fileName, Action<Transaction, BlockReference> action, bool saveFile = true) =>
      Wrappers.ExecuteActionOnBlockTable(fileName, (tr, bt) =>
        Wrappers.ExecuteActionOnBlockReferences(tr, bt, action), saveFile);

    public void AdjustAttributeAlignmentsInFile(string fileName) =>
      Wrappers.ExecuteActionOnBlockTable(fileName, (tr, bt) =>
        Wrappers.ExecuteActionOnBlockReferences(tr, bt, Wrappers.AdjustAttributeAlignments, OpenMode.ForWrite));

    public void AdjustAttributeAlignmentsInDatabase(Database db) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, bt =>
          Wrappers.ExecuteActionOnBlockReferences(tr, bt, Wrappers.AdjustAttributeAlignments, OpenMode.ForWrite)));

    public void CreateMandatoryLayers(Database db) => Wrappers.CreateMandatoryLayers(db);
    public void FixLayers(Database db) => Wrappers.FixLayers(db);

    public void FixValveLines(Database db) => Wrappers.FixValveLines(db);

    public void ExecuteActionOnAttributeReferences(string fileName, Action<AttributeReference> action, bool saveFile = true) =>
      Wrappers.ExecuteActionOnBlockTable(fileName, (tr, bt) =>
        Wrappers.ExecuteActionOnBlockReferences(tr, bt, (tran, br) =>
          Wrappers.ExecuteActionOnAttributeReferences(br, action)), saveFile);

    public void PurgeAll(Database db) => Wrappers.PurgeAll(db);

    public void PurgeAllOnBlockTable(Database db) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, bt => Wrappers.PurgeSymbolTable<BlockTable>(bt)));

    public void PurgeAllOnLayerTable(Database db) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnLayerTable(db, tr, lt => Wrappers.PurgeSymbolTable<LayerTable>(lt)));

    public void AddAttributeToBlocks(Database db, Predicate<BlockReference> shouldAddAttribute, string attributeTag, string attributeDefaultValue) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, bt =>
          Wrappers.ExecuteActionOnModelSpace(bt, modelSpace =>
            Wrappers.AddAttributeToBlocks(bt, tr, modelSpace, shouldAddAttribute, attributeTag, attributeDefaultValue))));

    public void ExecuteActionOnDnyProperties(BlockReference br, Action<DynamicBlockReferenceProperty> action)
    {
      foreach (var property in br.GetDynamicProperties())
      {
        action.Invoke(property);
      }
    }

    public void FixAttrMover(Database db) =>
      Wrappers.ExecuteActionInTransaction(db, tr =>
        Wrappers.ExecuteActionOnBlockTable(db, tr, bt =>
          Wrappers.ExecuteActionOnModelSpace(bt, Wrappers.FixAttrMover)));

		//tre function is a wrapper for setstandby
		public bool TryToSetStandByBasedonNoteAttribute(Database db,string blockNote,int numberOfStandby,bool isLeftToDownSelection) =>	
			Wrappers.TryToSetStandByBasedonNoteAttribute(db,blockNote,numberOfStandby,isLeftToDownSelection);

		//public void RecalculatePowers(Database db) =>
  //    Wrappers.ExecuteActionInTransaction(db, tr =>
  //      Wrappers.ExecuteActionOnBlockTable(db, tr, bt =>
  //        Wrappers.ExecuteActionOnBlockReferences(bt, Wrappers.RecalculatePowers)));

  //  public void CheckForErrors(Database db) =>
  //    Wrappers.ExecuteActionInTransaction(db, tr =>
  //      Wrappers.ExecuteActionOnBlockTable(db, tr, Wrappers.CheckForErrors));

    public void BindxRefs(Database db) =>
      Wrappers.ExecuteActionInTransaction(db, tr => Wrappers.BindxRefs(db, tr));

    //public void ExplodeBlocksWithNoAttributes(Database db) =>
    //  Wrappers.ExecuteActionInTransaction(db, tr => Wrappers.ExplodeBlocksWithNoAttributes(db, tr));

    //public void ExplodeBlocksByPredicate(Database db, Predicate<BlockReference> predicate) =>
    //  Wrappers.ExecuteActionInTransaction(db, tr => Wrappers.ExplodeBlocksByPredicate(db, tr, predicate));

    public void SendCommand(Autodesk.AutoCAD.ApplicationServices.Document doc, params string[] commandParts) => Wrappers.SendCommand(doc, commandParts);
						
	}
}
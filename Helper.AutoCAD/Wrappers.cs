using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
//using Organica.Core.Logging;
//using OrganiCAD.Mechanical.Models;

namespace OrganiCAD.AutoCAD
{
	internal static class Wrappers
	{
		internal static long FinallyCounter = 0;

		//private static void SendCommand(Document document, params object[] command) => 
		//  document.Editor.Command(command);

		///// <summary>
		///// 
		///// </summary>
		///// <param name="document"></param>
		///// <remarks>
		///// 'A' Purge every entity possible
		///// '*' Purge everything possible (no filters)
		///// 'N' Do not ask for confirmation on every entity purge
		///// </remarks>
		//internal static void SendCommandPurge(Document document) =>
		//  SendCommand(document, "-PURGE\nA\n*\nN\n");

		internal static void ExecuteActionOnDocument(Document document, Action<Document> action)
		{
			using (document.LockDocument())
			{
				action.Invoke(document);
				document.TransactionManager.FlushGraphics();
				document.Editor.UpdateScreen();
			}
		}

		internal static void ExecuteReadOnlyActionOnDatabase(string fileName, Action<Database> action)

		{
			ExecuteActionOnDatabase(fileName, action, saveFile: false, closeFile: false);
		}
			internal static void ExecuteActionOnDatabase(string fileName, Action<Database> action, bool saveFile = true, bool closeFile =false)
		{
			using (var db = new Database(false, true)) // do not build a default drawing and there is no UI document attached
			{
				try
				{
					db.ReadDwgFile(fileName, FileShare.ReadWrite, false, null);
					db.CloseInput(false); // read everything from the database, and keep the file open

					action.Invoke(db);
				}
				finally
				{
					FinallyCounter++;
					if (saveFile)
						db.SaveAs(fileName, DwgVersion.Current);
					if (closeFile)
						db.CloseInput(closeFile);
				}
			}
		}

		internal static void ExecuteActionOnDatabase(Document document, string fileName, Action<Database> action, bool saveFile = true)
		{
			using (var db = document.Database) // do not build a default drawing and there is no UI document attached
			{
				try
				{
					action.Invoke(db);
				}
				finally
				{
					FinallyCounter++;
					if (saveFile)
						db.SaveAs(fileName, DwgVersion.Current);
				}
			}
		}

		internal static void ExecuteActionOnDatabase(Document document, Action<Database> action, bool saveFile = true) =>
			ExecuteActionOnDatabase(document, document.Database.Filename, action, saveFile);

		// todo: lock document - how do we get a document from database?

		internal static void ExecuteActionInTransaction(Database db, Action<Transaction> action)
		{
			using (var tr = db.TransactionManager.StartTransaction())
			{
				try
				{
					action.Invoke(tr);
					//tr.TransactionManager.QueueForGraphicsFlush(); // ???
					tr.Commit();
				}
				catch (Exception ex)
				{
					// todo: logging
					System.Diagnostics.Debug.Print(ex.ToString());
					//LogHelper.GeneralError(ex);
					throw;
				}
			}
		}

		internal static void ExecuteActionOnBlockTable(Database db, Transaction tr, Action<BlockTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.BlockTableId, action);

		internal static void ExecuteActionOnLayerTable(Database db, Transaction tr, Action<LayerTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.LayerTableId, action);

		internal static void ExecuteActionOnDimStyleTable(Database db, Transaction tr, Action<DimStyleTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.DimStyleTableId, action);

		internal static void ExecuteActionOnLineTypeTable(Database db, Transaction tr, Action<LinetypeTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.LinetypeTableId, action);

		internal static void ExecuteActionOnRegAppTable(Database db, Transaction tr, Action<RegAppTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.RegAppTableId, action);

		internal static void ExecuteActionOnTextStyleTable(Database db, Transaction tr, Action<TextStyleTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.TextStyleTableId, action);

		internal static void ExecuteActionOnUcsTable(Database db, Transaction tr, Action<UcsTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.UcsTableId, action);

		internal static void ExecuteActionOnViewportTable(Database db, Transaction tr, Action<ViewportTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.ViewportTableId, action);

		internal static void ExecuteActionOnViewTable(Database db, Transaction tr, Action<ViewTable> action) =>
			ExecuteActionOnSymbolTable(db, tr, x => x.ViewTableId, action);

		internal static void ExecuteActionOnSymbolTable<T>(Database db, Transaction tr,
			Expression<Func<Database, ObjectId>> tableIdProperty, Action<T> action) where T : SymbolTable, IDisposable
		{
			var c = tableIdProperty.Compile();
			using (var t = c.Invoke(db).GetObject<T>())
			{
				action.Invoke(t);
			}
		}

		internal static void ExecuteActionOnBlockTable(Database db, Transaction tr, Action<Transaction, BlockTable> action)
		{
			using (var bt = db.BlockTableId.GetObject<BlockTable>())
			{
				action.Invoke(tr, bt);
			}
		}

		internal static void ExecuteActionOnLayerTable(Database db, Transaction tr, Action<Transaction, LayerTable> action)
		{
			using (var lt = db.LayerTableId.GetObject<LayerTable>())
			{
				action.Invoke(tr, lt);
			}
		}

		internal static void ExecuteActionOnBlockTable(string fileName, Action<Transaction, BlockTable> action,
			bool saveFile = true) =>
			ExecuteActionOnDatabase(fileName, db =>
				ExecuteActionInTransaction(db, tr =>
					ExecuteActionOnBlockTable(db, tr, bt => action.Invoke(tr, bt))), saveFile);

		internal static void ExecuteActionOnModelSpace(BlockTable bt, Action<BlockTableRecord> action)
		{
			using (var btrModelSpace = bt[BlockTableRecord.ModelSpace].GetObject<BlockTableRecord>())
			{
				action.Invoke(btrModelSpace);
			}
		}

		internal static void ExecuteActionOnModelSpace(Transaction tr, BlockTable bt, Action<Transaction, BlockTableRecord> action)
		{
			using (var btrModelSpace = bt[BlockTableRecord.ModelSpace].GetObject<BlockTableRecord>())
			{
				action.Invoke(tr, btrModelSpace);
			}
		}

		/// <summary>
		/// Executes an action on block table records matching the predicate, found in model space
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="btrModelSpace"></param>
		/// <param name="action"></param>
		/// <param name="predicate"></param>
		internal static void ExecuteActionOnItemsInModelSpace<TFilter>(Transaction tr, BlockTableRecord btrModelSpace,
			Action<TFilter> action, Predicate<TFilter> predicate)
			where TFilter : Autodesk.AutoCAD.DatabaseServices.Entity =>
				ExecuteActionOnItemsInBlockTableRecord(tr, btrModelSpace, action, predicate);

		/// <summary>
		/// Executes an action on block table records matching the predicate, found in model space
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="btr"></param>
		/// <param name="action"></param>
		/// <param name="predicate"></param>
		internal static void ExecuteActionOnItemsInBlockTableRecord<TFilter>(Transaction tr, BlockTableRecord btr, Action<TFilter> action, Predicate<TFilter> predicate)
			where TFilter : Autodesk.AutoCAD.DatabaseServices.Entity =>
			btr.ForEach<TFilter>(entity =>
			{
				if (IsBlockTableRecordNeeded(entity, predicate))
					action.Invoke(entity);
			});

		/// <summary>
		/// Get all block table records of a database. These are the block definitions
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="bt"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		internal static void ExecuteActionOnBlockTableRecords(BlockTable bt, Predicate<BlockTableRecord> predicate, Action<BlockTableRecord> action, bool executeOnAnonymousBlocks = true) =>
			bt.ForEach<BlockTableRecord>(btr =>
			{
				if (!predicate(btr)) return; // if item is not BTR, or it's a Layout => ignore
				action.Invoke(btr);
				if (executeOnAnonymousBlocks)
				{
					btr.GetAnonymousBlockIds().ForEach<BlockTableRecord>(abtr => action(abtr));
				}
			});

		/// <summary>
		/// Get all block references in a database. These are inserted blocks, "derived" from block table records (definitions).
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="bt"></param>
		/// <param name="action"></param>
		/// <param name="executeOnAnonymousBlocks"></param>
		/// <returns></returns>
		internal static void ExecuteActionOnBlockReferences(Transaction tr, BlockTable bt, Action<Transaction, BlockReference> action,
			OpenMode openMode = OpenMode.ForRead, bool executeOnAnonymousBlocks = true)
		{
			ExecuteActionOnBlockTableRecords(bt,
				btr => !btr.IsLayout,
				btr =>
				{
					// get block references normally
					ExecuteActionOnBlockReferences(tr, btr, (tran, br) => action.Invoke(tran, br), openMode);
					if (executeOnAnonymousBlocks)
						ExecuteActionOnAnonymousBlocks(tr, btr, (tran, br) => action.Invoke(tran, br), openMode);
				});
		}

		/// <summary>
		/// Executeas an action on all anonymous blocks of a block table record
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="btr"></param>
		/// <param name="action"></param>
		/// <param name="openMode"></param>
		private static void ExecuteActionOnAnonymousBlocks(Transaction tr, BlockTableRecord btr, Action<Transaction, BlockReference> action, OpenMode openMode = OpenMode.ForRead)
		{
			if (!btr.IsDynamicBlock) return; // nothing to do here

			// get anonymus blocks ...
			using (var abtrs = btr.GetAnonymousBlockIds())
			{
				abtrs.ForEach<BlockTableRecord>(abtr =>
				{
					// ... and through them, get block references
					ExecuteActionOnBlockReferences(tr, abtr, (tran, br) => action.Invoke(tran, br), openMode);
				});
			}
		}

		/// <summary>
		/// Executes an action on all block references of a block table record
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="btr"></param>
		/// <param name="action"></param>
		/// <param name="openMode"></param>
		private static void ExecuteActionOnBlockReferences(Transaction tr, BlockTableRecord btr, Action<Transaction, BlockReference> action, OpenMode openMode = OpenMode.ForRead)
		{
			// get block references normally
			using (var bris = btr.GetBlockReferenceIds(false, true)) // get all block references of a block table record
			{
				bris.ForEach<BlockReference>(br => action(tr, br), openMode);
			}
		}

		/// <summary>
		/// Get Bocks based on the blockName - it is not case sensitive
		/// </summary>
		/// <param name="db"></param>
		/// <param name="BlockName"></param>
		/// <returns></returns>
		public static IEnumerable<BlockReference> GetBlockByName(string blockName, Database db, BlockTableRecord modelSpace, OpenMode openMode = OpenMode.ForRead)
		{
			List<BlockReference> CollectedBlocks = new List<BlockReference>();
				// must collect objectIds, as the 2nd foreach will iterate on any new element - and we are creating a copy of every block, creating an infinite loop (the copy of the copy of the copy of the...)
				using (var objColl = new ObjectIdCollection())
				{
					foreach (var objectId in modelSpace)
					{
						objColl.Add(objectId);
					}

					objColl.ForEach<BlockReference>(br =>
					{
						if (blockName.ToUpper()== br.GetRealName().ToUpper()) CollectedBlocks.Add(br);

						//System.Diagnostics.Debug.Print($"AddAttributeToBlocks: {brName}");
						
						br.RecordGraphicsModified(true); // ???
					});
				}
			return CollectedBlocks;
	}



		/// <summary>
		/// Get all block references in a database. These are inserted blocks, "derived" from block table records (definitions).
		/// </summary>
		/// <param name="bt"></param>
		/// <param name="action"></param>
		/// <returns></returns>
		internal static void ExecuteActionOnBlockReferences(BlockTable bt, Action<BlockReference> action, OpenMode openMode = OpenMode.ForRead) =>
			ExecuteActionOnBlockReferences(null, bt, (tr, br) => action(br), openMode);

		/// <summary>
		/// Adjust attributes
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="br"></param>
		/// <remarks>All attributes must be adjusted if their alignment is not Middle Left</remarks>
		internal static void AdjustAttributeAlignments(Transaction tr, BlockReference br) =>
			AdjustAttributeAlignmentsInternal(tr, br);

		/// <summary>
		/// Removes unused layers
		/// </summary>
		/// <param name="doc"></param>
		/// <remarks>Only works when the command method is NOT in CommandFlags.Session</remarks>
		internal static void FixLayers(Database db)
		{
			ExecuteActionInTransaction(db, tr =>
			{
				ExecuteActionOnBlockTable(db, tr, bt =>
				{
					var layerNames = GetLayerNames(db);
					var unknownLayers = new List<string>();

					ExecuteActionOnBlockTableRecords(bt, btr => true, btr =>
					{
						FixLayerOfBlockTableRecord(btr, layerNames, unknownLayers);
					}, executeOnAnonymousBlocks: true);

					// log out layers not present in final drawing
					if (unknownLayers.Count > 0)
					{
						var aggr = unknownLayers.Aggregate((c, n) => c += $", {n}");
						System.Diagnostics.Debug.Print($"The following layers are not found in FixLayers: [{aggr}]");
					}
				});
			});

			PurgeAll(db);
		}

		private static void FixLayerOfBlockTableRecord(BlockTableRecord btr, Dictionary<string, ObjectId> layerNames, List<string> unknownLayers)
		{
			btr.ForEach<Autodesk.AutoCAD.DatabaseServices.Entity>(entity =>
			{
				FixLayerNameOfEntity(entity, layerNames, unknownLayers);
				if (entity is BlockReference br)
				{
					// BlockReferences have the list of attribute references - no other place store these, so we must handle block references differently
					FixLayerOfBlockReferenceAttributeRefernces(br, layerNames, unknownLayers);
				}
			}, OpenMode.ForWrite);
		}

		private static void FixLayerOfBlockReferenceAttributeRefernces(BlockReference br, Dictionary<string, ObjectId> layerNames, List<string> unknownLayers) =>
			ExecuteActionOnAttributeReferences(br, attRef => FixLayerNameOfEntity(attRef, layerNames, unknownLayers));

		/// <summary>
		/// Converts 'filename$0$layername' names into 'layername'
		/// </summary>
		/// <param name="entity"></param>
		/// <param name="layerNames"></param>
		/// <param name="unknownLayers"></param>
		internal static void FixLayerNameOfEntity(Autodesk.AutoCAD.DatabaseServices.Entity entity, Dictionary<string, ObjectId> layerNames, List<string> unknownLayers)
		{
			var layerNameParts = entity.Layer.Split('$');
			if (layerNameParts.Length == 1) return;
			var correctLayerName = layerNameParts.Last();
			if (unknownLayers.Contains(correctLayerName))
				return;
			//System.Diagnostics.Debug.WriteLine($"Fixing layer name of {entity.BlockName} from {entity.Layer} to {correctLayerName}");
			try
			{
				var newlayerid = layerNames[correctLayerName];
				entity.UpgradeOpen();
				entity.SetLayerId(newlayerid, true);
				entity.DowngradeOpen();
			}
			catch (KeyNotFoundException)
			{
				if (!unknownLayers.Contains(correctLayerName))
					unknownLayers.Add(correctLayerName);
			}
		}

		internal static void AttsyncByName(Document doc, string blockRealName) =>
			SendCommand(doc, "ATTSYNC", "n", blockRealName);

		internal static void SendCommand(Document doc, params string[] commandParts)
		{
			doc.Editor.Command(commandParts);
			System.Diagnostics.Debug.WriteLine("Executed: " + ConvertCommandPartsToCommandString(commandParts));
		}

		private static string ConvertCommandPartsToCommandString(params string[] commandParts)
		{
			const string partSeparator = " ";
			var commandWithSeparators = commandParts.ToList();
			for (var i = commandWithSeparators.Count - 2; i >= 0; --i)
			{
				commandWithSeparators.Insert(i + 1, partSeparator);
			}
			var commandString = commandWithSeparators.Aggregate((c, n) => c += n);
			return commandString;
		}

		/// <summary>
		/// Remove the extra helper line from valves
		/// </summary>
		/// <param name="db"></param>
		/// <remarks>If you are calling this from the GUI, and working on an opened document, you have to call
		/// Application.DocumentManager.MdiActiveDocument.Editor.Regen();
		/// to update the drawing. The lines will be fixed, but still need a Regen.
		/// </remarks>
		internal static void FixValveLines(Database db)
		{
			// guard
			var layerNames = GetLayerNames(db)
				.ToDictionary(x => x.Key.ToLower(), x => x.Value); // just converted all layer names to lowercase
			if (!layerNames.TryGetValue("valve2", out ObjectId valve2Id)) // lowercase !!!
			{
				System.Diagnostics.Debug.Print("No 'valve2' layer found in current drawing. Stopping.");
				return;
			}

			// action
			ExecuteActionInTransaction(db, tr =>
				ExecuteActionOnBlockTable(db, tr, bt =>
					ExecuteActionOnBlockReferences(tr, bt, (tran, br) =>
					{
						var layerName = br.Layer;
						if (layerName != "valve") return; // the specific line we are looking for is on the "valve" layer

						using (var btr = br.BlockTableRecord.GetObject<BlockTableRecord>()) // get the btr of this object
						{
							btr.ForEach<Line>(line =>
							{
								var lineLayerName = line.Layer;
								if (!lineLayerName.EndsWith("valve")) return; // ... on the "valve" layer (or in case of xRef blocks, the layer name will end with "valve")
								line.UpgradeOpen();
								line.SetLayerId(valve2Id, true);
								line.DowngradeOpen();
							});
						}
					})));
		}

		/// <summary>
		/// Collect all the blockreference in a DB based on the presence of the attribute where the tag is equal to "tagName"
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tr"></param>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static IEnumerable<BlockReference> SelectBlockReferenceByAttributeTag(Database db, Transaction tr, string tagName)
		{
			var result = new List<BlockReference>();
			int numberofBlocks = 0;
			var ids = GetIdsByTypeIteration(db, tr, typeof(BlockReference));
			foreach (var id in ids)
			{
				var blockreference = id.GetObject<BlockReference>();
				foreach (ObjectId attId in blockreference.AttributeCollection)
				{
					AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
					if (attRef.Tag == tagName.ToUpper())
					{
						numberofBlocks++;
						result.Add(blockreference);
						result.Distinct();
					}
				}
			}
			return result;
		}

		/// <summary>
		/// Collect given id based on C# into IEnumerables - http://exploitingautocad.blogspot.com/2015/01/getting-all-objects-in-modelspace-by.html
		/// </summary>
		/// <param name="types"></param>
		/// <returns></returns>
		public static IEnumerable<ObjectId> GetIdsByTypeIteration(Database db, Transaction tr, params Type[] types)
		{
			// We will use this Delegate to return the Class of the RXObject
			Func<Type, Autodesk.AutoCAD.Runtime.RXClass> getClass = Autodesk.AutoCAD.Runtime.RXObject.GetClass;

			// Make a HashSet of the Types of Entities we want to get from the modelspace.
			var acceptableTypes = new HashSet<Autodesk.AutoCAD.Runtime.RXClass>(types.Select(getClass));

			var resultList = new List<ObjectId>();

			using (var ms = (BlockTableRecord)tr
				.GetObject(SymbolUtilityServices.GetBlockModelSpaceId(db), OpenMode.ForRead))
			{
				// We use a little more linq to cast the IEnumerable object and iterate thru its ObjectIds
				// we will check our hashset contains the RXClass of the ObjectId by accessing the "ObjectClass" 
				// property of the RXObject.
				var ids = ms
					.Cast<ObjectId>()
					.Where(id => acceptableTypes.Contains(id.ObjectClass))
					.ToList();
				resultList.AddRange(ids);
			}
			using (var ps = (BlockTableRecord)tr
				.GetObject(SymbolUtilityServices.GetBlockPaperSpaceId(db), OpenMode.ForRead))
			{
				// We use a little more linq to cast the IEnumerable object and iterate thru its ObjectIds
				// we will check our hashset contains the RXClass of the ObjectId by accessing the "ObjectClass" 
				// property of the RXObject.
				var ids = ps
					.Cast<ObjectId>()
					.Where(id => acceptableTypes.Contains(id.ObjectClass))
					.ToList();
				resultList.AddRange(ids);
				return resultList;
			}
		}

		/// <summary>
		/// Set the attribute value to "attributeValue" of the block "br" based on the attribute tag "attributeTagName"
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tr"></param>
		/// <param name="br"></param>
		/// <param name="attributeTagName"></param>
		/// <param name="attributeValue"></param>
		/// <returns></returns>
		public static bool TryToSetAttribute(Database db, Transaction tr, BlockReference br, string attributeTagName, string attributeValue)
		{
			var result = false;
			using (var btr = br.BlockTableRecord.GetObject<BlockTableRecord>()) 
			{
				if (br != null)
				{

					BlockTableRecord bd = (BlockTableRecord)tr.GetObject(br.BlockTableRecord, OpenMode.ForRead);
					foreach (ObjectId arId in br.AttributeCollection)
					{
						var obj = tr.GetObject(arId, OpenMode.ForRead);
						var ar = obj as AttributeReference;

						if (ar != null)
						{
							if (ar.Tag.ToUpper() == attributeTagName)
							{
								ar.UpgradeOpen();
								ar.TextString = attributeValue;
								ar.DowngradeOpen();
								result = true;
							}
						}
					}
				}

			}
			return result;
		}

		/// <summary>
		/// //the function try to set the STB_DTY attributes to 1 for the blocks where the Note attribute of the autocad block is equal to blockNote
		/// </summary>
		/// <param name="db"></param>
		/// <param name="blockNote"></param>
		/// <param name="numberOfStandby"></param>
		/// <param name="isLeftToDownSelection"></param>
		/// <returns></returns>
		public static bool TryToSetStandByBasedonNoteAttribute(Database db, string blockNote, int numberOfStandby, bool isLeftToDownSelection)
		{
			var flag = true;
			Wrappers.ExecuteActionInTransaction(db, tr =>
			{
				//Filtering of the standby items (based on the numberOfStandby) from the blocks where  Note attribute of the autocad block is equal to blockNote
				var standByItems = SelectStandbyBasedonNoteAttribute(db, tr, blockNote, numberOfStandby, isLeftToDownSelection);

				foreach (var item in standByItems)
				{
					//Here is made the change of the attribute value
					//var newflag = TryToSetAttribute(db,tr, item, "STB_DTY", "0");
					//if (newflag) LogHelper.LogMessage($"{GetBlockAttributeValue(tr, item, "Tag")} set to standby");
					//flag &= newflag;
				}
			});
			return flag;
		}

		/// <summary>
		/// Based on the convenction the bottom rigth equipments will be set to standby
		/// There is two way of the setting based on the  isLeftToDownSelection
		/// in case of true the the right element and left before will be selected (sample x is selected, i not) 
		///																																				ooooooxx
		///																																				oooo
		/// in case of false the bottom and above items will be selected
		/// (sample x is selected, i not) 
		///																																				oooooooo
		///																																				ooxx
		/// </summary>
		/// <param name="db"></param>
		/// <param name="tr"></param>
		/// <param name="blockNote"></param>
		/// <param name="numberOfStandby"></param>
		/// <param name="isLeftToDownSelection"></param>
		/// <returns></returns>
		public static IEnumerable<BlockReference> SelectStandbyBasedonNoteAttribute(Database db, Transaction tr,
			string blockNote, int numberOfStandby, bool isLeftToDownSelection) =>
			SelectBlockReferenceByAttributeTag(db, tr, "Note")
				.Where(x => GetBlockAttributeValue(tr, x, "Note") == blockNote)
				.OrderBy(x => isLeftToDownSelection ? x.Position.Y : -(x.Position.X))
				.ThenBy(x => isLeftToDownSelection ? -(x.Position.X) : x.Position.Y)
				.Take(numberOfStandby)
			.ToList();

		/// <summary>
		/// Give back the attribute value as a string for the block "br" where the tag is "tagName"
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="br"></param>
		/// <param name="tagName"></param>
		/// <returns></returns>
		public static string GetBlockAttributeValue(Transaction tr, BlockReference br, string tagName)
		{
			string output = "not set value";
			if (br.AttributeCollection == null) return "No attributes are available";
			foreach (ObjectId attId in br.AttributeCollection)
			{
				AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);
				if (attRef.Tag == tagName.ToUpper())
				{
					output = attRef.TextString;
					break;
				}
			}
			if (output == "not set value")
			{
				throw new NoAttriburefound();
			}
			else
			{
				return output;
			}
		}

		private static Dictionary<string, ObjectId> GetLayerNames(Database db)
		{
			var layers = new Dictionary<string, ObjectId>();
			ExecuteActionInTransaction(db, tr =>
				ExecuteActionOnLayerTable(db, tr, lt =>
					lt.ForEach<LayerTableRecord>(ltr => layers.Add(ltr.Name, ltr.ObjectId))));
			return layers;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="tr"></param>
		/// <param name="br"></param>
		/// <see href="https://forums.autodesk.com/t5/net/attsync-in-vb-net/td-p/4645057"/>
		private static void AdjustAttributeAlignmentsInternal(Transaction tr, BlockReference br)
		{
			using (var btr = br.BlockTableRecord.GetObject<BlockTableRecord>()) // get the block definition of the block
			{
				ExecuteActionOnItemsInBlockTableRecord<AttributeDefinition>(tr, btr, attDef =>
				{
					try
					{
						//br.UpgradeOpen();

						ExecuteActionOnAttributeReferences(br, ar =>
					{
						if (!ar.ObjectId.IsValid()) return;
						if (attDef.Tag != ar.Tag) return; // "find" the same attribute reference we received from the caller
					if (!ar.Invisible)
						{
							var db = ar.Database;
						//System.Diagnostics.Debug.Print($"\t{br.Name} -> {ar.Tag}: {ar.TextString}");
						ExecuteActionOnDifferentWorkingDatabase(db, () => ar.AdjustAlignment(db));
						}
					}, OpenMode.ForWrite); // must open attributes for write, as we are modifying them with AdjustAlignment
																 //br.DowngradeOpen();
					}
					catch (Exception ex)
					{
						System.Diagnostics.Debug.Print($"{br.Name}.{attDef.Tag}\n{ex.ToString()}");
					}
				}, ad => true);
			}
		}

		internal static void ExecuteActionOnAttributeReferences(BlockReference br, Action<AttributeReference> action, OpenMode openMode = OpenMode.ForRead) =>
			br.ExecuteActionOnAttributeCollection(action, openMode);

		internal static void PurgeAll(Database db) =>
			ExecuteActionInTransaction(db, tr =>
			{
				var foundItems = true;
				while (foundItems)
				{
					foundItems = false;
					ExecuteActionOnBlockTable(db, tr, t => foundItems |= PurgeSymbolTable<BlockTable>(t));
					ExecuteActionOnDimStyleTable(db, tr, t => foundItems |= PurgeSymbolTable<DimStyleTable>(t));
					ExecuteActionOnLayerTable(db, tr, t => foundItems |= PurgeSymbolTable<LayerTable>(t));
					ExecuteActionOnLineTypeTable(db, tr, t => foundItems |= PurgeSymbolTable<LinetypeTable>(t));
					ExecuteActionOnRegAppTable(db, tr, t => foundItems |= PurgeSymbolTable<RegAppTable>(t));
					ExecuteActionOnTextStyleTable(db, tr, t => foundItems |= PurgeSymbolTable<TextStyleTable>(t));
					ExecuteActionOnUcsTable(db, tr, t => foundItems |= PurgeSymbolTable<UcsTable>(t));
					ExecuteActionOnViewportTable(db, tr, t => foundItems |= PurgeSymbolTable<ViewportTable>(t));
					ExecuteActionOnViewTable(db, tr, t => foundItems |= PurgeSymbolTable<ViewTable>(t));
				}
			});

		internal static bool PurgeSymbolTable<T>(SymbolTable st) where T : SymbolTable
		{
			using (var purgeIdCollection = new ObjectIdCollection())
			{
				//lets add all we want to check if purgable
				foreach (var objectId in st)
				{
					purgeIdCollection.Add(objectId);
				}
				//remove not purgable items
				st.Database.Purge(purgeIdCollection);

				purgeIdCollection.ForEach<SymbolTableRecord>(r => r.Erase(true), OpenMode.ForWrite);

				if (purgeIdCollection.Count > 0)
				{
					PurgeSymbolTable<T>(st);
					return true;
				}
				return false;
			}
		}

		internal static Point3d Clone(this Point3d source) =>
			new Point3d(source.X, source.Y, source.Z);

		internal static IEnumerable<DynamicBlockReferenceProperty> GetDynamicProperties(this BlockReference br)
		{
			// we are NOT yielding AutoCAD objects!
			var result = new List<DynamicBlockReferenceProperty>();
			var isDyn = br.IsDynamicBlock;
			if (!isDyn) return result;

			result.AddRange(br.DynamicBlockReferencePropertyCollection.Cast<DynamicBlockReferenceProperty>());
			return result;
		}

		///// <summary>
		///// Executes an action on every BlockTableRecord found in model space
		///// </summary>
		///// <param name="tr"></param>
		///// <param name="btrModelSpace"></param>
		///// <param name="action"></param>
		//public static void ExecuteActionOnItemsInModelSpace(Transaction tr, BlockTableRecord btrModelSpace,
		//  Action<BlockTableRecord> action) =>
		//  ExecuteActionOnItemsInModelSpace(tr, btrModelSpace, action, btr => true);

		private static bool IsBlockTableRecordNeeded<T>(T entity, Predicate<T> predicate) =>
			entity != null && predicate(entity);

		/// <summary>
		/// Saves current working database, executes action, sets back original working database
		/// </summary>
		/// <param name="newDatabase">New database to set during action</param>
		/// <param name="action">Action to execute</param>
		private static void ExecuteActionOnDifferentWorkingDatabase(Database newDatabase, Action action)
		{
			var currentWorkingDatabase = HostApplicationServices.WorkingDatabase;
			HostApplicationServices.WorkingDatabase = newDatabase;
			action.Invoke();
			HostApplicationServices.WorkingDatabase = currentWorkingDatabase;
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="bt"></param>
		/// <param name="tr"></param>
		/// <param name="modelSpace"></param>
		/// <param name="shouldAddAttribute"></param>
		/// <param name="attributeTag"></param>
		/// <param name="attributeDefaultValue"></param>
		/// <remarks></remarks>
		/// <see href="https://forums.autodesk.com/t5/net/how-to-use-a-block-properties-table-of-a-a-dynamic-block-using/td-p/3798548"/>
		public static void AddAttributeToBlocks(BlockTable bt, Transaction tr, BlockTableRecord modelSpace,
			Predicate<BlockReference> shouldAddAttribute, string attributeTag, string attributeDefaultValue)
		{
			// must collect objectIds, as the 2nd foreach will iterate on any new element - and we are creating a copy of every block, creating an infinite loop (the copy of the copy of the copy of the...)
			using (var objColl = new ObjectIdCollection())
			{
				foreach (var objectId in modelSpace)
				{
					objColl.Add(objectId);
				}

				objColl.ForEach<BlockReference>(br =>
				{
					if (!shouldAddAttribute(br)) return;
					var brName = br.GetRealName();

					//System.Diagnostics.Debug.Print($"AddAttributeToBlocks: {brName}");
					br.CreateAttributeDefinition(bt, tr, attributeTag, attributeDefaultValue);
					br.RecordGraphicsModified(true); // ???
				});
			}
		}

		public static void AddAttributeToBlocks2(BlockTable bt, Transaction tr,
			Predicate<BlockTableRecord> shouldAddAttribute, string attributeTag, string attributeDefaultValue)
		{
			foreach (var btrObjectId in bt)
			{
				using (var btr = btrObjectId.GetObject<BlockTableRecord>())
				{
					if (btr == null) continue;

					if (btr.Name.StartsWith("*Model_Space") || btr.Name.StartsWith("*Paper_Space"))
					{
						continue;
					}
					if (!shouldAddAttribute(btr))
					{
						//System.Diagnostics.Debug.Print($"\t'{btr.Name}' block does not meet predicate for attribute addition. Skipping.");
						continue;
					}
					if (AlreadyHasAttributeDefined(btr, attributeTag))
					{
						//System.Diagnostics.Debug.Print($"\t'{btr.Name}' block definition already has '{attributeTag}' attribute. Skipping.");
						continue;
					}
					if (!HasAttributes(btr))
					{
						//System.Diagnostics.Debug.Print($"\t'{btr.Name}' block definition does not have attributes. Skipping.");
						continue;
					}

					//if (btr.IsDynamicBlock)
					//{
					//  System.Diagnostics.Debug.Print($"'{btr.Name}' is a dynamic block table record.");
					//  continue;
					//}
					//if (btr.IsAnonymous)
					//{
					//  System.Diagnostics.Debug.Print($"\t'{btr.Name}' is an anonymus block table record. Skipping.");
					//  continue;
					//}

					using (var ad = new AttributeDefinition())
					{
						#region Add attr.def to blockDef.
						ad.Position = new Point3d(0.0d, 0.0d, 0.0d);
						ad.Verifiable = true;
						ad.Prompt = attributeTag;
						ad.Tag = attributeTag;
						ad.TextString = attributeDefaultValue;
						ad.Height = 1.0d;
						ad.Invisible = true; // should not be visible when inserted
						ad.Visible = false;

						btr.UpgradeOpen();
						btr.AppendEntity(ad);
						btr.DowngradeOpen();
						tr.AddNewlyCreatedDBObject(ad, true);
						btr.UpdateAnonymousBlocks();

						System.Diagnostics.Debug.Print($"\t'{attributeTag}' attribute added to '{btr.Name}' block definition.");
						#endregion

						#region Add attr. ref. to blockRefs

						var blockRefCounter = 0;
						var objectIdsToUpdate = new ObjectIdCollection();
						foreach (ObjectId anonymousBlockId in btr.GetAnonymousBlockIds())
						{
							objectIdsToUpdate.Add(anonymousBlockId);
						}
						foreach (ObjectId blockReferenceId in btr.GetBlockReferenceIds(true, true))
						{
							objectIdsToUpdate.Add(blockReferenceId);
						}

						// the inserted block references of a block definition are anonymus blocks (*Uxxxx)
						foreach (ObjectId anonymousBlockId in objectIdsToUpdate)
						{
							//System.Diagnostics.Debug.Print(anonymousBlockId.ObjectClass.Name);
							// btr.GetAnonymousBlockIds() returns block table records, which are the ObjectId "parent" of actual anonymus block
							using (var anonymBtr = anonymousBlockId.GetObject<BlockTableRecord>())
							{
								// once we retreive the actual block table record of the anonymus block, we must retreive it's block references ObjectIds ... 
								foreach (ObjectId brObjectId in anonymBtr.GetBlockReferenceIds(true, true))
								{
									//System.Diagnostics.Debug.Print(brObjectId.ObjectClass.Name);
									// ...  and convert the ObjectId to BlockReference
									using (var br = brObjectId.GetObject<BlockReference>())
									{
										if (br == null) continue;

										// make an AttributeReference, based on the AttributeDefinition (ATTSYNC)
										using (var ar = new AttributeReference())
										{
											ar.SetAttributeFromBlock(ad, br.BlockTransform);
											ar.TextString = attributeDefaultValue;
											br.UpgradeOpen();
											br.AttributeCollection.AppendAttribute(ar);
											br.DowngradeOpen();
											tr.AddNewlyCreatedDBObject(ar, true);

											blockRefCounter++;
										}
									}
								}
							}
						}

						System.Diagnostics.Debug.Print($"\tSynced attributes of {blockRefCounter} references.");
						#endregion
					}
				}
			}
		}

		private static void CreateAttributeDefinition(this BlockTableRecord btr, Transaction tr,
			string attributeTag, string attributeDefaultValue)
		{
			if (btr.AlreadyHasAttributeDefined(attributeTag)) return;
			if (!btr.ObjectId.IsValid()) return;

			#region Find the lowest, _invisible_ attribute and save it's position

			var lowestPoint = double.NaN;
			var left = double.NaN;
			var h = double.NaN;
			foreach (var adObjectId in btr)
			{
				using (var ad = adObjectId.GetObject<AttributeDefinition>())
				{
					if (ad == null) continue;
					if (!ad.Invisible) continue; // if it's not invisible -> skip

					if (double.IsNaN(lowestPoint) || lowestPoint > ad.AlignmentPoint.Y) // geometric coordinate system
					{
						lowestPoint = ad.AlignmentPoint.Y;
						left = ad.AlignmentPoint.X;
						h = ad.Height;
					}
				}
			}

			#endregion

			using (var ad = new AttributeDefinition())
			{
				// todo: magic number
				const double verticalOffset = 3.0d; // should be "h*something", but the usual distance is 3.0d...

				#region Add attr.def to blockDef.
				// all these settings are Organica block specific !!!!!!
				ad.Justify = AttachmentPoint.MiddleCenter;
				ad.AlignmentPoint = new Point3d(left, lowestPoint - verticalOffset, 0.0d); // this is not screen coordinate system, but geometrical (Y is increasing upward)
				ad.Verifiable = true;
				ad.Prompt = attributeTag;
				ad.Tag = attributeTag;
				ad.TextString = attributeDefaultValue;
				ad.Height = h;
				ad.Invisible = true; // should not be visible when inserted
				ad.Visible = true;
				#endregion

				btr.UpgradeOpen();
				btr.AppendEntity(ad);
				btr.DowngradeOpen();
				tr.AddNewlyCreatedDBObject(ad, true);

				//System.Diagnostics.Debug.Print($"\tAdding attribute '{attributeTag}' to blockTableRecord: {btr.Name}");
			}
		}

		/// <summary>
		/// Add an attribute to a block reference (and it's block "definition")
		/// </summary>
		/// <param name="br"></param>
		/// <param name="tr"></param>
		/// <param name="attributeTag"></param>
		/// <param name="attributeDefaultValue"></param>
		private static void CreateAttributeDefinition(this BlockReference br, BlockTable bt, Transaction tr,
			string attributeTag, string attributeDefaultValue)
		{
			// 1. add new attribute definition to block definition (if not present)
			using (var btr = br.DynamicBlockTableRecord.GetObject<BlockTableRecord>())
			{
				btr.CreateAttributeDefinition(tr, attributeTag, attributeDefaultValue);
			}

			// 2. insert a copy of the block reference, using the newly updated block definition
			br.CreateCopy(tr, bt);

			// 3. erase original block reference
			br.UpgradeOpen();
			br.Erase();
			br.DowngradeOpen();
		}

		private static void CreateCopy(this BlockReference original, Transaction tr, BlockTable bt)
		{
			using (var br = new BlockReference(new Point3d(original.Position.X, original.Position.Y, original.Position.Z), original.DynamicBlockTableRecord))
			{
				br.Rotation = original.Rotation;
				br.SetLayerId(original.LayerId, true);

				var originalValues = original.GetAttributeValues();

				using (var curSpace = bt.Database.CurrentSpaceId.GetObject<BlockTableRecord>(OpenMode.ForWrite))
				{
					curSpace.AppendEntity(br);
					tr.AddNewlyCreatedDBObject(br, true);
				}

				// copy dynamic property values (distance, visibility states, grips, etc.)
				br.CopyDynamicProperties(original);
				// copy non-dynamic property values
				br.AddAttributesFromDefinitionWithValues(originalValues, tr); // must be called after adding to transaction!
			}
		}

		private static void CopyDynamicProperties(this BlockReference br, BlockReference original)
		{
			if (!original.IsDynamicBlock) return;

			br.UpgradeOpen();
			foreach (DynamicBlockReferenceProperty originalDynProp in original.DynamicBlockReferencePropertyCollection)
			{
				if (originalDynProp.ReadOnly || originalDynProp.PropertyName == "Origin") continue;

				foreach (DynamicBlockReferenceProperty newDynProp in br.DynamicBlockReferencePropertyCollection)
				{
					if (originalDynProp.PropertyName == newDynProp.PropertyName)
					{
						if (newDynProp.ReadOnly) break;
						try
						{
							//System.Diagnostics.Debug.Print($"\tSetting dyn.prop. value of {br.GetRealName()} {originalDynProp.PropertyName}: {newDynProp.Value} to {originalDynProp.Value} ({originalDynProp.Description})");
							newDynProp.Value = originalDynProp.Value;
							//System.Diagnostics.Debug.Print($"\tNew dyn.prop. value of {br.GetRealName()} {originalDynProp.PropertyName}: {newDynProp.Value}");
						}
						catch (Exception)
						{
							System.Diagnostics.Debug.Print($"\tCouldn't set dyn.prop. value of {br.GetRealName()} {originalDynProp.PropertyName} {originalDynProp.Value}");
						}
						break;
					}
				}
			}
			br.DowngradeOpen();
			//System.Diagnostics.Debug.WriteLine("");
		}

		private static Dictionary<string, string> GetAttributeValues(this BlockReference br)
		{
			var result = new Dictionary<string, string>();
			br.ExecuteActionOnAttributeCollection(ar => result[ar.Tag] = ar.TextString);
			return result;
		}

		private static void DeleteAttributes(this BlockReference br)
		{
			br.ExecuteActionOnAttributeCollection(ar => ar.Erase(), OpenMode.ForWrite);
			System.Diagnostics.Debug.Print($"{br.GetRealName()} attribute collection count after erase: {br.AttributeCollection.Count}");
		}

		private static void AddAttributesFromDefinitionWithValues(this BlockReference br, Dictionary<string, string> attributeValues, Transaction tr)
		{
			// 2. add new attribute definition to block definition
			using (var btr = br.DynamicBlockTableRecord.GetObject<BlockTableRecord>())
			{
				foreach (var objectId in btr)
				{
					using (var ad = objectId.GetObject<AttributeDefinition>())
					{
						if (ad == null) continue;

						using (var ar = new AttributeReference())
						{
							ar.SetAttributeFromBlock(ad, br.BlockTransform);

							if (attributeValues.TryGetValue(ad.Tag, out string attributeValue))
							{
								ar.TextString = attributeValue;
							}

							//br.UpgradeOpen();
							br.AttributeCollection.AppendAttribute(ar);
							tr.AddNewlyCreatedDBObject(ar, true);
							//br.DowngradeOpen();
						}
					}
				}
			}
		}

		private static void ExecuteActionOnAttributeCollection(this BlockReference br, Action<AttributeReference> action, OpenMode openMode = OpenMode.ForRead) =>
			br.AttributeCollection.ForEach<AttributeReference>(ar => action.Invoke(ar), openMode);

		public static void FixAttrMover(BlockTableRecord btr) =>
			btr.ForEach<AttributeDefinition>(ad =>
			{
				var tag = ad.Tag.ToLower();
				var hideAttributes = new[] { "OCATTRMOVEDOWN", "OCATTRMOVEUP" }.Select(s => s.ToLower());
				if (hideAttributes.Contains(tag))
				{
					ad.UpgradeOpen();
					//ad.Visible = false;
					ad.Erase();
					ad.DowngradeOpen();
				}
			});

		//public static void RecalculatePowers(BlockReference br)
		//{
		//	var ci = new System.Globalization.CultureInfo("en-US");
		//	var brName = br.GetRealName();
		//	ObjectId arIPId = ObjectId.Null;
		//	ObjectId arCPId = ObjectId.Null;
		//	//BlockPowerData bpd = null;
		//	ExecuteActionOnAttributeReferences(br, ar =>
		//	{
		//		if (ar.Tag == "INSTALLED_POWER" || ar.Tag == "POWER_INSTALLED")
		//		{
		//			arIPId = ar.ObjectId;
		//			//bpd = new BlockPowerData(ar.TextString);
		//		}
		//		if (ar.Tag == "CONSUMED_POWER" || ar.Tag == "POWER_CONSUMED") arCPId = ar.ObjectId;
		//	});

		//	if (!arIPId.IsNull)
		//	{
		//		if (bpd.TrySelectMotor(bpd.InstalledPower, out double ratedPower))
		//		{
		//			using (var ar = arIPId.GetObject<AttributeReference>(OpenMode.ForWrite))
		//			{
		//				ar.TextString = ratedPower.ToString("N2", ci);
		//				//System.Diagnostics.Debug.Print($"\tratedPower = {ar.TextString}");
		//			}

		//			if (arCPId != null)
		//			{
		//				using (var ar = arCPId.GetObject<AttributeReference>(OpenMode.ForWrite))
		//				{
		//					var cp = bpd.ConsumedPower;
		//					if (!double.IsNaN(cp) && (ar.TextString == "0" || ar.TextString == "-"))
		//					{
		//						ar.TextString = cp.ToString("N2", ci);
		//						//System.Diagnostics.Debug.Print($"\tconsumedPower = {ar.TextString}");
		//					}
		//				}
		//			}
		//		}
		//	}
		//}

		//public static void CheckForErrors(Transaction tr, BlockTable bt)
		//{
		//	var installedPowerErrors = new List<string>();
		//	var consumedPowerErrors = new List<string>();
		//	ExecuteActionOnBlockReferences(tr, bt, (tran, br) =>
		//		ExecuteActionOnAttributeReferences(br, ar =>
		//		{
		//			if (!IsValidInstalledPower(br, ar, out string problematicBlockReferenceName)) installedPowerErrors.Add(problematicBlockReferenceName);
		//		}));

		//	if (installedPowerErrors.Count > 0)
		//	{
		//		System.Diagnostics.Debug.Print("DRAWING CONTAINS IP_POWER BUT IT IS SET TO - OR ZERO:\nThe following " + installedPowerErrors.Distinct().Aggregate((c, n) => c += $"\n{n}"));
		//		LogHelper.Warning("DRAWING CONTAINS IP_POWER BUT IT IS SET TO - OR ZERO:\nThe following {instlledpowererrors}", installedPowerErrors.Distinct().Aggregate((c, n) => c += $"\n{n}"));
		//	}
		//	if (consumedPowerErrors.Count > 0)
		//	{
		//		System.Diagnostics.Debug.Print("DRAWING CONTAINS CONSUMED_POWER BUT IT IS SET TO - OR ZERO:\nThe following " + consumedPowerErrors.Distinct().Aggregate((c, n) => c += $"\n{n}"));
		//		LogHelper.Warning("DRAWING CONTAINS CONSUMED_POWER BUT IT IS SET TO - OR ZERO:\nThe following {consumedpowererrors}", consumedPowerErrors.Distinct().Aggregate((c, n) => c += $"\n{n}"));
		//	}
		//}

		//private static bool IsValidInstalledPower(BlockReference br, AttributeReference ar, out string problematicBlockReferenceName)
		//{
		//	problematicBlockReferenceName = null;
		//	if (br.Layer.EndsWith("equipment") && br.Name.StartsWith("*") && (new[] { "INSTALLED_POWER", "POWER_INSTALLED" }).Contains(ar.Tag))
		//		if(!ar.TextString.TryConvertAcadAttributeToDouble(out double ip))
		//		{
		//		// block ref. is 
		//		// - on the equipment layer
		//		// - is anonymous (it's Name starts with *)
		//		// - has installed power tag, but it is empty (equals '-')
		//		problematicBlockReferenceName = br.GetRealName();
		//			return false;
		//		}
		//	return true;
		//}
		//private static bool IsValidConsumedPower(BlockReference br, AttributeReference ar, out string problematicBlockReferenceName)
		//{
		//	problematicBlockReferenceName = null;
		//	if (br.Layer.EndsWith("equipment") && br.Name.StartsWith("*") && (new[] { "CONSUMED_POWER", "POWER_CONSUMED" }).Contains(ar.Tag))
		//		if (!ar.TextString.TryConvertAcadAttributeToDouble(out double cp))
		//		{
		//			// block ref. is 
		//			// - on the equipment layer
		//			// - is anonymous (it's Name starts with *)
		//			// - has installed power tag, but it is empty (equals '-')
		//			problematicBlockReferenceName = br.GetRealName();
		//			return false;
		//		}
		//	return true;
		//}
		public static void BindxRefs(Database db, Transaction tr)
		{
			// ask AutoCAD to resolve xRefs
			db.ResolveXrefs(false, false); // true, false did not work. db.BindXrefs raised an exception
			var bindedBlockNames = new List<string>();
			using (var bindableObjectIdCollection = new ObjectIdCollection())
			{
				using (var xg = db.GetHostDwgXrefGraph(false))
				{
					var root = xg.RootNode;
					// collect items to bind
					for (var i = 0; i < root.NumOut; i++)
					{
						var child = root.Out(i) as XrefGraphNode;
						if (child == null) continue;
						if (child.XrefStatus != XrefStatus.Resolved) continue; // skip non-resolved xRefs
						bindedBlockNames.Add(child.Name);

						using (var collectionWithOneItem = new ObjectIdCollection())
						{
							collectionWithOneItem.Add(child.BlockTableRecordId);
							//System.Diagnostics.Debug.Print($"Binding {child.Name}");
							try
							{
								//  /*
								//    The BindXref method requires two parameters: xrefIds (collection of ObjectIDs) and insertBind (boolean). 
								//    If the insertBind parameter is set to True, the symbol names of the xref drawing are prefixed in the current 
								//    drawing with <blockname>$x$, where x is an integer that is automatically incremented to avoid overriding 
								//    existing block definitions. If the insertBind parameter is set to False, the symbol names of the xref 
								//    drawing are merged into the current drawing without the prefix. If duplicate names exist, AutoCAD uses 
								//    the symbols already defined in the local drawing. If you are unsure whether your drawing contains duplicate 
								//    symbol names, it is recommended that you set insertBind to True.
								//  */

								//  ACTUALLY, IT'S THE OPPOSITE OF THE OFFICIAL DOCUMENTATION !!!!

								//  // true  => originalBlockName (will use already existing blocks!!!)
								//  // false => <blockname>$x$<originalBlockName>

								db.BindXrefs(collectionWithOneItem, false);
							}
							catch (Exception ex)
							{
								System.Diagnostics.Debug.Print($"Exception raised while binding {child.Name}", ex.ToString());
							}
							foreach (ObjectId xRefId in collectionWithOneItem)
							{
								db.DetachXref(xRefId);
							}
						}
					}
				}
			}
		}

		//private static void ExplodeBlocksByName(Database db, Transaction tr, IEnumerable<string> blockNamesToExplode)
		//{
		//	ExecuteActionOnBlockTable(db, tr, bt =>
		//		ExecuteActionOnModelSpace(bt, ms =>
		//		{
		//			var explodeCounter = 0;
		//			var blockReferences = new List<BlockReference>();
		//			ms.ForEach<BlockReference>(br =>
		//			{
		//				var name = br.GetRealName();
		//				if (blockNamesToExplode.Contains(name))
		//				{
		//					blockReferences.Add(br);
		//				}
		//			});

		//			foreach (var br in blockReferences)
		//			{
		//				ExplodeBlockReference(br/*, bt[BlockTableRecord.ModelSpace], tr, x => false*/); // x.AttributeCollection.Count == 0
		//				explodeCounter++;
		//			}

		//			System.Diagnostics.Debug.Print($"Exploded {explodeCounter} entities in ExplodeBlocksWithName");
		//		}));
		//}

		//public static void ExplodeBlocksWithNoAttributes(Database db, Transaction tr) =>
		//	ExplodeBlocksByPredicate(db, tr, br => br.AttributeCollection.Count == 0);

		//public static void ExplodeBlocksByPredicate(Database db, Transaction tr, Predicate<BlockReference> predicate)
		//{
		//	ExecuteActionOnBlockTable(db, tr, bt =>
		//		ExecuteActionOnModelSpace(bt, ms =>
		//		{
		//			var explodeCounter = 0;
		//			ms.ForEach<BlockReference>(br =>
		//			{
		//				if (predicate(br))
		//				{
		//					ExplodeBlockReference(br);
		//					explodeCounter++;
		//				}
		//			});
		//			System.Diagnostics.Debug.Print($"Exploded {explodeCounter} entities in ExplodeBlocksWithNoAttributes");
		//		}));
		//}

		private static bool AlreadyHasAttributeDefined(this BlockTableRecord btr, string attributeTag)
		{
			var returnValue = false;
			try
			{
				returnValue = btr.ForEach<AttributeDefinition, bool>(ad => ad.Tag == attributeTag);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.Print($"\tError while checking '{btr.Name}' for existing {attributeTag}.", ex.ToString());
			}
			return false;
		}

		private static bool HasAttributes(BlockTableRecord btr) =>
			btr.ForEach<AttributeDefinition, bool>(ad => true);

		//private static void ExplodeBlockReference(BlockReference br/*, ObjectId currentSpaceId, Transaction tr, Predicate<BlockReference> shouldExplodeChildItem, int deep = 0*/)
		//{
		//	try
		//	{
		//		//System.Diagnostics.Debug.Print($"Exploding block reference {br.GetRealName()} (deep: {deep})...");
		//		//System.Diagnostics.Debug.Print($"Exploding block reference {br.GetRealName()}...");
		//		// this will work as expected, similar to the EXPLODE command in AutoCAD
		//		br.ExplodeToOwnerSpace();

		//		#region br.Explode(objectIdCollection)

		//		// the br.Explode(objectIdCollection) command would return the new items created, and you would have to append and and them manually
		//		// but the end result wouldn't keep dyanmic properties (visibility states, gripping, dimension changes, etc.)
		//		// so we are using ExplodeToOwnerSpace() which does not return the new items
		//		// on the other hand, once you explode new items into the database and you are in the middle of a "foreach (var br in modelSpace)", 
		//		// the new items will be iterated. (iterating an AutoCAD enumerable does not raise an exception when the list changes...)

		//		//using (var itemsAfterExplode = new DBObjectCollection())
		//		//{
		//		//  br.Explode(itemsAfterExplode);
		//		//}

		//		#endregion

		//		br.UpgradeOpen();
		//		br.Erase();
		//		br.DowngradeOpen();
		//	}
		//	catch (Exception ex)
		//	{
		//		System.Diagnostics.Debug.Print($"Could not explode block '{br.Name}'. Skipping.");
		//		//System.Diagnostics.Debug.WriteLine(ex.ToString());
		//	}
		//}

		private static void LoadLineTypes(Database db, LinetypeTable linetypeTable, LayerData layerData)
		{
			for (int i = 0; i < layerData.LoadLineTypes.Length; i++)
			{
				try
				{
					if (linetypeTable.Has(layerData.LoadLineTypes[i]) == false)
						db.LoadLineTypeFile(layerData.LoadLineTypes[i], "acadiso.lin");
				}
				catch (Autodesk.AutoCAD.Runtime.Exception ex)
				{
					if (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.KeyNotFound) throw new System.Exception($"LineType is already defined: {layerData.LoadLineTypes[i]}");

					else throw;
				}
			}
		}

		public static void CreateMandatoryLayers(Database db)
		{
			var layers = new[]
			{
				new LayerData("equipment", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 5), "Continuous"), // blue
        new LayerData("unit", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 3), "Continuous"), // red
        new LayerData("valve", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 7), "Continuous"), // white
        new LayerData("gas", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 3), "ACAD_ISO05W100"), // green
        new LayerData("instrumentation", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 4), "Continuous"), //cyan
        new LayerData("Text", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 7), "Continuous"), // white
        new LayerData("sewer", Autodesk.AutoCAD.Colors.Color.FromRgb(28, 38, 0), "Continuous"),
				new LayerData("sludge", Autodesk.AutoCAD.Colors.Color.FromRgb(38, 19, 19), "Continuous"),
				new LayerData("chemical", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 130), "HIDDEN"),
				new LayerData("chemicals", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 130), "HIDDEN"),
				new LayerData("biofilter", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 80), "Continuous"),
				new LayerData("DEFPOINTS", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 10), "Continuous"),
				new LayerData("hot water", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 5), "HIDDENX2"), // blue
        new LayerData("LEGEND", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 7), "Continuous"), // white
        new LayerData("water", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 150), "CENTER2"),
				new LayerData("TreatedWater", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 150), "CENTER2"),
				new LayerData("air", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 80), "ACAD_ISO11W100"),
				new LayerData("Leachate", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 63), "ACAD_ISO04W100"),
				new LayerData("effluent", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 5), "ACAD_ISO04W100"),
				new LayerData("potable water", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 150), "CENTER2"),
				new LayerData("Process_Unit_Limit", Autodesk.AutoCAD.Colors.Color.FromColorIndex(Autodesk.AutoCAD.Colors.ColorMethod.ByLayer, 5), "HIDDEN")
			};
			LayerCreatorEnumerate(db, layers);
		}

		private static void LayerCreatorEnumerate(Database db, IEnumerable<LayerData> layers)
		{
			foreach (var layer in layers)
			{
				LayerCreator(db, layer);
			}
		}

		private static void LayerCreator(Database db, LayerData layerData)
		{
			ExecuteActionInTransaction(db, tr =>
				ExecuteActionOnLayerTable(db, tr, lt =>
				{
					if (lt.Has(layerData.LayerName)) return;

					var layerTableRecord = new LayerTableRecord
					{
						Name = layerData.LayerName,
						Color = layerData.Color
					};

					if (lt.IsWriteEnabled == false) lt.UpgradeOpen();
					lt.Add(layerTableRecord);
					if (lt.IsWriteEnabled) lt.DowngradeOpen();
					tr.AddNewlyCreatedDBObject(layerTableRecord, true);

					//layerTableRecord.IsOff = isOff; // print visible turn off 

					ExecuteActionOnLineTypeTable(db, tr, ltt =>
				{
					if (ltt != null)
						LoadLineTypes(db, ltt, layerData);
					try
					{
						layerTableRecord.LinetypeObjectId = ltt[layerData.LineType];
					}
					catch (Autodesk.AutoCAD.Runtime.Exception ex)
					{
						if (ex.ErrorStatus == Autodesk.AutoCAD.Runtime.ErrorStatus.KeyNotFound) throw new System.Exception($"LineType not found: {layerData.LineType}");

						else throw;
					}
				});
				}));
		}

		private class LayerData
		{
			public string[] LoadLineTypes = new string[]
			{
				 "Continuous",
				 "HIDDEN",
				 "HIDDENX2",
				 "CENTER2",
				 "CENTER",
				 "ACAD_ISO11W100",
				 "ACAD_ISO05W100",
				 "ACAD_ISO04W100"
			};
			public string LayerName { get; set; }
			public Autodesk.AutoCAD.Colors.Color Color { get; set; }
			public string LineType { get; set; }

			public LayerData(string layerName, Autodesk.AutoCAD.Colors.Color color, string lineType)
			{
				LayerName = layerName;
				Color = color;
				LineType = lineType;
			}
		}
	}

	internal static class AutoCADSpecificEnumeratorExtensions
	{
		public static void ForEach<TEntity>(this System.Collections.IEnumerable enumerable,
			Action<TEntity> action, OpenMode openMode = OpenMode.ForRead) where TEntity : class, IDisposable
		{
			foreach (var objectId in enumerable.Cast<ObjectId>())
			{
				//if (!objectId.IsValid()) continue;
				using (var entity = objectId.GetObject<TEntity>(openMode))
				{
					if (entity == null) continue;
					try
					{
						action(entity);
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.Print("Exception while iterating an AutoCAD enumerable", ex.ToString());
					}
				}
			}
		}

		public static T ForEach<TEntity, T>(this System.Collections.IEnumerable enumerable,
			Func<TEntity, T> action, OpenMode openMode = OpenMode.ForRead) where TEntity : class, IDisposable
		{
			foreach (var objectId in enumerable.Cast<ObjectId>())
			{
				//if (!objectId.IsValid()) continue;
				using (var entity = objectId.GetObject<TEntity>(openMode))
				{
					if (entity == null) continue;
					try
					{
						return action(entity);
					}
					catch (System.Exception ex)
					{
						System.Diagnostics.Debug.Print("Exception while iterating an AutoCAD enumerable", ex.ToString());
					}
				}
			}
			return default(T);
		}
	}

	public static class ObjectIdExtensions
	{
		public static T GetObject<T>(this ObjectId objectId, OpenMode openMode = OpenMode.ForRead)
			where T : class
		{
			try
			{
				var returnValue = objectId.GetObject(openMode) as T;
				return returnValue;
			}
			catch (NullReferenceException ex)
			{
				//System.Diagnostics.Debug.Print($"NullReferenceException ignored while retrieving object from objectId. '{objectId.ObjectClass.DxfName}' to <{typeof(T).Name}>\n{ex.ToString()}");
				return null; // this might not be the best return value in this case, but we should always check for nulls anyways
			}
		}

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

		/// <summary>
		/// Collection of validation rules for an ObjectId
		/// </summary>
		/// <param name="objectId"></param>
		/// <returns></returns>
		public static bool IsValid(this ObjectId objectId) => !objectId.IsNull && !objectId.IsErased && objectId.IsValid && !objectId.IsEffectivelyErased;

		public static object GetVisiblityStates(this BlockReference br, string propertyName)
		{
			if (br == null) throw new ArgumentNullException(nameof(br));

			if (br.IsDynamicBlock)
			{
				foreach (DynamicBlockReferenceProperty prop in br.DynamicBlockReferencePropertyCollection)
				{
					if (prop.PropertyName == propertyName) return prop.Value;
				}
			}
			return null; // should we return something else here?
		}

	}
	[Serializable]
	internal class NoAttriburefound : System.Exception
	{
		public NoAttriburefound()
		{
		}

		public NoAttriburefound(string message) : base(message)
		{
		}

		public NoAttriburefound(string message, System.Exception innerException) : base(message, innerException)
		{
		}

		//protected NoAttriburefound(SerializationInfo info, StreamingContext context) : base(info, context)
		//{
		//}
	}
}


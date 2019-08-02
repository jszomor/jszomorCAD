using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;

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

    internal static void ExecuteActionOnDatabase(string fileName, Action<Database> action, bool saveFile = true)
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
          throw;
        }
      }
    }

    internal static void ExecuteActionOnBlockTable(Database db, Transaction tr, Action<BlockTable> action) =>
      ExecuteActionOnTable(db, tr, x => x.BlockTableId, action);

    internal static void ExecuteActionOnLayerTable(Database db, Transaction tr, Action<LayerTable> action) =>
      ExecuteActionOnTable(db, tr, x => x.LayerTableId, action);

    internal static void ExecuteActionOnTable<T>(Database db, Transaction tr,
      Expression<Func<Database, ObjectId>> tableIdProperty, Action<T> action) where T : class, IDisposable
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
      using (var lt = db.BlockTableId.GetObject<LayerTable>())
      {
        action.Invoke(tr, lt);
      }
    }

    internal static void ExecuteActionOnBlockTable(string fileName, Action<Transaction, BlockTable> action,
      bool saveFile = true) =>
      ExecuteActionOnDatabase(fileName, db =>
        ExecuteActionInTransaction(db, tr =>
          ExecuteActionOnBlockTable(db, tr, bt => action.Invoke(tr, bt))), saveFile);

    internal static void ExecuteActionOnModelSpace(Transaction tr, BlockTable bt, Action<BlockTableRecord> action)
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
      where TFilter : Entity =>
      ExecuteActionOnItemsInBlockTableRecord(tr, btrModelSpace, action, predicate);

    /// <summary>
    /// Executes an action on block table records matching the predicate, found in model space
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="btr"></param>
    /// <param name="action"></param>
    /// <param name="predicate"></param>
    internal static void ExecuteActionOnItemsInBlockTableRecord<TFilter>(Transaction tr, BlockTableRecord btr, Action<TFilter> action, Predicate<TFilter> predicate)
      where TFilter : Entity
    {
      foreach (var objectId in btr)
      {
        using (var entity = objectId.GetObject<TFilter>())
        {
          if (IsBlockTableRecordNeeded(entity, predicate))
            action.Invoke(entity);
        }
      }
    }

    /// <summary>
    /// Get all block table records of a database. These are the block definitions
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="bt"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    internal static void ExecuteActionOnBlockTableRecords(Transaction tr, BlockTable bt, Action<BlockTableRecord> action)
    {
      foreach (var objectId in bt)
      {
        if (objectId.IsErased || objectId.IsEffectivelyErased || objectId.IsNull || !objectId.IsValid)
        {
          continue;
        }

        var btr = objectId.GetObject<BlockTableRecord>();
        if (btr == null || btr.IsLayout) continue; // if item is not BTR, or it's a Layout => ignore

        action.Invoke(btr);
      }
    }

    /// <summary>
    /// Get all block references in a database. These are inserted blocks, "derived" from block table records (definitions).
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="bt"></param>
    /// <param name="action"></param>
    /// <returns></returns>
    internal static void ExecuteActionOnBlockReferences(Transaction tr, BlockTable bt, Action<Transaction, BlockReference> action, OpenMode openMode = OpenMode.ForRead)
    {
      ExecuteActionOnBlockTableRecords(tr, bt, btr =>
      {
        var bris = btr.GetBlockReferenceIds(false, true); // get all block references of a block table record
        foreach (ObjectId bri in bris) // must specify type
        {
          try
          {
            var blockReference = tr.GetObject(bri, openMode) as BlockReference; // bri.GetObject<BlockReference>(openMode);
            if (blockReference == null) continue; // if item is not a block reference => ignore
            //if (blockReference.AnonymousBlockTableRecord.OldIdPtr.ToInt64() == 0) continue; // these are not visible on the drawing... (for example: a block named "valve", and not "*UXXX")

            action(tr, blockReference);
          }
          catch (Exception ex)
          {
            System.Diagnostics.Debug.Print(ex.ToString());
            //throw;
          }
        }
      });
    }

    /// <summary>
    /// Adjust attributes
    /// </summary>
    /// <param name="tr"></param>
    /// <param name="br"></param>
    /// <remarks>All attributes must be adjusted if their alignment is not Middle Left</remarks>
    internal static void AdjustAttributeAlignments(Transaction tr, BlockReference br)
    {
      AdjustAttributeAlignmentsInternal(tr, br);

      var btr = br.BlockTableRecord.GetObject<BlockTableRecord>(); // get the block definition of the block
      if (btr.IsDynamicBlock)
      {
        foreach (ObjectId id in btr.GetAnonymousBlockIds())
        {
          var abtr = id.GetObject<BlockTableRecord>();
          foreach (ObjectId abrId in abtr.GetBlockReferenceIds(true, false))
          {
            var abr = abrId.GetObject<BlockReference>(OpenMode.ForWrite);
            AdjustAttributeAlignmentsInternal(tr, abr);
          }
        }
      }
    }

    internal static void FixLayers(Database db)
    {
      var layerNames = GetLayerNames(db);
      var unknownLayers = new List<string>();
      ExecuteActionInTransaction(db, tr =>
        ExecuteActionOnBlockTable(db, tr, bt =>
          ExecuteActionOnBlockReferences(tr, bt, (tran, br) =>
          {
            var layerNameParts = br.Layer.Split('$');
            if (layerNameParts.Length == 1) return;

            var correctLayerName = layerNameParts.Last();
            if (unknownLayers.Contains(correctLayerName))
              return;
            try
            {
              var newLayerId = layerNames[correctLayerName];
              br.UpgradeOpen();
              br.SetLayerId(newLayerId, true);
              br.DowngradeOpen();
            }
            catch (KeyNotFoundException)
            {
              if (!unknownLayers.Contains(correctLayerName))
                unknownLayers.Add(correctLayerName);
            }
          })));

      // log out layers not present in final drawing
      if (unknownLayers.Count > 0)
      {
        var aggr = unknownLayers.Aggregate((c, n) => c += $", {n}");
        System.Diagnostics.Debug.Print($"The following layers are not found in FixLayers: [{aggr}]");
      }
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
      var layerNames = GetLayerNames(db);
      if (!layerNames.TryGetValue("VALVE2", out ObjectId valve2Id))
      {
        System.Diagnostics.Debug.Print("No VALVE2 layer found in current drawing. Stopping.");
        return;
      }

      ExecuteActionInTransaction(db, tr =>
        ExecuteActionOnBlockTable(db, tr, bt =>
          ExecuteActionOnBlockReferences(tr, bt, (tran, br) =>
          {
            var layerName = br.Layer;
            if (layerName != "valve") return; // the specific line we are looking for is on the "valve" layer

            var btr = br.BlockTableRecord.GetObject<BlockTableRecord>(); // get the btr of this object
            foreach (var objectId in btr)
            {
              var objectClassName = objectId.ObjectClass.Name;
              if (objectClassName != "AcDbLine") continue; // we are only interested in lines ...
              var line = objectId.GetObject<Line>();
              var lineLayerName = line.Layer;
              if (!lineLayerName.EndsWith("valve")) continue; // ... on the "valve" layer (or in case of xRef blocks, the layer name will end with "valve")
              line.UpgradeOpen();
              line.SetLayerId(valve2Id, true);
              line.DowngradeOpen();
            }
          })));
    }

    private static Dictionary<string, ObjectId> GetLayerNames(Database db)
    {
      var layers = new Dictionary<string, ObjectId>();
      ExecuteActionInTransaction(db, tr =>
        ExecuteActionOnLayerTable(db, tr, lt =>
        {
          foreach (var layerObjectId in lt)
          {
            using (var ltr = layerObjectId.GetObject<LayerTableRecord>())
            {
              layers.Add(ltr.Name, ltr.ObjectId);
            }
          }
        }));
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
      var btr = br.BlockTableRecord.GetObject<BlockTableRecord>(); // get the block definition of the block
      ExecuteActionOnItemsInBlockTableRecord<AttributeDefinition>(tr, btr, attDef =>
      {
        try
        {
          br.UpgradeOpen();

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
          }, OpenMode.ForWrite);
          br.DowngradeOpen();
        }
        catch (Exception ex)
        {
          System.Diagnostics.Debug.Print($"{br.Name}.{attDef.Tag}\n{ex.ToString()}");
        }
      }, ad => true);
    }

    internal static void ExecuteActionOnAttributeReferences(BlockReference br, Action<AttributeReference> action, OpenMode openMode = OpenMode.ForRead) => br.
      ExecuteActionOnAttributeCollection(action);

    internal static string GetRealName(this BlockReference br)
    {
      var brName = br.Name;

      try
      {
        var isDyn = br.IsDynamicBlock;
        if (isDyn && !br.DynamicBlockTableRecord.IsNull)
        {
          var dynBtr = br.DynamicBlockTableRecord.GetObject<BlockTableRecord>();
          if (dynBtr != null)
          {
            brName = dynBtr.Name;
          }
        }
      }
      catch (Exception)
      {
        System.Diagnostics.Debug.Print($"Error in GetRealName {brName}");
      }

      return brName;
    }

    internal static void PurgeAll(Database db, BlockTable bt)
    {
      var purgeIdCollection = new ObjectIdCollection();

      //lets add all we want to check if purgable
      foreach (var objectId in bt)
      {
        purgeIdCollection.Add(objectId);
      }
      //remove not purgable items
      db.Purge(purgeIdCollection);

      foreach (ObjectId acObjId in purgeIdCollection)
      {
        using (var acSymTblRec = acObjId.GetObject(OpenMode.ForWrite) as SymbolTableRecord)
        {
          if (acSymTblRec == null) continue;
          acSymTblRec.Erase(true);
        }
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
      var objColl = new ObjectIdCollection();
      foreach (var objectId in modelSpace)
      {
        objColl.Add(objectId);
      }

      foreach (ObjectId objectId in objColl) // must use a "pre-processed" list of objectIds, and not the modelSpace directly!
      {
        using (var br = objectId.GetObject<BlockReference>(OpenMode.ForRead))
        {
          if (br == null) continue;
          if (!shouldAddAttribute(br)) continue;
          var brName = br.GetRealName();

          //System.Diagnostics.Debug.Print($"AddAttributeToBlocks: {brName}");
          br.CreateAttributeDefinition(bt, tr, attributeTag, attributeDefaultValue);
          br.RecordGraphicsModified(true); // ???
        }
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
              System.Diagnostics.Debug.Print(anonymousBlockId.ObjectClass.Name);
              // btr.GetAnonymousBlockIds() returns block table records, which are the ObjectId "parent" of actual anonymus block
              using (var anonymBtr = anonymousBlockId.GetObject<BlockTableRecord>())
              {
                // once we retreive the actual block table record of the anonymus block, we must retreive it's block references ObjectIds ... 
                foreach (ObjectId brObjectId in anonymBtr.GetBlockReferenceIds(true, true))
                {
                  System.Diagnostics.Debug.Print(brObjectId.ObjectClass.Name);
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
      br.ExecuteActionOnAttributeCollection(ar => ar.Erase());
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

    private static void ExecuteActionOnAttributeCollection(this BlockReference br, Action<AttributeReference> action)
    {
      br.UpgradeOpen();
      foreach (ObjectId objectId in br.AttributeCollection)
      {
        if (!objectId.IsValid()) continue;
        using (var ar = objectId.GetObject<AttributeReference>(OpenMode.ForWrite))
        {
          if (ar == null) continue;
          action.Invoke(ar);
        }
      }
      br.DowngradeOpen();
    }

    public static void FixAttrMover(BlockTableRecord btr)
    {
      foreach (var objectId in btr)
      {
        using (var ad = objectId.GetObject<AttributeDefinition>())
        {
          if (ad == null) continue;

          var tag = ad.Tag.ToLower();
          var hideAttributes = new[] { "OCATTRMOVEDOWN", "OCATTRMOVEUP" }.Select(s => s.ToLower());
          if (hideAttributes.Contains(tag))
          {
            ad.UpgradeOpen();
            //ad.Visible = false;
            ad.Erase();
            ad.DowngradeOpen();
          }
        }
      }
    }

    public static void CheckForErrors(Transaction tr, BlockTable bt)
    {
      var installedPowerErrors = new List<string>();
      ExecuteActionOnBlockReferences(tr, bt, (tran, br) =>
      {
        foreach (ObjectId attrObjectId in br.AttributeCollection)
        {
          using (var ar = attrObjectId.GetObject<AttributeReference>())
          {
            if (!ar.Visible) continue;
            if (br.Layer.EndsWith("equipment") && br.Name.StartsWith("*") && (new[] { "INSTALLED_POWER", "POWER_INSTALLED" }).Contains(ar.Tag) && ar.TextString == "-")
            {
              var brName = br.GetRealName();
              installedPowerErrors.Add(brName);
            }
          }
        }
      });

      if (installedPowerErrors.Count > 0)
      {
        System.Diagnostics.Debug.Print("MISSING INSTALLED POWER ERRORS:\nThe following " + installedPowerErrors.Distinct().Aggregate((c, n) => c += $"\n{n}"));
      }
        
    }

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

                //  ACTUALLY, IT'S THE OPPOSITE OF THE OFFICAIL DOCUMENTATION !!!!

                //  // true  => originalBlockName (will use already existing blocks!!!)
                //  // false => <blockname>$x$<originalBlockName>

                db.BindXrefs(collectionWithOneItem, false);
              }
              catch (Exception ex)
              {
                System.Diagnostics.Debug.Print($"Exception raised while binding {child.Name}\n{ex.ToString()}");
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

    private static void ExplodeBlocksByName(Database db, Transaction tr, IEnumerable<string> blockNamesToExplode)
    {
      ExecuteActionOnBlockTable(db, tr, bt =>
        ExecuteActionOnModelSpace(tr, bt, ms =>
        {
          var explodeCounter = 0;
          var blockReferences = new List<BlockReference>();
          foreach (var objectId in ms)
          {
            var br = objectId.GetObject<BlockReference>();
            if (br == null) continue;

            var name = br.GetRealName();
            if (blockNamesToExplode.Contains(name))
            {
              blockReferences.Add(br);
            }
          }

          foreach (var br in blockReferences)
          {
            ExplodeBlockReference(br/*, bt[BlockTableRecord.ModelSpace], tr, x => false*/); // x.AttributeCollection.Count == 0
            explodeCounter++;
          }

          System.Diagnostics.Debug.Print($"Exploded {explodeCounter} entities in ExplodeBlocksWithName");
        }));
    }

    public static void ExplodeBlocksWithNoAttributes(Database db, Transaction tr) =>
      ExplodeBlocksByPredicate(db, tr, br => br.AttributeCollection.Count == 0);

    public static void ExplodeBlocksByPredicate(Database db, Transaction tr, Predicate<BlockReference> predicate)
    {
      ExecuteActionOnBlockTable(db, tr, bt =>
        ExecuteActionOnModelSpace(tr, bt, ms =>
        {
          var explodeCounter = 0;
          //Predicate<BlockReference> predicate = br => { return br.AttributeCollection.Count == 0; };

          foreach (var objectId in ms)
          {
            var br = objectId.GetObject<BlockReference>();
            if (br == null) continue;

            if (predicate(br))
            {
              ExplodeBlockReference(br);
              explodeCounter++;
            }
          }
          System.Diagnostics.Debug.Print($"Exploded {explodeCounter} entities in ExplodeBlocksWithNoAttributes");
        }));
    }

    private static bool AlreadyHasAttributeDefined(this BlockTableRecord btr, string attributeTag)
    {
      try
      {
        foreach (var adObjectId in btr)
        {
          using (var ad = adObjectId.GetObject<AttributeDefinition>())
          {
            if (ad == null) continue;

            if (ad.Tag == attributeTag)
            {
              return true;
            }
          }
        }
      }
      catch (System.Exception ex)
      {
        System.Diagnostics.Debug.Print(ex.ToString());
        System.Diagnostics.Debug.Print($"\tError while checking '{btr.Name}' for existing {attributeTag}.");
      }
      return false;
    }

    private static bool HasAttributes(BlockTableRecord btr)
    {
      foreach (var adObjectId in btr)
      {
        using (var ad = adObjectId.GetObject<AttributeDefinition>())
        {
          if (ad == null) continue;

          return true;
        }
      }
      return false;
    }

    private static void ExplodeBlockReference(BlockReference br/*, ObjectId currentSpaceId, Transaction tr, Predicate<BlockReference> shouldExplodeChildItem, int deep = 0*/)
    {
      //System.Diagnostics.Debug.Print($"Exploding block reference {br.GetRealName()} (deep: {deep})...");
      //System.Diagnostics.Debug.Print($"Exploding block reference {br.GetRealName()}...");
      // this will work as expected, similar to the EXPLODE command in AutoCAD
      br.ExplodeToOwnerSpace();

      #region br.Explode(objectIdCollection)
      
      // the br.Explode(objectIdCollection) command would return the new items created, and you would have to append and and them manually
      // but the end result wouldn't keep dyanmic properties (visibility states, gripping, dimension changes, etc.)
      // so we are using ExplodeToOwnerSpace() which does not return the new items
      // on the other hand, once you explode new items into the database and you are in the middle of a "foreach (var br in modelSpace)", 
      // the new items will be iterated. (iterating an AutoCAD enumerable does not raise an exception when the list changes...)

      //using (var itemsAfterExplode = new DBObjectCollection())
      //{
      //  br.Explode(itemsAfterExplode);
      //}

      #endregion

      br.UpgradeOpen();
      br.Erase();
      br.DowngradeOpen();
    }

    private static bool IsValid(this ObjectId objectId) => !objectId.IsNull && !objectId.IsErased && objectId.IsValid;
  }

  internal static class ObjectIdCollectionExtensions
  {
    public static IEnumerable<T> Select<T>(this ObjectIdCollection source,
      Func<ObjectId, T> selector)
    {
      for (var i = 0; i < source.Count; i++)
        yield return selector(source[i]);
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
        System.Diagnostics.Debug.Print($"NullReferenceException ignored while retrieving object from objectId. '{objectId.ObjectClass.DxfName}' to <{typeof(T).Name}>\n{ex.ToString()}");
        return null; // this might not be the best return value in this case, but we should always check for nulls anyways
      }
    }
  }
}

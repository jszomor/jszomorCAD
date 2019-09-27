using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using AcRx = Autodesk.AutoCAD.Runtime;
using OrganiCAD.AutoCAD;

namespace jszomorCAD
{
  #region code from net
  //  class Attsync
  //  {
  //    [CommandMethod("MySynch", CommandFlags.Modal)]
  //    public void MySynch()
  //    {
  //      Document dwg = Application.DocumentManager.MdiActiveDocument;
  //      Editor ed = dwg.Editor;
  //      Database db = dwg.Database;
  //      TypedValue[] types = new TypedValue[] { new TypedValue((int)DxfCode.Start, "INSERT") };
  //      SelectionFilter sf = new SelectionFilter(types);
  //      PromptSelectionOptions pso = new PromptSelectionOptions();
  //      pso.MessageForAdding = "Select block reference";
  //      pso.SingleOnly = true;
  //      pso.AllowDuplicates = false;

  //      PromptSelectionResult psr = ed.GetSelection(pso, sf);
  //      if (psr.Status == PromptStatus.OK)
  //      {
  //        using (Transaction t = db.TransactionManager.StartTransaction())
  //        {
  //          BlockTable bt = (BlockTable)t.GetObject(db.BlockTableId, OpenMode.ForRead);
  //          BlockReference br = (BlockReference)t.GetObject(psr.Value[0].ObjectId, OpenMode.ForRead);
  //          BlockTableRecord btr = (BlockTableRecord)t.GetObject(bt[br.Name], OpenMode.ForRead);
  //          btr.AttSync(t, false, true, false);
  //          t.Commit();
  //        }
  //      }
  //      else
  //        ed.WriteMessage("Bad selectionа.\n");
  //    }
  //  }


  //  public static class ExtensionMethods
  //  {
  //    public static void SynchronizeAttributes(this BlockTableRecord target)
  //    {
  //      if (target == null)
  //        throw new ArgumentNullException("btr");

  //      using (Transaction tr = target.Database.TransactionManager.TopTransaction)
  //      {
  //        if (tr == null)
  //          throw new AcRx.Exception(ErrorStatus.NoActiveTransactions);

  //        RXClass attDefClass = RXClass.GetClass(typeof(AttributeDefinition));
  //        List<AttributeDefinition> attDefs = new List<AttributeDefinition>();
  //        foreach (ObjectId id in target)
  //        {
  //          if (id.ObjectClass == attDefClass)
  //          {
  //            AttributeDefinition attDef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
  //            attDefs.Add(attDef);
  //          }
  //        }

  //        foreach (ObjectId id in target.GetBlockReferenceIds(true, false))
  //        {
  //          BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForWrite);
  //          br.ResetAttributes(attDefs);
  //        }

  //        if (target.IsDynamicBlock)
  //        {
  //          foreach (ObjectId id in target.GetAnonymousBlockIds())
  //          {
  //            BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
  //            foreach (ObjectId brId in btr.GetBlockReferenceIds(true, false))
  //            {
  //              BlockReference br = (BlockReference)tr.GetObject(brId, OpenMode.ForWrite);
  //              br.ResetAttributes(attDefs);
  //            }
  //          }
  //        }
  //        tr.Commit();
  //      }
  //    }

  //    private static void ResetAttributes(this BlockReference br, List<AttributeDefinition> attDefs)
  //    {
  //      Autodesk.AutoCAD.ApplicationServices.TransactionManager tm = br.Database.TransactionManager;
  //      Dictionary<string, string> attValues = new Dictionary<string, string>();
  //      foreach (ObjectId id in br.AttributeCollection)
  //      {
  //        if (!id.IsErased)
  //        {
  //          AttributeReference attRef = (AttributeReference)tm.GetObject(id, OpenMode.ForWrite);
  //          attValues.Add(attRef.Tag, attRef.TextString);
  //          attRef.Erase();
  //        }
  //      }
  //      foreach (AttributeDefinition attDef in attDefs)
  //      {
  //        AttributeReference attRef = new AttributeReference();
  //        attRef.SetAttributeFromBlock(attDef, br.BlockTransform);
  //        if (attValues.ContainsKey(attDef.Tag))
  //        {
  //          attRef.TextString = attValues[attDef.Tag.ToUpper()];
  //        }
  //        br.AttributeCollection.AppendAttribute(attRef);
  //        tm.AddNewlyCreatedDBObject(attRef, true);
  //      }
  //    }
  //  }


  //  /// <summary>
  //  /// When modifying a drawing database, it is very important to control which database is current.
  //  /// Class <c> WorkingDatabaseSwitcher </c>
  //  /// takes control to ensure that the current database is needed.
  //  /// </summary>
  //  /// <example>
  //  /// Example of using the class:
  //  /// <code>
  //  /// // db - Database object
  //  /// using (WorkingDatabaseSwitcher hlp = new WorkingDatabaseSwitcher (db)) {
  //  /// // here is our code </code>
  //  ///} </example>
  //  public sealed class WorkingDatabaseSwitcher : IDisposable
  //  {
  //    private Database prevDb = null;

  //    /// <summary>
  //    /// The database in the context of which the work is to be performed. This database temporarily becomes current.
  //    /// Upon completion of the work, the base that was before it will become the current one.
  //    /// </summary>
  //    /// <param name = "db"> Database to be installed current </param>
  //    public WorkingDatabaseSwitcher(Database db)
  //    {
  //      prevDb = HostApplicationServices.WorkingDatabase;
  //      HostApplicationServices.WorkingDatabase = db;
  //    }

  //    /// <summary>
  //    /// Return the <c> HostApplicationServices.WorkingDatabase </c> property to its previous value
  //    /// </summary>
  //    public void Dispose()
  //    {
  //      HostApplicationServices.WorkingDatabase = prevDb;
  //    }
  //  }


  //  //Let's move on to the next step (note that using WorkingDatabaseSwitcher allows us not to keep in mind the moment that we need to control which database is working) ...



  //  /// <summary>
  //  /// Extension methods for objects of the Autodesk.AutoCAD.DatabaseServices.BlockTableRecord class
  //  /// </summary>
  //  public static class BlockTableRecordExtensionMethods
  //  {
  //    /// <summary>
  //    /// Synchronization of occurrences of blocks with their definition
  //    /// </summary>
  //    /// <param name = "btr"> Block table entry accepted as block definition </param>
  //    /// <param name = "directOnly"> Whether to search only at the top level, or whether
  //    /// analyze and nested occurrences, i.e. whether to process a block recursively in a block:
  //    /// true - only the top; false - recursively check nested blocks. </param>
  //    /// <param name = "removeSuperfluous">
  //    /// Whether to delete extra attributes in the block entries (those that are not in the block definition). </param>
  //    /// <param name = "setAttDefValues">
  //    /// Should all attributes in the block occurrences be set to the default value by the current value. </param>
  //    public static void AttSync(this BlockTableRecord btr, bool directOnly, bool removeSuperfluous, bool setAttDefValues)
  //    {
  //      Database db = btr.Database;
  //      using (WorkingDatabaseSwitcher wdb = new WorkingDatabaseSwitcher(db))
  //      {
  //        using (Transaction t = db.TransactionManager.StartTransaction())
  //        {
  //          BlockTable bt = (BlockTable)t.GetObject(db.BlockTableId, OpenMode.ForRead);

  //          // Get all attribute definitions from the block definition
  //          IEnumerable<AttributeDefinition> attdefs = btr.Cast<ObjectId>()
  //                    .Where(n => n.ObjectClass.Name == "AcDbAttributeDefinition")
  //                    .Select(n => (AttributeDefinition)t.GetObject(n, OpenMode.ForRead))
  //                    .Where(n => !n.Constant); // Exclude constant attributes since AttributeReference is not created for them.

  //          // In a loop, iterate over all occurrences of the desired block definition
  //          foreach (ObjectId brId in btr.GetBlockReferenceIds(directOnly, false))
  //          {
  //            BlockReference br = (BlockReference)t.GetObject(brId, OpenMode.ForWrite);

  //            // Check the names for compliance. In the event that the occurrence of block "A" is nested in the definition of block "B",
  //            // then the occurrences of block "B" will also fall into the selection. We need to exclude them from the set of processed objects.
  //            // - this is why we check the names.
  //            if (br.Name != btr.Name) continue;

  //            // Get all block entry attributes
  //            IEnumerable<AttributeReference> attrefs = br.AttributeCollection.Cast<ObjectId>()
  //                .Select(n => (AttributeReference)t.GetObject(n, OpenMode.ForWrite));

  //            // Tags for existing attribute definitions
  //            IEnumerable<string> dtags = attdefs.Select(n => n.Tag);
  //            // Tags of existing attributes in the entry
  //            IEnumerable<string> rtags = attrefs.Select(n => n.Tag);

  //            // If required, delete those attributes for which there is no definition
  //            // as part of the block definition
  //            if (removeSuperfluous)
  //            {
  //              foreach (AttributeReference attref in attrefs.Where(n => rtags

  //                  .Except(dtags).Contains(n.Tag)))
  //                attref.Erase(true);
  //            }

  //            // We synchronize the properties of existing attributes with the properties of their definitions
  //            foreach (AttributeReference attref in attrefs.Where(n => dtags.Join(rtags, a => a, b => b, (a, b) => a).Contains(n.Tag)))
  //            {
  //              AttributeDefinition ad = attdefs.First(n => n.Tag == attref.Tag);

  //              // The SetAttributeFromBlock method, which we use later in the code, resets
  //              // current value of the multi-line attribute. Therefore, we remember this value,
  //              // to restore it immediately after calling SetAttributeFromBlock.
  //              string value = attref.TextString;
  //              attref.SetAttributeFromBlock(ad, br.BlockTransform);
  //              // Restore the attribute value
  //              attref.TextString = value;

  //              if (attref.IsMTextAttribute)
  //              {

  //              }

  //              // If required, set the attribute to its default value
  //              if (setAttDefValues)
  //              {
  //                attref.TextString = ad.TextString;
  //                attref.AdjustAlignment(db);
  //              }                
  //            }

  //            // If the required attributes are not in the block entry, create them
  //            IEnumerable<AttributeDefinition> attdefsNew = attdefs.Where(n => dtags.Except(rtags).Contains(n.Tag));

  //            foreach (AttributeDefinition ad in attdefsNew)
  //            {
  //              AttributeReference attref = new AttributeReference();
  //              attref.SetAttributeFromBlock(ad, br.BlockTransform);
  //              attref.AdjustAlignment(db);
  //              br.AttributeCollection.AppendAttribute(attref);
  //              t.AddNewlyCreatedDBObject(attref, true);
  //            }
  //          }
  //          btr.UpdateAnonymousBlocks();
  //          t.Commit();
  //        }
  //        // If it is a dynamic block
  //        if (btr.IsDynamicBlock)
  //        {
  //          using (Transaction t = db.TransactionManager.StartTransaction())
  //          {
  //            foreach (ObjectId id in btr.GetAnonymousBlockIds())
  //            {
  //              BlockTableRecord _btr = (BlockTableRecord)t.GetObject(id, OpenMode.ForWrite);

  //              // Get all attribute definitions from the original block definition
  //              IEnumerable<AttributeDefinition> attdefs = btr.Cast<ObjectId>()
  //                           .Where(n => n.ObjectClass.Name == "AcDbAttributeDefinition")
  //                           .Select(n => (AttributeDefinition)t.GetObject(n, OpenMode.ForRead));

  //              // Get all attribute definitions from the anonymous block definition
  //              IEnumerable<AttributeDefinition> attdefs2 = _btr.Cast<ObjectId>()
  //                              .Where(n => n.ObjectClass.Name == "AcDbAttributeDefinition")
  //                              .Select(n => (AttributeDefinition)t.GetObject(n, OpenMode.ForWrite));

  //              // Anonymous block attribute definitions should be synchronized
  //              // with attribute definitions of the main block

  //              // Tags for existing attribute definitions
  //              IEnumerable<string> dtags = attdefs.Select(n => n.Tag);
  //              IEnumerable<string> dtags2 = attdefs2.Select(n => n.Tag);

  //              // 1. Remove unnecessary
  //              foreach (AttributeDefinition attdef in attdefs2.Where(n => !Dtags.Contains(n.Tag)))
  //              {
  //                attdef.Erase(true);
  //              }

  //              // 2. Sync existing
  //              foreach (AttributeDefinition attdef in attdefs.Where(n => dtags
  //                  .Join(dtags2, a => a, b => b, (a, b) => a).Contains(n.Tag)))
  //              {
  //                AttributeDefinition ad = attdefs2.First(n => n.Tag == attdef.Tag);
  //                ad.Position = attdef.Position;
  //                ad.TextStyleId = attdef.TextStyleId;
  //                ad.TextString = attdef.TextString;
  //                // If required, set the attribute to default value
  //                if (setAttDefValues)
  //                  ad.TextString = attdef.TextString;

  //                ad.Tag = attdef.Tag;
  //                ad.Prompt = attdef.Prompt;

  //                ad.LayerId = attdef.LayerId;
  //                ad.Rotation = attdef.Rotation;
  //                ad.LinetypeId = attdef.LinetypeId;
  //                ad.LineWeight = attdef.LineWeight;
  //                ad.LinetypeScale = attdef.LinetypeScale;
  //                ad.Annotative = attdef.Annotative;
  //                ad.Color = attdef.Color;
  //                ad.Height = attdef.Height;
  //                ad.HorizontalMode = attdef.HorizontalMode;
  //                ad.Invisible = attdef.Invisible;
  //                ad.IsMirroredInX = attdef.IsMirroredInX;
  //                ad.IsMirroredInY = attdef.IsMirroredInY;
  //                ad.Justify = attdef.Justify;
  //                ad.LockPositionInBlock = attdef.LockPositionInBlock;
  //                ad.MaterialId = attdef.MaterialId;
  //                ad.Oblique = attdef.Oblique;
  //                ad.Thickness = attdef.Thickness;
  //                ad.Transparency = attdef.Transparency;
  //                ad.VerticalMode = attdef.VerticalMode;
  //                ad.Visible = attdef.Visible;
  //                ad.WidthFactor = attdef.WidthFactor;

  //                ad.CastShadows = attdef.CastShadows;
  //                ad.Constant = attdef.Constant;
  //                ad.FieldLength = attdef.FieldLength;
  //                ad.ForceAnnoAllVisible = attdef.ForceAnnoAllVisible;
  //                ad.Preset = attdef.Preset;
  //                ad.Prompt = attdef.Prompt;
  //                ad.Verifiable = attdef.Verifiable;

  //                ad.AdjustAlignment(db);
  //              }

  //              // 3. Add the missing
  //              foreach (AttributeDefinition attdef in attdefs.Where(n => !Dtags2.Contains(n.Tag)))
  //              {
  //                AttributeDefinition ad = new AttributeDefinition();
  //                ad.SetDatabaseDefaults();
  //                ad.Position = attdef.Position;
  //                ad.TextStyleId = attdef.TextStyleId;
  //                ad.TextString = attdef.TextString;
  //                ad.Tag = attdef.Tag;
  //                ad.Prompt = attdef.Prompt;

  //                ad.LayerId = attdef.LayerId;
  //                ad.Rotation = attdef.Rotation;
  //                ad.LinetypeId = attdef.LinetypeId;
  //                ad.LineWeight = attdef.LineWeight;
  //                ad.LinetypeScale = attdef.LinetypeScale;
  //                ad.Annotative = attdef.Annotative;
  //                ad.Color = attdef.Color;
  //                ad.Height = attdef.Height;
  //                ad.HorizontalMode = attdef.HorizontalMode;
  //                ad.Invisible = attdef.Invisible;
  //                ad.IsMirroredInX = attdef.IsMirroredInX;
  //                ad.IsMirroredInY = attdef.IsMirroredInY;
  //                ad.Justify = attdef.Justify;
  //                ad.LockPositionInBlock = attdef.LockPositionInBlock;
  //                ad.MaterialId = attdef.MaterialId;
  //                ad.Oblique = attdef.Oblique;
  //                ad.Thickness = attdef.Thickness;
  //                ad.Transparency = attdef.Transparency;
  //                ad.VerticalMode = attdef.VerticalMode;
  //                ad.Visible = attdef.Visible;
  //                ad.WidthFactor = attdef.WidthFactor;

  //                ad.CastShadows = attdef.CastShadows;
  //                ad.Constant = attdef.Constant;
  //                ad.FieldLength = attdef.FieldLength;
  //                ad.ForceAnnoAllVisible = attdef.ForceAnnoAllVisible;
  //                ad.Preset = attdef.Preset;
  //                ad.Prompt = attdef.Prompt;
  //                ad.Verifiable = attdef.Verifiable;

  //                _btr.AppendEntity(ad);
  //                t.AddNewlyCreatedDBObject(ad, true);
  //                ad.AdjustAlignment(db);
  //              }
  //              // Synchronize all occurrences of this anonymous block definition
  //              _btr.AttSync(directOnly, removeSuperfluous, setAttDefValues);
  //            }
  //            // Update the geometry of the definitions of anonymous blocks derived from
  //            // of this dynamic block
  //            btr.UpdateAnonymousBlocks();
  //            t.Commit();
  //          }
  //        }
  //      }
  //    }
  //  }
  #endregion

  public static class SendClass
  {
    [CommandMethod("SendRegen", CommandFlags.Modal)]
    public static void SendRegen()
    {
      Document acDoc = Application.DocumentManager.MdiActiveDocument;

      acDoc.SendStringToExecute("_regen" + " ", true, false, false);
    }

    #region attsync (not work)
    public static void SendSync(Editor ed, string blockName, Database db)
    {
      using (Transaction acTrans = db.TransactionManager.StartTransaction())
      {
        var blockIds = new List<ObjectId>();

        using (var bt = db.BlockTableId.GetObject<BlockTable>(OpenMode.ForRead))
        {
          // Open the Block table record Model space for write
          using (var btrModelSpace = acTrans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord)
          {
            foreach (var btrId in btrModelSpace)
            {
              var item = btrId.GetObject(OpenMode.ForRead);
              if (item == null) continue;
              if (item is BlockReference)
              {
                var attrDef = item as BlockReference;

                if (attrDef.IsErased || attrDef.IsDisposed)
                {
                  continue;
                }
                var dynBtr = acTrans.GetObject(attrDef.DynamicBlockTableRecord, OpenMode.ForRead, false) as BlockTableRecord;

                System.Diagnostics.Debug.Print("DynBlockName: " + dynBtr.Name);

                if (!dynBtr.IsAnonymous && !dynBtr.IsLayout && dynBtr.Name == blockName)
                  blockIds.Add(btrId);
              }
            }

            var first = blockIds.First();
            var acEnt = acTrans.GetObject(first, OpenMode.ForWrite) as DBObject;

            ed.Command("_attsync", "\n", first, "\n"); // does not work, I have no idea why.

            if (blockIds.Count == 0) ed.WriteMessage($"No block record found with the name {blockName}");

          }
        }
      }
    }
    #endregion

    [CommandMethod("SendZoomExtents", CommandFlags.Modal)]
    public static void SendZoomExtents()
    {
      Document acDoc = Application.DocumentManager.MdiActiveDocument;

      acDoc.SendStringToExecute("_zoom _extents" + " ", true, false, false);
    }

    public static ObjectId ToModelSpace(Entity ent)
    {
      Database db = HostApplicationServices.WorkingDatabase;
      ObjectId endId;
      using (Transaction trans = db.TransactionManager.StartTransaction())
      {
        BlockTable bt = (BlockTable)trans.GetObject(db.BlockTableId, OpenMode.ForRead);
        BlockTableRecord btr = (BlockTableRecord)trans.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForWrite);
        endId = btr.AppendEntity(ent);
        trans.AddNewlyCreatedDBObject(ent, true);
        trans.Commit();
      }
      return endId;
    }

    public static void FilletCommand(Editor ed, Database db, Line l1, Line l2)
    {
      // Start a transaction
      using (Transaction acTrans = db.TransactionManager.StartTransaction())
      {
        // Open the selected object for write
        Entity acEnt1 = acTrans.GetObject(l1.ObjectId, OpenMode.ForWrite) as Entity;

        // Open the selected object for write
        Entity acEnt2 = acTrans.GetObject(l2.ObjectId, OpenMode.ForWrite) as Entity;

        ed.Command("_fillet", "_R", 0);
        ed.Command("_fillet", acEnt1.ObjectId, acEnt2.ObjectId);

        // Save the new object to the database
        acTrans.Commit();
      }
      // Dispose of the transaction  
    }
  }
}

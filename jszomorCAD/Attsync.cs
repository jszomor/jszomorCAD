using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AcRx = Autodesk.AutoCAD.Runtime;

namespace jszomorCAD
{
  class Attsync
  {
    [CommandMethod("MySynch", CommandFlags.Modal)]
    public void MySynch()
    {
      Document dwg = Application.DocumentManager.MdiActiveDocument;
      Editor ed = dwg.Editor;
      Database db = dwg.Database;
      TypedValue[] types = new TypedValue[] { new TypedValue((int)DxfCode.Start, "INSERT") };
      SelectionFilter sf = new SelectionFilter(types);
      PromptSelectionOptions pso = new PromptSelectionOptions();
      pso.MessageForAdding = "Select block reference";
      pso.SingleOnly = true;
      pso.AllowDuplicates = false;

      PromptSelectionResult psr = ed.GetSelection(pso, sf);
      if (psr.Status == PromptStatus.OK)
      {
        using (Transaction t = db.TransactionManager.StartTransaction())
        {
          BlockTable bt = (BlockTable)t.GetObject(db.BlockTableId, OpenMode.ForRead);
          BlockReference br = (BlockReference)t.GetObject(psr.Value[0].ObjectId, OpenMode.ForRead);
          BlockTableRecord btr = (BlockTableRecord)t.GetObject(bt[br.Name], OpenMode.ForRead);
          btr.AttSync(t, false, true, false);
          t.Commit();
        }
      }
      else
        ed.WriteMessage("Bad selectionа.\n");
    }
  }


  public static class ExtensionMethods
  {
    public static void SynchronizeAttributes(this BlockTableRecord target)
    {
      if (target == null)
        throw new ArgumentNullException("btr");

      Transaction tr = target.Database.TransactionManager.TopTransaction;
      if (tr == null)
        throw new AcRx.Exception(ErrorStatus.NoActiveTransactions);

      RXClass attDefClass = RXClass.GetClass(typeof(AttributeDefinition));
      List<AttributeDefinition> attDefs = new List<AttributeDefinition>();
      foreach (ObjectId id in target)
      {
        if (id.ObjectClass == attDefClass)
        {
          AttributeDefinition attDef = (AttributeDefinition)tr.GetObject(id, OpenMode.ForRead);
          attDefs.Add(attDef);
        }
      }

      foreach (ObjectId id in target.GetBlockReferenceIds(true, false))
      {
        BlockReference br = (BlockReference)tr.GetObject(id, OpenMode.ForWrite);
        br.ResetAttributes(attDefs);
      }

      if (target.IsDynamicBlock)
      {
        foreach (ObjectId id in target.GetAnonymousBlockIds())
        {
          BlockTableRecord btr = (BlockTableRecord)tr.GetObject(id, OpenMode.ForRead);
          foreach (ObjectId brId in btr.GetBlockReferenceIds(true, false))
          {
            BlockReference br = (BlockReference)tr.GetObject(brId, OpenMode.ForWrite);
            br.ResetAttributes(attDefs);
          }
        }
      }
    }

    private static void ResetAttributes(this BlockReference br, List<AttributeDefinition> attDefs)
    {
      Autodesk.AutoCAD.ApplicationServices.TransactionManager tm = br.Database.TransactionManager;
      Dictionary<string, string> attValues = new Dictionary<string, string>();
      foreach (ObjectId id in br.AttributeCollection)
      {
        if (!id.IsErased)
        {
          AttributeReference attRef = (AttributeReference)tm.GetObject(id, OpenMode.ForWrite);
          attValues.Add(attRef.Tag, attRef.TextString);
          attRef.Erase();
        }
      }
      foreach (AttributeDefinition attDef in attDefs)
      {
        AttributeReference attRef = new AttributeReference();
        attRef.SetAttributeFromBlock(attDef, br.BlockTransform);
        if (attValues.ContainsKey(attDef.Tag))
        {
          attRef.TextString = attValues[attDef.Tag.ToUpper()];
        }
        br.AttributeCollection.AppendAttribute(attRef);
        tm.AddNewlyCreatedDBObject(attRef, true);
      }
    }
  }  
}

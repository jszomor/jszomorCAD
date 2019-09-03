using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace jszomorCAD
{
  public static class MoveToBottom
  {
    //Filter selection and movetobottom for all line and polyline
    [CommandMethod("SendToBackLine")]
    public static void Execute()
    {
      // Get the current document editor
      //Editor acDocEd = Application.DocumentManager.MdiActiveDocument.Editor;
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      // Create a TypedValue array to define the filter criteria
      var filterItems = new List<TypedValue>
      {
        new TypedValue((int)DxfCode.Operator, "<OR"),
        new TypedValue((int)DxfCode.Start, "LINE"),
        new TypedValue((int)DxfCode.Start, "LWPOLYLINE"),
        //new TypedValue((int)DxfCode.Start, "WIPEOUT"),
        new TypedValue((int)DxfCode.Operator, "OR>")
      };

      // Assign the filter criteria to a SelectionFilter object
      SelectionFilter acSelFtr = new SelectionFilter(filterItems.ToArray());

      // Request for objects to be selected in the drawing area                    
      //PromptSelectionResult acSSPrompt = acDoc.Editor.GetSelection(acSelFtr);


      //Autoselection by filer criteria       
      PromptSelectionResult acSSPrompt = acDoc.Editor.SelectAll(acSelFtr);

      if (acSSPrompt.Status != PromptStatus.OK)
      {
        Application.ShowAlertDialog("Number of objects selected: 0");
        return;
      }

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        SelectionSet acSSet = acSSPrompt.Value;
        var objToMove = new ObjectIdCollection();

        foreach (SelectedObject acSSObj in acSSet)
        {
          ObjectId selectedObjectId = acSSObj.ObjectId;
          objToMove.Add(selectedObjectId);
        }

        SendToBack(acCurDb, acTrans, objToMove);

        acTrans.Commit();

      }
    }

    [CommandMethod("SendToBackWipeout")]
    public static void SendToBackWipeout()
    {
      Document acDoc = Application.DocumentManager.MdiActiveDocument;
      Database acCurDb = acDoc.Database;

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        var blockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
        foreach (var objectId in blockTable)
        {
          using (var btr = objectId.GetObject(OpenMode.ForRead) as BlockTableRecord)
          {
            System.Diagnostics.Debug.Print(btr.Name);
            ParseBlockTableRecord(btr);
          }
        }

        acTrans.Commit();
      }
    }

    private static void ParseBlockTableRecord(BlockTableRecord btr)
    {
      foreach (var objectId in btr)
      {
        //System.Diagnostics.Debug.Print("\t" + objectId.ObjectClass.Name);
        using (var wipeout = objectId.GetObject(OpenMode.ForRead) as Wipeout)
        {
          if (wipeout == null) continue;

          using (var drawOrderTable = btr.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable)
          {
            drawOrderTable.MoveToBottom(new ObjectIdCollection { objectId });
            //System.Diagnostics.Debug.Print("\tMoveToBottom");
          }
        }
      }      
    }

    private static void SendToBack(Database acCurDb, Transaction acTrans, ObjectIdCollection objToMove)
    {
      // get the BlockTable (which contains all elements in the current drawings database)
      var blockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;

      // fetch the model space element from block table
      var btrModelSpace = acTrans.GetObject(blockTable[BlockTableRecord.ModelSpace], OpenMode.ForRead)
          as BlockTableRecord;

      // fetch the "draw order table" element from Model Space
      var drawOrderTable = acTrans.GetObject(btrModelSpace.DrawOrderTableId, OpenMode.ForWrite) as DrawOrderTable;

      // call method on draw order table
      drawOrderTable.MoveToBottom(objToMove);
    }
  }
}

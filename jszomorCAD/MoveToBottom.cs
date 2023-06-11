﻿using Autodesk.AutoCAD.ApplicationServices;
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
        [CommandMethod("JCAD_SendToBackLine")]
        public static void SendToBackLine()
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

        [CommandMethod("JCAD_MoveBackWipesInBlockEditor")]
        public static void SendToBackBlock()
        {
            Document acDoc = Application.DocumentManager.MdiActiveDocument;
            Database acCurDb = acDoc.Database;

            using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
            {
                using (var blockTable = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable)
                {
                    // so called BlockDefinitions...
                    foreach (var btrObjectId in blockTable)
                    {
                        using (var btr = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord)
                        {
                            if (btr == null) continue;

                            SendWipeoutToBack(btr);
                        }
                    }
                }
                acTrans.Commit();
            }
        }

        private static void SendWipeoutToBack(BlockTableRecord btr)
        {
            using (var wipeoutCollection = new ObjectIdCollection())
            {
                var foundWipeout = false;
                foreach (var objectId in btr)
                {
                    if (objectId.IsNull) continue;
                    var wipeout = objectId.GetObject(OpenMode.ForRead) as Wipeout;
                    if (wipeout == null) continue;

                    wipeoutCollection.Add(objectId);
                    foundWipeout = true;
                }

                if (!foundWipeout) return;

                // found wipeout
                using (var drawOrderTable = btr.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable)
                {
                    drawOrderTable.MoveToBottom(wipeoutCollection);
                }

                //foreach (ObjectId id in wipeoutCollection)
                //{
                //  using (var w = id.GetObject(OpenMode.ForWrite) as Wipeout)
                //  {
                //    w.Erase(true);
                //  }
                //}
                btr.UpdateAnonymousBlocks();
            }

            //var anonymBlocks = btr.GetAnonymousBlockIds();
            //foreach (ObjectId anonymObjectId in anonymBlocks)
            //{
            //  using (var anonymBtr = anonymObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord)
            //  {
            //    if (anonymBtr == null) continue;
            //    SendWipeoutToBack(anonymBtr);
            //  }
            //}

            //var blockRefs = btr.GetBlockReferenceIds(true, false);
            //foreach (ObjectId brObjectId in blockRefs)
            //{
            //  using (var br = brObjectId.GetObject(OpenMode.ForRead) as BlockReference)
            //  {
            //    if (br == null) continue;
            //    System.Diagnostics.Debug.Print($"Name: {br.Name} | BlockName: {br.BlockName}");
            //    using (var parentBtr = br.DynamicBlockTableRecord.GetObject(OpenMode.ForRead) as BlockTableRecord)
            //    {
            //      SendWipeoutToBack(parentBtr);
            //    }
            //  }
            //}
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
        private static void ParseBlockWipe(BlockTableRecord btr, Transaction acTrans)
        {
            foreach (var objectId in btr)
            {
                var blockReference = acTrans.GetObject(objectId, OpenMode.ForRead) as BlockReference;
                if (blockReference == null) continue;

                ParseBlockWipe(blockReference);
            }
        }

        private static void ParseBlockWipe(BlockReference br)
        {
            var btrObjectId = br.BlockTableRecord;
            var blockDefinition = btrObjectId.GetObject(OpenMode.ForRead) as BlockTableRecord;

            var objToMove = new ObjectIdCollection();

            foreach (var itemId in blockDefinition)
            {
                if (itemId.IsNull) continue;
                var item = itemId.GetObject(OpenMode.ForRead);
                if (item == null) continue;
                if (item is Wipeout)
                {
                    objToMove.Add(itemId);
                    //yield return itemId;
                }
            }

            if (objToMove.Count > 0)
            {
                using (var drawOrderTable = blockDefinition.DrawOrderTableId.GetObject(OpenMode.ForWrite) as DrawOrderTable)
                {
                    drawOrderTable.MoveToBottom(objToMove);
                    //System.Diagnostics.Debug.Print("\tMoveToBottom");
                }
            }
        }

        private static void ParseBlockRef(BlockTableRecord btr)
        {
            foreach (var objectId in btr)
            {
                //System.Diagnostics.Debug.Print("\t" + objectId.ObjectClass.Name);
                using (var blockReference = objectId.GetObject(OpenMode.ForRead) as BlockReference)
                {

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

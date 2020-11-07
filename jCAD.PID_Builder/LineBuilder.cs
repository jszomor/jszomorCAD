using JsonFindKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Runtime;

namespace jCAD.PID_Builder
{
  public class LineBuilder
  {
    public void LineCreator(JsonLineProperty line, Database acCurDb)
    {
      
      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTable acBlkTbl;
        BlockTableRecord acBlkTblRec;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
        acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;
        if (line.Type == "Autodesk.AutoCAD.DatabaseServices.Line")
        {
          double StartX = 0;
          double StartY = 0;
          double EndX = 0;
          double EndY = 0;
          foreach (var point in line.LineOrCenterPoints)
          {
            if(point.Point == 1)
            {
              StartX = point.X;
              StartY = point.Y;
            }
            if (point.Point == 2)
            {
              EndX = point.X;
              EndY = point.Y;
            }
          }
          Line acLine = new Line(new Point3d(StartX, StartY, 0), new Point3d(EndX, EndY, 0));
          acLine.SetDatabaseDefaults();
          acBlkTblRec.AppendEntity(acLine);
          acTrans.AddNewlyCreatedDBObject(acLine, true);
        }
        else if (line.Type == "Autodesk.AutoCAD.DatabaseServices.Polyline")
        {
          Polyline acPoly = new Polyline();
          acPoly.SetDatabaseDefaults();
          foreach (var point in line.LineOrCenterPoints)
          {
            acPoly.AddVertexAt(point.Point-1, new Point2d(point.X, point.Y), 0, 0, 0);
          }            

          acBlkTblRec.AppendEntity(acPoly);
          acTrans.AddNewlyCreatedDBObject(acPoly, true);
        }
        else if (line.Type == "Autodesk.AutoCAD.DatabaseServices.Circle")
        {
          double centerX = 0;
          double centerY = 0;
          foreach (var point in line.LineOrCenterPoints)
          {
            centerX = point.X;
            centerY = point.Y;
          }
          var circle = new Circle();
          circle.SetDatabaseDefaults();
          circle.Center = new Point3d(centerX, centerY, 0);
          circle.Radius = Convert.ToDouble(line.Radius);

          acBlkTblRec.AppendEntity(circle);
          acTrans.AddNewlyCreatedDBObject(circle, true);
        }
          acTrans.Commit();
      }
    }
  }
}

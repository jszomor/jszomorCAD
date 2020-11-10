using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JsonFindKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  public class DimensionBuilder
  {
    public void DimensionCreator(JsonDimensionProperty dimension, Database acCurDb)
    {

      using (Transaction acTrans = acCurDb.TransactionManager.StartTransaction())
      {
        BlockTable acBlkTbl;
        BlockTableRecord acBlkTblRec;
        acBlkTbl = acTrans.GetObject(acCurDb.BlockTableId, OpenMode.ForRead) as BlockTable;
        acBlkTblRec = acTrans.GetObject(acBlkTbl[BlockTableRecord.ModelSpace], OpenMode.ForWrite) as BlockTableRecord;

        double XLine1Point_X = 0;
        double XLine1Point_Y = 0;
        double XLine2Point_X = 0;
        double XLine2Point_Y = 0;
        double DimLine2Point_X = 0;
        double DimLine2Point_Y = 0;

        foreach (var point in dimension.XDimPoints)
        {
          if (point.Point == 1)
          {
            XLine1Point_X = point.X;
            XLine1Point_Y = point.Y;
          }
          else if (point.Point == 2)
          {
            XLine2Point_X = point.X;
            XLine2Point_Y = point.Y;
          }
          else if (point.Point == 3)
          {
            DimLine2Point_X = point.X;
            DimLine2Point_Y = point.Y;
          }
        }

        RotatedDimension acRotDim = new RotatedDimension();
        acRotDim.SetDatabaseDefaults();
        acRotDim.XLine1Point = new Point3d(XLine1Point_X, XLine1Point_Y, 0);
        acRotDim.XLine2Point = new Point3d(XLine2Point_X, XLine2Point_Y, 0);
        acRotDim.Rotation = dimension.DimRotation;
        acRotDim.DimLinePoint = new Point3d(DimLine2Point_X, DimLine2Point_Y, 0);
        acRotDim.DimensionStyle = acCurDb.Dimstyle;
        //acRotDim.DimensionStyleName = dimension.DimensionStyleName;
        acBlkTblRec.AppendEntity(acRotDim);
        acTrans.AddNewlyCreatedDBObject(acRotDim, true);
        acTrans.Commit();
      }
    }
  }
}

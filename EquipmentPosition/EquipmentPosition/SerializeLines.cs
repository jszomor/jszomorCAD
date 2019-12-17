using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonFindKey;
using Autodesk.AutoCAD.Geometry;
using System.Linq.Expressions;
using OrganiCAD.AutoCAD;

namespace EquipmentPosition
{
  public class SerializeLines
  {
    public void ExecuteActionOnModelSpace(Database database, Action<Transaction, BlockTableRecord> action)
    {
      ExecuteActionInTransaction(database, (db, tr) =>
        ExecuteActionOnBlockTable(db, bt =>
        {
          using (var ms = bt[BlockTableRecord.ModelSpace].GetObject<BlockTableRecord>())
          {
            action.Invoke(tr, ms);
          }
        }
        ));
    }

    public void ExecuteActionInTransaction(Database db, Action<Database, Transaction> action)
    {
      using (var tr = db.TransactionManager.StartTransaction())
      {
        action.Invoke(db, tr);
        tr.Commit();
      }
    }

    private void ExecuteActionOnTable<T>(Database db,
      Expression<Func<Database, ObjectId>> tableIdProperty, Action<T> action) where T : class, IDisposable
    {
      var c = tableIdProperty.Compile();
      using (var t = c.Invoke(db).GetObject<T>())
      {
        action.Invoke(t);
      }
    }

    public void ExecuteActionOnBlockTable(Database db, Action<BlockTable> action) =>
      ExecuteActionOnTable(db, x => x.BlockTableId, action);

    public void ExecuteActionOnLayerTable(Database db, Action<LayerTable> action) =>
      ExecuteActionOnTable(db, x => x.LayerTableId, action);

    public Point2D ConvertAcadVertex2DToPoint2D(Vertex2d Acadvertex)
    {
      return new Point2D(Acadvertex.Position.X, Acadvertex.Position.Y, "test");
    }

    public Point2D ConvertAcadPoint2dToPoint2D(Autodesk.AutoCAD.Geometry.Point2d Acadpoint)
    {
      return new Point2D(Acadpoint.X, Acadpoint.Y, "test");
    }
    public List<Point2D> GetPointofAcadObjects(Database db, Polyline2d p2d)
    {
      var points2D = new List<Point2D>();
      ExecuteActionOnModelSpace(db, (tr, ms) =>
      {
        foreach (ObjectId vId in p2d)
        {
          Vertex2d v2d = (Vertex2d)tr.GetObject(vId, OpenMode.ForRead);
          var point = ConvertAcadVertex2DToPoint2D(v2d);
          points2D.Add(point);
        }
      });
      return points2D;
    }

    public JsonLineProperty LineSerializator(DBObject item)
    {
      var jsonLineProperty = new JsonLineProperty();

      jsonLineProperty.Type = Convert.ToString(item);

      if (item is Line)
      {
        var line = item as Line;

        jsonLineProperty.Start.X = line.StartPoint.X;
        jsonLineProperty.Start.Y = line.StartPoint.Y;
        jsonLineProperty.End.X = line.EndPoint.X;
        jsonLineProperty.End.Y = line.EndPoint.Y;
      }
      //else if (item is Polyline2d)
      //{
      //  var p2d = item as Polyline2d;
      //  //var pls2d = new List<double>();
      //  foreach (Vertex2d polyline in p2d)
      //  {
      //    jsonLineProperty.Start.X = polyline.Position.X;
      //    jsonLineProperty.Start.Y = polyline.Position.Y;
      //    jsonLineProperty.pls.Add(jsonLineProperty.Start.X);
      //    jsonLineProperty.pls.Add(jsonLineProperty.Start.Y);
      //  }
      //}
      else if (item is Polyline)
      {
        //ar Jpoint2D = new JsonFindKey.Point2D();

        var p = item as Polyline;
        var pls = new List<Point2d>();    

        for (var i = 0; i < p.NumberOfVertices; i++)
        {
           //Point2d acPoint = p.GetPoint2dAt(i);

          var point = p.GetPoint2dAt(i);
          //var mypoint = new Point2D(point.X, point.Y);
          jsonLineProperty.pls.Add(ConvertAcadPoint2dToPoint2D(point));

          //jsonLineProperty.Start.X = acPoint.X;
          //jsonLineProperty.Start.Y = acPoint.Y;
          //pls.Add(acPoint);
          //jsonLineProperty.pls.Add(20);
          System.Diagnostics.Debug.WriteLine($"\t\tPOLYLINE POINTS: {point}");
        }
        p.Closed = jsonLineProperty.IsClosed;
        System.Diagnostics.Debug.WriteLine($"\t\tPOLYLINE POINTS: {p.Closed}");
        //foreach (var i in pls)
        //{
        //  System.Diagnostics.Debug.WriteLine($"\t\tPOLYLINE POINTS: {i}");
        //}
      }

      //try
      //{
      //  var line = tr.GetObject(entityId, OpenMode.ForRead) as Line;
      //  var polyline = tr.GetObject(entityId, OpenMode.ForRead) as Vertex2d;

      //  jsonLineProperty.ObjectId = Convert.ToString(entityId);

      //  if (line != null || polyline != null)
      //  {
      //    jsonLineProperty.End.X = line.EndPoint.X;
      //    jsonLineProperty.End.Y = line.EndPoint.Y;
      //    jsonLineProperty.Start.X = line.StartPoint.X;
      //    jsonLineProperty.Start.Y = line.StartPoint.Y;

      //    jsonLineProperty.End.X = polyline.Position.X;
      //    jsonLineProperty.End.Y = polyline.Position.Y;
      //    //jsonLineProperty.Start.X = polyline.;
      //    //jsonLineProperty.Start.Y = polyline.StartPoint.Y;
      //  }
      //}
      //catch (NullReferenceException ex)
      //{
      //  System.Diagnostics.Debug.Print(ex.Message);
      //}   
      return jsonLineProperty;
    }
  }
}

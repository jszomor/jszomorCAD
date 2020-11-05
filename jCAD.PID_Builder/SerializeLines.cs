﻿using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonFindKey;
using Autodesk.AutoCAD.Geometry;
using System.Linq.Expressions;
using OrganiCAD.AutoCAD;

namespace jCAD.PID_Builder
{
  public class JsonLineSetup
  {
    public Point2D ConvertAcadVertex2DToPoint2D(Vertex2d Acadvertex, int number)
    {
      //return new Point2D(Math.Round(Acadvertex.Position.X,2), Math.Round(Acadvertex.Position.Y,2), number);
      return new Point2D(Acadvertex.Position.X, Acadvertex.Position.Y, number);
    }

    public Point2D ConvertAcadPoint2dToPoint2D(Autodesk.AutoCAD.Geometry.Point2d Acadpoint, int number)
    {
      //return new Point2D(Math.Round(Acadpoint.X,2), Math.Round(Acadpoint.Y,2), number);
      return new Point2D(Acadpoint.X, Acadpoint.Y, number);
    }

    public Point2D ConvertAcadPoint3dToPoint2D(Autodesk.AutoCAD.Geometry.Point3d Acadpoint, int number)
    {
      //return new Point2D(Math.Round(Acadpoint.X,2), Math.Round(Acadpoint.Y,2), number);
      return new Point2D(Acadpoint.X, Acadpoint.Y, number);
    }

    public Point2D ConvertAcadCircleToPoint2D(Autodesk.AutoCAD.Geometry.Point3d Acadpoint, double radius)
    {
      //return new Point2D(Math.Round(Acadpoint.X, 2), Math.Round(Acadpoint.Y, 2), radius);
      return new Point2D(Acadpoint.X, Acadpoint.Y, radius);
    }

    public JsonLineProperty SetupLineProperty(DBObject item, JsonBlockSetup jsonBlockSetup)
    {
      var jsonLineProperty = new JsonLineProperty();

      jsonLineProperty.Type = Convert.ToString(item);

      if (item is Line)
      {
        var line = item as Line;

        jsonLineProperty.LinePoints.Add(ConvertAcadPoint3dToPoint2D(line.StartPoint, 1));
        jsonLineProperty.LinePoints.Add(ConvertAcadPoint3dToPoint2D(line.EndPoint, 2));
        jsonLineProperty.Layer = jsonBlockSetup.RealNameFinder(line.Layer);
        jsonLineProperty.Internal_Id = BlockTableRead.InternalCounter;
      }
      else if (item is Polyline)
      {
        var p = item as Polyline;
        for (var i = 0; i < p.NumberOfVertices; i++)
        {
          var point = p.GetPoint2dAt(i);
          jsonLineProperty.LinePoints.Add(ConvertAcadPoint2dToPoint2D(point, i + 1));
          jsonLineProperty.Layer = jsonBlockSetup.RealNameFinder(p.Layer);
          //System.Diagnostics.Debug.WriteLine($"\t\tPOLYLINE POINTS: {point}");
        }
        p.Closed = false;
        jsonLineProperty.Internal_Id = BlockTableRead.InternalCounter;
      }
      else if (item is Polyline2d)
      {
        var p2d = item as Polyline2d;
        int i = 1;
        foreach (Vertex2d polyline in p2d)
        {
          jsonLineProperty.LinePoints.Add(ConvertAcadVertex2DToPoint2D(polyline, i));
          jsonLineProperty.Layer = jsonBlockSetup.RealNameFinder(p2d.Layer);
          i++;
        }
        p2d.Closed = false;
        jsonLineProperty.Internal_Id = BlockTableRead.InternalCounter;
      }
      else if (item is Polyline3d)
      {
        var p3d = item as Polyline3d;
        int i = 1;
        foreach (Vertex2d polyline in p3d)
        {
          jsonLineProperty.LinePoints.Add(ConvertAcadVertex2DToPoint2D(polyline, i));
          jsonLineProperty.Layer = jsonBlockSetup.RealNameFinder(p3d.Layer);
          i++;
        }
        p3d.Closed = false;
        jsonLineProperty.Internal_Id = BlockTableRead.InternalCounter;
      }
      else if (item is Circle)
      {
        var circle = item as Circle;
        jsonLineProperty.LinePoints.Add(ConvertAcadCircleToPoint2D(circle.StartPoint, circle.Radius));
        jsonLineProperty.Layer = jsonBlockSetup.RealNameFinder(circle.Layer);
        jsonLineProperty.Internal_Id = BlockTableRead.InternalCounter;
      }
      return jsonLineProperty;
    }
  }
}

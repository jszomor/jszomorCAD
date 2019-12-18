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
    public Point2D ConvertAcadVertex2DToPoint2D(Vertex2d Acadvertex, int number)
    {
      return new Point2D(Acadvertex.Position.X, Acadvertex.Position.Y, number);
    }

    public Point2D ConvertAcadPoint2dToPoint2D(Autodesk.AutoCAD.Geometry.Point2d Acadpoint, int number)
    {
      return new Point2D(Acadpoint.X, Acadpoint.Y, number);
    }

    public Point2D ConvertAcadPoint3dToPoint2D(Autodesk.AutoCAD.Geometry.Point3d Acadpoint, int number)
    {
      return new Point2D(Acadpoint.X, Acadpoint.Y, number);
    }

    public JsonClassProperty LineSerializator(DBObject item)
    {
      var jsonClassProperty = new JsonClassProperty();

      jsonClassProperty.jsonLineProperty.Type = Convert.ToString(item);

      if (item is Line)
      {
        var line = item as Line;

        jsonClassProperty.jsonLineProperty.LinePoints.Add(ConvertAcadPoint3dToPoint2D(line.StartPoint, 1));
        jsonClassProperty.jsonLineProperty.LinePoints.Add(ConvertAcadPoint3dToPoint2D(line.EndPoint, 2));
      }
      else if (item is Polyline)
      {
        var p = item as Polyline;
        for (var i = 0; i < p.NumberOfVertices; i++)
        {
          var point = p.GetPoint2dAt(i);
          jsonClassProperty.jsonLineProperty.LinePoints.Add(ConvertAcadPoint2dToPoint2D(point, i + 1));
          //System.Diagnostics.Debug.WriteLine($"\t\tPOLYLINE POINTS: {point}");
        }
        p.Closed = false;
      }
      else if (item is Polyline2d)
      {
        var p2d = item as Polyline2d;
        int i = 1;
        foreach (Vertex2d polyline in p2d)
        {
          jsonClassProperty.jsonLineProperty.LinePoints.Add(ConvertAcadVertex2DToPoint2D(polyline, i));
          i++;
        }
        p2d.Closed = false;
      }
      else if (item is Polyline3d)
      {
        var p3d = item as Polyline3d;
        int i = 1;
        foreach (Vertex2d polyline in p3d)
        {
          jsonClassProperty.jsonLineProperty.LinePoints.Add(ConvertAcadVertex2DToPoint2D(polyline, i));
          i++;
        }
        p3d.Closed = false;
      }
      return jsonClassProperty;
    }
  }
}

using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonFindKey;

namespace EquipmentPosition
{
  public class SerializeLines
  {
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
      else if (item is Polyline2d)
      {
        var p2d = item as Polyline2d;

        foreach (Vertex2d polyline in p2d)
        {
          jsonLineProperty.Start.X = polyline.Position.X;
          jsonLineProperty.Start.Y = polyline.Position.Y;
        }
      }
      else if (item is Polyline)
      {
        var p = item as Polyline;

        //System.Diagnostics.Debug.WriteLine($"\t\tPOLYLINE");
        for (var i = 0; i < p.NumberOfVertices; i++)
        {
           var acPoint = p.GetPoint2dAt(i);

          jsonLineProperty.Start.X = acPoint.X;
          jsonLineProperty.Start.Y = acPoint.Y;
        }
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

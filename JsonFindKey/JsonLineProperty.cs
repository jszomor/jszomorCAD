using JsonParse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonLineProperty : IComparable<JsonLineProperty>
  {
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Internal_Id")]
    public int Internal_Id { get; set; }

    [JsonProperty("Layer")]
    public string Layer { get; set; }

    public List<Point2D> LinePoints { get; set; } = new List<Point2D>();

    [JsonProperty("Radius")]
    public double? Radius { get; set; }

    [JsonProperty("StartAngle")]
    public double? StartAngle { get; set; }

    [JsonProperty("EndAngle")]
    public double? EndAngle { get; set; }

    public int CompareTo(JsonLineProperty comparePart) => Internal_Id.CompareTo(comparePart.Internal_Id);
  }
  public class Point2D
  {
    [JsonProperty("Point")]
    public int Point { get; set; }

    [JsonProperty("X")]
    public double X { get; set; }

    [JsonProperty("Y")]
    public double Y { get; set; }
    
  

    //public Point2D(double radius)
    //{
    //  Radius = radius;
    //}

    public Point2D(double x, double y, int point) // line, polyline
    {
      Point = point;
      X = x;
      Y = y;
    }

    //public Point2D(double x, double y, double radius, int point) // circle
    //{
    //  X = x;
    //  Y = y;
    //  Radius = radius;
    //}

    //public Point2D(double x, double y, double radius, double startAngle, double endAngle, int point) // arc
    //{
    //  X = x;
    //  Y = y;
    //  Radius = radius;
    //  StartAngle = startAngle;
    //  EndAngle = endAngle;
    //}
  }
}

using JsonParse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonLineProperty
  {
    [JsonProperty("Type")]
    public string Type { get; set; }

    [JsonProperty("Internal_Id")]
    public int Internal_Id { get; set; }

    public List<Point2D> LinePoints { get; set; } = new List<Point2D>();

    public bool IsIdentical(JsonLineProperty jsonLineProperty)
    {
      //throw new NotImplementedException();
      return true;
    }
  }
  public class Point2D
  {

    private const int _digits = 4;
    public const double distanceError = 0.0001d;//double.Epsilon;
    public const double angleError = 0.00001d;//double.Epsilon;

    [JsonProperty("X")]
    public double X { get; set; }

    [JsonProperty("Y")]
    public double Y { get; set; }

    [JsonProperty("Point")]
    public int Point;

    public Point2D(double x, double y, int point)
    {
      X = x;
      Y = y;
      Point = point;
    }
  }
}

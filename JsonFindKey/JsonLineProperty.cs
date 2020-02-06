using JsonParse;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonLineProperty
  {
    public string Type { get; set; }
    public List<Point2D> LinePoints { get; set; } = new List<Point2D>();

    public bool IsIdentical(JsonLineProperty jsonLineProperty)
    {
      //throw new NotImplementedException();
      return true;
    }
  }

  public class Coordinate
  {
    public double X {get;set;}
    public double Y { get; set; }
  }

  public class Point2D
  {

    private const int _digits = 4;
    public const double distanceError = 0.0001d;//double.Epsilon;
    public const double angleError = 0.00001d;//double.Epsilon;

    public double X { get; set; }

    public double Y { get; set; }

    public string Name;

    public int Point;

    //public decimal RoundedX
    //{
    //  get
    //  {
    //    return Convert.ToDecimal(Math.Round(X, _digits));
    //  }
    //}
    //public decimal RoundedY
    //{
    //  get
    //  {
    //    return Convert.ToDecimal(Math.Round(Y, _digits));
    //  }
    //}

    public Point2D()
    {

    }
    public Point2D(double x, double y, int point)
    {
      X = x;
      Y = y;
      Point = point;
    }
  }
}

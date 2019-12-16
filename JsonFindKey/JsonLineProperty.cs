using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonLineProperty
  {
    public string Type { get; set; }
    public string ObjectId { get; set; }
    public Coordinate Start { get; set; } = new Coordinate();
    public Coordinate End { get; set; } = new Coordinate();
  }

  public class Coordinate
  {
    public double X {get;set;}
    public double Y { get; set; }
  }
}

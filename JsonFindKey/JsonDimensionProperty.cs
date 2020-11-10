using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonDimensionProperty
  {
    [JsonProperty("Internal_Id")]
    public int Internal_Id { get; set; }

    [JsonProperty("Dimension Type")]
    public string DimensionType { get; set; }

    [JsonProperty("Dimension Style")]
    public string DimensionStyle { get; set; }
    
    [JsonProperty("Dimension Style Name")]
    public string DimensionStyleName { get; set; }

    [JsonProperty("Layer")]
    public string Layer { get; set; }

    [JsonProperty("Dim rotation")]
    public double DimRotation { get; set; }

    public List<DimPoint2D> XDimPoints = new List<DimPoint2D>();
  }
  public class DimPoint2D
  {
    [JsonProperty("Dim Point")]
    public int Point { get; set; }

    [JsonProperty("DimX")]
    public double X { get; set; }

    [JsonProperty("DimY")]
    public double Y { get; set; }

    public DimPoint2D (double x, double y, int point)
    {
      Point = point;
      X = x;
      Y = y;
    }
  }
}

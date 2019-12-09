using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonStringBuilderProperty
  {

    public string PropName { get; set; }

    public object Appaerance { get; set; }

    [JsonProperty("Name")]
    public string BlockName { get; set; }

    [JsonProperty("Position X")]
    public object BlockX { get; set; }

    [JsonProperty("Position Y")]
    public double BlockY { get; set; }

    public double Rotation { get; set; }

    [JsonProperty("Position X")]
    public double TagX { get; set; }

    [JsonProperty("Position Y")]
    public double TagY { get; set; }

    [JsonProperty("Angle")]
    public double TagAngle { get; set; }

    [JsonProperty("Visibility1")]
    public object VisibilityValue { get; set; }

    [JsonProperty("Visibility")]
    public string VisibilityName { get; set; }

    [JsonProperty("Flip state1")]
    public string FlipState { get; set; }

    [JsonProperty("Block Table1")]
    public string GetVisibilityName { get; set; }

    public object GetVisibilityValue { get; set; }
  }
}

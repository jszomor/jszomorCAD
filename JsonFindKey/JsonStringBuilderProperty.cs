using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class JsonStringBuilderProperty
  {
    //public JsonStringBuilderProperty(string blockName, string visibilityName, object visibilityValue)
    //{
    //  VisibilityName = visibilityName;
    //  VisibilityValue = visibilityValue;
    //  BlockName = blockName;
    //}

      [JsonProperty(NullValueHandling = NullValueHandling.Ignore, DefaultValueHandling = DefaultValueHandling.Ignore)]
    public string LayerName { get; set; }
    public string BlockName { get; set; }
    public object BlockX { get; set; }
    public object BlockY { get; set; }
    public object Rotation { get; set; }
    public object TagX { get; set; }
    public object TagY { get; set; }
    public object Angle { get; set; }
    public object Angle1 { get; set; }
    public object Angle2 { get; set; }
    public object Distance { get; set; }
    public object Distance1 { get; set; }
    public object Distance2 { get; set; }
    public object Distance3 { get; set; }
    public object Distance4 { get; set; }
    public object Distance5 { get; set; }
    public object VisibilityValue { get; set; }
    public string VisibilityName { get; set; }
    public object FlipState { get; set; }
  }
}

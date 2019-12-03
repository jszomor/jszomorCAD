using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public class Position
  {
    public double X { get; set; }
    public double Y { get; set;  }
  }
  public class JsonProperty
  {
    [JsonProperty("BlockName")]
    public string Error { get; set; }
  }
}

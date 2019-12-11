using Autodesk.AutoCAD.DatabaseServices;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentPosition
{
  public class SerializationProperty
  {
    public SerializationProperty(string blockName, string visibilityName)
    {
      BlockName = blockName;
      VisibilityName = visibilityName;
    }


    public string Note { get; set; }

    public string LayerName { get; set; }

    [JsonProperty("Name")]
    public string BlockName { get; set; }

    [JsonProperty("Position X")]
    public object BlockX { get; set; }

    [JsonProperty("Position Y")]
    public double BlockY { get; set; }

    public double Rotation { get; set; }

    [JsonProperty("Position TagX")]
    public double TagX { get; set; }

    [JsonProperty("Position TagY")]
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

    public IEnumerable<Action<AttributeReference>> ActionToExecuteOnAttRef { get; set; }
    public IEnumerable<Action<DynamicBlockReferenceProperty>> ActionToExecuteOnDynPropAfter { get; set; }
    public IEnumerable<Action<BlockReference>> ActionToExecuteOnBr { get; set; }
  }
}

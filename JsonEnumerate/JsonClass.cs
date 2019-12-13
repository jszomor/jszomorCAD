using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonEnumerate
{
  public class JsonBlockClass
  {
    //public Geometry Geometry { get; } = new Geometry();

    //public Misc Misc { get; } = new Misc();

    //public General General { get; } = new General();

    //public Custom Custom { get; } = new Custom();

    //public Attributes Attributes { get; } = new Attributes();

    public class Geometry
    {
      [JsonProperty("Position X")]
      public double X { get; set; }

      [JsonProperty("Position Y")]
      public double Y { get; set; }
    }
    public class General
    {
      public string Layer { get; set; }
    }
    public class Misc
    {
      [JsonProperty("Name")]
      public string BlockName { get; set; }
      public double Rotation { get; set; }
    }
    public class Custom
    {
      [JsonProperty("Position X")]
      public double? TagX { get; set; }

      [JsonProperty("Position Y")]
      public double? TagY { get; set; }

      [JsonProperty("Angle")]
      public double? Angle { get; set; }

      [JsonProperty("Flip state")]
      public double? FlipState { get; set; }

      [JsonProperty("Angle1")]
      public double? Angle1 { get; set; }

      [JsonProperty("Angle2")]
      public double? Angle2 { get; set; }

      [JsonProperty("Visibility")]
      public object VisibilityValue { get; set; }

      [JsonProperty("Block Table1")]
      public object BlockTableValue { get; set; }

      [JsonProperty("Centrifugal Pump")]
      public object PumpTableValue { get; set; }

      public double? Distance { get; set; }
      public double? Distance1 { get; set; }
      public double? Distance2 { get; set; }
      public double? Distance3 { get; set; }
      public double? Distance4 { get; set; }
      public double? Distance5 { get; set; }
    }
    public class Attributes
    {
      [JsonProperty("HOSTNAME")]
      public string HostName { get; set; }

      [JsonProperty("NOTE")]
      public string Note { get; set; }

      [JsonProperty("MANUFACTURER")]
      public string Manufacturer { get; set; }

      [JsonProperty("MODEL")]
      public string Model { get; set; }

      [JsonProperty("CONSUMED_POWER")]
      public double? ConsumedPower { get; set; }

      [JsonProperty("MATERIAL_HOUSING")]
      public string MaterialHousing { get; set; }

      [JsonProperty("SP_HEAD")]
      public double? SpHead { get; set; }

      [JsonProperty("SP_CAPACITY")]
      public double? SpCapacity { get; set; }

      [JsonProperty("INSTALLATION")]
      public string Installation { get; set; }

      [JsonProperty("TAG")]
      public string Tag { get; set; }

      [JsonProperty("RUNNINGHOURS")]
      public double? RunninigHours { get; set; }

      [JsonProperty("INSTALLED_POWER")]
      public double? InstalledPower { get; set; }

      [JsonProperty("PUMP_TYPE")]
      public string PumpType { get; set; }

      [JsonProperty("MATERIAL_IMPELLER")]
      public string MaterialImpeller { get; set; }

      [JsonProperty("NOTE_CHINESE")]
      public string NoteChinese { get; set; }

      [JsonProperty("STB_DTY")]
      public double StandByDuty { get; set; }

      [JsonProperty("AREA_CODE")]
      public double AreaCode { get; set; }

      public double DI_ { get; set; }

      public double PO_ { get; set; }

      [JsonProperty("TAG_ID")]
      public string TagId { get; set; }

      [JsonProperty("UNIT_HEAD")]
      public double UnitHead { get; set; }

      [JsonProperty("UNIT_CAPACITY")]
      public double UnitCapacity { get; set; }

      [JsonProperty("PROCESSUNITAREA")]
      public double ProcessUnitArea { get; set; }

      [JsonProperty("EQUIP_TYPE")]
      public string EquipmentType { get; set; }

      [JsonProperty("SP_VOLUME")]
      public double? SpVolume { get; set; }
    }
  }
}
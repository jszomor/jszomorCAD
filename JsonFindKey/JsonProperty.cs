using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace JsonParse
{
  public class JsonBlockProperty
  {
    public Geometry Geometry { get; set; }
    public Misc Misc { get; set; }
    public General General { get; set; }
    public Custom Custom { get; set; }
    public Attributes Attributes { get; set; }
    public long NumberOfItem { get; set; }
  }
  public class Geometry
  {
    [JsonProperty("Position X")]
    public double X { get; set; }

    [JsonProperty("Position Y")]
    public double Y { get; set; }
    public Geometry(double x, double y)
    {
      X = x;
      Y = y;
    }
  }
  public class General
  {
    public string Layer { get; set; }
    public General(string layer)
    {
      Layer = layer;
    }
  }
  public class Misc
  {
    [JsonProperty("Name")]
    public string BlockName { get; set; }
    public double Rotation { get; set; }
    public Misc(string blockName, double rotation)
    {
      BlockName = blockName;
      Rotation = rotation;
    }
  }
  public class Custom
  {
    [JsonProperty("Position X")]
    public double TagX { get; set; }

    [JsonProperty("Position Y")]
    public double TagY { get; set; }

    [JsonProperty("Angle")]
    public double TagAngle { get; set; }

    [JsonProperty("Visibility1")]
    public string Visibility1 { get; set; }

    [JsonProperty("Visibility")]
    public string Visibility { get; set; }

    [JsonProperty("Flip state1")]
    public string FlipState { get; set; }

    [JsonProperty("Block Table1")]
    public object BlockTableValue { get; set; }

    [JsonProperty("Centrifugal Pump")]
    public object PumpTableValue { get; set; }
    public double? Distance { get; set; }
    public double? Distance1 { get; set; }
    public Custom(double tagX, double tagY, double tagAngle, string visibility1, string visibility,
      string flipState, object blockTableValue, double? distance, double? distance1, object pumpTableValue)
    {
      TagX = tagX;
      TagY = tagY;
      TagAngle = tagAngle;
      Visibility1 = visibility1;
      Visibility = visibility;
      FlipState = flipState;
      BlockTableValue = blockTableValue;
      Distance = distance;
      Distance1 = distance1;
      PumpTableValue = pumpTableValue;
    }
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
    public double ConsumedPower { get; set; }

    [JsonProperty("MATERIAL_HOUSING")]
    public string MaterialHousing { get; set; }

    [JsonProperty("SP_HEAD")]
    public double SpHead { get; set; }

    [JsonProperty("SP_CAPACITY")]
    public double SpCapacity { get; set; }

    [JsonProperty("INSTALLATION")]
    public string Installation { get; set; }

    [JsonProperty("TAG")]
    public double Tag { get; set; }

    [JsonProperty("RUNNINGHOURS")]
    public double RunninigHours { get; set; }

    [JsonProperty("INSTALLED_POWER")]
    public double InstalledPower { get; set; }

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

    public Attributes(string note,
                      string hostName,
                      string manufacturer,
                      string model,
                      double consumedPower,
                      string materialHousing,
                      double spHead,
                      double spCapacity,
                      double tag,
                      double runninigHours,
                      double installedPower,
                      string pumpType,
                      string materialImpeller,
                      string noteChinese,
                      double standByDuty,
                      double areaCode,
                      double DI,
                      double PO,
                      string tagId,
                      double unitHead,
                      double unitCapacity,
                      double processUnitArea,
                      string equipmentType,
                      double? spVolume)
    {
      Note = note;
      HostName = hostName;
      Manufacturer = manufacturer;
      Model = model;
      ConsumedPower = consumedPower;
      MaterialHousing = materialHousing;
      SpHead = spHead;
      SpCapacity = spCapacity;
      Tag = tag;
      RunninigHours = runninigHours;
      InstalledPower = installedPower;
      PumpType = pumpType;
      MaterialImpeller = materialImpeller;
      NoteChinese = noteChinese;
      StandByDuty = standByDuty;
      AreaCode = areaCode;
      DI_ = DI;
      PO_ = PO;
      TagId = tagId;
      UnitHead = unitHead;
      UnitCapacity = unitCapacity;
      ProcessUnitArea = processUnitArea;
      EquipmentType = equipmentType;
      SpVolume = spVolume;
    }
  }
}
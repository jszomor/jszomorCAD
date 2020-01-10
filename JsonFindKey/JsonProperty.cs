using JsonFindKey;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace JsonParse
{
  public class JsonPID
  {
    public List<JsonBlockProperty> Blocks { get; } = new List<JsonBlockProperty>();
    public List<JsonLineProperty> Lines { get; } = new List<JsonLineProperty>();

    public JsonBlockProperty BlockSearch(string equipmentName) => 
      Blocks.SingleOrDefault(b => b.Misc.BlockName == equipmentName);
  }
  public class JsonBlockProperty
  {
    public Geometry Geometry { get; } = new Geometry();

    public Misc Misc { get; } = new Misc();

    public General General { get; } = new General();

    public Custom Custom { get; } = new Custom();

    public Attributes Attributes { get; } = new Attributes();
  }
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
    [JsonProperty("TAG X")]
    public double? TagX { get; set; }

    [JsonProperty("TAG Y")]
    public double? TagY { get; set; }

    [JsonProperty("TAG1 X")]
    public double? TagX1 { get; set; }

    [JsonProperty("TAG1 Y")]
    public double? TagY1 { get; set; }

    [JsonProperty("Angle")]
    public double? Angle { get; set; }

    [JsonProperty("Flip state")]
    public short FlipState { get; set; }

    [JsonProperty("Flip state1")]
    public short FlipState1 { get; set; }

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

    [JsonProperty("Visibility1")]
    public object Visibility1 { get; set; }

    [JsonProperty("Try1")]
    public object Try1 { get; set; }

    [JsonProperty("Try")]
    public string Try { get; set; }

    [JsonProperty("Housing")]
    public string Housing { get; set; }

    [JsonProperty("TTRY")]
    public string TTRY { get; set; }

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

    [JsonProperty("NAME1")]
    public string Name1 { get; set; }

    [JsonProperty("NAME2")]
    public string Name2 { get; set; }

    [JsonProperty("NOTE")]
    public string Note { get; set; }

    [JsonProperty("TYP")]
    public string Typ { get; set; }

    [JsonProperty("EQUIP_TYPE")]
    public string EquipType { get; set; }

    [JsonProperty("BLOWER_TYPE")]
    public string BlowerType { get; set; }

    [JsonProperty("MANUFACTURER")]
    public string Manufacturer { get; set; }

    [JsonProperty("MODEL")]
    public string Model { get; set; }

    [JsonProperty("CONSUMED_POWER")]
    public string ConsumedPower { get; set; }

    [JsonProperty("POWER_CONSUMED")]
    public string PowerConsumed { get; set; }

    [JsonProperty("INSTALLED_POWER")]
    public string InstalledPower { get; set; }

    [JsonProperty("POWER_INSTALLED")]
    public string PowerInstalled { get; set; }

    [JsonProperty("POWER")]
    public string Power { get; set; }

    [JsonProperty("INSTALLATION")]
    public string Installation { get; set; }

    [JsonProperty("MATERIAL_HOUSING")]
    public string MaterialHousing { get; set; }

    [JsonProperty("MATERIAL_BARS")]
    public string MaterialBars { get; set; }

    [JsonProperty("MATERIAL_FIXED")]
    public string MaterialFixed { get; set; }

    [JsonProperty("MATERIAL_FRAME")]
    public string MaterialFrame { get; set; }

    [JsonProperty("MATERIAL")]
    public string Material { get; set; }

    [JsonProperty("MATERIAL_IMPELLER")]
    public string MaterialImpeller { get; set; }

    [JsonProperty("MATERIAL_CARPENTRY")]
    public string MaterialCarpentry { get; set; }

    [JsonProperty("MATERIAL_SCREW_LINER")]
    public string MaterialScrewLiner { get; set; }

    [JsonProperty("MATERIAL_BODY")]
    public string MaterialBody { get; set; }

    [JsonProperty("MATERIAL_GEAR")]
    public string MaterialGear { get; set; }

    [JsonProperty("MATERIAL_SHAFT")]
    public string MaterialShaft { get; set; }

    [JsonProperty("MATERIAL_ROTOR")]
    public string MaterialRotor { get; set; }

    [JsonProperty("MATERIAL_SUBSURFACE")]
    public string MaterialSubsurface { get; set; }

    [JsonProperty("MATERIAL_ABOVE_WATER")]
    public string MaterialAboveWater { get; set; }

    [JsonProperty("MATERIAL_ABOVE")]
    public string MaterialAbove { get; set; }

    [JsonProperty("MATERIAL_SEALING")]
    public string MaterialSealing { get; set; }

    [JsonProperty("MATERIAL_STEM")]
    public string MaterialStem { get; set; }

    [JsonProperty("MATERIAL_BLADE")]
    public string MaterialBlade { get; set; }

    [JsonProperty("SIZE")]
    public string Size { get; set; }

    [JsonProperty("SP_HEAD")]
    public string SpHead { get; set; }

    [JsonProperty("SP_ACTUATED")]
    public string SpActuated { get; set; }

    [JsonProperty("SP_CAPACITY")]
    public string SpCapacity { get; set; }

    [JsonProperty("SP_TSS_INLET")]
    public string SpTssInlet { get; set; }

    [JsonProperty("SP_TSS_OUTLET")]
    public string SpTssOutlet { get; set; }

    [JsonProperty("SP_VOLUME")]
    public string SpVolume { get; set; }

    [JsonProperty("SP_FLOW")]
    public string SpFlow { get; set; }

    [JsonProperty("FLOW")]
    public string Flow { get; set; }

    [JsonProperty("SP_SPACING")]
    public string SpSpacing { get; set; }

    [JsonProperty("SP_DIAMETER")]
    public string SpDiameter { get; set; }

    [JsonProperty("SP_PRESSURE")]
    public string SpPressure { get; set; }

    [JsonProperty("SP_WIDTH")]
    public string SpWidth { get; set; }

    [JsonProperty("SP_LEVEL")]
    public string SpLevel { get; set; }

    [JsonProperty("SP_BOARD")]
    public string SpBoard { get; set; }

    [JsonProperty("SP_LENGTH")]
    public string SpLength { get; set; }

    [JsonProperty("SP_INLET")]
    public string SpInlet { get; set; }

    [JsonProperty("SP_OUTLET")]
    public string SpOutlet { get; set; }

    [JsonProperty("SP_CHANNELH")]
    public string SpChannelH { get; set; }

    [JsonProperty("SP_CHANNELW")]
    public string SpChannelW { get; set; }

    [JsonProperty("SP_CLARIFIER_DIA")]
    public string SpClarifierDia { get; set; }

    [JsonProperty("SP_OPERATION_LEVEL")]
    public string SpOperationLevel { get; set; }

    [JsonProperty("SP_FREE_BOARD")]
    public string SpFreeboard { get; set; }

    [JsonProperty("SP_TANKW")]
    public string SpTankW { get; set; }

    [JsonProperty("SP_TANKL")]
    public string SpTankL { get; set; }

    [JsonProperty("SP_TANKD")]
    public string SpTankD { get; set; }

    [JsonProperty("SP_TANKV")]
    public string SpTankV { get; set; }

    [JsonProperty("SP_WATER_LEVEL")]
    public string SpWaterLevel { get; set; }

    [JsonProperty("SP_FLOWMAX")]
    public string SpFlowMax { get; set; }

    [JsonProperty("SP_FLOWMIN")]
    public string SpFlowMin { get; set; }

    [JsonProperty("SP_FLUID")]
    public string SpFluid { get; set; }

    [JsonProperty("UNIT_HEAD")]
    public string UnitHead { get; set; }

    [JsonProperty("UNIT_CAPACITY")]
    public string UnitCapacity { get; set; }

    [JsonProperty("UNIT_SPACING")]
    public string UnitSpacing { get; set; }

    [JsonProperty("UNIT_FLOW")]
    public string UnitFlow { get; set; }

    [JsonProperty("UNIT_TSS_INLET")]
    public string UnitTssInlet { get; set; }

    [JsonProperty("UNIT_TSS_OUTLET")]
    public string UnitTssOutlet { get; set; }

    [JsonProperty("UNIT_DIAMETER")]
    public string UnitDiameter { get; set; }

    [JsonProperty("UNIT_CLARIFIER_DIA")]
    public string UnitClarifierDia { get; set; }

    [JsonProperty("UNIT_PRESSURE")]
    public string UnitPressure { get; set; }

    [JsonProperty("UNIT_WIDTH")]
    public string UnitWidth { get; set; }

    [JsonProperty("UNIT_LEVEL")]
    public string UnitLevel { get; set; }

    [JsonProperty("UNIT_BOARD")]
    public string UnitBoard { get; set; }

    [JsonProperty("UNIT_LENGTH")]
    public string UnitLength { get; set; }

    [JsonProperty("UNIT_INLET")]
    public string UnitInlet { get; set; }

    [JsonProperty("UNIT_OUTLET")]
    public string UnitOutlet { get; set; }

    [JsonProperty("UNIT_CHANNELW")]
    public string UnitChannelW { get; set; }

    [JsonProperty("UNIT_CHANNELH")]
    public string UnitChannelH { get; set; }

    [JsonProperty("UNIT_WATER_LEVEL")]
    public string UnitWaterLevel { get; set; }

    [JsonProperty("UNIT_FLOWMAX")]
    public string UnitFlowMax { get; set; }

    [JsonProperty("UNIT_FLOWMIN")]
    public string UnitFlowMin { get; set; }

    [JsonProperty("UNIT_CLARIFIER")]
    public string UnitClarifier { get; set; }

    [JsonProperty("UNIT_OPERATIONAL_LEVEL")]
    public string UnitOperationalLevel { get; set; }

    [JsonProperty("UNIT_FREE_BOARD")]
    public string UnitFreeboard { get; set; }

    [JsonProperty("UNIT_TANKV")]
    public string UnitTankV { get; set; }

    [JsonProperty("UNIT_TANKD")]
    public string UnitTankD { get; set; }

    [JsonProperty("UNIT_TANKL")]
    public string UnitTankL { get; set; }

    [JsonProperty("UNIT_TANW")]
    public string UnitTankW { get; set; }

    [JsonProperty("UNIT_VOLUME")]
    public string UnitVolume { get; set; }

    [JsonProperty("TAG")]
    public string Tag { get; set; }

    [JsonProperty("RUNNINGHOURS")]
    public string RunningHours { get; set; }

    [JsonProperty("NUMBER_OF_UNIT")]
    public string NumberOfUnits { get; set; }

    [JsonProperty("PUMP_TYPE")]
    public string PumpType { get; set; }

    [JsonProperty("FILTER")]
    public string Filter { get; set; }

    [JsonProperty("NOTE_CHINESE")]
    public string NoteChinese { get; set; }

    [JsonProperty("STB_DTY")]
    public string StandByDuty { get; set; }

    [JsonProperty("AREA_CODE")]
    public string AreaCode { get; set; }

    [JsonProperty("DI")]
    public string DI { get; set; }

    [JsonProperty("DO")]
    public string DO { get; set; }

    [JsonProperty("AI")]
    public string AI { get; set; }

    [JsonProperty("AO")]
    public string AO { get; set; }

    [JsonProperty("PB")]
    public string PB { get; set; }

    [JsonProperty("PO")]
    public string PO { get; set; }

    [JsonProperty("FC_MOD")]
    public string FcMod { get; set; }

    [JsonProperty("FC_MAN")]
    public string FcMan { get; set; }

    [JsonProperty("TAG_ID")]
    public string TagId { get; set; }

    [JsonProperty("PROCESSUNITAREA")]
    public string ProcessUnitArea { get; set; }

    [JsonProperty("VOLUME")]
    public string Volume { get; set; }

    [JsonProperty("PRESSURE")]
    public string Pressure { get; set; }

    [JsonProperty("MATERIAL_COVER")]
    public string MaterialCover { get; set; }

    [JsonProperty("CHANNELWIDTH")]
    public string ChannelWidth { get; set; }

    [JsonProperty("WIDTH")]
    public string Width { get; set; }

    [JsonProperty("LIQUIDLEVEL")]
    public string LiquidLevel { get; set; }

    [JsonProperty("LENGTH")]
    public string Length { get; set; }

    [JsonProperty("HEIGHT")]
    public string Height { get; set; }

    [JsonProperty("DIAMETER")]
    public string Diameter { get; set; }
  }

}
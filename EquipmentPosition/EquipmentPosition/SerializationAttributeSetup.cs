using Autodesk.AutoCAD.DatabaseServices;
using JsonFindKey;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class JsonAttributeSetup
  {
    private void SetRefTextString(AttributeReference attRef, JsonBlockProperty block)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties = block.Attributes.GetType().GetProperties();
      foreach (var prop in properties)
      {
        var customAttributes = prop
          .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (attRef.Tag == jsonTagName)
          {
            prop.SetValue(block.Attributes, attRef.TextString); //serialization
          }
        }
      }
    }

    public JsonBlockProperty SetupAttributeProperty (Transaction tr, BlockReference blockReference, JsonBlockProperty jsonBlockProperty)
    {
      AttributeCollection attCol = blockReference.AttributeCollection;
      foreach (ObjectId attId in attCol)
      {
        AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);

        SetRefTextString(attRef, jsonBlockProperty);

        #region Tags
        //if (attRef.Tag == "NOTE") { jsonBlockProperty.Attributes.Note = attRef.TextString; continue; }
        //if (attRef.Tag == "NOTE_CHINESE") { jsonBlockProperty.Attributes.NoteChinese = attRef.TextString; continue; }
        //if (attRef.Tag == "Name") { jsonBlockProperty.Attributes.Name = attRef.TextString; continue; }
        //if (attRef.Tag == "NAME1") { jsonBlockProperty.Attributes.Name1 = attRef.TextString; continue; }
        //if (attRef.Tag == "NAME2") { jsonBlockProperty.Attributes.Name2 = attRef.TextString; continue; }
        //if (attRef.Tag == "TAG") { jsonBlockProperty.Attributes.Tag = attRef.TextString; continue; }
        //if (attRef.Tag == "TAG_ID") { jsonBlockProperty.Attributes.TagId = attRef.TextString; continue; }
        //if (attRef.Tag == "AREA_CODE") { jsonBlockProperty.Attributes.AreaCode = attRef.TextString; continue; }
        //if (attRef.Tag == "MANUFACTURER") { jsonBlockProperty.Attributes.Manufacturer = attRef.TextString; continue; }
        //if (attRef.Tag == "MODEL") { jsonBlockProperty.Attributes.Model = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_COVER") { jsonBlockProperty.Attributes.MaterialCover = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_BARS") { jsonBlockProperty.Attributes.MaterialBars = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_FIXED") { jsonBlockProperty.Attributes.MaterialFixed = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_FRAME") { jsonBlockProperty.Attributes.MaterialFrame = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_HOUSING") { jsonBlockProperty.Attributes.MaterialHousing = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL") { jsonBlockProperty.Attributes.Material = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_SCREW_LINER") { jsonBlockProperty.Attributes.MaterialScrewLiner = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_CARPENTRY") { jsonBlockProperty.Attributes.MaterialCarpentry = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_BODY") { jsonBlockProperty.Attributes.MaterialBody = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_GEAR") { jsonBlockProperty.Attributes.MaterialGear = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_SHAFT") { jsonBlockProperty.Attributes.MaterialShaft = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_ROTOR") { jsonBlockProperty.Attributes.MaterialRotor = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_SUBSURFACE") { jsonBlockProperty.Attributes.MaterialSubsurface = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_ABOVE_WATER") { jsonBlockProperty.Attributes.MaterialAboveWater = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_SEALING") { jsonBlockProperty.Attributes.MaterialSealing = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_STEM") { jsonBlockProperty.Attributes.MaterialStem = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_BLADE") { jsonBlockProperty.Attributes.MaterialBlade = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_ABOVE") { jsonBlockProperty.Attributes.MaterialAbove = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLOW") { jsonBlockProperty.Attributes.SpFlow = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLUID") { jsonBlockProperty.Attributes.SpFluid = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_SPACING") { jsonBlockProperty.Attributes.SpSpacing = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_ACTUATED") { jsonBlockProperty.Attributes.SpActuated = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CAPACITY") { jsonBlockProperty.Attributes.SpCapacity = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_HEAD") { jsonBlockProperty.Attributes.SpHead = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TSS_INLET") { jsonBlockProperty.Attributes.SpTssInlet = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TSS_OUTLET") { jsonBlockProperty.Attributes.SpTssOutlet = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_DIAMETER") { jsonBlockProperty.Attributes.SpDiameter = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_VOLUME") { jsonBlockProperty.Attributes.SpVolume = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_PRESSURE") { jsonBlockProperty.Attributes.SpPressure = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_WIDTH") { jsonBlockProperty.Attributes.SpWidth = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_LEVEL") { jsonBlockProperty.Attributes.SpLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_BOARD") { jsonBlockProperty.Attributes.SpBoard = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_LENGTH") { jsonBlockProperty.Attributes.SpLength = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_INLET") { jsonBlockProperty.Attributes.SpInlet = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_OUTLET") { jsonBlockProperty.Attributes.SpOutlet = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CHANNELH") { jsonBlockProperty.Attributes.SpChannelH = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CHANNELW") { jsonBlockProperty.Attributes.SpChannelW = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_WATER_LEVEL") { jsonBlockProperty.Attributes.SpWaterLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLOWMAX") { jsonBlockProperty.Attributes.SpFlowMax = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLOWMIN") { jsonBlockProperty.Attributes.SpFlowMin = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKW") { jsonBlockProperty.Attributes.SpTankW = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKL") { jsonBlockProperty.Attributes.SpTankL = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKD") { jsonBlockProperty.Attributes.SpTankD = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKV") { jsonBlockProperty.Attributes.SpTankV = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CLARIFIER_DIA") { jsonBlockProperty.Attributes.SpClarifierDia = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_OPERATION_LEVEL") { jsonBlockProperty.Attributes.SpOperationLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FREE_BOARD") { jsonBlockProperty.Attributes.SpFreeboard = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_SPACING") { jsonBlockProperty.Attributes.UnitSpacing = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FLOW") { jsonBlockProperty.Attributes.UnitFlow = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_HEAD") { jsonBlockProperty.Attributes.UnitHead = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CAPACITY") { jsonBlockProperty.Attributes.UnitCapacity = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TSS_INLET") { jsonBlockProperty.Attributes.UnitTssInlet = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TSS_OUTLET") { jsonBlockProperty.Attributes.UnitTssOutlet = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_DIAMETER") { jsonBlockProperty.Attributes.UnitDiameter = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_PRESSURE") { jsonBlockProperty.Attributes.UnitPressure = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_WIDTH") { jsonBlockProperty.Attributes.UnitWidth = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_LEVEL") { jsonBlockProperty.Attributes.UnitLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_BOARD") { jsonBlockProperty.Attributes.UnitBoard = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_LENGTH") { jsonBlockProperty.Attributes.UnitLength = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_INLET") { jsonBlockProperty.Attributes.UnitInlet = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_OUTLET") { jsonBlockProperty.Attributes.UnitOutlet = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CHANNELW") { jsonBlockProperty.Attributes.UnitChannelW = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CHANNELH") { jsonBlockProperty.Attributes.UnitChannelH = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CLARIFIER") { jsonBlockProperty.Attributes.UnitClarifier = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_OPERATIONAL_LEVEL") { jsonBlockProperty.Attributes.UnitOperationalLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FREE_BOARD") { jsonBlockProperty.Attributes.UnitFreeboard = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_WATER_LEVEL") { jsonBlockProperty.Attributes.UnitWaterLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FLOWMAX") { jsonBlockProperty.Attributes.UnitFlowMax = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FLOWMIN") { jsonBlockProperty.Attributes.UnitFlowMin = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CLARIFIER_DIA") { jsonBlockProperty.Attributes.UnitClarifierDia = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_VOLUME") { jsonBlockProperty.Attributes.UnitVolume = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANKD") { jsonBlockProperty.Attributes.UnitTankD = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANKL") { jsonBlockProperty.Attributes.UnitTankL = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANW") { jsonBlockProperty.Attributes.UnitTankW = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANKV") { jsonBlockProperty.Attributes.UnitTankV = attRef.TextString; continue; }
        //if (attRef.Tag == "RUNNINGHOURS") { jsonBlockProperty.Attributes.RunningHours = attRef.TextString; continue; }
        //if (attRef.Tag == "EQUIP_TYPE") { jsonBlockProperty.Attributes.EquipType = attRef.TextString; continue; }
        //if (attRef.Tag == "BLOWER_TYPE") { jsonBlockProperty.Attributes.BlowerType = attRef.TextString; continue; }
        //if (attRef.Tag == "STB_DTY") { jsonBlockProperty.Attributes.StandByDuty = attRef.TextString; continue; }
        //if (attRef.Tag == "DI") { jsonBlockProperty.Attributes.DI = attRef.TextString; continue; }
        //if (attRef.Tag == "DO") { jsonBlockProperty.Attributes.DO = attRef.TextString; continue; }
        //if (attRef.Tag == "AI") { jsonBlockProperty.Attributes.AI = attRef.TextString; continue; }
        //if (attRef.Tag == "AO") { jsonBlockProperty.Attributes.AO = attRef.TextString; continue; }
        //if (attRef.Tag == "PB") { jsonBlockProperty.Attributes.PB = attRef.TextString; continue; }
        //if (attRef.Tag == "PO") { jsonBlockProperty.Attributes.PO = attRef.TextString; continue; }
        //if (attRef.Tag == "PROCESSUNITAREA") { jsonBlockProperty.Attributes.ProcessUnitArea = attRef.TextString; continue; }
        //if (attRef.Tag == "VOLUME") { jsonBlockProperty.Attributes.Volume = attRef.TextString; continue; }
        //if (attRef.Tag == "LIQUIDLEVEL") { jsonBlockProperty.Attributes.LiquidLevel = attRef.TextString; continue; }
        //if (attRef.Tag == "LENGTH") { jsonBlockProperty.Attributes.Length = attRef.TextString; continue; }
        //if (attRef.Tag == "HEIGHT") { jsonBlockProperty.Attributes.Height = attRef.TextString; continue; }
        //if (attRef.Tag == "WIDTH") { jsonBlockProperty.Attributes.Width = attRef.TextString; continue; }
        //if (attRef.Tag == "CHANNELWIDTH") { jsonBlockProperty.Attributes.ChannelWidth = attRef.TextString; continue; }
        //if (attRef.Tag == "PRESSURE") { jsonBlockProperty.Attributes.Pressure = attRef.TextString; continue; }
        //if (attRef.Tag == "SIZE") { jsonBlockProperty.Attributes.Size = attRef.TextString; continue; }
        //if (attRef.Tag == "INSTALLATION") { jsonBlockProperty.Attributes.Installation = attRef.TextString; continue; }
        //if (attRef.Tag == "FC_MOD") { jsonBlockProperty.Attributes.FcMod = attRef.TextString; continue; }
        //if (attRef.Tag == "FC_MAN") { jsonBlockProperty.Attributes.FcMan = attRef.TextString; continue; }
        //if (attRef.Tag == "PUMP_TYPE") { jsonBlockProperty.Attributes.PumpType = attRef.TextString; continue; }
        //if (attRef.Tag == "FILTER") { jsonBlockProperty.Attributes.Filter = attRef.TextString; continue; }
        //if (attRef.Tag == "FLOW") { jsonBlockProperty.Attributes.Flow = attRef.TextString; continue; }
        //if (attRef.Tag == "DIAMETER") { jsonBlockProperty.Attributes.Diameter = attRef.TextString; continue; }
        //if (attRef.Tag == "POWER") { jsonBlockProperty.Attributes.Power = attRef.TextString; continue; }
        //if (attRef.Tag == "INSTALLED_POWER") { jsonBlockProperty.Attributes.InstalledPower = attRef.TextString; continue; }
        //if (attRef.Tag == "POWER_INSTALLED") { jsonBlockProperty.Attributes.PowerInstalled = attRef.TextString; continue; }
        //if (attRef.Tag == "CONSUMED_POWER") { jsonBlockProperty.Attributes.ConsumedPower = attRef.TextString; continue; }
        //if (attRef.Tag == "POWER_CONSUMED") { jsonBlockProperty.Attributes.PowerConsumed = attRef.TextString; continue; }
        //if (attRef.Tag == "NUMBER_OF_UNIT") { jsonBlockProperty.Attributes.NumberOfUnits = attRef.TextString; continue; }
        //if (attRef.Tag == "TYP") { jsonBlockProperty.Attributes.Typ = attRef.TextString; continue; }
        //if (attRef.Tag == "HOST_NAME") { jsonBlockProperty.Attributes.HostName = attRef.TextString; continue; }
        #endregion //
      }

      return jsonBlockProperty;
    }
  }
}

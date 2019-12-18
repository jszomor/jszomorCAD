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
  public class SerializationAttributeSetup
  {
    public JsonClassProperty SetupAttributeProperty (Transaction tr, BlockReference blockReference, JsonClassProperty jsonClassPorperty)
    {
      AttributeCollection attCol = blockReference.AttributeCollection;
      foreach (ObjectId attId in attCol)
      {
        AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);

        if (attRef.Tag == "NOTE") { jsonClassPorperty.jsonBlockProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "NOTE_CHINESE") { jsonClassPorperty.jsonBlockProperty.Attributes.NoteChinese = attRef.TextString; continue; }
        if (attRef.Tag == "NAME1") { jsonClassPorperty.jsonBlockProperty.Attributes.Name1 = attRef.TextString; continue; }
        if (attRef.Tag == "NAME2") { jsonClassPorperty.jsonBlockProperty.Attributes.Name2 = attRef.TextString; continue; }
        if (attRef.Tag == "TAG") { jsonClassPorperty.jsonBlockProperty.Attributes.Tag = attRef.TextString; continue; }
        if (attRef.Tag == "TAG_ID") { jsonClassPorperty.jsonBlockProperty.Attributes.TagId = attRef.TextString; continue; }
        if (attRef.Tag == "AREA_CODE") { jsonClassPorperty.jsonBlockProperty.Attributes.AreaCode = attRef.TextString; continue; }
        if (attRef.Tag == "MANUFACTURER") { jsonClassPorperty.jsonBlockProperty.Attributes.Manufacturer = attRef.TextString; continue; }
        if (attRef.Tag == "MODEL") { jsonClassPorperty.jsonBlockProperty.Attributes.Model = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_COVER") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialCover = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BARS") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialBars = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_FIXED") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialFixed = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_FRAME") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialFrame = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_HOUSING") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialHousing = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL") { jsonClassPorperty.jsonBlockProperty.Attributes.Material = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SCREW_LINER") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialScrewLiner = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_CARPENTRY") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialCarpentry = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BODY") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialBody = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_GEAR") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialGear = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SHAFT") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialShaft = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ROTOR") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialRotor = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SUBSURFACE") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialSubsurface = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ABOVE_WATER") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialAboveWater = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SEALING") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialSealing = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_STEM") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialStem = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BLADE") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialBlade = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ABOVE") { jsonClassPorperty.jsonBlockProperty.Attributes.MaterialAbove = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FLOW") { jsonClassPorperty.jsonBlockProperty.Attributes.SpFlow = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FLUID") { jsonClassPorperty.jsonBlockProperty.Attributes.SpFluid = attRef.TextString; continue; }
        if (attRef.Tag == "SP_SPACING") { jsonClassPorperty.jsonBlockProperty.Attributes.SpSpacing = attRef.TextString; continue; }
        if (attRef.Tag == "SP_ACTUATED") { jsonClassPorperty.jsonBlockProperty.Attributes.SpActuated = attRef.TextString; continue; }
        if (attRef.Tag == "SP_CAPACITY") { jsonClassPorperty.jsonBlockProperty.Attributes.SpCapacity = attRef.TextString; continue; }
        if (attRef.Tag == "SP_HEAD") { jsonClassPorperty.jsonBlockProperty.Attributes.SpHead = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TSS_INLET") { jsonClassPorperty.jsonBlockProperty.Attributes.SpTssInlet = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TSS_OUTLET") { jsonClassPorperty.jsonBlockProperty.Attributes.SpTssOutlet = attRef.TextString; continue; }
        if (attRef.Tag == "SP_DIAMETER") { jsonClassPorperty.jsonBlockProperty.Attributes.SpDiameter = attRef.TextString; continue; }
        if (attRef.Tag == "SP_VOLUME") { jsonClassPorperty.jsonBlockProperty.Attributes.SpVolume = attRef.TextString; continue; }
        if (attRef.Tag == "SP_PRESSURE") { jsonClassPorperty.jsonBlockProperty.Attributes.SpPressure = attRef.TextString; continue; }
        if (attRef.Tag == "SP_WIDTH") { jsonClassPorperty.jsonBlockProperty.Attributes.SpWidth = attRef.TextString; continue; }
        if (attRef.Tag == "SP_LEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.SpLevel = attRef.TextString; continue; }
        if (attRef.Tag == "SP_BOARD") { jsonClassPorperty.jsonBlockProperty.Attributes.SpBoard = attRef.TextString; continue; }
        if (attRef.Tag == "SP_LENGTH") { jsonClassPorperty.jsonBlockProperty.Attributes.SpLength = attRef.TextString; continue; }
        if (attRef.Tag == "SP_INLET") { jsonClassPorperty.jsonBlockProperty.Attributes.SpInlet = attRef.TextString; continue; }
        if (attRef.Tag == "SP_OUTLET") { jsonClassPorperty.jsonBlockProperty.Attributes.SpOutlet = attRef.TextString; continue; }
        if (attRef.Tag == "SP_CHANNELH") { jsonClassPorperty.jsonBlockProperty.Attributes.SpChannelH = attRef.TextString; continue; }
        if (attRef.Tag == "SP_CHANNELW") { jsonClassPorperty.jsonBlockProperty.Attributes.SpChannelW = attRef.TextString; continue; }
        if (attRef.Tag == "SP_WATER_LEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.SpWaterLevel = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FLOWMAX") { jsonClassPorperty.jsonBlockProperty.Attributes.SpFlowMax = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FLOWMIN") { jsonClassPorperty.jsonBlockProperty.Attributes.SpFlowMin = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TANKW") { jsonClassPorperty.jsonBlockProperty.Attributes.SpTankW = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TANKL") { jsonClassPorperty.jsonBlockProperty.Attributes.SpTankL = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TANKD") { jsonClassPorperty.jsonBlockProperty.Attributes.SpTankD = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TANKV") { jsonClassPorperty.jsonBlockProperty.Attributes.SpTankV = attRef.TextString; continue; }
        if (attRef.Tag == "SP_CLARIFIER_DIA") { jsonClassPorperty.jsonBlockProperty.Attributes.SpClarifierDia = attRef.TextString; continue; }
        if (attRef.Tag == "SP_OPERATION_LEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.SpOperationLevel = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FREE_BOARD") { jsonClassPorperty.jsonBlockProperty.Attributes.SpFreeboard = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_SPACING") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitSpacing = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_FLOW") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitFlow = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_HEAD") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitHead = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CAPACITY") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitCapacity = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TSS_INLET") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitTssInlet = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TSS_OUTLET") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitTssOutlet = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_DIAMETER") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitDiameter = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_PRESSURE") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitPressure = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_WIDTH") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitWidth = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_LEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitLevel = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_BOARD") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitBoard = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_LENGTH") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitLength = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_INLET") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitInlet = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_OUTLET") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitOutlet = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CHANNELW") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitChannelW = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CHANNELH") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitChannelH = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CLARIFIER") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitClarifier = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_OPERATIONAL_LEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitOperationalLevel = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_FREE_BOARD") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitFreeboard = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_WATER_LEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitWaterLevel = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_FLOWMAX") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitFlowMax = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_FLOWMIN") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitFlowMin = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CLARIFIER_DIA") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitClarifierDia = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_VOLUME") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitVolume = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TANKD") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitTankD = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TANKL") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitTankL = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TANW") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitTankW = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TANKV") { jsonClassPorperty.jsonBlockProperty.Attributes.UnitTankV = attRef.TextString; continue; }
        if (attRef.Tag == "RUNNINGHOURS") { jsonClassPorperty.jsonBlockProperty.Attributes.RunningHours = attRef.TextString; continue; }
        if (attRef.Tag == "EQUIP_TYPE") { jsonClassPorperty.jsonBlockProperty.Attributes.EquipType = attRef.TextString; continue; }
        if (attRef.Tag == "BLOWER_TYPE") { jsonClassPorperty.jsonBlockProperty.Attributes.BlowerType = attRef.TextString; continue; }
        if (attRef.Tag == "STB_DTY") { jsonClassPorperty.jsonBlockProperty.Attributes.StandByDuty = attRef.TextString; continue; }
        if (attRef.Tag == "DI") { jsonClassPorperty.jsonBlockProperty.Attributes.DI = attRef.TextString; continue; }
        if (attRef.Tag == "DO") { jsonClassPorperty.jsonBlockProperty.Attributes.DO = attRef.TextString; continue; }
        if (attRef.Tag == "AI") { jsonClassPorperty.jsonBlockProperty.Attributes.AI = attRef.TextString; continue; }
        if (attRef.Tag == "AO") { jsonClassPorperty.jsonBlockProperty.Attributes.AO = attRef.TextString; continue; }
        if (attRef.Tag == "PB") { jsonClassPorperty.jsonBlockProperty.Attributes.PB = attRef.TextString; continue; }
        if (attRef.Tag == "PO") { jsonClassPorperty.jsonBlockProperty.Attributes.PO = attRef.TextString; continue; }
        if (attRef.Tag == "PROCESSUNITAREA") { jsonClassPorperty.jsonBlockProperty.Attributes.ProcessUnitArea = attRef.TextString; continue; }
        if (attRef.Tag == "VOLUME") { jsonClassPorperty.jsonBlockProperty.Attributes.Volume = attRef.TextString; continue; }
        if (attRef.Tag == "LIQUIDLEVEL") { jsonClassPorperty.jsonBlockProperty.Attributes.LiquidLevel = attRef.TextString; continue; }
        if (attRef.Tag == "LENGTH") { jsonClassPorperty.jsonBlockProperty.Attributes.Length = attRef.TextString; continue; }
        if (attRef.Tag == "HEIGHT") { jsonClassPorperty.jsonBlockProperty.Attributes.Height = attRef.TextString; continue; }
        if (attRef.Tag == "WIDTH") { jsonClassPorperty.jsonBlockProperty.Attributes.Width = attRef.TextString; continue; }
        if (attRef.Tag == "CHANNELWIDTH") { jsonClassPorperty.jsonBlockProperty.Attributes.ChannelWidth = attRef.TextString; continue; }
        if (attRef.Tag == "PRESSURE") { jsonClassPorperty.jsonBlockProperty.Attributes.Pressure = attRef.TextString; continue; }
        if (attRef.Tag == "SIZE") { jsonClassPorperty.jsonBlockProperty.Attributes.Size = attRef.TextString; continue; }
        if (attRef.Tag == "INSTALLATION") { jsonClassPorperty.jsonBlockProperty.Attributes.Installation = attRef.TextString; continue; }
        if (attRef.Tag == "FC_MOD") { jsonClassPorperty.jsonBlockProperty.Attributes.FcMod = attRef.TextString; continue; }
        if (attRef.Tag == "FC_MAN") { jsonClassPorperty.jsonBlockProperty.Attributes.FcMan = attRef.TextString; continue; }
        if (attRef.Tag == "PUMP_TYPE") { jsonClassPorperty.jsonBlockProperty.Attributes.PumpType = attRef.TextString; continue; }
        if (attRef.Tag == "FILTER") { jsonClassPorperty.jsonBlockProperty.Attributes.Filter = attRef.TextString; continue; }
        if (attRef.Tag == "FLOW") { jsonClassPorperty.jsonBlockProperty.Attributes.Flow = attRef.TextString; continue; }
        if (attRef.Tag == "DIAMETER") { jsonClassPorperty.jsonBlockProperty.Attributes.Diameter = attRef.TextString; continue; }
        if (attRef.Tag == "POWER") { jsonClassPorperty.jsonBlockProperty.Attributes.Power = attRef.TextString; continue; }
        if (attRef.Tag == "INSTALLED_POWER") { jsonClassPorperty.jsonBlockProperty.Attributes.InstalledPower = attRef.TextString; continue; }
        if (attRef.Tag == "POWER_INSTALLED") { jsonClassPorperty.jsonBlockProperty.Attributes.PowerInstalled = attRef.TextString; continue; }
        if (attRef.Tag == "CONSUMED_POWER") { jsonClassPorperty.jsonBlockProperty.Attributes.ConsumedPower = attRef.TextString; continue; }
        if (attRef.Tag == "POWER_CONSUMED") { jsonClassPorperty.jsonBlockProperty.Attributes.PowerConsumed = attRef.TextString; continue; }
        if (attRef.Tag == "NUMBER_OF_UNIT") { jsonClassPorperty.jsonBlockProperty.Attributes.NumberOfUnits = attRef.TextString; continue; }
        if (attRef.Tag == "TYP") { jsonClassPorperty.jsonBlockProperty.Attributes.Typ = attRef.TextString; continue; }
        if (attRef.Tag == "HOST_NAME") { jsonClassPorperty.jsonBlockProperty.Attributes.HostName = attRef.TextString; continue; }
      }

      return jsonClassPorperty;
    }
  }
}

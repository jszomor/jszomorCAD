using Autodesk.AutoCAD.DatabaseServices;
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
    public JsonBlockProperty SetupAttributeProperty (Transaction tr, BlockReference blockReference, JsonBlockProperty jsonProperty)
    {
      AttributeCollection attCol = blockReference.AttributeCollection;
      foreach (ObjectId attId in attCol)
      {
        AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);

        if (attRef.Tag == "TAG") { jsonProperty.Attributes.Tag = attRef.TextString; continue; }
        if (attRef.Tag == "AREA_CODE") { jsonProperty.Attributes.AreaCode = attRef.TextString; continue; }
        if (attRef.Tag == "MANUFACTURER") { jsonProperty.Attributes.Manufacturer = attRef.TextString; continue; }
        if (attRef.Tag == "MODEL") { jsonProperty.Attributes.Model = attRef.TextString; continue; }
        if (attRef.Tag == "INSTALLED_POWER") { jsonProperty.Attributes.InstalledPower = attRef.TextString; continue; }
        if (attRef.Tag == "POWER_INSTALLED") { jsonProperty.Attributes.PowerInstalled = attRef.TextString; continue; }
        if (attRef.Tag == "CONSUMED_POWER") { jsonProperty.Attributes.ConsumedPower = attRef.TextString; continue; }
        if (attRef.Tag == "POWER_CONSUMED") { jsonProperty.Attributes.PowerConsumed = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_COVER") { jsonProperty.Attributes.MaterialCover = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BARS") { jsonProperty.Attributes.MaterialBars = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_FIXED") { jsonProperty.Attributes.MaterialFixed = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_FRAME") { jsonProperty.Attributes.MaterialFrame = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_HOUSING") { jsonProperty.Attributes.MaterialHousing = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL") { jsonProperty.Attributes.Material = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SCREW_LINER") { jsonProperty.Attributes.MaterialScrewLiner = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_CARPENTRY") { jsonProperty.Attributes.MaterialCarpentry = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BODY") { jsonProperty.Attributes.MaterialBody = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_GEAR") { jsonProperty.Attributes.MaterialGear = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SHAFT") { jsonProperty.Attributes.MaterialShaft = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ROTOR") { jsonProperty.Attributes.MaterialRotor = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SUBSURFACE") { jsonProperty.Attributes.MaterialSubsurface = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ABOVE_WATER") { jsonProperty.Attributes.MaterialAboveWater = attRef.TextString; continue; }
        if (attRef.Tag == "CHANNELWIDTH") { jsonProperty.Attributes.ChannelWidth = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FLOW") { jsonProperty.Attributes.SpFlow = attRef.TextString; continue; }
        if (attRef.Tag == "SP_SPACING") { jsonProperty.Attributes.SpSpacing = attRef.TextString; continue; }
        if (attRef.Tag == "SP_ACTUATED") { jsonProperty.Attributes.SpActuated = attRef.TextString; continue; }
        if (attRef.Tag == "SP_CAPACITY") { jsonProperty.Attributes.SpCapacity = attRef.TextString; continue; }
        if (attRef.Tag == "SP_HEAD") { jsonProperty.Attributes.SpHead = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TSS_INLET") { jsonProperty.Attributes.SpTssInlet = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TSS_OUTLET") { jsonProperty.Attributes.SpTssOutlet = attRef.TextString; continue; }
        if (attRef.Tag == "SP_DIAMETER") { jsonProperty.Attributes.SpDiameter = attRef.TextString; continue; }
        if (attRef.Tag == "SP_PRESSURE") { jsonProperty.Attributes.SpPressure = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_SPACING") { jsonProperty.Attributes.UnitSpacing = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_FLOW") { jsonProperty.Attributes.UnitFlow = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_HEAD") { jsonProperty.Attributes.UnitHead = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CAPACITY") { jsonProperty.Attributes.UnitCapacity = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TSS_INLET") { jsonProperty.Attributes.UnitTssInlet = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TSS_OUTLET") { jsonProperty.Attributes.UnitTssOutlet = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_DIAMETER") { jsonProperty.Attributes.UnitDiameter = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_PRESSURE") { jsonProperty.Attributes.UnitPressure = attRef.TextString; continue; }
        if (attRef.Tag == "RUNNINGHOURS") { jsonProperty.Attributes.RunningHours = attRef.TextString; continue; }
        if (attRef.Tag == "EQUIP_TYPE") { jsonProperty.Attributes.EquipType = attRef.TextString; continue; }
        if (attRef.Tag == "BLOWER_TYPE") { jsonProperty.Attributes.BlowerType = attRef.TextString; continue; }
        if (attRef.Tag == "NOTE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "NOTE_CHINESE") { jsonProperty.Attributes.NoteChinese = attRef.TextString; continue; }
        if (attRef.Tag == "NAME1") { jsonProperty.Attributes.Name1 = attRef.TextString; continue; }
        if (attRef.Tag == "NAME2") { jsonProperty.Attributes.Name2 = attRef.TextString; continue; }
        if (attRef.Tag == "STB_DTY") { jsonProperty.Attributes.StandByDuty = attRef.TextString; continue; }
        if (attRef.Tag == "DI") { jsonProperty.Attributes.DI = attRef.TextString; continue; }
        if (attRef.Tag == "DO") { jsonProperty.Attributes.DO = attRef.TextString; continue; }
        if (attRef.Tag == "AI") { jsonProperty.Attributes.AI = attRef.TextString; continue; }
        if (attRef.Tag == "AO") { jsonProperty.Attributes.AO = attRef.TextString; continue; }
        if (attRef.Tag == "PB") { jsonProperty.Attributes.PB = attRef.TextString; continue; }
        if (attRef.Tag == "PO") { jsonProperty.Attributes.PO = attRef.TextString; continue; }
        if (attRef.Tag == "TAG_ID") { jsonProperty.Attributes.TagId = attRef.TextString; continue; }
        if (attRef.Tag == "PROCESSUNITAREA") { jsonProperty.Attributes.ProcessUnitArea = attRef.TextString; continue; }
        if (attRef.Tag == "VOLUME") { jsonProperty.Attributes.Volume = attRef.TextString; continue; }
        if (attRef.Tag == "LIQUIDLEVEL") { jsonProperty.Attributes.LiquidLevel = attRef.TextString; continue; }
        if (attRef.Tag == "LENGTH") { jsonProperty.Attributes.Length = attRef.TextString; continue; }
        if (attRef.Tag == "HEIGHT") { jsonProperty.Attributes.Height = attRef.TextString; continue; }
        if (attRef.Tag == "WIDTH") { jsonProperty.Attributes.Width = attRef.TextString; continue; }
        if (attRef.Tag == "PRESSURE") { jsonProperty.Attributes.Pressure = attRef.TextString; continue; }
        if (attRef.Tag == "SIZE") { jsonProperty.Attributes.Size = attRef.TextString; continue; }
        if (attRef.Tag == "INSTALLATION") { jsonProperty.Attributes.Installation = attRef.TextString; continue; }
        if (attRef.Tag == "FC_MOD") { jsonProperty.Attributes.FcMod = attRef.TextString; continue; }
        if (attRef.Tag == "FC_MAN") { jsonProperty.Attributes.FcMan = attRef.TextString; continue; }
        if (attRef.Tag == "PUMP_TYPE") { jsonProperty.Attributes.PumpType = attRef.TextString; continue; }
        if (attRef.Tag == "FILTER") { jsonProperty.Attributes.Filter = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_WIDTH") { jsonProperty.Attributes.SP_WIDTH = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_LEVEL") { jsonProperty.Attributes.SP_LEVEL = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_BOARD") { jsonProperty.Attributes.SP_BOARD = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_LENGTH") { jsonProperty.Attributes.SP_LENGTH = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_WIDTH") { jsonProperty.Attributes.UNIT_WIDTH = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_LEVEL") { jsonProperty.Attributes.UNIT_LEVEL = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_BOARD") { jsonProperty.Attributes.UNIT_BOARD = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_LENGTH") { jsonProperty.Attributes.UNIT_LENGTH = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_INLET") { jsonProperty.Attributes.SP_INLET = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_OUTLET") { jsonProperty.Attributes.SP_OUTLET = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_INLET") { jsonProperty.Attributes.UNIT_INLET = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_OUTLET") { jsonProperty.Attributes.UNIT_OUTLET = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CHANNELH") { jsonProperty.Attributes.SP_CHANNELH = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CHANNELW") { jsonProperty.Attributes.SP_CHANNELW = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_SEALING") { jsonProperty.Attributes.MATERIAL_SEALING = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_STEM") { jsonProperty.Attributes.MATERIAL_STEM = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CHANNELW") { jsonProperty.Attributes.UNIT_CHANNELW = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CHANNELH") { jsonProperty.Attributes.UNIT_CHANNELH = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_WATER_LEVEL") { jsonProperty.Attributes.SP_WATER_LEVEL = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_WATER_LEVEL") { jsonProperty.Attributes.UNIT_WATER_LEVEL = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLOWMAX") { jsonProperty.Attributes.SP_FLOWMAX = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLOWMIN") { jsonProperty.Attributes.SP_FLOWMIN = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FLOWMAX") { jsonProperty.Attributes.UNIT_FLOWMAX = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FLOWMIN") { jsonProperty.Attributes.UNIT_FLOWMIN = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_ABOVE") { jsonProperty.Attributes.MATERIAL_ABOVE = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_CLARIFIER_DIA") { jsonProperty.Attributes.SP_CLARIFIER_DIA = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_OPERATION_LEVEL") { jsonProperty.Attributes.SP_OPERATION_LEVEL = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FREE_BOARD") { jsonProperty.Attributes.SP_FREE_BOARD = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_CLARIFIER") { jsonProperty.Attributes.UNIT_CLARIFIER = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_OPERATION_LEVEL") { jsonProperty.Attributes.UNIT_OPERATION_LEVEL = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_FREE_BOARD") { jsonProperty.Attributes.UNIT_FREE_BOARD = attRef.TextString; continue; }
        //if (attRef.Tag == "FLOW") { jsonProperty.Attributes.FLOW = attRef.TextString; continue; }
        //if (attRef.Tag == "DIAMETER") { jsonProperty.Attributes.DIAMETER = attRef.TextString; continue; }
        //if (attRef.Tag == "POWER") { jsonProperty.Attributes.POWER = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKW") { jsonProperty.Attributes.SP_TANKW = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKL") { jsonProperty.Attributes.SP_TANKL = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKD") { jsonProperty.Attributes.SP_TANKD = attRef.TextString; continue; }
        //if (attRef.Tag == "MATERIAL_BLADE") { jsonProperty.Attributes.MATERIAL_BLADE = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_TANKV") { jsonProperty.Attributes.SP_TANKV = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANKV") { jsonProperty.Attributes.UNIT_TANKV = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANKD") { jsonProperty.Attributes.UNIT_TANKD = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANKL") { jsonProperty.Attributes.UNIT_TANKL = attRef.TextString; continue; }
        //if (attRef.Tag == "UNIT_TANW") { jsonProperty.Attributes.UNIT_TANW = attRef.TextString; continue; }
        //if (attRef.Tag == "NUMBER_OF_UNIT") { jsonProperty.Attributes.NUMBER_OF_UNIT = attRef.TextString; continue; }
        //if (attRef.Tag == "SP_FLUID") { jsonProperty.AttributesSP_FLUID = attRef.TextString; continue; }
        //if (attRef.Tag == "TYP") { jsonProperty.Attributes.TYP = attRef.TextString; continue; }
        if (attRef.Tag == "HOST_NAME") { jsonProperty.Attributes.HostName = attRef.TextString; continue; }
      }

      return jsonProperty;
    }
  }
}

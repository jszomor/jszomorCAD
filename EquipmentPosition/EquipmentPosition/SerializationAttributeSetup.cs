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

        #region
        //string[] blockAttributeNames = new string[]
        //{
        //  #region // content
        //  "TAG",
        //  "AREA_CODE",
        //  "MANUFACTURER",
        //  "MODEL",
        //  "INSTALLED_POWER",
        //  "MATERIAL_COVER",
        //  "CHANNELWIDTH",
        //  "SP_FLOW",
        //  "SP_SPACING",
        //  "RUNNINGHOURS",
        //  "EQUIP_TYPE",
        //  "NOTE",
        //  "NOTE_CHINESE",
        //  "MATERIAL_BARS",
        //  "MATERIAL_FIXED",
        //  "MATERIAL_FRAME",
        //  "CONSUMED_POWER",
        //  "STB_DTY",
        //  "DI",
        //  "DO",
        //  "AI",
        //  "AO",
        //  "PB",
        //  "PO",
        //  "TAG_ID",
        //  "UNIT_SPACING",
        //  "UNIT_FLOW",
        //  "PROCESSUNITAREA",
        //  "VOLUME",
        //  "LIQUIDLEVEL",
        //  "LENGTH",
        //  "HEIGHT",
        //  "HEIGHT",
        //  "WIDTH",
        //  "NAME1",
        //  "NAME2",
        //  "PRESSURE",
        //  "MATERIAL",
        //  "SIZE",
        //  "SP_ACTUATED",
        //  "MATERIAL_HOUSING",
        //  "SP_CAPACITY",
        //  "SP_HEAD",
        //  "INSTALLATION",
        //  "FC_MOD",
        //  "FC_MAN",
        //  "RUNNINGHOURS",
        //  "PUMP_TYPE",
        //  "UNIT_HEAD",
        //  "UNIT_CAPACITY",
        //  "MATERIAL_CARPENTRY",
        //  "MATERIAL_SCREW_LINER",
        //  "POWER_CONSUMED",
        //  "POWER_INSTALLED",
        //  "FILTER",
        //  "SP_TSS_INLET",
        //  "SP_TSS_OUTLET",
        //  "UNIT_TSS_INLET",
        //  "UNIT_TSS_OUTLET",
        //  "SP_DIAMETER",
        //  "UNIT_DIAMETER",
        //  "MATERIAL_BODY",
        //  "MATERIAL_GEAR",
        //  "MATERIAL_SHAFT",
        //  "MATERIAL_ROTOR",
        //  "BLOWER_TYPE",
        //  "SP_PRESSURE",
        //  "UNIT_PRESSURE",
        //  "MATERIAL_SUBSURFACE",
        //  "MATERIAL_ABOVE_WATER",
        //  "SP_WIDTH",
        //  "SP_LEVEL",
        //  "SP_BOARD",
        //  "SP_LENGTH",
        //  "UNIT_WIDTH",
        //  "UNIT_LEVEL",
        //  "UNIT_BOARD",
        //  "UNIT_LENGTH",
        //  "SP_INLET",
        //  "SP_OUTLET",
        //  "UNIT_INLET",
        //  "UNIT_OUTLET",
        //  "SIZE",


        //  #endregion
        //};

        //var attributesList = Attributes.ToList();
        //int counter = 0;

        //foreach (var i in blockAttributeNames)
        //{
        //  if (attRef.Tag == i) { jsonProperty.Attributes.Note = attRef.TextString; }
        //  counter++;
        //}

        #endregion

        if (attRef.Tag == "TAG") { jsonProperty.Attributes.Tag = attRef.TextString; continue; }
        if (attRef.Tag == "AREA_CODE") { jsonProperty.Attributes.AREA_CODE = attRef.TextString; continue; }
        if (attRef.Tag == "MANUFACTURER") { jsonProperty.Attributes.MANUFACTURER = attRef.TextString; continue; }
        if (attRef.Tag == "MODEL") { jsonProperty.Attributes.MODEL = attRef.TextString; continue; }
        if (attRef.Tag == "INSTALLED_POWER") { jsonProperty.Attributes.INSTALLED_POWER = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_COVER") { jsonProperty.Attributes.MATERIAL_COVER = attRef.TextString; continue; }
        if (attRef.Tag == "CHANNELWIDTH") { jsonProperty.Attributes.CHANNELWIDTH = attRef.TextString; continue; }
        if (attRef.Tag == "SP_FLOW") { jsonProperty.Attributes.NoSP_FLOWte = attRef.TextString; continue; }
        if (attRef.Tag == "SP_SPACING") { jsonProperty.Attributes.SP_SPACING = attRef.TextString; continue; }
        if (attRef.Tag == "RUNNINGHOURS") { jsonProperty.Attributes.RUNNINGHOURS = attRef.TextString; continue; }
        if (attRef.Tag == "EQUIP_TYPE") { jsonProperty.Attributes.EQUIP_TYPE = attRef.TextString; continue; }
        if (attRef.Tag == "NOTE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "NOTE_CHINESE") { jsonProperty.Attributes.NOTE_CHINESE = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BARS") { jsonProperty.Attributes.MATERIAL_BARS = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_FIXED") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_FRAME") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "CONSUMED_POWER") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "STB_DTY") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "DI") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "DO") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "AI") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "AO") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "PB") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "PO") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "TAG_ID") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_SPACING") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_FLOW") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "PROCESSUNITAREA") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "VOLUME") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "LIQUIDLEVEL") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "LENGTH") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "HEIGHT") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "WIDTH") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "NAME1") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "NAME2") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "PRESSURE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SIZE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_ACTUATED") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_HOUSING") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_CAPACITY") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_HEAD") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "INSTALLATION") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "FC_MOD") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "FC_MAN") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "RUNNINGHOURS") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "PUMP_TYPE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_HEAD") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_CAPACITY") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_CARPENTRY") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SCREW_LINER") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "POWER_CONSUMED") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "POWER_INSTALLED") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "FILTER") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TSS_INLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_TSS_OUTLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TSS_INLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_TSS_OUTLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_DIAMETER") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_DIAMETER") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_BODY") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_GEAR") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SHAFT") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ROTOR") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "BLOWER_TYPE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_PRESSURE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_PRESSURE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_SUBSURFACE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "MATERIAL_ABOVE_WATER") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_WIDTH") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_LEVEL") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_BOARD") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_LENGTH") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_WIDTH") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_LEVEL") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_BOARD") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_LENGTH") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_INLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SP_OUTLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_INLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "UNIT_OUTLET") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
        if (attRef.Tag == "SIZE") { jsonProperty.Attributes.Note = attRef.TextString; continue; }
      }

      return jsonProperty;
    }
  }
}

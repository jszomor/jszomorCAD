using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JsonFindKey;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace EquipmentPosition
{
  public class JsonBlockSetup
  {
    public JsonClassProperty SetupBlockProperty (BlockTableRecord btr, Transaction tr,  BlockReference blockReference, JsonClassProperty jsonClassProperty)
    {
      jsonClassProperty.jsonBlockProperty.ObjectId = blockReference.ObjectId;

      var setupAttributeProperty = new SerializationAttributeSetup();
      setupAttributeProperty.SetupAttributeProperty(tr, blockReference, jsonClassProperty);

      if (!btr.IsAnonymous && !btr.IsLayout)
      jsonClassProperty.jsonBlockProperty.Misc.BlockName = btr.Name;

      jsonClassProperty.jsonBlockProperty.Geometry.X = blockReference.Position.X;
      jsonClassProperty.jsonBlockProperty.Geometry.Y = blockReference.Position.Y;

      jsonClassProperty.jsonBlockProperty.Misc.Rotation = blockReference.Rotation;
      jsonClassProperty.jsonBlockProperty.General.Layer = blockReference.Layer;

      foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
      {
        if (dbrProp.PropertyName == "Position X") { jsonClassProperty.jsonBlockProperty.Custom.TagX = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position Y") { jsonClassProperty.jsonBlockProperty.Custom.TagY = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position1 X") { jsonClassProperty.jsonBlockProperty.Custom.TagX1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position1 Y") { jsonClassProperty.jsonBlockProperty.Custom.TagY1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Angle") { jsonClassProperty.jsonBlockProperty.Custom.Angle = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Angle1") { jsonClassProperty.jsonBlockProperty.Custom.Angle1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Angle2") { jsonClassProperty.jsonBlockProperty.Custom.Angle2 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance") { jsonClassProperty.jsonBlockProperty.Custom.Distance = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance1") { jsonClassProperty.jsonBlockProperty.Custom.Distance1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance2") { jsonClassProperty.jsonBlockProperty.Custom.Distance2 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance3") { jsonClassProperty.jsonBlockProperty.Custom.Distance3 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance4") { jsonClassProperty.jsonBlockProperty.Custom.Distance4 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance5") { jsonClassProperty.jsonBlockProperty.Custom.Distance5 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Flip state") { jsonClassProperty.jsonBlockProperty.Custom.FlipState = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Flip state1") { jsonClassProperty.jsonBlockProperty.Custom.FlipState1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Try1") { jsonClassProperty.jsonBlockProperty.Custom.Try1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Try") { jsonClassProperty.jsonBlockProperty.Custom.Try = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Housing") { jsonClassProperty.jsonBlockProperty.Custom.Housing = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "TTRY") { jsonClassProperty.jsonBlockProperty.Custom.TTRY = DoubleConverter(dbrProp.Value); continue; }

        if (dbrProp.PropertyName == "Centrifugal Pump")
          jsonClassProperty.jsonBlockProperty.Custom.PumpTableValue = dbrProp.Value;

        else if (jsonClassProperty.jsonBlockProperty.Misc.BlockName == "chamber" && dbrProp.PropertyName == "Visibility")
          jsonClassProperty.jsonBlockProperty.Custom.VisibilityValue = dbrProp.Value;

        else if (dbrProp.PropertyName == "Block Table1")
          jsonClassProperty.jsonBlockProperty.Custom.BlockTableValue = dbrProp.Value;
      }
      return jsonClassProperty;
    }

    public double? DoubleConverter(object value)
    {
      if (value.GetType() != typeof(string))
      {
        double doubleValue = Convert.ToDouble(value);

        return doubleValue;
      }
      return null;
    }
  }
}

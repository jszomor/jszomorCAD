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
    public JsonBlockProperty SetupBlockProperty (BlockTableRecord btr, Transaction tr,  BlockReference blockReference)
    {
      var jsonBlockProperty = new JsonBlockProperty();

      var setupAttributeProperty = new JsonAttributeSetup();
      setupAttributeProperty.SetupAttributeProperty(tr, blockReference, jsonBlockProperty);

      if (!btr.IsAnonymous && !btr.IsLayout)
      jsonBlockProperty.Misc.BlockName = btr.Name;

      jsonBlockProperty.Geometry.X = blockReference.Position.X;
      jsonBlockProperty.Geometry.Y = blockReference.Position.Y;

      jsonBlockProperty.Misc.Rotation = blockReference.Rotation;
      jsonBlockProperty.General.Layer = blockReference.Layer;

      foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
      {
        if (dbrProp.PropertyName == "Position X") { jsonBlockProperty.Custom.TagX = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position Y") { jsonBlockProperty.Custom.TagY = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position1 X") { jsonBlockProperty.Custom.TagX1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position1 Y") { jsonBlockProperty.Custom.TagY1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Angle") { jsonBlockProperty.Custom.Angle = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Angle1") { jsonBlockProperty.Custom.Angle1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Angle2") { jsonBlockProperty.Custom.Angle2 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance") { jsonBlockProperty.Custom.Distance = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance1") { jsonBlockProperty.Custom.Distance1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance2") { jsonBlockProperty.Custom.Distance2 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance3") { jsonBlockProperty.Custom.Distance3 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance4") { jsonBlockProperty.Custom.Distance4 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Distance5") { jsonBlockProperty.Custom.Distance5 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Flip state") { jsonBlockProperty.Custom.FlipState = Convert.ToInt16(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Flip state1") { jsonBlockProperty.Custom.FlipState1 = Convert.ToInt16(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Try1") { jsonBlockProperty.Custom.Try1 = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Try") { jsonBlockProperty.Custom.Try = Convert.ToString(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Housing") { jsonBlockProperty.Custom.Housing = Convert.ToString(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "TTRY") { jsonBlockProperty.Custom.TTRY = Convert.ToString(dbrProp.Value); continue; }

        if (dbrProp.PropertyName == "Centrifugal Pump")
          jsonBlockProperty.Custom.PumpTableValue = dbrProp.Value;

        else if (jsonBlockProperty.Misc.BlockName.EndsWith("chamber") && dbrProp.PropertyName == "Visibility")
          jsonBlockProperty.Custom.VisibilityValue = dbrProp.Value;

        else if (dbrProp.PropertyName == "Block Table1")
          jsonBlockProperty.Custom.BlockTableValue = dbrProp.Value;
      }
      return jsonBlockProperty;
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

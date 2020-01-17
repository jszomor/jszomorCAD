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
      string validBlockName = RealNameFinder(btr.Name);
      string validLayerName = RealNameFinder(blockReference.Layer);
      var setupAttributeProperty = new JsonAttributeSetup();
      setupAttributeProperty.SetupAttributeProperty(tr, blockReference, jsonBlockProperty);

      if (!btr.IsAnonymous && !btr.IsLayout)
      jsonBlockProperty.Misc.BlockName = validBlockName;

      jsonBlockProperty.Geometry.X = blockReference.Position.X;
      jsonBlockProperty.Geometry.Y = blockReference.Position.Y;

      jsonBlockProperty.Misc.Rotation = blockReference.Rotation;
      jsonBlockProperty.General.Layer = validLayerName;

      foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
      {
        if (dbrProp.PropertyName == "Centrifugal Pump" && jsonBlockProperty.Misc.BlockName == "pump")
          jsonBlockProperty.Custom.PumpTableValue = dbrProp.Value;

        else if (dbrProp.PropertyName == "Visibility")
        {
          string value = Convert.ToString(dbrProp.Value);
          var isNumeric = double.TryParse(value, out double n);
          if (isNumeric)
          {
            jsonBlockProperty.Custom.VisibilityValue = n;
          }
          else
            jsonBlockProperty.Custom.VisibilityValue = dbrProp.Value;
        }
        else if (dbrProp.PropertyName == "Block Table1")
        {
          if (jsonBlockProperty.Misc.BlockName == "filter")
          {
            jsonBlockProperty.Custom.BlockTableValue = DoubleConverter(dbrProp.Value) + 1; // for some reason this equipment get one less value
          }
          else
          {
            jsonBlockProperty.Custom.BlockTableValue = DoubleConverter(dbrProp.Value);
          }
        }

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
      }
      return jsonBlockProperty;
    }

    public String RealNameFinder(string originalName)
    {
      if (originalName == null) return null;
      var result = originalName;
      if (originalName.Contains("$")) result = originalName.Substring(originalName.LastIndexOf('$') + 1);
      else if (originalName == "refvalve" || originalName == "ref_valve")
      {
        result = "valve";
      }
      return result;
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

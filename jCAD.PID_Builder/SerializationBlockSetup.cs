using Autodesk.AutoCAD.DatabaseServices;
using JsonParse;
using System;

namespace jCAD.PID_Builder
{
  public class JsonBlockSetup
  {
    public JsonBlockProperty SetupBlockProperty (BlockTableRecord btr, Transaction tr,  BlockReference blockReference)
    {
      var jsonBlockProperty = new JsonBlockProperty();
      string validBlockName = RealNameFinder(btr.Name);
      string validLayerName = RealNameFinder(blockReference.Layer);
      var setupAttributeProperty = new JsonAttributeSetup();
      setupAttributeProperty.SetupAttributeProperty(tr, blockReference, jsonBlockProperty, btr);

      if (!btr.IsAnonymous && !btr.IsLayout)
      jsonBlockProperty.Misc.BlockName = validBlockName;

      jsonBlockProperty.Geometry.X = Math.Round(blockReference.Position.X,2);
      jsonBlockProperty.Geometry.Y = Math.Round(blockReference.Position.Y,2);

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
          jsonBlockProperty.Custom.BlockTableValue = DoubleConverter(dbrProp.Value);
         
        }
        else
        {
          SetDynamicValue(dbrProp, jsonBlockProperty);
        }

        #region old mapper
        //if (dbrProp.PropertyName == "Position X") { jsonBlockProperty.Custom.TagX = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Position Y") { jsonBlockProperty.Custom.TagY = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Position1 X") { jsonBlockProperty.Custom.TagX1 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Position1 Y") { jsonBlockProperty.Custom.TagY1 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Angle") { jsonBlockProperty.Custom.Angle = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Angle1") { jsonBlockProperty.Custom.Angle1 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Angle2") { jsonBlockProperty.Custom.Angle2 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Distance") { jsonBlockProperty.Custom.Distance = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Distance1") { jsonBlockProperty.Custom.Distance1 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Distance2") { jsonBlockProperty.Custom.Distance2 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Distance3") { jsonBlockProperty.Custom.Distance3 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Distance4") { jsonBlockProperty.Custom.Distance4 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Distance5") { jsonBlockProperty.Custom.Distance5 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Flip state") { jsonBlockProperty.Custom.FlipState = Convert.ToInt16(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Flip state1") { jsonBlockProperty.Custom.FlipState1 = Convert.ToInt16(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Try1") { jsonBlockProperty.Custom.Try1 = DoubleConverter(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Try") { jsonBlockProperty.Custom.Try = Convert.ToString(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "Housing") { jsonBlockProperty.Custom.Housing = Convert.ToString(dbrProp.Value); continue; }
        //if (dbrProp.PropertyName == "TTRY") { jsonBlockProperty.Custom.TTRY = Convert.ToString(dbrProp.Value); continue; }
        #endregion
      }
      return jsonBlockProperty;
    }

    public string RealNameFinder(string originalName)
    {
      if (originalName == null) return null;
      var result = originalName;

      if (originalName.Contains("$") && originalName.StartsWith("RefA"))
      {
        result = originalName.Substring(0, originalName.LastIndexOf('$') - 2);
      }

      else if (originalName.Contains("$"))
      {
        result = originalName.Substring(originalName.LastIndexOf('$') + 1);        
      }

      else if (originalName == "refvalve" || originalName == "ref_valve")
      {
        result = "valve";
      }

      return result;
    }

    public double? DoubleConverter(object value)
    {
      if (value.GetType() == typeof(double))
      {
        double doubleValue =  Convert.ToDouble(value);

        return Math.Round(doubleValue,2);
      }
      return null;
    }

    private void SetDynamicValue(DynamicBlockReferenceProperty dbrProp, JsonBlockProperty block)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties = block.Custom.GetType().GetProperties();
      foreach (var prop in properties)
      {
        var customAttributes = prop
          .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (dbrProp.PropertyName == jsonTagName & !dbrProp.PropertyName.StartsWith("Flip state"))
          {
            prop.SetValue(block.Custom, DoubleConverter(dbrProp.Value)); //serialization
            break;
          }
          else if (dbrProp.PropertyName == jsonTagName & dbrProp.PropertyName.StartsWith("Flip state"))
          {
            prop.SetValue(block.Custom, dbrProp.Value); //serialization
            break;
          }
        }
      }
    }
  }
}

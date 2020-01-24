using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JsonFindKey;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace JsonEnumerate
{
  public class JsonBlockEnumerate
  {
    public JsonBlockClass SetupBlockProperty (BlockTableRecord btr, BlockReference blockReference, JsonBlockClass jsonProperty)
    {
      //var custom = new JsonBlockClass.Custom();
      var Custom = new List<JsonBlockClass.Custom>();
      //Custom.ToList();


      if (!btr.IsAnonymous && !btr.IsLayout)
      jsonProperty.Misc.BlockName = btr.Name;

      jsonProperty.Geometry.X = blockReference.Position.X;
      jsonProperty.Geometry.Y = blockReference.Position.Y;

      jsonProperty.Misc.Rotation = blockReference.Rotation;
      jsonProperty.General.Layer = blockReference.Layer;

      foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
      {
        if (dbrProp.PropertyName == "Position X") { Custom[i] = DoubleConverter(dbrProp.Value); continue; }
        if (dbrProp.PropertyName == "Position Y") { jsonProperty.Custom.TagY = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Angle") { jsonProperty.Custom.Angle = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Angle1") { jsonProperty.Custom.Angle1 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Angle2") { jsonProperty.Custom.Angle2 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Distance") { jsonProperty.Custom.Distance = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Distance1") { jsonProperty.Custom.Distance1 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Distance2") { jsonProperty.Custom.Distance2 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Distance3") { jsonProperty.Custom.Distance3 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Distance4") { jsonProperty.Custom.Distance4 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Distance5") { jsonProperty.Custom.Distance5 = DoubleConverter(dbrProp.Value); }
        if (dbrProp.PropertyName == "Flip state") { jsonProperty.Custom.FlipState = DoubleConverter(dbrProp.Value); }

        if (dbrProp.PropertyName == "Centrifugal Pump")
          jsonProperty.Custom.PumpTableValue = dbrProp.Value;        

        else if (jsonProperty.Misc.BlockName == "chamber" && dbrProp.PropertyName == "Visibility")        
          jsonProperty.Custom.VisibilityValue = dbrProp.Value;        

        else if (dbrProp.PropertyName == "Block Table1")        
          jsonProperty.Custom.BlockTableValue = dbrProp.Value;        
      }
      return jsonProperty;
    }

    public double? DoubleConverter(object value)
    {
      if (value.GetType() == typeof(double))
      {
        double doubleValue = Convert.ToDouble(value);

        return doubleValue;
      }
      return null;
    }
  }
}

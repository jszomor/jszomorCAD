using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JsonFindKey;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace EquipmentPosition
{
  public class JsonSeriSetup
  {
    public void JsonStringBuilderSetup (BlockTableRecord btr, BlockReference blockReference)
    {
      var properties = new JsonStringBuilderProperty();
      //var jsonProperty = new JsonBlockProperty();
      #region
      //var properties = new JsonStringBuilderProperty(blockName: "", visibilityName: "", visibilityValue: 11);
      var stringBuilderSerialize = new JsonStringBuilderSerialize();
      //var jsonSerialize = new JsonWrite();

      if (!btr.IsAnonymous && !btr.IsLayout)
        properties.BlockName = btr.Name;

      if (btr.Name == "break")
        properties.BlockName = "break";

      properties.BlockX = blockReference.Position.X;
      properties.BlockY = blockReference.Position.Y;

      properties.Rotation = blockReference.Rotation;
      properties.LayerName = blockReference.Layer;

      foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
      {

        if (dbrProp.PropertyName == "Position X") { properties.TagX = dbrProp.Value; }

        if (dbrProp.PropertyName == "Position Y") { properties.TagY = dbrProp.Value; }

        if (dbrProp.PropertyName == "Angle") { properties.Angle = dbrProp.Value; }

        if (dbrProp.PropertyName == "Angle1") { properties.Angle1 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Angle2") { properties.Angle1 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Distance") { properties.Distance = dbrProp.Value; }

        if (dbrProp.PropertyName == "Distance1") { properties.Distance1 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Distance2") { properties.Distance2 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Distance3") { properties.Distance3 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Distance4") { properties.Distance4 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Distance5") { properties.Distance5 = dbrProp.Value; }

        if (dbrProp.PropertyName == "Flip state") { properties.FlipState = dbrProp.Value; }


        var visibilityConnectDict = new Dictionary<string, string>()
            {
              { "pump","Centrifugal Pump"},
              { "chamber", "Visibility"},
              { "visibility","Block Table1"},
            };

        foreach (var i in visibilityConnectDict)
        {
          if (properties.BlockName == visibilityConnectDict.Keys.ElementAt(0) &&
            dbrProp.PropertyName == visibilityConnectDict.Values.ElementAt(0))
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else if (properties.BlockName == visibilityConnectDict.Keys.ElementAt(1) &&
            dbrProp.PropertyName == visibilityConnectDict.Values.ElementAt(1))
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else if (/*dbrProp.PropertyName == visibilityConnectDict.Keys.ElementAt(2) && */
            dbrProp.PropertyName == visibilityConnectDict.Values.ElementAt(2))
          {
            properties.VisibilityName = visibilityConnectDict.Values.ElementAt(2);
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else
            continue;
        }

      }

      if (btr.Name != null)
        System.Diagnostics.Debug.Print($"blockname: {btr.Name}");

      if (!btr.IsAnonymous && !btr.IsLayout && btr.Name != null)
      {
        stringBuilderSerialize.StringBuilderSerialize(properties);
      }
      //if (jsonProperty != null)
      //{
      //  jsonSerialize.JsonSeri(jsonProperty);
      //}
      #endregion
    }

  }
}

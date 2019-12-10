using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using JsonFindKey;
using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentPosition
{
  public class JsonSeriSetup
  {
    public void JsonStringBuilderSetup(BlockTableRecord blockDefinition, BlockReference blockReference)
    {
      var properties = new JsonStringBuilderProperty();
      #region
      //var properties = new JsonStringBuilderProperty(blockName: "", visibilityName: "", visibilityValue: 11);
      var stringBuilderSerialize = new JsonStringBuilderSerialize();
      foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
      {
        if (!blockDefinition.IsAnonymous && !blockDefinition.IsLayout)
        {
          properties.BlockName = blockDefinition.Name;
        }

        if (dbrProp.PropertyName == "Position X")
        {
          properties.TagX = Convert.ToDouble(dbrProp.Value);
        }

        if (dbrProp.PropertyName == "Position Y")
        {
          properties.TagY = Convert.ToDouble(dbrProp.Value);
        }

        var point3d = new Point3d();
        if(properties.BlockName == "valve")
        {
          properties.BlockX = point3d.X;
          properties.BlockY = point3d.Y;
        }

        properties.Rotation = blockReference.Rotation;

        string[] blockNameArray = new string[] { "pump", "chamber", "mixer", "valve", "blower" };
        string[] visArray = new string[] { "Centrifugal Pump", "Visibility", "Block Table1" };

        foreach (var i in visArray)
        {
          if (properties.BlockName == blockNameArray[0] && dbrProp.PropertyName == visArray[0])
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else if (properties.BlockName == blockNameArray[1] && dbrProp.PropertyName == visArray[1])
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else if (properties.BlockName == blockNameArray[2] && dbrProp.PropertyName == visArray[2])
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else if (properties.BlockName == blockNameArray[3] && dbrProp.PropertyName == visArray[2])
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else if (properties.BlockName == blockNameArray[4] && dbrProp.PropertyName == visArray[2])
          {
            properties.VisibilityName = dbrProp.PropertyName;
            properties.VisibilityValue = dbrProp.Value;
            break;
          }
          else
            continue;
        }
      }

      if (properties != null)
        stringBuilderSerialize.StringBuilderSerialize(properties);
      #endregion
    }

  }
}

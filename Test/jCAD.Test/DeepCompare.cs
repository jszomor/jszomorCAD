using JsonFindKey;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace jCAD.Test
{
  public static class DeepEx
  {
    public static bool LineTypeComparer(JsonLineProperty line1, JsonLineProperty line2)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = line1.Type;//.GetType().GetProperties();
      var properties2 = line2.Type;//.GetType().GetProperties();
      if (properties1.Length != properties2.Length ||
          properties1.ToString() != properties2.ToString())
      {
        //MessageBox.Show(line1.Internal_Id.ToString());
        return false;
      }


      //for (int i = 0; i < properties1.Length; i++)
      //{
      //  var customAttributes = properties1[i]
      //     .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
      //  if (customAttributes.Length == 1)
      //  {
      //    var jsonProp = customAttributes[0];
      //    var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
      //    //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
      //    if (jsonTagName != null)
      //    {
      //      var propValue1 = properties1[i].GetValue(line1.Type);
      //      var propValue2 = properties2[i].GetValue(line2.Type);
      //      if (propValue1 == null || propValue2 == null) continue;
      //      if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
      //      {
      //        if (propValue1.ToString() != propValue2.ToString())
      //        {
      //          return false;
      //        }
      //      }
      //    }
      //  }
      //}
      return true;
    }
    
    public static bool LinePointsComparer(JsonLineProperty line1, JsonLineProperty line2)
    {
      for(int j = 0; j < line1.LinePoints.Count; j++)
      {
        var properties1 = line1.LinePoints[j].GetType().GetProperties();
        var properties2 = line2.LinePoints[j].GetType().GetProperties();
        if (properties1.Length != properties2.Length) return false;
        for (int i = 0; i < properties1.Length; i++)
        {
          var customAttributes = properties1[i]
             .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
          if (customAttributes.Length == 1)
          {
            var jsonProp = customAttributes[0];
            var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
            //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
            if (jsonTagName != null)
            {
              var propValue1 = properties1[i].GetValue(line1.LinePoints[j]);
              var propValue2 = properties2[i].GetValue(line2.LinePoints[j]);
              if (propValue1 == null || propValue2 == null) continue;
              if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
              {
                if (propValue1.ToString() != propValue2.ToString())
                {
                  return false;
                }
              }
            }
          }
        }
      }
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      
      return true;
    }
    public static bool BlockGeometryCompare(JsonBlockProperty block1, JsonBlockProperty block2)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = block1.Geometry.GetType().GetProperties();
      var properties2 = block2.Geometry.GetType().GetProperties();
      if (properties1.Length != properties2.Length) return false;
      for (int i = 0; i < properties1.Length; i++)
      {
        var customAttributes = properties1[i]
           .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (jsonTagName != null)
          {
            var propValue1 = properties1[i].GetValue(block1.Geometry);
            var propValue2 = properties2[i].GetValue(block2.Geometry);
            if (propValue1 == null || propValue2 == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
            {
              if (propValue1.ToString() != propValue2.ToString())
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    public static bool BlockMiscCompare(JsonBlockProperty block1, JsonBlockProperty block2)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = block1.Misc.GetType().GetProperties();
      var properties2 = block2.Misc.GetType().GetProperties();
      if (properties1.Length != properties2.Length) return false;
      for (int i = 0; i < properties1.Length; i++)
      {
        var customAttributes = properties1[i]
           .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (jsonTagName != null)
          {
            var propValue1 = properties1[i].GetValue(block1.Misc);
            var propValue2 = properties2[i].GetValue(block2.Misc);
            if (propValue1 == null || propValue2 == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
            {
              if (propValue1.ToString() != propValue2.ToString())
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    public static bool BlockGeneralCompare(JsonBlockProperty block1, JsonBlockProperty block2)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = block1.General.GetType().GetProperties();
      var properties2 = block2.General.GetType().GetProperties();
      if (properties1.Length != properties2.Length) return false;
      for (int i = 0; i < properties1.Length; i++)
      {
        var customAttributes = properties1[i]
           .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (jsonTagName != null)
          {
            var propValue1 = properties1[i].GetValue(block1.General);
            var propValue2 = properties2[i].GetValue(block2.General);
            if (propValue1 == null || propValue2 == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
            {
              if (propValue1.ToString() != propValue2.ToString())
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    public static bool BlockCustomCompare(JsonBlockProperty block1, JsonBlockProperty block2)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = block1.Custom.GetType().GetProperties();
      var properties2 = block2.Custom.GetType().GetProperties();
      if (properties1.Length != properties2.Length) return false;
      for (int i = 0; i < properties1.Length; i++)
      {
        var customAttributes = properties1[i]
           .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (jsonTagName != null)
          {
            var propValue1 = properties1[i].GetValue(block1.Custom);
            var propValue2 = properties2[i].GetValue(block2.Custom);
            if (propValue1 == null || propValue2 == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
            {
              if (propValue1.ToString() != propValue2.ToString())
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }
    public static bool BlockAttributesCompare(JsonBlockProperty block1, JsonBlockProperty block2)
    {      
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = block1.Attributes.GetType().GetProperties();
      var properties2 = block2.Attributes.GetType().GetProperties();
      if (properties1.Length != properties2.Length) return false;
      for (int i = 0; i < properties1.Length; i++)
      {
        var customAttributes = properties1[i]
           .GetCustomAttributes(typeof(Newtonsoft.Json.JsonPropertyAttribute), false);
        if (customAttributes.Length == 1)
        {
          var jsonProp = customAttributes[0];
          var jsonTagName = (jsonProp as Newtonsoft.Json.JsonPropertyAttribute).PropertyName;
          //System.Diagnostics.Debug.WriteLine($"\tJSONProperty Name: {jsonTagName}");
          if (jsonTagName != null)
          {
            var propValue1 = properties1[i].GetValue(block1.Attributes);
            var propValue2 = properties2[i].GetValue(block2.Attributes);
            if (propValue1 == null || propValue2 == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue1.ToString()) || !string.IsNullOrWhiteSpace(propValue2.ToString()))
            {
              if (propValue1.ToString() != propValue2.ToString())
              {
                return false;
              }
            }
          }
        }
      }
      return true;
    }    
  }
}

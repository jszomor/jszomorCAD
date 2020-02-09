using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.Test
{
  public static class DeepEx
  {
    public static void GetAttributeTextString(JsonBlockProperty block1, JsonBlockProperty block2)
    {
      //System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
      var properties1 = block1.Attributes.GetType().GetProperties();
      var properties2 = block2.Attributes.GetType().GetProperties();

      if (properties1.Length != properties2.Length) return;

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
            if (propValue1 == null) continue;
            if (!string.IsNullOrWhiteSpace(propValue1.ToString()))
            {
              if (propValue1 == propValue2)
              {
                Console.WriteLine("win");
              }
            }

            //prop.SetValue(block.Attributes, attRef.TextString); //serialization
          }
        }
      }
    }

    public static bool DeepCompare(this object obj, object another)
    {
      if (ReferenceEquals(obj, another)) return true;
      if ((obj == null) || (another == null)) return false;
      //Compare two object's class, return false if they are difference
      if (obj.GetType() != another.GetType()) return false;

      var result = true;
      //Get all properties of obj
      //And compare each other
      foreach (var property in obj.GetType().GetProperties())
      {
        var objValue = property.GetValue(obj);
        var anotherValue = property.GetValue(another);
        if (!objValue.Equals(anotherValue)) result = false;
      }

      return result;
    }

    public static bool CompareEx(this object obj, object another)
    {
      if (ReferenceEquals(obj, another)) return true;
      if ((obj == null) || (another == null)) return false;
      if (obj.GetType() != another.GetType()) return false;

      //properties: int, double, DateTime, etc, not class
      if (!obj.GetType().IsClass) return obj.Equals(another);

      var result = true;
      foreach (var property in obj.GetType().GetProperties())
      {
        var objValue = property.GetValue(obj);
        var anotherValue = property.GetValue(another);
        //Recursion
        if (!objValue.DeepCompare(anotherValue)) result = false;
      }
      return result;
    }
  }
}

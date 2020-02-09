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
    public static bool AttributesCompare(JsonBlockProperty block1, JsonBlockProperty block2)
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

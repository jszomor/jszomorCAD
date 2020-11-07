using JsonFindKey;
using JsonParse;
using System.Collections.Generic;

namespace jCAD.Test
{
	public class DeepEx
	{
		public List<object> Comments { get; set; } = new List<object>();
		public bool LineTypeComparer(JsonLineProperty line1, JsonLineProperty line2)
		{
			//System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
			var properties1 = line1.Type;//.GetType().GetProperties();
			var properties2 = line2.Type;//.GetType().GetProperties();
			if (properties1.Length != properties2.Length ||
					properties1.ToString() != properties2.ToString())
			{
				if (!Comments.Contains($"InternalId: {line1.Internal_Id}"))
				{
					Comments.Add($"InternalId: {line1.Internal_Id}");
				}
				Comments.Add(($"Type: {properties1}"));
				return false;
			}
			return true;
		}
		public bool LineCompare(JsonLineProperty line1, JsonLineProperty line2)
		{
			if (!line1.Type.Equals(line2.Type))
			{
				AddInternalIdToComments(line1.Internal_Id);
				Comments.Add($"\tType does not match: {line1.Type}");
				return false;
			}

			if (line1.LineOrCenterPoints.Count != line2.LineOrCenterPoints.Count)
			{
				AddInternalIdToComments(line1.Internal_Id);
				Comments.Add($"\tLinePoint number is not equal.");
				return false;
			}

			var localErrors = new List<string>();
			for (int j = 0; j < line1.LineOrCenterPoints.Count; j++)
			{
				var p1 = line1.LineOrCenterPoints[j];
				var p2 = line2.LineOrCenterPoints[j];

				if (p1.X != p2.X || p1.Y != p2.Y)
				{
					localErrors.Add($"\tPoint[{p1.Point}] (X1:{p1.X}|Y1:{p1.Y}) != (X2:{p2.X}|Y2:{p2.Y})"); // id
				}
			}

			if (localErrors.Count > 0)
			{
				AddInternalIdToComments(line1.Internal_Id);
				Comments.AddRange(localErrors);
				return false;
			}
			return true;
		}
		public bool BlockCompare(JsonBlockProperty block1, JsonBlockProperty block2)
		{
			var blockName1 = block1.Misc.BlockName;
			var blockName2 = block2.Misc.BlockName;
			var X1 = block1.Geometry.X;
			var Y1 = block1.Geometry.Y;
			var X2 = block2.Geometry.X;
			var Y2 = block2.Geometry.Y;
			var rotation1 = block1.Misc.Rotation;
			var rotation2 = block2.Misc.Rotation;
			var layer1 = block1.General.Layer;
			var layer2 = block2.General.Layer;

			var localErrors = new List<string>();
			if (blockName1 != blockName2)
			{
				localErrors.Add($"\tblockName:{blockName1}!={blockName2}");
			}
			if (X1 != X2 || Y1 != Y2)
			{
				localErrors.Add($"\tItem Postion:(X1:{X1}|Y1:{Y1}) != (X2:{X2}|Y2:{Y2})"); // id
			}
			if (rotation1 != rotation2)
			{
				localErrors.Add($"\trotation:{rotation1}!={rotation2}");
			}
			if (layer1 != layer2)
			{
				localErrors.Add($"\tlayer:{layer1}!={layer2}");
			}
			if (localErrors.Count > 0)
			{
				AddInternalIdToComments(block1.Attributes.Internal_Id);
				Comments.AddRange(localErrors);
				return false;
			}
			return true;
		}		
		public bool BlockCustomCompare(JsonBlockProperty block1, JsonBlockProperty block2)
		{
			//System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
			var properties1 = block1.Custom.GetType().GetProperties();
			var properties2 = block2.Custom.GetType().GetProperties();
			if (properties1.Length != properties2.Length) return false;
			var localErrors = new List<string>();
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
								localErrors.Add($"\tCustom Category:{jsonTagName}:{propValue1}!={jsonTagName}:{propValue2}");								
							}
						}
					}
				}
			}
			if (localErrors.Count > 0)
			{
				AddInternalIdToComments(block1.Attributes.Internal_Id);
				Comments.AddRange(localErrors);
				return false;
			}
			return true;
		}
		public bool BlockAttributesCompare(JsonBlockProperty block1, JsonBlockProperty block2)
		{
			//System.Diagnostics.Debug.WriteLine($"AutoCAD TAG: {attRef.Tag}");
			var properties1 = block1.Attributes.GetType().GetProperties();
			var properties2 = block2.Attributes.GetType().GetProperties();
			if (properties1.Length != properties2.Length) return false;
			var localErrors = new List<string>();
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
								localErrors.Add($"\tAttributes Category:{jsonTagName}:{propValue1}!={jsonTagName}:{propValue2}");								
							}
						}
					}
				}
			}
			if (localErrors.Count > 0)
			{
				AddInternalIdToComments(block1.Attributes.Internal_Id);
				Comments.AddRange(localErrors);
				return false;
			}
			return true;
		}

		private void AddInternalIdToComments(int internalId)
		{
			if(!Comments.Contains($"InternalId: {internalId}"))
			{
				Comments.Add($"InternalId: {internalId}");
			}
		}
	}
}

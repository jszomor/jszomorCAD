using Autodesk.AutoCAD.DatabaseServices;
using JsonFindKey;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  class SerializeDimension
  {
    public DimPoint2D ConvertAcadPoint3dToPoint2D(Autodesk.AutoCAD.Geometry.Point3d Acadpoint, int number)
    {
      return new DimPoint2D(Acadpoint.X, Acadpoint.Y, number);
    }

    public JsonDimensionProperty SetupDimensionProperty(DBObject item, JsonBlockSetup jsonBlockSetup)
    {
      JsonDimensionProperty jsonDimensionProperty = new JsonDimensionProperty();

      if (item is RotatedDimension)
      {
        var dimension = item as RotatedDimension;
        jsonDimensionProperty.XDimPoints.Add(ConvertAcadPoint3dToPoint2D(dimension.XLine1Point, 1));
        jsonDimensionProperty.XDimPoints.Add(ConvertAcadPoint3dToPoint2D(dimension.XLine2Point, 2));
        jsonDimensionProperty.XDimPoints.Add(ConvertAcadPoint3dToPoint2D(dimension.DimLinePoint, 3));
        jsonDimensionProperty.Layer = dimension.Layer;
        jsonDimensionProperty.DimensionType = "Rotated Dimension";
        jsonDimensionProperty.Internal_Id = BlockTableRead.InternalCounter;
        jsonDimensionProperty.DimensionStyle = Convert.ToString(dimension.DimensionStyle);
        jsonDimensionProperty.DimensionStyleName = dimension.DimensionStyleName;
      }
      return jsonDimensionProperty;
    }
  }
}

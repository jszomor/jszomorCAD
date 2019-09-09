using Autodesk.AutoCAD.DatabaseServices;
using System;
using System.Collections.Generic;

namespace EquipmentPosition
{
  public class EqualizationTank : EquipmentBase
  {
    const double _pumpDistance = 20.0d;
    const double _endPadding = 40.0d;
    const double _startPadding = 50.0d;

    public int NumberOfPumps { get; private set; }

    public EqualizationTank(int numberOfPumps)
    {
      NumberOfPumps = numberOfPumps;
    }

    public override string EquipmentName => "Equalization Tank";

    public override IEnumerable<InsertBlockBase> Blocks
    {
      get
      {
        #region Chamber
        yield return 
        new InsertBlockBase(
          numberOfItem: 1,
          blockName: "chamber",
          layerName: "unit",
          x: 0,
          y: 0,
          hostName: EquipmentName)
          {
          ActionToExecuteOnDynProp = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              if (dbrProp.PropertyName == "Visibility")
                dbrProp.Value = "no channel";
            }
          },
          ActionToExecuteOnAttRef = new Action<AttributeReference>[]
          {
            ar =>
            {
              //text for EQ tank - Attributes
              if (ar.Tag == "NAME1")
                ar.TextString = "EQUALIZATION";
              if (ar.Tag == "NAME2")
                ar.TextString = "TANK";
            }
          },
          ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              //setup chamber width
              if (dbrProp.PropertyName == "Distance")
                dbrProp.Value = (NumberOfPumps - 1) * _pumpDistance + _endPadding * 2; //last value is the free space for other items
              //text position for chamber
              if (dbrProp.PropertyName == "Position X")
                dbrProp.Value = ((NumberOfPumps - 1) * _pumpDistance + _endPadding * 2) / 2.0d; //place text middle of chamber horizontaly 
            }
          },
        };
        #endregion

        #region Eq Pump
        yield return 
        new InsertBlockBase(
        numberOfItem: NumberOfPumps,
        blockName: "pump",
        layerName: "equipment",
        x: _startPadding,
        y: 10.0d,
        hostName: EquipmentName)
        {
          OffsetX = _pumpDistance, // distamce between pumps
          //OffsetY = 0.0d,
          ActionToExecuteOnDynProp = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              if (dbrProp.PropertyName == "Centrifugal Pump")
                dbrProp.Value = (short)22;
            }
          },
          ActionToExecuteOnAttRef = null,
          ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              //for pumps VFD rotate
              if (dbrProp.PropertyName == "Angle1")
                dbrProp.Value = DegreeHelper.DegreeToRadian(90);

              // pumps VFD rotate
              if (dbrProp.PropertyName == "Angle2")
                dbrProp.Value = DegreeHelper.DegreeToRadian(270);
            }
          },
        };
        #endregion

        #region Jet Pump
        yield return
        new InsertBlockBase(
        numberOfItem: 1,
        blockName: "pump",
        layerName: "equipment",
        x: 20.0d,
        y: 10.0d,
        hostName: EquipmentName)
        {
          //OffsetX = 0, // distance between pumps
          //OffsetY = 0.0d,
          ActionToExecuteOnDynProp = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              if (dbrProp.PropertyName == "Centrifugal Pump")
                dbrProp.Value = (short)17;
            }
          },
          ActionToExecuteOnBlockRef = new Action<BlockReference>[]
          {
            br =>
            {
              //rotate              
              br.Rotation = DegreeHelper.DegreeToRadian(270);
            }
          },
          ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {              
              // tag horizontal positioning
              if (dbrProp.PropertyName == "Angle")
                dbrProp.Value = DegreeHelper.DegreeToRadian(90);
              if (dbrProp.PropertyName == "Position X")
                dbrProp.Value = (double)6;
              if (dbrProp.PropertyName == "Position Y")
                dbrProp.Value = (double)0;
            }
          },
        };
        #endregion
      }
    }
  }
}

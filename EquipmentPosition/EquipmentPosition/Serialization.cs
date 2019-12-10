using Autodesk.AutoCAD.DatabaseServices;
using JsonFindKey;
using System;
using System.Collections.Generic;

namespace EquipmentPosition
{
  public class Serialization : SerializationBase
  {
    public override IEnumerable<SerializationProperty> SeriProp
    {
      get
      {
        #region pump setup
        yield return
        new SerializationProperty()
        {
          ActionToExecuteOnBr = new Action<BlockReference>[]
          {
            br =>
            {
              var prop = new SerializationProperty();
              prop.Rotation = br.Rotation;
              prop.BlockName = br.BlockName;
            }
          },
          ActionToExecuteOnAttRef = new Action<AttributeReference>[]
          {
            ar =>
            {
              var prop = new SerializationProperty();
              if (ar.Tag == "NOTE")
                prop.Note =  ar.TextString;
            }
          },
          ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              var prop = new SerializationProperty();
              if (dbrProp.PropertyName == "Centrifugal Pump" && prop.BlockName == "pump")
              {
                prop.VisibilityName = dbrProp.PropertyName;
                prop.VisibilityValue = dbrProp.Value;
              }
            }
          },
        };
        #endregion

        #region chamber setup
        yield return
        new SerializationProperty()
        {
          ActionToExecuteOnBr = new Action<BlockReference>[]
          {
            br =>
            {
              var prop = new SerializationProperty();
              prop.Rotation = br.Rotation;
              prop.BlockName = br.BlockName;
            }
          },
          ActionToExecuteOnAttRef = new Action<AttributeReference>[]
          {
            ar =>
            {
              var prop = new SerializationProperty();
              if (ar.Tag == "NOTE")
                prop.Note =  ar.TextString;
            }
          },
          ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              var prop = new SerializationProperty();
              if (dbrProp.PropertyName == "Visibility" && prop.BlockName == "chamber")
              {
                prop.VisibilityName = dbrProp.PropertyName;
                prop.VisibilityValue = dbrProp.Value;
              }
            }
          },
        };
        #endregion

        #region valve setup
        yield return
        new SerializationProperty()
        {
          ActionToExecuteOnBr = new Action<BlockReference>[]
          {
            br =>
            {
              var prop = new SerializationProperty();
              prop.Rotation = br.Rotation;
              prop.BlockName = br.BlockName;
            }
          },
          ActionToExecuteOnAttRef = new Action<AttributeReference>[]
          {
            ar =>
            {
              var prop = new SerializationProperty();
              if (ar.Tag == "NOTE")
                prop.Note =  ar.TextString;
            }
          },
          ActionToExecuteOnDynPropAfter = new Action<DynamicBlockReferenceProperty>[]
          {
            dbrProp =>
            {
              var prop = new SerializationProperty();
              if (dbrProp.PropertyName == "Block Table1" && prop.BlockName == "valve")
              {
                prop.VisibilityName = dbrProp.PropertyName;
                prop.VisibilityValue = dbrProp.Value;
              }
            }
          },
        };
        #endregion
      }
    }
  }
}

using Autodesk.AutoCAD.DatabaseServices;
using EquipmentPosition;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.PID_Builder
{
  //public class BlockMapping : EquipmentBase
  //{
  //  //public void SetBlockMapping()
  //  //{
  //  //  var InsertBlockBase = new InsertBlockBase(Convert.ToString(BlockDeserialize.BlockSearch("Name")));

  //  //  //var jsonBlockProperty = new JsonBlockProperty();
  //  //  //jsonBlockProperty.ObjectId = blockReference.ObjectId;

  //  //  //if (!btr.IsAnonymous && !btr.IsLayout)
  //  //  //  jsonBlockProperty.Misc.BlockName = btr.Name;

  //  //  ////long blockName = BlockDeserialize.BlockSearch("Name");

  //  //  //foreach (DynamicBlockReferenceProperty dbrProp in blockReference.DynamicBlockReferencePropertyCollection)
  //  //  //{
  //  //  //  if (dbrProp.PropertyName == "Position X") { jsonBlockProperty.Custom.TagX = DoubleConverter(dbrProp.Value); continue; }
  //  //  //}
  //  //}

  //  //public override IEnumerable<InsertBlockBase> Blocks
  //  //{
  //  //  get
  //  //  {
  //  //    #region Eq Pump
  //  //    yield return
  //  //    new InsertBlockBase(
  //  //      //blockName: Convert.ToString(BlockDeserialize.BlockSearch("Name"))
  //  //      );
  //  //    #endregion
  //  //  }
  //  //}
  //  //public override string AreaName => throw new NotImplementedException();
  //}
}

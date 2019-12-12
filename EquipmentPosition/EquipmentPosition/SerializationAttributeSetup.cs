using Autodesk.AutoCAD.DatabaseServices;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class SerializationAttributeSetup
  {
    public JsonBlockProperty SetupAttributes (Transaction tr, BlockReference blockReference)
    {
      var jsonProperty = new JsonBlockProperty();

      AttributeCollection attCol = blockReference.AttributeCollection;
      foreach (ObjectId attId in attCol)
      {
        AttributeReference attRef = (AttributeReference)tr.GetObject(attId, OpenMode.ForRead);

        if (attRef.Tag == "NOTE") { jsonProperty.Attributes.Note = attRef.Tag; }
      }

      return jsonProperty;
    }
  }
}

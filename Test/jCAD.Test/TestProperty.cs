using JsonFindKey;
using JsonParse;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace jCAD.Test
{
  class TestProperty
  {
  //  public List<TestJsonBlockProperty> Blocks { get; } = new List<TestJsonBlockProperty>();
  //  public List<JsonLineProperty> Lines { get; } = new List<JsonLineProperty>();

  //  //[Obsolete("Use BlocksSearch method instead, as it returns multiple lements with the same name")]
  //  //public JsonBlockProperty BlockSearch(string equipmentName) => 
  //  //  Blocks.SingleOrDefault(b => b.Misc.BlockName == equipmentName);

  //  public bool IsEqual(TestProperty other)
  //  {

  //    for (int i = 0; i < Blocks.Count; i++)
  //    {
  //      if (!Blocks[i].IsIdentical(other.Blocks[i]))
  //        return false;
  //    }

  //    for (int i = 0; i < Lines.Count; i++)
  //    {
  //      if (!Lines[i].IsIdentical(other.Lines[i]))
  //        return false;
  //    }

  //    return true;
  //  }

  //  public IEnumerable<TestJsonBlockProperty> InternalIdSearch(int internalId) =>
  //    Blocks.Where(b => b.Attributes.Contains(internalId));
  //}
  //public class TestJsonBlockProperty : IComparable<TestJsonBlockProperty>
  //{
  //  public IEnumerable<Geometry> Geometries { get; }
  //  public IEnumerable<Misc> Miscs { get; }
  //  public IEnumerable<General> Generals { get; }
  //  public IEnumerable<Custom> Customs { get; }
  //  public IEnumerable<Attributes> Attributes { get; }

  //  public bool IsIdentical(TestJsonBlockProperty jsonBlockProperty)
  //  {
  //    //foreach (var i in Geometries)
  //    //{

  //    //}
  //    //throw new NotImplementedException();
  //    return true;
  //  }


  //  public int CompareTo(TestJsonBlockProperty comparePart)
  //  {
  //    // A null value means that this object is greater.
  //    if (comparePart == null)
  //      return 1;

  //    else
  //      return Attributes.Internal_Id.CompareTo(comparePart.Attributes.Internal_Id);
  //  }
  }

}

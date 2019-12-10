using System;
using System.Collections.Generic;
using System.Text;

namespace JsonFindKey
{
  public abstract class JsonPropBase
  {
    public abstract IEnumerable<JsonStringBuilderProperty> JsonProp { get; }
  }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace EquipmentPosition
{
  public abstract class SerializationBase
  {
    public abstract IEnumerable<SerializationProperty> SeriProp { get; }
  }
}

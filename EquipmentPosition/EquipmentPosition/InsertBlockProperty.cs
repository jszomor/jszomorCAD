using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EquipmentPosition
{
  public class ProcessVariables
  {
    public int NumberOfEqTanks { get; }
  }

  public class InsertBlockBase
  {
    public double X { get; set; }
    public double Y { get; set; }
    public double NumberOfItem { get; set; }
    public string BlockName { get; set; }
    public string LayerName { get; set; }
    public string TableName { get; set; }
    public string HostName { get; set; }

    public InsertBlockBase(double x, double y, double numberOfItem, string blockName, string layerName, string tableName, string hostName)
    {
      X = x;
      Y = y;
      NumberOfItem = numberOfItem;
      BlockName = blockName;
      LayerName = layerName;
      TableName = tableName;
      HostName = hostName;
    }
  }





  public class InsertBlockProperty
  {
    public double Distance { get; set; }
    public double PipeLength { get; set; }
    public object EqIndex { get; set; }    
    
  }

  //public InsertBlockProperty (double x, double y, double numberOfItem)
  //{
  //  X = y;

  //}

}

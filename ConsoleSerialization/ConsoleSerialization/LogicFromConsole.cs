using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using JsonFindKey;
using JsonParse;

namespace ConsoleSerialization
{
  class LogicFromConsole
  {
    static void Main(string[] args)
    {
      //Console.WriteLine("Enter length:");
      //double length = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter width:");
      //double width = Convert.ToDouble(Console.ReadLine());

      var jsonPID = new JsonPID();
      string fileName = "JsonPIDBuild.json";
      
      var coordiates = new Coordiates();

      coordiates.StartX = 100;
      coordiates.StartY = 90;
      coordiates.EndX = 80;
      coordiates.EndY= 70;

      jsonPID.Lines.Add(SetLines(coordiates));

      jsonPID.Blocks.Sort();
      jsonPID.Lines.Sort();
      var seralizer = new JsonStringBuilderSerialize();
      seralizer.StringBuilderSerialize(jsonPID, fileName);
    }

    public static JsonLineProperty SetLines(Coordiates coordiates)
    {
      var jsonLineProperty = new JsonLineProperty();
      jsonLineProperty.LineOrCenterPoints.Add(ReturnStartCoordinates(coordiates, 1));
      jsonLineProperty.LineOrCenterPoints.Add(ReturnEndCoordinates(coordiates, 2));
      jsonLineProperty.Type = "Autodesk.AutoCAD.DatabaseServices.Line";
      jsonLineProperty.Layer = "0";
      return jsonLineProperty;
    }

    public static Point2D ReturnStartCoordinates(Coordiates coordiates, int number)
    {
      return new Point2D(coordiates.StartX, coordiates.StartY, number);
    }

    public static Point2D ReturnEndCoordinates(Coordiates coordiates, int number)
    {
      return new Point2D(coordiates.EndX, coordiates.EndY, number);
    }
  }
}

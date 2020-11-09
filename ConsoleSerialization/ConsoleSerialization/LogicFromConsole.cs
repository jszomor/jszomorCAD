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
      //Console.WriteLine("Enter flow:");
      //double flow = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter number of tank:");
      double numberOfTanks = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter length:");
      double length = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter width:");
      double width = Convert.ToDouble(Console.ReadLine());

      Console.WriteLine("Enter wall thickness:");
      double wallThickness = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter adsorp volume:");
      //double volAdsorp = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter preselec volume:");
      //double volPreselec = Convert.ToDouble(Console.ReadLine());

      //Console.WriteLine("Enter length of decanter:");
      //double lengthOfDec = Convert.ToDouble(Console.ReadLine());

      SBRCoordinates(length, width, numberOfTanks, wallThickness);
    }
    public static void SBRCoordinates(double length, double width, double numberOfTanks, double mainWallThickness)
    {
      var jsonPID = new JsonPID();
      string fileName = "JsonPIDBuild.json";
      
      var coordiates = new Coordiates();
      
      //line1 vertical
      coordiates.StartX = 0;
      coordiates.StartY = 0;
      coordiates.EndX = 0;
      coordiates.EndY = length;
      jsonPID.Lines.Add(SetLines(coordiates));

      //line1wall
      coordiates.StartX = - mainWallThickness;
      coordiates.StartY = - mainWallThickness;
      coordiates.EndX = - mainWallThickness;
      coordiates.EndY = length + mainWallThickness;
      jsonPID.Lines.Add(SetLines(coordiates));

      //line2 horizontal
      coordiates.StartX = 0;
      coordiates.StartY = 0;
      coordiates.EndX = width;
      coordiates.EndY = 0;
      jsonPID.Lines.Add(SetLines(coordiates));

      if(numberOfTanks == 1)
      {
        //line2wall
        coordiates.StartX = - mainWallThickness;
        coordiates.StartY = - mainWallThickness;
        coordiates.EndX = width + mainWallThickness;
        coordiates.EndY = - mainWallThickness;
        jsonPID.Lines.Add(SetLines(coordiates));
      }
      else if(numberOfTanks == 2)
      {
        //line2 horizontal
        coordiates.StartX = width + mainWallThickness;
        coordiates.StartY = 0;
        coordiates.EndX = 2 * width + mainWallThickness;
        coordiates.EndY = 0;
        jsonPID.Lines.Add(SetLines(coordiates));

        //line2wall
        coordiates.StartX = - mainWallThickness;
        coordiates.StartY = - mainWallThickness;
        coordiates.EndX = 2 * width + mainWallThickness * 2;
        coordiates.EndY = - mainWallThickness;
        jsonPID.Lines.Add(SetLines(coordiates));
      }

      //line3 horizontal
      coordiates.StartX = 0;
      coordiates.StartY = length;
      coordiates.EndX = width;
      coordiates.EndY = length;
      jsonPID.Lines.Add(SetLines(coordiates));

      if(numberOfTanks == 1)
      {
        //line3wall
        coordiates.StartX = - mainWallThickness;
        coordiates.StartY = length + mainWallThickness;
        coordiates.EndX = width + mainWallThickness;
        coordiates.EndY = length + mainWallThickness;
        jsonPID.Lines.Add(SetLines(coordiates));
      }
      else if(numberOfTanks == 2)
      {
        //line3 horizontal
        coordiates.StartX = width + mainWallThickness;
        coordiates.StartY = length;
        coordiates.EndX = 2 * width + mainWallThickness;
        coordiates.EndY = length;
        jsonPID.Lines.Add(SetLines(coordiates));

        //line3wall
        coordiates.StartX = - mainWallThickness;
        coordiates.StartY = length + mainWallThickness;
        coordiates.EndX = 2 * width +  mainWallThickness * 2;
        coordiates.EndY = length + mainWallThickness;
        jsonPID.Lines.Add(SetLines(coordiates));
      }

      //line4 vertical
      coordiates.StartX = width;
      coordiates.StartY = 0;
      coordiates.EndX = width;
      coordiates.EndY = length;
      jsonPID.Lines.Add(SetLines(coordiates));

      if(numberOfTanks == 1)
      {
        //line4wall
        coordiates.StartX = width + mainWallThickness;
        coordiates.StartY = - mainWallThickness;
        coordiates.EndX = width + mainWallThickness;
        coordiates.EndY = length + mainWallThickness;
        jsonPID.Lines.Add(SetLines(coordiates));

      }
      else if (numberOfTanks == 2)
      {
        //line4.2 vertical
        coordiates.StartX = width + mainWallThickness;
        coordiates.StartY = 0;
        coordiates.EndX = width + mainWallThickness;
        coordiates.EndY = length;
        jsonPID.Lines.Add(SetLines(coordiates));

        //line5 vertical
        coordiates.StartX = 2 * width +  mainWallThickness;
        coordiates.StartY = 0;
        coordiates.EndX = 2 * width +  mainWallThickness;
        coordiates.EndY = length;
        jsonPID.Lines.Add(SetLines(coordiates));

        //line5wall
        coordiates.StartX = 2 * width + mainWallThickness * 2;
        coordiates.StartY = -mainWallThickness;
        coordiates.EndX = 2 * width + mainWallThickness * 2;
        coordiates.EndY = length + mainWallThickness;
        jsonPID.Lines.Add(SetLines(coordiates));
      }

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
      jsonLineProperty.Layer = "equipment";
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

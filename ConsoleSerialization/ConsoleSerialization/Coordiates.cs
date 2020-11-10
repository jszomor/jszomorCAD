using JsonFindKey;
using JsonParse;
using jCAD.PID_Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleSerialization
{
  public class Coordiates
  {
    public double StartX { get; set; }
    public double StartY { get; set; }
    public double EndX { get; set; }
    public double EndY { get; set; }

    public void MainZoneCoordinates(double length, double width, double numberOfTanks, double mainWallThickness, Coordiates coordiates)
    {
      var jsonPID = new JsonPID();
      string fileName = "JsonPIDBuild.json";

      StartX = 0; StartY = 0; EndX = 0; EndY = length; jsonPID.Lines.Add(SetLines(coordiates));
      StartX = -mainWallThickness; StartY = -mainWallThickness; EndX = -mainWallThickness; EndY = length + mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      StartX = 0; StartY = 0; EndX = width; EndY = 0; jsonPID.Lines.Add(SetLines(coordiates));
      if (numberOfTanks == 1)
      {
        StartX = -mainWallThickness; StartY = -mainWallThickness; EndX = width + mainWallThickness; EndY = -mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      }
      else if (numberOfTanks == 2)
      {
        StartX = width + mainWallThickness; StartY = 0; EndX = 2 * width + mainWallThickness; EndY = 0; jsonPID.Lines.Add(SetLines(coordiates));
        StartX = -mainWallThickness; StartY = -mainWallThickness; EndX = 2 * width + mainWallThickness * 2; EndY = -mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      }
      StartX = 0; StartY = length; EndX = width; EndY = length; jsonPID.Lines.Add(SetLines(coordiates));
      if (numberOfTanks == 1)
      {
        StartX = -mainWallThickness; StartY = length + mainWallThickness; EndX = width + mainWallThickness; EndY = length + mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      }
      else if (numberOfTanks == 2)
      {
        StartX = width + mainWallThickness; StartY = length; EndX = 2 * width + mainWallThickness; EndY = length; jsonPID.Lines.Add(SetLines(coordiates));
        StartX = -mainWallThickness; StartY = length + mainWallThickness; EndX = 2 * width + mainWallThickness * 2; EndY = length + mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      }
      StartX = width; StartY = 0; EndX = width; EndY = length; jsonPID.Lines.Add(SetLines(coordiates));

      if (numberOfTanks == 1)
      {
        StartX = width + mainWallThickness; StartY = -mainWallThickness; EndX = width + mainWallThickness; EndY = length + mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      }
      else if (numberOfTanks == 2)
      {
        StartX = width + mainWallThickness; StartY = 0; EndX = width + mainWallThickness; EndY = length; jsonPID.Lines.Add(SetLines(coordiates));
        StartX = 2 * width + mainWallThickness; StartY = 0; EndX = 2 * width + mainWallThickness; EndY = length; jsonPID.Lines.Add(SetLines(coordiates));
        StartX = 2 * width + mainWallThickness * 2; StartY = -mainWallThickness; EndX = 2 * width + mainWallThickness * 2; EndY = length + mainWallThickness; jsonPID.Lines.Add(SetLines(coordiates));
      }

      jsonPID.Blocks.Sort();
      jsonPID.Lines.Sort();
      var seralizer = new JsonStringBuilderSerialize();
      seralizer.StringBuilderSerialize(jsonPID, fileName);

      Console.ReadKey();
    }

    public JsonLineProperty SetLines(Coordiates coordiates)
    {
      var jsonLineProperty = new JsonLineProperty();
      jsonLineProperty.LineOrCenterPoints.Add(ReturnStartCoordinates(coordiates, 1));
      jsonLineProperty.LineOrCenterPoints.Add(ReturnEndCoordinates(coordiates, 2));
      jsonLineProperty.Type = "Autodesk.AutoCAD.DatabaseServices.Line";
      jsonLineProperty.Layer = "equipment";
      jsonLineProperty.Internal_Id = BlockTableRead.InternalCounter;
      BlockTableRead.InternalCounter++;
      return jsonLineProperty;
    }

    public Point2D ReturnStartCoordinates(Coordiates coordiates, int number)
    {
      return new Point2D(StartX, StartY, number);
    }

    public Point2D ReturnEndCoordinates(Coordiates coordiates, int number)
    {
      return new Point2D(EndX, EndY, number);
    }
  }  
}

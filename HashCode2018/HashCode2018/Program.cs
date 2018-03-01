using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace HashCode2018
{
    internal class Program
    {
	    private static string _header;
	    private static List<string> _content;
	    private static int _rows;
	    private static int _columns;
	    private static int _vehicles;
	    private static int _rides;
	    private static int _bonus;
	    private static int _steps;

		private static void Main(string[] args)
        {
	        // Example file
	        var inputFileAsString = File.ReadAllText(@".\Inputs\a_example.in"));
	        Console.WriteLine("Processing a_example file...");

	        SetupEnvironment(inputFileAsString);

			//var result = CalculateTimes();
	        Console.WriteLine("Done.\n");

		}

	    private static void SetupEnvironment(string inputFileAsString)
	    {
			var lines = inputFileAsString.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

		    _header = lines[0];
		    _content = lines.Skip(1).ToList();

		    _rows = int.Parse(_header.Split(' ').First());
		    _columns = int.Parse(_header.Split(' ').Skip(1).First());
			_vehicles = int.Parse(_header.Split(' ').Skip(1).First());
			_rides = int.Parse(_header.Split(' ').Skip(1).First());
			_bonus = int.Parse(_header.Split(' ').Skip(1).First());
			_steps = int.Parse(_header.Split(' ').Skip(1).First());

		}
	}
}

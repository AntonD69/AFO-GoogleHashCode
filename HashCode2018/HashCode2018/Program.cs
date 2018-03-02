using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;

namespace HashCode2018
{
	internal class Program
	{
		private static string _header;
		private static List<string> _content;
		private static int _rows;
		private static int _columns;
		private static int _carsAvailable;
		private static int _wantedRides;
		private static int _bonusPoints;
		private static int _totalTicks;
		private static char[,] _grid;
		private static List<WantedRide> _listOfRides = new List<WantedRide>();

		private static void Main(string[] args)
		{
			var baseInputFolder = ".\\InputFiles\\";

			//var baseFileName = "a_example"; 
			var baseFileName = "b_should_be_easy";
			//var baseFileName = "c_no_hurry";
			//var baseFileName = "d_metropolis";
			//var baseFileName = "e_high_bonus";

			// Example file
			var inputFileAsString = File.ReadAllText(Path.Combine(baseInputFolder, $"{baseFileName}.in"));
			Console.WriteLine("Processing a_example file...");
			SetupEnvironment(inputFileAsString);
			var carRideResult = CalculateTimes();
			var submissionFileContent = FormatResult(carRideResult);
			File.WriteAllText(Path.Combine(baseInputFolder, $"{baseFileName}.out"), submissionFileContent);
			Console.WriteLine($"Done with '{baseFileName}'.\n");
		}

		private static string FormatResult(List<Tuple<int, List<int>>> carRideResult)
		{
			var sb = new StringBuilder();

			foreach (var carRide in carRideResult)
			{
				var theList = string.Empty;

				foreach (var ride in carRide.Item2)
				{
					theList += ride + " ";
				}
				var line = $"{carRide.Item1} {theList.Trim()}";

				sb.AppendLine(line);
			}

			return sb.ToString();
		}

		private static List<Tuple<int, List<int>>> CalculateTimes()
		{
			var cars = new List<Car>();

			for (int i = 1; i <= _carsAvailable; i++)
			{
				cars.Add(new Car(i, new Point(0, 0)));
			}

			for (int currentTimeTick = 0; currentTimeTick < _totalTicks; currentTimeTick++)
			{
				var timeTicksLeftToEnd = _totalTicks - currentTimeTick;

				var availableRides = _listOfRides.Where(x => x.DistanceTicks < timeTicksLeftToEnd && x.IsAvailable).ToList();
				var availableCars = cars.Where(c => c.IsAvailable).ToList();

				Console.WriteLine($"Tick [{currentTimeTick}] of [{_totalTicks}]");
				Console.WriteLine($"   Cars Available : {availableCars.Count}]");
				Console.WriteLine($"   Rides Available : {availableRides.Count}]");

				foreach (var car in availableCars)
				{
					//Get the Longest Rides
					//var maxTotalTicks = 0;
					WantedRide takeRide = null;

					//foreach (var availableRide in availableRides)
					//{
					//	var checkTotalRideTicks = GetDistanceTicks(car.CurrentPosition, availableRide.FromPoint, availableRide.ToPoint);
					//	var ticksToStart = GetDistanceTicks(car.CurrentPosition, availableRide.FromPoint);

					//	if (checkTotalRideTicks > maxTotalTicks
					//		&& availableRide.EarliestStart >= (ticksToStart + currentTimeTick)
					//		&& checkTotalRideTicks + currentTimeTick < availableRide.LatestFinish
					//		&& availableRide.IsAvailable)
					//	{
					//		maxTotalTicks = checkTotalRideTicks;
					//		takeRide = availableRide;
					//	}
					//}


					//if (takeRide != null)
					//{
					//	var totalRideTicks = GetDistanceTicks(car.CurrentPosition, takeRide.FromPoint, takeRide.ToPoint);

					//	car.AddRideIndex(takeRide.Index);
					//	car.AvailableAgainAtTimeTick = totalRideTicks + currentTimeTick;
					//	car.AvailableAtPoint = takeRide.ToPoint;
					//	car.IsAvailable = false;
					//	takeRide.IsAvailable = false;
					//}

					if (takeRide == null)
					{
						//Find Closest Pickup Time.

						var ridesByTicks = availableRides.Where(x => x.IsAvailable).OrderBy(r => r.EarliestStart);

						var closestTick = GetClosestTick(currentTimeTick, ridesByTicks);

						var closestPickupPoint = ridesByTicks
							.Where(c => c.EarliestStart == closestTick)
							.OrderBy(c => c.GetStartTicks(car.CurrentPosition)).FirstOrDefault();

						if (closestPickupPoint != null)
						{
							var totalRideTicks = GetDistanceTicks(car.CurrentPosition, closestPickupPoint.FromPoint, closestPickupPoint.ToPoint);

							car.AddRideIndex(closestPickupPoint.Index);
							car.AvailableAgainAtTimeTick = totalRideTicks + currentTimeTick;
							car.AvailableAtPoint = closestPickupPoint.ToPoint;
							car.IsAvailable = false;
							closestPickupPoint.IsAvailable = false;
						}
					}
				}

				foreach (var car in cars)
				{
					if (currentTimeTick == car.AvailableAgainAtTimeTick)
					{
						car.IsAvailable = true;
						car.CurrentPosition = car.AvailableAtPoint;
					}
				}
			}

			var answer = new List<Tuple<int, List<int>>>();

			foreach (var car in cars)
			{
				answer.Add(new Tuple<int, List<int>>(car.CompletedRideIndexes.Count, car.CompletedRideIndexes));
			}

			return answer;
		}

		private static int GetClosestTick(int currentTimeTick, IOrderedEnumerable<WantedRide> ridesByTicks)
		{
			var closestTick = 20000;

			foreach (var ride in ridesByTicks)
			{
				if (ride.EarliestStart < closestTick)
				{
					closestTick = ride.EarliestStart;
				}
			}
			;
			return closestTick;
		}

		private static int GetDistanceTicks(Point carCurrentPosition, Point availableRideFromPoint, Point availableRideToPoint)
		{
			var ticksToStart = Math.Abs(availableRideFromPoint.X - carCurrentPosition.X) + Math.Abs(availableRideFromPoint.Y - carCurrentPosition.Y);
			var ticksStartToEnd = Math.Abs(availableRideToPoint.X - carCurrentPosition.X) + Math.Abs(availableRideToPoint.Y - carCurrentPosition.Y);

			return ticksToStart + ticksStartToEnd;
		}

		private static int GetDistanceTicks(Point point1, Point point2)
		{
			var numberOfTicks = Math.Abs(point1.X - point2.X) + Math.Abs(point1.Y - point2.Y);

			return numberOfTicks;
		}


		private static void SetupEnvironment(string inputFileAsString)
		{
			var lines = inputFileAsString.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

			_header = lines[0];
			_content = lines.Skip(1).ToList();

			_rows = int.Parse(_header.Split(' ').First());
			_columns = int.Parse(_header.Split(' ').Skip(1).First());
			_carsAvailable = int.Parse(_header.Split(' ').Skip(2).First());
			_wantedRides = int.Parse(_header.Split(' ').Skip(3).First());
			_bonusPoints = int.Parse(_header.Split(' ').Skip(4).First());
			_totalTicks = int.Parse(_header.Split(' ').Skip(5).First());

			_grid = new char[_columns, _rows];

			var index = 0;

			foreach (var line in _content)
			{
				var numbers = line.Split(' ').ToList();

				_listOfRides.Add(new WantedRide
				{
					Index = index++,
					IsAvailable = true,
					FromPoint = new Point(int.Parse(numbers[0]), int.Parse(numbers[1])),
					ToPoint = new Point(int.Parse(numbers[2]), int.Parse(numbers[3])),
					EarliestStart = int.Parse(numbers[4]),
					LatestFinish = int.Parse(numbers[5])
				});
			}
		}

		internal class Car
		{
			private List<int> _completedRideIndexes = new List<int>();

			public Car(int index, Point startPoint)
			{
				Index = index;
				CurrentPosition = startPoint;
				IsAvailable = true;
			}

			public List<int> CompletedRideIndexes { get { return _completedRideIndexes; } }
			public int Index { get; private set; }
			public Point CurrentPosition { get; set; }

			public void AddRideIndex(int rideIndex)
			{
				_completedRideIndexes.Add(rideIndex);
			}

			public bool IsAvailable { get; set; }
			public int AvailableAgainAtTimeTick { get; set; }
			public Point AvailableAtPoint { get; set; }
		}


		public class WantedRide
		{
			public int Index { get; set; }
			public Point FromPoint { get; set; }
			public Point ToPoint { get; set; }
			public int EarliestStart { get; set; }
			public int LatestFinish { get; set; }

			public int DistanceTicks
			{
				get { return Math.Abs(ToPoint.X - FromPoint.X) + Math.Abs(ToPoint.Y - FromPoint.Y); }
			}

			public int StartTick
			{
				get { return EarliestStart; }
			}

			public bool IsAvailable { get; set; }

			public int GetStartTicks(Point carCurrentPosition)
			{
				return Math.Abs(ToPoint.X - carCurrentPosition.X) + Math.Abs(ToPoint.Y - carCurrentPosition.Y);
			}
		}
	}
}
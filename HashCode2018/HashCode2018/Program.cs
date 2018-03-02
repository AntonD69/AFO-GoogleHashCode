using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;

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
        private static char[,] _grid;
        private static List<Ride> _listOfRides = new List<Ride>();

        private static void Main(string[] args)
        {
            var baseInputFolder = ".\\InputFiles\\";

            // Example file
            var inputFileAsString = File.ReadAllText(Path.Combine(baseInputFolder, "b_should_be_easy.in"));
            Console.WriteLine("Processing a_example file...");

            SetupEnvironment(inputFileAsString);

            var carRideResult = CalculateTimes();

            var submissionFileContent = FormatResult(carRideResult);

            File.WriteAllText(Path.Combine(baseInputFolder, "b_should_be_easy.out"), submissionFileContent);

            Console.WriteLine("Done with example.\n");
        }

        private static string FormatResult(Dictionary<int, List<int>> carRideResult)
        {
            var sb = new StringBuilder();

            foreach (var carRide in carRideResult)
            {
                var theList = string.Empty;

                foreach (var ride in carRide.Value)
                {
                    theList += ride + " ";
                }
                var line = $"{carRide.Key} {theList.Trim()}";

                sb.AppendLine(line);
            }

            return sb.ToString();

        }

        private static Dictionary<int, List<int>> CalculateTimes()
        {
            Dictionary<int, List<int>> carRideResult = new Dictionary<int, List<int>>();

            var cars = new List<Car>();

            for (int i = 0; i < _vehicles; i++)
            {
                cars.Add(new Car(i));
            }

            foreach (var ride in _listOfRides.OrderBy(((x => x.EarliestStart))).Where(x => x.EarliestStart > 0 && x.IsActive == false))
            {
                foreach (var availableCar in cars.Where(c => !c.RideInProgress))
                {
                    availableCar.RideIDs.Add(ride.RideId);
                    availableCar.RideInProgress = false;
                    availableCar.DistanceToEnd = ride.DistanceToEnd(availableCar.Position);
                    ride.IsActive = true;
                    break;
                }
            }

            for (int T = 0; T < _steps - 1; T++)
            {
                //foreach (var ride in ridesToCheck)
                //{s
                foreach (var availableCar in cars.Where(c => !c.RideInProgress))
                {
                    //Get the closest ride
                    var validRides =
                        _listOfRides.Where(x => x.EarliestStart <= T && (x.IsActive == false && x.IsCompleted == false));
                    Ride bestRide = validRides.FirstOrDefault();
                    foreach (var currentRide in validRides)
                    {

                        var BestDistance = bestRide.DistanceToStart(availableCar.Position);
                        var currentDistance = currentRide.DistanceToStart(availableCar.Position);
                        bool wtf = BestDistance >=
                                       currentDistance;
                        if (wtf)
                            bestRide = currentRide;
                    }
                    if (bestRide == null)
                        break;
                    availableCar.RideIDs.Add(bestRide.RideId);
                    availableCar.DistanceToEnd = T + bestRide.DistanceToEnd(availableCar.Position);
                    availableCar.RideInProgress = true;
                    bestRide.IsActive = true;
                    ////}

                }

                foreach (var car in cars.Where(x => x.RideInProgress))
                {
                    if (car.RideInProgress == false)
                    {
                        foreach (var rideId in car.RideIDs)
                        {
                            var currentRide = _listOfRides.Single((x => x.RideId == rideId));
                            if (T == currentRide.EarliestStart)
                            {
                                car.RideInProgress = true;
                            }
                        }
                    }

                    if (car.DistanceToEnd == T)
                    {
                        car.RideInProgress = false;
                    }

                }

            }

            //carRideResult.Add(1, new List<int> { 0 });
            //carRideResult.Add(2, new List<int> { 2, 1 });

            foreach (var car in cars)
            {
                carRideResult.Add(car.Id, car.RideIDs);
            }

            return carRideResult;
        }

        private static void SetupEnvironment(string inputFileAsString)
        {
            var lines = inputFileAsString.Split(new[] { "\n", "\r" }, StringSplitOptions.RemoveEmptyEntries);

            _header = lines[0];
            _content = lines.Skip(1).ToList();

            _rows = int.Parse(_header.Split(' ').First());
            _columns = int.Parse(_header.Split(' ').Skip(1).First());
            _vehicles = int.Parse(_header.Split(' ').Skip(2).First());
            _rides = int.Parse(_header.Split(' ').Skip(3).First());
            _bonus = int.Parse(_header.Split(' ').Skip(4).First());
            _steps = int.Parse(_header.Split(' ').Skip(5).First());

            _grid = new char[_columns, _rows];

            int counter = 0;
            foreach (var line in _content)
            {
                var numbers = line.Split(' ').ToList();

                _listOfRides.Add(new Ride
                {
                    RideId = counter++,
                    FromPoint = new Point(int.Parse(numbers[0]), int.Parse(numbers[1])),
                    ToPoint = new Point(int.Parse(numbers[2]), int.Parse(numbers[3])),
                    EarliestStart = int.Parse(numbers[4]),
                    LatestFinish = int.Parse(numbers[5])
                });
            }
        }
    }

    public class Ride
    {
        public int RideId { get; set; }
        public Point FromPoint { get; set; }
        public Point ToPoint { get; set; }
        public int EarliestStart { get; set; }
        public int LatestFinish { get; set; }
        public bool IsActive { get; set; }
        public bool IsCompleted { get; set; }

        public int DistanceToStart(Point point)
        {
            return Math.Abs(FromPoint.X - point.X) + Math.Abs(FromPoint.Y - point.Y);
        }

        public int DistanceToEnd(Point point)
        {
            return Math.Abs(ToPoint.X - point.X) + Math.Abs(ToPoint.Y - point.Y);
        }


    }

    public class Car
    {
        public Car(int index)
        {
            Id = index;
            Position = new Point(0, 0);
            RideIDs = new List<int>();
        }

        public int DistanceFromCurrent(Point point)
        {
            return Math.Abs(Position.X - point.X) + Math.Abs(Position.Y - point.Y);
        }
        public int Id { get; set; }
        public int DistanceToEnd { get; set; }

        public bool RideInProgress { get; set; }
        public List<int> RideIDs { get; set; }
        public Point Position { get; set; }
    }
}

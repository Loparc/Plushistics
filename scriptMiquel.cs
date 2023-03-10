using System;
using System.IO;
using System.Collections.Generic;

namespace WriteTextFile
{
    class Program
    {
        public struct Location {
            public string name;
            public int sharksNeeded;
            public int sharksAvailable;
        };

        public struct Path {
            public Location one;
            public Location two;
            public int fuel; 
        };

        public struct Van {
            public Location start;
            public int maxLoad;
        }

        static HashSet<Location> locations = new HashSet<Location>();
        static HashSet<Path> paths = new HashSet<Path>();
        static HashSet<Van> vans = new HashSet<Van>();

        static Location toLocation(string name, int need, int available) 
        {
            Location loc;
            loc.name = name;
            loc.sharksNeeded = need;
            loc.sharksAvailable = available;
            return loc;
        }

        static Van toVan(Location loc, int maxLoad) 
        {
            Van van;
            van.start = loc;
            van.maxLoad = maxLoad;
            return van;
        }

        static void addLocation(string name, int need, int available) 
        {
            Location loc = toLocation(name, need, available);
            locations.Add(loc);
        }

        static void addVan(Location loc, int maxLoad) {
            Van van = toVan(loc, maxLoad);
            vans.Add(van);
        }

        static void addPath(Location first, Location second, int gasNeeded)
        {
            Path path;
            path.one = first;
            path.two = second;
            path.fuel = gasNeeded;
            paths.Add(path);
        }

        static void Main(string[] args)
        {            
            addLocation("Egham", 1, 0);
            addLocation("London", 1, 0);
            addLocation("Cambridge", 1, 4);
            addLocation("Slough", 2, 1);

            addVan(toLocation("Egham", 1, 0), 3);

            Location[] locationsArray = new Location[locations.Count];
            locations.CopyTo(locationsArray);

            addPath(locationsArray[0], locationsArray[1], 1);
            addPath(locationsArray[1], locationsArray[2], 1);
            addPath(locationsArray[2], locationsArray[0], 1);
            addPath(locationsArray[3], locationsArray[0], 1);

            string text = "(define (problem one)\n" +
                            "\t(:domain plushistics)\n\n" + 
                            "\t(:objects\n" ;
            
            text += "\t\t";
            foreach (Location loc in locations) {text += loc.name + " ";}
            text += "- city\n";

            text += "\t\t";
            for (int i = 1; i <= vans.Count; ++i) {text +=  "V" + i + " ";}
            text += "- van\n\n\t)\n";

            text += "\t(:init\n" + 
                    "\t\t(= (gas) 0)\n";

            for (int i = 1; i <= vans.Count; ++i) {
                text += "\t\t(= (capacity V" + i + ") 10)\n";
                text += "\t\t(= (cargo V" + i + ") 0)\n";
                text += "\t\t(parked V" + i + " C1)\n";
            }

            foreach (Path p in paths) {
                text += "\t\t(path "+ p.one.name + " " + p.two.name +")\n";
                text += "\t\t(path "+ p.two.name + " " + p.one.name +")\n";
                text += "\t\t(= (cost "+ p.one.name + " " + p.two.name +") " + p.fuel +")\n";
                text += "\t\t(= (cost "+ p.two.name + " " + p.one.name +") " + p.fuel +")\n";
            }

            foreach (Location loc in locations) {
                text += "\t\t(= (sharks " + loc.name + ") " + loc.sharksAvailable + ")\n";
                text += "\t\t(= (sharks " + loc.name + ") " + loc.sharksNeeded + ")\n";
            }
            text += "\t)\n\n";

            text += "\t(:goal\n\t\t(forall (?c - city)\n";
            text += "\t\t\t(>= (sharks ?c) (demand ?c))\n\t)\n\n";
            text += "\t(:metric minimize\n\t\t\t(gas)\n\t)\n)";

            // Write the string to a file
            File.WriteAllText("./gen_problem.pddl", text);

            Console.WriteLine("Text written to file successfully.");
        }
    }
}
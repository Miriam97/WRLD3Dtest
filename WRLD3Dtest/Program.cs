using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

/* Miriam Jennings 
 * technical test for WRLD3D application process
 * 29/07/2019 */

namespace WRLD3Dtest
{
    class Program
    {
        //the names of the input files (in bin/Debug)
        const string bigFile = "problem_big.txt";
        const string smallFile = "problem_small.txt"; 
        static void Main(string[] args)
        {
            //send the filename to test to the function readFile()
            //comment out whichever not testing...
            readFile(smallFile);
            readFile(bigFile);
            //(bigFile takes about 13 to 14 minutes to produce a result)
        }
        public static void readFile(string file)
        {
            //check that the file can be found
            if (System.IO.File.Exists(file) == true)
            {
                System.IO.StreamReader objectReader;
                objectReader = new System.IO.StreamReader(file);
                string input;
                string[] allLines;
                //read all of file in one - if not null/empty
                if ((input = objectReader.ReadToEnd()) != null)
                {
                    //given a newline can be found in the file,
                    if (input.Contains("\n"))
                    {
                        //split the entire file by new line into positions in the array allLines
                        allLines = Regex.Split(input, "\r\n|\r|\n");
                        //make an array to store the data which will be gathered from the lines
                        Information[] allInfos = new Information[allLines.Length - 1];
                        //make a lineNum counter to index the array of points
                        int lineNum = 0;
                        //each of the lines which have been read in...
                        foreach (string line in allLines)
                        {
                            //given a space can be found in the line,
                            if (line.Contains(' '))
                            {
                                //split the line by space into positions in the array splitLine
                                string[] splitLine = line.Split(' ');
                                //construct a new instance of the Information class,
                                Information myInfo = new Information
                                {
                                    //store each piece of data found in this line
                                    name = splitLine[0],
                                    xCoord = Int32.Parse(splitLine[1]),
                                    yCoord = Int32.Parse(splitLine[2]),
                                    //and a high intial shortest distance so that any value will overwrite this when compared
                                    myCurrentMin = 1E+14F
                                };
                                //add this new instance to an array at the current index lineNum
                                allInfos[lineNum] = myInfo;
                                //increase the index counter
                                lineNum++;
                            }
                        }
                        //pass the array of data points to a function to find the nearest neighbour for each one
                        findMins(allInfos);
                    }
                }
                //close the reader
                objectReader.Close();
            }
            //if the file could not be found,
            else
            {
                //report that
                Console.WriteLine("File Not Found " + file);
                Console.ReadLine();
            }
        }
        //this is the function to get the nearest neighbour for each point in the array allInfos
        public static void findMins(Information[] allInfos)
        {
            /*this method will remove redundancy by comparing only those which have not yet been checked from the other side, and skipping self-comparisons
            for example - place0-place1 checked at the same time as place1-place0, since the same distance, and not compared in another round
                0   1   2   3 
            0   X   X   X   X
            1   O   X   X   X
            2   O   O   X   X
            3   O   O   O   X
            Where O will be checked and X will be skipped using the redundancy cutter
            In this example, reduces from 16 to 6 checks
            */
            int redundancyCutter = 1;
            //for each point in the array,
            for(int i = 0; i < allInfos.Length; i++)
            {
                Information place1 = allInfos[i];
                //compare to each other point which has not yet been checked the other way around
                for(int j = redundancyCutter; j < allInfos.Length; j++)
                {
                    Information place2 = allInfos[j];
                    //send the two points to compare to function getDist()
                    float dist = getDist(place1, place2);
                    //for each of the points, check whether this distance is shorter than the smallest distance from this point found so far,
                    //if so, replace as the current shortest distance found
                    place1.myCurrentMin = Math.Min(dist, place1.myCurrentMin);
                    place2.myCurrentMin = Math.Min(dist, place2.myCurrentMin);
                }
                //increment the counter
                redundancyCutter++;
            }
            //once all points have a shortest distance, order the array into a new list by this
            IEnumerable<Information> sorted = allInfos.OrderBy(info => info.myCurrentMin);
            //and the one with the maximum shortest distance (furthest to another point), will be the most isolated - the last in the sorted list
            //output the name of the most isolated point.
            Console.WriteLine(sorted.Last<Information>().name);
            Console.ReadLine();
        }
        public static float getDist(Information first, Information second)
        {
            //Find Euclidean distance between the two points passed in
            float sumSquared = ((float)Math.Pow((first.xCoord - second.xCoord), 2) + (float)Math.Pow((first.yCoord - second.yCoord), 2));
            // float root = (float)Math.Sqrt(sumSquared); 
            // ^ Faster to just compare results without finding square root, still relative 
            return sumSquared;
        }

        public class Information
        {
            //for each point, store the point name, x and y coords, and the shortest distance to another point found so far
            public string name { get; internal set; }
            public int xCoord { get; internal set; }
            public int yCoord { get; internal set; }
            public float myCurrentMin { get; internal set; }
        }
    }

}

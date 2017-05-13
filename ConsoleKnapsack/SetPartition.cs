using GAMultidimKnapsack;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace GAMultidimKnapsack
{
    public partial class SetPartition
    {
        static Random rand = new Random();
        static ConcurrentQueue<double> averageValuations = new ConcurrentQueue<double>();
        static ConcurrentQueue<double> maxValuations = new ConcurrentQueue<double>();
        static ConcurrentQueue<double> ages = new ConcurrentQueue<double>();

        static List<string> algorithmWithRestart(int itemsAmount, int dimensions, double maxCost, double[] restrictions, double[] costs, double[,] itemsSet)
        {
            int ConfigsAmount = 10, restartTime = 100000, currentMaxValueLiveLength = 0;
            double PrevCost = 0;
            double mutationPercent = 0.20;//FROM 0 TO 1
            GeneticalAlgorithm ga = new GeneticalAlgorithm(itemsAmount, dimensions, restrictions, costs, itemsSet, ConfigsAmount, GeneticalAlgorithm.TwoPointCrossover, GeneticalAlgorithm.SinglePointMutation, mutationPercent);
            int iterationNumber = 0, endIteration = 10000000;
            List<double> resetPoints = new List<double>();

            while (ga.GetAbsoluteMaximalKnapsackCost() != maxCost && iterationNumber < endIteration)
            {
                //var watch = new Stopwatch();
                //watch.Start();
                ga.MakeIteration();
                iterationNumber++;
                double tmp = ga.GetBestConfigsCosts()[0];
                if (tmp != PrevCost)
                {
                    PrevCost = tmp;
                    currentMaxValueLiveLength = 0;
                }
                else
                {
                    currentMaxValueLiveLength++;
                }

                //if (iterationNumber % 10000 == 0)//Отрисовка
                //{
                //    Console.Write(iterationNumber + ") ");// delta with avg is " + (maxCost - ga.GetAbsoluteAverageKnapsackCost()) + "\n delta with max is " + (maxCost - ga.GetAbsoluteMaximalKnapsackCost()));
                //    var bestCosts = ga.GetBestConfigsCosts();
                //    Console.WriteLine("Top 3 of the best configs pool are {0}, {1}, {2}, {3}, {4}",
                //        (maxCost - bestCosts[0]),
                //        (maxCost - bestCosts[1]),
                //        (maxCost - bestCosts[2]),
                //        (maxCost - bestCosts[3]),
                //        (maxCost - bestCosts[4]));
                //}

                if (currentMaxValueLiveLength == restartTime)
                {
                    var restartPercent = 0.4;
                    resetPoints.Add(maxCost - ga.GetBestConfigsCosts()[0]);
                    ga.RestartAlgorithm(restartPercent);
                    PrevCost = 0;
                    currentMaxValueLiveLength = 0;
                    //Console.WriteLine("Restart");

                }
                //  watch.Stop();
            }
            List<string> results = new List<string>();

            results.Add("Finished in " + iterationNumber);
            string tmpString = "";
            foreach (var x in resetPoints)
                tmpString += x.ToString() + ",";
            results.Add(tmpString);
            results.Add(resetPoints.Count.ToString());
            return results;
        }

        static void WriteResutls(int experimentNumber, List<string> results, string filename)
        {
            var fs = new FileStream(filename, FileMode.Append);
            using (StreamWriter sw = new StreamWriter(fs))
            {
                sw.WriteLine(experimentNumber.ToString()+')');
                foreach (var result in results)
                    sw.WriteLine(result);
                sw.WriteLine();
            }
        }

        static void ProcessTestSet(string inputFileData, string inputFileResults)//WORK WITH IT! 
        {
            using (StreamReader dataReader = new StreamReader(inputFileData))
            {
                string[] resultsArray = File.ReadAllLines(inputFileResults);
                var resultsStringNumber = 12;
                int experimentsAmount = Convert.ToInt32(dataReader.ReadLine());
                for (int experimentNumber = 0; experimentNumber < experimentsAmount; experimentNumber++, resultsStringNumber++)
                {
                    string[] initializationSequence;
                    string firstString = dataReader.ReadLine();
                    if (firstString.Trim() == "")
                        initializationSequence = dataReader.ReadLine().Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries);
                    else initializationSequence = firstString.Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries); ;
                    int itemsAmount = Convert.ToInt32(initializationSequence[0]),
                    dimensions = Convert.ToInt32(initializationSequence[1]);
                    double maxCost = Convert.ToDouble(resultsArray[resultsStringNumber].Substring(25));//Convert.ToDouble(temp);
                    List<double> tempCosts = new List<double>();
                    while (tempCosts.Count() != itemsAmount)
                        tempCosts.AddRange(dataReader
                            .ReadLine()
                            .Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => Convert.ToDouble(x))
                            .ToList());
                    double[] costs = tempCosts.ToArray();
                    double[,] itemsSet = new double[itemsAmount, dimensions];
                    for (int i = 0; i < dimensions; i++)
                    {
                        int itemsReaden = 0;
                        while (itemsReaden != itemsAmount)
                        {
                            double[] currentString = dataReader.ReadLine()
                                .Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries).
                                Select(x => Convert.ToDouble(x)).
                                ToArray();
                            for (int j = itemsReaden, k = 0; j < currentString.Count() + itemsReaden; j++, k++)
                                itemsSet[j, i] = currentString[k];
                            itemsReaden += currentString.Count();
                        }
                    }
                    List<double> tempRestrictions = new List<double>();
                    while (tempRestrictions.Count() != dimensions)
                        tempRestrictions.AddRange(dataReader
                            .ReadLine()
                            .Split(" ".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                            .Select(x => Convert.ToDouble(x))
                            .ToList());
                    double[] restrictions = tempRestrictions.ToArray();
                    //some silly work with reading from file. 

                    List<string> resultsList = algorithmWithRestart(itemsAmount, dimensions, maxCost, restrictions, costs, itemsSet);
                    WriteResutls(experimentNumber, resultsList, "results.txt");

                    Thread.Sleep(3000);
                    maxValuations.Enqueue(0);
                    averageValuations.Enqueue(0);
                }
            }
        }

        static void Main()
        {
            //new Thread(TestAlgorithm) { IsBackground = true }.Start(); 
            //new Thread(() => ProcessTestSet(@"C:\Users\black_000\Source\Repos\GeneticKnapsack\GAMultidimKnapsack\3.txt", @"C:\Users\black_000\Source\Repos\GeneticKnapsack\GAMultidimKnapsack\_res.txt")) { IsBackground = true }.Start();
            ProcessTestSet(@"C:\Users\black_000\Source\Repos\GeneticKnapsack\GAMultidimKnapsack\3.txt", @"C:\Users\black_000\Source\Repos\GeneticKnapsack\GAMultidimKnapsack\_res.txt");
        }
    }

}

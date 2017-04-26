﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Datasets;

namespace FuzzificationLibrary
{
   public abstract class Fuzzification
   {
        public DataSets DataToTransform { get; set; }
      /// <summary>
      /// dimension, interval, dataitem
      /// </summary>
        public double[][][] Results { get; private set; }
        public int[] Intervals { get; private set; }
        /// <summary>
        /// [i] - dimension,
        /// [j] - dataset value,
        /// [k] - center info =>
        /// 0 - closest center
        /// 1 - closest distance
        /// 2 - closest index
        /// 3 - data
        /// 4 - left center 
        /// 5 - right center
        /// </summary>
        public double[][][] IntervalCentersAndWidth { get; private set; }
        public double [][] Centers { get; set; }
        public int [][] ClassesInInterval { get; set; }
      
        /// <summary>
        /// Total entropy - []-dimension, [][] - interval
        /// </summary>
          public double [][] TotalEntropy { get; set; }

       protected Fuzzification(DataSets dataToTransform)
       {
           DataToTransform = dataToTransform;
       }
        
       public void Initialize()
        {
          if (DataToTransform != null )
           {
                Intervals = new int[DataToTransform.Attributes];
                Centers = new double[DataToTransform.InputAttributes][];
                TotalEntropy = new double[DataToTransform.InputAttributes][];
                ClassesInInterval = new int[DataToTransform.InputAttributes][];

                for (int i = 0; i < DataToTransform.Attributes; i++)
                {
                   if (DataToTransform.LingvisticAttribute[i] != 0)
                    {
                        Intervals[i] = DataToTransform.LingvisticAttribute[i];
                    }
                    else
                    {
                        Intervals[i] = 2;
                    }

                 }
                

                Results = new double[DataToTransform.Attributes][][];

                for (int i = 0; i < DataToTransform.Attributes; i++)
               {
                   Results[i] = new double[Intervals[i]][];

                   for (int j = 0; j < Intervals[i]; j++)
                   {
                       Results[i][j] = new double[DataToTransform.DatasetSize];
                       for (int k = 0; k < DataToTransform.DatasetSize; k++)
                       {
                           Results[i][j][k] = 0;
                       }
                   }
                  }
           }
       }

   
       public virtual void RunFuzzification()
       {
           Initialize();

            for (int i = 0; i < DataToTransform.Attributes; i++)
            {
                //ak je to lingvisticky atribut nastav intervaly na 1 
                if (DataToTransform.LingvisticAttribute[i] != 0)
                {
                    Intervals[i] = DataToTransform.LingvisticAttribute[i];
                    for (int j = 0; j < DataToTransform.DatasetSize; j++)
                    {
                        Results[i]
                            [Convert.ToInt32(DataToTransform.Dataset[j][i])]
                            [j] = 1.0;
                    }
                }
            }

            for (int i = 0; i < DataToTransform.Attributes; i++)
            {
                if (DataToTransform.LingvisticAttribute[i] == 0)
                {
                    RunFuzzificationInDimension(i);
                }
            }
        }

       public virtual void RunFuzzificationInDimension(int dimension)
       {
            //  Step 1) Set the initial number of intervals I = 2.
            int interval = 2;
            TotalEntropy[dimension] = new double[100];
            TotalEntropy[dimension][1] = 99999999999; 
           int does_entropy_decrease = 0;
           bool condition;
            do
            {   
                //Partition of interval to I = I + 1
                ResizeResultToNewInterval(dimension, interval);  
                
                // Step 2) Locate the centers of intervals.
                Centers[dimension]= DeterminationIntervalsLocation(dimension, Intervals[dimension]);
                
                // Step 3) Assign membership function for each interval.
                MembershipFunctionAssignment(dimension, interval);

                // Step 4) Compute the total fuzzy entropy of all intervals for I and I - 1 intervals.
                Console.WriteLine("****************************************************************************************");
                TotalEntropy[dimension][interval] = ComputeTotalFuzzyEntropy(dimension);
                Console.WriteLine("Total fuzzy entropy("+dimension+","+interval+"): \t" + TotalEntropy[dimension][interval]);
               
                // Step 5) Does the total fuzzy entropy decrease?
                // If the total fuzzy entropy of I intervals is less than that of I - 1 intervals, 
                // then partition again(I := I + 1) and go to Step 2; else go to Step 6.
                 condition = ConditionForStopingFuzzificationInDimension(dimension,
                    TotalEntropy[dimension][interval], TotalEntropy[dimension][interval - 1]);
                interval++;

            } while (!condition);

            //Step 6) I - 1 is the number of intervals on specified dimension.
           
            interval=interval-2;
            ResizeResultToNewInterval(dimension, interval);
            Centers[dimension] = DeterminationIntervalsLocation(dimension, interval);
            MembershipFunctionAssignment(dimension, interval);
           // Console.WriteLine(ComputeTotalFuzzyEntropy(dimension));
           //Console.WriteLine(Intervals[dimension]);
           Console.WriteLine("DONE");

       }
        /// <summary>
        /// Does the total fuzzy entropy decrease?
        /// If the total fuzzy entropy of I intervals is less than that
        /// of I - 1 intervals. return true else false;
        /// </summary>
        /// <param name="dimension"></param>
        /// <param name="totalEntropyI"></param>
        /// <param name="totalEntropyIPrevious"></param>
        /// <returns></returns>
        protected abstract double ComputeTotalFuzzyEntropy(int dimension);

       protected virtual bool ConditionForStopingFuzzificationInDimension(int dimension, double totalEntropyI,
           double totalEntropyIPrevious)
       {
           return ((totalEntropyI > totalEntropyIPrevious ) || totalEntropyI==0); 
       }

        //public virtual void DeterminationNumberOfIntervals(int dimension, int interval)
        //{
        //     ResizeResultToNewInterval(dimension, interval);
        //}

        /// <summary>
        /// [] center of konkretnehoo data bodu
        // [][0] - closest center
        // [][1] - closest distance
        // [][2] - closest index
        // [][3] - data
        // [][4] - left center 
        // [][5] - right center
        /// </summary>
       public abstract double[] DeterminationIntervalsLocation(int dimension, int intervals);

       public abstract void MembershipFunctionAssignment(int dimension, int interval);
       

       public virtual void ClassLabelAssigment(int dimension)
       {
           
       }

       public void ResizeResultToNewInterval(int dimension, int interval)
       {
            Intervals[dimension] = interval;

            Array.Clear(Results[dimension],0, Results[dimension].Length);
            Results[dimension] = new double[Intervals[dimension]][];
           for (int i = 0; i < Intervals[dimension]; i++)
           {
                Results[dimension][i] = new double[DataToTransform.DatasetSize];
               for (int j = 0; j < DataToTransform.DatasetSize; j++)
               {
                   Results[dimension][i][j] = 0.0; 
               }
           }
       }

        public void WriteToFile(string filename = "results.txt")
        {
            using (StreamWriter w = new StreamWriter(filename))
            {

                if (TotalEntropy != null)
                {
                    w.WriteLine("Total Entropy of dimension on intervals");
                    for (int i = 0; i < TotalEntropy.Length; i++)
                    {
                        for (int j = 0; j < TotalEntropy[i].Length; j++)
                        {
                            if (Math.Abs(TotalEntropy[i][j]) > 0.0000001) w.WriteLine("Dimension:  \t" + i + ", Interval: \t" + j + ", Total Entropy = \t" + TotalEntropy[i][j]);
                        }
                        w.WriteLine();
                    }
                    w.WriteLine();
                    w.WriteLine();
                }


                if (Intervals != null)
                {
                    for (int i = 0; i < Intervals?.Length; i++)
                    {
                        w.Write(Intervals[i] + "\t");
                        Console.WriteLine(Intervals[i]);
                    }
                    w.WriteLine();
                }

               
                if (Centers != null)
                {
                    w.WriteLine("Centers");
                    for (int i = 0; i < Centers.Length; i++)
                    {
                        for (int j = 0; j < Centers[i].Length; j++)
                        {
                            w.Write((Math.Round(Centers[i][j], 4)).ToString("0.0000") + "\t");
                        }
                        w.Write("\t\t");
                    }

                    w.WriteLine();
                }

                if (ClassesInInterval != null)
                {
                    for (int i = 0; i < ClassesInInterval.Length; i++)
                    {
                        if (ClassesInInterval[i]!=null)
                        {
                            w.WriteLine("Dimension " + i);
                            for (int j = 0; j < ClassesInInterval[i].Length; j++)
                            {
                                w.WriteLine(i + " = "+ ClassesInInterval[i][j]);
                            }
                            w.WriteLine();
                        }
                    }
                }

                w.WriteLine("*****************************************RESULTS***************************************************************");

                if ( Results != null)
                {
                     double sum = 0; 
                    for (int i = 0; i < DataToTransform?.DatasetSize; i++)
                    {
                        for (int j = 0; j < DataToTransform.Attributes; j++)
                        {
                            for (int k = 0; k < Intervals[j]; k++)
                            {
                                sum += Results[j][k][i];
                                w.Write((Math.Round(Results[j][k][i], 4)).ToString("0.0000") + "\t");                          
                            }
                            w.Write("\t");
                            w.Write("sum("+ Math.Round(sum,4).ToString("0.0000") + ")"+ "\t\t");
                            sum = 0; 
                        }
                        w.WriteLine();
                    }
                    w.WriteLine();
                }
            }
        }









    }
}

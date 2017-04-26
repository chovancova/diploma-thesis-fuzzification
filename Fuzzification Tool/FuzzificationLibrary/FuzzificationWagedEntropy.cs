﻿using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Management.Instrumentation;
using System.Text;
using System.Threading.Tasks;
using Datasets;

namespace FuzzificationLibrary
{
    public class FuzzificationWagedEntropy : FuzzificationEntropy
    {
        public FuzzificationWagedEntropy(DataSets dataToTransform) : base(dataToTransform)
        {
        }
       
        protected override double ComputeTotalFuzzyEntropy(int dimension)
        {
            //1) Let X = {r1, ... , rn} be a universal set with elements ri distributed in pattern space where i = 1..n. 
            //2) Let A be a fuzzy set defined on a interval of pattern space which kontains k elements (k < n). 

            double totalEntropy = 0;
            int countIntervalsInDimension = Intervals[dimension];
            int[] countM = new int[countIntervalsInDimension];
            for (int i = 0; i < countIntervalsInDimension; i++)
            {
                countM[i] = 0;
            }
            //---------------------------II.C.----STEP 1 - SET UNIVERSAL SET X -------------------------------------------
            double[][][] mu = new double[countIntervalsInDimension][][];
            double[][] sumMu = new double[countIntervalsInDimension][];
            for (int i = 0; i < countIntervalsInDimension; i++)
            {
                mu[i] = new double[countIntervalsInDimension][];
                sumMu[i] = new double[countIntervalsInDimension];
                for (int j = 0; j < countIntervalsInDimension; j++)
                {
                    mu[i][j] = new double[DataToTransform.OutputIntervals];
                    sumMu[i][j] = 0;
                    for (int k = 0; k < DataToTransform.OutputIntervals; k++)
                    {
                        mu[i][j][k] = 0;
                    }
                }
            }
            //---------------------------II.C.----STEP 2 - SET FUZZY SET A CONTAINS K ELEMENTS ---------------------------
            //---------------------------II.C.----STEP 3 - SET ARRAY REPRESENTING M CLASSES INTO WHICH THE N ELEMENTS ARE DEVIDED -------
            double max = 0;
            int classM = 0;
            double temp = 0;
            for (int i = 0; i < DataToTransform.DatasetSize; i++)
            {
                max = Results[dimension][0][i];
                //---------------------------II.C.----STEP 4 - SET SCJ AS SET OF ELEMENTS OF CLASS J ON X -------------------------------------------
                // Console.WriteLine("II.C.----STEP 4 - SET SCJ AS SET OF ELEMENTS OF CLASS J ON X ");
                for (int j = 0; j < countIntervalsInDimension; j++)
                {
                    temp = Results[dimension][j][i];
                    if (max <= temp)
                    {
                       classM = j;
                       max = temp;
                    }
                
                }
                countM[classM]++;
                //---------------------------II.C.----STEP 5 - COMPUTE MATCH DEGREE DJ -------------------------------------------
                for (int j = 0; j < countIntervalsInDimension; j++)
                    for (int k = 0; k < DataToTransform.OutputIntervals; k++) //OutputIntervals???
                    {
                        mu[classM][j][k] += Results[dimension][j][i] * Results[DataToTransform.InputAttributes][k][i]; //toto priradi to kde patri do akej triedy
                    }
            }
            Console.WriteLine("number of class in intervals ");
            double sum = 0;
            for (int i = 0; i < countM.Length; i++)
            {
                Console.WriteLine(i + "\t (" + countM[i] + ")");
                sum += countM[i];
            }
            Console.WriteLine("Sum: " + sum);
            Console.WriteLine();

            for (int j = 0; j < countIntervalsInDimension; j++)
                for (int k = 0; k < countIntervalsInDimension; k++)
                    for (int l = 0; l < DataToTransform.OutputIntervals; l++)
                    {
                        sumMu[j][k] += mu[j][k][l];
                    }


            //---------------------------II.C.----STEP 6 - COMPUTE FUZZY ENTROPY FECJ A  -------------------------------------------
            //---------------------------II.C.----STEP 7 - COMPUTE FUZZY ENTORPY FEA ON X -------------------------------------------
            //Console.WriteLine("II.C.----STEP 6 - COMPUTE FUZZY ENTROPY FECJ A ");
            //  Fuzzy Entropy Calculation
            // The fuzzy entropy FE on the universal uset X for the elements within an interval 
            double matchDegreeDj = 0;
            for (int i = 0; i < countIntervalsInDimension; i++)
            {
                double newEntropy = 0;
                for (int j = 0; j < countIntervalsInDimension; j++)
                {
                    for (int k = 0; k < DataToTransform.OutputIntervals; k++)
                    {
                        //---------------------------II.C.----STEP 5 - COMPUTE MATCH DEGREE DJ -------------------------------------------
                        matchDegreeDj = (sumMu[i][j] < 0.000001) ? 0 : (mu[i][j][k] / sumMu[i][j]); //proti deleniu nulov
                        //---------------------------II.C.----STEP 6 - COMPUTE FUZZY ENTROPY FECJ A  -------------------------------------------
                                                                                                  
                        if (Math.Abs(matchDegreeDj) > 0.0000001)
                        {
                            if ((Math.Abs(mu[i][j][k]) > 0.00001))
                                newEntropy -= matchDegreeDj * Math.Log(matchDegreeDj, 2);
                        }
                    }
                }
                //---------------------------II.C.----STEP 7 - COMPUTE FUZZY ENTORPY FEA ON X -------------------------------------------
               totalEntropy += newEntropy * countM[i] / DataToTransform.DatasetSize;
              }

            ClassesInInterval[dimension] = countM;
            return totalEntropy;
        }
        

        public override void MembershipFunctionAssignment(int dimension, int interval)
        {
            for (int i = 0; i < DataToTransform.DatasetSize; i++)
            {
                var x = DataToTransform.Dataset[i][dimension];

                for (int j = 0; j < interval - 1; j++)
                {
                    var c4 = Centers[dimension][j + 1];
                    var c3 = Centers[dimension][j];
                    if (x >= c3
                        && x <= c4)
                    {
                        Results[dimension][j][i] = (c4 - x) / (c4 - c3);
                        Results[dimension][j + 1][i] = (x - c3) / (c4 - c3);
                    }
                }

                var c1 = Centers[dimension][0];
                var c2 = Centers[dimension][interval - 1];
                if (x < c1)
                {
                    Results[dimension][0][i] = 1;
                }
                if (x > c2)
                {
                    Results[dimension][interval - 1][i] = 1;
                }
            }
        }
    }
}

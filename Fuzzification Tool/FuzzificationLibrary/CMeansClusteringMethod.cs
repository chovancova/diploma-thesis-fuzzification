﻿using System;

namespace FuzzificationLibrary
{
    public class CMeansClusteringMethod
    {
        private Fuzzification _fc;
        private Random _rand;

        public CMeansClusteringMethod(Fuzzification fc)
        {
            _rand = new Random();
            _fc = fc;
        }

        public double[] DeterminationIntervalsLocation(int dimension, int intervals)
        {
            return CMeansClustering(intervals, dimension);
        }
        /// <summary>
        /// [] center of konkretnehoo data bodu
        // [][0] - closest center
        // [][1] - closest distance
        // [][2] - closest index
        // [][3] - data
        /// </summary>
        private double[] CMeansClustering(int numberOfIntervals, int dimension)
        {
            int count = 0; 
            double[][] centersForDataset;
            bool doesAnyCenterChange;
            double [] result = new double[numberOfIntervals];
            //Step 2) Set initial centers of clusters.
            double[] centers = InitializeUniformCenters(dimension, numberOfIntervals);
            do
            {
                //Step 3) Assign cluster label to each element.
                centersForDataset = AssignClusterLabelToEachInterval(numberOfIntervals, dimension,
                    centers);
                //Step 4) Recompute the cluster centers.
                 result = RecomputeClusterCenters(numberOfIntervals, centersForDataset);
                //Step 5) Does any center change?
                //If each cluster center is determined appropriately, the
                //recomputed center in Step 4 would not change.
                //If so, stop the determination of interval centers, otherwise go to Step 3.
                doesAnyCenterChange = DoesAnyCenterChange(result, centers);

                if (!doesAnyCenterChange) break;
                count++;
                centers = result;
            } while (count<500);
            return result;
        }

        //If each cluster center is determined appropriately, the recomputed center in Step 4 would not change.
        //If so, stop the determination of interval centers, otherwise go to Step 3.
        private static bool DoesAnyCenterChange(double[] result, double[] centers )
        {
            //ak bola nejaka zmena v umiestneni - tak false, inak true
            for (var i = 0; i < result.Length; i++)
                if ((result[i] != centers[i]) )
                    return true;

            return false; 
        }

        private double[] RecomputeClusterCenters(int numberOfIntervals, double[][] centers)
        {
            var result = new double[numberOfIntervals];
            for (var i = 0; i < numberOfIntervals; i++)
            {
                double Nq = 0;
                double sumNq = 0;
                for (var j = 0; j < _fc.DataToTransform.DatasetSize; j++)
                    if (Math.Abs(centers[j][0] - i) < 0.000001)
                    {
                        Nq++;
                        sumNq += centers[j][1];
                    }
               result[i] = (double) sumNq/Nq;
            }
            return result;
        }

        private double[][] AssignClusterLabelToEachInterval(int numberOfIntervals, int dimension, double[] c)
        {
            double[][] centers;
            var closest = 999999999.0;
            var distance = 0.0;
            var closestIndex = 0;
            var data = 0.0;
            centers = new double[_fc.DataToTransform.DatasetSize][];

            for (var i = 0; i < _fc.DataToTransform.DatasetSize; i++)
            {
                data = _fc.DataToTransform.Dataset[i][dimension];
                centers[i] = new double[6];
                closest = 999999999.0;
                closestIndex = 0;

                for (var j = 0; j < numberOfIntervals; j++)
                {
                    distance = ComputeEuclidDistance(data, c[j]);
                    if (distance < closest)
                    {
                        closest = distance;
                        closestIndex = j;
                    }
                }
               
                // 0 - closest index
                // 1 - data
                centers[i][0] = closestIndex;
                centers[i][1] = data;
             }
            return centers;
        }

        private double[] InitializeUniformCenters(int dimension, int q)
        {
            var c = new double[q];
            var indexData = new int[q];
            var notSame = false;
            double temp;
            for (var i = 1; i <= q; i++)
               c[i-1] = (double)(i - 1) / (q - 1);
          
            return c;
        }

        private double ComputeEuclidDistance(double a, double b)
        {
            return Math.Sqrt((a - b)*(a - b));
        }
    }
}
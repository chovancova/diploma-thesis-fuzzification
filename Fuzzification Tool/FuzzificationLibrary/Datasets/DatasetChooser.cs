﻿using FuzzificationLibrary.Datasets;

namespace Datasets
{
    public enum DatasetType
    {
        Heart,
        Iris,
        Seeds,
       Wine,
        Yeast,
      Test
    }

    public static class DatasetChooser
    {
        public static DataSets ReadDatasetFromFile(DatasetType type, int datasetSize,
            int inputAttributes, int outputIntervals, string filename)
        {
            DataSets dataset = null;
            switch (type)
            {
                case DatasetType.Iris:
                    dataset = new DatasetIris(datasetSize, inputAttributes, outputIntervals, filename);
                    break;
                case DatasetType.Test:
                    dataset = new DatasetTest(datasetSize, inputAttributes, outputIntervals, filename);
                    break;
                case DatasetType.Heart:
                    dataset = new DatasetHeart(datasetSize, inputAttributes, outputIntervals, filename);
                    break;
                case DatasetType.Seeds:
                    dataset = new DatasetSeeds(datasetSize, inputAttributes, outputIntervals, filename);
                    break;
                case DatasetType.Yeast:
                    dataset = new DatasetYeast(datasetSize, inputAttributes, outputIntervals, filename);
                    break;
                case DatasetType.Wine:
                    dataset = new DatasetWine(datasetSize, inputAttributes, outputIntervals, filename);
                    break;
                default:
                    return null;
            }

            dataset.InitializeDataset();
            dataset.NormalizeDataset();
         //   dataset.WriteInfoToFile();
            return dataset;
        }

        public static DataSets ReadDatasetFromFile(DatasetType type, string filename)
        {
            DataSets dataset = null;
            switch (type)
            {
                case DatasetType.Iris:
                    dataset = new DatasetIris(filename);
                    break;
                case DatasetType.Test:
                    dataset = new DatasetTest(filename);
                    break;
                case DatasetType.Heart:
                    dataset = new DatasetHeart(filename);
                    break;
                case DatasetType.Seeds:
                    dataset = new DatasetSeeds(filename);
                    break;
                case DatasetType.Yeast:
                    dataset = new DatasetYeast(filename);
                    break;
                case DatasetType.Wine:
                    dataset = new DatasetWine(filename);
                    break;
                default:
                    return null;
            }

            dataset.InitializeDataset();
            dataset.NormalizeDataset();
           // dataset.WriteInfoToFile();
            return dataset;
        }
    }
}
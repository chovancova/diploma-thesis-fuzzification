﻿namespace Datasets
{
    public enum DatasetType
    {
        Heart,
        Iris,
        Seeds,
        Skin,
        Wine,
        Yeast,
        Bupa,
        Balance,
        Vowel,
        Pima,
        Venicle,
        Wpbc,
        Wdbc,
        Test
    }

    public static class DatasetChooser
    {
        public static DataSets ReadDatasetFromFile(DatasetType type, int datasetSize, int attributes,
            int inputAttributes, int outputAttributes, int outputIntervals, string filename)
        {
            DataSets dataset = null;
            switch (type)
            {
                case DatasetType.Iris:
                    dataset = new DatasetIris(datasetSize, attributes, inputAttributes, outputAttributes,
                        outputIntervals, filename);
                    break;
                case DatasetType.Test:
                    dataset = new DatasetTest(datasetSize, attributes, inputAttributes, outputAttributes,
                        outputIntervals, filename);
                    break;
                default:
                    return null;
            }

            dataset.InitializeDataset();
            dataset.NormalizeDataset();
            dataset.WriteInfoToFile();
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
                default:
                    return null;
            }

            dataset.InitializeDataset();
            dataset.NormalizeDataset();
            dataset.WriteInfoToFile();
            return dataset;
        }
    }
}
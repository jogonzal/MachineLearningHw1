﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using MachineLearningHw1.DataSet;
using MachineLearningHw1.DecisionTreeClasses;

namespace MachineLearningHw1
{
	public class Program
	{
		private static string DataSetPath => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\training_subsetD.arff");
		private static string TestSetPath => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\testingD.arff");

		static void Main(string[] args)
		{
			Console.WriteLine("Reading training data...");
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath);

			Console.WriteLine("Validating data set");
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			// Initialize the required trees with their respective chiTestLimits
			List<DecisionTreeLevel> listOfTreesToRunTestOn = new List<DecisionTreeLevel>()
			{
				new DecisionTreeLevel(chiTestLimit:0.99),
				new DecisionTreeLevel(chiTestLimit:0.95),
				new DecisionTreeLevel(chiTestLimit:0),
			};

			Console.WriteLine("Runnind D3...");
			Parallel.ForEach(listOfTreesToRunTestOn, l => l.D3(trainingData.Attributes, trainingData.Values));

			Console.WriteLine("Deleting unecessary nodes...");
			Parallel.ForEach(listOfTreesToRunTestOn, l => l.TrimTree());

			Console.WriteLine("Getting test data set...");
			ParserResults testData = ParserUtils.ParseData(TestSetPath);

			//Console.WriteLine("Writing trees to text files (for debugging/visualization)...");
			// Dump the trees to a txt file for debugging/visualization
			// NOTE: This won't work the the Chi=0 case - the JSON file generated is too big
			// Parallel.ForEach(listOfTreesToRunTestOn, l => File.WriteAllText("Chi" + Convert.ToInt64(l.ChiTestLimit * 10000000000000) + ".json", l.SerializeDecisionTree()));

			Console.WriteLine("Evaluating trees against test data...");
			List<DecisionTreeScore> scores = listOfTreesToRunTestOn.AsParallel().Select(t => DecisionTreeScorer.ScoreWithTreeWithTestSet(t, testData.Values)).ToList();

			// Print the results to console
			foreach (var score in scores)
			{
				score.PrintTotalScore();
			}

			Console.WriteLine("Press any key to quit...");
			Console.ReadKey();
		}
	}
}

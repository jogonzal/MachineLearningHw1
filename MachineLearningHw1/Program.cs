using System;
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
			// Get and parse the training dataset
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath);

			Console.WriteLine("Validating data set");
			// Validate and clean the dataset
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			// Initialize the required trees
			List<DecisionTreeLevel> listOfTreesToRunTestOn = new List<DecisionTreeLevel>()
			{
				new DecisionTreeLevel(0.99),
				new DecisionTreeLevel(0.95),
				new DecisionTreeLevel(0.50),
				new DecisionTreeLevel(0),
			};

			Console.WriteLine("Runnind D3...");
			// Run D3 on all trees
			Parallel.ForEach(listOfTreesToRunTestOn, l => l.D3(trainingData.Attributes, trainingData.Values));

			Console.WriteLine("Getting test data set...");
			// Get and parse the test dataset
			ParserResults testData = ParserUtils.ParseData(TestSetPath);

			//Console.WriteLine("Writing trees to text files (for debugging/visualization)...");
			// Dump the trees to a txt file for debugging/visualization
			//Parallel.ForEach(listOfTreesToRunTestOn, l => File.WriteAllText("Chi" + Convert.ToInt32(l.ChiTestLimit * 1000) + ".json", l.SerializeDecisionTree()));

			Console.WriteLine("Evaluating trees against test data...");
			// Evaluate the trees with the test dataset
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

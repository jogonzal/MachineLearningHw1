using System;
using System.Collections.Generic;
using System.IO;
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
			// Get and parse the training dataset
			ParserResults trainingData = ParserUtils.ParseData(DataSetPath);

			// Validate and clean the dataset
			DataSetCleaner.ValidateDataSet(trainingData.Attributes, trainingData.Values);

			// Initialize the required trees
			List<DecisionTreeLevel> listOfTreesToRunTestOn = new List<DecisionTreeLevel>()
			{
				new DecisionTreeLevel(trainingData.Attributes, trainingData.Values, 0.99),
				new DecisionTreeLevel(trainingData.Attributes, trainingData.Values, 0.95),
				new DecisionTreeLevel(trainingData.Attributes, trainingData.Values, 0.50),
				new DecisionTreeLevel(trainingData.Attributes, trainingData.Values, 0),
			};

			// Run D3 on all trees
			Parallel.ForEach(listOfTreesToRunTestOn, l => l.D3());

			// Get and parse the test dataset
			ParserResults testData = ParserUtils.ParseData(TestSetPath);

			// Evaluate the trees with the test dataset
			foreach (var decisionTree in listOfTreesToRunTestOn)
			{
				DecisionTreeScore score = DecisionTreeScorer.ScoreWithTreeWithTestSet(decisionTree, testData.Values);
				score.PrintTotalScore();
			}
		}
	}
}

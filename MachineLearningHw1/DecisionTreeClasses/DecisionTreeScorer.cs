using System;
using System.Collections.Generic;

using MachineLearningHw1.DataSet;

namespace MachineLearningHw1.DecisionTreeClasses
{
	public class DecisionTreeScore
	{
		private readonly DecisionTreeLevel _decisionTree;
		public double PositiveHit { get; set; }
		public double FalsePositive { get; set; }
		public double NegativeHits { get; set; }
		public double FalseNegative { get; set; }
		public int NodeCount { get; set; }

		public DecisionTreeScore(double positiveHit, double falsePositive, double negativeHits, double falseNegative, DecisionTreeLevel decisionTree)
		{
			_decisionTree = decisionTree;
			PositiveHit = positiveHit;
			FalsePositive = falsePositive;
			NegativeHits = negativeHits;
			FalseNegative = falseNegative;
		}

		public double GetTotalScore()
		{
			return (PositiveHit + NegativeHits)/(PositiveHit + NegativeHits + FalsePositive + FalseNegative);
		}

		public void PrintTotalScore()
		{
			Console.WriteLine($"Score for tree with CHI({_decisionTree.ChiTestLimit}) = {GetTotalScore()}. Total nodes: {NodeCount}");
		}
	}

	public static class DecisionTreeScorer
	{
		public static DecisionTreeScore ScoreWithTreeWithTestSet(DecisionTreeLevel decisionTree, List<DataSetValue> testDataSetValues)
		{
			DecisionTreeScore score = new DecisionTreeScore(0, 0, 0, 0, decisionTree);
			foreach (var testDataSetValue in testDataSetValues)
			{
				bool output = decisionTree.Evaluate(testDataSetValue.Values);
				if (output && testDataSetValue.Output)
				{
					score.PositiveHit++;
				}
				else if (!output && !testDataSetValue.Output)
				{
					score.NegativeHits++;
				}
				else if (output && !testDataSetValue.Output)
				{
					score.FalsePositive++;
				}
				else if (!output && testDataSetValue.Output)
				{
					score.FalseNegative++;
				}
			}

			score.NodeCount = decisionTree.GetNodeCount();

			return score;
		}
	}
}

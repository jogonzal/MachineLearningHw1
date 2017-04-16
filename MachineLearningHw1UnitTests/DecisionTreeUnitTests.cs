using System.Collections.Generic;

using FluentAssertions;

using MachineLearningHw1.DataSet;
using MachineLearningHw1.DecisionTreeClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MachineLearningHw1UnitTests
{
	[TestClass]
	public class DecisionTreeUnitTests
	{
		private List<DataSetAttribute> _generic3BooleanAttributes = new List<DataSetAttribute>()
			{
				new DataSetAttribute("x0", new HashSet<string>() {"0", "1"}, 0),
				new DataSetAttribute("x1", new HashSet<string>() {"0", "1"}, 1),
				new DataSetAttribute("x2", new HashSet<string>() {"0", "1"}, 2),
			};

		[TestMethod]
		public void BooleanAndFunction_SingleVariable_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, false),
				new DataSetValue(new List<string>() {"0", "0", "1"}, false),
				new DataSetValue(new List<string>() {"0", "1", "0"}, false),
				new DataSetValue(new List<string>() {"0", "1", "1"}, false),
				new DataSetValue(new List<string>() {"1", "0", "0"}, true),
				new DataSetValue(new List<string>() {"1", "0", "1"}, true),
				new DataSetValue(new List<string>() {"1", "1", "0"}, true),
				new DataSetValue(new List<string>() {"1", "1", "1"}, true),
			});
		}

		[TestMethod]
		public void BooleanAndFunction_TwoVariables_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, false),
				new DataSetValue(new List<string>() {"0", "0", "1"}, false),
				new DataSetValue(new List<string>() {"0", "1", "0"}, false),
				new DataSetValue(new List<string>() {"0", "1", "1"}, true),
				new DataSetValue(new List<string>() {"1", "0", "0"}, false),
				new DataSetValue(new List<string>() {"1", "0", "1"}, false),
				new DataSetValue(new List<string>() {"1", "1", "0"}, false),
				new DataSetValue(new List<string>() {"1", "1", "1"}, true),
			});
		}

		[TestMethod]
		public void BooleanAndFunction_ThreeVariables_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, false),
				new DataSetValue(new List<string>() {"0", "0", "1"}, false),
				new DataSetValue(new List<string>() {"0", "1", "0"}, false),
				new DataSetValue(new List<string>() {"0", "1", "1"}, true),
				new DataSetValue(new List<string>() {"1", "0", "0"}, false),
				new DataSetValue(new List<string>() {"1", "0", "1"}, false),
				new DataSetValue(new List<string>() {"1", "1", "0"}, false),
				new DataSetValue(new List<string>() {"1", "1", "1"}, false),
			});
		}

		[TestMethod]
		public void BooleanAndFunction_AllTrue_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, true),
				new DataSetValue(new List<string>() {"0", "0", "1"}, true),
				new DataSetValue(new List<string>() {"0", "1", "0"}, true),
				new DataSetValue(new List<string>() {"0", "1", "1"}, true),
				new DataSetValue(new List<string>() {"1", "0", "0"}, true),
				new DataSetValue(new List<string>() {"1", "0", "1"}, true),
				new DataSetValue(new List<string>() {"1", "1", "0"}, true),
				new DataSetValue(new List<string>() {"1", "1", "1"}, true),
			});
		}

		[TestMethod]
		public void BooleanAndFunction_AllFalse_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, false),
				new DataSetValue(new List<string>() {"0", "0", "1"}, false),
				new DataSetValue(new List<string>() {"0", "1", "0"}, false),
				new DataSetValue(new List<string>() {"0", "1", "1"}, false),
				new DataSetValue(new List<string>() {"1", "0", "0"}, false),
				new DataSetValue(new List<string>() {"1", "0", "1"}, false),
				new DataSetValue(new List<string>() {"1", "1", "0"}, false),
				new DataSetValue(new List<string>() {"1", "1", "1"}, false),
			});
		}


		[TestMethod]
		public void BooleanAndFunction_XORBetweenTwoVariables_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, false),
				new DataSetValue(new List<string>() {"0", "0", "1"}, true),
				new DataSetValue(new List<string>() {"0", "1", "0"}, true),
				new DataSetValue(new List<string>() {"0", "1", "1"}, false),
				new DataSetValue(new List<string>() {"1", "0", "0"}, false),
				new DataSetValue(new List<string>() {"1", "0", "1"}, true),
				new DataSetValue(new List<string>() {"1", "1", "0"}, true),
				new DataSetValue(new List<string>() {"1", "1", "1"}, false),
			});
		}

		[TestMethod]
		public void BooleanAndFunction_XORBetweenThreeVariables_DecisionTreeLearnsIt()
		{
			VerifyDecisionTreeCanLearnTable(new List<DataSetValue>()
			{
				// x0
				new DataSetValue(new List<string>() {"0", "0", "0"}, false),
				new DataSetValue(new List<string>() {"0", "0", "1"}, true),
				new DataSetValue(new List<string>() {"0", "1", "0"}, true),
				new DataSetValue(new List<string>() {"0", "1", "1"}, false),
				new DataSetValue(new List<string>() {"1", "0", "0"}, true),
				new DataSetValue(new List<string>() {"1", "0", "1"}, false),
				new DataSetValue(new List<string>() {"1", "1", "0"}, false),
				new DataSetValue(new List<string>() {"1", "1", "1"}, true),
			});
		}

		private void VerifyDecisionTreeCanLearnTable(List<DataSetValue> tableToLearn)
		{
			DecisionTreeLevel decisionTree = new DecisionTreeLevel(_generic3BooleanAttributes,
			tableToLearn,
			0);

			decisionTree.D3();

			// The tree should have learnt every value
			foreach (var dataSetValue in tableToLearn)
			{
				decisionTree.Evaluate(dataSetValue.Values).Should().Be(dataSetValue.Output);
			}
		}
	}
}

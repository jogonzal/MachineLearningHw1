using System.Collections.Generic;
using System.IO;

using MachineLearningHw1.DataSet;
using MachineLearningHw1.DecisionTreeClasses;

namespace MachineLearningHw1
{
	public class Program
	{
		private static string DataSetPath => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\training_subsetD.arff");

		static void Main(string[] args)
		{
			string dataSetAsString = File.ReadAllText(DataSetPath);

			// Parse the dataset
			List<DataSetAttribute> attributes = AttributeParser.ParseAttributes(dataSetAsString);
			List<DataSetValue> dataSetValues = DataParser.ParseData(dataSetAsString);

			// Validate the dataset
			DataSetValidator.ValidateDataSet(attributes, dataSetValues);

			// Initialize the tree
			DecisionTreeLevel treeLevel = new DecisionTreeLevel(attributes, dataSetValues);

			// Run D3 on the tree
			treeLevel.D3();
		}
	}
}

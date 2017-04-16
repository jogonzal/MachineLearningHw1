using System;
using System.Collections.Generic;
using System.IO;

using MachineLearningHw1.DataSet;

namespace MachineLearningHw1
{
	public class Program
	{
		private static string DataSetPath => Path.Combine(Directory.GetCurrentDirectory() + @"\..\..\..\training_subsetD.arff");

		static void Main(string[] args)
		{
			string dataSetAsString = File.ReadAllText(DataSetPath);

			List<DataSetAttribute> attributes = AttributeParser.ParseAttributes(dataSetAsString);
			List<List<string>> dataSetValues = DataParser.ParseData(dataSetAsString);

			DataSetValidator.ValidateDataSet(attributes, dataSetValues);
		}
	}
}

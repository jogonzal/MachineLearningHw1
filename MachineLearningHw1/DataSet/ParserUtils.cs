using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningHw1.DataSet
{
	public class ParserResults
	{
		public List<DataSetAttribute> Attributes { get; set; }
		public List<DataSetValue> Values { get; set; }

		public ParserResults(List<DataSetAttribute> attributes, List<DataSetValue> values)
		{
			Attributes = attributes;
			Values = values;
		}
	}

	public static class ParserUtils
	{
		public static ParserResults ParseData(string dataSetPath)
		{
			string testDataSetAsString = File.ReadAllText(dataSetPath);

			List<DataSetAttribute> attributes = AttributeParser.ParseAttributes(testDataSetAsString);
			List<DataSetValue> trainingDataSetValues = DataParser.ParseData(testDataSetAsString);

			return new ParserResults(attributes, trainingDataSetValues);
		}
	}
}

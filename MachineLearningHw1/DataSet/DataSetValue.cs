using System.Collections.Generic;

namespace MachineLearningHw1.DataSet
{
	public class DataSetValue
	{
		public List<string> Values { get; set; }
		public bool Output { get; set; }

		public DataSetValue(List<string> values, bool output)
		{
			Values = values;
			Output = output;
		}
	}
}

using System;
using System.Collections.Generic;

namespace MachineLearningHw1.DataSet
{
	public class DataSetAttribute
	{
		public string Name { get; set; }
		public HashSet<string> PossibleValues { get; set; }

		public DataSetAttribute(string name, HashSet<string> possibleValues)
		{
			Name = name;
			PossibleValues = possibleValues;
		}

		public override string ToString()
		{
			return string.Format("{0}, {1} values", Name, PossibleValues.Count);
		}
	}
}

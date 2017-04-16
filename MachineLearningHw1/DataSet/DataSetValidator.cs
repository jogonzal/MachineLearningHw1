using System;
using System.Collections.Generic;

namespace MachineLearningHw1.DataSet
{
	public static class DataSetValidator
	{
		public static void ValidateDataSet(List<DataSetAttribute> attributes, List<List<string>> dataSetValues)
		{
			foreach (var dataSetValue in dataSetValues)
			{
				for (int i = 0; i < attributes.Count; i++)
				{
					string val = dataSetValue[i];
					bool isValueAllowed = attributes[i].PossibleValues.Contains(val);

					if (val == "?")
					{
						// Allowing questionmarks
						continue;
					}

					if (!isValueAllowed)
					{
						throw new InvalidOperationException($"Failed to find value {val} of type {attributes[i].Name}. Allowed list is {string.Join(",", attributes[i].PossibleValues)}");
					}
				}
			}
		}
	}
}

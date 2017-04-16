using System.Collections.Generic;
using System.IO;

using CsvHelper;

namespace MachineLearningHw1.DataSet
{
	public static class DataParser
	{
		private const int expectedAttributeCount = 275;

		public static List<DataSetValue> ParseData(string dataSetAsString)
		{
			int indexOfData = dataSetAsString.IndexOf("@data");
			string dataString = dataSetAsString.Substring(indexOfData + 5);

			var result = ReadInCSV(dataString);

			return result;
		}

		public static List<DataSetValue> ReadInCSV(string stringToRead)
		{
			List<DataSetValue> result = new List<DataSetValue>();
			string value;
			using (StringReader sr = new StringReader(stringToRead))
			{
				using (var csv = new CsvReader(sr))
				{
					csv.Configuration.HasHeaderRecord = false;
					while (csv.Read())
					{
						List<string> line = new List<string>();
						for (int i = 0; csv.TryGetField<string>(i, out value); i++)
						{
							line.Add(value);
						}

						// Get the last attribute, which should be boolean
						bool lastValue = bool.Parse(line[line.Count - 1]);
						line.RemoveAt(line.Count - 1);

						result.Add(new DataSetValue(line, lastValue));
					}
				}
			}
			return result;
		}
	}
}

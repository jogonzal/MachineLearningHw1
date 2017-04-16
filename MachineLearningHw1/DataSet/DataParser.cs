using System.Collections.Generic;
using System.IO;

using CsvHelper;

namespace MachineLearningHw1.DataSet
{
	public static class DataParser
	{
		public static List<List<string>> ParseData(string dataSetAsString)
		{
			int indexOfData = dataSetAsString.IndexOf("@data");
			string dataString = dataSetAsString.Substring(indexOfData + 5);

			var result = ReadInCSV(dataString);

			return result;
		}

		public static List<List<string>> ReadInCSV(string stringToRead)
		{
			List<List<string>> result = new List<List<string>>();
			string value;
			using (StringReader sr = new StringReader(stringToRead))
			{
				var csv = new CsvReader(sr);
				csv.Configuration.HasHeaderRecord = false;
				while (csv.Read())
				{
					List<string> line = new List<string>();
					for (int i = 0; csv.TryGetField<string>(i, out value); i++)
					{
						line.Add(value);
					}
					result.Add(line);
				}
			}
			return result;
		}
	}
}

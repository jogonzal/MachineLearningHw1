﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MachineLearningHw1.DataSet
{
	public static class AttributeParser
	{
		public static List<DataSetAttribute> ParseAttributes(string dataSetAsString)
		{
			int indexOfData = dataSetAsString.IndexOf("@data");
			int indexOfAttributes = dataSetAsString.IndexOf("@attribute");

			string attributesString = dataSetAsString.Substring(indexOfAttributes, indexOfData - indexOfAttributes);

			var listOfAttributesAsString = attributesString.Split('\n');
			List<DataSetAttribute> attributes = new List<DataSetAttribute>(listOfAttributesAsString.Length);
			for (int index = 0; index < listOfAttributesAsString.Length; index++)
			{
				var attributeAsString = listOfAttributesAsString[index];
				if (string.IsNullOrEmpty(attributeAsString))
				{
					continue;
				}

				int indexOfFirstSpace = attributeAsString.IndexOf(" ");
				int indexOfStartOfEnums = attributeAsString.IndexOf(" {");
				int indexOfEndOfEnums = attributeAsString.IndexOf("}");

				string name = attributeAsString.Substring(indexOfFirstSpace + 1, indexOfStartOfEnums - (indexOfFirstSpace + 1));
				string enums = attributeAsString.Substring(indexOfStartOfEnums + 2, (indexOfEndOfEnums) - (indexOfStartOfEnums + 2));

				var enumValues = enums.Split(',');
				var hashSet = new HashSet<string>();
				foreach (var enumValue in enumValues)
				{
					if (!hashSet.Add(enumValue))
					{
						throw new InvalidOperationException();
					}
				}

				var attribute = new DataSetAttribute(name, hashSet, index);
				attributes.Add(attribute);
			}

			// Remove the last attribute, as it is the result
			attributes.RemoveAt(attributes.Count - 1);

			return attributes;
		}
	}
}

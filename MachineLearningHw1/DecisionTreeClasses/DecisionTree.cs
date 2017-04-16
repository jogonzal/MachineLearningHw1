using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using MachineLearningHw1.DataSet;

namespace MachineLearningHw1.DecisionTreeClasses
{
	public class DecisionTree
	{
		public List<DataSetAttribute> Attributes { get; set; }
		public List<DataSetValue> Values { get; set; }

		public DecisionTree(List<DataSetAttribute> attributes, List<DataSetValue> values)
		{
			Attributes = attributes;
			Values = values;
		}

		public void D3()
		{
			// First, find the attribute with the highest "E"
			List<DataSetAttributeWithCounts> e = CalculateEForAllAttributes();
			DataSetAttribute attributeWithMinEntropy = FindAttributeWithMinEntropy(e);

			throw new NotImplementedException();
		}

		private DataSetAttribute FindAttributeWithMinEntropy(List<DataSetAttributeWithCounts> dataSetAttributeWithCountses)
		{
			double minEntropy = dataSetAttributeWithCountses.Select(s => s.Entropy).Min();
			var attributeWithMinEntropy = dataSetAttributeWithCountses.First(a => a.Entropy == minEntropy);
			return Attributes.Single(a => a.Name == attributeWithMinEntropy.Name);
		}

		private List<DataSetAttributeWithCounts> CalculateEForAllAttributes()
		{
			// First, compute the count of appearences of possible vlaues and their counts
			List<DataSetAttributeWithCounts> attributeWithCounts =
				Attributes.Select(s => new DataSetAttributeWithCounts(s.Name, s.PossibleValues)).ToList();
			foreach (var value in Values)
			{
				for (int i = 0; i < attributeWithCounts.Count; i++)
				{
					var attribute = attributeWithCounts[i];
					var attributeValue = value.Values[i];
					attribute.UpdateWith(attributeValue, value.Output);
				}
			}

			// Now, compute "E"
			foreach (var dataSetAttributeWithCountse in attributeWithCounts)
			{
				dataSetAttributeWithCountse.CalculateEntropy();
			}
			return attributeWithCounts;
		}
	}
}

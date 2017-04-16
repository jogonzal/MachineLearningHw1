using System.Collections.Generic;
using System.Linq;

using MachineLearningHw1.DataSet;
using MachineLearningHw1.Statistics;
using Newtonsoft.Json;

namespace MachineLearningHw1.DecisionTreeClasses
{
	public class DecisionTreeLevel
	{
		private readonly double _chiTestLimit;

		/// <summary>
		/// Keep track on the attribute we split on
		/// </summary>
		private DataSetAttribute _attributeToSplitOn;

		/// <summary>
		/// The dictionary of subtrees
		/// </summary>
		private Dictionary<string, DecisionTreeLevel> _dictionaryOfSubTrees;

		private bool? _localValue;

		public List<DataSetAttribute> Attributes { get; set; }
		public List<DataSetValue> Values { get; set; }

		public DecisionTreeLevel(List<DataSetAttribute> attributes, List<DataSetValue> values, double chiTestLimit)
		{
			_chiTestLimit = chiTestLimit;
			Attributes = attributes;
			Values = values;
		}

		public void D3()
		{
			// Check whether we even need to split or not
			int totalTrueValues = Values.Count(v => v.Output);
			int totalFalseValues = Values.Count(v => !v.Output);

			if (totalFalseValues == 0 && totalTrueValues > 0)
			{
				_localValue = true;
				return;
			}

			if (totalTrueValues == 0 && totalFalseValues > 0)
			{
				_localValue = false;
				return;
			}

			// We'll need to split based on attributes

			// First, find the attribute with the highest "E"
			List<DataSetAttributeWithCounts> e = CalculateEForAllAttributes();
			DataSetAttributeWithCounts attributeWithMinEntropy = FindAttributeWithMinEntropy(e);
			_attributeToSplitOn = attributeWithMinEntropy;

			// Decide whether it's worth it to split here
			if (Attributes.Count == 0)
			{
				// Can't split anymore. We'll decide on the most prevalent value
				_localValue = totalTrueValues > totalFalseValues;
				return;
			}

			if(!ShouldSplitOnAttributeAccordingToChiSquared(attributeWithMinEntropy))
			{
				// Not worth it to split. We'll decide on the most prevalent value
				_localValue = totalTrueValues > totalFalseValues;
				return;
			}

			// Remove this attribute from the list of new attributes to create new subtrees
			List<DataSetAttribute> newAttributes = Attributes.Where(a => a.Name != attributeWithMinEntropy.Name).ToList();

			// Split the values in many sets
			_dictionaryOfSubTrees = new Dictionary<string, DecisionTreeLevel>(attributeWithMinEntropy.PossibleValues.Count);
			foreach (var dataSetValue in Values)
			{
				string value = dataSetValue.Values[attributeWithMinEntropy.ValueIndex];
				DecisionTreeLevel localTreeLevel;
				if (!_dictionaryOfSubTrees.TryGetValue(value, out localTreeLevel))
				{
					localTreeLevel = new DecisionTreeLevel(newAttributes, new List<DataSetValue>(), _chiTestLimit);
					_dictionaryOfSubTrees[value] = localTreeLevel;
				}

				localTreeLevel.Values.Add(dataSetValue);
			}

			// Recursively run D3 on them
			foreach (var decisionTree in _dictionaryOfSubTrees.Values)
			{
				decisionTree.D3();
			}
		}

		private bool ShouldSplitOnAttributeAccordingToChiSquared(DataSetAttributeWithCounts attributeToSplitOn)
		{
			int positiveValuesGlobal = attributeToSplitOn.PossibleValueCounts.Values.Select(s => s.AppearWhenTrueCount).Sum();
			int negativeValuesGlobal = attributeToSplitOn.PossibleValueCounts.Values.Select(s => s.AppearWhenFalseCount).Sum();

			double chiTestValue = 0;
			foreach (var possibleValueCounts in attributeToSplitOn.PossibleValueCounts)
			{
				string valueKey = possibleValueCounts.Key;
				int positiveValuesLocal = possibleValueCounts.Value.AppearWhenTrueCount;
				int negativeValuesLocal = possibleValueCounts.Value.AppearWhenFalseCount;

				double weightFactor = 1.0 * (positiveValuesLocal + negativeValuesLocal) / (positiveValuesGlobal + negativeValuesGlobal);
				double pExpected = positiveValuesGlobal * weightFactor;
				double nExpected = negativeValuesGlobal * weightFactor;

				double pActual = positiveValuesLocal;
				double nActual = negativeValuesLocal;

				double diffP = (pExpected - pActual);
				double diffN = (nExpected - nActual);

				double localChiTestValue = diffP*diffP/pExpected + diffN*diffN/nExpected;

				chiTestValue += localChiTestValue;
			}

			double chiQuareCumulative = ChiSquaredUtils.CalculateChiSquareCDT(attributeToSplitOn.PossibleValueCounts.Count - 1, chiTestValue);

			return chiQuareCumulative >= _chiTestLimit;
		}

		private DataSetAttributeWithCounts FindAttributeWithMinEntropy(List<DataSetAttributeWithCounts> dataSetAttributeWithCountses)
		{
			double minEntropy = dataSetAttributeWithCountses.Select(s => s.Entropy).Min();
			var attributeWithMinEntropy = dataSetAttributeWithCountses.First(a => a.Entropy == minEntropy);
			return attributeWithMinEntropy;
		}

		private List<DataSetAttributeWithCounts> CalculateEForAllAttributes()
		{
			// First, compute the count of appearences of possible vlaues and their counts
			List<DataSetAttributeWithCounts> attributeWithCounts =
				Attributes.Select(s => new DataSetAttributeWithCounts(s.Name, s.PossibleValues, s.ValueIndex)).ToList();
			foreach (var value in Values)
			{
				foreach (var dataSetAttributeWithCounts in attributeWithCounts)
				{
					var attributeValue = value.Values[dataSetAttributeWithCounts.ValueIndex];
					dataSetAttributeWithCounts.UpdateWith(attributeValue, value.Output);
				}
			}

			// Now, compute "E"
			foreach (var dataSetAttributeWithCountse in attributeWithCounts)
			{
				dataSetAttributeWithCountse.CalculateEntropy();
			}
			return attributeWithCounts;
		}

		public bool Evaluate(List<string> list)
		{
			if (_localValue.HasValue)
			{
				return _localValue.Value;
			}

			string attributeValue = list[_attributeToSplitOn.ValueIndex];
			var nextTreeLevel = _dictionaryOfSubTrees[attributeValue];
			return nextTreeLevel.Evaluate(list);
		}

		public string SerializeDecisionTree()
		{
			var decisionTree = GetDecisionTree();
			return JsonConvert.SerializeObject(decisionTree, Formatting.Indented);
		}

		private object GetDecisionTree()
		{
			if (_localValue.HasValue)
			{
				return _localValue;
			}

			var dict = new Dictionary<string, Dictionary<string, object>>();
			var internalDict = new Dictionary<string, object>();
			dict[_attributeToSplitOn.Name] = internalDict;
			foreach (var keyValuePair in _dictionaryOfSubTrees)
			{
				internalDict[keyValuePair.Key] = keyValuePair.Value.GetDecisionTree();
			}
			return dict;
		}
	}
}

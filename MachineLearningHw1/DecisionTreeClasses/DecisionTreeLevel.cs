﻿using System;
using System.Collections.Generic;
using System.Linq;

using MachineLearningHw1.DataSet;
using MachineLearningHw1.Statistics;
using Newtonsoft.Json;

namespace MachineLearningHw1.DecisionTreeClasses
{
	public class DecisionTreeLevel
	{
		public double ChiTestLimit { get; }

		/// <summary>
		/// Keep track on the attribute we split on
		/// </summary>
		private DataSetAttributeWithCounts _attributeToSplitOn;

		/// <summary>
		/// The dictionary of subtrees
		/// </summary>
		private Dictionary<string, DecisionTreeLevel> _dictionaryOfSubTrees;

		private bool? _localValue;

		private int _evaluatedTrueCount = 0;
		private int _evaluatedFalseCount = 0;

		public DecisionTreeLevel(double chiTestLimit)
		{
			ChiTestLimit = chiTestLimit;
		}

		public void D3(List<DataSetAttribute> attributes, List<DataSetValue> values)
		{
			// Check whether we even need to split or not
			int totalTrueValues = values.Count(v => v.Output);
			int totalFalseValues = values.Count(v => !v.Output);

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

			// Can we split on attributes?
			if (attributes.Count == 0)
			{
				// Can't split anymore. We'll decide on the most prevalent value
				_localValue = totalTrueValues > totalFalseValues;
				return;
			}

			// First, find the attribute with the highest "E"
			List<DataSetAttributeWithCounts> e = CalculateEForAllAttributes(attributes, values);
			DataSetAttributeWithCounts attributeWithMinEntropy = FindAttributeWithMinEntropy(e);
			_attributeToSplitOn = attributeWithMinEntropy;

			// Is it worth it to split on attributes
			if (!ShouldSplitOnAttributeAccordingToChiSquared(attributeWithMinEntropy))
			{
				// Not worth it to split. We'll decide on the most prevalent value
				_localValue = totalTrueValues > totalFalseValues;
				return;
			}

			// Remove this attribute from the list of new attributes to create new subtrees
			List<DataSetAttribute> newAttributes = attributes.Where(a => a.Name != attributeWithMinEntropy.Name).ToList();

			// Split the values in many sets
			_dictionaryOfSubTrees = new Dictionary<string, DecisionTreeLevel>(attributeWithMinEntropy.PossibleValues.Count);
			var dictionaryOfValues = new Dictionary<string, List<DataSetValue>>();
			foreach (var dataSetValue in values)
			{
				string value = dataSetValue.Values[attributeWithMinEntropy.ValueIndex];
				DecisionTreeLevel localTreeLevel;
				List<DataSetValue> localValues;
				if (!_dictionaryOfSubTrees.TryGetValue(value, out localTreeLevel))
				{
					localTreeLevel = new DecisionTreeLevel(ChiTestLimit);
					_dictionaryOfSubTrees[value] = localTreeLevel;
					localValues = new List<DataSetValue>();
					dictionaryOfValues[value] = localValues;
				}
				else
				{
					localValues = dictionaryOfValues[value];
				}

				localValues.Add(dataSetValue);
			}

			// Recursively run D3 on them
			foreach (var decisionTree in _dictionaryOfSubTrees)
			{
				var localValues = dictionaryOfValues[decisionTree.Key];
				decisionTree.Value.D3(newAttributes, localValues);
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

			return chiQuareCumulative >= ChiTestLimit;
		}

		private DataSetAttributeWithCounts FindAttributeWithMinEntropy(List<DataSetAttributeWithCounts> dataSetAttributeWithCountses)
		{
			double minEntropy = dataSetAttributeWithCountses.Select(s => s.Entropy).Min();
			var attributeWithMinEntropy = dataSetAttributeWithCountses.First(a => a.Entropy == minEntropy);
			return attributeWithMinEntropy;
		}

		private List<DataSetAttributeWithCounts> CalculateEForAllAttributes(List<DataSetAttribute> attributes, List<DataSetValue> values)
		{
			// First, compute the count of appearences of possible vlaues and their counts
			List<DataSetAttributeWithCounts> attributeWithCounts =
				attributes.Select(s => new DataSetAttributeWithCounts(s.Name, s.PossibleValues, s.ValueIndex)).ToList();
			foreach (var value in values)
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

		private bool EvaluatePrivate(List<string> list)
		{
			if (_localValue.HasValue)
			{
				return _localValue.Value;
			}

			string attributeValue = list[_attributeToSplitOn.ValueIndex];

			// Need to handle case where we've never seen the value
			DecisionTreeLevel nextTreeLevel;
			if (!_dictionaryOfSubTrees.TryGetValue(attributeValue, out nextTreeLevel))
			{
				int maxAppearCount = _attributeToSplitOn.PossibleValueCounts.Max(m => m.Value.AppearCount);
				var attributeToChoose = _attributeToSplitOn.PossibleValueCounts.First(s => s.Value.AppearCount == maxAppearCount);
				nextTreeLevel = _dictionaryOfSubTrees[attributeToChoose.Key];
			}

			return nextTreeLevel.Evaluate(list);
		}

		public bool Evaluate(List<string> list)
		{
			bool evaluated = EvaluatePrivate(list);
			if (evaluated)
			{
				_evaluatedTrueCount++;
			}
			else
			{
				_evaluatedFalseCount++;
			}
			return evaluated;
		}

		public string SerializeDecisionTree()
		{
			var decisionTree = GetDecisionTree();
			return JsonConvert.SerializeObject(decisionTree, Formatting.Indented);
		}

		public int GetNodeCount()
		{
			if (_localValue.HasValue)
			{
				return 1;
			}

			int nodeCount = 1;
			foreach (var keyValuePair in _dictionaryOfSubTrees)
			{
				nodeCount += keyValuePair.Value.GetNodeCount();
			}
			return nodeCount;
		}

		private string EvaluatedString()
		{
			string s = "";
			if (_evaluatedTrueCount > 0)
			{
				s += "(true=" + _evaluatedTrueCount + ") ";
			}
			if (_evaluatedFalseCount > 0)
			{
				s += "(false=" + _evaluatedFalseCount + ") ";
			}

			if (_evaluatedTrueCount == 0 && _evaluatedFalseCount == 0)
			{
				s += "(0 evaluated)";
			}

			return s;
		}

		private object GetDecisionTree()
		{
			if (_localValue.HasValue)
			{
				return _localValue + EvaluatedString();
			}

			var dict = new Dictionary<string, Dictionary<string, object>>();
			var internalDict = new Dictionary<string, object>();
			dict[_attributeToSplitOn.Name + EvaluatedString()] = internalDict;
			foreach (var keyValuePair in _dictionaryOfSubTrees)
			{
				internalDict[keyValuePair.Key] = keyValuePair.Value.GetDecisionTree();
			}
			return dict;
		}

		public bool? TrimTree()
		{
			if (_localValue.HasValue)
			{
				return _localValue.Value;
			}

			HashSet<bool?> booleanBelow = new HashSet<bool?>();
			foreach (var keyValuePair in _dictionaryOfSubTrees)
			{
				bool? val = keyValuePair.Value.TrimTree();
				booleanBelow.Add(val);
			}

			bool hasBoth = booleanBelow.Contains(true) && booleanBelow.Contains(false);
			bool hasNull = booleanBelow.Contains(null);

			if (hasBoth || hasNull)
			{
				return null;
			}

			if (booleanBelow.Contains(true))
			{
				_localValue = true;
				_dictionaryOfSubTrees = null;
				return true;
			}

			if (booleanBelow.Contains(false))
			{
				_localValue = false;
				_dictionaryOfSubTrees = null;
				return false;
			}

			throw new InvalidOperationException();
		}
	}
}

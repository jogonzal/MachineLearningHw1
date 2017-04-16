using System;
using System.Collections.Generic;
using System.Linq;

using MachineLearningHw1.DataSet;

namespace MachineLearningHw1.DecisionTreeClasses
{
	public class PossibleValuesCounts
	{
		public int AppearCount { get; set; }
		public int AppearWhenTrueCount { get; set; }

		public PossibleValuesCounts(int appearCount, int appearWhenTrueCount)
		{
			AppearCount = appearCount;
			AppearWhenTrueCount = appearWhenTrueCount;
		}

		public override string ToString()
		{
			return $"{AppearWhenTrueCount} / {AppearCount}";
		}
	}

	public class DataSetAttributeWithCounts : DataSetAttribute
	{
		private readonly Dictionary<string, PossibleValuesCounts> _possibleValueCounts;

		public double Entropy { get; set; }

		public DataSetAttributeWithCounts(string name, HashSet<string> possibleValues) : base(name, possibleValues)
		{
			_possibleValueCounts = new Dictionary<string, PossibleValuesCounts>(PossibleValues.Count);
			Entropy = -999999999999;
		}

		public void UpdateWith(string attributeValue, bool valueOutput)
		{
			PossibleValuesCounts possibleCount;
			// Add it if it isn't there
			if (!_possibleValueCounts.TryGetValue(attributeValue, out possibleCount))
			{
				possibleCount = new PossibleValuesCounts(0, 0);
				_possibleValueCounts[attributeValue] = possibleCount;
			}

			possibleCount.AppearCount++;
			if (valueOutput)
			{
				possibleCount.AppearWhenTrueCount++;
			}
		}

		public void CalculateEntropy()
		{
			double accumulatedEntropy = 0;
			int totalAppearancesForAllPossibleValues = _possibleValueCounts.Select(s => s.Value.AppearCount).Sum();
			foreach (var possibleValuesCountse in _possibleValueCounts)
			{
				int appearCount = possibleValuesCountse.Value.AppearCount;
				int appearCountWhenTrue = possibleValuesCountse.Value.AppearWhenTrueCount;

				if (appearCountWhenTrue == 0)
				{
					continue;
				}

				double weight = 1.0 * appearCount / totalAppearancesForAllPossibleValues;
				var i = -1.0 * appearCountWhenTrue / appearCount * Math.Log(1.0 * appearCountWhenTrue / appearCount, 2);

				double localEntropy = weight*i;

				accumulatedEntropy += localEntropy;
			}

			Entropy = accumulatedEntropy;
		}

		public override string ToString()
		{
			return string.Format($"{Name}, {Entropy}");
		}
	}
}

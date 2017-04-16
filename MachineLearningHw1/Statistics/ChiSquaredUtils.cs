using MathNet.Numerics.Distributions;

namespace MachineLearningHw1.Statistics
{
	public static class ChiSquaredUtils
	{
		public static double CalculateChiSquareCDT(int degreesOfFreedom, double testValue)
		{
			// 0 degrees of freedom is not valid
			if (degreesOfFreedom == 0)
			{
				degreesOfFreedom = 1;
			}

			ChiSquared c = new ChiSquared(degreesOfFreedom);

			return c.CumulativeDistribution(testValue);
		}
	}
}

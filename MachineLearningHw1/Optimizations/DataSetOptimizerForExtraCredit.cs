using System.Collections.Generic;
using System.Linq;

using MachineLearningHw1.DataSet;

namespace MachineLearningHw1.Optimizations
{
	public static class DataSetOptimizerForExtraCredit
	{
		public static void OptimizeDataSetForExtraCredit(List<DataSetAttribute> trainingDataAttributes, List<DataSetValue> trainingDataValues)
		{
			// Remove state
			DataSetAttribute usStateAttribute = trainingDataAttributes.First(n => n.Name == "'US State'");
			DataSetAttribute browserAttribute = trainingDataAttributes.First(n => n.Name == "'Session Browser Family'");
			DataSetAttribute level3PathLast = trainingDataAttributes.First(n => n.Name == "'Assortment Level 3 Path Last'");
			trainingDataAttributes.Remove(usStateAttribute);
			trainingDataAttributes.Remove(browserAttribute);

			foreach (var trainingDataValue in trainingDataValues)
			{
				trainingDataValue.Values[browserAttribute.ValueIndex] = TransformBrowserAttributeIntoIsSpam(trainingDataValue.Values[browserAttribute.ValueIndex]);
				trainingDataValue.Values[usStateAttribute.ValueIndex] = GetVisitedLegWearPage(trainingDataValue.Values[level3PathLast.ValueIndex]);
			}

			DataSetAttribute isSpamUserAgentAttribute = new DataSetAttribute("IsSpamUserAgent", new HashSet<string>() {"true", "false"}, browserAttribute.ValueIndex);
			DataSetAttribute visitedLegWearPageAttribute = new DataSetAttribute("VisitedLegWearPage", new HashSet<string>() {"true", "false"}, usStateAttribute.ValueIndex);
			trainingDataAttributes.Add(isSpamUserAgentAttribute);
			trainingDataAttributes.Add(visitedLegWearPageAttribute);
		}

		private static string GetVisitedLegWearPage(string s)
		{
			return s.ToLowerInvariant().Contains("leg") ? "true" : "false";
		}

		private static string TransformBrowserAttributeIntoIsSpam(string browser)
		{
			// Google,'Teleport Pro',AltaVista,'PerMan Surfer','Novell Border Manager',link-check,EmailSiphon,Java,Genie,WebTV,WebTrends,'Enfish Tracker',Lycos,ergyBot,'Lotus Notes',Lesszilla,Netscape,InfoSeek,'Northern Light',Unknown,'Cute FTP','Nitro e-mail collector',AOL,Mozilla,'Internet Explorer',Other
			browser = browser.ToLowerInvariant();
			if (browser.Contains("google")
			    || browser.Contains("altavista")
			    || browser.Contains("surfer")
			    || browser.Contains("link")
			    || browser.Contains("java")
			    || browser.Contains("webtrends")
			    || browser.Contains("tracker")
			    || browser.Contains("collector")
				)
			{
				return "true";
			}

			return "false";
		}
	}
}

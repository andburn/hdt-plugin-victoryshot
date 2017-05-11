using System;
using HDT.Plugins.VictoryShot.Utilities;
using NUnit.Framework;

namespace HDT.Plugins.VictoryShot.Tests
{
	[TestFixture]
	public class NamingPatternTest
	{
		private NamingPattern np;

		[SetUp]
		public void Setup()
		{
			np = null;
		}

		[Test]
		public void SingleTokenPattern()
		{
			Assert.IsTrue(NamingPattern.TryParse("{PlayerName}", out np));
			Assert.AreEqual("Player1", ApplyPattern(np));
		}

		[Test]
		public void MultiTokenPattern()
		{
			Assert.IsTrue(NamingPattern.TryParse("{PlayerName}{PlayerClass}", out np));
			Assert.AreEqual("Player1Mage", ApplyPattern(np));
		}

		[Test]
		public void MultiTokenPlusStringsPattern()
		{
			Assert.IsTrue(NamingPattern.TryParse("Prefix {PlayerName} vs {OpponentName} Postfix", out np));
			Assert.AreEqual("Prefix Player1 vs Player2 Postfix", ApplyPattern(np));
		}

		[Test]
		public void PatternWithNoTokens()
		{
			Assert.IsTrue(NamingPattern.TryParse("Just a String", out np));
			Assert.AreEqual("Just a String", ApplyPattern(np));
		}

		[Test]
		public void EmptyPattern()
		{
			Assert.IsFalse(NamingPattern.TryParse("", out np));
			Assert.IsNull(np);
		}

		[Test]
		public void InValidTokens()
		{
			Assert.IsFalse(NamingPattern.TryParse("{nolower}", out np));
			Assert.IsNull(np);
		}

		[Test]
		public void ValidTokens()
		{
			Assert.IsTrue(NamingPattern.TryParse("{PlayerName}", out np));
			Assert.AreEqual("Player1", ApplyPattern(np));
		}

		[Test]
		public void ParseTokensInCorrectOrder()
		{
			NamingPattern.TryParse("{PlayerName}{PlayerClass}", out np);
			Assert.AreEqual(2, np.Pattern.Count);
			Assert.AreEqual("{PlayerName}", np.Pattern[0]);
			Assert.AreEqual("{PlayerClass}", np.Pattern[1]);
			Assert.AreEqual("Player1Mage", ApplyPattern(np));
		}

		[Test]
		public void ParseFullPatternInCorrectOrder()
		{
			NamingPattern.TryParse(" Before {PlayerName} Middle {PlayerClass} After ", out np);
			Assert.AreEqual(5, np.Pattern.Count);
			Assert.AreEqual(" Before ", np.Pattern[0]);
			Assert.AreEqual("{PlayerName}", np.Pattern[1]);
			Assert.AreEqual(" Middle ", np.Pattern[2]);
			Assert.AreEqual("{PlayerClass}", np.Pattern[3]);
			Assert.AreEqual(" After ", np.Pattern[4]);
			Assert.AreEqual(" Before Player1 Middle Mage After ", ApplyPattern(np));
		}

		[Test]
		public void PatternEvaluatesCorrectly()
		{
			NamingPattern.TryParse(" Before {PlayerName} ({PlayerClass}) VS {OpponentName} ({OpponentClass}) After ", out np);
			Assert.AreEqual(" Before Player1 (Mage) VS Player2 (Warlock) After ", ApplyPattern(np));
		}

		[Test]
		public void ValidDatePattern()
		{
			NamingPattern.TryParse("{Date:ddMMyyy HH:mm}", out np);
			Assert.AreEqual(DateTime.Now.ToString("ddMMyyy HH:mm"), ApplyPattern(np));
		}

		[Test]
		public void InValidDatePatternUsesDefault()
		{
			NamingPattern.TryParse("{Date:X}", out np);
			Assert.AreEqual(DateTime.Now.ToString("dd.MM.yyy_HH.mm"), ApplyPattern(np));
		}

		[Test]
		public void UnknownCustomDatePatternUsesDefault()
		{
			NamingPattern.TryParse("{Date:ABC}", out np);
			Assert.AreEqual(DateTime.Now.ToString("dd.MM.yyy_HH.mm"), ApplyPattern(np));
		}		

		private static string ApplyPattern(NamingPattern np)
		{
			return np.Apply("Mage", "Warlock", "Player1", "Player2");
		}
	}
}
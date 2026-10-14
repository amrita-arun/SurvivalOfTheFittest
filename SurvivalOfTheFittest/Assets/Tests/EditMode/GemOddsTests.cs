using NUnit.Framework;
using SurvivalOfTheFittest;
using SurvivalOfTheFittest.UI;

namespace SurvivalOfTheFittest.Tests
{
    public class GemOddsTests
    {
        [TestCase(0, 5)]
        [TestCase(1, 5)]
        [TestCase(2, 5)]
        [TestCase(3, 4)]
        [TestCase(5, 4)]
        [TestCase(6, 3)]
        [TestCase(10, 3)]
        public void SafeAction_RequiredRoll_MatchesGemTier(int gems, int expectedRequired)
        {
            Assert.AreEqual(expectedRequired, GemOddsCalculator.GetSafeRequiredRoll(gems));
        }

        [Test]
        public void With_0_to_2_Gems_Safe_Succeeds_Only_On_5_And_6()
        {
            var required = GemOddsCalculator.GetSafeRequiredRoll(0);
            Assert.AreEqual(5, required);
            Assert.IsFalse(GemOddsCalculator.IsSuccess(4, required));
            Assert.IsTrue(GemOddsCalculator.IsSuccess(5, required));
            Assert.IsTrue(GemOddsCalculator.IsSuccess(6, required));
        }

        [Test]
        public void With_3_to_5_Gems_Safe_Succeeds_On_4_Through_6()
        {
            var required = GemOddsCalculator.GetSafeRequiredRoll(4);
            Assert.AreEqual(4, required);
            Assert.IsFalse(GemOddsCalculator.IsSuccess(3, required));
            Assert.IsTrue(GemOddsCalculator.IsSuccess(4, required));
            Assert.IsTrue(GemOddsCalculator.IsSuccess(6, required));
        }

        [Test]
        public void With_6_Plus_Gems_Safe_Succeeds_On_3_Through_6()
        {
            var required = GemOddsCalculator.GetSafeRequiredRoll(6);
            Assert.AreEqual(3, required);
            Assert.IsFalse(GemOddsCalculator.IsSuccess(2, required));
            Assert.IsTrue(GemOddsCalculator.IsSuccess(3, required));
            Assert.IsTrue(GemOddsCalculator.IsSuccess(6, required));
        }

        [TestCase(0)]
        [TestCase(3)]
        [TestCase(6)]
        public void RiskyAction_Always_Requires_One_Higher_Than_Safe(int gems)
        {
            var safe = GemOddsCalculator.GetSafeRequiredRoll(gems);
            var risky = GemOddsCalculator.GetRiskyRequiredRoll(gems);
            Assert.AreEqual(GemOddsCalculator.ClampRequiredRoll(safe + 1), risky);
            Assert.Greater(risky, safe);
        }

        [Test]
        public void RequiredRoll_Is_Clamped_To_2_Through_6()
        {
            Assert.AreEqual(2, GemOddsCalculator.ClampRequiredRoll(1));
            Assert.AreEqual(6, GemOddsCalculator.ClampRequiredRoll(9));
        }

        [Test]
        public void UiChoiceText_Matches_Actual_Success_Calculation()
        {
            var action = new EncounterActionDef
            {
                Label = "Block and Run",
                Risk = EncounterRisk.Safe,
                RewardHint = "Escape safely"
            };

            const int gems = 4;
            var required = GemOddsCalculator.GetRequiredRoll(gems, action.Risk);
            var ui = EncounterUI.BuildChoiceDescriptionForTest(action, gems);

            StringAssert.Contains(GemOddsCalculator.FormatSuccessRange(required), ui);
            StringAssert.Contains(GemOddsCalculator.FormatSuccessChancePercent(required), ui);
            StringAssert.Contains("Safe", ui);
        }

        [Test]
        public void ResultLine_Matches_Roll_And_Threshold()
        {
            var line = GemOddsCalculator.FormatResultLine(4, 4, true);
            Assert.AreEqual("You rolled a 4. You needed 4 or higher. Success!", line);

            var fail = GemOddsCalculator.FormatResultLine(3, 5, false);
            Assert.AreEqual("You rolled a 3. You needed 5 or higher. Failure!", fail);
        }

        [Test]
        public void Tier_Names_Match_Gem_Counts()
        {
            Assert.AreEqual(ConfidenceTier.Unprepared, GemOddsCalculator.GetTier(2));
            Assert.AreEqual(ConfidenceTier.Confident, GemOddsCalculator.GetTier(3));
            Assert.AreEqual(ConfidenceTier.Thriving, GemOddsCalculator.GetTier(6));
        }
    }
}

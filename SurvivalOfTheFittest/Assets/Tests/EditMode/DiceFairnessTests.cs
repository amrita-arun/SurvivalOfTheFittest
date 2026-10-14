using NUnit.Framework;
using SurvivalOfTheFittest;
using UnityEngine;

namespace SurvivalOfTheFittest.Tests
{
    public class DiceFairnessTests
    {
        [Test]
        public void Die_Results_Are_Always_Between_1_And_6()
        {
            var go = new GameObject("DiceTest");
            var dice = go.AddComponent<DiceRoller>();

            DiceRoller.ClearDebugRoll();
            for (var i = 0; i < 100; i++)
            {
                var roll = dice.RollImmediate();
                Assert.GreaterOrEqual(roll, 1);
                Assert.LessOrEqual(roll, 6);
            }

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Die_Is_Not_Secretly_Weighted_Across_Large_Sample()
        {
            var go = new GameObject("DiceFairness");
            var dice = go.AddComponent<DiceRoller>();
            DiceRoller.ClearDebugRoll();

            const int samples = 6000;
            var counts = new int[7];
            for (var i = 0; i < samples; i++)
                counts[dice.RollImmediate()]++;

            // Fair d6 expected ~1000 each. Allow generous chi-like tolerance for CI noise.
            for (var face = 1; face <= 6; face++)
            {
                Assert.Greater(counts[face], 700, $"Face {face} under-represented — die may be weighted.");
                Assert.Less(counts[face], 1300, $"Face {face} over-represented — die may be weighted.");
            }

            Object.DestroyImmediate(go);
        }

        [Test]
        public void DebugOverride_Allows_Deterministic_Testing()
        {
            var go = new GameObject("DiceOverride");
            var dice = go.AddComponent<DiceRoller>();

            DiceRoller.SetDebugRoll(6, keep: false);
            Assert.AreEqual(6, dice.RollImmediate());
            // Override consumed after one use
            DiceRoller.ClearDebugRoll();

            Object.DestroyImmediate(go);
        }

        [Test]
        public void No_Additional_Random_Systems_In_Odds_Calculator()
        {
            // GemOddsCalculator must be pure / deterministic for the same inputs.
            var a = GemOddsCalculator.GetRequiredRoll(4, EncounterRisk.Risky);
            var b = GemOddsCalculator.GetRequiredRoll(4, EncounterRisk.Risky);
            Assert.AreEqual(a, b);
            Assert.AreEqual(5, a);
        }
    }
}

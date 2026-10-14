using NUnit.Framework;
using SurvivalOfTheFittest;
using UnityEngine;

namespace SurvivalOfTheFittest.Tests
{
    public class RevealMeterTests
    {
        [Test]
        public void Resetting_Clarity_Does_Not_Reset_Lifetime_Gem_Total()
        {
            var go = new GameObject("RevealMeterTest");
            var meter = go.AddComponent<RevealMeter>();

            meter.RegisterGem(2);
            Assert.AreEqual(2, meter.LifetimeGems);
            Assert.AreEqual(2, meter.SegmentFill);

            meter.ResetClaritySegmentsOnly();
            Assert.AreEqual(0, meter.SegmentFill);
            Assert.AreEqual(2, meter.LifetimeGems);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void BonusGems_Update_Both_Lifetime_And_Clarity_Meter()
        {
            var go = new GameObject("RevealMeterBonusTest");
            var meter = go.AddComponent<RevealMeter>();

            // Simulate two map gems + one bonus gem from encounter
            meter.RegisterGem(1);
            meter.RegisterGem(1);
            var clarity = meter.RegisterGem(1);

            Assert.IsTrue(clarity);
            Assert.AreEqual(3, meter.LifetimeGems);
            Assert.AreEqual(0, meter.SegmentFill); // meter resets on activation

            Object.DestroyImmediate(go);
        }

        [Test]
        public void Every_Third_Gem_Triggers_Clarity_And_Resets_Segments_Only()
        {
            var go = new GameObject("ClarityTriggerTest");
            var meter = go.AddComponent<RevealMeter>();

            Assert.IsFalse(meter.RegisterGem(1));
            Assert.IsFalse(meter.RegisterGem(1));
            Assert.IsTrue(meter.RegisterGem(1));
            Assert.AreEqual(3, meter.LifetimeGems);
            Assert.AreEqual(0, meter.SegmentFill);

            Assert.IsFalse(meter.RegisterGem(1));
            Assert.AreEqual(4, meter.LifetimeGems);
            Assert.AreEqual(1, meter.SegmentFill);

            Object.DestroyImmediate(go);
        }
    }
}

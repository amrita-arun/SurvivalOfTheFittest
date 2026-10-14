using NUnit.Framework;
using SurvivalOfTheFittest;
using UnityEngine;

namespace SurvivalOfTheFittest.Tests
{
    public class RevealMeterTests
    {
        [Test]
        public void RegisterGem_Increases_Lifetime_And_Score()
        {
            var go = new GameObject("RevealMeterTest");
            var meter = go.AddComponent<RevealMeter>();

            meter.RegisterGem(2);
            Assert.AreEqual(2, meter.LifetimeGems);
            Assert.AreEqual(2, meter.Score);

            meter.ResetMeters();
            Assert.AreEqual(0, meter.LifetimeGems);
            Assert.AreEqual(0, meter.Score);

            Object.DestroyImmediate(go);
        }

        [Test]
        public void BonusGems_Add_To_Lifetime_Total()
        {
            var go = new GameObject("RevealMeterBonusTest");
            var meter = go.AddComponent<RevealMeter>();

            meter.RegisterGem(1);
            meter.RegisterGem(1);
            meter.RegisterGem(1);

            Assert.AreEqual(3, meter.LifetimeGems);
            Assert.AreEqual(3, meter.Score);

            Object.DestroyImmediate(go);
        }
    }
}

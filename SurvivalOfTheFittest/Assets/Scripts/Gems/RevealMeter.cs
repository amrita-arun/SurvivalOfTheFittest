using System;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Lifetime green-flag total. Odds use this number. There is no map reveal.
    /// </summary>
    public class RevealMeter : MonoBehaviour
    {
        public int LifetimeGems { get; private set; }
        public int Score { get; private set; }

        public event Action<int> OnMetersChanged;

        public void ResetMeters()
        {
            LifetimeGems = 0;
            Score = 0;
            OnMetersChanged?.Invoke(LifetimeGems);
        }

        public void RegisterGem(int amount = 1)
        {
            if (amount < 1)
                return;

            LifetimeGems += amount;
            Score += amount;
            OnMetersChanged?.Invoke(LifetimeGems);
        }
    }
}

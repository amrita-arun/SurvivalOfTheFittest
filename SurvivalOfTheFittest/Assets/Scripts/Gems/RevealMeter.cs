using System;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Three-segment Clarity meter. Separate from lifetime gem total.
    /// Resetting Clarity does NOT reduce total gems or encounter odds.
    /// </summary>
    public class RevealMeter : MonoBehaviour
    {
        public const int SegmentsRequired = 3;

        [Header("Clarity Reveal")]
        [Tooltip("How long the full maze stays revealed (seconds).")]
        [SerializeField] private float revealDuration = 2f;

        public int SegmentFill { get; private set; }
        public int LifetimeGems { get; private set; }
        public int Score { get; private set; }

        public float RevealDuration => revealDuration;

        public event Action<int, int> OnMetersChanged; // segmentFill, lifetime
        public event Action OnClarityReady;

        public void ResetMeters()
        {
            SegmentFill = 0;
            LifetimeGems = 0;
            Score = 0;
            OnMetersChanged?.Invoke(SegmentFill, LifetimeGems);
        }

        /// <summary>
        /// Adds a gem to score, lifetime total, and Clarity meter.
        /// Returns true if Clarity should activate (every 3rd gem in the meter).
        /// </summary>
        public bool RegisterGem(int amount = 1)
        {
            var clarityTriggered = false;

            for (var i = 0; i < amount; i++)
            {
                LifetimeGems++;
                Score++;
                SegmentFill++;

                if (SegmentFill >= SegmentsRequired)
                {
                    SegmentFill = 0;
                    clarityTriggered = true;
                    OnClarityReady?.Invoke();
                }
            }

            OnMetersChanged?.Invoke(SegmentFill, LifetimeGems);
            return clarityTriggered;
        }

        /// <summary>
        /// Explicitly clears only the Clarity segments (e.g. after reveal finishes).
        /// Lifetime gem total is untouched.
        /// </summary>
        public void ResetClaritySegmentsOnly()
        {
            SegmentFill = 0;
            OnMetersChanged?.Invoke(SegmentFill, LifetimeGems);
        }
    }
}

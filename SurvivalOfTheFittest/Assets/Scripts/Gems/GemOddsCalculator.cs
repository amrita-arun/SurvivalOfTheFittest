using UnityEngine;

namespace SurvivalOfTheFittest
{
    public enum ConfidenceTier
    {
        Unprepared,
        Confident,
        Thriving
    }

    public enum EncounterRisk
    {
        Safe,
        Risky
    }

    /// <summary>
    /// Deterministic gem-tier odds. No randomness — only lowers the required roll.
    /// The die itself remains a fair 1–6 roll elsewhere.
    /// </summary>
    public static class GemOddsCalculator
    {
        public const int MinRequiredRoll = 2;
        public const int MaxRequiredRoll = 6;

        public static ConfidenceTier GetTier(int totalGems)
        {
            if (totalGems >= 6)
                return ConfidenceTier.Thriving;
            if (totalGems >= 3)
                return ConfidenceTier.Confident;
            return ConfidenceTier.Unprepared;
        }

        public static string GetTierDisplayName(ConfidenceTier tier)
        {
            return tier switch
            {
                ConfidenceTier.Unprepared => "Unprepared",
                ConfidenceTier.Confident => "Confident",
                ConfidenceTier.Thriving => "Thriving",
                _ => tier.ToString()
            };
        }

        /// <summary>
        /// Minimum roll (inclusive) needed for a safe action at the given gem total.
        /// 0–2 gems: 5, 3–5 gems: 4, 6+: 3.
        /// </summary>
        public static int GetSafeRequiredRoll(int totalGems)
        {
            return GetTier(totalGems) switch
            {
                ConfidenceTier.Unprepared => 5,
                ConfidenceTier.Confident => 4,
                ConfidenceTier.Thriving => 3,
                _ => 5
            };
        }

        /// <summary>
        /// Risky actions require one higher than safe, clamped to 2–6.
        /// </summary>
        public static int GetRiskyRequiredRoll(int totalGems)
        {
            return ClampRequiredRoll(GetSafeRequiredRoll(totalGems) + 1);
        }

        public static int GetRequiredRoll(int totalGems, EncounterRisk risk)
        {
            return risk == EncounterRisk.Safe
                ? ClampRequiredRoll(GetSafeRequiredRoll(totalGems))
                : GetRiskyRequiredRoll(totalGems);
        }

        public static int ClampRequiredRoll(int required)
        {
            return Mathf.Clamp(required, MinRequiredRoll, MaxRequiredRoll);
        }

        public static bool IsSuccess(int roll, int requiredRoll)
        {
            return roll >= requiredRoll;
        }

        public static int CountSuccessFaces(int requiredRoll)
        {
            requiredRoll = ClampRequiredRoll(requiredRoll);
            return 7 - requiredRoll;
        }

        public static float GetSuccessChance(int requiredRoll)
        {
            return CountSuccessFaces(requiredRoll) / 6f;
        }

        public static string FormatSuccessRange(int requiredRoll)
        {
            requiredRoll = ClampRequiredRoll(requiredRoll);
            return requiredRoll == 6 ? "6" : $"{requiredRoll}–6";
        }

        public static string FormatSuccessChancePercent(int requiredRoll)
        {
            var faces = CountSuccessFaces(requiredRoll);
            var pct = Mathf.RoundToInt(faces / 6f * 100f);
            return $"{pct}%";
        }

        public static string FormatResultLine(int roll, int requiredRoll, bool success)
        {
            var outcome = success ? "Success!" : "Failure!";
            return $"You rolled a {roll}. You needed {requiredRoll} or higher. {outcome}";
        }

        public static string GetConfidenceBlurb(int totalGems)
        {
            var tier = GetTier(totalGems);
            return tier switch
            {
                ConfidenceTier.Unprepared =>
                    "Your gems have barely prepared you for this emotional crisis.",
                ConfidenceTier.Confident =>
                    "Your gems have prepared you for this emotional crisis.",
                ConfidenceTier.Thriving =>
                    "Your gem haul has you thriving through this emotional crisis.",
                _ => "Your gems influence your odds."
            };
        }
    }
}

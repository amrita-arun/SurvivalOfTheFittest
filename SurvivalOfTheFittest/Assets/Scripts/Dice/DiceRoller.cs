using System;
using System.Collections;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// THE ONLY place in the project that may call UnityEngine.Random.
    /// Fair six-sided die: every face 1–6 has equal chance.
    /// </summary>
    public class DiceRoller : MonoBehaviour
    {
        public static DiceRoller Instance { get; private set; }

        [Header("Animation")]
        [Tooltip("How long the die visually cycles before settling (unscaled seconds).")]
        [SerializeField] private float rollDuration = 0.85f;

        [Tooltip("How often the preview face advances during the roll animation.")]
        [SerializeField] private float previewTickInterval = 0.07f;

        /// <summary>
        /// Optional deterministic override for tests. When set, Roll() returns this value
        /// instead of calling Random. Cleared after one use unless KeepOverride is true.
        /// </summary>
        public static int? DebugOverrideValue { get; set; }

        public static bool KeepDebugOverride { get; set; }

        public bool IsRolling { get; private set; }

        public event Action<int> OnRollCompleted;

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        /// <summary>
        /// Rolls a fair d6. Animation preview uses a deterministic cycle; only the final
        /// result uses randomness (or the debug override).
        /// </summary>
        public void CancelRoll()
        {
            StopAllCoroutines();
            IsRolling = false;
        }

        public void Roll(Action<int> onComplete, Action<int> onPreviewFace = null)
        {
            if (IsRolling)
                return;

            StartCoroutine(RollRoutine(onComplete, onPreviewFace));
        }

        public int RollImmediate()
        {
            return GenerateFairFace();
        }

        private IEnumerator RollRoutine(Action<int> onComplete, Action<int> onPreviewFace)
        {
            IsRolling = true;
            var elapsed = 0f;
            var previewFace = 1;
            var tick = 0;

            while (elapsed < rollDuration)
            {
                // Deterministic preview sequence — no Random calls.
                previewFace = (tick % 6) + 1;
                onPreviewFace?.Invoke(previewFace);
                tick++;
                elapsed += previewTickInterval;
                yield return new WaitForSecondsRealtime(previewTickInterval);
            }

            var result = GenerateFairFace();
            onPreviewFace?.Invoke(result);
            IsRolling = false;
            OnRollCompleted?.Invoke(result);
            onComplete?.Invoke(result);
        }

        /// <summary>
        /// Sole UnityEngine.Random usage in the entire game.
        /// </summary>
        private static int GenerateFairFace()
        {
            if (DebugOverrideValue.HasValue)
            {
                var forced = Mathf.Clamp(DebugOverrideValue.Value, 1, 6);
                if (!KeepDebugOverride)
                    DebugOverrideValue = null;
                return forced;
            }

            // Inclusive 1..6 — Random.Range max is exclusive for ints.
            return UnityEngine.Random.Range(1, 7);
        }

        public static void SetDebugRoll(int value, bool keep = false)
        {
            DebugOverrideValue = Mathf.Clamp(value, 1, 6);
            KeepDebugOverride = keep;
        }

        public static void ClearDebugRoll()
        {
            DebugOverrideValue = null;
            KeepDebugOverride = false;
        }
    }
}

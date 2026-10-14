using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Tiny runtime-generated one-shot tones so the prototype has sound
    /// without shipping audio files.
    /// </summary>
    public static class ProceduralSfx
    {
        private const int SampleRate = 22050;

        public static AudioClip Gem { get; private set; }
        public static AudioClip Encounter { get; private set; }
        public static AudioClip Dice { get; private set; }
        public static AudioClip Success { get; private set; }
        public static AudioClip Failure { get; private set; }
        public static AudioClip Clarity { get; private set; }
        public static AudioClip Win { get; private set; }
        public static AudioClip Loss { get; private set; }
        public static AudioClip Dash { get; private set; }
        public static AudioClip Footstep { get; private set; }
        public static AudioClip Blocked { get; private set; }
        public static AudioClip UiClick { get; private set; }

        private static bool _built;

        public static void EnsureBuilt()
        {
            if (_built)
                return;
            _built = true;

            Gem = Blip(880f, 0.09f, 0.35f, slideTo: 1320f);
            Encounter = Blip(180f, 0.22f, 0.4f, slideTo: 120f, square: true);
            Dice = NoiseBurst(0.12f, 0.22f);
            Success = Chord(new[] { 523f, 659f, 784f }, 0.28f, 0.32f);
            Failure = Blip(220f, 0.25f, 0.38f, slideTo: 90f, square: true);
            Clarity = Sweep(300f, 900f, 0.45f, 0.28f);
            Win = Chord(new[] { 392f, 523f, 659f, 784f }, 0.55f, 0.35f);
            Loss = Sweep(400f, 120f, 0.5f, 0.35f);
            Dash = Blip(240f, 0.08f, 0.25f, slideTo: 480f);
            Footstep = NoiseBurst(0.035f, 0.12f);
            Blocked = Blip(140f, 0.1f, 0.3f, square: true);
            UiClick = Blip(660f, 0.04f, 0.2f);
        }

        private static AudioClip Blip(float startHz, float duration, float volume, float slideTo = -1f, bool square = false)
        {
            var samples = Mathf.CeilToInt(SampleRate * duration);
            var data = new float[samples];
            var endHz = slideTo > 0f ? slideTo : startHz;

            for (var i = 0; i < samples; i++)
            {
                var t = i / (float)SampleRate;
                var n = i / (float)samples;
                var hz = Mathf.Lerp(startHz, endHz, n);
                var env = Envelope(n, 0.05f, 0.35f);
                var phase = t * hz;
                var wave = square
                    ? (Mathf.Repeat(phase, 1f) < 0.5f ? 1f : -1f) * 0.55f
                    : Mathf.Sin(phase * Mathf.PI * 2f);
                data[i] = wave * env * volume;
            }

            return ClipFrom("sfx_blip", data);
        }

        private static AudioClip Sweep(float fromHz, float toHz, float duration, float volume)
        {
            return Blip(fromHz, duration, volume, slideTo: toHz);
        }

        private static AudioClip Chord(float[] freqs, float duration, float volume)
        {
            var samples = Mathf.CeilToInt(SampleRate * duration);
            var data = new float[samples];
            for (var i = 0; i < samples; i++)
            {
                var t = i / (float)SampleRate;
                var n = i / (float)samples;
                var env = Envelope(n, 0.08f, 0.4f);
                float sum = 0f;
                foreach (var f in freqs)
                    sum += Mathf.Sin(t * f * Mathf.PI * 2f);
                data[i] = (sum / freqs.Length) * env * volume;
            }

            return ClipFrom("sfx_chord", data);
        }

        private static AudioClip NoiseBurst(float duration, float volume)
        {
            var samples = Mathf.CeilToInt(SampleRate * duration);
            var data = new float[samples];
            // Deterministic hash noise (no engine RNG).
            var seed = 7919u;
            for (var i = 0; i < samples; i++)
            {
                seed = seed * 1664525u + 1013904223u;
                var n = i / (float)samples;
                var env = Envelope(n, 0.02f, 0.5f);
                var noise = ((seed >> 8) & 0xFFFF) / 32768f - 1f;
                data[i] = noise * env * volume;
            }

            return ClipFrom("sfx_noise", data);
        }

        private static float Envelope(float n, float attack, float releaseStart)
        {
            if (n < attack)
                return n / attack;
            if (n > releaseStart)
                return 1f - (n - releaseStart) / Mathf.Max(0.001f, 1f - releaseStart);
            return 1f;
        }

        private static AudioClip ClipFrom(string name, float[] data)
        {
            var clip = AudioClip.Create(name, data.Length, 1, SampleRate, false);
            clip.SetData(data, 0);
            return clip;
        }
    }
}

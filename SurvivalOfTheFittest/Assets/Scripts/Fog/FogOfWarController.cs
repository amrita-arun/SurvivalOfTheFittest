using System.Collections;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Nearly opaque darkness with a soft circular visibility hole that follows the player.
    /// Previously visited areas return to darkness — no permanent uncovering.
    /// Full Clarity reveal fades the fog out temporarily (deterministic).
    /// </summary>
    public class FogOfWarController : MonoBehaviour
    {
        [Header("Visibility")]
        [Tooltip("World units of clear visibility around the player.")]
        [SerializeField] private float visibilityRadius = 1.4f;

        [SerializeField] private float fogSpriteWorldSize = 80f;
        [SerializeField] private float maxFogAlpha = 1f;
        [SerializeField] private float revealFadeSeconds = 0.35f;

        private Transform _follow;
        private SpriteRenderer _sr;
        private Coroutine _fadeRoutine;
        private float _currentAlpha;

        public void Initialize(Transform followTarget)
        {
            _follow = followTarget;

            _sr = gameObject.GetComponent<SpriteRenderer>();
            if (_sr == null)
                _sr = gameObject.AddComponent<SpriteRenderer>();

            // Soft hole occupies ~visibility portion of the large fog sprite.
            var clearNormalized = (visibilityRadius * 2f) / fogSpriteWorldSize;
            _sr.sprite = SpriteFactory.CreateFogHoleSprite(256, clearNormalized, 0.04f);
            _sr.sortingOrder = 50;
            _sr.color = new Color(0f, 0f, 0f, maxFogAlpha);
            _currentAlpha = maxFogAlpha;

            transform.localScale = Vector3.one * (fogSpriteWorldSize / (_sr.sprite.rect.width / _sr.sprite.pixelsPerUnit));
        }

        public void SetVisibilityRadius(float radius)
        {
            visibilityRadius = radius;
            if (_sr != null && _sr.sprite != null)
            {
                var clearNormalized = (visibilityRadius * 2f) / fogSpriteWorldSize;
                _sr.sprite = SpriteFactory.CreateFogHoleSprite(256, clearNormalized, 0.04f);
            }
        }

        private void LateUpdate()
        {
            if (_follow == null)
                return;

            var p = _follow.position;
            transform.position = new Vector3(p.x, p.y, 0f);
        }

        public void PlayFullReveal(float holdDuration, System.Action onComplete = null)
        {
            if (_fadeRoutine != null)
                StopCoroutine(_fadeRoutine);
            _fadeRoutine = StartCoroutine(FullRevealRoutine(holdDuration, onComplete));
        }

        private IEnumerator FullRevealRoutine(float holdDuration, System.Action onComplete)
        {
            yield return FadeTo(0f, revealFadeSeconds);
            yield return new WaitForSecondsRealtime(holdDuration);
            yield return FadeTo(maxFogAlpha, revealFadeSeconds);
            _fadeRoutine = null;
            onComplete?.Invoke();
        }

        private IEnumerator FadeTo(float target, float duration)
        {
            var start = _currentAlpha;
            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                _currentAlpha = Mathf.Lerp(start, target, Mathf.Clamp01(t / duration));
                ApplyAlpha();
                yield return null;
            }

            _currentAlpha = target;
            ApplyAlpha();
        }

        private void ApplyAlpha()
        {
            if (_sr == null)
                return;
            var c = _sr.color;
            c.a = _currentAlpha;
            _sr.color = c;
        }

        public void ResetFog()
        {
            if (_fadeRoutine != null)
            {
                StopCoroutine(_fadeRoutine);
                _fadeRoutine = null;
            }

            _currentAlpha = maxFogAlpha;
            ApplyAlpha();
        }
    }
}

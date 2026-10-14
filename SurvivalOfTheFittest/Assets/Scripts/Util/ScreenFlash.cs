using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace SurvivalOfTheFittest
{
    public class ScreenFlash : MonoBehaviour
    {
        public static ScreenFlash Instance { get; private set; }

        private Image _flash;
        private Coroutine _routine;

        private void Awake()
        {
            Instance = this;
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void Build(Transform canvasRoot)
        {
            var go = new GameObject("ScreenFlash");
            go.transform.SetParent(canvasRoot, false);
            var rt = go.AddComponent<RectTransform>();
            rt.anchorMin = Vector2.zero;
            rt.anchorMax = Vector2.one;
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
            _flash = go.AddComponent<Image>();
            _flash.color = new Color(1f, 1f, 1f, 0f);
            _flash.raycastTarget = false;
        }

        public void Flash(Color color, float peakAlpha = 0.45f, float duration = 0.25f)
        {
            if (_flash == null)
                return;
            if (_routine != null)
                StopCoroutine(_routine);
            _routine = StartCoroutine(FlashRoutine(color, peakAlpha, duration));
        }

        private IEnumerator FlashRoutine(Color color, float peakAlpha, float duration)
        {
            var half = duration * 0.35f;
            var t = 0f;
            while (t < half)
            {
                t += Time.unscaledDeltaTime;
                var a = Mathf.Lerp(0f, peakAlpha, t / half);
                _flash.color = new Color(color.r, color.g, color.b, a);
                yield return null;
            }

            t = 0f;
            var rest = duration - half;
            while (t < rest)
            {
                t += Time.unscaledDeltaTime;
                var a = Mathf.Lerp(peakAlpha, 0f, t / rest);
                _flash.color = new Color(color.r, color.g, color.b, a);
                yield return null;
            }

            _flash.color = new Color(color.r, color.g, color.b, 0f);
            _routine = null;
        }
    }
}

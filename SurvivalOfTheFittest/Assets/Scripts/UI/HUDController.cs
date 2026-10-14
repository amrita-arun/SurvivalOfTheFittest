using UnityEngine;
using UnityEngine.UI;

namespace SurvivalOfTheFittest.UI
{
    public class HUDController : MonoBehaviour
    {
        public static HUDController Instance { get; private set; }

        private Text _timeText;
        private Text _heartsText;
        private Text _gemsText;
        private Text _clarityText;
        private Text _tierText;
        private Text _controlsText;
        private Text _bannerText;
        private CanvasGroup _rootGroup;
        private Image[] _clarityPips;

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
            var root = UiFactory.Box(canvasRoot, "HUD", Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            _rootGroup = root.gameObject.AddComponent<CanvasGroup>();
            _rootGroup.blocksRaycasts = false;

            var top = UiFactory.Box(root, "TopBar", new Vector2(0, 1), new Vector2(1, 1),
                new Vector2(20, -90), new Vector2(-20, -16), new Color(0f, 0f, 0f, 0.45f));

            _timeText = UiFactory.Text(top, "Time", "Time 90", 28, new Color(0.6f, 1f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
            SetRect(_timeText.rectTransform, new Vector2(0, 0), new Vector2(0.25f, 1), Vector2.zero, Vector2.zero);

            _heartsText = UiFactory.Text(top, "Hearts", "♥♥♥", 28, new Color(1f, 0.35f, 0.5f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetRect(_heartsText.rectTransform, new Vector2(0.25f, 0), new Vector2(0.45f, 1), Vector2.zero, Vector2.zero);

            _gemsText = UiFactory.Text(top, "Gems", "Flags 0", 26, new Color(0.3f, 0.9f, 0.45f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetRect(_gemsText.rectTransform, new Vector2(0.45f, 0), new Vector2(0.65f, 1), Vector2.zero, Vector2.zero);

            _tierText = UiFactory.Text(top, "Tier", "Unprepared", 22, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            SetRect(_tierText.rectTransform, new Vector2(0.65f, 0), new Vector2(0.85f, 1), Vector2.zero, Vector2.zero);

            var clarityBox = UiFactory.Box(top, "Clarity", new Vector2(0.85f, 0.15f), new Vector2(0.98f, 0.85f),
                Vector2.zero, Vector2.zero, new Color(0.15f, 0.05f, 0.2f, 0.8f));
            _clarityText = UiFactory.Text(clarityBox, "ClarityLabel", "CLARITY", 14, Color.white, TextAnchor.UpperCenter, FontStyle.Bold);
            SetRect(_clarityText.rectTransform, new Vector2(0, 0.55f), new Vector2(1, 1), Vector2.zero, Vector2.zero);

            _clarityPips = new Image[3];
            for (var i = 0; i < 3; i++)
            {
                var pip = UiFactory.Box(clarityBox, $"Pip{i}", new Vector2(0.1f + i * 0.3f, 0.1f),
                    new Vector2(0.35f + i * 0.3f, 0.5f), Vector2.zero, Vector2.zero, new Color(0.3f, 0.3f, 0.35f, 1f));
                _clarityPips[i] = pip.GetComponent<Image>();
            }

            _controlsText = UiFactory.Text(root, "Controls", "WASD / Arrows  •  R Restart  •  Q/Esc Quit", 18,
                new Color(0.75f, 0.75f, 0.85f, 0.85f), TextAnchor.LowerCenter);
            SetRect(_controlsText.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 12), new Vector2(0, 40));

            _bannerText = UiFactory.Text(root, "Banner", "", 42, new Color(1f, 0.45f, 0.95f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetRect(_bannerText.rectTransform, new Vector2(0.15f, 0.55f), new Vector2(0.85f, 0.7f), Vector2.zero, Vector2.zero);
            _bannerText.gameObject.SetActive(false);
        }

        private static void SetRect(RectTransform rt, Vector2 min, Vector2 max, Vector2 offMin, Vector2 offMax)
        {
            rt.anchorMin = min;
            rt.anchorMax = max;
            rt.offsetMin = offMin;
            rt.offsetMax = offMax;
        }

        public void SetVisible(bool visible)
        {
            if (_rootGroup != null)
                _rootGroup.alpha = visible ? 1f : 0f;
        }

        public void UpdateHud(float timeRemaining, int hearts, int lifetimeGems, int claritySegments)
        {
            if (_timeText != null)
                _timeText.text = $"TIME {Mathf.CeilToInt(Mathf.Max(0f, timeRemaining))}";

            if (_heartsText != null)
            {
                var h = Mathf.Max(0, hearts);
                _heartsText.text = new string('♥', h) + new string('♡', Mathf.Max(0, 3 - h));
            }

            if (_gemsText != null)
                _gemsText.text = $"FLAGS {lifetimeGems}";

            if (_tierText != null)
            {
                var tier = GemOddsCalculator.GetTier(lifetimeGems);
                _tierText.text = GemOddsCalculator.GetTierDisplayName(tier);
            }

            if (_clarityPips != null)
            {
                for (var i = 0; i < _clarityPips.Length; i++)
                {
                    if (_clarityPips[i] == null)
                        continue;
                    _clarityPips[i].color = i < claritySegments
                        ? new Color(1f, 0.4f, 0.95f, 1f)
                        : new Color(0.3f, 0.3f, 0.35f, 1f);
                }
            }
        }

        public void ShowBanner(string message, float duration = 2f)
        {
            StopAllCoroutines();
            StartCoroutine(BannerRoutine(message, duration));
        }

        private System.Collections.IEnumerator BannerRoutine(string message, float duration)
        {
            _bannerText.gameObject.SetActive(true);
            _bannerText.text = message;
            var t = 0f;
            while (t < duration)
            {
                t += Time.unscaledDeltaTime;
                var pulse = 1f + 0.05f * Mathf.Sin(t * 10f);
                _bannerText.rectTransform.localScale = Vector3.one * pulse;
                yield return null;
            }

            _bannerText.gameObject.SetActive(false);
        }
    }
}

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
        private Text _tierText;
        private Text _encounterText;
        private Text _controlsText;
        private Text _bannerText;
        private Text _dashText;
        private CanvasGroup _rootGroup;
        private Image _dashFill;

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
                new Vector2(16, -100), new Vector2(-16, -12), new Color(0f, 0f, 0f, 0.5f));

            _timeText = UiFactory.Text(top, "Time", "Time 90", 26, new Color(0.6f, 1f, 1f), TextAnchor.MiddleLeft, FontStyle.Bold);
            SetRect(_timeText.rectTransform, new Vector2(0, 0.5f), new Vector2(0.22f, 1), Vector2.zero, Vector2.zero);

            _heartsText = UiFactory.Text(top, "Hearts", "♥♥♥", 26, new Color(1f, 0.35f, 0.5f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetRect(_heartsText.rectTransform, new Vector2(0.22f, 0.5f), new Vector2(0.4f, 1), Vector2.zero, Vector2.zero);

            _gemsText = UiFactory.Text(top, "Gems", "Flags 0", 22, new Color(0.3f, 0.9f, 0.45f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetRect(_gemsText.rectTransform, new Vector2(0.4f, 0.5f), new Vector2(0.58f, 1), Vector2.zero, Vector2.zero);

            _tierText = UiFactory.Text(top, "Tier", "Unprepared", 18, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter);
            SetRect(_tierText.rectTransform, new Vector2(0.58f, 0.5f), new Vector2(0.98f, 1), Vector2.zero, Vector2.zero);

            _encounterText = UiFactory.Text(top, "Encounters", "Obstacles 0/3", 18,
                new Color(1f, 0.55f, 0.75f), TextAnchor.MiddleLeft, FontStyle.Bold);
            SetRect(_encounterText.rectTransform, new Vector2(0, 0), new Vector2(0.45f, 0.5f), Vector2.zero, Vector2.zero);

            var dashBox = UiFactory.Box(top, "Dash", new Vector2(0.5f, 0.08f), new Vector2(0.98f, 0.42f),
                Vector2.zero, Vector2.zero, new Color(0.1f, 0.15f, 0.2f, 0.85f));
            _dashText = UiFactory.Text(dashBox, "DashLabel", "DASH Ready  (Shift/Space)", 14,
                new Color(0.5f, 1f, 0.9f), TextAnchor.MiddleLeft);
            SetRect(_dashText.rectTransform, new Vector2(0.02f, 0), new Vector2(0.7f, 1), Vector2.zero, Vector2.zero);

            var fillBg = UiFactory.Box(dashBox, "DashFillBg", new Vector2(0.72f, 0.25f), new Vector2(0.96f, 0.75f),
                Vector2.zero, Vector2.zero, new Color(0.2f, 0.25f, 0.3f, 1f));
            var fill = UiFactory.Box(fillBg, "DashFill", new Vector2(0, 0), new Vector2(1, 1),
                Vector2.zero, Vector2.zero, new Color(0.35f, 1f, 0.85f, 1f));
            _dashFill = fill.GetComponent<Image>();
            _dashFill.type = Image.Type.Filled;
            _dashFill.fillMethod = Image.FillMethod.Horizontal;

            _controlsText = UiFactory.Text(root, "Controls",
                "WASD / Arrows move  •  Shift/Space dash  •  R restart  •  F reveal  •  Q/Esc quit", 17,
                new Color(0.75f, 0.75f, 0.85f, 0.9f), TextAnchor.LowerCenter);
            SetRect(_controlsText.rectTransform, new Vector2(0, 0), new Vector2(1, 0), new Vector2(0, 12), new Vector2(0, 40));

            _bannerText = UiFactory.Text(root, "Banner", "", 40, new Color(1f, 0.45f, 0.95f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetRect(_bannerText.rectTransform, new Vector2(0.12f, 0.55f), new Vector2(0.88f, 0.7f), Vector2.zero, Vector2.zero);
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

        public void UpdateHud(
            float timeRemaining,
            int hearts,
            int lifetimeGems,
            int encountersResolved,
            int encountersRequired,
            float dashCooldownNormalized)
        {
            if (_timeText != null)
            {
                var secs = Mathf.CeilToInt(Mathf.Max(0f, timeRemaining));
                _timeText.text = $"TIME {secs}";
                _timeText.color = timeRemaining <= 15f
                    ? Color.Lerp(new Color(1f, 0.3f, 0.35f), new Color(1f, 0.85f, 0.2f), Mathf.PingPong(Time.unscaledTime * 3f, 1f))
                    : new Color(0.6f, 1f, 1f);
            }

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

            if (_encounterText != null)
            {
                var unlocked = encountersResolved >= encountersRequired;
                _encounterText.text = unlocked
                    ? $"Obstacles {encountersResolved}/{encountersRequired}  ✓ EXIT OPEN"
                    : $"Obstacles {encountersResolved}/{encountersRequired}  (exit locked)";
                _encounterText.color = unlocked
                    ? new Color(0.45f, 1f, 0.7f)
                    : new Color(1f, 0.55f, 0.75f);
            }

            if (_dashFill != null)
            {
                var ready = dashCooldownNormalized <= 0.01f;
                _dashFill.fillAmount = ready ? 1f : 1f - dashCooldownNormalized;
                _dashFill.color = ready
                    ? new Color(0.35f, 1f, 0.85f, 1f)
                    : new Color(0.4f, 0.55f, 0.6f, 1f);
                if (_dashText != null)
                    _dashText.text = ready ? "DASH Ready  (Shift/Space)" : "DASH cooling…";
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

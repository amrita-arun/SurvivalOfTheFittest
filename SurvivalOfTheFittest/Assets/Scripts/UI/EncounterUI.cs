using UnityEngine;
using UnityEngine.UI;

namespace SurvivalOfTheFittest.UI
{
    public class EncounterUI : MonoBehaviour
    {
        public static EncounterUI Instance { get; private set; }

        private GameObject _root;
        private Text _nameText;
        private Text _dialogueText;
        private Text _confidenceText;
        private Text _blurbText;
        private Text _dieText;
        private Text _resultText;
        private GameObject _choicePanel;
        private GameObject _resultPanel;
        private Button _safeBtn;
        private Button _riskyBtn;
        private Button _ackBtn;
        private Text _safeLabel;
        private Text _riskyLabel;
        private EncounterDefinition _current;
        private EncounterActionDef _safe;
        private EncounterActionDef _risky;

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
            _root = UiFactory.Panel(canvasRoot, "EncounterPanel", new Color(0.02f, 0.02f, 0.05f, 0.82f),
                Vector2.zero, Vector2.one);
            _root.SetActive(false);

            var card = UiFactory.Box(_root.transform, "Card", new Vector2(0.18f, 0.1f), new Vector2(0.82f, 0.9f),
                Vector2.zero, Vector2.zero, new Color(0.1f, 0.04f, 0.14f, 0.97f));

            _nameText = UiFactory.Text(card, "Name", "", 34, new Color(1f, 0.45f, 0.85f), TextAnchor.UpperCenter, FontStyle.Bold);
            SetAnchors(_nameText.rectTransform, 0.05f, 0.88f, 0.95f, 0.98f);

            _dialogueText = UiFactory.Text(card, "Dialogue", "", 22, new Color(0.95f, 0.9f, 1f), TextAnchor.UpperCenter, FontStyle.Italic);
            SetAnchors(_dialogueText.rectTransform, 0.08f, 0.78f, 0.92f, 0.88f);

            _confidenceText = UiFactory.Text(card, "Confidence", "", 22, new Color(1f, 0.85f, 0.4f), TextAnchor.MiddleCenter, FontStyle.Bold);
            SetAnchors(_confidenceText.rectTransform, 0.08f, 0.7f, 0.92f, 0.78f);

            _blurbText = UiFactory.Text(card, "Blurb", "", 18, new Color(0.8f, 0.8f, 0.9f), TextAnchor.MiddleCenter);
            SetAnchors(_blurbText.rectTransform, 0.08f, 0.64f, 0.92f, 0.7f);

            _dieText = UiFactory.Text(card, "Die", "⚀", 72, Color.white, TextAnchor.MiddleCenter, FontStyle.Bold);
            SetAnchors(_dieText.rectTransform, 0.35f, 0.48f, 0.65f, 0.64f);
            _dieText.gameObject.SetActive(false);

            _choicePanel = UiFactory.Box(card, "Choices", new Vector2(0.05f, 0.08f), new Vector2(0.95f, 0.62f),
                Vector2.zero, Vector2.zero).gameObject;

            _safeBtn = CreateChoiceButton(_choicePanel.transform, "SafeBtn", new Vector2(-200f, -20f),
                () => EncounterManager.Instance?.ChooseAction(_safe));
            _safeLabel = _safeBtn.GetComponentInChildren<Text>();

            _riskyBtn = CreateChoiceButton(_choicePanel.transform, "RiskyBtn", new Vector2(200f, -20f),
                () => EncounterManager.Instance?.ChooseAction(_risky));
            _riskyLabel = _riskyBtn.GetComponentInChildren<Text>();

            _resultPanel = UiFactory.Box(card, "ResultPanel", new Vector2(0.08f, 0.08f), new Vector2(0.92f, 0.48f),
                Vector2.zero, Vector2.zero).gameObject;
            _resultPanel.SetActive(false);

            _resultText = UiFactory.Text(_resultPanel.transform, "Result", "", 24, Color.white, TextAnchor.UpperCenter);
            SetAnchors(_resultText.rectTransform, 0f, 0.35f, 1f, 1f);

            _ackBtn = UiFactory.Button(_resultPanel.transform, "AckBtn", "CONTINUE", new Color(0.7f, 0.2f, 0.55f),
                () => EncounterManager.Instance?.AcknowledgeResult(),
                new Vector2(0f, -80f), new Vector2(240f, 52f));
        }

        private static Button CreateChoiceButton(Transform parent, string name, Vector2 pos, UnityEngine.Events.UnityAction action)
        {
            var btn = UiFactory.Button(parent, name, "", new Color(0.35f, 0.1f, 0.4f), action, pos, new Vector2(360f, 160f));
            var label = btn.GetComponentInChildren<Text>();
            label.fontSize = 18;
            label.alignment = TextAnchor.UpperLeft;
            label.rectTransform.offsetMin = new Vector2(16, 12);
            label.rectTransform.offsetMax = new Vector2(-16, -12);
            return btn;
        }

        private static void SetAnchors(RectTransform rt, float xMin, float yMin, float xMax, float yMax)
        {
            rt.anchorMin = new Vector2(xMin, yMin);
            rt.anchorMax = new Vector2(xMax, yMax);
            rt.offsetMin = Vector2.zero;
            rt.offsetMax = Vector2.zero;
        }

        public void ShowEncounter(EncounterDefinition def, int lifetimeGems)
        {
            _current = def;
            _safe = def.SafeAction;
            _risky = def.RiskyAction;

            _root.SetActive(true);
            _choicePanel.SetActive(true);
            _resultPanel.SetActive(false);
            _dieText.gameObject.SetActive(false);
            _safeBtn.interactable = true;
            _riskyBtn.interactable = true;

            _nameText.text = def.DisplayName;
            _nameText.color = def.Color;
            _dialogueText.text = $"\"{def.Dialogue}\"";

            RefreshOdds(lifetimeGems);
        }

        public void RefreshOdds(int lifetimeGems)
        {
            if (_current == null)
                return;

            var tier = GemOddsCalculator.GetTier(lifetimeGems);
            _confidenceText.text =
                $"Confidence: {GemOddsCalculator.GetTierDisplayName(tier)} — {lifetimeGems} Gems";
            _blurbText.text = GemOddsCalculator.GetConfidenceBlurb(lifetimeGems) +
                              "\nGems improve your odds by lowering the roll you need. The die stays fair.";

            if (_safeLabel != null && _safe != null)
                _safeLabel.text = FormatChoice(_safe, lifetimeGems);
            if (_riskyLabel != null && _risky != null)
                _riskyLabel.text = FormatChoice(_risky, lifetimeGems);
        }

        private static string FormatChoice(EncounterActionDef action, int gems)
        {
            var required = GemOddsCalculator.GetRequiredRoll(gems, action.Risk);
            var riskTag = action.Risk == EncounterRisk.Safe ? "Safe" : "Risky";
            var reward = string.IsNullOrEmpty(action.RewardHint) ? "" : $"\nReward: {action.RewardHint}";
            return $"{action.Label} — {riskTag}\n" +
                   $"Success on {GemOddsCalculator.FormatSuccessRange(required)}\n" +
                   $"{GemOddsCalculator.FormatSuccessChancePercent(required)} chance{reward}";
        }

        public void ShowRolling(EncounterActionDef action, int required, int gems)
        {
            _safeBtn.interactable = false;
            _riskyBtn.interactable = false;
            _dieText.gameObject.SetActive(true);
            _dieText.text = "1";
            _blurbText.text = $"Rolling… need {required}+  ({action.Label})";
        }

        public void SetDieFace(int face)
        {
            _dieText.gameObject.SetActive(true);
            _dieText.text = face.ToString();
            // Subtle pulse using unscaled time via scale
            var s = 1f + 0.08f * Mathf.Sin(Time.unscaledTime * 24f);
            _dieText.rectTransform.localScale = Vector3.one * s;
        }

        public void ShowResult(string resultLine, bool success, int gemsNow, EncounterActionDef action)
        {
            _choicePanel.SetActive(false);
            _resultPanel.SetActive(true);
            _dieText.rectTransform.localScale = Vector3.one;

            var flavor = success ? action.SuccessFlavor : action.FailFlavor;
            _resultText.color = success ? new Color(0.5f, 1f, 0.7f) : new Color(1f, 0.45f, 0.5f);
            _resultText.text = $"{resultLine}\n\n{flavor}";

            // If bonus gem changed tier, refresh confidence line for clarity.
            RefreshOdds(gemsNow);
            _choicePanel.SetActive(false);
        }

        public void Hide()
        {
            if (_root != null)
                _root.SetActive(false);
        }

        /// <summary>
        /// Used by tests to verify UI strings match calculator output.
        /// </summary>
        public static string BuildChoiceDescriptionForTest(EncounterActionDef action, int gems)
        {
            return FormatChoice(action, gems);
        }
    }
}

using UnityEngine;
using UnityEngine.UI;

namespace SurvivalOfTheFittest.UI
{
    public class TutorialUI : MonoBehaviour
    {
        public static TutorialUI Instance { get; private set; }

        private GameObject _root;

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
            _root = UiFactory.Panel(canvasRoot, "Tutorial", new Color(0.02f, 0.02f, 0.06f, 0.92f),
                Vector2.zero, Vector2.one);

            var card = UiFactory.Box(_root.transform, "Card", new Vector2(0.18f, 0.12f), new Vector2(0.82f, 0.88f),
                Vector2.zero, Vector2.zero, new Color(0.12f, 0.05f, 0.18f, 0.95f));

            UiFactory.Text(card, "Title", "SURVIVAL OF THE FITTEST", 44,
                new Color(1f, 0.4f, 0.9f), TextAnchor.UpperCenter, FontStyle.Bold);
            var title = card.Find("Title").GetComponent<Text>();
            title.rectTransform.anchorMin = new Vector2(0, 0.82f);
            title.rectTransform.anchorMax = new Vector2(1, 1);

            var body =
                "Survive modern dating in a dark maze.\n\n" +
                "• Move: WASD / Arrows  •  Dash: Shift or Space\n" +
                "• Green flags raise Confidence, but they sit on side paths.\n" +
                "• Taking every flag is a long detour.\n" +
                "• The exit is behind three gates: the Ex, the App, then Mixed Signals.\n" +
                "• You cannot walk around them. Grab what you can afford, then commit.\n" +
                "• 90 seconds. The maze stays dark. Keep your hearts.\n\n" +
                "R = Restart   •   Q / Esc = Quit";

            UiFactory.Text(card, "Body", body, 24, new Color(0.9f, 0.9f, 0.95f), TextAnchor.MiddleLeft);
            var bodyT = card.Find("Body").GetComponent<Text>();
            bodyT.rectTransform.anchorMin = new Vector2(0.08f, 0.22f);
            bodyT.rectTransform.anchorMax = new Vector2(0.92f, 0.8f);

            UiFactory.Button(card, "StartBtn", "START DATING (THE MAZE)", new Color(0.9f, 0.2f, 0.55f),
                () => GameManager.Instance?.StartGameFromTutorial(),
                new Vector2(0f, -280f), new Vector2(420f, 64f));
        }

        public void Show()
        {
            if (_root != null)
                _root.SetActive(true);
        }

        public void Hide()
        {
            if (_root != null)
                _root.SetActive(false);
        }
    }
}

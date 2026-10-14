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
                "• Move with WASD or Arrow Keys\n" +
                "• Explore — only a small circle around you is visible\n" +
                "• Collect green flags to raise your Confidence\n" +
                "• Every 3rd green flag activates CLARITY (full maze reveal)\n" +
                "• More green flags = better encounter odds (die stays fair!)\n" +
                "• Face dating obstacles: choose an action, then roll a d6\n" +
                "• Reach the EXIT with hearts left before time runs out\n\n" +
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

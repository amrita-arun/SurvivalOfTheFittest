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

            var card = UiFactory.Box(_root.transform, "Card", new Vector2(0.28f, 0.34f), new Vector2(0.72f, 0.66f),
                Vector2.zero, Vector2.zero, new Color(0.12f, 0.05f, 0.18f, 0.95f));

            UiFactory.Text(card, "Title", "SURVIVAL OF THE FITTEST", 44,
                new Color(1f, 0.4f, 0.9f), TextAnchor.MiddleCenter, FontStyle.Bold);
            var title = card.Find("Title").GetComponent<Text>();
            title.rectTransform.anchorMin = new Vector2(0.05f, 0.42f);
            title.rectTransform.anchorMax = new Vector2(0.95f, 0.95f);

            UiFactory.Button(card, "StartBtn", "START DATING (THE MAZE)", new Color(0.9f, 0.2f, 0.55f),
                () => GameManager.Instance?.StartGameFromTutorial(),
                new Vector2(0f, -70f), new Vector2(420f, 64f));
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

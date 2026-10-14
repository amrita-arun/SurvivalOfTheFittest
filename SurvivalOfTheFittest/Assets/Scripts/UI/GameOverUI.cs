using UnityEngine;
using UnityEngine.UI;

namespace SurvivalOfTheFittest.UI
{
    public class GameOverUI : MonoBehaviour
    {
        public static GameOverUI Instance { get; private set; }

        private GameObject _root;
        private Text _title;
        private Text _details;

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
            _root = UiFactory.Panel(canvasRoot, "GameOver", new Color(0.02f, 0.02f, 0.06f, 0.9f),
                Vector2.zero, Vector2.one);
            _root.SetActive(false);

            var card = UiFactory.Box(_root.transform, "Card", new Vector2(0.22f, 0.25f), new Vector2(0.78f, 0.75f),
                Vector2.zero, Vector2.zero, new Color(0.14f, 0.04f, 0.16f, 0.96f));

            _title = UiFactory.Text(card, "Title", "", 40, new Color(1f, 0.45f, 0.9f), TextAnchor.UpperCenter, FontStyle.Bold);
            _title.rectTransform.anchorMin = new Vector2(0.05f, 0.65f);
            _title.rectTransform.anchorMax = new Vector2(0.95f, 0.95f);

            _details = UiFactory.Text(card, "Details", "", 24, Color.white, TextAnchor.MiddleCenter);
            _details.rectTransform.anchorMin = new Vector2(0.1f, 0.3f);
            _details.rectTransform.anchorMax = new Vector2(0.9f, 0.65f);

            UiFactory.Button(card, "RestartBtn", "RESTART (R)", new Color(0.85f, 0.2f, 0.5f),
                () => GameManager.Instance?.RestartGame(),
                new Vector2(0f, -160f), new Vector2(280f, 56f));
        }

        public void ShowWin(float timeLeft, int gems)
        {
            _root.SetActive(true);
            _title.color = new Color(0.4f, 1f, 0.8f);
            _title.text = "YOU ESCAPED THE DATING POOL!";
            _details.text = $"Time remaining: {Mathf.CeilToInt(timeLeft)}s\nTotal gems: {gems}\n\nPress R to try another round.";
        }

        public void ShowLoss(string reason, int gems)
        {
            _root.SetActive(true);
            _title.color = new Color(1f, 0.4f, 0.5f);
            _title.text = "BACK TO THE APPS…";
            _details.text = $"{reason}\nGems collected: {gems}\n\nPress R to restart.";
        }

        public void Hide()
        {
            if (_root != null)
                _root.SetActive(false);
        }
    }
}

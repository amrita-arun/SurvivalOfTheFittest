using UnityEngine;

namespace SurvivalOfTheFittest
{
    public class ExitTrigger : MonoBehaviour
    {
        private TextMesh _label;

        private void Awake()
        {
            var labelTransform = transform.Find("Label");
            if (labelTransform != null)
                _label = labelTransform.GetComponent<TextMesh>();
        }

        private void Update()
        {
            if (_label == null || GameManager.Instance == null)
                return;

            if (GameManager.Instance.ExitIsUnlocked)
            {
                _label.text = "EXIT";
                _label.color = new Color(0.2f, 1f, 0.75f, 1f);
            }
            else
            {
                _label.text = "LOCKED";
                _label.color = new Color(1f, 0.45f, 0.55f, 1f);
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (!other.CompareTag("Player"))
                return;

            if (GameManager.Instance == null)
                return;

            GameManager.Instance.TryWin();
        }
    }
}

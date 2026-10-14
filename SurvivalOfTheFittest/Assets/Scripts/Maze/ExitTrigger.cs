using UnityEngine;

namespace SurvivalOfTheFittest
{
    public class ExitTrigger : MonoBehaviour
    {
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

using System;
using UnityEngine;

namespace SurvivalOfTheFittest
{
    public class GemCollectible : MonoBehaviour
    {
        [SerializeField] private float pulseSpeed = 3f;
        [SerializeField] private float pulseAmount = 0.12f;

        private Action _onCollected;
        private Vector3 _baseScale;
        private SpriteRenderer _sr;
        private bool _collected;
        private Color _color;

        public void Initialize(Action onCollected, Color color)
        {
            _onCollected = onCollected;
            _color = color;
            _baseScale = transform.localScale;
            _sr = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_collected || GameManager.Instance == null)
                return;

            if (!GameManager.Instance.AllowsGameplayMotion)
                return;

            var t = Time.time * pulseSpeed;
            var s = 1f + Mathf.Sin(t) * pulseAmount;
            transform.localScale = _baseScale * s;

            if (_sr != null)
            {
                var c = _color;
                c.a = 0.75f + 0.25f * (Mathf.Sin(t * 1.4f) * 0.5f + 0.5f);
                _sr.color = c;
            }
        }

        private void OnTriggerEnter2D(Collider2D other)
        {
            if (_collected || !other.CompareTag("Player"))
                return;

            Collect();
        }

        public void Collect()
        {
            if (_collected)
                return;

            _collected = true;
            _onCollected?.Invoke();
            Destroy(gameObject);
        }
    }
}

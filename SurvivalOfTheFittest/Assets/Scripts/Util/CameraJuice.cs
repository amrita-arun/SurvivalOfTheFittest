using UnityEngine;

namespace SurvivalOfTheFittest
{
    /// <summary>
    /// Lightweight camera shake + punch for hit feedback.
    /// </summary>
    public class CameraJuice : MonoBehaviour
    {
        public static CameraJuice Instance { get; private set; }

        [SerializeField] private float recovery = 12f;

        private Vector3 _basePos;
        private Vector2 _shakeOffset;
        private float _shakePower;
        private float _shakeTime;

        private void Awake()
        {
            Instance = this;
            CacheBase();
        }

        private void OnDestroy()
        {
            if (Instance == this)
                Instance = null;
        }

        public void CacheBase()
        {
            if (Camera.main != null)
                _basePos = Camera.main.transform.position;
        }

        public void Shake(float power = 0.18f, float duration = 0.18f)
        {
            _shakePower = Mathf.Max(_shakePower, power);
            _shakeTime = Mathf.Max(_shakeTime, duration);
        }

        public void Punch(Vector2 dir, float power = 0.22f)
        {
            _shakeOffset += dir.normalized * power;
            _shakeTime = Mathf.Max(_shakeTime, 0.15f);
            _shakePower = Mathf.Max(_shakePower, power * 0.5f);
        }

        private void LateUpdate()
        {
            var cam = Camera.main;
            if (cam == null)
                return;

            if (_shakeTime > 0f)
            {
                _shakeTime -= Time.unscaledDeltaTime;
                // Deterministic oscillating shake (no Random).
                var t = Time.unscaledTime * 55f;
                _shakeOffset = new Vector2(Mathf.Sin(t), Mathf.Cos(t * 1.3f)) * _shakePower;
                _shakePower = Mathf.Lerp(_shakePower, 0f, Time.unscaledDeltaTime * recovery);
            }
            else
            {
                _shakeOffset = Vector2.Lerp(_shakeOffset, Vector2.zero, Time.unscaledDeltaTime * recovery);
            }

            cam.transform.position = _basePos + (Vector3)_shakeOffset;
        }
    }
}

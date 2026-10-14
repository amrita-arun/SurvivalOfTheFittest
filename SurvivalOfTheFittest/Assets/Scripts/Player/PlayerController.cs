using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalOfTheFittest
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Top speed in units/sec. Diagonal input is normalized.")]
        [SerializeField] private float moveSpeed = 4.1f;

        [Tooltip("How quickly velocity reaches the target (higher = snappier).")]
        [SerializeField] private float acceleration = 28f;

        [Tooltip("How quickly you stop when no input (higher = tighter brakes).")]
        [SerializeField] private float deceleration = 34f;

        [SerializeField] private float colliderRadius = 0.28f;

        private Rigidbody2D _rb;
        private SpriteRenderer _body;
        private Transform _glow;
        private Vector2 _lastSafePosition;
        private Vector3 _baseScale;
        private bool _canMove = true;
        private float _footstepTimer;

        public Vector2 LastSafePosition => _lastSafePosition;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody2D>();
            _rb.bodyType = RigidbodyType2D.Dynamic;
            _rb.gravityScale = 0f;
            _rb.freezeRotation = true;
            _rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
            _rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            var col = GetComponent<CircleCollider2D>();
            col.radius = colliderRadius;
            col.isTrigger = false;

            _body = GetComponent<SpriteRenderer>();
            _baseScale = transform.localScale;
            gameObject.tag = "Player";
        }

        public void SetVisualRefs(SpriteRenderer body, Transform glow)
        {
            _body = body;
            _glow = glow;
            _baseScale = transform.localScale;
        }

        public void SetCanMove(bool canMove)
        {
            _canMove = canMove;
            if (!_canMove)
                _rb.linearVelocity = Vector2.zero;
        }

        public void Teleport(Vector2 world)
        {
            _rb.position = world;
            transform.position = world;
            _lastSafePosition = world;
            _rb.linearVelocity = Vector2.zero;
        }

        public void MarkSafePosition()
        {
            _lastSafePosition = _rb.position;
        }

        public void PlayHurtPulse()
        {
            if (_body != null)
                StartCoroutine(HurtFlash());
        }

        private System.Collections.IEnumerator HurtFlash()
        {
            if (_body == null)
                yield break;
            var original = _body.color;
            _body.color = Color.white;
            yield return new WaitForSecondsRealtime(0.06f);
            _body.color = new Color(1f, 0.35f, 0.45f);
            yield return new WaitForSecondsRealtime(0.08f);
            _body.color = original;
        }

        private void Update()
        {
            if (!_canMove || GameManager.Instance == null || !GameManager.Instance.AllowsPlayerMovement)
                return;

            AnimateSquash();
        }

        private void FixedUpdate()
        {
            if (!_canMove || GameManager.Instance == null || !GameManager.Instance.AllowsPlayerMovement)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            var input = ReadMoveInput();
            if (input.sqrMagnitude > 1f)
                input.Normalize();

            var target = input * moveSpeed;
            var current = _rb.linearVelocity;
            var rate = input.sqrMagnitude > 0.01f ? acceleration : deceleration;
            _rb.linearVelocity = Vector2.MoveTowards(current, target, rate * Time.fixedDeltaTime);

            if (input.sqrMagnitude > 0.01f)
            {
                MarkMovingSafe();
                TickFootsteps();
            }
        }

        private void MarkMovingSafe()
        {
            _lastSafePosition = _rb.position;
        }

        private void TickFootsteps()
        {
            _footstepTimer -= Time.fixedDeltaTime;
            if (_footstepTimer > 0f)
                return;
            _footstepTimer = 0.22f;
            GameAudio.Instance?.PlayFootstep();
        }

        private void AnimateSquash()
        {
            var speed = _rb != null ? _rb.linearVelocity.magnitude : 0f;
            var moving = speed > 0.4f;
            var sx = moving ? 1.08f : 1f;
            var sy = moving ? 0.9f : 1f;
            var target = Vector3.Scale(_baseScale, new Vector3(sx, sy, 1f));
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * 18f);

            if (_glow != null)
            {
                var pulse = 1.25f + 0.15f * Mathf.Sin(Time.time * 6f);
                _glow.localScale = Vector3.one * pulse;
            }
        }

        private static Vector2 ReadMoveInput()
        {
            var keyboard = Keyboard.current;
            if (keyboard == null)
                return Vector2.zero;

            var x = 0f;
            var y = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
                x -= 1f;
            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
                x += 1f;
            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
                y -= 1f;
            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
                y += 1f;

            return new Vector2(x, y);
        }
    }
}

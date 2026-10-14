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

        [Header("Dash")]
        [SerializeField] private float dashSpeed = 11f;
        [SerializeField] private float dashDuration = 0.12f;
        [SerializeField] private float dashCooldown = 1.15f;

        [SerializeField] private float colliderRadius = 0.28f;

        private Rigidbody2D _rb;
        private SpriteRenderer _body;
        private Transform _glow;
        private Vector2 _lastSafePosition;
        private Vector2 _lastAim = Vector2.right;
        private Vector3 _baseScale;
        private bool _canMove = true;
        private float _dashTimer;
        private float _dashCooldownLeft;
        private float _footstepTimer;
        private Vector2 _dashDir;

        public Vector2 LastSafePosition => _lastSafePosition;
        public float DashCooldownNormalized =>
            dashCooldown <= 0f ? 0f : Mathf.Clamp01(_dashCooldownLeft / dashCooldown);

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
            {
                _rb.linearVelocity = Vector2.zero;
                _dashTimer = 0f;
            }
        }

        public void Teleport(Vector2 world)
        {
            _rb.position = world;
            transform.position = world;
            _lastSafePosition = world;
            _rb.linearVelocity = Vector2.zero;
            _dashTimer = 0f;
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

            if (_dashCooldownLeft > 0f)
                _dashCooldownLeft -= Time.deltaTime;

            TryStartDash();
            AnimateSquash();
        }

        private void FixedUpdate()
        {
            if (!_canMove || GameManager.Instance == null || !GameManager.Instance.AllowsPlayerMovement)
            {
                _rb.linearVelocity = Vector2.zero;
                return;
            }

            if (_dashTimer > 0f)
            {
                _dashTimer -= Time.fixedDeltaTime;
                _rb.linearVelocity = _dashDir * dashSpeed;
                if (_dashTimer <= 0f)
                    _dashCooldownLeft = dashCooldown;
                MarkMovingSafe();
                return;
            }

            var input = ReadMoveInput();
            if (input.sqrMagnitude > 1f)
                input.Normalize();

            if (input.sqrMagnitude > 0.01f)
                _lastAim = input;

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

        private void TryStartDash()
        {
            if (_dashTimer > 0f || _dashCooldownLeft > 0f)
                return;

            var kb = Keyboard.current;
            if (kb == null)
                return;

            var pressed = kb.leftShiftKey.wasPressedThisFrame
                          || kb.rightShiftKey.wasPressedThisFrame
                          || kb.spaceKey.wasPressedThisFrame;
            if (!pressed)
                return;

            _dashDir = _lastAim.sqrMagnitude > 0.01f ? _lastAim.normalized : Vector2.right;
            var input = ReadMoveInput();
            if (input.sqrMagnitude > 0.01f)
                _dashDir = input.normalized;

            _dashTimer = dashDuration;
            GameAudio.Instance?.PlayDash();
            CameraJuice.Instance?.Punch(_dashDir, 0.12f);
            ScreenFlash.Instance?.Flash(new Color(0.4f, 1f, 0.9f), 0.18f, 0.12f);
        }

        private void AnimateSquash()
        {
            var speed = _rb != null ? _rb.linearVelocity.magnitude : 0f;
            var moving = speed > 0.4f;
            var dash = _dashTimer > 0f;
            var sx = dash ? 1.25f : moving ? 1.08f : 1f;
            var sy = dash ? 0.78f : moving ? 0.9f : 1f;
            var target = Vector3.Scale(_baseScale, new Vector3(sx, sy, 1f));
            transform.localScale = Vector3.Lerp(transform.localScale, target, Time.deltaTime * 18f);

            if (_glow != null)
            {
                var pulse = 1.25f + 0.15f * Mathf.Sin(Time.time * 6f);
                if (dash)
                    pulse = 1.6f;
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

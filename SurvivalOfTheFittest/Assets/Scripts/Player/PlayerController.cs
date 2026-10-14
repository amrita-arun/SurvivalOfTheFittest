using UnityEngine;
using UnityEngine.InputSystem;

namespace SurvivalOfTheFittest
{
    [RequireComponent(typeof(Rigidbody2D))]
    [RequireComponent(typeof(CircleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        [Header("Movement")]
        [Tooltip("Units per second. Diagonal input is normalized to avoid speed boost.")]
        [SerializeField] private float moveSpeed = 3.6f;

        [SerializeField] private float colliderRadius = 0.28f;

        private Rigidbody2D _rb;
        private Vector2 _lastSafePosition;
        private bool _canMove = true;

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

            gameObject.tag = "Player";
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

            _rb.linearVelocity = input * moveSpeed;

            if (input.sqrMagnitude > 0.01f)
                _lastSafePosition = _rb.position;
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

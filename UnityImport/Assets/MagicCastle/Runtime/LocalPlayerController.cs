using UnityEngine;

namespace MagicCastle
{
    [RequireComponent(typeof(CharacterController))]
    public sealed class LocalPlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private Rect movementBounds = new Rect(-11f, -8f, 22f, 16f);
        private CharacterController controller;
        private float verticalSpeed;

        private void Awake() => controller = GetComponent<CharacterController>();

        public void Tick(Vector2 input, float deltaTime)
        {
            Vector2 move = Vector2.ClampMagnitude(input, 1f);
            Vector3 position = transform.position;
            Vector3 desired = position + new Vector3(move.x, 0f, move.y) * moveSpeed * deltaTime;
            desired.x = Mathf.Clamp(desired.x, movementBounds.xMin, movementBounds.xMax);
            desired.z = Mathf.Clamp(desired.z, movementBounds.yMin, movementBounds.yMax);

            if (controller.isGrounded && verticalSpeed < 0f)
                verticalSpeed = -2f;
            verticalSpeed = Mathf.Max(verticalSpeed - 20f * deltaTime, -30f);
            controller.Move(new Vector3(desired.x - position.x, verticalSpeed * deltaTime,
                desired.z - position.z));

            if (move.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.LookRotation(new Vector3(move.x, 0f, move.y));
        }
    }
}

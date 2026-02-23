using UnityEngine;
using UnityEngine.InputSystem;

public class FPSMovement : MonoBehaviour
{
        public PlayerInput playerInput;
        private InputAction moveAction;
        public float moveSpeed = 6f;
        public Rigidbody rb;
 
        private Vector2 moveInput;

        void Start()
        {
            playerInput = GetComponent<PlayerInput>();
            moveAction = playerInput.actions.FindAction("Move");
            rb = GetComponent<Rigidbody>();
        }

        void OnEnable()
        {
             moveAction = playerInput.actions.FindAction("Move");
             moveAction.Enable();
        }

         void OnDisable()
         {
             moveAction.Disable();
         }

        void Update()
        {
            MovePlayer();
            moveInput = moveAction.ReadValue<Vector2>();
        }

        void MovePlayer()
        {
            if (moveAction == null)
            {
                Debug.Log("Move action not found");
                return;
            }

             Vector3 moveDirection = transform.forward * moveInput.y + transform.right * moveInput.x;
             Vector3 velocity = moveDirection * moveSpeed;

             rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);
        }
}

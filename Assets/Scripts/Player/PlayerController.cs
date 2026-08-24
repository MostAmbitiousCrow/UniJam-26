using System.Linq;
using EditorAttributes;
using Triggerable;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Player
{
    public class PlayerController : MonoBehaviour
    {
        private static readonly int MoveBlend = Animator.StringToHash("MoveBlend");
        private static readonly int Multiplier = Animator.StringToHash("Multiplier");
        
        [SerializeField] private float moveSpeed = 5f;
        [SerializeField] private float boostMultiplier = 2f;
        private float _currentBoost = 1f;

        private float currentSpeed => moveSpeed * _currentBoost;

        [Header("Detection")]
        [SerializeField] private LayerMask detectionLayer;
        [SerializeField] private float detectionRadius = 2f;
        
        [Header("Components")]
        [SerializeField] private Animator animator;
        [SerializeField] private Rigidbody rb;
        
        private InputAction _moveInput, _interactInput;
        private Vector2 _moveAxis;
        
        [SerializeField, ReadOnly] private Entrance selectedEntrance;

        private void Awake()
        {
            var actionMap = InputSystem.actions;
            _moveInput = actionMap.FindAction("Move");
            _interactInput = actionMap.FindAction("Interact");
        }

        private void Update()
        {
            Movement();
            if (_interactInput.WasCompletedThisFrame()) OnInteract();
        }

        private void FixedUpdate()
        {
            DetectEnterances();
        }

        private void Movement()
        {
            _moveAxis = _moveInput.ReadValue<Vector2>();
            
            rb.linearVelocity = new Vector3(_moveAxis.x * currentSpeed, 0f, _moveAxis.y * currentSpeed);
            
            animator.SetFloat(MoveBlend, _moveAxis.y);
            // animator.SetFloat(Multiplier, _moveAxis.y);
        }

        private void OnInteract()
        {
            if (selectedEntrance)
            {
                selectedEntrance.TriggerEntrance();
            }
        }

        private void DetectEnterances()
        {
            var sweep = Physics.SphereCastAll(transform.position, detectionRadius, 
                Vector3.down, 1f, detectionLayer);

            if (sweep.Length <= 0)
            {
                selectedEntrance = null;
                return;
            }
            var detect = sweep.First();
            if (!detect.transform) return;
            
            Debug.Log($"Detected {detect}");
            selectedEntrance = detect.transform.GetComponent<Entrance>();
            selectedEntrance.DetectedEntrance();
        }

        private void OnSurvivorSaved()
        {
            
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

}

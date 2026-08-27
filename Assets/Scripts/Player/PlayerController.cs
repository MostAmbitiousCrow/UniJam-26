using System.Linq;
using EditorAttributes;
using Managers;
using Triggerable;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

namespace Player
{
    public class PlayerController : Character
    {
        private static readonly int MoveYBlend = Animator.StringToHash("MoveYBlend");
        private static readonly int MoveXBlend = Animator.StringToHash("MoveXBlend");
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
        [SerializeField] private CinemachineCamera cinemachine;
        
        private InputAction _moveInput, _interactInput;
        private Vector2 _moveAxis;
        
        [SerializeField, ReadOnly] private Entrance selectedEntrance;
        
        [Header("Audio")]
        [SerializeField] private AudioSource walkingAudio;

        private void Awake()
        {
            var actionMap = InputSystem.actions;
            _moveInput = actionMap.FindAction("Move");
            _interactInput = actionMap.FindAction("Interact");
        }

        private void Update()
        {
            if (_interactInput.WasCompletedThisFrame()) OnInteract();
            
            // DEV
            if (Keyboard.current.digit1Key.wasPressedThisFrame) Time.timeScale = 1f;
            if (Keyboard.current.digit2Key.wasPressedThisFrame) Time.timeScale = 2f;
            if (Keyboard.current.digit3Key.wasPressedThisFrame) Time.timeScale = 4f;
            if (Keyboard.current.digit4Key.wasPressedThisFrame) Time.timeScale = 8f;
        }

        private void FixedUpdate()
        {
            DetectEnterances();
            if (isBeingGrabbed)
            {
                rb.linearVelocity = Vector3.zero;
                return;
            }
            
            Movement();
        }

        private void Movement()
        {
            _moveAxis = _moveInput.ReadValue<Vector2>();
            
            rb.linearVelocity = new Vector3(_moveAxis.x * currentSpeed, 0f, _moveAxis.y * currentSpeed);
            
            animator.SetFloat(MoveYBlend, _moveAxis.y);
            animator.SetFloat(MoveXBlend, _moveAxis.x);
            
            walkingAudio.mute = !(_moveAxis.magnitude > .1f);
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
            
            selectedEntrance = detect.transform.GetComponent<Entrance>();
            selectedEntrance.DetectedEntrance();
        }

        private void OnSurvivorSaved()
        {
            
        }

        public override void OnGrabbed()
        {
            base.OnGrabbed();
            cinemachine.Follow = null;
        }

        public override void OnStolen()
        {
            base.OnStolen();
            
            GameManager.TriggerGameOver(GameOverType.Died);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, detectionRadius);
        }
    }

}

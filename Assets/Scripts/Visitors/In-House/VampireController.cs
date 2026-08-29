using DG.Tweening;
using EditorAttributes;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters
{
    public class VampireController : MonoBehaviour
    {
        private static readonly int MoveYBlend = Animator.StringToHash("MoveYBlend");
        private static readonly int MoveXBlend = Animator.StringToHash("MoveXBlend");
        private static readonly int Actioning = Animator.StringToHash("Actioning");

        [Header("Movement")]
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float stoppingDistance = 1f;

        [Header("Grab")]
        [SerializeField] private Transform grabPoint;
        [SerializeField] private float grabDuration = .35f;

        [Header("Lift")]
        [SerializeField] private float carryHeight = 10f;
        [SerializeField] private float liftDuration = 2.5f;
        [SerializeField] private float liftDelay = .25f;

        [Header("Effects")]
        [SerializeField] private ParticleSystem transformParticles;
        
        [Header("Art")]
        [SerializeField] private Animator animator;
        [SerializeField] private SpriteRenderer renderer;

        private Character _target;
        private bool _isStealing;

        private Tween _liftTween;
        private Tween _grabTween;

        private bool HasTarget => _target;

        private void Awake()
        {
            agent.stoppingDistance = stoppingDistance;
            
            animator.transform.parent = null;
            animator.transform.rotation = Quaternion.identity;
        }

        private void FixedUpdate()
        {
            // Keep the artwork on the agent
            animator.transform.position = agent.transform.position;
            
            if (!HasTarget || _isStealing)
                return;

            // Keep following the target while they're moving.
            agent.SetDestination(_target.transform.position);
            
            animator.SetFloat(MoveYBlend, agent.velocity.y);
            animator.SetFloat(MoveXBlend, agent.velocity.x);

            if (HasReachedTarget())
            {
                StealTarget();
            }
        }

        public void OnSpawned(Character newTarget)
        {
            if (!newTarget)
            {
                Debug.LogWarning($"{name} spawned without a target.");
                ReturnToPool();
                return;
            }

            _target = newTarget;
            _isStealing = false;

            agent.isStopped = false;
            agent.stoppingDistance = stoppingDistance;

            agent.SetDestination(_target.transform.position);
        }

        private bool HasReachedTarget()
        {
            if (!agent.isOnNavMesh)
                return false;

            return !agent.pathPending &&
                   agent.remainingDistance <= agent.stoppingDistance;
        }

        private void StealTarget()
        {
            if (!HasTarget || _isStealing) return;

            _isStealing = true;

            agent.isStopped = true;
            agent.ResetPath();

            transformParticles?.Play();

            _target.OnGrabbed();
            _target.transform.rotation = Quaternion.identity;
            
            animator.SetBool(Actioning, true);

            // Move the target into the vampire's hands
            _grabTween = _target.transform.DOMove(grabPoint.position, grabDuration).SetEase(Ease.InOutSine)
                .OnComplete(BeginLift);
        }

        private void BeginLift()
        {
            if (!HasTarget)
            {
                ReturnToPool();
                return;
            }

            // Parent the victim to the vampire so they rise together.
            _target.transform.SetParent(grabPoint, true);

            // Snap them precisely into the grab position.
            _target.transform.localPosition = Vector3.zero;
            _target.transform.localRotation = Quaternion.identity;

            var targetY = transform.position.y + carryHeight;

            _liftTween = DOTween.Sequence().AppendInterval(liftDelay)
                .Append(transform.DOMoveY(targetY, liftDuration).SetEase(Ease.InCirc))
                .OnComplete(FinishSteal);
        }

        private void FinishSteal()
        {
            if (_target)
            {
                // Remove the target from the house.
                VisitorManager.Instance.RemoveCapturedCharacter(_target);
                _target.OnStolen();

                _target.transform.SetParent(null);
            }
            
            _target = null;

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _grabTween?.Kill();
            _liftTween?.Kill();

            // If you're using an object pool, return it here.
            // Otherwise:
            Destroy(animator.gameObject);
            Destroy(gameObject);
        }

        private void OnDestroy()
        {
            _grabTween?.Kill();
            _liftTween?.Kill();
        }

#if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying || !_target)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, _target.transform.position);

            if (grabPoint)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(grabPoint.position, .15f);
            }
        }
#endif
    }
}
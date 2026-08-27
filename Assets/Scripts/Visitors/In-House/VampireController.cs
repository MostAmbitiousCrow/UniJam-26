using DG.Tweening;
using EditorAttributes;
using Managers;
using UnityEngine;
using UnityEngine.AI;

namespace Characters
{
    public class VampireController : MonoBehaviour
    {
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

        [Header("Debug")]
        [SerializeField] private Character target;
        [SerializeField, ReadOnly] private bool isStealing;

        private Tween _liftTween;
        private Tween _grabTween;

        private bool HasTarget => target;

        private void Awake()
        {
            agent.stoppingDistance = stoppingDistance;
        }

        private void FixedUpdate()
        {
            if (!HasTarget || isStealing)
                return;

            // Keep following the target while they're moving.
            agent.SetDestination(target.transform.position);

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

            target = newTarget;
            isStealing = false;

            agent.isStopped = false;
            agent.stoppingDistance = stoppingDistance;

            agent.SetDestination(target.transform.position);
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
            if (!HasTarget || isStealing) return;

            isStealing = true;

            agent.isStopped = true;
            agent.ResetPath();

            transformParticles?.Play();

            target.OnGrabbed();

            // Move the target into the vampire's hands
            _grabTween = target.transform.DOMove(grabPoint.position, grabDuration).SetEase(Ease.InOutSine)
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
            target.transform.SetParent(grabPoint, true);

            // Snap them precisely into the grab position.
            target.transform.localPosition = Vector3.zero;
            target.transform.localRotation = Quaternion.identity;

            var targetY = transform.position.y + carryHeight;

            _liftTween = DOTween.Sequence().AppendInterval(liftDelay)
                .Append(transform.DOMoveY(targetY, liftDuration).SetEase(Ease.InCirc))
                .OnComplete(FinishSteal);
        }

        private void FinishSteal()
        {
            if (target)
            {
                // Remove the target from the house.
                VisitorManager.Instance.RemoveCapturedCharacter(target);
                target.OnStolen();

                target.transform.SetParent(null);
            }
            
            target = null;

            ReturnToPool();
        }

        private void ReturnToPool()
        {
            _grabTween?.Kill();
            _liftTween?.Kill();

            // If you're using an object pool, return it here.
            // Otherwise:
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
            if (!Application.isPlaying || target == null)
                return;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(
                transform.position,
                target.transform.position);

            if (grabPoint != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(
                    grabPoint.position,
                    .15f);
            }
        }
#endif
    }
}
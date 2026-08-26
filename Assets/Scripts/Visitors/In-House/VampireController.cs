using DG.Tweening;
using EditorAttributes;
using UnityEngine;
using UnityEngine.AI;

namespace Characters
{
    public class VampireController : MonoBehaviour
    {
        [Header("Parameters")]
        [SerializeField, ReadOnly] private Character targetSurvivor;
        [SerializeField] private NavMeshAgent agent;
        [SerializeField] private float carryHeight = 10f;
        
        [Header("Effects")]
        [SerializeField] private ParticleSystem transformParticles;
        
        private float distance => Vector3.Distance(targetSurvivor.transform.position, transform.position);
        private bool hasReachedSurvivor => distance < .25f;
        private bool _isStealing;

        private void Start()
        {
            
        }

        private void FixedUpdate()
        {
            if (hasReachedSurvivor && !_isStealing)
            {
                StealSurvivor();
            }
            // else if (targetSurvivor is Survivor)
            // {
            //     agent.SetDestination(targetSurvivor.transform.position);
            // }
        }

        public void OnSpawned(Character target)
        {
            targetSurvivor = target;
            agent.SetDestination(targetSurvivor.transform.position);
        }

        private void ReturnToPool()
        {
            
        }

        private void StealSurvivor()
        {
            _isStealing = true;
            
            transformParticles.Play();
            
            var tween = transform.DOMoveY(carryHeight, 3f);
            tween.SetEase(Ease.InOutBack);
            tween.SetDelay(.5f);
            tween.OnComplete(() => ReturnToPool());
        }

        #if UNITY_EDITOR
        private void OnDrawGizmos()
        {
            if (!Application.isPlaying) return;
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, targetSurvivor.transform.position);
        }
        #endif
    }
}

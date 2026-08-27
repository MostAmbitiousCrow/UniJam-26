using System;
using System.Collections;
using DG.Tweening;
using EditorAttributes;
using Managers;
using UnityEngine;
using Visitors;

namespace Triggerable
{
    public class Entrance : MonoBehaviour
    {
        [Header("Data")]
        public EntranceType entranceType;
        [SerializeField, ReadOnly] private VisitorData _visitorData;
        public VisitorData visitorData => _visitorData;
        public bool hasVisitor => _visitorData;
        
        [SerializeField] private GameObject alert;
        [SerializeField] private float alertIntensity = 1f;
        
        [Space]
        [SerializeField] private float patienceTime = 20f;
        [SerializeField] private float patienceAlert = 5f;
        private Coroutine _patienceRoutine;
        [Space]
        [SerializeField] public Transform entrancePoint;
        public Transform EntrancePoint => entrancePoint;
        
        [Header("Audio")]
        [SerializeField] private AudioSource triggerSound;
        [SerializeField] private AudioSource alertSound;
        
        private VisitorManager _visitorManager;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Awake()
        {
            _visitorManager = FindAnyObjectByType<VisitorManager>();
        }

        private void Start()
        {
            alert.SetActive(false);
        }

        #region Visitor Spawning

        public void DeliverVisitor(VisitorData data)
        {
            if (!data)
            {
                Debug.LogWarning($"Missing Visitor at {this}");
                return;
            }
            
            alert.SetActive(true);
            _visitorData = data;
            
             _patienceRoutine = StartCoroutine(PatienceTimerRoutine());
            
            Debug.Log($"Delivered a {data.visitorType} at {gameObject}");
        }

        /// <summary>
        /// Remove this entrances visitor and return it to the Visitor Manager
        /// </summary>
        public void RemoveVisitor()
        {
            _visitorData = null;
            _visitorManager.ReturnEntrance(this);
            
            alert.SetActive(false);
            
            if (_patienceRoutine != null) StopCoroutine(_patienceRoutine);
        }

        private IEnumerator PatienceTimerRoutine()
        {
            var time = 0f;
            var alerted = false;
            
            alertSound.Play();

            while (time < patienceTime)
            {
                if (time > patienceTime - patienceAlert && !alerted)
                {
                    var tween =
                        alert.transform.DOPunchScale(Vector3.one * alertIntensity, .35f);
                    tween.SetEase(Ease.InOutCubic);
                    tween.SetLoops(-1, LoopType.Yoyo);
                    alerted = true;
                    
                    alertSound.Play();
                }
                
                yield return time += Time.deltaTime;
            }

            alert.transform.DOKill(true);
            
            OnVisitorPatienceEnded();
        }

        private void OnVisitorPatienceEnded()
        {
            // The Survivor waited too long as was killed
            if (visitorData.visitorType == VisitorType.Survivor) 
                VisitorManager.Instance.DecideVisitorChoice(visitorData, RejectionChoice.No);
            
            // The Vampire/Imposter was able to enter the house after not being rejected
            else VisitorManager.Instance.DecideVisitorChoice(visitorData, RejectionChoice.Yes);
            
            RemoveVisitor();
        }

        #endregion

        #region Detection

        public void DetectedEntrance()
        {
            
        }

        public void TriggerEntrance()
        {
            if (!hasVisitor) return;
            
            triggerSound.Play();
            VisitorScreen.instance.OpenScreen(this);
            
            Debug.Log($"{gameObject} Entrance Triggered");
        }
        #endregion

        #if UNITY_EDITOR
        private void OnValidate()
        {
            name = $"Entrance Point ({entranceType})";
        }

        private void OnDrawGizmos()
        {
            if (entrancePoint)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(entrancePoint.position, .25f);
            }
        }
        #endif
    }
}

public enum EntranceType { Door, Window }
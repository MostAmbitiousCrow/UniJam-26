using System;
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
        [SerializeField] private float alertRate = 2.6f;
        
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
            
            Debug.Log($"Delivered a {data.visitorType} at {gameObject}");
        }

        /// <summary>
        /// Remove this entrances visitor and return it to the Visitor Manager
        /// </summary>
        /// <param name="choice"></param>
        public void RemoveVisitor(RejectionChoice choice)
        {
            _visitorData = null;
            _visitorManager.ReturnEntrance(this);
            
            alert.SetActive(false);
        }

        #endregion

        #region Detection

        public void DetectedEntrance()
        {
            
        }

        public void TriggerEntrance()
        {
            if (!hasVisitor) return;
            
            triggerSound?.Play();
            VisitorScreen.instance.OpenScreen(this);
            
            Debug.Log($"{gameObject} Entrance Triggered");
        }
        #endregion

        private void OnValidate()
        {
            name = $"Entrance Point ({entranceType})";
        }
    }
}

public enum EntranceType { Door, Window }
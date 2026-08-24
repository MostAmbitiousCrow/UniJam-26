using EditorAttributes;
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

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        private void Start()
        {
            alert.SetActive(false);
        }

        #region Visitor Spawning

        public void DeliverVisitor(VisitorData data)
        {
            alert.SetActive(true);
            _visitorData = data;
        }

        public void RemoveVisitor()
        {
            _visitorData = null;
        }

        #endregion

        #region Detection

        public void DetectedEntrance()
        {
            
        }

        public void TriggerEntrance()
        {
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
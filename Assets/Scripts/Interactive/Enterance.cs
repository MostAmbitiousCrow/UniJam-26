using System;
using UnityEngine;

namespace Triggerable
{
    public class Entrance : MonoBehaviour
    {
        public EntranceType entranceType;

        [Header("Audio")]
        [SerializeField] private AudioSource triggerSound;

        private void OnEnable()
        {
            
        }

        private void OnDisable()
        {
            
        }

        public void DetectedEnterance()
        {
            
        }

        public void TriggerEnterance()
        {
            triggerSound?.Play();
            Debug.Log($"{gameObject} Entrance Triggered");
        }

        private void OnValidate()
        {
            name = $"Entrance Point ({entranceType})";
        }
    }
}

public enum EntranceType { Door, Window }
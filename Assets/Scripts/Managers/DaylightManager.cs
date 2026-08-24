using System;
using EditorAttributes;
using UnityEngine;

namespace Managers
{
    public class DaylightManager : MonoBehaviour
    {
        [Header("Parameters")]
        [Tooltip("The target game time in minutes")]
        [SerializeField] private float targetGameTime = 2.5f;
        [SerializeField, ReadOnly] private float currentGameTime;
        public float CurrentGameTime => currentGameTime;
        public bool doCountDown;

        public float NormalisedTime => Mathf.Clamp01(currentGameTime / (targetGameTime * 60f));
        public bool IsSunRise => currentGameTime <= 0f;

        private void OnEnable()
        {
            GameManager.OnGameStarted += StartTimer;
        }       
        private void OnDisable()
        {
            GameManager.OnGameStarted -= StartTimer;
        }

        private void FixedUpdate()
        {
            if (doCountDown) CountTimer();
        }

        private void StartTimer()
        {
            doCountDown = true;
            currentGameTime = targetGameTime * 60f;
        }

        private void CountTimer()
        {
            currentGameTime -= Time.fixedDeltaTime;

            if (currentGameTime <= 0f)
            {
                OnSunRisen();
            }
        }

        private void OnSunRisen()
        {
            currentGameTime = 0f;
            doCountDown = false;
            
            GameManager.EndGame();
        }
    }
}

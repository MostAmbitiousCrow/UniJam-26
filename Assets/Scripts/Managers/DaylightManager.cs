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
        public bool IsSunRise => currentGameTime >= targetGameTime * 60f;
        
        [Header("Visuals")]
        [SerializeField] private Light globalLight;
        [SerializeField] private AnimationCurve globalLightIntensity, globalLightRotation;

        private void Start()
        {
            UpdateGlobalLight();
        }

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
            if (doCountDown)
            {
                CountTimer();
                UpdateGlobalLight();
            }
        }

        private void StartTimer()
        {
            doCountDown = true;
            currentGameTime = 0f;
        }

        private void CountTimer()
        {
            currentGameTime += Time.fixedDeltaTime;

            if (IsSunRise)
            {
                OnSunRisen();
            }
        }

        private void UpdateGlobalLight()
        {
            globalLight.intensity = globalLightIntensity.Evaluate(NormalisedTime);
            globalLight.transform.rotation = Quaternion.Euler(globalLightRotation.Evaluate(NormalisedTime), 0f, 0f);
        }

        private void OnSunRisen()
        {
            currentGameTime = 0f;
            doCountDown = false;
            
            GameManager.TriggerGameOver(GameOverType.NightSurvived);
        }
    }
}

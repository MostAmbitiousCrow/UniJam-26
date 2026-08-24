using Managers;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI Elements")]
        [SerializeField] private Image timerImage;
        
        [Header("Dependencies")]
        private DaylightManager _daylightManager;

        private void Start()
        {
            _daylightManager = FindAnyObjectByType<DaylightManager>();
            if (!_daylightManager) Debug.LogError("Unable to find DaylightManager");
        }

        private void FixedUpdate()
        {
            UpdateTimerVisuals();
        }

        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = _daylightManager.NormalisedTime;
        }
    }
}

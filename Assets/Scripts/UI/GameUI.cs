using System;
using Managers;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class GameUI : MonoBehaviour
    {
        [Header("UI Elements")] [SerializeField]
        private Image timerImage;
        [Space]
        [SerializeField] private TextMeshProUGUI guardText;

        [Header("Dependencies")] private DaylightManager _daylightManager;

        private void Start()
        {
            _daylightManager = FindAnyObjectByType<DaylightManager>();
            if (!_daylightManager) Debug.LogError("Unable to find DaylightManager");
            
            UpdateGuardVisuals();
            UpdateTimerVisuals();
        }

        private void OnEnable()
        {
            GameManager.OnSurvivorsUpdated += UpdateGuardVisuals;
        }

        private void OnDisable()
        {
            GameManager.OnSurvivorsUpdated -= UpdateGuardVisuals;
        }

        private void FixedUpdate()
        {
            UpdateTimerVisuals();
        }

        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = _daylightManager.NormalisedTime;
        }

        private void UpdateGuardVisuals()
        {
            guardText.SetText($"{GameManager.Instance.currentGuards}/{GameManager.Instance.maxGuards}");
        }
    }
}

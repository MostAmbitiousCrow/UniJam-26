using System;
using System.Runtime.CompilerServices;
using EditorAttributes;
using Managers;
using Triggerable;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace Visitors
{
    public class VisitorScreen : MonoBehaviour
    {
        [Header("Interaction")]
        [SerializeField] private CanvasGroup canvasGroup;
        [SerializeField] private Button yesButton, noButton;
        public bool isOpen { get; private set; } = false;

        [Header("Visuals")]
        [SerializeField] private Image background;
        [SerializeField] private Sprite doorBackground;
        [SerializeField] private Sprite windowBackground;

        [Header("Audio")]
        [SerializeField] private AudioSource yesAudio;
        [SerializeField] private AudioSource noAudio;
        
        public static Action OnEntranceOpened, OnEntranceClosed;
        
        [SerializeField, ReadOnly] private Entrance _currentEntrance;
        
        public static VisitorScreen instance { get; private set; }
        
        private InputAction _yesInput, _noInput;

        private void Awake()
        {
            yesButton.onClick.AddListener(OnYes);
            noButton.onClick.AddListener(OnNo);

            instance = this;

            var actions = InputSystem.actions;
            _yesInput = actions.FindAction("Action1");
            _noInput = actions.FindAction("Action2");
        }

        private void Start()
        {
            CloseScreen();
        }

        private void Update()
        {
            if (!isOpen) return;
            
            if (_yesInput.WasPerformedThisFrame()) OnYes();
            if (_noInput.WasPerformedThisFrame()) OnNo();
        }

        #region Choices

        private void OnYes()
        {
            if (_currentEntrance == null) return;
            Debug.Log($"{_currentEntrance.visitorData.visitorType} was Accepted");
            
            yesAudio.clip = _currentEntrance.visitorData.acceptedSound;
            yesAudio?.Play();
            
            VisitorManager.DecideVisitorChoice(_currentEntrance.visitorData.visitorType, RejectionChoice.Yes);
            _currentEntrance.RemoveVisitor(RejectionChoice.Yes);

            CloseScreen();
        }

        private void OnNo()
        {
            if (_currentEntrance == null) return;
             Debug.Log($"{_currentEntrance.visitorData.visitorType} was Rejected");
            
            yesAudio.clip = _currentEntrance.visitorData.rejectionSound;
            
            VisitorManager.DecideVisitorChoice(_currentEntrance.visitorData.visitorType, RejectionChoice.No);
            _currentEntrance.RemoveVisitor(RejectionChoice.No);

            noAudio?.Play();
            CloseScreen();
        }

        public void OpenScreen(Entrance entrance)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            canvasGroup.interactable = true;
            
            _currentEntrance = entrance;
            isOpen = true;
            
            // Update Background
            background.sprite = entrance.entranceType == EntranceType.Door? doorBackground : windowBackground;
        }

        private void CloseScreen()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            canvasGroup.interactable = false;
            
            isOpen = false;
        }
        #endregion
        
    }
}

public enum RejectionChoice { Yes, No }
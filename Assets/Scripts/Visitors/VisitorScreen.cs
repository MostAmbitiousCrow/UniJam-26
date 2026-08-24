using System;
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

        [Header("Audio")]
        [SerializeField] private AudioSource yesAudio;
        [SerializeField] private AudioSource noAudio;
        
        public static Action OnEntranceOpened, OnEntranceClosed;
        
        private Entrance _currentEntrance;
        
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

        private void OnYes()
        {
            yesAudio?.Play();
            CloseScreen();
        }

        private void OnNo()
        {
            noAudio?.Play();
            CloseScreen();
        }

        public void OpenScreen(Entrance entrance)
        {
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
            
            _currentEntrance = entrance;
            isOpen = true;
        }

        private void CloseScreen()
        {
            canvasGroup.alpha = 0;
            canvasGroup.blocksRaycasts = false;
            
            isOpen = false;
        }
    }
}

using DG.Tweening;
using Managers;
using TMPro;
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
        [SerializeField] private TextMeshProUGUI surplusSurvivorsText;

        [Header("Dependencies")]
        private DaylightManager _daylightManager;

        private bool _canEscape;

        private void Start()
        {
            _daylightManager = FindAnyObjectByType<DaylightManager>();
            if (!_daylightManager) Debug.LogError("Unable to find DaylightManager");
            
            HideResults();
            
            UpdateSurvivorVisuals();
            UpdateTimerVisuals();
            
            retryButton.onClick.AddListener(RetryGame);
            returnButton.onClick.AddListener(ReturnToMenu);
        }

        private void OnEnable()
        {
            GameManager.OnSurvivorsUpdated += UpdateSurvivorVisuals;
            GameManager.OnGameOver += DisplayResults;

            GameManager.OnPauseUpdated += TogglePauseMenu;
        }

        private void OnDisable()
        {
            GameManager.OnSurvivorsUpdated -= UpdateSurvivorVisuals;
            GameManager.OnGameOver -= DisplayResults;
            
            GameManager.OnPauseUpdated -= TogglePauseMenu;
        }

        private void FixedUpdate()
        {
            UpdateTimerVisuals();
        }

        #region Visuals
        

        private void UpdateTimerVisuals()
        {
            timerImage.fillAmount = _daylightManager.NormalisedTime;
        }

        private void UpdateSurvivorVisuals()
        {
            guardText.SetText($"{GameManager.Instance.currentGuards}/{GameManager.Instance.maxGuards}");
            surplusSurvivorsText.SetText($"{GameManager.Instance.survivorsInInventory}");
        }
        #endregion

        #region Game Results

        [Header("Game Results")]
        [SerializeField] private CanvasGroup resultsGroup;
        [SerializeField] private TextMeshProUGUI endingText;
        [SerializeField] private TextMeshProUGUI[] resultsText =  new TextMeshProUGUI[5];
        [Space]
        [SerializeField, TextArea] private string[] gameOverText;
        [Space]
        [SerializeField] private Button returnButton;
        [SerializeField] private Button retryButton;

        private void HideResults()
        {
            resultsGroup.alpha = 0f;
            resultsGroup.blocksRaycasts = false;
            resultsGroup.interactable = false;
        }

        private void ShowButtonOptions()
        {
            _canEscape = true;
            
            
        }

        private void DisplayResults(GameOverType gameOverType)
        {
            resultsGroup.alpha = 1f;
            resultsGroup.blocksRaycasts = true;
            resultsGroup.interactable = true;
            
            var titleText = gameOverType == GameOverType.NightSurvived ? gameOverText[0] : gameOverText[1];
            endingText.SetText(titleText);
            
            resultsText[0].SetText($"Survivors Accepted: {GameManager.CurrentGameStats.SurvivorsAccepted}");
            resultsText[1].SetText($"Survivors Rejected: {GameManager.CurrentGameStats.SurvivorsRejected}");
            resultsText[2].SetText($"Vampires Accepted: {GameManager.CurrentGameStats.VampiresAccepted}");
            resultsText[3].SetText($"Vampires Rejected: {GameManager.CurrentGameStats.VampiresRejected}");
            resultsText[4].SetText($"Imposters Accepted: {GameManager.CurrentGameStats.ImpostersAccepted}");
            resultsText[5].SetText($"Imposters Rejected: {GameManager.CurrentGameStats.ImpostersRejected}");
            resultsText[6].SetText($"Time Survived {GameManager.CurrentGameStats.timeSurvived}");
            
            // Do Slide Animation

            /*float delay = .15f;
            foreach (var result in resultsText)
            {
                var parent = result.rectTransform.parent.GetComponent<RectTransform>();
                parent.localPosition += new Vector3(1000f, 0f, 0f);
                
                var tween = parent.DOLocalMoveX
                    (parent.localPosition.x - 1000f, 1.25f);
                tween.SetEase(Ease.InExpo);
                tween.SetDelay(delay);
                
                delay += .15f;
            }*/

        }

        private void ReturnToMenu()
        {
            LoadingScreen.Instance.LoadScene("MainMenu");
        }
        
        private void RetryGame()
        {
            LoadingScreen.Instance.LoadScene("MainGame");
        }

        #endregion

        #region Pause Menu

        [Header("Pause Menu")]
        [SerializeField] private GameObject pauseMenu;
        
        private void TogglePauseMenu(bool state)
        {
            pauseMenu.SetActive(state);
        }

        #endregion
    }
}

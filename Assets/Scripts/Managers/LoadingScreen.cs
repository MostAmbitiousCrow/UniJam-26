using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;

namespace Managers
{
    public class LoadingScreen : MonoBehaviour
    {
        public static LoadingScreen Instance { get; private set; }

        [Header("Fade")]
        [SerializeField] private CanvasGroup fadeCanvasGroup;
        [SerializeField] private float fadeDuration = 0.5f;
        public bool IsLoading { get; private set; }

        private void Awake()
        {
            if (Instance && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        private void Start()
        {
            // Fade in when the game starts.
            fadeCanvasGroup.alpha = 1f;
            fadeCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad).SetUpdate(true);
        }

        public void LoadScene(string sceneName)
        {
            if (IsLoading) return;
            IsLoading = true;
            var scene = SceneManager.GetSceneByName(sceneName);
            StartCoroutine(LoadSceneRoutine(scene));
        }

        public void LoadScene(int sceneIndex)
        {
            if (IsLoading) return;
            IsLoading = true;
            var scene = SceneManager.GetSceneByBuildIndex(sceneIndex);
            StartCoroutine(LoadSceneRoutine(scene));
        }

        private void OnSceneLoaded()
        {
            IsLoading = false;
            Debug.Log("Loaded scene: " + SceneManager.GetActiveScene().name);
        }

        private IEnumerator LoadSceneRoutine(Scene scene)
        {
            Debug.Log("Loading scene: " + scene.name);
            yield return FadeOut();

            var operation = SceneManager.LoadSceneAsync(scene.buildIndex);

            while (operation is not { isDone: true })
                yield return null;
            yield return new WaitForEndOfFrame();

            yield return FadeIn();
            
            OnSceneLoaded();
        }
        
        private IEnumerator FadeOut()
        {
            fadeCanvasGroup.DOKill();
            
            yield return 
                fadeCanvasGroup.DOFade(1f, fadeDuration).SetEase(Ease.InOutQuad)
                    .SetUpdate(true).WaitForCompletion();
        }

        private IEnumerator FadeIn()
        {
            fadeCanvasGroup.DOKill();

            yield return 
                fadeCanvasGroup.DOFade(0f, fadeDuration).SetEase(Ease.InOutQuad)
                    .SetUpdate(true).WaitForCompletion();
        }
    }
}
using DG.Tweening;
using Managers;
using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [Header("")]
    [SerializeField] private RectTransform buttonsContainer;

    [Space]
    [SerializeField] private GameObject quitButton;

    private void Start()
    {
        if (Application.platform == RuntimePlatform.WebGLPlayer) quitButton.SetActive(false);
        
        GameManager.ResumeGame();

        buttonsContainer.localScale = new Vector3(1f,0f,1f);

        var tween = buttonsContainer.DOScaleY(1f, 1.67f);
        tween.SetEase(Ease.OutExpo);
        tween.SetDelay(1f);
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
    }    
    
    public void CloseMenu(GameObject menu)
    {
        menu.SetActive(false);
    }

    public void OpenLink(string link)
    {
        Application.OpenURL(link);
    }
    
    public void StartGame()
    {
        LoadingScreen.Instance.LoadScene("Story");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}

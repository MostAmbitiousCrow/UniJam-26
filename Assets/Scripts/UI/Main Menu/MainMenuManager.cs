using DG.Tweening;
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

        buttonsContainer.localScale = new Vector3(1f,0f,1f);

        var tween = buttonsContainer.DOScaleY(1f, 1.67f);
        tween.SetEase(Ease.OutExpo);
        tween.SetDelay(1f);
    }
}

using Managers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StoryUI : MonoBehaviour
{
    [SerializeField] private Button selectButton;

    public void TriggerExit()
    {
        LoadingScreen.Instance.LoadScene("MainGame");
        
        EventSystem.current.SetSelectedGameObject(selectButton.gameObject);
    }
}

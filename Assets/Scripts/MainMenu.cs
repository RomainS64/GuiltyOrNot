using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button mainMenuButton;
    void Start()
    {
        startButton.onClick.AddListener(StartClickListener);
        settingsButton.onClick.AddListener(SettingsClickListener);
        quitButton.onClick.AddListener(QuitClickListener);
        mainMenuButton.onClick.AddListener(MainMenuClickListener);
        
    }

    private void MainMenuClickListener()
    {
        SceneCameraManager.Instance.LoadScene(0);
    }

    private void QuitClickListener()
    {
        Application.Quit();
    }

    private void SettingsClickListener()
    {
        SceneCameraManager.Instance.LoadScene(1);
    }

    private void StartClickListener()
    {
        ScenarioFlow.Instance.StartGenerating();
        PlacePin.Instance.SetPlacePinMode(false);
        CameraHandler.Instance.ToggleCameraMovement(true);
        SceneCameraManager.Instance.LoadScene(2);
        
    }
}

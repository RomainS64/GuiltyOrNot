using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private MenuSettings menuSettings; 
    [SerializeField] private Button startButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private Button creditsButton;
    [SerializeField] private Button mainMenuButton1;
    [SerializeField] private Button mainMenuButton2;
    
    [SerializeField] private GameObject settingsPart;
    [SerializeField] private GameObject creditPart;
    [SerializeField] private GameObject mainPart;
    
    [SerializeField] private GameObject loadingPart;
    [SerializeField] private float transitionDuration;
    private IEnumerator transitionCoroutine = null;
    void Start()
    {
        menuSettings.OnApiAccessChanged += ChangeStartState;
        startButton.onClick.AddListener(StartClickListener);
        settingsButton.onClick.AddListener(SettingsClickListener);
        quitButton.onClick.AddListener(QuitClickListener);
        creditsButton.onClick.AddListener(CreditsClickListener);
        mainMenuButton1.onClick.AddListener(MainMenuClickListener);
        mainMenuButton2.onClick.AddListener(MainMenuClickListener);
        
        AudioManager.instance.audioEvents["Game Music"].Play();
        AudioManager.instance.SetParameter("music_state", 0);
    }

    private void ChangeStartState(bool _active)
    {
        startButton.interactable = _active;

    }

    private void CreditsClickListener()
    {
        AudioManager.instance.audioEvents["Start Game Button"].Play();
        mainPart.SetActive(false);
        creditPart.SetActive(true);
        settingsPart.SetActive(false);
    }

    private void MainMenuClickListener()
    {
        AudioManager.instance.audioEvents["Start Game Button"].Play();
        mainPart.SetActive(true);
        creditPart.SetActive(false);
        settingsPart.SetActive(false);
    }

    private void QuitClickListener()
    {
        AudioManager.instance.audioEvents["Start Game Button"].Play();
        Application.Quit();
    }

    private void SettingsClickListener()
    {
        AudioManager.instance.audioEvents["Start Game Button"].Play();
        mainPart.SetActive(false);
        creditPart.SetActive(false);
        settingsPart.SetActive(true);
    }

    private void StartClickListener()
    {
        AudioManager.instance.audioEvents["Start Game Button"].Play();
        loadingPart.SetActive(true);
        if(transitionCoroutine != null)StopCoroutine(transitionCoroutine);
        transitionCoroutine = ClickDelay();
        StartCoroutine(transitionCoroutine);
    }

    private IEnumerator ClickDelay()
    {
        yield return new WaitForSeconds(transitionDuration);
        SceneManager.LoadScene(sceneBuildIndex: 1);
    }
}


using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuSettings : MonoBehaviour
{
    public static bool isAPIAccessible = false;
    public Action<bool> OnApiAccessChanged;
    
    [SerializeField] private GptGeneration generation;
    [SerializeField] private GameObject apiAccessOkText;
    [SerializeField]private GameObject apiAccessLoadingText;
    [SerializeField]private GameObject apiAccessNOkText;
    [SerializeField]private GameObject apiAccessNOkMain;
    [SerializeField]private TMP_Dropdown languageDropdown;
    [SerializeField]private TMP_InputField organization;
    [SerializeField]private TMP_InputField apiKey;
    [SerializeField] private Button validateAPIAccess;

    private void Start()
    {
        TryAPIAccess();
        
        languageDropdown.value = PlayerPrefs.GetInt("LanguageInt",0);
        organization.text = PlayerPrefs.GetString("Organization",string.Empty);
        apiKey.text = PlayerPrefs.GetString("PrivateAPIKey",string.Empty);
        organization.onValueChanged.AddListener(OnOrganizationChanged);
        apiKey.onValueChanged.AddListener(OnApiKeyChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        validateAPIAccess.onClick.AddListener(TryAPIAccess);
        generation.OnGPTResponseReceived += APIAccessible;
        generation.OnGPTError += APINotAccessible;
    }

    private void APINotAccessible(string _response)
    {
        isAPIAccessible = false;
        OnApiAccessChanged?.Invoke(false);
        apiAccessLoadingText.SetActive(false);
        apiAccessNOkText.SetActive(true);
        apiAccessNOkMain.SetActive(true);
    }

    private void APIAccessible(string _response)
    {
        isAPIAccessible = true;
        OnApiAccessChanged?.Invoke(true);
        apiAccessLoadingText.SetActive(false);
        apiAccessOkText.SetActive(true);
        apiAccessNOkText.SetActive(false);
        apiAccessNOkMain.SetActive(false);
    }

    public void TryAPIAccess()
    {
        generation.SetUpConversation();
        generation.SendMessage("Can you recieve this message ?");
        apiAccessLoadingText.SetActive(true);
        apiAccessOkText.SetActive(false);
        apiAccessNOkText.SetActive(false);
        apiAccessNOkMain.SetActive(false);
    }

    private void OnOrganizationChanged(string organization)
    {
        PlayerPrefs.SetString("Organization",organization);
    }
    private void OnApiKeyChanged(string apiKey)
    {
        PlayerPrefs.SetString("PrivateAPIKey",apiKey);
    }

    private void OnLanguageChanged(int value)
    {
        PlayerPrefs.SetString("Language",languageDropdown.options[value].text);
        PlayerPrefs.SetInt("LanguageInt",value);
    }
}


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    public static bool isAPIAccessible = false;
    [SerializeField] private GameObject apiAccessOkText;
    [SerializeField]private GameObject apiAccessNOkText;
    [SerializeField]private TMP_Dropdown languageDropdown;
    [SerializeField]private TMP_InputField organization;
    [SerializeField]private TMP_InputField apiKey;
    

    private void Start()
    {
        languageDropdown.value = PlayerPrefs.GetInt("LanguageInt",0);
        organization.text = PlayerPrefs.GetString("Organization",string.Empty);
        apiKey.text = PlayerPrefs.GetString("PrivateAPIKey",string.Empty);
        organization.onValueChanged.AddListener(OnOrganizationChanged);
        apiKey.onValueChanged.AddListener(OnApiKeyChanged);
        languageDropdown.onValueChanged.AddListener(OnLanguageChanged);
        
    }

    public void tryAPIAccess()
    {
        isAPIAccessible = true;
    }

    private void OnOrganizationChanged(string organization)
    {
        
    }
    private void OnApiKeyChanged(string apiKey)
    {
        
    }

    private void OnLanguageChanged(int value)
    {
        PlayerPrefs.SetString("Language",languageDropdown.options[value].text);
        PlayerPrefs.SetInt("LanguageInt",value);
    }
}

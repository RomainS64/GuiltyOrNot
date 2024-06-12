using UnityEngine;

public class LanguageSetter : MonoBehaviour
{
    void Start()
    {
        GptGeneration[] generations = FindObjectsOfType<GptGeneration>(true);
        string language = PlayerPrefs.GetString("Language", "English");
        foreach (var generation in generations)
        {
            generation.PromptDirection = $"Answer only in {language}, "+generation.PromptDirection;
        }
    }
}

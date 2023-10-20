using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


[Serializable]
public enum EmotionType
{
    Concentrated=0,
    Happy,
    Confused,
    Scared
}

public class SuspectVisualGenerator : MonoBehaviour
{
    [SerializeField]
    private StableDiffusionText2Image suspectGenerator;
    [SerializeField]
    private string negatifPrompt;
    
    private Dictionary<EmotionType, string> visualEmotions = new()
    {
        { EmotionType.Concentrated, "concentrated face" },
        { EmotionType.Happy, "happy face" },
        { EmotionType.Confused, "confused face" },
        { EmotionType.Scared, "scared face" },    
    };

    private void Awake()
    {
        suspectGenerator.OnGenerationDone += OnSuspectGenerationDone;
    }
    
    private int currentGenerationDone = 0;
    private Dictionary<EmotionType, Texture> generatedSprites = new();
    private int seed;
    public async void GenerateSuspectFacesAsync(string suspectPrompt,Action<Dictionary<EmotionType, Texture>> OnGenerated)
    {
        seed = Random.Range(0, 1000000000);
        
        for (int i = 0; i < 4; i++)
        {
            suspectGenerator.prompt = suspectPrompt;
            suspectGenerator.negativePrompt = negatifPrompt;
            //suspectGenerator.selectedModel = 0;
            suspectGenerator.seed = seed;
            suspectGenerator.prompt = $"{suspectPrompt}, {visualEmotions[(EmotionType)i]}";
            suspectGenerator.Generate();
            while (currentGenerationDone == i) await Task.Delay(100);
            Debug.Log($"Generation {i + 1}/4");
        }
        currentGenerationDone = 0;
        OnGenerated?.Invoke(generatedSprites);
        generatedSprites = null;
    }
    private void OnSuspectGenerationDone(Texture _sprite)
    {
        generatedSprites.Add((EmotionType)currentGenerationDone,_sprite);
        currentGenerationDone++;
    }
}

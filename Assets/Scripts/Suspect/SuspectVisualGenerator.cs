using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using JetBrains.Annotations;
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
    private SuspectVisualAttributes HairAttribute;
    [SerializeField] 
    private SuspectVisualAttributes HairColorAttribute;
    [SerializeField] 
    private SuspectVisualAttributes BeardAttributes;
    
    [SerializeField] 
    private SuspectVisualAttributes ClothAttribute;
    [SerializeField] 
    private SuspectVisualAttributes ClothColorAttribute;
    [SerializeField] 
    private SuspectVisualAttributes HeadAccessoryAttribute;

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
    private Texture generatedSprite;
    private int seed;
    public async void GenerateSuspectFaceAsync(Suspect suspect,EmotionType _emotion,bool generatePrompt,Action<(Suspect,Texture)> OnGenerated)
    {
        currentGenerationDone = 0;
        string suspectPrompt;
        if (generatePrompt)
        {
            seed = Random.Range(0, 1000000000);
            suspectPrompt = GeneratePrompt(suspect.sexe == "Male");
            suspect.visualPrompt = suspectPrompt;
            suspect.visualSeed = seed;
        }
        else
        {
            seed = suspect.visualSeed;
            suspectPrompt = suspect.visualPrompt;
        }
        suspectGenerator.negativePrompt = negatifPrompt;
        suspectGenerator.seed = seed;
        suspectGenerator.prompt = $"{suspectPrompt}, {visualEmotions[_emotion]}";
        suspectGenerator.Generate();
        while (currentGenerationDone <= 0 ) await Task.Delay(100);
        OnGenerated?.Invoke((suspect,generatedSprite));
    }

    private string GeneratePrompt(bool _isAMan)
    {
        string style = "in caricature exaggerate face comics in stylized realistic digital art";
        string additionalInfo = "visible bust";
        string suspectCaracteristics = GenerateSuspectVisualCaracteristics(_isAMan);
        string background = "a grey background behind(grey)";
        string prompt = $"{style}, {additionalInfo}, {suspectCaracteristics}, {background},";
        Debug.Log(prompt);
        return prompt;
    }

    private string GenerateSuspectVisualCaracteristics(bool _isAMan)
    {
        string prompt = "";
        prompt += _isAMan ? "A man" : "A woman" ;
        int weightSum = HairAttribute.attributes.Sum(va => _isAMan?va.weights.manProbabilityWeight:va.weights.womanProbabilityWeight);
        int generatedWeight = Random.Range(1,weightSum);
        int currentWeight = 0;
        prompt += $",{GetRandomFromAttribute(HairAttribute, _isAMan)} hairs ({GetRandomFromAttribute(HairColorAttribute,_isAMan)})";
        if(_isAMan)prompt += $",{GetRandomFromAttribute(BeardAttributes, _isAMan)}";
        prompt += $",{GetRandomFromAttribute(HeadAccessoryAttribute,_isAMan)}";
        prompt += $",in a {GetRandomFromAttribute(ClothAttribute, _isAMan)}({GetRandomFromAttribute(ClothColorAttribute, _isAMan)})";
        return prompt;
    }

    private string GetRandomFromAttribute(SuspectVisualAttributes _attributes, bool _isAMan)
    {
        int weightSum = _attributes.attributes.Sum(va => _isAMan?va.weights.manProbabilityWeight:va.weights.womanProbabilityWeight);
        int generatedWeight = Random.Range(1,weightSum);
        int currentWeight = 0;
        foreach (VisualAttribute attribute in _attributes.attributes)
        {
            currentWeight += _isAMan ? attribute.weights.manProbabilityWeight : attribute.weights.womanProbabilityWeight;
            if (currentWeight >= generatedWeight)
            {
                return attribute.prompt;
            }
        }
        return string.Empty;
    }
    private void OnSuspectGenerationDone(Texture _sprite)
    {
        generatedSprite = _sprite;
        currentGenerationDone++;
    }
}

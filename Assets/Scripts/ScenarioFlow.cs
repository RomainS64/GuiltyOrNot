using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ScenarioFlow : MonoSingleton<ScenarioFlow>
{
    [SerializeField] private Texture[] debugTexture;
    [SerializeField] private Suspect[] debugSuspects;
    [SerializeField] private bool generateImages;
    [SerializeField] private bool generateSuspect;
    [SerializeField] private int numberOfSuspects;
    [SerializeField] private GameObject waitingCanvas;
    [SerializeField] private GameObject notebookCanvas;
    [SerializeField] private ScenarioGenerator scenarioGenerator;

    [SerializeField] private SuspectVisualGenerator suspectGenerator;
    private bool scenarioViewSkiped;

    //SCENRAIO
    private Scenario generatedScenario;
    private bool isScenarioGenerated;
    
    //SUSPECT
    public List<Suspect> GeneratedSuspects { get; private set;}
    private int neutralSuspectGenerated = 0;
    private int suspectGenerated = 0;
    
    private ThresholdSpawnableObject[] thresholdSpawnableObject;

    public void SetSuspectEliminationStatus(int _suspectId, bool _eliminated)
    {
        var generatedSuspect = GeneratedSuspects[_suspectId];
        generatedSuspect.isEliminated = _eliminated;
        GeneratedSuspects[_suspectId] = generatedSuspect;
        foreach (var spawnable in thresholdSpawnableObject)
        {
            spawnable.NotifyThreshold(GetAlivedSuspectCout());
        }
    }

    private void Start()
    {
        if(notebookCanvas != null)notebookCanvas.SetActive(false);
    }

    private int GetAlivedSuspectCout()=>GeneratedSuspects.Count(suspect => !suspect.isEliminated);
    
    public void StartGenerating()=>StartCoroutine(StartGeneratingCoroutine());

    private IEnumerator StartGeneratingCoroutine()
    {
        thresholdSpawnableObject = FindObjectsOfType<ThresholdSpawnableObject>(true);
        GeneratedSuspects = new List<Suspect>();
        ScenarioView.OnScenarioViewSkiped+= ()=> scenarioViewSkiped = true;
        notebookCanvas.SetActive(true);
        //DisplayWaiting();
        //StartCoroutine(GenerateScenario());
        StartCoroutine(GenerateSuspectFirstInformations(numberOfSuspects));
        yield return new WaitUntil(() => GeneratedSuspects.Count>=numberOfSuspects);
        //yield return new WaitUntil(() => isScenarioGenerated);
        DisplayCorkBoard();
        
    }

    void DisplayWaiting()
    {
        waitingCanvas.SetActive(true);
    }
    void DisplayCorkBoard()
    {
        foreach (var spawnable in thresholdSpawnableObject)
        {
            spawnable.NotifyThreshold(GetAlivedSuspectCout());
        }
        waitingCanvas.SetActive(false);
        CorkBoardFlowHandler.Instance.StartCorkBoard(generatedScenario,GeneratedSuspects.ToArray());
    }
    private IEnumerator GenerateScenario()
    {
        isScenarioGenerated = false;
        Scenario generatedScenario = new Scenario();
        bool isGenerated = false;
        Debug.LogError("Generate Scenario");
        scenarioGenerator.GenerateScenario((_scenario) =>
        {
            Debug.LogError("Scenario Generated");
            generatedScenario = _scenario;
            isGenerated = true;
        });
        yield return new WaitUntil(() => isGenerated);
        Debug.LogError("Scenario Generated !");
        this.generatedScenario = generatedScenario;
        isScenarioGenerated = true;
    }
    private IEnumerator GenerateSuspectFirstInformations(int numberOfSuspect)
    {
        int generated = 0;
        for (int i = 0; i < numberOfSuspect; i++)
        {
            Suspect generatedSuspect;
            if(generateSuspect){
                generatedSuspect = SuspectGenerator.Instance.GenerateSuspect();
            }
            else
            {
                generatedSuspect = debugSuspects[i];
            }
            
            Debug.LogError($"Generate Suspect {generatedSuspect.name}");
            if (generateImages)
            {
                suspectGenerator.GenerateSuspectFaceAsync(generatedSuspect,EmotionType.Concentrated,true,(_result =>
                {
                    generatedSuspect.emotions = new Dictionary<EmotionType, Texture>()
                        { { EmotionType.Concentrated, _result.Item2 } };
                    GeneratedSuspects.Add(generatedSuspect);
                    generated++;
                }));
            }
            else
            {
                generatedSuspect.emotions = new Dictionary<EmotionType, Texture>()
                    { { EmotionType.Concentrated, debugTexture[generated] } };
                GeneratedSuspects.Add(generatedSuspect);
                generated++;
            }
            
            yield return new WaitWhile(() => generated == i);
        }
    }
    private IEnumerator GenerateSuspectEmotions(Suspect suspect,Action onGenerated)
    {
        int generated = 0;
        for (int i=1;i<=(int)EmotionType.Scared;i++)
        {
            EmotionType emotion = (EmotionType)i;
            Debug.Log($"Generate Suspect Emotion({emotion.ToString()}) {suspect.name}");
            suspectGenerator.GenerateSuspectFaceAsync(suspect,emotion,false,(_result =>
            {
                suspect.emotions.Add((EmotionType)i,_result.Item2);
                generated++;
            }));
            yield return new WaitWhile(() => generated == i-1);
        }
        onGenerated?.Invoke();
    }
}
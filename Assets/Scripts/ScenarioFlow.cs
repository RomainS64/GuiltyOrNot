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
        StartGenerating();
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
        StartCoroutine(GenerateScenario());
        StartCoroutine(GenerateSuspects(numberOfSuspects));
        yield return new WaitUntil(() => GeneratedSuspects.Count>=numberOfSuspects);
        yield return new WaitUntil(() => isScenarioGenerated);
        InternetHistoryGenerator.Instance.GenerateRandomInternetHistory(generatedScenario,null);
        CorkBoardFlowHandler.Instance.StartCorkBoard(generatedScenario,GeneratedSuspects);
        Debug.Log(generatedScenario.scenarioString);
    }
    private IEnumerator GenerateScenario()
    {
        isScenarioGenerated = false;
        Scenario generatedScenario = new Scenario();
        bool isGenerated = false;
        Debug.Log("Generate Scenario");
        scenarioGenerator.GenerateScenario((_scenario) =>
        {
            generatedScenario = _scenario;
            isGenerated = true;
        });
        yield return new WaitUntil(() => isGenerated);
        Debug.Log("Scenario Generated");
        this.generatedScenario = generatedScenario;
        isScenarioGenerated = true;
    }
    private IEnumerator GenerateSuspects(int numberOfSuspect)
    {
        int generated = 0;
        for (int i = 0; i < numberOfSuspect; i++)
        {
            Suspect generatedSuspect;
            if(generateSuspect)
            {
                generatedSuspect = SuspectGenerator.Instance.GenerateSuspect(i);
            }
            else
            { 
                generatedSuspect = debugSuspects[i];
            }
            if (generateImages)
            {
                suspectGenerator.GenerateSuspectFaceAsync(generatedSuspect,EmotionType.Concentrated,true,(_result =>
                {
                    generatedSuspect.portrait = _result.Item2;
                    GeneratedSuspects.Add(generatedSuspect);
                    generated++;
                }));
            }
            else
            {
                generatedSuspect.portrait = debugTexture[generated];
                GeneratedSuspects.Add(generatedSuspect);
                generated++;
            }
            yield return new WaitWhile(() => generated == i);
        }
    }
}

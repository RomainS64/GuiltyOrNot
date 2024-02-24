using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Serialization;
using Debug = UnityEngine.Debug;
using Random = UnityEngine.Random;

public class ScenarioFlow : MonoSingleton<ScenarioFlow>
{
    [SerializeField] private Texture[] debugTexture;
    [SerializeField] private Suspect[] debugSuspects;
    [SerializeField] private string[] debugText;
    [SerializeField] private bool generateTexts;
    [SerializeField] private bool generateImages;
    [SerializeField] private bool generateSuspect;
    [SerializeField] private int numberOfSuspects;
    [SerializeField] private GameObject notebookCanvas;
    [SerializeField] private ScenarioGenerator scenarioGenerator;
    [SerializeField] private InternetHistoryGenerator[] internetHistoryGenerator;
    [SerializeField] private BankAccountGenerator[] bankAccountGenerator;
    [SerializeField] private CriminalRecordGenerator[] criminalRecords;
    
    [SerializeField] private SuspectVisualGenerator suspectGenerator;
    private bool scenarioViewSkiped;

    //SCENRAIO
    private Scenario generatedScenario;
    private bool isScenarioGenerated;
    
    
    //SUSPECT
    public List<Suspect> GeneratedSuspects { get; private set;}
    private int neutralSuspectGenerated = 0;
    private int suspectGenerated = 0;
    
    //INTERNET HISTORY
    private bool isInternetHistoryGenerated = false;
    //BANK ACCOUNT
    private bool isBankAccountGenerated = false;
    //CRIMINAL RECORD
    private bool isCriminalRecordGenerated = false;
    
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
        //if(notebookCanvas != null)notebookCanvas.SetActive(false);
    }

    private int GetAlivedSuspectCout()=>GeneratedSuspects.Count(suspect => !suspect.isEliminated);
    
    public void StartGenerating()=>StartCoroutine(StartGeneratingCoroutine());

    
    
    private IEnumerator StartGeneratingCoroutine()
    {
        thresholdSpawnableObject = FindObjectsOfType<ThresholdSpawnableObject>(true);
        GeneratedSuspects = new List<Suspect>();
        ScenarioView.OnScenarioViewSkiped+= ()=> scenarioViewSkiped = true;
        //notebookCanvas.SetActive(true);
        StartCoroutine(GenerateScenario());
        StartCoroutine(GenerateSuspects(numberOfSuspects));
        yield return new WaitUntil(() => isScenarioGenerated);
        yield return new WaitUntil(() => GeneratedSuspects.Count>=numberOfSuspects);
        
        StartCoroutine(GenerateInternetHystory(numberOfSuspects));
        StartCoroutine(GenerateBankAccount(numberOfSuspects));
        StartCoroutine(GenerateCriminalRecord(numberOfSuspects));
        
        yield return new WaitUntil(() => isInternetHistoryGenerated);
        yield return new WaitUntil(() => isBankAccountGenerated);
        yield return new WaitUntil(() => isCriminalRecordGenerated);
        
        CorkBoardFlowHandler.Instance.StartCorkBoard(generatedScenario,GeneratedSuspects);
        foreach (var spawnable in thresholdSpawnableObject) spawnable.NotifyThreshold(GetAlivedSuspectCout());
    }

    public List<string> innocentInternetHistory = new ();
    public List<string> guiltyInternetHistory = new ();
    private IEnumerator GenerateInternetHystory(int _numberOfSuspect)
    {
        int nbGenerated = 0;
        if (generateTexts)
        {
            for (int i = 0; i < _numberOfSuspect; i++)
            {
                void OnInnocentHistoryGenerated(List<string> _generated)
                {
                    innocentInternetHistory.AddRange(_generated);
                    nbGenerated++;
                }
                void OnGuiltyHistoryGenerated(List<string> _generated)
                {
                    guiltyInternetHistory.AddRange(_generated);
                    nbGenerated++;
                }
                internetHistoryGenerator[i].GenerateRandomInternetHistoryAsync(generatedScenario,false,10,OnInnocentHistoryGenerated);
                //internetHistoryGenerator[i].GenerateRandomInternetHistoryAsync(generatedScenario,true,5,OnGuiltyHistoryGenerated);
            }
            yield return new WaitWhile(() => nbGenerated < _numberOfSuspect);
        }
        else
        {
            foreach (var history in debugText)
            {
                innocentInternetHistory.Add(history);
                guiltyInternetHistory.Add(history);
            }
        }
        
        for(int i=0;i<GeneratedSuspects.Count;++i)
        {
            Shuffle(innocentInternetHistory);
            Shuffle(guiltyInternetHistory);
            var suspect = GeneratedSuspects[i];
            suspect.internetHistory.stringList = innocentInternetHistory.GetRange(0, 7);
            //Ajouter les coupables
            Shuffle(suspect.internetHistory.stringList);
            GeneratedSuspects[i] = suspect;
        }
        isInternetHistoryGenerated = true;
    }
    
    public List<KeyValuePair<string,float>> innocentBankAccount = new ();
    public List<KeyValuePair<string,float>> guiltyBankAccount = new ();
    private IEnumerator GenerateBankAccount(int _numberOfSuspect)
    {
        int nbGenerated = 0;
        if (generateTexts)
        {
            for (int i = 0; i < _numberOfSuspect; i++)
            {
                void OnInnocentBankAccountGenerated(List<KeyValuePair<string, float>> _generated)
                {
                    innocentBankAccount.AddRange(_generated);
                    nbGenerated++;
                }

                void OnGuiltyBankAccountGenerated(List<KeyValuePair<string, float>> _generated)
                {
                    guiltyBankAccount.AddRange(_generated);
                    nbGenerated++;
                }

                bankAccountGenerator[i]
                    .GenerateRandomBankAccountAsync(generatedScenario, false, 10, OnInnocentBankAccountGenerated);
                //internetHistoryGenerator[i].GenerateRandomInternetHistoryAsync(generatedScenario,true,5,OnGuiltyHistoryGenerated);
            }

            yield return new WaitWhile(() => nbGenerated < _numberOfSuspect);
        }
        else
        {
            foreach (var history in debugText)
            {
                innocentBankAccount.Add(new KeyValuePair<string, float>(history,0));
                guiltyBankAccount.Add(new KeyValuePair<string, float>(history,0));
                
            }
        }

        for(int i=0;i<GeneratedSuspects.Count;++i)
        {
            Shuffle(innocentBankAccount);
            Shuffle(guiltyBankAccount);
            var suspect = GeneratedSuspects[i];
            suspect.bankAccountHistory.current = Random.Range(-1000000f, 1000000f);
            suspect.bankAccountHistory.saving = Random.Range(0, 1000000);
            suspect.bankAccountHistory.transactions = innocentBankAccount.GetRange(0, 5);
            GeneratedSuspects[i] = suspect;
        }
        isBankAccountGenerated = true;
    }
    public List<KeyValuePair<string,string>> innocentCriminalRecord = new ();
    public List<KeyValuePair<string,string>> guiltyCriminalRecord = new ();
    private IEnumerator GenerateCriminalRecord(int _numberOfSuspect)
    {
        int nbGenerated = 0;
        if (generateTexts)
        {
            for (int i = 0; i < _numberOfSuspect; i++)
            {
                void OnInnocentCriminalRecord(List<KeyValuePair<string, string>> _generated)
                {
                    innocentCriminalRecord.AddRange(_generated);
                    nbGenerated++;
                }

                void OnGuiltyCriminalRecord(List<KeyValuePair<string, string>> _generated)
                {
                    guiltyCriminalRecord.AddRange(_generated);
                    nbGenerated++;
                }

                criminalRecords[i]
                    .GenerateRandomCriminalRecordAsync(generatedScenario, false, 10, OnInnocentCriminalRecord);
                //internetHistoryGenerator[i].GenerateRandomInternetHistoryAsync(generatedScenario,true,5,OnGuiltyHistoryGenerated);
            }

            yield return new WaitWhile(() => nbGenerated < _numberOfSuspect);
        }
        else
        {
            foreach (var history in debugText)
            {
                innocentCriminalRecord.Add(new KeyValuePair<string, string>(history,history));
                guiltyCriminalRecord.Add(new KeyValuePair<string, string>(history,history));
                
            }
        }

        for(int i=0;i<GeneratedSuspects.Count;++i)
        {
            Shuffle(innocentCriminalRecord);
            Shuffle(guiltyCriminalRecord);
            var suspect = GeneratedSuspects[i];
            suspect.criminalRecord.records = innocentCriminalRecord.GetRange(0, 5);
            Shuffle(suspect.criminalRecord.records);
            GeneratedSuspects[i] = suspect;
        }
        isCriminalRecordGenerated = true;
    }
    private IEnumerator GenerateScenario()
    {
        isScenarioGenerated = false;
        Scenario generatedScenario = new Scenario();
        bool isGenerated = false;
        Debug.Log("Generate Scenario");
        if (generateTexts)
        {
            scenarioGenerator.GenerateScenario((_scenario) =>
            {
                generatedScenario = _scenario;
                isGenerated = true;
            });
            yield return new WaitUntil(() => isGenerated);
        }
        else
        {
            generatedScenario.scenarioString = "[DEBUG] Scenario not generated";
        }
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
            GeneratedSuspects.Add(generatedSuspect);
            if (generateImages)
            {
                suspectGenerator.GenerateSuspectFaceAsync(generatedSuspect,EmotionType.Concentrated,true,(_result =>
                {
                    var suspect = GeneratedSuspects[i];
                    suspect.portrait = _result.Item2;
                    GeneratedSuspects[i] = suspect;
                    generated++;
                }));
            }
            else
            {
                var suspect = GeneratedSuspects[i];
                suspect.portrait = debugTexture[i];
                GeneratedSuspects[i] = suspect;
                generated++;
            }
            yield return new WaitWhile(() => generated == i);
        }
    }
    // Fisher-Yates shuffle algorithm
    static public void Shuffle<T>(List<T> list)
    {
        int n = list.Count;
        System.Random rng = new System.Random();

        while (n > 1)
        {
            n--;
            int k = rng.Next(n + 1);
            T value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
}

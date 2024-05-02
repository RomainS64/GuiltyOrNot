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
    [SerializeField] private string[] debugTextInternet;
    [SerializeField] private string[] debugTextCriminal;
    [SerializeField] private string[] debugTextBank;
    [SerializeField] private bool generateAtStart = true;
    [SerializeField] private bool generateTexts;
    [SerializeField] private bool generateImages;
    [SerializeField] private bool generateSuspect;
    [SerializeField] private int numberOfSuspects;
    [SerializeField] private GameObject notebookCanvas;
    [SerializeField] private ScenarioGenerator scenarioGenerator;
    [SerializeField] private InternetHistoryGenerator[] internetHistoryGenerator;
    [SerializeField] private BankAccountGenerator[] bankAccountGenerator;
    [SerializeField] private CriminalRecordGenerator[] criminalRecords;
    
    [SerializeField] private DallESuspectVisualGenerator suspectGenerator;
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

    private ThresholdSpawnableObject[] thresholdSpawnableObject = { };

    public void SetSuspectEliminationStatus(int _suspectId, bool _eliminated)
    {
        var generatedSuspect = GeneratedSuspects[_suspectId];
        generatedSuspect.isEliminated = _eliminated;
        generatedSuspect.aliveCountWhenEliminated = GetAlivedSuspectCout();
        GeneratedSuspects[_suspectId] = generatedSuspect;
        
        foreach (var spawnable in thresholdSpawnableObject)
        {
            spawnable.NotifyThreshold(GetAlivedSuspectCout(),GeneratedSuspects[spawnable.GetPlayerId()].aliveCountWhenEliminated,GeneratedSuspects[spawnable.GetPlayerId()].isEliminated);

        }
    }

    private void Start()
    {
        if(generateAtStart)StartGenerating();
        //if(notebookCanvas != null)notebookCanvas.SetActive(false);
    }

    private int GetAlivedSuspectCout()=>GeneratedSuspects.Count(suspect => !suspect.isEliminated);
    
    public void StartGenerating()=>StartCoroutine(StartGeneratingCoroutine());

    [SerializeField] private GameObject loadingCanvas;
    private IEnumerator StartGeneratingCoroutine()
    {
        loadingCanvas.SetActive(true);
        thresholdSpawnableObject = FindObjectsOfType<ThresholdSpawnableObject>(true);
        GeneratedSuspects = new List<Suspect>();
        ScenarioView.OnScenarioViewSkiped+= ()=> scenarioViewSkiped = true;
        //notebookCanvas.SetActive(true);
        StartCoroutine(GenerateScenario());
        yield return new WaitUntil(() => isScenarioGenerated);
        StartCoroutine(GenerateSuspects(numberOfSuspects,null));
        yield return new WaitUntil(() => generatedSuspect >= numberOfSuspects);
        
        StartCoroutine(GenerateInternetHystory(numberOfSuspects));
        StartCoroutine(GenerateBankAccount(numberOfSuspects));
        StartCoroutine(GenerateCriminalRecord(numberOfSuspects));
        
        yield return new WaitUntil(() => isInternetHistoryGenerated);
        yield return new WaitUntil(() => isBankAccountGenerated);
        yield return new WaitUntil(() => isCriminalRecordGenerated);
        
        CorkBoardFlowHandler.Instance.StartCorkBoard(generatedScenario,GeneratedSuspects);
        foreach (var spawnable in thresholdSpawnableObject) spawnable.NotifyThreshold(GetAlivedSuspectCout(),numberOfSuspects,false);
        loadingCanvas.SetActive(false);
    }

    public List<string> innocentInternetHistory = new ();
    public List<string> guiltyInternetHistory = new ();
    private IEnumerator GenerateInternetHystory(int _numberOfSuspect)
    {
        int nbGenerated = 0;
        if (generateTexts)
        {
            innocentInternetHistory = new ();
            guiltyInternetHistory = new ();
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
                internetHistoryGenerator[i].GenerateRandomInternetHistoryAsync(generatedScenario,true,5,OnGuiltyHistoryGenerated);
            }
            yield return new WaitWhile(() => nbGenerated < _numberOfSuspect*2);
        }
        else
        {
            foreach (var history in debugTextInternet)
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
            if (i <= 5)
            {
                suspect.internetHistory.stringList = innocentInternetHistory.GetRange(0, 7);
                //Ajouter les coupables
                Shuffle(suspect.internetHistory.stringList);
                GeneratedSuspects[i] = suspect;
            }
            else
            {
                suspect.internetHistory.stringList = innocentInternetHistory.GetRange(0, 5);
                suspect.internetHistory.stringList.AddRange(guiltyInternetHistory.GetRange(0,2));
                //Ajouter les coupables
                Shuffle(suspect.internetHistory.stringList);
                GeneratedSuspects[i] = suspect;
            }
            
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

                bankAccountGenerator[i].GenerateRandomBankAccountAsync(generatedScenario, false, 10, OnInnocentBankAccountGenerated);
                bankAccountGenerator[i].GenerateRandomBankAccountAsync(generatedScenario,true,5,OnGuiltyBankAccountGenerated);
            }

            yield return new WaitWhile(() => nbGenerated < _numberOfSuspect*2);
        }
        else
        {
            foreach (var history in debugTextBank)
            {
                innocentBankAccount.Add(new KeyValuePair<string, float>(history,0));
                guiltyBankAccount.Add(new KeyValuePair<string, float>(history,0));
                
            }
        }

        for(int i=0;i<GeneratedSuspects.Count;++i)
        {
            Shuffle(innocentBankAccount);
            Shuffle(guiltyBankAccount);
            
            if (i <= 7)
            {
                var suspect = GeneratedSuspects[i];
                suspect.bankAccountHistory.current = Random.Range(0, 10_000f);
                suspect.bankAccountHistory.saving = Random.Range(0, 100_000);
                suspect.bankAccountHistory.transactions = innocentBankAccount.GetRange(0, 5);
                GeneratedSuspects[i] = suspect;
            }
            else
            {
                var suspect = GeneratedSuspects[i];
                suspect.bankAccountHistory.current = Random.Range(-10_000, 10_000f);
                suspect.bankAccountHistory.saving = Random.Range(-10_000, 100_000);
                suspect.bankAccountHistory.transactions = innocentBankAccount.GetRange(0, 3);
                suspect.bankAccountHistory.transactions.AddRange(guiltyBankAccount.GetRange(0,2));
                Shuffle(suspect.bankAccountHistory.transactions);
                GeneratedSuspects[i] = suspect;
            }
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

                criminalRecords[i].GenerateRandomCriminalRecordAsync(generatedScenario, false, 10, OnInnocentCriminalRecord);
                criminalRecords[i].GenerateRandomCriminalRecordAsync(generatedScenario,true,5,OnGuiltyCriminalRecord);
            }

            yield return new WaitWhile(() => nbGenerated < _numberOfSuspect*2);
        }
        else
        {
            foreach (var history in debugTextInternet)
            {
                innocentCriminalRecord.Add(new KeyValuePair<string, string>(history,history));
                guiltyCriminalRecord.Add(new KeyValuePair<string, string>(history,history));
                
            }
        }

        for(int i=0;i<GeneratedSuspects.Count;++i)
        {
            Shuffle(innocentCriminalRecord);
            Shuffle(guiltyCriminalRecord);
            
            
            if (i <= 9)
            {
                var suspect = GeneratedSuspects[i];
                suspect.criminalRecord.records = innocentCriminalRecord.GetRange(0, 5);
                Shuffle(suspect.criminalRecord.records);
                GeneratedSuspects[i] = suspect;
            }
            else
            {
                var suspect = GeneratedSuspects[i];
                suspect.criminalRecord.records = innocentCriminalRecord.GetRange(0, 3);
                suspect.criminalRecord.records.AddRange(guiltyCriminalRecord.GetRange(0, 2));
                Shuffle(suspect.criminalRecord.records);
                GeneratedSuspects[i] = suspect;
            }
            
        }
        isCriminalRecordGenerated = true;
    }
    public IEnumerator GenerateScenario(Action<Scenario> _onScenarioGenerated = null)
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
            generatedScenario.scenarioString = "[DEBUG] Scenario not generatedSuspect";
        }
        Debug.Log("Scenario Generated");
        this.generatedScenario = generatedScenario;
        _onScenarioGenerated?.Invoke(generatedScenario);
        isScenarioGenerated = true;
    }
    int generatedSuspect = 0;
    public IEnumerator GenerateSuspects(int numberOfSuspect,Action<List<Suspect>> _onSuspectsGenerated = null)
    {
        GeneratedSuspects = new List<Suspect>();
        generatedSuspect = 0;
        for (int i = 0; i < numberOfSuspect; i++)
        {
            Suspect generatedSuspect;
            if(generateSuspect)
            {
                generatedSuspect = SuspectGenerator.Instance.GenerateSuspect(generatedScenario,i);
            }
            else
            { 
                generatedSuspect = debugSuspects[i];
            }
            GeneratedSuspects.Add(generatedSuspect);
            if (generateImages)
            {
                int suspectId = i;
                suspectGenerator.GenerateSuspectFaceAsync(generatedSuspect,EmotionType.Concentrated,true,(_result =>
                {
                    var suspect = GeneratedSuspects[suspectId];
                    suspect.portrait = _result.Item2;
                    GeneratedSuspects[suspectId] = suspect;
                    this.generatedSuspect++;
                }));
            }
            else
            {
                var suspect = GeneratedSuspects[i];
                suspect.portrait = debugTexture[i];
                GeneratedSuspects[i] = suspect;
                this.generatedSuspect++;
            }
        }
        yield return new WaitWhile(() => this.generatedSuspect == numberOfSuspect);
        _onSuspectsGenerated?.Invoke(GeneratedSuspects);
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

using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;


public class InternetHistoryGenerator : MonoSingleton<InternetHistoryGenerator>
{
    private string guiltyPrompt = "Provide directly a list of 10 internet histories related to the investigation.Here is the investigation:";
    private string innocentPrompt = "Provide directly a list of 10 internet histories NOT related to the investigation.Here is the investigation:";
    [SerializeField] private GptGeneration guiltyGeneration;
    [SerializeField] private GptGeneration innocentGeneration;

    public async void GenerateRandomInternetHistory(Scenario _scenario,bool guilty,int nb,Action<List<string>> _onGenerated)
    {
        string innocentGenerated = string.Empty;
        string guiltyGenerated = string.Empty;
        
        guiltyGeneration.SetUpConversation();
        innocentGeneration.SetUpConversation();
        guiltyGeneration.OnGPTResponseReceived += (response) => guiltyGenerated = response;
        innocentGeneration.OnGPTResponseReceived += (response) => innocentGenerated = response;
        guiltyGeneration.SendMessage(guiltyPrompt+_scenario.scenarioString);
        innocentGeneration.SendMessage(innocentPrompt+_scenario.scenarioString);
        while (guiltyGenerated == string.Empty || innocentGenerated == string.Empty) await Task.Delay(50);
        
        Debug.Log(innocentGenerated);
        Debug.Log(guiltyGenerated);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;


public class InternetHistoryGenerator : MonoBehaviour
{
    private string guiltyPrompt = "Provide directly a list of [NUMBER] internet histories related to the investigation.Here is the investigation:";
    private string innocentPrompt = "Provide directly a list of [NUMBER] internet histories NOT related to the investigation.Here is the investigation:";
    private string numberTag = "[NUMBER]";
    [SerializeField] private GptGeneration guiltyGeneration;
    [SerializeField] private GptGeneration innocentGeneration;

    public void GenerateRandomInternetHistoryAsync(Scenario _scenario,bool guilty,int nb,Action<List<string>> _onGenerated)
    {
        if (guilty)
        {
            GenerateGuilty(_scenario.scenarioString, nb, list => _onGenerated.Invoke(list));
        }
        GenerateInnocent(_scenario.scenarioString, nb, list => _onGenerated.Invoke(list));
    }

    private async void GenerateGuilty(string _scenario,int nb,Action<List<string>> _onGenerated)
    {
        string guiltyGenerated = string.Empty;
        
        guiltyGeneration.SetUpConversation();

        guiltyGeneration.OnGPTResponseReceived += (response) => guiltyGenerated = response;
        guiltyGeneration.SendMessage(guiltyPrompt.Replace(numberTag,nb.ToString())+_scenario);
        while (guiltyGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(SplitString(guiltyGenerated));
    }
    private async void GenerateInnocent(string _scenario,int nb,Action<List<string>> _onGenerated)
    {
        string innocentGenerated = string.Empty;
        innocentGeneration.SetUpConversation();
        innocentGeneration.OnGPTResponseReceived += (response) => innocentGenerated = response;
        innocentGeneration.SendMessage(innocentPrompt.Replace(numberTag,nb.ToString())+_scenario);
        while (innocentGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(SplitString(innocentGenerated));
    }

    private List<string> SplitString(string _string)
    {
        string startPattern = @"^\d+[.\-]? ";
        List<string> values =  new List<string>(_string.Split("\n").ToArray());
        for (int i = 0; i < values.Count; i++)
        {
            values[i] =  Regex.Replace(values[i],startPattern, string.Empty);
        }
        

        return values;
    }
}

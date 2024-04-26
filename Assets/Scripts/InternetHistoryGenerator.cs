using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;


public class InternetHistoryGenerator : MonoBehaviour
{
    private string guiltyPrompt = "Generate me a list of [NUMBER] very short and realistic internet searches that have to be linked to this investigation (very short sentence), (list format), (no animals), (forbidden word: search, find), (internet searches should not be similar in content and form), (no imaginary animals) (it is imperative to bo linked with the following investigation):";
    private string innocentPrompt = "Generate me a list of [NUMBER] very short internet searches of some realistic and humorous word. (very short sentence), (list format), (no animals), (forbidden word: search, find), (internet searches must not be similar in content and form), (no imaginary animals).";
    private string numberTag = "[NUMBER]";
    [SerializeField] private GptGeneration guiltyGeneration;
    [SerializeField] private GptGeneration innocentGeneration;

    public void GenerateRandomInternetHistoryAsync(Scenario _scenario,bool guilty,int nb,Action<List<string>> _onGenerated)
    {
        if (guilty)
        {
            GenerateGuilty(_scenario.scenarioString, nb, list => _onGenerated.Invoke(list));
        }
        else
        {
            GenerateInnocent(_scenario.scenarioString, nb, list => _onGenerated.Invoke(list));
        }
        
    }

    private async void GenerateGuilty(string _scenario,int nb,Action<List<string>> _onGenerated)
    {
        string guiltyGenerated = string.Empty;
        
        guiltyGeneration.SetUpConversation();

        guiltyGeneration.OnGPTResponseReceived += (response) => guiltyGenerated = response;
        guiltyGeneration.SendMessage(guiltyPrompt.Replace(numberTag,nb.ToString())+_scenario);
        while (guiltyGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(SplitString(guiltyGenerated,""));
    }
    private async void GenerateInnocent(string _scenario,int nb,Action<List<string>> _onGenerated)
    {
        string innocentGenerated = string.Empty;
        innocentGeneration.SetUpConversation();
        innocentGeneration.OnGPTResponseReceived += (response) => innocentGenerated = response;
        innocentGeneration.SendMessage(innocentPrompt.Replace(numberTag,nb.ToString()));
        while (innocentGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(SplitString(innocentGenerated,""));
    }

    private List<string> SplitString(string _string,string _debugMarker = "")
    {
        string startPattern = @"^\d+[.\-]? ";
        List<string> values =  new List<string>(_string.Split("\n").ToArray());
        for (int i = 0; i < values.Count; i++)
        {
            values[i] =  Regex.Replace(values[i],startPattern, string.Empty);
            values[i] = _debugMarker+values[i];
        }
        

        return values;
    }
}

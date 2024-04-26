using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


public class CriminalRecordGenerator : MonoBehaviour
{
    private string guiltyPrompt = "Directly provide a list of [NUMBER] crimes RELATED to the investigation.Here is the investigation:";
    private string innocentPrompt = "Directly provide a list of [NUMBER] crimes NOT related to the investigation.Here is the investigation:";
    private string numberTag = "[NUMBER]";
    [SerializeField] private GptGeneration guiltyGeneration;
    [SerializeField] private GptGeneration innocentGeneration;

    public void GenerateRandomCriminalRecordAsync(Scenario _scenario,bool guilty,int nb,Action<List<KeyValuePair<string,string>>> _onGenerated)
    {
        if (guilty) GenerateGuilty(_scenario.scenarioString, nb, list => _onGenerated.Invoke(list));
        else GenerateInnocent(_scenario.scenarioString, nb, list => _onGenerated.Invoke(list));
    }

    private async void GenerateGuilty(string _scenario,int nb,Action< List<KeyValuePair<string,string>> > _onGenerated)
    {
        string guiltyGenerated = string.Empty;
        
        guiltyGeneration.SetUpConversation();

        guiltyGeneration.OnGPTResponseReceived += (response) => guiltyGenerated = response;
        guiltyGeneration.SendMessage(guiltyPrompt.Replace(numberTag,nb.ToString())+_scenario);
        while (guiltyGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(GenerateValues(guiltyGenerated));
    }
    private async void GenerateInnocent(string _scenario,int nb,Action< List<KeyValuePair<string,string>> > _onGenerated)
    {
        string innocentGenerated = string.Empty;
        innocentGeneration.SetUpConversation();
        innocentGeneration.OnGPTResponseReceived += (response) => innocentGenerated = response;
        innocentGeneration.SendMessage(innocentPrompt.Replace(numberTag,nb.ToString())+_scenario);
        while (innocentGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(GenerateValues(innocentGenerated));
    }

    private List<KeyValuePair<string,string>> GenerateValues(string _string)
    {
        string valuePattern = @"\[([^\]]*)\]";
        string startPattern = @"^\d+[.\-]? ";
        List<string> values =  new List<string>(_string.Split("\n").ToArray());
        List<KeyValuePair<string, string>> transactions = new ();
        for (int i = 0; i < values.Count; i++)
        {
            MatchCollection matches = Regex.Matches(values[i], valuePattern);
            if (matches.Count == 0) continue;
            string date = matches[0].Value.Replace("[",String.Empty).Replace("]",String.Empty);
            string text  = Regex.Replace( values[i].Replace(matches[0].Value,String.Empty), startPattern, String.Empty);
           
            transactions.Add(new KeyValuePair<string, string>(text,date));
        }
        return transactions;
    }
}

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
    private string guiltyPrompt = "generate for me a list of [NUMBER] realistic, funny and humorous criminal records of a suspect that closely resembles this investigation. (sentence of 8 words maximum), (list format), (the antecedents must not resemble each other in form or content), (indicate the year for each antecedent in square brackets):";
    private string innocentPrompt = "generate a list of [NUMBER] realistic and humorous judicial precedents that have nothing to do with 'theft'. Sentence starters should never start the same way. (sentence of 8 words maximum), (list format), (the antecedents must not resemble each other in form or content), (indicate the year for each antecedent in square brackets).";
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
        _onGenerated?.Invoke(GenerateValues(guiltyGenerated,""));
    }
    private async void GenerateInnocent(string _scenario,int nb,Action< List<KeyValuePair<string,string>> > _onGenerated)
    {
        string innocentGenerated = string.Empty;
        innocentGeneration.SetUpConversation();
        innocentGeneration.OnGPTResponseReceived += (response) => innocentGenerated = response;
        innocentGeneration.SendMessage(innocentPrompt.Replace(numberTag,nb.ToString()));
        while (innocentGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(GenerateValues(innocentGenerated));
    }

    private List<KeyValuePair<string,string>> GenerateValues(string _string,string _debug = "")
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
            string text  = _debug+Regex.Replace( values[i].Replace(matches[0].Value,String.Empty), startPattern, String.Empty);
            text = text.Replace("\"", "").Replace("-", "");
            transactions.Add(new KeyValuePair<string, string>(text,date));
        }
        return transactions;
    }
}

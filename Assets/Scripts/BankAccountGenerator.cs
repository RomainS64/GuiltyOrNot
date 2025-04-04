using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


public class BankAccountGenerator : MonoBehaviour
{
    private string guiltyPrompt = "Generate a list of [NUMBER] purchases that can be linked to this scenario :";
    private string innocentPrompt = "Generate a list of [NUMBER] bank statements whose utility or use is changed to make them absurd, zany and funny in 3 words maximum.";
    private string numberTag = "[NUMBER]";
    [SerializeField] private GptGeneration guiltyGeneration;
    [SerializeField] private GptGeneration innocentGeneration;

    public void GenerateRandomBankAccountAsync(Scenario _scenario,bool guilty,int nb,Action<List<KeyValuePair<string,float>>> _onGenerated)
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

    private async void GenerateGuilty(string _scenario,int nb,Action< List<KeyValuePair<string,float>> > _onGenerated)
    {
        string guiltyGenerated = string.Empty;
        
        guiltyGeneration.SetUpConversation();

        guiltyGeneration.OnGPTResponseReceived += (response) => guiltyGenerated = response;
        guiltyGeneration.SendMessage(guiltyPrompt.Replace(numberTag,nb.ToString())+_scenario);
        while (guiltyGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(GenerateValues(guiltyGenerated,""));
    }
    private async void GenerateInnocent(string _scenario,int nb,Action< List<KeyValuePair<string,float>> > _onGenerated)
    {
        string innocentGenerated = string.Empty;
        innocentGeneration.SetUpConversation();
        innocentGeneration.OnGPTResponseReceived += (response) => innocentGenerated = response;
        innocentGeneration.SendMessage(innocentPrompt.Replace(numberTag,nb.ToString()));
        while (innocentGenerated == string.Empty) await Task.Delay(50);
        _onGenerated?.Invoke(GenerateValues(innocentGenerated));
    }

    private List<KeyValuePair<string,float>> GenerateValues(string _string,string _debug = "")
    {
        string pattern = @"\[([^\]]*)\]";
        string startPattern = @"^\d+[.\-]? ";
        List<string> values =  new List<string>(_string.Split("\n").ToArray());
        List<KeyValuePair<string, float>> transactions = new List<KeyValuePair<string, float>>();
        for (int i = 0; i < values.Count; i++)
        {
            MatchCollection matches = Regex.Matches(values[i], pattern);
            if (matches.Count == 0) continue;
            
            string money = matches[0].Value.Replace("[",String.Empty).Replace("]",String.Empty);
            if (float.TryParse(money,NumberStyles.Any,CultureInfo.InvariantCulture,out float result))
            {
                string text  = _debug+Regex.Replace( values[i].Replace(matches[0].Value,String.Empty), startPattern, String.Empty);
                text = text.Replace("\"", "").Replace("-", "");
                Debug.Log("value parsed:"+result.ToString("F")+"$");
                transactions.Add(new KeyValuePair<string, float>(text,result));
            }
        }
        return transactions;
    }
}

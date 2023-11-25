using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;

public class ScenarioGenerator : MonoBehaviour
{
    [SerializeField] private GptGeneration scenarioGenerator;
    [SerializeField] private GptGeneration objectLostGenerator;
    [SerializeField] private GptGeneration thiefLocationGenerator;
    
    public const string LocationPath = "locations";
    public const string DataFolder = "Data/";
    public const string ObjectPath = "objects";
    
    private static string GetRandomExamplesFrom(string _path)
    {
        TextAsset locations = Resources.Load<TextAsset>(DataFolder + _path);
        string[] allLocations = locations.text.Split("\n");
        string result = "examples:\n";
        for (int i = 0; i < 5; i++)
        {
            result += allLocations[Random.Range(0, allLocations.Length)] + "\n";
        }

        return result;
    }
    public async void GenerateScenario(Action<Scenario> _onGenerated)
    {
        string objectLost = string.Empty;
        string thiefLocation = string.Empty;

        objectLostGenerator.Temperature = Random.Range(1f, 2f);
        objectLostGenerator.Frequency_Penality = Random.Range(-2f, 2f);
        objectLostGenerator.Presence_Penality = Random.Range(-2f, 2f);
        thiefLocationGenerator.Temperature = Random.Range(1f, 2f);
        thiefLocationGenerator.Frequency_Penality = Random.Range(-2f, 2f);
        thiefLocationGenerator.Presence_Penality = Random.Range(-2f, 2f);

        scenarioGenerator.SetUpConversation();
        objectLostGenerator.SetUpConversation();
        thiefLocationGenerator.SetUpConversation();


        objectLostGenerator.OnGPTResponseReceived += (response) => objectLost = response;
        thiefLocationGenerator.OnGPTResponseReceived += (response) => thiefLocation = response;

        objectLostGenerator.SendMessage("Be creative, give me an object that can be stolen like:\n" +
                                        GetRandomExamplesFrom(ObjectPath));
        thiefLocationGenerator.SendMessage("Be creative, give me a location like:\n" +
                                           GetRandomExamplesFrom(LocationPath));
        while (objectLost == string.Empty || thiefLocation == string.Empty) await Task.Delay(50);
        Debug.LogError(objectLost);

        Scenario generatedScenario = new Scenario()
        {
            thiefDate = DateTime.Now - TimeSpan.FromHours(Random.Range(0f, 24)) - TimeSpan.FromDays(Random.Range(0f, 30)),
            objectLost = objectLost,
            thiefLocation = thiefLocation
        };
        string scenario = string.Empty;

        scenarioGenerator.OnGPTResponseReceived += (response) => scenario = response;
        scenarioGenerator.SendMessage("You must generate theft report for me:\n" +
                                      "the nature of the theft is " + objectLost + " theft.\n" +
                                      "the theft took place in" + thiefLocation + ".\n" +
                                      "the flight took place the " + generatedScenario.thiefDate.Date);
        while (scenario == string.Empty) await Task.Delay(50);
        generatedScenario.scenarioString = scenario;
        _onGenerated?.Invoke(generatedScenario);
    }
}
public struct Scenario
{
    public DateTime thiefDate;
    public string objectLost;
    public string thiefLocation;
    public string scenarioString;
}
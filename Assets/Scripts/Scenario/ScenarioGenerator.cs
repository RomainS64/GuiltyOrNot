using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Random = UnityEngine.Random;


public enum TheftType{
    Shoplifting,
    Burglary,
    TheftByDeception,
    AutoThief
}

public enum TestimonialHairType
{
    None,Short,Long,Any
}
public abstract class Testimonial
{
    public abstract string Prompt { get; }
    public string testimonialString;
}
public class HairTestimonial : Testimonial
{
    public override string Prompt => "without being too obvious, invent a testimony indicating that the criminal has" + (hairType == TestimonialHairType.Long? "long hair" : hairType == TestimonialHairType.Short?"short hair":"no hair");
    public TestimonialHairType hairType;
}
public class BeardTestimonial : Testimonial
{
    public override string Prompt => "without being too obvious, invent a testimony indicating that the criminal" + (haveBeard ? "has a beard" : "has no beard");
    public bool haveBeard;
}
public class BodyTestimonial : Testimonial
{
    public override string Prompt => "without being too obvious,invent a testimony indicating that the criminal is " + (isBig ? "heavy build" : "very light build");
    public bool isBig;
}
public class AgeTestimonial : Testimonial
{
    public override string Prompt => "without being too obvious,invent a testimony indicating that the criminal is " + (isOld ? "really old": "young");
    public bool isOld;
}
public class SizeTestimonial : Testimonial
{
    public override string Prompt => "without being too obvious,invent a testimony indicating that the criminal is " + (isTall ? "really tall" : "really short");
    public bool isTall;
}
public class ScenarioGenerator : MonoBehaviour
{
    [SerializeField] private GptGeneration scenarioGenerator;
    [SerializeField] private GptGeneration testimonial1Generator;
    [SerializeField] private GptGeneration testimonial2Generator;
    [SerializeField] private GptGeneration testimonial3Generator;
    
    public const string ShopliftingText = "Shoplifting";
    public const string BurglaryText = "Burglary";
    public const string TheftByDeceptionText = "Theft by deception";
    public const string AutoTheftText = "Auto theft";
    
    public const string LocationPath1 = "locations01";
    public const string LocationPath2 = "locations02";
    public const string LocationPath3 = "locations03";
    public const string LocationPath4 = "locations04";
    
    public const string ObjectPath = "objects";
    
    public const string DataFolder = "Data/";

    public List<int> generatedTestimonial = new();
    
    private static string GetRandomExamplesFrom(string _path)
    {
        TextAsset locations = Resources.Load<TextAsset>(DataFolder + _path);
        string[] allLocations = locations.text.Split("\n");
        string result = "examples:\n";
        for (int i = 0; i < 1; i++)
        {
            result += allLocations[Random.Range(0, allLocations.Length)] + "\n";
        }
        return result;
    }
    public async void GenerateScenario(Action<Scenario> _onGenerated)
    {

        TheftType generatedthiefType = GenereateThiefType();
        string objectLost = string.Empty;
        string thiefLocation = string.Empty;

        scenarioGenerator.SetUpConversation();
        testimonial1Generator.SetUpConversation();
        testimonial2Generator.SetUpConversation();
        testimonial3Generator.SetUpConversation();
        objectLost = GetRandomExamplesFrom(ObjectPath);
        
        string locationPath = "";
        switch (generatedthiefType)
        {
            case TheftType.Shoplifting:
                locationPath = LocationPath1;
                break;
            case TheftType.Burglary:
                locationPath = LocationPath2;
                break;
            case TheftType.TheftByDeception:
                locationPath = LocationPath3;
                break;
            case TheftType.AutoThief:
                locationPath = LocationPath4;
                break;
        }
        thiefLocation = GetRandomExamplesFrom(locationPath);
        while (objectLost == string.Empty || thiefLocation == string.Empty) await Task.Delay(50);

        generatedTestimonial = new();
        Scenario generatedScenario = new Scenario()
        {
            TheftType = generatedthiefType,
            thiefDate = DateTime.Now - TimeSpan.FromHours(Random.Range(0f, 24)) - TimeSpan.FromDays(Random.Range(0f, 30)),
            objectLost = objectLost,
            thiefLocation = thiefLocation,
            testimonial1 = GenerateTestimonial(),
            testimonial2 = GenerateTestimonial(),
            testimonial3 = GenerateTestimonial()
        };
        string scenario = string.Empty;
        testimonial1Generator.OnGPTResponseReceived +=
            (response) => generatedScenario.testimonial1.testimonialString = response;
        testimonial2Generator.OnGPTResponseReceived +=
            (response) => generatedScenario.testimonial2.testimonialString = response;
        testimonial3Generator.OnGPTResponseReceived +=
            (response) => generatedScenario.testimonial3.testimonialString = response;
        
        scenarioGenerator.OnGPTResponseReceived += (response) => scenario = response;
        
        
        scenarioGenerator.SendMessage("You must generate theft report for me:\n"+
                                      "the nature of the theft is a " + TheftTypeToString(generatedthiefType) + "\n" +
                                      (generatedthiefType == TheftType.AutoThief ? "" : "The object theft is "+ objectLost)+"\n"+
                                      "the theft took place in" + thiefLocation + ".\n" +
                                      "the flight took place the " + generatedScenario.thiefDate.Date+"\n");
        
        while (scenario == string.Empty) await Task.Delay(50);
        testimonial1Generator.SendMessage("you have to generate a first-person testimony:\n"+generatedScenario.testimonial1.Prompt +
                                          "\n the testimonial have to be linked to the scenario repport: "+scenario);
        testimonial2Generator.SendMessage("you have to generate a first-person testimony:\n"+generatedScenario.testimonial2.Prompt +  
                                          "\n the testimonial have to be linked to the scenario repport: "+scenario);
        testimonial3Generator.SendMessage("you have to generate a first-person testimony:\n"+generatedScenario.testimonial3.Prompt+ 
                                          "\n the testimonial have to be linked to the scenario repport: "+scenario);
        while (generatedScenario.testimonial1.testimonialString == String.Empty ||generatedScenario.testimonial2.testimonialString == String.Empty ||generatedScenario.testimonial3.testimonialString == String.Empty) await Task.Delay(50);

        generatedScenario.scenarioString = scenario;
        _onGenerated?.Invoke(generatedScenario);
    }

    private TheftType GenereateThiefType()=>(TheftType)Random.Range(0, 4);
    
    private Testimonial GenerateTestimonial()
    {
        int rdmChoice = Random.Range(0, 5);
        while (generatedTestimonial.Contains(rdmChoice))
        {
            rdmChoice = Random.Range(0, 5);
        }
        generatedTestimonial.Add(rdmChoice);
        Testimonial testimonial;
        switch (rdmChoice)
        {
            case 0:
                Debug.Log("Age selected");
                return GenerateAgeTestimonial();
            case 1:
                Debug.Log("Beard selected");
                return GenerateBeardTestimonial();
            case 2:
                Debug.Log("Body selected");
                return GenerateBodyTestimonial();
            case 3:
                Debug.Log("Hair selected");
                return GenerateHairTestimonial();
            case 4:
                Debug.Log("Size selected");
                return GenerateSizeTestimonial();
        }
        throw new Exception("PAS ICII");
    }

    private AgeTestimonial GenerateAgeTestimonial()
    {
        var testimonial = new AgeTestimonial();
        testimonial.isOld = Random.Range(0, 2) == 0;
        return testimonial;
    }
    private BeardTestimonial GenerateBeardTestimonial()
    {
        var testimonial = new BeardTestimonial();
        testimonial.haveBeard = Random.Range(0, 2) == 0;
        return testimonial;
    }
    private BodyTestimonial GenerateBodyTestimonial()
    {
        var testimonial = new BodyTestimonial();
        testimonial.isBig = Random.Range(0, 2) == 0;
        return testimonial;
    }
    private HairTestimonial GenerateHairTestimonial()
    {
        var testimonal = new HairTestimonial();
        testimonal.hairType = (TestimonialHairType)Random.Range(0, 3);
        return testimonal;
    }
    private SizeTestimonial GenerateSizeTestimonial()
    {
        var testimonal = new SizeTestimonial();
        testimonal.isTall = Random.Range(0, 2) == 0;
        return testimonal;
    }

    private string TheftTypeToString(TheftType _theftType)
    {
        switch (_theftType)
        {
            case TheftType.Shoplifting:
                return ShopliftingText;
            case TheftType.Burglary:
                return BurglaryText;
            case TheftType.TheftByDeception:
                return TheftByDeceptionText;
            case TheftType.AutoThief:
                return AutoTheftText;
        } 
        return String.Empty;
    }
}
public struct Scenario
{
    public DateTime thiefDate;
    public TheftType TheftType;
    
    public string objectLost;
    public string thiefLocation;
    public string scenarioString;
    
    public Testimonial testimonial1;
    public Testimonial testimonial2;
    public Testimonial testimonial3;
}
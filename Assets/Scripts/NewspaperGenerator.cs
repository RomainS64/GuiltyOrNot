using System;
using System.Collections;
using System.Collections.Generic;
using FMOD;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class NewspaperGenerator : MonoSingleton<NewspaperGenerator>
{
    private string titleprompt = "generate one funny newspaper title.";
    private string descriptionPrompt = "generate one funny paragraph from this title (like if you was the journalist):";

    private string mainTitlePrompt = "generate one funny newspaper title about an acused pearson, the theft was:";
    private string mainCatchPrompt = "generate one funny newspaper catchphrase subtitle about an acused pearson, the theft was: and the article title was: ";
    private string mainCitationPrompt = "generate one funny citation catchphrase from an acused pearson, the theft was: ";
    private string titleLink = " /and the article title was: ";
    [SerializeField] private GptGeneration news1GPT;
    [SerializeField] private GptGeneration news2GPT;
    [SerializeField] private GptGeneration news3GPT;
    [SerializeField] private GptGeneration news4GPT;
    [SerializeField] private GptGeneration news5GPT;
    [SerializeField] private GptGeneration news6GPT;
    [SerializeField] private GptGeneration description5GPT;
    [SerializeField] private GptGeneration description6GPT;

    [SerializeField] private TMP_Text news1Title;
    [SerializeField] private TMP_Text news2Title;
    [SerializeField] private TMP_Text news3Title;
    [SerializeField] private TMP_Text news4Title;
    [SerializeField] private TMP_Text description5;
    [SerializeField] private TMP_Text description6;

    [Space(10)] 
    [SerializeField] private GptGeneration goodTitle;
    [SerializeField] private GptGeneration goodCatch;
    [SerializeField] private GptGeneration goodCitation;
    [SerializeField] private GptGeneration badTitle;
    [SerializeField] private GptGeneration badCatch;
    [SerializeField] private GptGeneration badCitation;
    
    [SerializeField] private TMP_Text goodTitleText;
    [SerializeField] private TMP_Text goodCatchText;
    [SerializeField] private TMP_Text goodCitationText;
    [SerializeField] private TMP_Text badTitleText;
    [SerializeField] private TMP_Text badCatchText;
    [SerializeField] private TMP_Text badCitationText;
    [Space(10)] 
    [SerializeField] private Image news1Image;
    [SerializeField] private Image news2Image;
    [SerializeField] private Image news3Image;
    [SerializeField] private Image news4Image;
    [SerializeField] private Image news5Image;
    [SerializeField] private Image news6Image;

    private string currentScenarioString;
    private DallENews dalleE;

    protected override void Awake()
    {
        base.Awake();
        dalleE = GetComponent<DallENews>();
    }

    public void Generate(Scenario _scenario)
    {
        currentScenarioString = _scenario.scenarioString;
        news1GPT.SetUpConversation();
        news1GPT.SendMessage(titleprompt);
        news1GPT.OnGPTResponseReceived += OnNews1Recieved;
        news2GPT.SetUpConversation();
        news2GPT.SendMessage(titleprompt);
        news2GPT.OnGPTResponseReceived += OnNews2Recieved;
        news3GPT.SetUpConversation();
        news3GPT.SendMessage(titleprompt);
        news3GPT.OnGPTResponseReceived += OnNews3Recieved;
        news4GPT.SetUpConversation();
        news4GPT.SendMessage(titleprompt);
        news4GPT.OnGPTResponseReceived += OnNews4Recieved;
        news5GPT.SetUpConversation();
        news5GPT.SendMessage(titleprompt);
        news5GPT.OnGPTResponseReceived += OnNews5Recieved;
        news6GPT.SetUpConversation();
        news6GPT.SendMessage(titleprompt);
        news6GPT.OnGPTResponseReceived += OnNews6Recieved;

        goodTitle.SetUpConversation();
        goodTitle.SendMessage(mainTitlePrompt+currentScenarioString);
        goodTitle.OnGPTResponseReceived += OnGoodTitleRecieved;
        
        badTitle.SetUpConversation();
        badTitle.SendMessage(mainTitlePrompt+currentScenarioString);
        badTitle.OnGPTResponseReceived += OnBadTitleRecieved;

    }

    private void OnBadTitleRecieved(string _title)
    {
        badTitleText.text = _title;
        badCatch.SetUpConversation();
        badCatch.SendMessage(mainCatchPrompt+currentScenarioString+titleLink+_title);
        badCatch.OnGPTResponseReceived += OnBadCatchRRecieved;
        
        badCitation.SetUpConversation();
        badCitation.SendMessage(mainCitationPrompt+currentScenarioString+titleLink+_title);
        badCitation.OnGPTResponseReceived += OnBadCitationRecieved;
    }

    private void OnBadCitationRecieved(string _citation)
    {
        badCitationText.text = _citation;
    }

    private void OnBadCatchRRecieved(string _catch)
    {
        badCatchText.text = _catch;
    }

    private void OnGoodTitleRecieved(string _title)
    {
        goodTitleText.text = _title;
        
        goodCatch.SetUpConversation();
        goodCatch.SendMessage(mainCatchPrompt+currentScenarioString+titleLink+_title);
        goodCatch.OnGPTResponseReceived += OnGoodCatchRRecieved;
        
        goodCitation.SetUpConversation();
        goodCitation.SendMessage(mainCitationPrompt+currentScenarioString+titleLink+_title);
        goodCitation.OnGPTResponseReceived += OnGoodCitationRecieved;
    }
    private void OnGoodCitationRecieved(string _citation)
    {
        goodCitationText.text = _citation;
    }

    private void OnGoodCatchRRecieved(string _catch)
    {
        goodCatchText.text = _catch;
    }

    private void OnNews1Recieved(string _news)
    {
        news1Title.text = _news;
        dalleE.GenerateImage("Generate an image about this title:"+_news, list =>
        {
            news1Image.sprite = Sprite.Create((Texture2D)list[0], new Rect(0, 0, list[0].width, list[0].height), new Vector2(0.5f, 0.5f), 100.0f);
        } );
    }
    private void OnNews2Recieved(string _news)
    {
        news2Title.text = _news;
        dalleE.GenerateImage("Generate an image about this title:"+_news, list =>
        {
            news2Image.sprite = Sprite.Create((Texture2D)list[0], new Rect(0, 0, list[0].width, list[0].height), new Vector2(0.5f, 0.5f), 100.0f);
        } );
    }
    private void OnNews3Recieved(string _news)
    {
        news3Title.text = _news;
        dalleE.GenerateImage("Generate an image about this title:"+_news, list =>
        {
            news3Image.sprite = Sprite.Create((Texture2D)list[0], new Rect(0, 0, list[0].width, list[0].height), new Vector2(0.5f, 0.5f), 100.0f);
        } );
    }
    private void OnNews4Recieved(string _news)
    {
        news4Title.text = _news;
        dalleE.GenerateImage("Generate an image about this title:"+_news, list =>
        {
            news4Image.sprite = Sprite.Create((Texture2D)list[0], new Rect(0, 0, list[0].width, list[0].height), new Vector2(0.5f, 0.5f), 100.0f);
        } );
    }
    private void OnNews5Recieved(string _news)
    {
        description5GPT.SetUpConversation();
        description5GPT.SendMessage(descriptionPrompt+_news);
        description5GPT.OnGPTResponseReceived += OnDescription5Recieved;
        dalleE.GenerateImage("Generate an image about this title:"+_news, list =>
        {
            news5Image.sprite = Sprite.Create((Texture2D)list[0], new Rect(0, 0, list[0].width, list[0].height), new Vector2(0.5f, 0.5f), 100.0f);
        } );
    }

    

    private void OnNews6Recieved(string _news)
    {
        description6GPT.SetUpConversation();
        description6GPT.SendMessage(descriptionPrompt+_news);
        description6GPT.OnGPTResponseReceived += OnDescription6Recieved;
        dalleE.GenerateImage("Generate an image about this title:"+_news, list =>
        {
            news6Image.sprite = Sprite.Create((Texture2D)list[0], new Rect(0, 0, list[0].width, list[0].height), new Vector2(0.5f, 0.5f), 100.0f);
        } );
    }
    private void OnDescription5Recieved(string _desription)
    {
        description5.text = _desription;
    }
    private void OnDescription6Recieved(string _desription)
    {
        description6.text = _desription;
    }
}

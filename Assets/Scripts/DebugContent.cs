using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.U2D;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class DebugContent : MonoBehaviour
{
    [SerializeField] private bool DontLoadImages;
    [SerializeField] private Suspect suspect;
    private string prompt = "in caricature exaggerate face comics in stylized realistic digital art, a wowan with a yellow dress, a grey background behind";
    [SerializeField]
    private StableDiffusionText2Image background;
    [SerializeField]
    private SuspectVisualGenerator suspectGenerator;
    [SerializeField] 
    private GameObject loadingCanvas;
    [SerializeField] 
    private GameObject SelectionPart;
    [SerializeField]
    private Button yesButton,noButton;
    [SerializeField] 
    private GptNpc gptNpc;
    
    
    private bool isLoaded = false;
    private int currentDialogue = 0;
    private int currentGeneration = 0;

    public void DisplaySheet()
    {
        InfoSheet.Instance.DisplaySheet(
            suspect.name,
            suspect.surname,
            suspect.age,
            suspect.sexe,
            suspect.height,
            suspect.criminalRecord,
            suspect.psychologicalReport,
            suspect.medicalReport);
    }

    public void HideSheet()
    {
        InfoSheet.Instance.HideSheet();   
    }
    void Start()
    {
        gptNpc.OnGPTResponseReceived += SuspectRespondHandler;
        InfoSheet.Instance.HideSheet();
        if (DontLoadImages)
        {
            loadingCanvas.SetActive(false);
            gptNpc.SendMessage("Hello.");
            isLoaded = true;
        }
        else
        {
            LoadContent();
        }
        
        
        yesButton.onClick.AddListener(() =>
        {
            SelectionPart.SetActive(false);
            gptNpc.SendMessage("You are innocent.");
        });
        noButton.onClick.AddListener(() =>
        {
            SelectionPart.SetActive(false);
            gptNpc.SendMessage("You are guilty.");
        });
    }

    private void Update()
    {
        if (!isLoaded) return;
        if (Input.GetKey(KeyCode.Alpha1)) suspect.suspectImage.texture = suspect.emotions[EmotionType.Concentrated];
        if (Input.GetKey(KeyCode.Alpha2)) suspect.suspectImage.texture = suspect.emotions[EmotionType.Happy];
        if (Input.GetKey(KeyCode.Alpha3)) suspect.suspectImage.texture = suspect.emotions[EmotionType.Confused];
        if (Input.GetKey(KeyCode.Alpha4)) suspect.suspectImage.texture = suspect.emotions[EmotionType.Scared];
    }

    void GenerateImages()
    {
        background.OnGenerationDone+= (_)=>
        {
            suspectGenerator.GenerateSuspectFacesAsync(prompt, (sprites)=>
            {
                suspect.emotions = sprites;
                suspect.suspectImage.texture = suspect.emotions[EmotionType.Concentrated];
                Debug.Log("Is Loaded !");
                loadingCanvas.SetActive(false);
                isLoaded = true;
            });
        };
        background.Generate();
    }
    private async void SuspectRespondHandler(string _response)
    {
        while (!isLoaded)
        {
            await Task.Delay(50);
        }
        DialogueManager.Instance.DisplayDialogue(new Character(){guid = null,name = "Suspect"},_response,currentDialogue==0?PrepareChoice:null);
        currentDialogue++;
    }
    void LoadContent()
    {
        loadingCanvas.SetActive(true);
        gptNpc.SendMessage("Hello.");
        GenerateImages();
    }
    void PrepareChoice()
    {
        SelectionPart.SetActive(true);
    }
}

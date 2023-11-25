using System;
using System.Collections;
using System.Collections.Generic;
using BitSplash.AI.GPT;
using UnityEditor;
using UnityEngine;

public class GptGeneration : MonoBehaviour
{
    public Action<string> OnGPTResponseReceived;
    
    public string PromptDirection = "Answer as a helpful Ninjitsu master of stealth";
    public bool TrackConversation = false;
    public int MaximumTokens = 600;
    [Range(0f, 1f)]
    public float Temperature = 0f;
    [Range(0f, 1f)]
    public float Top_P = 0f;
    [Range(-2f, 2f)]
    public float Frequency_Penality = 0f;
    [Range(-2f, 2f)]
    public float Presence_Penality = 0f;
    ChatGPTConversation Conversation;
    
    public void SetUpConversation()
    {
        Conversation = ChatGPTConversation.Start(this)
            .MaximumLength(MaximumTokens)
            .SaveHistory(TrackConversation)
            .System(PromptDirection);
        Conversation.Temperature = Temperature;
        Conversation.Top_P = Top_P;
        Conversation.Frequency_Penalty = Frequency_Penality;
        Conversation.Presence_Penalty = Presence_Penality;
    }
    public void SendMessage(string _message)=>Conversation.Say(_message);
    private void OnConversationResponse(string _text)=>OnGPTResponseReceived?.Invoke(_text);
    private void OnConversationError(string _text)=>Debug.LogError("CONVERSATION DONT WORK");

}

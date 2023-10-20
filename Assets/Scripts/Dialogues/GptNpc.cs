using System;
using System.Collections;
using System.Collections.Generic;
using BitSplash.AI.GPT;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GptNpc : MonoBehaviour
{
    public Action<string> OnGPTResponseReceived;
    
    public string NpcDirection = "Answer as a helpful Ninjitsu master of stealth";
    public string[] Facts;
    public bool TrackConversation = false;
    public int MaximumTokens = 600;
    [Range(0f, 1f)]
    public float Temperature = 0f;
    ChatGPTConversation Conversation;

    private void Awake()
    {
        SetUpConversation();
    }

    void SetUpConversation()
    {
        Conversation = ChatGPTConversation.Start(this)
            .MaximumLength(MaximumTokens)
            .SaveHistory(TrackConversation)
            .System(string.Join("\n", Facts) + "\n" + NpcDirection);
        Conversation.Temperature = Temperature;
    }
    public void SendMessage(string _message)
    {
        Conversation.Say(_message);
    }
    
    void OnConversationResponse(string _text)
    {
        OnGPTResponseReceived?.Invoke(_text);
    }
    void OnConversationError(string _text)
    {
        Debug.LogError("CONVERSATION DONT WORK");
    }
    
}

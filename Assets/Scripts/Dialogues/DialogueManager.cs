using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DialogueManager : MonoSingleton<DialogueManager>
{
    [SerializeField]
    private TMP_Text name;
    [SerializeField]
    private TMP_Text dialogue;
    
    
    private const float dialogueSpeedFactor = 0.035f;
    private IEnumerator dialogueCoroutine = null;

    public bool IsDisplaying { get; private set; }
    public void DisplayDialogue(Character _speakingChacter,string _dialogue,Action _OnDialogueFinished = null,float _dialogueSpeed = 1f)
    {
        if (dialogueCoroutine != null)
        {
            Debug.LogWarning("Trying to start a dialogue, but an other is already running");
        }

        name.text = _speakingChacter.name;
        dialogue.text = String.Empty;
        dialogueCoroutine = DisplayDialogueCoroutine(_speakingChacter, _dialogue, _OnDialogueFinished, _dialogueSpeed);
        StartCoroutine(dialogueCoroutine);
    }
    private IEnumerator DisplayDialogueCoroutine(Character speakingChaacter, string _dialogue,Action _OnDialogueFinished = null, float _dialogueSpeed = 1f)
    {
        IsDisplaying = true;
        foreach (char character in _dialogue)
        {
            dialogue.text += character;
            yield return new WaitForSeconds(_dialogueSpeed*dialogueSpeedFactor);
        }
        _OnDialogueFinished?.Invoke();
        IsDisplaying = false;
        dialogueCoroutine = null;
    }

    private void HideDialogueBox()
    {
        name.text = String.Empty;
        dialogue.text = String.Empty;
    }
}

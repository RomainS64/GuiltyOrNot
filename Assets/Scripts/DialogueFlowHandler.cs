using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueFlowHandler : MonoSingleton<DialogueFlowHandler>
{
    [SerializeField] private GameObject _dialogueCanvas;
    public void StartDialogue(Suspect _suspect)
    {
        _dialogueCanvas.SetActive(true);
    }

    public void CloseDialogue()
    {
        _dialogueCanvas.SetActive(false);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour {

    // For playing au FMod event in every scripts : AudioManager.instance.audioEvents["Audio Event Name"].Play();


    public static AudioManager instance;
    private void Awake()
    {
        if (instance != null)
        {
            Debug.LogWarning("More than one AudioManager in scene");
            return;
        }
        
        instance = this;

        InitializeDictionary();
    }

    public Dictionary<string, AudioEvent> audioEvents = new Dictionary<string, AudioEvent>();

    public AudioEvent[] fmodEventsList;

    private void InitializeDictionary()
    {
        for (int i = 0; i < fmodEventsList.Length; i++)
        {
            audioEvents.Add(
                fmodEventsList[i].Name,
                new AudioEvent {
                    Name = fmodEventsList[i].Name,
                    fmodEvent = fmodEventsList[i].fmodEvent
                }
            );
        }
    }

    [System.Serializable]
    public class AudioEvent
    {
        public string Name;

        public EventReference fmodEvent;

        public void Play(bool debugMessage = true)
        {
            RuntimeManager.PlayOneShot(fmodEvent);
            if (debugMessage)
            {
                Debug.Log("FMod played : " + fmodEvent.Path.Remove(0, 7));
            }
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMODUnity;

public class AudioManager : MonoBehaviour {

    // For playing an FMod event : AudioManager.instance.audioEvents["Audio Event Name"].Play();

    // For stopping an FMod event : AudioManager.instance.audioEvents["Audio Event Name"].Stop();

    // For modify a parameter : AudioManager.instance.audioEvents["Audio Event Name"].SetParameter("parameter_name", value);

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
                    fmodEventEmitter = fmodEventsList[i].fmodEventEmitter
                }
            );
        }
    }

    public void SetParameter(string name, float value, bool debugMessage = false)
    {
        RuntimeManager.StudioSystem.setParameterByName(name, value, false);

        if (debugMessage)
        {
            Debug.Log("FMod param : " + name + " set to : " + value);
        }
    }

    public float GetParameter(string name)
    {
        float value;
        RuntimeManager.StudioSystem.getParameterByName(name, out value);
        return value;
    }

    


    [System.Serializable]
    public class AudioEvent
    {
        public string Name;

        public StudioEventEmitter fmodEventEmitter;

        public void Play(bool debugMessage = false)
        {
            fmodEventEmitter.Play();

            if (debugMessage)
            {
                Debug.Log("FMod played : " + fmodEventEmitter.name);
            }
        }

        public void Stop(bool debugMessage = false)
        {
            fmodEventEmitter.Stop();

            if (debugMessage)
            {
                Debug.Log("FMod stopped : " + fmodEventEmitter.name);
            }
        }
    }
}

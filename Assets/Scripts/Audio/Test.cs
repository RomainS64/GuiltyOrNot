using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public float paramValue = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            AudioManager.instance.audioEvents["Pin"].Play();
        }
        if (Input.GetKeyDown(KeyCode.T))
        {
            AudioManager.instance.audioEvents["Game Music"].Play();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            AudioManager.instance.audioEvents["Game Music"].Stop();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            paramValue--;
            AudioManager.instance.SetParameter("music_state", paramValue);
            Debug.Log(AudioManager.instance.GetParameter("music_state"));
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            paramValue++;
            AudioManager.instance.SetParameter("music_state", paramValue);
            Debug.Log(AudioManager.instance.GetParameter("music_state"));
        }
    }
}

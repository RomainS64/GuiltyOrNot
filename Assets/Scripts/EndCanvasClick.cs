using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class EndCanvasClick : MonoBehaviour,IPointerClickHandler
{
    
    public void OnPointerClick(PointerEventData _eventData)
    {
        SceneManager.LoadScene(0);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;


public class PhotoSceneSwith : MonoBehaviour, IPointerClickHandler
{
    
    public bool nextSceneIsPinable = true;
    public int LinkedScene;

    private CrossPhoto crossPhoto = null;
    private Notebook notebook;

    private void Start()
    {
        notebook = FindObjectOfType<Notebook>();
         TryGetComponent(out crossPhoto);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left || (crossPhoto != null  && crossPhoto.IsCrossed))return;
        
        SceneCameraManager.Instance.LoadScene(LinkedScene);
        PlacePin.Instance.SetPlacePinMode(nextSceneIsPinable);
        notebook?.HideNotebook(0.1f);
    }
}

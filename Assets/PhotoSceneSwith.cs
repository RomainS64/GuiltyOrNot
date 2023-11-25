using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PhotoSceneSwith : MonoBehaviour, IPointerClickHandler
{

    public bool nextSceneIsPinable = true;
    public int LinkedScene;
    
    public void OnPointerClick(PointerEventData eventData)
    {
        SceneCameraManager.Instance.LoadScene(LinkedScene);
        PlacePin.Instance.SetPlacePinMode(nextSceneIsPinable);
    }
}

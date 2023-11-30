using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class FolderButton : MonoBehaviour,IPointerDownHandler
{
    [SerializeField] private SuspectOverviewHandler suspectOverview;

    public void OnPointerDown(PointerEventData eventData)
    {
        suspectOverview.OnFolderClicked();
    }
}

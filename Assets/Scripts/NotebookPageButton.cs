using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class NotebookPageButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private bool isNextPage;
    private Notebook notebook;

    private void Start()
    {
        notebook = GetComponentInParent<Notebook>();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (isNextPage)
        {
            notebook.ShowNextPage();
        }
        else
        {
            notebook.ShowPreviousPage();
        }
    }
}

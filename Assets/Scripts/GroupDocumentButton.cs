using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class GroupDocumentButton : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private GameObject suspectsIcon;
    [SerializeField] private GameObject idCardIcon;
    [SerializeField] private GameObject bankAccountIcon;
    [SerializeField] private GameObject criminalRecordIcon;
    [SerializeField] private GameObject internetHistoryIcon;
    [SerializeField] private GameObject scriptIcon;

    private void Start()
    {
        LastTouchedDocument.Instance.OnSelectedDocumentChanged += RefreshIcons;
    }

    private void RefreshIcons(DocumentType _doc)
    {
        SetIcons(_doc,DocumentPlacement.Instance.IsGroupedByDocument);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        DocumentType lastDocumentTouched = LastTouchedDocument.Instance.GetLastDocumentTouched();
        if(LastTouchedDocument.Instance.GetLastDocumentTouched() == DocumentType.None)return;
        
        if (!DocumentPlacement.Instance.IsGroupedByDocument)
        {
            DocumentPlacement.Instance.GroupByDocuments((int)lastDocumentTouched);

        }
        else
        {
            DocumentPlacement.Instance.GroupBySuspect();
        }
        SetIcons(lastDocumentTouched,DocumentPlacement.Instance.IsGroupedByDocument);
        
    }

    public void SetIcons(DocumentType _currentDocumentSelected, bool isGroupedByDocument)
    {
        suspectsIcon.SetActive(isGroupedByDocument && _currentDocumentSelected != DocumentType.None);
        idCardIcon.SetActive(!isGroupedByDocument && _currentDocumentSelected == DocumentType.IDCard);
        internetHistoryIcon.SetActive(!isGroupedByDocument && _currentDocumentSelected == DocumentType.InternetHistory);
        bankAccountIcon.SetActive(!isGroupedByDocument && _currentDocumentSelected == DocumentType.BankAccount);
        criminalRecordIcon.SetActive(!isGroupedByDocument && _currentDocumentSelected == DocumentType.CriminalRecord);
        scriptIcon.SetActive(!isGroupedByDocument && _currentDocumentSelected == DocumentType.Script);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = UnityEngine.Random;


[Serializable]
public struct NotebookPage
{
     public GameObject pageObject;
}
public class Notebook : MonoBehaviour
{
    [SerializeField] private NotebookPage[] pages;
    [SerializeField] private RectTransform notDisplayedPosition;
    [SerializeField] private RectTransform startDisplayedPosition;
    private int pageIndex = 0;
    private ClickableObject clickable;
    private MovableObject movableObject;
    private HoverObject hover;
    private IEnumerator moveObjectCoroutine;
    private bool isDisplayed = false;
    private string clickParameter = "click";
    private RectTransform rect;
    private void Awake()
    {
        rect = GetComponent<RectTransform>();
        clickable = GetComponent<ClickableObject>();
        hover = GetComponent<HoverObject>();
        movableObject = GetComponent<MovableObject>();
        clickable.OnAssetClicked += OnPointerDown;
        transform.position = notDisplayedPosition.position;
        ShowPage(pageIndex);
    }
    public void OnPointerDown()
    {
        if (!isDisplayed)
        {
            ShowNotebook();
        }
    }
    
    public void ShowNextPage()
    {
        if (pageIndex == pages.Length - 1)
        {
            HideNotebook();
            return;
        }
        ShowPage(++pageIndex);
    }

    public void ShowPreviousPage()
    {
        if (pageIndex == 0)
        {
            HideNotebook();
            return;
        }
        ShowPage(--pageIndex);
    }

    private void ShowPage(int _index)
    {
        
        AudioManager.instance.audioEvents[Random.Range(0,2)==0?"Object Grab":"Object Release"].Play();
        for (int i = 0; i < pages.Length; ++i)
        {
            pages[i].pageObject.SetActive(_index == i);
        }
    }

    public void ShowNotebook(float _speed = 0.3f)
    {
        isDisplayed = true;
        clickable.IsLock = false;
        hover.IsLock = true;
        movableObject.Lock(false);
        if (moveObjectCoroutine != null)
        {
            StopCoroutine(moveObjectCoroutine);
        }
        moveObjectCoroutine = MoveObject(
            notDisplayedPosition.anchoredPosition,
            startDisplayedPosition.anchoredPosition,_speed);
        StartCoroutine(moveObjectCoroutine);
    }
    public void HideNotebook(float _speed = 0.3f)
    {
        isDisplayed = false;
        clickable.IsLock = false;
        hover.IsLock = false;
        movableObject.Lock(true);
        if (moveObjectCoroutine != null)
        {
            StopCoroutine(moveObjectCoroutine);
        }
        moveObjectCoroutine = MoveObject(
            rect.anchoredPosition,
            notDisplayedPosition.anchoredPosition,_speed);
        StartCoroutine(moveObjectCoroutine);
    }
    IEnumerator MoveObject(Vector2 startPos, Vector2 endPos, float moveTime)
    {
        float elapsedTime = 0f;

        while (elapsedTime < moveTime)
        {
            rect.anchoredPosition = Vector3.Lerp(startPos, endPos, elapsedTime / moveTime);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        rect.anchoredPosition = endPos;
    }
    
}

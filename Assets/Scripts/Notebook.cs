using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
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
    [SerializeField] private TMP_Text scenario;
    [SerializeField] private TMP_Text testimonial1;
    [SerializeField] private TMP_Text testimonial2;
    [SerializeField] private TMP_Text testimonial3;
    
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
        HideNotebook();
        //ScenarioFlow.OnGameStart += () => { ShowNotebook(); };
    }

    public void SetScenario(string _scenario) => scenario.text = _scenario;
    public void SetTestimonial1(string _testimonial) => testimonial1.text = _testimonial;
    public void SetTestimonial2(string _testimonial) => testimonial2.text = _testimonial;
    public void SetTestimonial3(string _testimonial) => testimonial3.text = _testimonial;
    
    public void OnPointerDown()
    {
        TutoBehaviour.NotebookClicked?.Invoke();
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

    public int suspectIndex = 2;
    private void ShowPage(int _index)
    {
        if(_index == suspectIndex)TutoBehaviour.SuspectPageSelected?.Invoke();
        if(AudioManager.instance != null)
        {
            AudioManager.instance.audioEvents[Random.Range(0,2)==0?"Object Grab":"Object Release"].Play();
        }
        
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

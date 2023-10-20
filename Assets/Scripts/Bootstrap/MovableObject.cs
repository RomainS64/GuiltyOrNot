using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class MovableObject : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private int maxX,minX,maxY,minY;
    [SerializeField] private float unknownFactor;
    [SerializeField] private RectTransform canvas;
    private bool isMoving;
    private Vector2 firstMousePos;
    private Vector2 firstObjectPos;

    private Animator animator;
    private RectTransform objectTransform;
    
    private const string holdParameter = "hold";

    private void Awake()
    {
        objectTransform = GetComponent<RectTransform>();
        animator = GetComponent<Animator>();
    }

    public void OnPointerDown(PointerEventData _eventData)
    {
        animator.SetBool(holdParameter,true);
        isMoving = true;
        firstMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        firstObjectPos = objectTransform.anchoredPosition;
    }
    
    public void OnPointerUp(PointerEventData _eventData)
    {
        animator.SetBool(holdParameter,false);
        isMoving = false;
    }

    public void Update()
    {
        if (!isMoving) return;
        
        Vector2 currentPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 delta = firstMousePos - currentPos;
        Vector2 newPos = firstObjectPos - (delta * unknownFactor);
        
        newPos.x = newPos.x > maxX ? maxX : newPos.x;
        newPos.x = newPos.x < minX ? minX : newPos.x;
        newPos.y = newPos.y > maxY ? maxY : newPos.y;
        newPos.y = newPos.y < minY ? minY : newPos.y;
        
        objectTransform.anchoredPosition = newPos;
    }
}

using System;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class MovableObject : MonoBehaviour, IPointerDownHandler,IPointerUpHandler
{
    public static Action OnSiblingChanged;
    [HideInInspector][CanBeNull] public GameObject linkedIdCard;
    [HideInInspector]public int ropeId = -1;
    
    [SerializeField] private DocumentType documentType;
    [SerializeField] private float unknownFactorX;
    [SerializeField] private float unknownFactorY;
    [SerializeField] private float zoomMaxMultiplicator;
    [SerializeField] private float maxXPos = 33;
    [SerializeField] private float minXPos = -28;
    [SerializeField] private float maxYPos = 21;
    [SerializeField] private float minYPos = -13;
    private bool isMoving;
    private Vector2 firstMousePos;
    private Vector2 firstObjectPos;
    
    private Animator animator;
    private RectTransform objectTransform;
    private const string holdParameter = "hold";

    private bool isLock = false;

    public void Lock(bool _lock)=>isLock = _lock;


    private void Awake()
    {
        if (TryGetComponent(out RectTransform rectTransform))
        {
            objectTransform = rectTransform;
        }
        else
        {
            objectTransform = GetComponentInChildren<RectTransform>();
        }
        animator = GetComponent<Animator>();
    }
    

    public void OnPointerDown(PointerEventData _eventData)
    {
        if (isLock) return;
        LastTouchedDocument.Instance.NotifyTouchedDocument(documentType);
        
        transform.SetAsLastSibling();
        OnSiblingChanged?.Invoke();
        animator.SetBool(holdParameter,true);
        AudioManager.instance.audioEvents["Object Grab"].Play();
        isMoving = true;
        firstMousePos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        firstObjectPos = objectTransform.anchoredPosition;
    }
    
    public void OnPointerUp(PointerEventData _eventData)
    {
        if (isLock) return;
        animator.SetBool(holdParameter,false);
        AudioManager.instance.audioEvents["Object Release"].Play();
        
        isMoving = false;
    }

    public void Update()
    {
        if (!isMoving) return;
        float zoomMultiplicator = Mathf.Lerp(1, zoomMaxMultiplicator, CameraHandler.Instance.ZoomLevel);
        Vector2 currentPos = Camera.main.ScreenToViewportPoint(Input.mousePosition);
        Vector2 delta = firstMousePos - currentPos;
        Vector2 newPos = 
            new Vector2(firstObjectPos.x -(delta.x * unknownFactorX * zoomMultiplicator),firstObjectPos.y - (delta.y * unknownFactorY * zoomMultiplicator));

        if (newPos.x > maxXPos) newPos.x = maxXPos;
        if (newPos.y > maxYPos) newPos.y = maxYPos;
        if (newPos.x < minXPos) newPos.x = minXPos;
        if (newPos.y < minYPos) newPos.y = minYPos;
        objectTransform.anchoredPosition = newPos;
        
    }
}

using System;
using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.EventSystems;

[Serializable]
public struct Limits
{
    public Transform limTop;
    public Transform limRight;
    public Transform limLeft;
    public Transform limBottom;
}
public enum LimitsState
{
    InBounds,More,Less
};
public class CameraHandler : MonoSingleton<CameraHandler>,IPointerDownHandler,IPointerUpHandler
{
    [SerializeField] private bool toggleOnStart;
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private float mouvMultiplicator = 1f;
    [SerializeField] float scrollMultiplicator = 1f;    
    [SerializeField] private CinemachineVirtualCamera camera;
    [SerializeField] private float minZoom = 1f;
    [SerializeField] private float maxZoom = 10f;
    [SerializeField] private Limits minLimit;
    [SerializeField] private Limits maxLimit;
    private IEnumerator movingBehaviour = null;
    private IEnumerator scrollingBehaviour = null;

    public float ZoomLevel { get; private set; } = 0;

    private bool isCameraMoving = false;

    private void Start()
    {
        ToggleCameraMovement(toggleOnStart);
    }

    public void ToggleCameraMovement(bool _toggle)
    {
        isCameraMoving = _toggle;
        if (isCameraMoving)
        {
            StartScrolling();
        }
        else
        {
            StopScrolling();
        }
    }
    private void StartMoving()
    {
        if (movingBehaviour != null)
        {
            StopCoroutine(movingBehaviour);
        }
        movingBehaviour = MovingBehaviour();
        StartCoroutine(movingBehaviour);
    }

    private void StopMoving()
    {
        if (movingBehaviour != null)
        {
            StopCoroutine(movingBehaviour);
        }
        movingBehaviour = null;
    }

    private void StartScrolling()
    {
        if (scrollingBehaviour != null)
        {
            StopCoroutine(scrollingBehaviour);
        }
        scrollingBehaviour = ScrollingBehaviour();
        StartCoroutine(scrollingBehaviour);
    }

    private void StopScrolling()
    {
        if (scrollingBehaviour != null)
        {
            StopCoroutine(scrollingBehaviour);
        }
        scrollingBehaviour = null;
    }
    

    private IEnumerator MovingBehaviour()
    {
        Vector2 prevMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 nextMousePos = prevMousePos;
        while (true)
        {
            nextMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 delta = prevMousePos - nextMousePos;
            LimitsState horizontalState = IsInHorizontalBounds();
            LimitsState verticalState = IsInVerticalBounds();
            delta.x = (horizontalState == LimitsState.Less && delta.x<0) || (horizontalState == LimitsState.More && delta.x>0) ? 0 : delta.x;
            delta.y = (verticalState == LimitsState.Less && delta.y<0) || (verticalState == LimitsState.More && delta.y>0) ? 0 : delta.y;
            cameraTarget.position += (Vector3)delta * mouvMultiplicator;
            yield return new WaitForFixedUpdate();
            prevMousePos = nextMousePos;
        }
    }
    private IEnumerator ScrollingBehaviour()
    {
        while (true)
        {
            if ((Input.mouseScrollDelta.y > 0 && camera.m_Lens.OrthographicSize < maxZoom) ||
                (Input.mouseScrollDelta.y < 0 && camera.m_Lens.OrthographicSize > minZoom))
            {
                camera.m_Lens.OrthographicSize += Input.mouseScrollDelta.y * Time.deltaTime * scrollMultiplicator;
                RecalibrateCamera();
            }

            yield return null;
        }
    }
    private void RecalibrateCamera()
    {
        ZoomLevel = Mathf.InverseLerp(minZoom, maxZoom, camera.m_Lens.OrthographicSize);
        Vector3 limitTop = Vector3.Lerp(minLimit.limTop.position,maxLimit.limTop.position, ZoomLevel);
        Vector3 limitBottom = Vector3.Lerp(minLimit.limBottom.position,maxLimit.limBottom.position, ZoomLevel);
        Vector3 limitRight = Vector3.Lerp(minLimit.limRight.position,maxLimit.limRight.position, ZoomLevel);
        Vector3 limitLeft = Vector3.Lerp(minLimit.limLeft.position,maxLimit.limLeft.position, ZoomLevel);
        Vector3 cameraPosition = cameraTarget.transform.position;
        if (cameraPosition.y > limitTop.y) cameraPosition.y = limitTop.y;
        else if (cameraPosition.y < limitBottom.y) cameraPosition.y = limitBottom.y;
        
        if (cameraPosition.x > limitRight.x) cameraPosition.x = limitRight.x;
        else if (cameraPosition.x < limitLeft.x) cameraPosition.x = limitLeft.x;

        cameraTarget.position = cameraPosition;

    }
    private LimitsState IsInVerticalBounds()
    {
        ZoomLevel = Mathf.InverseLerp(minZoom, maxZoom, camera.m_Lens.OrthographicSize);
        
        Vector3 limTop = Vector3.Lerp(minLimit.limTop.position,maxLimit.limTop.position, ZoomLevel);
        Vector3 limBottom = Vector3.Lerp(minLimit.limBottom.position,maxLimit.limBottom.position, ZoomLevel);
        Vector3 cameraPos = cameraTarget.position;
        if (cameraPos.y > limTop.y) return LimitsState.More;
        if (cameraPos.y < limBottom.y) return LimitsState.Less;
        return LimitsState.InBounds;
    }
    private LimitsState IsInHorizontalBounds()
    {
        ZoomLevel = Mathf.InverseLerp(minZoom, maxZoom, camera.m_Lens.OrthographicSize);
        Vector3 limRight = Vector3.Lerp(minLimit.limRight.position,maxLimit.limRight.position, ZoomLevel);
        Vector3 limLeft = Vector3.Lerp(minLimit.limLeft.position,maxLimit.limLeft.position, ZoomLevel);
        Vector3 cameraPos = cameraTarget.position;
        if (cameraPos.x > limRight.x) return LimitsState.More;
        if (cameraPos.x < limLeft.x) return LimitsState.Less;
        return LimitsState.InBounds;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!isCameraMoving) return;
        StartMoving();
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!isCameraMoving) return;
        StopMoving();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!isCameraMoving) return;
        StartScrolling();
        
    }
    
    public void OnPointerExit(PointerEventData eventData)
    {
        if (!isCameraMoving) return;
        StopScrolling();
    }
}

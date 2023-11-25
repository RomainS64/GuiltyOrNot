using System;
using System.Collections.Generic;
using Cinemachine;
using UnityEngine;


[Serializable]
public struct CanvasScene
{
    public GameObject worldCanvas;
    public GameObject screenCanvas;
}
public class SceneCameraManager : MonoSingleton<SceneCameraManager>
{
    [SerializeField] private CinemachineVirtualCamera virtualCam;
    [SerializeField] private List<CanvasScene> scenes;

    private CanvasScene currentCanvasScene;

    public void LoadScene(int _sceneIndex)
    {
        if (currentCanvasScene.screenCanvas != null)currentCanvasScene.screenCanvas.SetActive(false);
        currentCanvasScene = scenes[_sceneIndex];
        virtualCam.Follow = currentCanvasScene.worldCanvas.transform;
        if (currentCanvasScene.screenCanvas != null)currentCanvasScene.screenCanvas.SetActive(true);
    }
}

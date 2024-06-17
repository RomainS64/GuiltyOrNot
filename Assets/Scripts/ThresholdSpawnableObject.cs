using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;

public class ThresholdSpawnableObject : MonoBehaviour
{
    [SerializeField] private Transform pinPosition;
    [SerializeField] private int spawnThreshold;
    [SerializeField] private int playerId;

    private IDCard linkedIDCard;
    private int ropeId = -1;
    public int GetPlayerId() => playerId;

    private void Start()
    {
        linkedIDCard = FindObjectsOfType<IDCard>(true).First(paper => paper.GetPlayerId() == playerId);
        if (TryGetComponent(out MovableObject movableObject))
        {
            movableObject.linkedIdCard = linkedIDCard.gameObject;
        }
    }

    public void NotifyThreshold(int _amount,int _forceThreshold,bool _isEliminated)
    {
        //bool keepFile = (_isEliminated && spawnThreshold >= _forceThreshold) || (!_isEliminated && spawnThreshold>=_amount);
        bool keepFile = (!_isEliminated && spawnThreshold >= _amount);
        if (!keepFile && TryGetComponent(out PinableObject pinable))
        {
            foreach (int id in pinable.PinIds)
            {
                PlacePin.Instance.RemovePinsAndRope(id);
            }

            pinable.PinIds = new List<int>();
        }
        if (!keepFile && ropeId != -1)
        {
            PlacePin.Instance.ShowPinsAndRope(ropeId,false);
        }

        if (keepFile && !gameObject.activeSelf)
        {
            if (spawnThreshold < 10)
            {
                if (ropeId != -1)
                {
                    PlacePin.Instance.ShowPinsAndRope(ropeId,true);
                }
                else
                {
                    ropeId =PlacePin.Instance.PlaceLink(transform,linkedIDCard.transform,pinPosition,linkedIDCard.PinPosition);   
                    if (TryGetComponent(out MovableObject movableObject))
                    {
                        movableObject.ropeId = ropeId;
                    }
                }
            }
            
        }
        gameObject.SetActive(keepFile);
        linkedIDCard.transform.SetAsLastSibling();
        MovableObject.OnSiblingChanged?.Invoke();
    }
}

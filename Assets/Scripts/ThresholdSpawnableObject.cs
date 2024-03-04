using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ThresholdSpawnableObject : MonoBehaviour
{
    [SerializeField] private int spawnThreshold;
    [SerializeField] private int playerId;
    public int GetPlayerId() => playerId;
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
        gameObject.SetActive(keepFile);
    }
}

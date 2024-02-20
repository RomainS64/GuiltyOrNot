using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IDCard : MonoBehaviour
{
    [SerializeField] private int suspectId;
    public int GetPlayerId() => suspectId;
    
    [SerializeField] private PhotoSetter picture;
    [SerializeField] private TextSetter name,surname,gender,size,date;
    void Start()
    {
        picture.SetPlayerId(suspectId);
        name.SetPlayerId(suspectId);
        surname.SetPlayerId(suspectId);
        gender.SetPlayerId(suspectId);
        size.SetPlayerId(suspectId);
        date.SetPlayerId(suspectId);
    }

}

using TMPro;
using UnityEngine;


public enum TextType
{
    Date,Firstname,Surname,Size,Age,Gender
}
public class TextSetter : MonoBehaviour
{
    [SerializeField] private int playerId;
    [SerializeField] private TextType setterType;
    [SerializeField] private TMP_Text text;

    public TextType GeSetterType() => setterType;
    public int GetPlayerId() => playerId;
    public void SetPlayerId(int id) => playerId = id;
    
    public void SetText(string _text)
    {
        text.text = _text;
    }
}

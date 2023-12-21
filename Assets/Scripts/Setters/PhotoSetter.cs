using UnityEngine;
using UnityEngine.UI;

public class PhotoSetter : MonoBehaviour
{
    
    [SerializeField] private int playerId;
    public int GetPlayerId() => playerId;
    public void SetPlayerId(int id)=>playerId = id;
    [SerializeField] private RawImage photo;
    public void SetPhoto(Texture _photo)
    {
        photo.texture = _photo;
    }
}

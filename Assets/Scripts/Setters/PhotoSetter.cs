using UnityEngine;
using UnityEngine.UI;

public class PhotoSetter : MonoBehaviour
{
    
    [SerializeField] private int playerId;
    public int GetPlayerId() => playerId;
    public void SetPlayerId(int id)=>playerId = id;
    [SerializeField] private RawImage rawPhoto;
    [SerializeField] private Image photo;
    public void SetPhoto(Texture _photo)
    {
        if (photo != null)
        {
            photo.sprite = Sprite.Create((Texture2D)_photo, new Rect(0, 0, _photo.width, _photo.height), new Vector2(0.5f, 0.5f), 100.0f);
        }

        if (rawPhoto != null)
        {
            rawPhoto.texture = _photo;
        }
        
    }
}

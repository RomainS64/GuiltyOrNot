using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class debug : MonoBehaviour
{
    [SerializeField] private DallESuspectVisualGenerator visualGenerator;
    [SerializeField] private Material image;
    public void OnClick()
    {
        Suspect suspect = SuspectGenerator.Instance.GenerateSuspect(8);
        visualGenerator.GenerateSuspectFaceAsync(suspect,EmotionType.Concentrated,true,OnGenerated);
    }

    private void OnGenerated((Suspect, Texture) obj)
    {
        image.mainTexture = obj.Item2;
    }
}

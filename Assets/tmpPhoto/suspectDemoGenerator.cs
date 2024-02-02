using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class suspectDemoGenerator : MonoBehaviour
{
    [SerializeField] private RawImage image;
    [SerializeField] private Texture loadingImage;
    [SerializeField] private SuspectVisualGenerator suspectGenerator;
    [SerializeField] private TMP_Text name;
    [SerializeField] private TMP_Text surname;
    [SerializeField] private TMP_Text age;
    [SerializeField] private TMP_Text size;
    [SerializeField] private TMP_Text gender;

    private bool isGenerating = false;
    private Suspect generatedSuspect;
    public void Generate()
    {
        if (isGenerating) return;
        image.texture = loadingImage;
        name.text = "";
        surname.text = "";
        age.text = "";
        size.text = "";
        gender.text = "";
        isGenerating = true;
        generatedSuspect = SuspectGenerator.Instance.GenerateSuspect(0);
        StartCoroutine(GenerateSuspect());
    }
    private IEnumerator GenerateSuspect()
    {
        
        suspectGenerator.GenerateSuspectFaceAsync(generatedSuspect,EmotionType.Concentrated,true,(_result =>
        {
            generatedSuspect.portrait = _result.Item2;
            image.texture = _result.Item2;
            name.text = generatedSuspect.name;
            surname.text = generatedSuspect.surname;
            age.text = generatedSuspect.age + " yo";
            size.text = generatedSuspect.height + " cm";
            gender.text = generatedSuspect.gender == "Male" ? "M" : "F";
            isGenerating = false;
        }));
        yield return new WaitWhile(() => isGenerating);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class CorkBoardFlowHandler : MonoSingleton<CorkBoardFlowHandler>
{
    [SerializeField] private int firstSuspectSceneIndex;
    [SerializeField] private Transform BoardCanvas;
    [SerializeField] private ScenarioView scenarioSheet;
    [SerializeField] private GameObject suspectPhotoPrefab;
    [SerializeField] private Transform[] photoPositions;
    private PhotoSetter[] photoSetters; 
    private TextSetter[] firstnameSetters;
    private TextSetter[] surnameSetters;
    private TextSetter[] sizeSetters;
    private TextSetter[] dateSetters;
    private TextSetter[] genderSetters;
    public void StartCorkBoard(Scenario _scenario,Suspect[] _suspects)
    {
        Debug.Log("StartCorkBoard");
        scenarioSheet.DisplayText(_scenario.scenarioString);
        int i = 0;
        TextSetter[] allTextSetters = FindObjectsOfType<TextSetter>(true);
        PhotoSetter[] photoSetters = FindObjectsOfType<PhotoSetter>(true);
            
        dateSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Date).ToArray();
        sizeSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Size).ToArray();
        genderSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Gender).ToArray();
        surnameSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Surname).ToArray();
        firstnameSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Firstname).ToArray();
        foreach (Suspect suspect in _suspects)
        {
            Vector3 position = photoPositions[i].position;
            GameObject newPhoto = Instantiate(suspectPhotoPrefab,position,Quaternion.identity);
            newPhoto.transform.parent = BoardCanvas;
            newPhoto.GetComponent<PhotoSetter>().SetPhoto(suspect.emotions[EmotionType.Concentrated]);
            newPhoto.GetComponent<PhotoSceneSwith>().LinkedScene = firstSuspectSceneIndex + i;

            foreach (PhotoSetter setter in photoSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetPhoto(suspect.emotions[EmotionType.Concentrated]);
            }
            foreach (TextSetter setter in firstnameSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetText(suspect.name);
            }
            foreach (TextSetter setter in surnameSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetText(suspect.surname);
            }
            foreach (TextSetter setter in sizeSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetText($"{suspect.height} cm");
            }
            foreach (TextSetter setter in dateSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetText($"{suspect.date:yyyy-MM-dd}");
            }
            foreach (TextSetter setter in genderSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetText(suspect.sexe == "Male"?"M":"F");
            }
            i++;
        }
    }
    public void CloseDialogue()
    {
        Debug.Log("CloseCorkBoard");
    }
}

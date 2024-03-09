using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;


public class CorkBoardFlowHandler : MonoSingleton<CorkBoardFlowHandler>
{
    [SerializeField] private GameObject suspectPhotoPrefab;
    [SerializeField] private Transform[] photoPositions;
    private InternetHistoryPaper[] internetHistoryPapers;
    private BankAccountPaper[] bankAccountPapers;
    private CriminalRecordPaper[] criminalRecordPapers;
    private IDCard[] idCards;
    private ScriptPaper[] scriptPapers;
    private PhotoSetter[] photoSetters; 
    private TextSetter[] firstnameSetters;
    private TextSetter[] surnameSetters;
    private TextSetter[] sizeSetters;
    private TextSetter[] dateSetters;
    private TextSetter[] genderSetters;
    public void StartCorkBoard(Scenario _scenario,List<Suspect> _suspects)
    {
        int i = 0;
        AudioManager.instance.audioEvents["Game Music"].Play();
        TextSetter[] allTextSetters = FindObjectsOfType<TextSetter>(true);
        PhotoSetter[] photoSetters = FindObjectsOfType<PhotoSetter>(true);
        Notebook notebook = FindObjectOfType<Notebook>();
        notebook.SetScenario(_scenario.scenarioString);
        
        internetHistoryPapers = FindObjectsOfType<InternetHistoryPaper>(true);
        bankAccountPapers = FindObjectsOfType<BankAccountPaper>(true);
        criminalRecordPapers = FindObjectsOfType<CriminalRecordPaper>(true);
        idCards = FindObjectsOfType<IDCard>(true);
        scriptPapers = FindObjectsOfType<ScriptPaper>(true);
        
        dateSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Date).ToArray();
        sizeSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Size).ToArray();
        genderSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Gender).ToArray();
        surnameSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Surname).ToArray();
        firstnameSetters = allTextSetters.Where(textSetter => textSetter.GeSetterType() == TextType.Firstname).ToArray();
        foreach (Suspect suspect in _suspects)
        {
            List<Transform> documents = new();
            foreach (IDCard idCard in idCards.Where(paper => paper.GetPlayerId()== i))
            {
                documents.Add(idCard.transform);
                break;
            }
            foreach (InternetHistoryPaper history in internetHistoryPapers.Where(paper => paper.GetPlayerId() == i))
            {
                documents.Add(history.transform);
                history.SetInternetHistory(suspect.internetHistory.stringList);
                break;
            }
            foreach (BankAccountPaper bankAccount in bankAccountPapers.Where(paper => paper.GetPlayerId() == i))
            {
                documents.Add(bankAccount.transform);
                bankAccount.SetBankAccountTransactions(suspect.bankAccountHistory.current,suspect.bankAccountHistory.saving,suspect.bankAccountHistory.transactions);
                break;
            }
            foreach (CriminalRecordPaper criminalRecord in criminalRecordPapers.Where(paper => paper.GetPlayerId() == i))
            {
                documents.Add(criminalRecord.transform);
                criminalRecord.SetCriminalRecords(suspect.criminalRecord.records);
                break;
            }
            foreach (ScriptPaper script in scriptPapers.Where(script => script.GetPlayerId() == i))
            {
                documents.Add(script.transform);
                break;
            }
            DocumentPlacement.Instance.PlaceDocument(i,documents);
            /*
            Vector3 position = photoPositions[i].position;
            GameObject newPhoto = Instantiate(suspectPhotoPrefab,position,Quaternion.identity);
            newPhoto.transform.SetSiblingIndex(0);
            PhotoSetter photoSetter =  newPhoto.GetComponent<PhotoSetter>();
            photoSetter.SetPlayerId(i);
            photoSetter.SetPhoto(suspect.portrait);
            */

            foreach (PhotoSetter setter in photoSetters)
            {
                if (setter.GetPlayerId() != i) continue;
                setter.SetPhoto(suspect.portrait);
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
                setter.SetText(suspect.gender == "Male"?"M":"F");
            }
            i++;
        }
    }
    public void CloseDialogue()
    {
        Debug.Log("CloseCorkBoard");
    }
}

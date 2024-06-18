using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;




[Serializable]
public struct TutoText
{
   [Serializable]
   public enum TutoLanguage { English,French }
   public TutoLanguage language;
   public string text;
}
[Serializable]
public struct TutoPart
{
   public enum NextPartCondition{Click, None, Zoom, WaitTime, NotebookClicked, SuspectPageSelected}
   public TutoText[] texts;
   public TutoText[] altTexts;
   public NextPartCondition condition;
   public bool isHappyFace;
}
public class TutoBehaviour : MonoBehaviour
{
   public static Action OnPlayerClicked;
   public static Action OnPlayerZoom;
   public static Action NotebookClicked;
   public static Action SuspectPageSelected;
   public static Action<bool> FirstInnocented;
   public static Action<bool> SecondInnocented;
   public static Action<bool> ThirdInnocented;
   public static Action<bool> FourthInnocented;
   public static Action<bool> FiftInnocented;

   [SerializeField] private int LastSequentialIndex;
   [SerializeField] private TutoPart[] tutoParts;
   private int currentPart = 0;
   private IEnumerator tutoFlow;
   private IEnumerator popupFlow;
   private IEnumerator displayTextFlow;
   private TutoText.TutoLanguage tutoLanguage;

   [SerializeField] private GameObject bubble;
   [SerializeField] private TMP_Text bubbleText;
   [SerializeField] private GameObject bubbleHappyFace;
   [SerializeField] private GameObject bubbleAngryFace;

   private bool useAltText = false;
   private bool isTextDisplaying = false;
   private void Start()
   {
      bubble.SetActive(false);
      if (PlayerPrefs.GetInt("PlayTuto", 1) == 1)
      {
         ScenarioFlow.OnGameStart += () => { StartTuto(); };  
      }
   }

   public void StartTuto()
   {
      if (tutoFlow != null)
      {
         StopCoroutine(tutoFlow);
      }
      tutoLanguage = PlayerPrefs.GetString("Language", "English") == "English"?
            TutoText.TutoLanguage.English:
            TutoText.TutoLanguage.French;
      
      currentPart = 0;
      tutoFlow = TutoFlow();
      StartCoroutine(tutoFlow);
      FirstInnocented += FirstInnocentedHandler;
      SecondInnocented += SecondInnocentedHandler;
      ThirdInnocented += ThirdInnocentedHandler;
      FourthInnocented += FourthInnocentedHandler;
      FiftInnocented += FiftInnocentedHandler;
   }

   private void FiftInnocentedHandler(bool obj)
   {
      if(tutoFlow != null)StopCoroutine(tutoFlow);
      if(popupFlow != null)StopCoroutine(popupFlow);
      popupFlow = PopupFlow(4, !obj);
      StartCoroutine(popupFlow);
      StartCoroutine(FinishGame(obj));
      if(obj) FiftInnocented -= FiftInnocentedHandler;
   }

   private void FourthInnocentedHandler(bool obj)
   {
      if(tutoFlow != null)StopCoroutine(tutoFlow);
      if(popupFlow != null)StopCoroutine(popupFlow);
      popupFlow = PopupFlow(3, !obj);
      StartCoroutine(popupFlow);
      if(obj) FourthInnocented -= FourthInnocentedHandler;
   }

   private void ThirdInnocentedHandler(bool obj)
   {
      if(tutoFlow != null)StopCoroutine(tutoFlow);
      if(popupFlow != null)StopCoroutine(popupFlow);
      popupFlow = PopupFlow(2, !obj);
      StartCoroutine(popupFlow);
      if(obj) ThirdInnocented -= ThirdInnocentedHandler;
   }

   private void SecondInnocentedHandler(bool obj)
   {
      if(tutoFlow != null)StopCoroutine(tutoFlow);
      if(popupFlow != null)StopCoroutine(popupFlow);
      popupFlow = PopupFlow(1, !obj);
      StartCoroutine(popupFlow);
      if(obj) SecondInnocented -= SecondInnocentedHandler;
   }

   private void FirstInnocentedHandler(bool obj)
   {
      if(tutoFlow != null)StopCoroutine(tutoFlow);
      if(popupFlow != null)StopCoroutine(popupFlow);
      popupFlow = PopupFlow(0, !obj);
      StartCoroutine(popupFlow);
      if(obj) FirstInnocented -= FirstInnocentedHandler;
   }

   private IEnumerator PopupFlow(int _index,bool _altText)
   {
      ShowTextFromPart(tutoParts[LastSequentialIndex+1+_index],_altText);
      yield return WaitFor(tutoParts[LastSequentialIndex+1+_index].condition);
      HideText();
   }
   private IEnumerator FinishGame(bool _win)
   {
      yield return new WaitUntil(() => !isTextDisplaying);
      yield return new WaitForSeconds(2f);
      ScenarioFlow.Instance.FinishGame(_win);
   }

   private IEnumerator TutoFlow()
   {
      while (currentPart <= LastSequentialIndex)
      {
         ShowTextFromPart(tutoParts[currentPart],false);
         yield return WaitFor(tutoParts[currentPart].condition);
         currentPart++;
      }
      HideText();
   }
   IEnumerator ShowText(TMP_Text _uiText, string _text, float _speed = 0.02f)
   {
      int index = 0;
      _uiText.text = "";
      while (index < _text.Length)
      {
         _uiText.text += _text[index];
         yield return new WaitForSeconds(_speed);
         index++;
      }

      isTextDisplaying = false;
   }

   private void ShowTextFromPart(TutoPart part,bool _altText)
   {
      isTextDisplaying = true;
      string textToShow;
      if (_altText)
      {
          textToShow = part.altTexts.First(text => text.language == tutoLanguage).text;
      }
      else
      {
         textToShow = part.texts.First(text => text.language == tutoLanguage).text;
      }
      
      if(displayTextFlow != null)StopCoroutine(displayTextFlow);
      displayTextFlow = ShowText(bubbleText, textToShow);
      StartCoroutine(displayTextFlow);
      
      bubble.SetActive(!string.IsNullOrEmpty(textToShow));
      bubbleHappyFace.SetActive(part.isHappyFace);
      bubbleAngryFace.SetActive(!part.isHappyFace);
   }

   private void HideText()
   {
      bubble.SetActive(false);
   }

   private void Update()
   {
      if(Input.GetMouseButtonDown(0))OnPlayerClicked?.Invoke();
   }

   private IEnumerator WaitFor(TutoPart.NextPartCondition condition)
   {
      switch (condition)
      {
         case TutoPart.NextPartCondition.Click:
            return WaitClick();
         case TutoPart.NextPartCondition.Zoom:
            return WaitZoom();
         case TutoPart.NextPartCondition.WaitTime:
            return WaitTime();
         case TutoPart.NextPartCondition.NotebookClicked:
            return WaitNotebook();
         case TutoPart.NextPartCondition.SuspectPageSelected:
            return WaitSuspectPage();
         default:
            return null;
      }
   }

   IEnumerator WaitClick()
   {
      bool hasClicked = false;
      void OnClicked() => hasClicked = true;

      OnPlayerClicked += OnClicked;
      yield return new WaitUntil(() => hasClicked);
      OnPlayerClicked -= OnClicked;
   }
   IEnumerator WaitZoom()
   {
      bool hasZoomed = false;
      void OnZoom() => hasZoomed = true;

      OnPlayerZoom += OnZoom;
      yield return new WaitUntil(() => hasZoomed);
      OnPlayerZoom -= OnZoom;
   }
   IEnumerator WaitNotebook()
   {
      bool hasNotebook = false;
      void OnNotebook() => hasNotebook = true;

      NotebookClicked += OnNotebook;
      yield return new WaitUntil(() => hasNotebook);
      NotebookClicked -= OnNotebook;
   }
   IEnumerator WaitSuspectPage()
   {
      bool hasGoodPage = false;
      void OnSuspectPage() => hasGoodPage = true;

      SuspectPageSelected += OnSuspectPage;
      yield return new WaitUntil(() => hasGoodPage);
      SuspectPageSelected -= OnSuspectPage;
   }
   IEnumerator WaitTime()
   {
      yield return new WaitUntil(() => !isTextDisplaying);
       yield return new WaitForSeconds(1f);
   }
}

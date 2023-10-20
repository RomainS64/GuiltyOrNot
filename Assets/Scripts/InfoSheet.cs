using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InfoSheet : MonoSingleton<InfoSheet>
{
        [SerializeField] private TMP_Text name;
        [SerializeField] private TMP_Text surname;
        
        [SerializeField] private TMP_Text age;
        [SerializeField] private TMP_Text sexe;
        [SerializeField] private TMP_Text height;
    
        [SerializeField] private TMP_Text criminalRecord;
        [SerializeField] private TMP_Text psychologicalReport;
        [SerializeField] private TMP_Text medicalReport;


        public void DisplaySheet(string _name,string _surname,int _age,string _sexe,int _height,string _criminalRecord,string _psychologicalReport,string _medicalReport)
        {
                name.text = _name;
                surname.text = _surname;
                age.text = _age+" yo";

                sexe.text = _sexe;
                height.text = _height+" cm";
                criminalRecord.text = _criminalRecord;
                psychologicalReport.text = _psychologicalReport;
                medicalReport.text = _medicalReport;
                
                gameObject.SetActive(true);
        }

        public void HideSheet()
        { 
                gameObject.SetActive(false);       
        }
}

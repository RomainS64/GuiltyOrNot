
using Unity.VisualScripting;
using UnityEngine;

public static class NameAndSurnameParser
{
        public const string WomanFilePath = "NamesWoman";
        public const string ManFilePath = "NamesMan";
        public const string SurnamesFilePath = "Surnames";
        public const string DataFolder = "Data/";

        public static string GetManName()
        {
            TextAsset mansName = Resources.Load<TextAsset>(DataFolder + ManFilePath);
            string[] allManNames = mansName.text.Split("\n");
            return allManNames[Random.Range(0, allManNames.Length)].Split(",")[0];
        }
        public static string GetWomanName()
        {
            TextAsset womansName = Resources.Load<TextAsset>(DataFolder + WomanFilePath);
            string[] allWomanNames = womansName.text.Split("\n");
            return allWomanNames[Random.Range(0, allWomanNames.Length)].Split(",")[0]; 
        }
        public static string GetSurname()
        {
            TextAsset surnames = Resources.Load<TextAsset>(DataFolder + SurnamesFilePath);
            string[] allSurnames = surnames.text.Split("\n");
            return allSurnames[Random.Range(0, allSurnames.Length)].Split(",")[0];   
        }
        
}


using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class scenarioDemoGenerator : MonoBehaviour
{
    [SerializeField] private ScenarioGenerator scenarioGenerator;
    [SerializeField] private TMP_Text scenarioText;
    // Start is called before the first frame update
    public void Generate()
    {
        scenarioText.text = "";
        scenarioGenerator.GenerateScenario(OnGenerated);
    }

    private void OnGenerated(Scenario scenario)
    {
        scenarioText.text = scenario.scenarioString;
    }
    
}

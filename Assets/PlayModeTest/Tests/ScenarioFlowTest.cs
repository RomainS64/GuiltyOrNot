using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class ScenarioFlowTests
{
    [UnityTest]
    public IEnumerator GenerateScenario_ReturnsValidScenario()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        yield return null;
        
        Scenario? generatedScenario = null;
        yield return  ScenarioFlow.Instance.GenerateScenario(scenario => generatedScenario = scenario);
        
        Assert.IsNotNull(generatedScenario);
        Assert.IsFalse(string.IsNullOrEmpty(generatedScenario?.scenarioString));
        Assert.IsFalse(generatedScenario?.scenarioString.Equals("[DEBUG] Scenario not generate"));
        Assert.IsFalse(string.IsNullOrEmpty(generatedScenario?.objectLost));
        Assert.IsFalse(string.IsNullOrEmpty(generatedScenario?.thiefLocation));
        Assert.IsFalse(generatedScenario?.thiefDate.Equals(DateTime.Now));
        
    }
}
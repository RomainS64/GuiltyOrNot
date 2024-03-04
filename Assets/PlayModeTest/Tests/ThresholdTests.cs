
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

[TestFixture]
public class ThresholdTests : MonoBehaviour
{
    [UnityTest]
    public IEnumerator GenerateAndEliminateSuspects()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        
        List<Suspect> generatedSuspects = null;
        yield return null;
        yield return ScenarioFlow.Instance.GenerateSuspects(5, suspects => generatedSuspects = suspects);
        
        Assert.IsNotNull(generatedSuspects);

        for (int i = 0; i < generatedSuspects.Count; i++)
        {
            Assert.IsFalse(generatedSuspects[i].isEliminated);

            ScenarioFlow.Instance.SetSuspectEliminationStatus(i, true);
            
            yield return null;
            Assert.IsTrue(generatedSuspects[i].isEliminated);
        }
    }
}

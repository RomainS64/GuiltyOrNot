using System.Collections;
using NUnit.Framework;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;


/*
[TestFixture]
public class SuspectGeneratorTests
{
    [UnityTest]
    public IEnumerator  GenerateSuspect_ReturnsSuspectWithValidName()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        yield return null;
        Suspect suspect =  SuspectGenerator.Instance.GenerateSuspect(0);
        yield return null;
        Assert.IsFalse(string.IsNullOrEmpty(suspect.name));
    }

    [UnityTest]
    public IEnumerator GenerateSuspect_ReturnsSuspectWithValidSurname()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        yield return null;
        Suspect suspect = SuspectGenerator.Instance.GenerateSuspect(0);
        yield return null;
        Assert.IsFalse(string.IsNullOrEmpty(suspect.surname));
    }

    [UnityTest]
    public IEnumerator GenerateSuspect_ReturnsSuspectWithValidAge()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        yield return null;
        Suspect suspect = SuspectGenerator.Instance.GenerateSuspect(0);
        yield return null;
        Assert.GreaterOrEqual(suspect.age, SuspectGenerator.Instance.minAge);
        Assert.LessOrEqual(suspect.age, SuspectGenerator.Instance.maxAge);
    }

    [UnityTest]
    public IEnumerator GenerateSuspect_ReturnsSuspectWithValidGender()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        yield return null;
        SuspectGenerator generator = SuspectGenerator.Instance;
        yield return null;
        Suspect suspect = generator.GenerateSuspect(0);
        Assert.IsTrue(suspect.gender == "Male" || suspect.gender == "Female");
    }

    [UnityTest]
    public IEnumerator GenerateSuspect_ReturnsSuspectWithValidHeight()
    {
        if(SceneManager.GetActiveScene().name != "ScenarioGenerationTest")SceneManager.LoadScene("ScenarioGenerationTest");
        yield return null;
        Suspect suspect = SuspectGenerator.Instance.GenerateSuspect(0);
        yield return null;
        Assert.GreaterOrEqual(suspect.height, SuspectGenerator.Instance.minSize);
        Assert.LessOrEqual(suspect.height, SuspectGenerator.Instance.maxSize);
    }
    
    // Add more tests as needed
}
*/
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using REST_API_HANDLER;
using UnityEngine;
using UnityEngine.Networking;
using TMPro;
using System;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Random = UnityEngine.Random;


public class ErrorDetail
{
	public string Code { get; set; }
	public string Message { get; set; }
	public string Param { get; set; }
	public string Type { get; set; }
}

public class DallEError
{
	public ErrorDetail Error { get; set; }
}
public class DallESuspectVisualGenerator : MonoBehaviour
{
	[SerializeField]
	private string generationStyle = "in caricature exaggerate face comics in stylized realistic digital art";

	[SerializeField] private Texture notGeneratedTexture;
	/// HAIRS
	[SerializeField] 
	private SuspectVisualAttributes ShortHairAttribute;
	[SerializeField] 
	private SuspectVisualAttributes LongHairAttribute;
	[SerializeField] 
	private SuspectVisualAttributes HairAttribute;
	[SerializeField] 
	private SuspectVisualAttributes HairColorAttribute;
	
	/// BEARD
	[SerializeField] 
	private SuspectVisualAttributes BeardAttributes;
	[SerializeField] 
	private SuspectVisualAttributes MustacheAttributes;
	
	/// OTHER
	[SerializeField] 
	private SuspectVisualAttributes ClothAttribute;
	[SerializeField] 
	private SuspectVisualAttributes ClothColorAttribute;
	[SerializeField] 
	private SuspectVisualAttributes HeadAccessoryAttribute;
	

	private string IMAGE_GENERTION_API_URL = "https://api.openai.com/v1/images/generations";

	public void GenerateSuspectFaceAsync(Suspect suspect, EmotionType _emotion, bool generatePrompt,
		Action<(Suspect, Texture)> OnGenerated)
	{
		string prompt  = GeneratePrompt(suspect);
		GenerateImage(prompt, "1024x1024", OnCompletedAction);
		void OnCompletedAction(List<Texture> textures) => OnGenerated?.Invoke((suspect, textures[0]));
	}

	public void GenerateImage(string description, string resolution, Action<List<Texture>> completationAction)
	{
		GenerateImageRequestModel reqModel = new GenerateImageRequestModel("dall-e-3",description, 1 ,resolution);
		ApiCall.instance.PostRequest<GenerateImageResponseModel>(IMAGE_GENERTION_API_URL, reqModel.ToCustomHeader(), null, reqModel.ToBody(), (result =>
		{
			LoadTexture(result.data, completationAction);
		}), (error =>
		{
			StartCoroutine(RegenerateCoroutine(error, description, resolution, completationAction));
		}));
	}

	IEnumerator RegenerateCoroutine(string error,string description, string resolution, Action<List<Texture>> completationAction)
	{
		var errorObject = JsonConvert.DeserializeObject<DallEError>(error);
		Debug.Log("error code:" + errorObject.Error.Code);
		switch (errorObject.Error.Code)
		{
			case "rate_limit_exceeded":
				yield return new WaitForSeconds(61);
				Debug.Log("rate_limit_exceeded >>>> Regenerate image");
				GenerateImage(description, resolution, completationAction);
				break;
			case "content_policy_violation":
				completationAction(new List<Texture>() { notGeneratedTexture });
				break;
			default:
				Debug.Log("image not generated");
				break;
		}
		yield return null;
	}
	
	
	async void LoadTexture(List<UrlClass> urls, Action<List<Texture>> completationAction)
	{
		List<Texture> textures = new List<Texture>();
		for (int i = 0; i < urls.Count; i++)
        {
			Texture2D texture = await GetRemoteTexture(urls[i].url);
			textures.Add(texture);
	    }
		completationAction.Invoke(textures);
	}

	public static async Task<Texture2D> GetRemoteTexture(string url)
	{
		using (UnityWebRequest www = UnityWebRequestTexture.GetTexture(url))
		{
			var asyncOp = www.SendWebRequest();

			while (asyncOp.isDone == false)
				await Task.Delay(1000 / 30);//30 hertz

			// read results:
			if (www.isNetworkError || www.isHttpError)
			{
				return null;
			}
			return DownloadHandlerTexture.GetContent(www);
			
		}
	}

	private void WriteImageOnDisk(Texture2D texture, string fileName)
	{
		byte[] textureBytes = texture.EncodeToPNG();
		string path = Application.persistentDataPath + fileName;
		File.WriteAllBytes(path, textureBytes);
		Debug.Log("File Written On Disk! "  + path );
	}
	private string GeneratePrompt(Suspect _suspect)
	{
		string style = generationStyle;
		string additionalInfo = ", visible bust";
		string suspectCaracteristics = GenerateSuspectVisualCaracteristics(_suspect);
		string background = "a grey background behind(grey)";
		string prompt = $"{style}, {additionalInfo}, {suspectCaracteristics}, {background},";
		Debug.Log(prompt);
		return prompt;
	}
	private string GenerateSuspectVisualCaracteristics(Suspect _suspect)
	{
		string prompt = "";
		
		//-------------------GENDER
		prompt += "a " + _suspect.gender;
		
		//-------------------AGE
		string ageDecorator = "";
		if (_suspect.age < 35) ageDecorator = "young";
		if (_suspect.age > 65) ageDecorator = "very old";
		prompt += $",{ageDecorator} {_suspect.age} years old";
		
		//-------------------HAIRS
		SuspectVisualAttributes hairAttribute = null;
		if ((_suspect.hairToGenerate.Item2 == TestimonialHairType.Long && _suspect.hairToGenerate.Item1) ||
		    (_suspect.hairToGenerate.Item2 == TestimonialHairType.Short && !_suspect.hairToGenerate.Item1))
			hairAttribute = LongHairAttribute;
		if ((_suspect.hairToGenerate.Item2 == TestimonialHairType.Short && _suspect.hairToGenerate.Item1) ||
		    (_suspect.hairToGenerate.Item2 == TestimonialHairType.Long && !_suspect.hairToGenerate.Item1))
			hairAttribute = ShortHairAttribute;
		if (_suspect.hairToGenerate.Item2 == TestimonialHairType.Any)hairAttribute = HairAttribute;
		prompt += prompt += $",{GetRandomFromAttribute(hairAttribute, true)} hairs ({GetRandomFromAttribute(HairColorAttribute,true)})";

		//-------------------BEARD
		if (_suspect.generateBeard)
		{
			prompt += $",{GetRandomFromAttribute(BeardAttributes, true)}";
		}

		//------------------OTHER
		prompt += $",{GetRandomFromAttribute(HeadAccessoryAttribute,true)}";
		prompt += $",in a {GetRandomFromAttribute(ClothAttribute, true)}({GetRandomFromAttribute(ClothColorAttribute, true)})";
		return prompt;
	}
	private string GetRandomFromAttribute(SuspectVisualAttributes _attributes, bool _isAMan)
	{
		int weightSum = _attributes.attributes.Sum(va => _isAMan?va.weights.manProbabilityWeight:va.weights.womanProbabilityWeight);
		int generatedWeight = Random.Range(1,weightSum);
		int currentWeight = 0;
		foreach (VisualAttribute attribute in _attributes.attributes)
		{
			currentWeight += _isAMan ? attribute.weights.manProbabilityWeight : attribute.weights.womanProbabilityWeight;
			if (currentWeight >= generatedWeight)
			{
				return attribute.prompt;
			}
		}
		return string.Empty;
	}
}

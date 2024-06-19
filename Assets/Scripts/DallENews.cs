using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using REST_API_HANDLER;
using UnityEngine;
using UnityEngine.Networking;

using System;
using System.IO;

using Newtonsoft.Json;

public class DallENews: MonoBehaviour
{
	[SerializeField]
	private string generationStyle = "in caricature exaggerate face comics in stylized realistic digital art:";

	[SerializeField] private Texture notGeneratedTexture;

	private string IMAGE_GENERTION_API_URL = "https://api.openai.com/v1/images/generations";
	
	
	public void GenerateImage(string description, Action<List<Texture>> completationAction,string resolution="1024x1024")
	{
		GenerateImageRequestModel reqModel = new GenerateImageRequestModel("dall-e-3",generationStyle+description, 1 ,resolution);
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
				GenerateImage(description, completationAction,resolution);
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
}

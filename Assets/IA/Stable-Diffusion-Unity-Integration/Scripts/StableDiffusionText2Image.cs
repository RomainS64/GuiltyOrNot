using Newtonsoft.Json;
using System;
using System.Collections;
using System.IO;
using System.Net;
using UnityEngine;
using System.Text;
using UnityEngine.UI;
using System.Threading.Tasks;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
#endif

/// <summary>
/// Component to help generate a UI Image or RawImage using Stable Diffusion.
/// </summary>
[ExecuteAlways]
public class StableDiffusionText2Image : StableDiffusionGenerator
{
    public Action<Texture> OnGenerationDone;
    public bool removeBackgroundAfterGeneration;
    private BackgroundRemover backgroundRemover = null;
    
    [ReadOnly]
    public string guid = "";
    
    public string prompt;
    public string negativePrompt;

    /// <summary>
    /// List of samplers to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] samplersList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.samplers;
        }
    }
    /// <summary>
    /// Actual sampler selected in the drop-down list
    /// </summary>
    [HideInInspector]
    public int selectedSampler = 0;

    public int width = 512;
    public int height = 512;
    public int steps = 90;
    public float cfgScale = 7;
    public long seed = -1;

    public long generatedSeed = -1;

    string filename = "";



    /// <summary>
    /// List of models to display as Drop-Down in the inspector
    /// </summary>
    [SerializeField]
    public string[] modelsList
    {
        get
        {
            if (sdc == null)
                sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            return sdc.modelNames;
        }
    }
    /// <summary>
    /// Actual model selected in the drop-down list
    /// </summary>
    [HideInInspector]
    public int selectedModel = 0;


    /// <summary>
    /// On Awake, fill the properties with default values from the selected settings.
    /// </summary>
    void Awake()
    {

        if (removeBackgroundAfterGeneration)
        {
            backgroundRemover = FindObjectOfType<BackgroundRemover>();
        }
#if UNITY_EDITOR
        if (width < 0 || height < 0)
        {
            StableDiffusionConfiguration sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();
            if (sdc != null)
            {
                SDSettings settings = sdc.settings;
                if (settings != null)
                {

                    width = settings.width;
                    height = settings.height;
                    steps = settings.steps;
                    cfgScale = settings.cfgScale;
                    seed = settings.seed;
                    return;
                }
            }

            width = 512;
            height = 512;
            steps = 50;
            cfgScale = 7;
            seed = -1;
        }
#endif
    }


    void Update()
    {
#if UNITY_EDITOR
        // Clamp image dimensions values between 128 and 2048 pixels
        if (width < 128) width = 128;
        if (height < 128) height = 128;
        if (width > 2048) width = 2048;
        if (height > 2048) height = 2048;

        // If not setup already, generate a GUID (Global Unique Identifier)
        if (guid == "")
            guid = Guid.NewGuid().ToString();
#endif
    }

    // Internally keep tracking if we are currently generating (prevent re-entry)
    bool generating = false;

    /// <summary>
    /// Callback function for the inspector Generate button.
    /// </summary>
    public void Generate()
    {
        // Start generation asynchronously
        if (!generating && !string.IsNullOrEmpty(prompt))
        {
            StartCoroutine(GenerateAsync());
        }
    }

    /// <summary>
    /// Setup the output path and filename for image generation
    /// </summary>
    void SetupFolders()
    {
        // Get the configuration settings
        if (sdc == null)
            sdc = GameObject.FindObjectOfType<StableDiffusionConfiguration>();

        try
        {
            // Determine output path
            string root = Application.dataPath + sdc.settings.OutputFolder;
            if (root == "" || !Directory.Exists(root))
                root = Application.streamingAssetsPath;
            string mat = Path.Combine(root, "SDImages");
            filename = Path.Combine(mat, guid + ".png");

            // If folders not already exists, create them
            if (!Directory.Exists(root))
                Directory.CreateDirectory(root);
            if (!Directory.Exists(mat))
                Directory.CreateDirectory(mat);

            // If the file already exists, delete it
            if (File.Exists(filename))
                File.Delete(filename);
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }

    IEnumerator GenerateAsync()
    {
        generating = true;

        SetupFolders();
        
        // Set the model parameters
        yield return sdc.SetModelAsync(modelsList[selectedModel]);

        // Generate the image
        HttpWebRequest httpWebRequest = null;
        try
        {
            // Make a HTTP POST request to the Stable Diffusion server
            httpWebRequest = (HttpWebRequest)WebRequest.Create(sdc.settings.StableDiffusionServerURL + sdc.settings.TextToImageAPI);
            httpWebRequest.ContentType = "application/json";
            httpWebRequest.Method = "POST";

            // add auth-header to request
            if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
            {
                httpWebRequest.PreAuthenticate = true;
                byte[] bytesToEncode = Encoding.UTF8.GetBytes(sdc.settings.user + ":" + sdc.settings.pass);
                string encodedCredentials = Convert.ToBase64String(bytesToEncode);
                httpWebRequest.Headers.Add("Authorization", "Basic " + encodedCredentials);
            }
            
            // Send the generation parameters along with the POST request
            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                SDParamsInTxt2Img sd = new SDParamsInTxt2Img();
                sd.prompt = prompt;
                sd.negative_prompt = negativePrompt;
                sd.steps = steps;
                sd.cfg_scale = cfgScale;
                sd.width = width;
                sd.height = height;
                sd.seed = seed;
                sd.tiling = false;

                if (selectedSampler >= 0 && selectedSampler < samplersList.Length)
                    sd.sampler_name = samplersList[selectedSampler];

                // Serialize the input parameters
                string json = JsonConvert.SerializeObject(sd);

                // Send to the server
                streamWriter.Write(json);
            }
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }

        // Read the output of generation
        if (httpWebRequest != null)
        {
            // Wait that the generation is complete before procedding
            Task<WebResponse> webResponse = httpWebRequest.GetResponseAsync();

            while (!webResponse.IsCompleted)
            {
                if (sdc.settings.useAuth && !sdc.settings.user.Equals("") && !sdc.settings.pass.Equals(""))
                    UpdateGenerationProgressWithAuth();
                else
                    UpdateGenerationProgress();

                yield return new WaitForSeconds(0.5f);
            }

            // Stream the result from the server
            var httpResponse = webResponse.Result;

            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                // Decode the response as a JSON string
                string result = streamReader.ReadToEnd();

                // Deserialize the JSON string into a data structure
                SDResponseTxt2Img json = JsonConvert.DeserializeObject<SDResponseTxt2Img>(result);

                // If no image, there was probably an error so abort
                if (json.images == null || json.images.Length == 0)
                {
                    Debug.LogError("No image was return by the server. This should not happen. Verify that the server is correctly setup.");

                    generating = false;
#if UNITY_EDITOR 
                    //EditorUtility.ClearProgressBar();
#endif
                    yield break;
                }

                // Decode the image from Base64 string into an array of bytes
                byte[] imageData = Convert.FromBase64String(json.images[0]);

                // Write it in the specified project output folder
                using (FileStream imageFile = new FileStream(filename, FileMode.Create))
                {
#if UNITY_EDITOR  
                    AssetDatabase.StartAssetEditing();
#endif
                    yield return imageFile.WriteAsync(imageData, 0, imageData.Length);
#if UNITY_EDITOR  
                    AssetDatabase.StopAssetEditing();
                    AssetDatabase.SaveAssets();
#endif
                }

                try
                {
                    // Read back the image into a texture
                    if (File.Exists(filename))
                    {
                        Texture2D texture = new Texture2D(2, 2);
                        texture.LoadImage(imageData);
                        texture.Apply();
                        if (removeBackgroundAfterGeneration)
                        {
                            Texture2D newTexture = backgroundRemover.RemoveBackground(ResizeTexture(texture));
                            LoadIntoImage(newTexture);
                        }
                        else
                        {
                            
                            LoadIntoImage(texture);
                        }
                        
                    }
                    // Read the generation info back (only seed should have changed, as the generation picked a particular seed)
                    if (json.info != "")
                    {
                        SDParamsOutTxt2Img info = JsonConvert.DeserializeObject<SDParamsOutTxt2Img>(json.info);

                        // Read the seed that was used by Stable Diffusion to generate this result
                        generatedSeed = info.seed;
                    }
                }
                catch (Exception e)
                {
                    Debug.LogError(e.Message + "\n\n" + e.StackTrace);
                }
            }
        }
#if UNITY_EDITOR 
        //EditorUtility.ClearProgressBar();
#endif
        generating = false;
        RawImage im = GetComponent<RawImage>();
        OnGenerationDone?.Invoke(im.texture);
        yield return null;
    }

    Texture2D ResizeTexture(Texture2D sourceTexture)
    {

        // Créez une nouvelle texture 2D de 1024x1024.
        int newWidth = 1024;
        int newHeight = 1024;
        Texture2D resizedTexture = new Texture2D(newWidth, newHeight);

    // Calculez le ratio de redimensionnement.
        float xRatio = (float)sourceTexture.width / newWidth;
        float yRatio = (float)sourceTexture.height / newHeight;

        for (int y = 0; y < newHeight; y++)
        {
            for (int x = 0; x < newWidth; x++)
            {
                // Utilisez l'interpolation bilinéaire pour calculer la couleur du pixel redimensionné.
                float sourceX = x * xRatio;
                float sourceY = y * yRatio;

                Color c00 = sourceTexture.GetPixel((int)sourceX, (int)sourceY);
                Color c10 = sourceTexture.GetPixel((int)sourceX + 1, (int)sourceY);
                Color c01 = sourceTexture.GetPixel((int)sourceX, (int)sourceY + 1);
                Color c11 = sourceTexture.GetPixel((int)sourceX + 1, (int)sourceY + 1);

                float fracX = sourceX - (int)sourceX;
                float fracY = sourceY - (int)sourceY;

                Color interpolatedColor = Color.Lerp(Color.Lerp(c00, c10, fracX), Color.Lerp(c01, c11, fracX), fracY);

                resizedTexture.SetPixel(x, y, interpolatedColor);
            }
        }
        // Appliquez les modifications à la nouvelle texture.
        resizedTexture.Apply();
        return resizedTexture;
    }
    /// <summary>
    /// Load the texture into an Image or RawImage.
    /// </summary>
    /// <param name="texture">Texture to setup</param>
    void LoadIntoImage(Texture2D texture)
    {
        try
        {
            // Find the image component
            Image im = GetComponent<Image>();
            if (im != null)
            {
                // Create a new Sprite from the loaded image
                Sprite sprite = Sprite.Create(texture, new Rect(0, 0, texture.width, texture.height), Vector2.zero);

                // Set the sprite as the source for the UI Image
                im.sprite = sprite;
            }
            // If no image found, try to find a RawImage component
            else
            {
                RawImage rim = GetComponent<RawImage>();
                if (rim != null)
                {
                    rim.texture = texture;
                }
            }

#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                // Force Unity inspector to refresh with new asset
                AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                EditorApplication.QueuePlayerLoopUpdate();
                EditorSceneManager.MarkAllScenesDirty();
                EditorUtility.RequestScriptReload();
            }
#endif
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "\n\n" + e.StackTrace);
        }
    }
}

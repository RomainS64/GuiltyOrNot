using Unity.Barracuda;
using UnityEngine;


public class BackgroundRemover : MonoBehaviour
{
    [SerializeField] private NNModel modelAsset;
    private Model runtimeModel;
    private IWorker worker;
    
    void Awake()
    {   
        runtimeModel = ModelLoader.Load(modelAsset);
        worker = WorkerFactory.CreateWorker(runtimeModel);
        
    }
    private void Destroy()
    {
        worker.Dispose();
    }
    public Texture2D RemoveBackground(Texture2D _inputTexture)
    {
        Tensor inputTensor = new Tensor(1, 1024, 1024, 3);
        for (int h = 0; h < 1024; h++)
        {
            for (int w = 0; w < 1024; w++)
            {
                inputTensor[0,h,w,0] = _inputTexture.GetPixel(w,h).r;
                inputTensor[0,h,w,1] = _inputTexture.GetPixel(w,h).g;
                inputTensor[0,h,w,2] = _inputTexture.GetPixel(w,h).b;
            }
        }
        Tensor outputTensor = worker.Execute(inputTensor).PeekOutput("mask");
        inputTensor.Dispose();
        float[] mask = outputTensor.data.Download(new TensorShape(1,1024,1024,1));
        outputTensor.Dispose();
        
        Color[] outputColors = new Color[mask.Length];
        Color[] inputColors = _inputTexture.GetPixels();
        
        for (int i = 0; i < mask.Length; i++)
        {
            
            float value = mask[i];
            Color color = new Color(inputColors[i].r,inputColors[i].g,inputColors[i].b, value > 0.5f ? 1:0);
            outputColors[i] = color;
        }
        
        Texture2D outputTexture = new Texture2D(1024,1024);
        outputTexture.SetPixels(outputColors);
        outputTexture.Apply();
        return outputTexture;
    }
}

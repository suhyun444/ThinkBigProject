using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using Unity.Barracuda;

public class RecognizeDigitsAI : MonoBehaviour
{
    public NNModel modelAsset;
    private Model _runtimeModel;
    private IWorker _engine;
    // Start is called before the first frame update
    public void Init()
    {
        _runtimeModel = ModelLoader.Load(modelAsset);
        _engine = WorkerFactory.CreateWorker(_runtimeModel, WorkerFactory.Device.GPU);
    }

    public int Calc(DrawingCalculator drawingCalculator)
    {
        Texture2D tex = drawingCalculator.GetTexture();
        var channelCount = 1; 
        var inputX = new Tensor(tex, channelCount);

        Tensor outputY = _engine.Execute(inputX).PeekOutput();
        float[] predicted = outputY.AsFloats();
        int predictedValue = Array.IndexOf(predicted, predicted.Max());
        inputX.Dispose();
        outputY.Dispose();
        return predictedValue;
    }
    public void Dispose()
    {
        _engine.Dispose();
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawingCalculator
{
    public Camera camera;
    RenderTexture rt;
    public Material sampleMat;
    public void BindCamera(Camera camera)
    {
        this.camera = camera;
    }
    // This will retrive a 2D float representing the pixel data from a screenshot of the main camera
    public Texture2D CalculateDrawing()
    {
        float[] outputData = new float[28 * 28];

        Texture2D tex = new Texture2D(28, 28, TextureFormat.RGB24, false);
        RenderTexture rt = new RenderTexture(28, 28, 24);
        rt.depth = 24;
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;
        Rect rectReadPixels = new Rect(0, 0, 28, 28);
        tex.ReadPixels(rectReadPixels, 0, 0);
        tex.Apply();
        return tex;
    }

}

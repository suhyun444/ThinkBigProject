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
    public float[] CalculateDrawing(int[] dim)
    {
        float[] outputData = new float[dim[0] * dim[1]];

        Texture2D tex = new Texture2D(dim[0], dim[1], TextureFormat.RGB24, false);
        RenderTexture rt = new RenderTexture(dim[0], dim[1], 24);
        rt.depth = 24;
        camera.targetTexture = rt;
        camera.Render();
        RenderTexture.active = rt;
        Rect rectReadPixels = new Rect(0, 0, dim[0], dim[1]);
        tex.ReadPixels(rectReadPixels, 0, 0);
        tex.Apply();

        //camera.targetTexture = null;

        for (int j = 0; j < tex.height; j++)
        {
            for (int i = 0; i < tex.width; i++)
            {
                Color pixelColor = tex.GetPixel(i, tex.height - j - 1); // flip y
                float grayscale = (pixelColor.r + pixelColor.g + pixelColor.b) / 3f;
                outputData[j * tex.width + i] = grayscale;
            }
        }
        //sampleMat.SetTexture("_MainTex", tex);
        return outputData;
    }

}

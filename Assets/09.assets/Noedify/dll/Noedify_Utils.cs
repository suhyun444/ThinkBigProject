// Decompiled with JetBrains decompiler
// Type: Noedify_Utils
// Assembly: Noedify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8080F0DD-0A3F-460C-80E2-226F05F6E4A2
// Assembly location: D:\Unity\Github\RaisingDragons\RecognizeHandWrittenDigitsTest\Assets\Noedify\dll\Noedify.dll

using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class Noedify_Utils
{
  public static int ConvertOneHotToInt(float[] oneHot)
  {
    int num1 = -1;
    float num2 = -10f;
    for (int index = 0; index < oneHot.Length; ++index)
    {
      if ((double) oneHot[index] > (double) num2)
      {
        num2 = oneHot[index];
        num1 = index;
      }
    }
    return num1;
  }

  public static float[] FlattenDataset(float[,,] matrix)
  {
    int length1 = matrix.GetLength(0);
    int length2 = matrix.GetLength(1);
    int length3 = matrix.GetLength(2);
    float[] numArray = new float[length1 * length2 * length3];
    for (int index1 = 0; index1 < length1; ++index1)
    {
      for (int index2 = 0; index2 < length2; ++index2)
      {
        for (int index3 = 0; index3 < length3; ++index3)
          numArray[index1 * length2 * length3 + index2 * length3 + index3] = matrix[index1, index2, index3];
      }
    }
    return numArray;
  }

  public static List<float[]> FlattenDataset(List<float[,,]> matrix)
  {
    int length1 = matrix[0].GetLength(0);
    int length2 = matrix[0].GetLength(1);
    int length3 = matrix[0].GetLength(2);
    List<float[]> numArrayList = new List<float[]>();
    for (int index1 = 0; index1 < matrix.Count; ++index1)
    {
      float[] numArray = new float[length1 * length2 * length3];
      for (int index2 = 0; index2 < length1; ++index2)
      {
        for (int index3 = 0; index3 < length2; ++index3)
        {
          for (int index4 = 0; index4 < length3; ++index4)
            numArray[index2 * length2 * length3 + index3 * length3 + index4] = matrix[index1][index2, index3, index4];
        }
      }
      numArrayList.Add(numArray);
    }
    return numArrayList;
  }

  public static int[] Shuffle(int[] inputArray)
  {
    if (inputArray.Length == 1)
      return inputArray;
    for (int index1 = 0; index1 < inputArray.Length; ++index1)
    {
      int index2 = Random.Range(0, inputArray.Length);
      int input = inputArray[index2];
      inputArray[index2] = inputArray[index1];
      inputArray[index1] = input;
    }
    return inputArray;
  }

  public static float[,] RotateImage90(float[,] imageData)
  {
    int length1 = imageData.GetLength(0);
    int length2 = imageData.GetLength(1);
    float[,] numArray = new float[length1, length2];
    for (int index1 = 0; index1 < length2; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
        numArray[index1, index2] = imageData[index2, index1];
    }
    return numArray;
  }

  public static float[,] FlipImage(float[,] imageData, bool horizontal = true)
  {
    int length1 = imageData.GetLength(0);
    int length2 = imageData.GetLength(1);
    float[,] numArray = new float[length1, length2];
    for (int index1 = 0; index1 < length2; ++index1)
    {
      for (int index2 = 0; index2 < length1; ++index2)
        numArray[index1, index2] = !horizontal ? imageData[length2, length1 - index2 - 1] : imageData[length2 - index1 - 1, index2];
    }
    return numArray;
  }

  public static float[,,] AddSingularDim(float[,] array)
  {
    int length1 = array.GetLength(0);
    int length2 = array.GetLength(1);
    float[,,] numArray = new float[1, length1, length2];
    for (int index1 = 0; index1 < length1; ++index1)
    {
      for (int index2 = 0; index2 < length2; ++index2)
        numArray[0, index1, index2] = array[index1, index2];
    }
    return numArray;
  }

  public static float[,] AddSingularDim(float[] array)
  {
    int length = array.GetLength(0);
    float[,] numArray = new float[1, length];
    for (int index = 0; index < length; ++index)
      numArray[0, index] = array[index];
    return numArray;
  }

  public static float[,,] AddTwoSingularDims(float[] array)
  {
    int length = array.Length;
    float[,,] numArray = new float[1, 1, length];
    for (int index = 0; index < length; ++index)
      numArray[0, 0, index] = array[index];
    return numArray;
  }

  public static float[,] SqueezeDim(float[,,] array, int squeezeIndex, int dimensionIndex = 0)
  {
    int length1 = array.GetLength(0);
    int length2 = array.GetLength(1);
    int length3 = array.GetLength(2);
    float[,] numArray;
    switch (dimensionIndex)
    {
      case 0:
        numArray = new float[length2, length3];
        for (int index1 = 0; index1 < length2; ++index1)
        {
          for (int index2 = 0; index2 < length3; ++index2)
            numArray[index1, index2] = array[squeezeIndex, index1, index2];
        }
        break;
      case 1:
        numArray = new float[length1, length3];
        for (int index3 = 0; index3 < length1; ++index3)
        {
          for (int index4 = 0; index4 < length3; ++index4)
            numArray[index3, index4] = array[index3, squeezeIndex, index4];
        }
        break;
      default:
        numArray = new float[length1, length2];
        for (int index5 = 0; index5 < length1; ++index5)
        {
          for (int index6 = 0; index6 < length2; ++index6)
            numArray[index5, index6] = array[index5, index6, squeezeIndex];
        }
        break;
    }
    return numArray;
  }

  public static float[] SqueezeDim(float[,] array, int squeezeIndex, int dimensionIndex = 0)
  {
    int length1 = array.GetLength(0);
    int length2 = array.GetLength(1);
    float[] numArray;
    if (dimensionIndex == 0)
    {
      numArray = new float[length2];
      for (int index = 0; index < length2; ++index)
        numArray[index] = array[squeezeIndex, index];
    }
    else
    {
      numArray = new float[length1];
      for (int index = 0; index < length1; ++index)
        numArray[index] = array[index, squeezeIndex];
    }
    return numArray;
  }

  public static string Print2DArray(float[,] array)
  {
    string str = "";
    for (int index1 = 0; index1 < array.GetLength(0); ++index1)
    {
      str = "";
      for (int index2 = 0; index2 < array.GetLength(1); ++index2)
        str = str + (object) array[index1, index2] + ",";
    }
    return str;
  }

  public static void WriteImage(string fileName, float[,] imgData)
  {
    Texture2D tex = new Texture2D(32, 32);
    for (int x = 0; x < 32; ++x)
    {
      for (int y = 0; y < 32; ++y)
        tex.SetPixel(x, y, new Color(imgData[x, y], imgData[x, y], imgData[x, y]));
    }
    byte[] png = tex.EncodeToPNG();
    File.WriteAllBytes(Application.dataPath + "/PreviewImages/" + fileName + ".png", png);
  }

  public static void ImportImageData(
    ref List<float[,,]> trainingInputData,
    ref List<float[]> trainingOutputData,
    List<Texture2D[]> imageDataset,
    bool grayscale)
  {
    int length = 10;
    trainingInputData = new List<float[,,]>();
    trainingOutputData = new List<float[]>();
    int width = imageDataset[0][0].width;
    int height = imageDataset[0][0].height;
    for (int index1 = 0; index1 < imageDataset.Count; ++index1)
    {
      for (int index2 = 0; index2 < imageDataset[index1].Length; ++index2)
      {
        float[,,] predictionInputData = new float[1, 1, 1];
        Noedify_Utils.ImportImageData(ref predictionInputData, imageDataset[index1][index2], grayscale);
        float[] numArray = new float[length];
        numArray[index1] = 1f;
        trainingOutputData.Add(numArray);
        trainingInputData.Add(predictionInputData);
      }
    }
  }

  public static void ImportImageData(
    ref float[,,] predictionInputData,
    Texture2D imageDataset,
    bool grayscale)
  {
    int width = imageDataset.width;
    int height = imageDataset.height;
    predictionInputData = !grayscale ? new float[3, width, height] : new float[1, width, height];
    if (grayscale)
    {
      float num1 = 0.0f;
      float[,,] numArray = new float[1, height, width];
      for (int x = 0; x < width; ++x)
      {
        for (int y = 0; y < height; ++y)
        {
          Color pixel = imageDataset.GetPixel(x, y);
          float num2 = pixel.r + pixel.b + pixel.g;
          if ((double) num2 > (double) num1)
            num1 = num2;
          numArray[0, x, y] = num2;
        }
      }
      if ((double) num1 > 0.0)
      {
        for (int index1 = 0; index1 < width; ++index1)
        {
          for (int index2 = 0; index2 < height; ++index2)
            numArray[0, index1, index2] /= num1;
        }
      }
      predictionInputData = numArray;
    }
    else
    {
      float[] numArray1 = new float[3];
      float[,,] numArray2 = new float[3, height, width];
      for (int x = 0; x < width; ++x)
      {
        for (int y = 0; y < height; ++y)
        {
          Color pixel = imageDataset.GetPixel(x, y);
          if ((double) pixel.r > (double) numArray1[0])
            numArray1[0] = pixel.r;
          if ((double) pixel.g > (double) numArray1[1])
            numArray1[1] = pixel.g;
          if ((double) pixel.b > (double) numArray1[2])
            numArray1[2] = pixel.b;
          numArray2[0, x, y] = pixel.r;
          numArray2[1, x, y] = pixel.g;
          numArray2[2, x, y] = pixel.b;
        }
      }
      for (int index3 = 0; index3 < width; ++index3)
      {
        for (int index4 = 0; index4 < height; ++index4)
        {
          if ((double) numArray1[0] > 0.0)
            numArray2[0, index3, index4] /= numArray1[0];
          if ((double) numArray1[1] > 0.0)
            numArray2[1, index3, index4] /= numArray1[1];
          if ((double) numArray1[2] > 0.0)
            numArray2[2, index3, index4] /= numArray1[2];
        }
      }
      predictionInputData = numArray2;
    }
  }
}

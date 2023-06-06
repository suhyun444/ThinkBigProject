// Decompiled with JetBrains decompiler
// Type: NSAI_Manager
// Assembly: Noedify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8080F0DD-0A3F-460C-80E2-226F05F6E4A2
// Assembly location: D:\Unity\Github\RaisingDragons\RecognizeHandWrittenDigitsTest\Assets\Noedify\dll\Noedify.dll

using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class NSAI_Manager
{
  public static Noedify.Net Load(string modelName, string dir)
  {
    NSAI_Manager.SavedModel savedModel1 = new NSAI_Manager.SavedModel((Noedify.Net) null, "", new DateTime());
    if (string.IsNullOrEmpty(dir))
      dir = Application.persistentDataPath;
    NSAI_Manager.SavedModel savedModel2;
    if (File.Exists(dir + "/" + modelName + ".dat"))
    {
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      FileStream serializationStream = File.Open(dir + "/" + modelName + ".dat", FileMode.Open);
      savedModel2 = (NSAI_Manager.SavedModel) binaryFormatter.Deserialize((Stream) serializationStream);
      serializationStream.Close();
    }
    else
    {
      if (!File.Exists(dir + "/" + modelName))
        return (Noedify.Net) null;
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      FileStream serializationStream = File.Open(dir + "/" + modelName, FileMode.Open);
      savedModel2 = (NSAI_Manager.SavedModel) binaryFormatter.Deserialize((Stream) serializationStream);
      serializationStream.Close();
    }
    return savedModel2.model.ReturnNet();
  }

  public static bool ImportNetworkParameters(Noedify.Net net, string filename)
  {
    // string path = "Assets/Resources/Noedify/ModelParameterFiles/" + filename + ".txt";
    // if (!File.Exists(path))
    //   return false;
    // StreamReader streamReader = new StreamReader(path);
    TextAsset theList = (TextAsset)Resources.Load("Noedify/ModelParameterFiles/" + filename, typeof(TextAsset));
    string str = theList.text;
    StringReader streamReader = new StringReader(str);
    for (int index1 = 1; index1 < net.LayerCount(); ++index1)
    {
      Noedify.NN_Weights nnWeights;
      Noedify.NN_Biases nnBiases;
      if (net.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
      {
        nnWeights = new Noedify.NN_Weights(net.layers[index1], net.layers[index1 - 1], false);
        nnBiases = new Noedify.NN_Biases(net.layers[index1].conv2DLayer.no_filters, false);
      }
      else
      {
        nnWeights = new Noedify.NN_Weights(net.layers[index1].layerSize, net.layers[index1 - 1].layerSize, false);
        nnBiases = new Noedify.NN_Biases(net.layers[index1].layerSize, false);
      }
      if (net.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
      {
        if (net.layers[index1 - 1].layer_type == Noedify.LayerType.Input2D)
        {
          for (int index2 = 0; index2 < net.layers[index1].conv2DLayer.no_filters; ++index2)
          {
            char[] chArray = new char[net.layers[index1].layerSize];
            for (int index3 = 0; index3 < net.layers[index1].layerSize; ++index3)
              chArray[index3] = ',';
            string[] strArray = streamReader.ReadLine().Split(chArray);
            if (strArray.Length != net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1])
            {
              Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " kernal size incorrect. Expected " + (object) (net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1]) + " weights/filter found: " + (object) strArray.Length + " weights/filter"));
              return false;
            }
            for (int index4 = 0; index4 < net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1]; ++index4)
              float.TryParse(strArray[index4], out net.layers[index1].weights.valuesConv2D[index2, index4]);
          }
          if (streamReader.ReadLine() != "*")
          {
            Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (too many weights)"));
            return false;
          }
          for (int index5 = 0; index5 < net.layers[index1].conv2DLayer.no_filters; ++index5)
          {
            string s = streamReader.ReadLine();
            if (s == "***")
            {
              Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (too many biases in parameter file)"));
              return false;
            }
            float.TryParse(s, out net.layers[index1].biases.valuesConv2D[index5]);
          }
          if (streamReader.ReadLine() != "***")
          {
            Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (not enough biases in parameter file)"));
            return false;
          }
        }
        else if (net.layers[index1 - 1].layer_type == Noedify.LayerType.Convolutional2D)
        {
          for (int index6 = 0; index6 < net.layers[index1].conv2DLayer.no_filters; ++index6)
          {
            char[] chArray = new char[net.layers[index1].layerSize];
            for (int index7 = 0; index7 < net.layers[index1].layerSize; ++index7)
              chArray[index7] = ',';
            string[] strArray = streamReader.ReadLine().Split(chArray);
            if (strArray.Length != net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1] * net.layers[index1 - 1].conv2DLayer.no_filters)
            {
              Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " kernal size incorrect. Expected " + (object) (net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1]) + " weights/filter found: " + (object) strArray.Length + " weights/filter"));
              return false;
            }
            for (int index8 = 0; index8 < net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1]; ++index8)
              float.TryParse(strArray[index8], out net.layers[index1].weights.valuesConv2D[index6, index8]);
          }
          if (streamReader.ReadLine() != "*")
          {
            Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (too many weights)"));
            return false;
          }
          for (int index9 = 0; index9 < net.layers[index1].conv2DLayer.no_filters; ++index9)
          {
            string s = streamReader.ReadLine();
            if (s == "***")
            {
              Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (too many biases in parameter file)"));
              return false;
            }
            float.TryParse(s, out net.layers[index1].biases.valuesConv2D[index9]);
          }
          if (streamReader.ReadLine() != "***")
          {
            Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (not enough biases in parameter file)"));
            return false;
          }
        }
      }
      else if (net.layers[index1].layer_type == Noedify.LayerType.FullyConnected | net.layers[index1].layer_type == Noedify.LayerType.Output)
      {
        for (int index10 = 0; index10 < net.layers[index1].layerSize; ++index10)
        {
          char[] chArray = new char[net.layers[index1].layerSize];
          for (int index11 = 0; index11 < net.layers[index1].layerSize; ++index11)
            chArray[index11] = ',';
          string[] strArray = streamReader.ReadLine().Split(chArray);
          if (strArray.Length != net.layers[index1 - 1].layerSize)
          {
            Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " previous layer size incorrect. Expected: " + (object) net.layers[index1 - 1].layerSize + " weights, found: " + (object) strArray.Length + " weights."));
            return false;
          }
          for (int index12 = 0; index12 < net.layers[index1 - 1].layerSize; ++index12)
            float.TryParse(strArray[index12], out net.layers[index1].weights.values[index12, index10]);
        }
        if (streamReader.ReadLine() != "*")
        {
          Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (not enough weights in parameter file)"));
          return false;
        }
        for (int index13 = 0; index13 < net.layers[index1].layerSize; ++index13)
        {
          string s = streamReader.ReadLine();
          if (s == "***")
          {
            Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (too many biases in parameter file)"));
            return false;
          }
          float.TryParse(s, out net.layers[index1].biases.values[index13]);
        }
        if (streamReader.ReadLine() != "***")
        {
          Debug.Log((object) ("Import network parameters failed: layer " + (object) index1 + " layer size incorrect (not enough biases in parameter file)"));
          return false;
        }
      }
    }
    return true;
  }

  public static void ExportNetworkParameters(Noedify.Net net, string fileName)
  {
    StreamWriter streamWriter = new StreamWriter("Assets/Resources/NodeAI/ExportedModels/" + fileName + ".txt", false);
    for (int index1 = 1; index1 < net.LayerCount(); ++index1)
    {
      if (net.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
      {
        if (net.layers[index1 - 1].layer_type == Noedify.LayerType.Input2D)
        {
          for (int index2 = 0; index2 < net.layers[index1].conv2DLayer.no_filters; ++index2)
          {
            string str1 = "";
            for (int index3 = 0; index3 < net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1]; ++index3)
              str1 = str1 + (object) net.layers[index1].weights.valuesConv2D[index2, index3] + ",";
            string str2 = str1.Remove(str1.Length - 1);
            streamWriter.WriteLine(str2);
          }
          streamWriter.WriteLine("*");
          for (int index4 = 0; index4 < net.layers[index1].conv2DLayer.no_filters; ++index4)
            streamWriter.WriteLine(net.layers[index1].biases.valuesConv2D[index4]);
        }
        else if (net.layers[index1 - 1].layer_type == Noedify.LayerType.Convolutional2D)
        {
          int noFilters = net.layers[index1 - 1].conv2DLayer.no_filters;
          for (int index5 = 0; index5 < net.layers[index1].conv2DLayer.no_filters; ++index5)
          {
            string str3 = "";
            for (int index6 = 0; index6 < noFilters * net.layers[index1].conv2DLayer.filterSize[0] * net.layers[index1].conv2DLayer.filterSize[1]; ++index6)
              str3 = str3 + (object) net.layers[index1].weights.valuesConv2D[index5, index6] + ",";
            string str4 = str3.Remove(str3.Length - 1);
            streamWriter.WriteLine(str4);
          }
          streamWriter.WriteLine("*");
          for (int index7 = 0; index7 < net.layers[index1].conv2DLayer.no_filters; ++index7)
            streamWriter.WriteLine(net.layers[index1].biases.valuesConv2D[index7]);
        }
      }
      else if (net.layers[index1].layer_type == Noedify.LayerType.FullyConnected | net.layers[index1].layer_type == Noedify.LayerType.Output)
      {
        for (int index8 = 0; index8 < net.layers[index1].layerSize; ++index8)
        {
          string str5 = "";
          for (int index9 = 0; index9 < net.layers[index1 - 1].layerSize; ++index9)
            str5 = str5 + (object) net.layers[index1].weights.values[index9, index8] + ",";
          string str6 = str5.Remove(str5.Length - 2);
          streamWriter.WriteLine(str6);
        }
        streamWriter.WriteLine("*");
        for (int index10 = 0; index10 < net.layers[index1].layerSize; ++index10)
          streamWriter.WriteLine(net.layers[index1].biases.values[index10]);
      }
      streamWriter.WriteLine("***");
    }
    streamWriter.Close();
  }

  [Serializable]
  public class SerializedModel
  {
    public List<Noedify.Layer> layers;
    public float trainingRate;
    public Noedify_Solver.CostFunction costFunction;
    public int total_no_nodes;
    public int total_no_activeNodes;
    public int total_no_weights;
    public int total_no_biases;

    public SerializedModel(Noedify.Net model)
    {
      if (model == null)
        return;
      this.layers = model.layers;
      this.total_no_nodes = model.total_no_nodes;
      this.total_no_activeNodes = model.total_no_activeNodes;
      this.total_no_weights = model.total_no_weights;
      this.total_no_biases = model.total_no_biases;
    }

    public Noedify.Net ReturnNet() => new Noedify.Net()
    {
      layers = this.layers,
      total_no_nodes = this.total_no_nodes,
      total_no_activeNodes = this.total_no_activeNodes,
      total_no_weights = this.total_no_weights,
      total_no_biases = this.total_no_biases
    };
  }

  [Serializable]
  public class SavedModel
  {
    public NSAI_Manager.SerializedModel model;
    public string modelName;
    public string dir;
    public string dateCreated;

    public SavedModel(Noedify.Net newModel, string newModelName, DateTime creationDate)
    {
      this.model = new NSAI_Manager.SerializedModel(newModel);
      this.modelName = newModelName;
      this.dateCreated = DateTime.Now.Year.ToString() + "-" + (object) DateTime.Now.Month + "-" + (object) DateTime.Now.Day;
    }

    public void Save(string dir_save = "")
    {
      string modelName = this.modelName;
      this.dir = !string.IsNullOrEmpty(dir_save) ? dir_save : Application.persistentDataPath;
      BinaryFormatter binaryFormatter = new BinaryFormatter();
      FileStream serializationStream = File.Create(this.dir + "/" + modelName + ".dat");
      NSAI_Manager.SavedModel graph = this;
      binaryFormatter.Serialize((Stream) serializationStream, (object) graph);
      serializationStream.Close();
    }
  }
}

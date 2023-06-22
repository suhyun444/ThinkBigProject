// Decompiled with JetBrains decompiler
// Type: Noedify
// Assembly: Noedify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8080F0DD-0A3F-460C-80E2-226F05F6E4A2
// Assembly location: D:\Unity\Github\RaisingDragons\RecognizeHandWrittenDigitsTest\Assets\Noedify\dll\Noedify.dll

using System;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class Noedify : MonoBehaviour
{
  public static void PrintArrayLine(string name, float[] array, bool includeIndex = false)
  {
    MonoBehaviour.print((object) name);
    string message = "";
    for (int index = 0; index < array.Length; ++index)
    {
      if (includeIndex)
        message = message + "(" + (object) index + ")";
      message = message + (object) array[index] + ", ";
    }
    MonoBehaviour.print((object) message);
  }

  public static void PrintArrayLine(
    string name,
    NativeArray<float> array,
    int[] bounds,
    bool includeIndex = false)
  {
    string message = name + ": ";
    for (int bound = bounds[0]; bound < bounds[1]; ++bound)
    {
      if (includeIndex)
        message = message + "(" + (object) (bound - bounds[0]) + ")";
      message = message + (object) array[bound] + ", ";
    }
    MonoBehaviour.print((object) message);
  }

  public static void PrintTrainingSet(List<float[,,]> trainingSet, int size1D)
  {
    for (int index1 = 0; index1 < trainingSet.Count; ++index1)
    {
      string message = "Training Input " + (object) index1 + ": ";
      for (int index2 = 0; index2 < size1D; ++index2)
        message = message + (object) trainingSet[index1][0, 0, index2] + " ";
      MonoBehaviour.print((object) message);
    }
  }

  public static void PrintTrainingSet(List<float[]> trainingSet)
  {
    for (int index1 = 0; index1 < trainingSet.Count; ++index1)
    {
      string message = "Training Output " + (object) index1 + ": ";
      for (int index2 = 0; index2 < trainingSet[index1].Length; ++index2)
        message = message + (object) trainingSet[index1][index2] + " ";
      MonoBehaviour.print((object) message);
    }
  }

  public static Noedify_Solver CreateSolver()
  {
    Noedify_Solver solver = new GameObject("Noedify Solver").AddComponent<Noedify_Solver>();
    solver.evaluationInProgress = false;
    solver.trainingInProgress = false;
    return solver;
  }

  public static void DestroySolver(Noedify_Solver solver) => UnityEngine.Object.Destroy((UnityEngine.Object) solver.gameObject);

  public enum LayerType
  {
    Input,
    Input2D,
    Output,
    FullyConnected,
    Convolutional2D,
    Pool2D,
  }

  public enum ActivationFunction
  {
    Sigmoid,
    ReLU,
    LeakyReLU,
    Linear,
    SoftMax,
    ELU,
    Hard_sigmoid,
    Tanh,
  }

  public enum PoolingType
  {
    Max,
    Avg,
  }

  [Serializable]
  public class Layer
  {
    public Noedify.LayerType layer_type;
    public string name;
    public int layerSize;
    public int[] layerSize2D;
    public int channels;
    public int layer_no;
    public bool trainingActive;
    public Noedify.NN_Weights weights;
    public Noedify.NN_Biases biases;
    public Noedify.ActivationFunction activationFunction;
    public Noedify.PoolingType pool_type;
    public Noedify_Convolutional2D.Convolutional2DLayer conv2DLayer;

    public Layer(Noedify.LayerType newType, int newLayerSize, string newLayerName = "")
    {
      if (newType != Noedify.LayerType.Convolutional2D)
      {
        this.name = newLayerName;
        this.layer_type = newType;
        this.layerSize = newLayerSize;
        this.layerSize2D = new int[2];
        this.layerSize2D[0] = this.layerSize;
        this.layerSize2D[1] = 1;
        this.channels = 1;
      }
      else
        MonoBehaviour.print((object) ("Error adding layer " + this.name));
    }

    public Layer(
      Noedify.LayerType newType,
      int[] inputSize2D,
      int noChannels,
      string newLayerName = "")
    {
      if (newType == Noedify.LayerType.Input2D)
      {
        this.name = newLayerName;
        this.channels = noChannels;
        this.layer_type = Noedify.LayerType.Input2D;
        this.layerSize2D = inputSize2D;
        this.layerSize = this.layerSize2D[0] * this.layerSize2D[1] * this.channels;
      }
      else
        MonoBehaviour.print((object) ("Error adding layer " + this.name));
    }

    public Layer(
      Noedify.LayerType newType,
      int newLayerSize,
      Noedify.ActivationFunction actFunction = Noedify.ActivationFunction.Sigmoid,
      string newLayerName = "")
    {
      if (newType != Noedify.LayerType.Convolutional2D)
      {
        this.name = newLayerName;
        this.layer_type = newType;
        this.layerSize = newLayerSize;
        this.layerSize2D = new int[2];
        this.layerSize2D[0] = this.layerSize;
        this.layerSize2D[1] = 1;
        this.channels = 1;
        this.activationFunction = actFunction;
      }
      else
        MonoBehaviour.print((object) ("Error adding layer " + this.name));
    }

    public Layer(
      Noedify.LayerType newType,
      Noedify.Layer previousLayer,
      int[] filtsize,
      int[] strd,
      int nfilters,
      int[] pdding,
      Noedify.ActivationFunction actFunction = Noedify.ActivationFunction.Sigmoid,
      string newLayerName = "")
    {
      if (newType == Noedify.LayerType.Convolutional2D)
      {
        this.name = newLayerName;
        this.conv2DLayer = new Noedify_Convolutional2D.Convolutional2DLayer();
        this.channels = 1;
        this.layer_type = newType;
        this.conv2DLayer.stride = strd;
        this.activationFunction = actFunction;
        this.conv2DLayer.no_filters = previousLayer.layer_type != Noedify.LayerType.Input2D ? nfilters : nfilters * previousLayer.channels;
        this.conv2DLayer.filterSize = filtsize;
        this.conv2DLayer.padding = pdding;
        this.layerSize2D = new int[2];
        this.layerSize2D[0] = Mathf.CeilToInt((float) (((double) previousLayer.layerSize2D[0] + 2.0 * (double) this.conv2DLayer.padding[0] - (double) this.conv2DLayer.filterSize[0]) / (double) this.conv2DLayer.stride[0] + 1.0));
        this.layerSize2D[1] = Mathf.CeilToInt((float) (((double) previousLayer.layerSize2D[1] + 2.0 * (double) this.conv2DLayer.padding[1] - (double) this.conv2DLayer.filterSize[1]) / (double) this.conv2DLayer.stride[1] + 1.0));
        this.layerSize = this.layerSize2D[0] * this.layerSize2D[1] * this.channels * this.conv2DLayer.no_filters;
      }
      else
        MonoBehaviour.print((object) ("Error adding layer " + this.name));
    }

    public Layer(
      Noedify.LayerType newType,
      Noedify.Layer previousLayer,
      int[] shape,
      int[] strd,
      int[] pdding,
      Noedify.PoolingType pooling_type,
      string newLayerName = "")
    {
      if (newType == Noedify.LayerType.Pool2D)
      {
        this.name = newLayerName;
        this.conv2DLayer = new Noedify_Convolutional2D.Convolutional2DLayer();
        this.channels = 1;
        this.layer_type = newType;
        this.conv2DLayer.stride = strd;
        this.pool_type = pooling_type;
        this.activationFunction = Noedify.ActivationFunction.Linear;
        if (previousLayer.layer_type != Noedify.LayerType.Convolutional2D)
        {
          MonoBehaviour.print((object) "Error: Pool2D layer must appear after a convolutional2D layer");
        }
        else
        {
          this.conv2DLayer.filterSize[0] = shape[0];
          this.conv2DLayer.filterSize[1] = shape[1];
          this.conv2DLayer.padding = pdding;
          this.layerSize2D = new int[2];
          this.layerSize2D[0] = Mathf.CeilToInt((float) (((double) previousLayer.layerSize2D[0] + 2.0 * (double) this.conv2DLayer.padding[0] - (double) this.conv2DLayer.filterSize[0]) / (double) this.conv2DLayer.stride[0] + 1.0));
          this.layerSize2D[1] = Mathf.CeilToInt((float) (((double) previousLayer.layerSize2D[1] + 2.0 * (double) this.conv2DLayer.padding[1] - (double) this.conv2DLayer.filterSize[1]) / (double) this.conv2DLayer.stride[1] + 1.0));
          this.layerSize = this.layerSize2D[0] * this.layerSize2D[1] * previousLayer.conv2DLayer.no_filters;
          this.conv2DLayer.no_filters = previousLayer.conv2DLayer.no_filters;
        }
      }
      else
        MonoBehaviour.print((object) ("Error adding layer " + newLayerName));
    }
  }

  [Serializable]
  public class NN_Weights
  {
    public int count;
    public int count_prevLyr;
    public float[,] values;
    public float[,] valuesConv2D;

    public NN_Weights(int newCount, int newCountPrevLyr, bool randomize)
    {
      this.count = newCount;
      this.count_prevLyr = newCountPrevLyr;
      this.values = new float[this.count_prevLyr, this.count];
      if (!randomize)
        return;
      for (int index1 = 0; index1 < this.count; ++index1)
      {
        for (int index2 = 0; index2 < this.count_prevLyr; ++index2)
          this.values[index2, index1] = (float) (((double) UnityEngine.Random.Range(0.0f, 10f) - 5.0) / 20.0);
      }
    }

    public NN_Weights(Noedify.Layer layer, Noedify.Layer previousLayer, bool randomize)
    {
      this.count = layer.layerSize;
      int num = 1;
      if (previousLayer.layer_type == Noedify.LayerType.Convolutional2D | previousLayer.layer_type == Noedify.LayerType.Pool2D)
        num = previousLayer.conv2DLayer.no_filters;
      this.valuesConv2D = new float[layer.conv2DLayer.no_filters, num * layer.conv2DLayer.filterSize[1] * layer.conv2DLayer.filterSize[0]];
      if (!randomize)
        return;
      for (int index1 = 0; index1 < num; ++index1)
      {
        for (int index2 = 0; index2 < layer.conv2DLayer.no_filters; ++index2)
        {
          for (int index3 = 0; index3 < layer.conv2DLayer.filterSize[1]; ++index3)
          {
            for (int index4 = 0; index4 < layer.conv2DLayer.filterSize[0]; ++index4)
              this.valuesConv2D[index2, index1 * (layer.conv2DLayer.filterSize[1] * layer.conv2DLayer.filterSize[0]) + index4 * layer.conv2DLayer.filterSize[0] + index3] = (float) (((double) UnityEngine.Random.Range(0.0f, 10f) - 5.0) / 30.0);
          }
        }
      }
    }

    public void GaussianInitialization()
    {
    }

    public void TestInitialize()
    {
      if (this.valuesConv2D != null)
      {
        for (int index1 = 0; index1 < 4; ++index1)
        {
          for (int index2 = 0; index2 < 3; ++index2)
          {
            for (int index3 = 0; index3 < 3; ++index3)
              this.valuesConv2D[index1, index2 * 3 + index3] = 0.0f;
          }
        }
      }
      else
      {
        for (int index4 = 0; index4 < this.count; ++index4)
        {
          for (int index5 = 0; index5 < this.count_prevLyr; ++index5)
            this.values[index5, index4] = index4 != index5 ? 0.0f : 1f;
        }
      }
    }
  }

  [Serializable]
  public class NN_Biases
  {
    public int count;
    public float[] values;
    public float[] valuesConv2D;

    public NN_Biases(int newCount, bool randomize)
    {
      this.count = newCount;
      this.values = new float[this.count];
      if (!randomize)
        return;
      for (int index = 0; index < this.count; ++index)
        this.values[index] = (float) (((double) UnityEngine.Random.Range(0.0f, 10f) - 5.0) / 20.0);
    }

    public NN_Biases(int newCount, int no_filters, bool randomize)
    {
      this.count = newCount;
      this.valuesConv2D = new float[no_filters];
      if (!randomize)
        return;
      for (int index = 0; index < no_filters; ++index)
        this.valuesConv2D[index] = UnityEngine.Random.Range(0.0f, 10f) / 10f;
    }

    public void GaussianInitialization()
    {
    }

    public void TestInitialize()
    {
      if (this.valuesConv2D != null)
      {
        for (int index = 0; index < 4; ++index)
          this.valuesConv2D[index] = 0.0f;
      }
      else
      {
        for (int index = 0; index < this.count; ++index)
          this.values[index] = 0.0f;
      }
    }
  }

  [Serializable]
  public class NN_LayerGradients
  {
    public int no_nodes;
    public Noedify.NN_Weights weight_gradients;
    public Noedify.NN_Biases bias_gradients;

    public NN_LayerGradients(List<Noedify.Layer> layers, int layer_no)
    {
      this.no_nodes = layers[layer_no].layerSize;
      if (layers[layer_no].layer_type == Noedify.LayerType.Convolutional2D)
      {
        this.weight_gradients = new Noedify.NN_Weights(layers[layer_no], layers[layer_no - 1], false);
        this.bias_gradients = new Noedify.NN_Biases(this.no_nodes, layers[layer_no].conv2DLayer.no_filters, false);
      }
      else
      {
        this.weight_gradients = new Noedify.NN_Weights(this.no_nodes, layers[layer_no - 1].layerSize, false);
        this.bias_gradients = new Noedify.NN_Biases(this.no_nodes, false);
      }
    }
  }

  [Serializable]
  public class Net
  {
    public List<Noedify.Layer> layers;
    public bool trainingInProgress;
    public int total_no_nodes;
    public int total_no_activeNodes;
    public int total_no_weights;
    public int total_no_biases;
    public bool nativeArraysInitialized;
    public NativeArray<float> PARA0000x1;
    public NativeArray<float> PARA0000x2;
    public NativeArray<float> PARA0000x3;
    public NativeArray<float> PARA0000x4;
    public NativeArray<int> PARA0000x5;
    public NativeArray<int> PARA0000x6;
    public NativeArray<int> PARA0000x7;
    public NativeArray<int> PARA0000x8;
    public NativeArray<int> PARA0000x9;
    public NativeArray<int> nodeIndeces_start;
    public NativeArray<int> VG0000x61_par;
    public NativeArray<int> VG0000x61Indeces_start;

    public Net()
    {
      this.layers = new List<Noedify.Layer>();
      this.nativeArraysInitialized = false;
    }

    public Noedify.Layer AddLayer(Noedify.Layer new_layer)
    {
      if ((new_layer.layer_type == Noedify.LayerType.Input | new_layer.layer_type == Noedify.LayerType.Input2D) & (uint) this.LayerCount() > 0U)
        MonoBehaviour.print((object) "Warning: Input layer can only be added first");
      else if ((uint) new_layer.layer_type > 0U & new_layer.layer_type != Noedify.LayerType.Input2D & this.LayerCount() == 0)
        MonoBehaviour.print((object) "Warning: Input layer must be added first");
      else if (new_layer.layer_type == Noedify.LayerType.Convolutional2D & this.LayerCount() > 0)
      {
        if (this.layers[this.LayerCount() - 1].layer_type != Noedify.LayerType.Input2D & this.layers[this.LayerCount() - 1].layer_type != Noedify.LayerType.Convolutional2D & this.layers[this.LayerCount() - 1].layer_type != Noedify.LayerType.Pool2D)
        {
          MonoBehaviour.print((object) "Warning: Can only add convolutional layer after 2D layer");
        }
        else
        {
          new_layer.layer_no = this.LayerCount();
          this.layers.Add(new_layer);
        }
      }
      else if (new_layer.layer_type == Noedify.LayerType.Pool2D)
      {
        if (this.layers[this.LayerCount() - 1].layer_type != Noedify.LayerType.Convolutional2D)
        {
          MonoBehaviour.print((object) ("Warning: Pool2D layer " + new_layer.name + " can only be added after a Convolutional2D layer."));
        }
        else
        {
          new_layer.layer_no = this.LayerCount();
          this.layers.Add(new_layer);
        }
      }
      else
      {
        new_layer.layer_no = this.LayerCount();
        this.layers.Add(new_layer);
      }
      return new_layer;
    }

    public void BuildNetwork()
    {
      if (this.LayerCount() > 0)
      {
        if (this.layers[this.LayerCount() - 1].layer_type == Noedify.LayerType.Output)
        {
          int totalNoBiases = this.Get_Total_No_Biases();
          int totalNoWeights = this.Get_Total_No_Weights();
          MonoBehaviour.print((object) ("Building network with " + (object) (this.LayerCount() - 2) + " hidden layers, " + (object) totalNoBiases + " biases, and " + (object) totalNoWeights + " weights"));
          for (int index = 1; index < this.LayerCount(); ++index)
          {
            if (this.layers[index].layer_type == Noedify.LayerType.FullyConnected | this.layers[index].layer_type == Noedify.LayerType.Output)
            {
              this.layers[index].weights = new Noedify.NN_Weights(this.layers[index].layerSize, this.layers[index - 1].layerSize, true);
              this.layers[index].biases = new Noedify.NN_Biases(this.layers[index].layerSize, true);
            }
            else if (this.layers[index].layer_type == Noedify.LayerType.Convolutional2D)
            {
              int num = 1;
              if (this.layers[index - 1].layer_type == Noedify.LayerType.Convolutional2D | this.layers[index - 1].layer_type == Noedify.LayerType.Pool2D)
                num = this.layers[index - 1].conv2DLayer.no_filters;
              this.layers[index].conv2DLayer.BuildConnections(this.layers[index - 1], this.layers[index]);
              this.layers[index].weights = new Noedify.NN_Weights(this.layers[index], this.layers[index - 1], true);
              this.layers[index].biases = new Noedify.NN_Biases(this.layers[index].layerSize, this.layers[index].conv2DLayer.no_filters, true);
            }
            else if (this.layers[index].layer_type == Noedify.LayerType.Pool2D)
              this.layers[index].conv2DLayer.BuildConnectionsPool2D(this.layers[index - 1], this.layers[index]);
          }
          this.trainingInProgress = false;
        }
        else
          MonoBehaviour.print((object) "WARNING: BuildNetwork failed. Network must have an output layer at the end");
      }
      else
        MonoBehaviour.print((object) "WARNING: BuildNetwork failed. Network is empty");
    }

    public void SaveModel(string name, string dir = "") => new NSAI_Manager.SavedModel(this, name, DateTime.Now)
    {
      modelName = name,
      model = new NSAI_Manager.SerializedModel(this)
    }.Save(dir);

    public bool LoadModel(string name, string dir = "")
    {
      Noedify.Net net = NSAI_Manager.Load(name, dir);
      if (net == null)
      {
        MonoBehaviour.print((object) ("WARNING: Loading of " + name + " failed. File not found."));
        return false;
      }
      this.layers = net.layers;
      return true;
    }

    public void ApplyGradients(
      Noedify.NN_LayerGradients[] KLA0000x71,
      int batch_size,
      bool par = false,
      int fineTuningLayerLimit = 0)
    {
      if (!par)
      {
        for (int index1 = 1; index1 < this.LayerCount(); ++index1)
        {
          if (index1 >= fineTuningLayerLimit)
          {
            if (this.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
            {
              for (int index2 = 0; index2 < this.layers[index1].conv2DLayer.no_filters; ++index2)
              {
                for (int index3 = 0; index3 < this.layers[index1].conv2DLayer.N_weights_per_filter; ++index3)
                  this.layers[index1].weights.valuesConv2D[index2, index3] += KLA0000x71[index1 - 1].weight_gradients.valuesConv2D[index2, index3];
                this.layers[index1].biases.valuesConv2D[index2] += KLA0000x71[index1 - 1].bias_gradients.valuesConv2D[index2];
              }
            }
            else if (this.layers[index1].layer_type != Noedify.LayerType.Pool2D)
            {
              for (int index4 = 0; index4 < this.layers[index1].layerSize; ++index4)
              {
                for (int index5 = 0; index5 < this.layers[index1 - 1].layerSize; ++index5)
                  this.layers[index1].weights.values[index5, index4] += KLA0000x71[index1 - 1].weight_gradients.values[index5, index4];
                this.layers[index1].biases.values[index4] += KLA0000x71[index1 - 1].bias_gradients.values[index4];
              }
            }
          }
        }
      }
      else
      {
        for (int index6 = 1; index6 < this.LayerCount(); ++index6)
        {
          if (index6 >= fineTuningLayerLimit)
          {
            if (this.layers[index6].layer_type == Noedify.LayerType.Convolutional2D)
            {
              for (int index7 = 0; index7 < this.layers[index6].conv2DLayer.no_filters; ++index7)
              {
                for (int index8 = 0; index8 < this.layers[index6].conv2DLayer.N_weights_per_filter; ++index8)
                  this.PARA0000x1[this.PARA0000x5[index6 - 1] + index7 * this.layers[index6].conv2DLayer.N_weights_per_filter + index8] += KLA0000x71[index6 - 1].weight_gradients.valuesConv2D[index7, index8];
                this.PARA0000x2[this.PARA0000x6[index6 - 1] + index7] += KLA0000x71[index6 - 1].bias_gradients.valuesConv2D[index7];
              }
            }
            else if (this.layers[index6].layer_type != Noedify.LayerType.Pool2D)
            {
              for (int index9 = 0; index9 < this.layers[index6].layerSize; ++index9)
              {
                for (int index10 = 0; index10 < this.layers[index6 - 1].layerSize; ++index10)
                  this.PARA0000x1[this.PARA0000x5[index6 - 1] + index10 * this.layers[index6].layerSize + index9] += KLA0000x71[index6 - 1].weight_gradients.values[index10, index9];
                this.PARA0000x2[this.PARA0000x6[index6 - 1] + index9] += KLA0000x71[index6 - 1].bias_gradients.values[index9];
              }
            }
          }
        }
      }
    }

    public void GenerateNativeParameterArrays()
    {
      this.total_no_nodes = 0;
      this.total_no_activeNodes = 0;
      this.total_no_weights = 0;
      this.total_no_biases = 0;
      int length = 0;
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      List<int> intList3 = new List<int>();
      List<int> intList4 = new List<int>();
      List<int> intList5 = new List<int>();
      for (int index = 1; index < this.LayerCount(); ++index)
      {
        intList5.Add(length);
        if (this.layers[index].layer_type == Noedify.LayerType.Convolutional2D)
        {
          intList1.Add(this.total_no_weights);
          if (this.layers[index - 1].layer_type == Noedify.LayerType.Pool2D)
            this.total_no_weights += this.layers[index - 1].conv2DLayer.no_filters * this.layers[index].conv2DLayer.filterSize[0] * this.layers[index].conv2DLayer.filterSize[1] * this.layers[index].conv2DLayer.no_filters;
          else
            this.total_no_weights += this.layers[index].conv2DLayer.no_filters * this.layers[index].conv2DLayer.N_weights_per_filter;
          intList2.Add(this.total_no_biases);
          this.total_no_biases += this.layers[index].conv2DLayer.no_filters;
          length += this.layers[index].layerSize * this.layers[index - 1].layerSize;
        }
        else if (this.layers[index].layer_type == Noedify.LayerType.Pool2D)
        {
          length += this.layers[index].layerSize * this.layers[index - 1].layerSize;
          intList1.Add(this.total_no_weights);
          intList2.Add(this.total_no_biases);
        }
        else
        {
          intList1.Add(this.total_no_weights);
          this.total_no_weights += this.layers[index - 1].layerSize * this.layers[index].layerSize;
          intList2.Add(this.total_no_biases);
          this.total_no_biases += this.layers[index].layerSize;
        }
        intList3.Add(this.total_no_activeNodes);
        this.total_no_activeNodes += this.layers[index].layerSize;
      }
      for (int index = 0; index < this.LayerCount(); ++index)
      {
        intList4.Add(this.total_no_nodes);
        this.total_no_nodes += this.layers[index].layerSize;
      }
      this.PARA0000x1 = new NativeArray<float>(this.total_no_weights, Allocator.Persistent);
      this.PARA0000x2 = new NativeArray<float>(this.total_no_biases, Allocator.Persistent);
      this.VG0000x61_par = new NativeArray<int>(length, Allocator.Persistent);
      this.PARA0000x5 = new NativeArray<int>(intList1.Count, Allocator.Persistent);
      for (int index = 0; index < intList1.Count; ++index)
        this.PARA0000x5[index] = intList1[index];
      this.PARA0000x6 = new NativeArray<int>(intList2.Count, Allocator.Persistent);
      for (int index = 0; index < intList2.Count; ++index)
        this.PARA0000x6[index] = intList2[index];
      this.PARA0000x9 = new NativeArray<int>(intList3.Count, Allocator.Persistent);
      for (int index = 0; index < intList3.Count; ++index)
        this.PARA0000x9[index] = intList3[index];
      this.nodeIndeces_start = new NativeArray<int>(intList4.Count, Allocator.Persistent);
      for (int index = 0; index < intList4.Count; ++index)
        this.nodeIndeces_start[index] = intList4[index];
      this.VG0000x61Indeces_start = new NativeArray<int>(intList5.Count, Allocator.Persistent);
      for (int index = 0; index < intList5.Count; ++index)
        this.VG0000x61Indeces_start[index] = intList5[index];
      for (int index1 = 1; index1 < this.LayerCount(); ++index1)
      {
        if (this.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
        {
          for (int index2 = 0; index2 < this.layers[index1].conv2DLayer.no_filters; ++index2)
          {
            for (int index3 = 0; index3 < this.layers[index1].conv2DLayer.N_weights_per_filter; ++index3)
              this.PARA0000x1[this.PARA0000x5[index1 - 1] + index2 * this.layers[index1].conv2DLayer.N_weights_per_filter + index3] = this.layers[index1].weights.valuesConv2D[index2, index3];
          }
          for (int index4 = 0; index4 < this.layers[index1].conv2DLayer.no_filters; ++index4)
            this.PARA0000x2[this.PARA0000x6[index1 - 1] + index4] = this.layers[index1].biases.valuesConv2D[index4];
          for (int index5 = 0; index5 < this.layers[index1 - 1].layerSize; ++index5)
          {
            for (int index6 = 0; index6 < this.layers[index1].layerSize; ++index6)
              this.VG0000x61_par[this.VG0000x61Indeces_start[index1 - 1] + index5 * this.layers[index1].layerSize + index6] = this.layers[index1].conv2DLayer.VG0000x61[index5, index6];
          }
        }
        else if (this.layers[index1].layer_type == Noedify.LayerType.Pool2D)
        {
          for (int index7 = 0; index7 < this.layers[index1 - 1].layerSize; ++index7)
          {
            for (int index8 = 0; index8 < this.layers[index1].layerSize; ++index8)
              this.VG0000x61_par[this.VG0000x61Indeces_start[index1 - 1] + index7 * this.layers[index1].layerSize + index8] = this.layers[index1].conv2DLayer.VG0000x61[index7, index8];
          }
        }
        else
        {
          for (int index9 = 0; index9 < this.layers[index1 - 1].layerSize; ++index9)
          {
            for (int index10 = 0; index10 < this.layers[index1].layerSize; ++index10)
              this.PARA0000x1[this.PARA0000x5[index1 - 1] + index9 * this.layers[index1].layerSize + index10] = this.layers[index1].weights.values[index9, index10];
          }
          for (int index11 = 0; index11 < this.layers[index1].layerSize; ++index11)
            this.PARA0000x2[this.PARA0000x6[index1 - 1] + index11] = this.layers[index1].biases.values[index11];
        }
      }
      this.nativeArraysInitialized = true;
    }

    public void OffloadNativeParameterArrays()
    {
      this.total_no_nodes = 0;
      this.total_no_activeNodes = 0;
      this.total_no_weights = 0;
      this.total_no_biases = 0;
      List<int> intList1 = new List<int>();
      List<int> intList2 = new List<int>();
      List<int> intList3 = new List<int>();
      List<int> intList4 = new List<int>();
      for (int index1 = 1; index1 < this.LayerCount(); ++index1)
      {
        if (this.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
        {
          for (int index2 = 0; index2 < this.layers[index1].conv2DLayer.no_filters; ++index2)
          {
            for (int index3 = 0; index3 < this.layers[index1].conv2DLayer.N_weights_per_filter; ++index3)
              this.layers[index1].weights.valuesConv2D[index2, index3] = this.PARA0000x1[this.PARA0000x5[index1 - 1] + index2 * this.layers[index1].conv2DLayer.N_weights_per_filter + index3];
          }
          for (int index4 = 0; index4 < this.layers[index1].conv2DLayer.no_filters; ++index4)
            this.layers[index1].biases.valuesConv2D[index4] = this.PARA0000x2[this.PARA0000x6[index1 - 1] + index4];
        }
        else if (this.layers[index1].layer_type != Noedify.LayerType.Pool2D)
        {
          for (int index5 = 0; index5 < this.layers[index1 - 1].layerSize; ++index5)
          {
            for (int index6 = 0; index6 < this.layers[index1].layerSize; ++index6)
              this.layers[index1].weights.values[index5, index6] = this.PARA0000x1[this.PARA0000x5[index1 - 1] + index5 * this.layers[index1].layerSize + index6];
          }
          for (int index7 = 0; index7 < this.layers[index1].layerSize; ++index7)
            this.layers[index1].biases.values[index7] = this.PARA0000x2[this.PARA0000x6[index1 - 1] + index7];
        }
      }
    }

    public void Cleanup_Par()
    {
      this.PARA0000x1.Dispose();
      this.PARA0000x2.Dispose();
      this.VG0000x61_par.Dispose();
      this.PARA0000x5.Dispose();
      this.PARA0000x6.Dispose();
      this.PARA0000x9.Dispose();
      this.nodeIndeces_start.Dispose();
      this.VG0000x61Indeces_start.Dispose();
      this.nativeArraysInitialized = false;
    }

    public int LayerCount() => this.layers.Count;

    public int Get_Total_No_Weights()
    {
      int totalNoWeights = 0;
      for (int index = 1; index < this.LayerCount(); ++index)
      {
        if (this.layers[index].layer_type == Noedify.LayerType.Convolutional2D & this.layers[index - 1].layer_type == Noedify.LayerType.Pool2D)
          totalNoWeights += this.layers[index - 1].conv2DLayer.no_filters * this.layers[index].conv2DLayer.filterSize[0] * this.layers[index].conv2DLayer.filterSize[1] * this.layers[index].conv2DLayer.no_filters;
        else if (this.layers[index].layer_type == Noedify.LayerType.Convolutional2D)
          totalNoWeights += this.layers[index].channels * this.layers[index].conv2DLayer.filterSize[0] * this.layers[index].conv2DLayer.filterSize[1] * this.layers[index].conv2DLayer.no_filters;
        else if (this.layers[index].layer_type != Noedify.LayerType.Pool2D)
          totalNoWeights += this.layers[index].layerSize * this.layers[index - 1].layerSize;
      }
      return totalNoWeights;
    }

    public int Get_Total_No_Biases()
    {
      int totalNoBiases = 0;
      for (int index = 1; index < this.LayerCount(); ++index)
      {
        if (this.layers[index].layer_type == Noedify.LayerType.Convolutional2D)
          totalNoBiases += this.layers[index].channels * this.layers[index].conv2DLayer.no_filters;
        else
          totalNoBiases += this.layers[index].layerSize;
      }
      return totalNoBiases;
    }
  }
}

// Decompiled with JetBrains decompiler
// Type: Noedify_Solver
// Assembly: Noedify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8080F0DD-0A3F-460C-80E2-226F05F6E4A2
// Assembly location: D:\Unity\Github\RaisingDragons\RecognizeHandWrittenDigitsTest\Assets\Noedify\dll\Noedify.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

public class Noedify_Solver : MonoBehaviour
{
  public bool trainingInProgress;
  public bool evaluationInProgress;
  public bool suppressMessages = false;
  public float[] cost_report;
  public float[] prediction;
  public float costThreshold = -100f;
  public JobHandle activeJob;
  public Noedify_Solver.DebugReport debug;
  public int fineTuningLayerLimit;
  private Coroutine CD0000x1;
  private Noedify.Net CD0000x5;
  private List<object> nativeArrayCleanupList;
  public List<Noedify.Net> nets_cleanup_list;

  public Noedify_Solver() => this.nets_cleanup_list = new List<Noedify.Net>();

  public void Evaluate(
    Noedify.Net net,
    float[,,] evaluationInputs,
    Noedify_Solver.SolverMethod solverMethod = Noedify_Solver.SolverMethod.MainThread)
  {
    this.nets_cleanup_list.Add(net);
    if (this.debug == null)
      this.debug = new Noedify_Solver.DebugReport();
    if (solverMethod == Noedify_Solver.SolverMethod.MainThread)
    {
      List<float[]> numArrayList1 = new List<float[]>();
      List<float[]> numArrayList2 = new List<float[]>();
      numArrayList1.Add(Noedify_Utils.FlattenDataset(evaluationInputs));
      numArrayList2.Add(new float[net.layers[0].layerSize]);
      if (net.layers[0].layer_type == Noedify.LayerType.Input & evaluationInputs.GetLength(2) != net.layers[0].layerSize & evaluationInputs.GetLength(1) != 1 & evaluationInputs.GetLength(0) != 1)
      {
        if (!this.suppressMessages)
          MonoBehaviour.print((object) "WARNING: Input wrong size");
        else if (net.layers[0].layer_type == Noedify.LayerType.Input2D & evaluationInputs.GetLength(2) != net.layers[0].layerSize2D[0] & evaluationInputs.GetLength(1) != net.layers[0].layerSize2D[1] & evaluationInputs.GetLength(0) != net.layers[0].channels && !this.suppressMessages)
          MonoBehaviour.print((object) "WARNING: Input wrong size");
      }
      for (int index = 1; index < net.LayerCount(); ++index)
      {
        float[] inputs = numArrayList1[index - 1];
        float[] JJR0000x18 = new float[net.layers[index].layerSize];
        float[] ZO0000x18 = new float[net.layers[index].layerSize];
        this.PROC0000x14(net, net.layers[index], inputs, ref JJR0000x18, ref ZO0000x18);
        numArrayList1.Add(JJR0000x18);
        numArrayList2.Add(ZO0000x18);
      }
      if (this.debug.print_nodeOutputs)
      {
        for (int index1 = 0; index1 < net.LayerCount(); ++index1)
        {
          string message = "layer " + (object) index1 + " node outputs: ";
          for (int index2 = 0; index2 < net.layers[index1].layerSize; ++index2)
            message = message + (object) numArrayList1[index1][index2] + ", ";
          MonoBehaviour.print((object) message);
        }
      }
      if (this.debug.print_nodeInputs)
      {
        for (int index3 = 0; index3 < net.LayerCount(); ++index3)
        {
          string message = "layer " + (object) index3 + " node inputs: ";
          for (int index4 = 0; index4 < net.layers[index3].layerSize; ++index4)
            message = message + (object) numArrayList2[index3][index4] + ", ";
          MonoBehaviour.print((object) message);
        }
      }
      this.prediction = numArrayList1[net.LayerCount() - 1];
    }
    else
    {
      this.CD0000x5 = net;
      this.evaluationInProgress = true;
      if (!net.trainingInProgress)
      {
        if (!this.CD0000x5.nativeArraysInitialized)
          net.GenerateNativeParameterArrays();
        this.CD0000x1 = this.StartCoroutine(this.PROC0000x15(net, evaluationInputs));
      }
      else
      {
        if (this.suppressMessages)
          return;
        MonoBehaviour.print((object) "Error: Evaluation initiated before previous job competed");
      }
    }
  }

  public void TrainNetwork(
    Noedify.Net net,
    List<float[,,]> trainingInputs,
    List<float[]> trainingOutputs,
    int no_epochs,
    int batch_size,
    float trainingRate,
    Noedify_Solver.CostFunction costFunction,
    Noedify_Solver.SolverMethod solverMethod,
    List<float> trainingSetWeighting = null,
    int N_threads = 8)
  {
    this.nets_cleanup_list.Add(net);
    if (this.debug == null)
      this.debug = new Noedify_Solver.DebugReport();
    if (trainingSetWeighting == null)
    {
      trainingSetWeighting = new List<float>();
      for (int index = 0; index < trainingInputs.Count; ++index)
        trainingSetWeighting.Add(1f);
    }
    else if (trainingSetWeighting.Count != trainingInputs.Count)
    {
      if (!this.suppressMessages)
        MonoBehaviour.print((object) ("Error: Number of training weights (" + (object) trainingSetWeighting.Count + ") doesn't match number of training input sets (" + (object) trainingInputs.Count + ")"));
      if (!this.suppressMessages)
        MonoBehaviour.print((object) "Warning: Ignoring trainingSetWeighting.");
      trainingSetWeighting = new List<float>();
      for (int index = 0; index < trainingInputs.Count; ++index)
        trainingSetWeighting.Add(1f);
    }
    else
    {
      for (int index = 0; index < trainingSetWeighting.Count; ++index)
      {
        if ((double) trainingSetWeighting[index] < -1.0)
          trainingSetWeighting[index] = -1f;
        else if ((double) trainingSetWeighting[index] > 1.0)
          trainingSetWeighting[index] = 1f;
      }
    }
    float num1 = trainingRate;
    if (solverMethod == Noedify_Solver.SolverMethod.MainThread)
    {
      int count = trainingInputs.Count;
      if (trainingInputs.Count != trainingOutputs.Count && !this.suppressMessages)
        MonoBehaviour.print((object) ("Error: Number of training input arrays (" + (object) trainingInputs.Count + ") doesn't match number of training output arrays (" + (object) trainingOutputs.Count + ")"));
      this.cost_report = new float[no_epochs];
      if (batch_size > count)
      {
        batch_size = count;
        if (!this.suppressMessages)
          MonoBehaviour.print((object) ("WARNING: batch size greater than number of training sets. batch_size reduced to " + (object) count));
      }
      if (batch_size > count)
        return;
      if (!this.suppressMessages)
        MonoBehaviour.print((object) ("Starting training for " + (object) no_epochs + " epochs with batch size " + (object) batch_size + " across " + (object) count + " training sets"));
      for (int index1 = 0; index1 < no_epochs; ++index1)
      {
        int[] inputArray = new int[count];
        for (int index2 = 0; index2 < count; ++index2)
          inputArray[index2] = index2;
        int[] numArray = Noedify_Utils.Shuffle(inputArray);
        List<Noedify.NN_LayerGradients[]> OU0000x2 = new List<Noedify.NN_LayerGradients[]>();
        float num2 = 0.0f;
        for (int index3 = 0; index3 < batch_size; ++index3)
        {
          int index4 = numArray[index3];
          List<float[]> ZO0000x18 = new List<float[]>();
          List<float[]> JJR0000x18 = new List<float[]>();
          this.PROC0000x18(net, trainingInputs[index4], ref JJR0000x18, ref ZO0000x18);
          OU0000x2.Add(this.PROC0000x19(net, costFunction, trainingOutputs[index4], JJR0000x18, ZO0000x18));
          num2 += Noedify_Solver.Cost(JJR0000x18[net.LayerCount() - 1], trainingOutputs[index4], costFunction) / (float) batch_size;
          if (this.debug.print_nodeOutputs)
          {
            for (int index5 = 0; index5 < net.LayerCount(); ++index5)
            {
              string message = "Epoch " + (object) index1 + ", layer " + (object) index5 + " node outputs: ";
              for (int index6 = 0; index6 < net.layers[index5].layerSize; ++index6)
                message = message + "(" + (object) index6 + ")" + (object) JJR0000x18[index5][index6] + ", ";
              MonoBehaviour.print((object) message);
            }
            MonoBehaviour.print((object) "------------------------------------");
          }
          if (this.debug.print_nodeInputs)
          {
            for (int index7 = 0; index7 < net.LayerCount(); ++index7)
            {
              string message = "layer " + (object) index7 + " node inputs: ";
              for (int index8 = 0; index8 < net.layers[index7].layerSize; ++index8)
                message = message + (object) ZO0000x18[index7][index8] + ", ";
              MonoBehaviour.print((object) message);
            }
          }
        }
        if (!this.suppressMessages & (index1 % 10 == 0 | index1 == no_epochs - 1))
          MonoBehaviour.print((object) ("epoch " + (object) (index1 + 1) + "/" + (object) no_epochs + ", cost: " + (object) num2));
        this.cost_report[index1] = num2;
        if (index1 <= 0 || (double) Mathf.Clamp(this.cost_report[index1 - 1] - this.cost_report[index1] / this.cost_report[index1 - 1], 0.0f, 1f) <= 0.200000002980232 || (double) trainingRate <= (double) num1 / 10.0)
          ;
        float[] OU0000x3 = new float[batch_size];
        for (int index9 = 0; index9 < batch_size; ++index9)
          OU0000x3[index9] = trainingSetWeighting[numArray[index9]];
        Noedify.NN_LayerGradients[] KLA0000x71 = this.PROC0000x20(net, trainingRate, OU0000x2, batch_size, OU0000x3);
        if (((this.debug.print_bias_Gradients ? 1 : 0) & (index1 % 10 == 0 ? 1 : (index1 == no_epochs - 1 ? 1 : 0))) != 0)
        {
          for (int index10 = 1; index10 < net.LayerCount(); ++index10)
          {
            string message = "Epoch " + (object) index1 + " Accumulated: bias gradients layer " + (object) index10 + ": ";
            if (net.layers[index10].layer_type == Noedify.LayerType.Convolutional2D)
            {
              for (int index11 = 0; index11 < net.layers[index10].conv2DLayer.no_filters; ++index11)
                message = message + "(b" + (object) index11 + "=" + (object) net.layers[index10].biases.valuesConv2D[index11] + ")" + (object) KLA0000x71[index10 - 1].bias_gradients.valuesConv2D[index11] + " ";
            }
            else if (net.layers[index10].layer_type != Noedify.LayerType.Pool2D)
            {
              for (int index12 = 0; index12 < net.layers[index10].layerSize; ++index12)
                message = message + "(b" + (object) index12 + "=" + (object) net.layers[index10].biases.values[index12] + ")" + (object) KLA0000x71[index10 - 1].bias_gradients.values[index12] + " ";
            }
            MonoBehaviour.print((object) message);
          }
        }
        if (((this.debug.print_weight_Gradients ? 1 : 0) & (index1 % 10 == 0 ? 1 : (index1 == no_epochs - 1 ? 1 : 0))) != 0)
        {
          for (int index13 = 1; index13 < net.LayerCount(); ++index13)
          {
            if (net.layers[index13].layer_type == Noedify.LayerType.Convolutional2D)
            {
              for (int index14 = 0; index14 < net.layers[index13].conv2DLayer.no_filters; ++index14)
              {
                string message = "Epoch " + (object) index1 + " layer " + (object) index13 + " from filter " + (object) index14 + ": ";
                for (int index15 = 0; index15 < net.layers[index13].conv2DLayer.N_weights_per_filter; ++index15)
                  message = message + "(w_" + (object) index14 + "_" + (object) index15 + "=" + (object) net.layers[index13].weights.valuesConv2D[index14, index15] + ")" + (object) KLA0000x71[index13 - 1].weight_gradients.valuesConv2D[index14, index15] + " ";
                MonoBehaviour.print((object) message);
              }
            }
            else if (net.layers[index13].layer_type != Noedify.LayerType.Pool2D)
            {
              for (int index16 = 0; index16 < net.layers[index13 - 1].layerSize && index16 <= 10; ++index16)
              {
                string message = "Epoch " + (object) index1 + " layer " + (object) index13 + " from node " + (object) index16 + ": ";
                for (int index17 = 0; index17 < net.layers[index13].layerSize; ++index17)
                  message = message + "(w_" + (object) index16 + "_" + (object) index17 + "=" + (object) net.layers[index13].weights.values[index16, index17] + ")" + (object) KLA0000x71[index13 - 1].weight_gradients.values[index16, index17] + " ";
                MonoBehaviour.print((object) message);
              }
            }
          }
        }
        net.ApplyGradients(KLA0000x71, batch_size, fineTuningLayerLimit: this.fineTuningLayerLimit);
        if ((double) num2 < (double) this.costThreshold)
        {
          if (!this.suppressMessages)
          {
            MonoBehaviour.print((object) ("Cost Threshold reached (" + (object) this.costThreshold + "). Training Complete."));
            break;
          }
          break;
        }
      }
    }
    else
    {
      this.trainingInProgress = true;
      if (!net.trainingInProgress)
      {
        int count = trainingInputs.Count;
        if (trainingInputs.Count != trainingOutputs.Count)
        {
          if (this.suppressMessages)
            return;
          MonoBehaviour.print((object) ("Error: Number of training input arrays (" + (object) trainingInputs.Count + ") doesn't match number of training output arrays (" + (object) trainingOutputs.Count + ")"));
        }
        else
        {
          this.cost_report = new float[no_epochs];
          if (batch_size > count)
          {
            batch_size = count;
            if (!this.suppressMessages)
              MonoBehaviour.print((object) ("WARNING: batch size greater than number of training sets. batch_size reduced to " + (object) count));
          }
          net.GenerateNativeParameterArrays();
          this.CD0000x1 = this.StartCoroutine(this.TrainNetwork_Par_Queue(net, batch_size, count, no_epochs, trainingRate, costFunction, trainingInputs, trainingOutputs, trainingSetWeighting, N_threads));
        }
      }
      else
      {
        if (this.suppressMessages)
          return;
        MonoBehaviour.print((object) "Error: Training initiated before previous job competed");
      }
    }
  }

  private void PROC0000x18(
    Noedify.Net net,
    float[,,] trainingInputs,
    ref List<float[]> JJR0000x18,
    ref List<float[]> ZO0000x18)
  {
    if (net.layers[0].layer_type == Noedify.LayerType.Input & trainingInputs.GetLength(2) != net.layers[0].layerSize & trainingInputs.GetLength(1) != 1 & trainingInputs.GetLength(0) != 1)
      MonoBehaviour.print((object) "WARNING: Input wrong size");
    else if (net.layers[0].layer_type == Noedify.LayerType.Input2D & trainingInputs.GetLength(2) != net.layers[0].layerSize2D[0] & trainingInputs.GetLength(1) != net.layers[0].layerSize2D[1] & trainingInputs.GetLength(0) != net.layers[0].channels)
      MonoBehaviour.print((object) "WARNING: Input wrong size");
    JJR0000x18.Add(Noedify_Utils.FlattenDataset(trainingInputs));
    ZO0000x18.Add(new float[net.layers[0].layerSize]);
    for (int index = 1; index < net.LayerCount(); ++index)
    {
      float[] inputs = JJR0000x18[index - 1];
      float[] JJR0000x18_1 = new float[net.layers[index].layerSize];
      float[] ZO0000x18_1 = new float[net.layers[index].layerSize];
      this.PROC0000x14(net, net.layers[index], inputs, ref JJR0000x18_1, ref ZO0000x18_1);
      JJR0000x18.Add(JJR0000x18_1);
      ZO0000x18.Add(ZO0000x18_1);
    }
  }

  private void PROC0000x14(
    Noedify.Net net,
    Noedify.Layer layer,
    float[] inputs,
    ref float[] JJR0000x18,
    ref float[] ZO0000x18)
  {
    if (layer.layer_type == Noedify.LayerType.Convolutional2D)
    {
      for (int index1 = 0; index1 < layer.layerSize; ++index1)
      {
        int num = 0;
        ZO0000x18[index1] = 0.0f;
        for (int index2 = 0; index2 < inputs.Length; ++index2)
        {
          if (layer.conv2DLayer.VG0000x61[index2, index1] >= 0)
          {
            ++num;
            ZO0000x18[index1] += inputs[index2] * layer.weights.valuesConv2D[layer.conv2DLayer.CPAR0000x7[index1], layer.conv2DLayer.VG0000x61[index2, index1]];
          }
          if (num >= layer.conv2DLayer.N_connections_per_node)
            break;
        }
        ZO0000x18[index1] += layer.biases.valuesConv2D[layer.conv2DLayer.CPAR0000x7[index1]];
      }
    }
    else if (layer.layer_type == Noedify.LayerType.Pool2D)
    {
      for (int index3 = 0; index3 < layer.layerSize; ++index3)
      {
        if (layer.pool_type == Noedify.PoolingType.Max)
          ZO0000x18[index3] = -100f;
        else if (layer.pool_type == Noedify.PoolingType.Avg)
          ZO0000x18[index3] = 0.0f;
        int num = 0;
        for (int index4 = 0; index4 < inputs.Length; ++index4)
        {
          if (layer.conv2DLayer.VG0000x61[index4, index3] >= 0)
          {
            ++num;
            if (layer.pool_type == Noedify.PoolingType.Avg)
              ZO0000x18[index3] += inputs[index4];
            else if (layer.pool_type == Noedify.PoolingType.Max && (double) inputs[index4] > (double) ZO0000x18[index3])
              ZO0000x18[index3] = inputs[index4];
          }
          if (num >= layer.conv2DLayer.N_connections_per_node)
            break;
        }
        if (layer.pool_type == Noedify.PoolingType.Avg)
          ZO0000x18[index3] /= (float) (layer.conv2DLayer.filterSize[0] * layer.conv2DLayer.filterSize[1]);
      }
    }
    else
    {
      for (int index5 = 0; index5 < layer.layerSize; ++index5)
      {
        ZO0000x18[index5] = 0.0f;
        for (int index6 = 0; index6 < inputs.Length; ++index6)
          ZO0000x18[index5] += inputs[index6] * layer.weights.values[index6, index5];
        ZO0000x18[index5] += layer.biases.values[index5];
      }
    }
    JJR0000x18 = Noedify_Solver.PROC0000x39(ZO0000x18, layer.activationFunction);
  }

  private Noedify.NN_LayerGradients[] PROC0000x19(
    Noedify.Net net,
    Noedify_Solver.CostFunction costFunction,
    float[] JUJ0000x81,
    List<float[]> DXI00000x14,
    List<float[]> EXI00000x77)
  {
    float[] numArray1 = Noedify_Solver.PROC0004x91(DXI00000x14[net.LayerCount() - 1], JUJ0000x81, costFunction);
    if (this.debug.print_outputError)
    {
      string message = "Cost Calculation: ";
      int index1 = net.LayerCount() - 1;
      for (int index2 = 0; index2 < net.layers[index1].layerSize; ++index2)
      {
        float num1 = DXI00000x14[index1][index2];
        float num2 = JUJ0000x81[index2];
        float num3 = numArray1[index2];
        message = message + "(" + (object) index2 + ")[" + (object) num1 + " - " + (object) num2 + " = " + (object) num3 + "] , ";
      }
      MonoBehaviour.print((object) message);
    }
    float[] numArray2 = new float[net.layers[net.LayerCount() - 1].layerSize];
    float[] numArray3 = Noedify_Solver.PROC0000x38(EXI00000x77[net.LayerCount() - 1], net.layers[net.LayerCount() - 1].activationFunction);
    for (int index = 0; index < net.layers[net.LayerCount() - 1].layerSize; ++index)
      numArray1[index] *= numArray3[index];
    if (this.debug.print_deltas)
    {
      string message = "Output layer deltas: ";
      for (int index = 0; index < net.layers[net.LayerCount() - 1].layerSize; ++index)
        message = message + "(" + (object) numArray3[index] + ")" + (object) numArray1[index] + ", ";
      MonoBehaviour.print((object) message);
    }
    Noedify.NN_LayerGradients[] nnLayerGradientsArray = new Noedify.NN_LayerGradients[net.LayerCount() - 1];
    nnLayerGradientsArray[net.LayerCount() - 2] = this.CalculateGradientsLayer(net, net.layers[net.LayerCount() - 1], DXI00000x14[net.LayerCount() - 2], numArray1);
    for (int index3 = net.LayerCount() - 2; index3 > 0; --index3)
    {
      numArray1 = this.BackpropagateLayer(net, net.layers[index3], net.layers[index3 + 1], numArray1, EXI00000x77[index3], DXI00000x14[index3], EXI00000x77[index3 + 1]);
      nnLayerGradientsArray[index3 - 1] = this.CalculateGradientsLayer(net, net.layers[index3], DXI00000x14[index3 - 1], numArray1);
      if (this.debug.print_deltas)
      {
        string message = "layer " + (object) index3 + " deltas: ";
        for (int index4 = 0; index4 < net.layers[index3].layerSize; ++index4)
          message = message + (object) numArray1[index4] + ", ";
        MonoBehaviour.print((object) message);
      }
      if (index3 == this.fineTuningLayerLimit)
        break;
    }
    return nnLayerGradientsArray;
  }

  private Noedify.NN_LayerGradients CalculateGradientsLayer(
    Noedify.Net net,
    Noedify.Layer layer,
    float[] KKZ0000x12s_prevLyr,
    float[] HY0000x74)
  {
    Noedify.NN_LayerGradients gradientsLayer = new Noedify.NN_LayerGradients(net.layers, layer.layer_no);
    if (layer.layer_type == Noedify.LayerType.Pool2D)
      return gradientsLayer;
    for (int index1 = 0; index1 < layer.layerSize; ++index1)
    {
      int num = 0;
      for (int index2 = 0; index2 < KKZ0000x12s_prevLyr.Length; ++index2)
      {
        if (layer.layer_type == Noedify.LayerType.Convolutional2D)
        {
          if (layer.conv2DLayer.VG0000x61[index2, index1] >= 0)
          {
            ++num;
            gradientsLayer.weight_gradients.valuesConv2D[layer.conv2DLayer.CPAR0000x7[index1], layer.conv2DLayer.VG0000x61[index2, index1]] += HY0000x74[index1] * KKZ0000x12s_prevLyr[index2];
          }
        }
        else
          gradientsLayer.weight_gradients.values[index2, index1] += HY0000x74[index1] * KKZ0000x12s_prevLyr[index2];
        if (layer.layer_type == Noedify.LayerType.Convolutional2D && num >= layer.conv2DLayer.N_connections_per_node)
          break;
      }
      if (layer.layer_type == Noedify.LayerType.Convolutional2D)
        gradientsLayer.bias_gradients.valuesConv2D[layer.conv2DLayer.CPAR0000x7[index1]] += HY0000x74[index1];
      else
        gradientsLayer.bias_gradients.values[index1] = HY0000x74[index1];
    }
    return gradientsLayer;
  }

  private float[] BackpropagateLayer(
    Noedify.Net net,
    Noedify.Layer ER0000x14,
    Noedify.Layer ER0000x15,
    float[] ER0000x16,
    float[] EXI00000x77,
    float[] DXI00000x14,
    float[] nextEXI00000x77)
  {
    float[] numArray1 = new float[ER0000x14.layerSize];
    for (int index1 = 0; index1 < ER0000x14.layerSize; ++index1)
    {
      numArray1[index1] = 0.0f;
      for (int index2 = 0; index2 < ER0000x16.Length; ++index2)
      {
        if (ER0000x15.layer_type == Noedify.LayerType.Convolutional2D)
        {
          if (ER0000x15.conv2DLayer.VG0000x61[index1, index2] >= 0)
            numArray1[index1] += ER0000x15.weights.valuesConv2D[ER0000x15.conv2DLayer.CPAR0000x7[index2], ER0000x15.conv2DLayer.VG0000x61[index1, index2]] * ER0000x16[index2];
        }
        else if (ER0000x15.layer_type == Noedify.LayerType.Pool2D)
        {
          if (ER0000x15.conv2DLayer.VG0000x61[index1, index2] >= 0)
          {
            if (ER0000x15.pool_type == Noedify.PoolingType.Avg)
              numArray1[index1] += ER0000x16[index2] / (float) ER0000x15.conv2DLayer.filterSize[0] / (float) ER0000x15.conv2DLayer.filterSize[1];
            else if (ER0000x15.pool_type == Noedify.PoolingType.Max && (double) DXI00000x14[index1] == (double) nextEXI00000x77[index2])
              numArray1[index1] += ER0000x16[index2];
          }
        }
        else
          numArray1[index1] += ER0000x15.weights.values[index1, index2] * ER0000x16[index2];
      }
    }
    if (ER0000x14.layer_type == Noedify.LayerType.Convolutional2D)
    {
      float[] numArray2 = Noedify_Solver.PROC0000x38(EXI00000x77, ER0000x14.activationFunction);
      for (int index = 0; index < ER0000x14.layerSize; ++index)
        numArray1[index] *= numArray2[index];
    }
    return numArray1;
  }

  private IEnumerator PROC0000x15(Noedify.Net net, float[,,] inputs)
  {
    if (!net.trainingInProgress)
    {
      this.CleanupTrainingNativeArrays();
      NativeArray<Noedify.ActivationFunction> activationFunctionList = new NativeArray<Noedify.ActivationFunction>(net.LayerCount(), Allocator.Persistent);
      NativeArray<Noedify.PoolingType> poolingTypeList = new NativeArray<Noedify.PoolingType>(net.LayerCount(), Allocator.Persistent);
      NativeArray<Noedify.LayerType> layerTypeList = new NativeArray<Noedify.LayerType>(net.LayerCount(), Allocator.Persistent);
      this.nativeArrayCleanupList.Add((object) activationFunctionList);
      this.nativeArrayCleanupList.Add((object) poolingTypeList);
      this.nativeArrayCleanupList.Add((object) layerTypeList);
      NativeArray<int> layerSizeList = this.NativeAllocInt(net.LayerCount());
      NativeArray<float> netInputs = this.NativeAllocFloat(net.layers[0].layerSize);
      NativeArray<float> netOutputs = this.NativeAllocFloat(net.layers[net.LayerCount() - 1].layerSize);
      NativeArray<float> JJR0000x18_par = this.NativeAllocFloat(net.total_no_nodes);
      NativeArray<float> ZO0000x18_par = this.NativeAllocFloat(net.total_no_nodes);
      NativeArray<int> CNN_nodes_per_filter_par = this.NativeAllocInt(net.LayerCount() - 1);
      NativeArray<int> CNN_weights_per_filter_par = this.NativeAllocInt(net.LayerCount() - 1);
      NativeArray<int> CNN_no_filters_par = this.NativeAllocInt(net.LayerCount() - 1);
      NativeArray<int> CNN_no_conns_per_node = this.NativeAllocInt(net.LayerCount() - 1);
      Noedify_Solver.PROC0001x71(net, ref activationFunctionList, ref layerTypeList, ref layerSizeList, ref CNN_nodes_per_filter_par, ref CNN_weights_per_filter_par, ref CNN_no_filters_par, ref CNN_no_conns_per_node, ref poolingTypeList);
      try
      {
        for (int i = 0; i < net.layers[0].layerSize; ++i)
          netInputs[i] = Noedify_Utils.FlattenDataset(inputs)[i];
      }
      catch
      {
        MonoBehaviour.print((object) ("ERROR (Noedify_Solver.Evaluate_Par): Node input size (" + (object) inputs.GetLength(2) + ") incompatable with input layer size (" + (object) net.layers[0].layerSize + ")"));
        goto label_12;
      }
      Noedify_Solver.EvaluateNetwork_Job evalNetwork_Job = new Noedify_Solver.EvaluateNetwork_Job();
      evalNetwork_Job.activationFunction = activationFunctionList;
      evalNetwork_Job.KJU0000x441 = layerTypeList;
      evalNetwork_Job.layerPooling = poolingTypeList;
      evalNetwork_Job.layerSize = layerSizeList;
      evalNetwork_Job.KJU0000x313 = net.PARA0000x1;
      evalNetwork_Job.KJU0000x21 = net.PARA0000x2;
      evalNetwork_Job.KJU0000x241 = net.PARA0000x5;
      evalNetwork_Job.PARA0000x6 = net.PARA0000x6;
      evalNetwork_Job.KJU0000x71 = net.PARA0000x9;
      evalNetwork_Job.KJU0000x23 = net.nodeIndeces_start;
      evalNetwork_Job.ZO0000x18 = ZO0000x18_par;
      evalNetwork_Job.JJR0000x18 = JJR0000x18_par;
      evalNetwork_Job.VG0000x61 = net.VG0000x61_par;
      evalNetwork_Job.VG0000x61Start_index = net.VG0000x61Indeces_start;
      evalNetwork_Job.CNN_nodes_per_filter = CNN_nodes_per_filter_par;
      evalNetwork_Job.JUO0000x91 = CNN_weights_per_filter_par;
      evalNetwork_Job.JUO0000x90 = CNN_no_filters_par;
      evalNetwork_Job.JUO0000x92 = CNN_no_conns_per_node;
      evalNetwork_Job.networkInputs = netInputs;
      evalNetwork_Job.networkOutputs = netOutputs;
      evalNetwork_Job.total_no_active_nodes = net.total_no_activeNodes;
      evalNetwork_Job.network_input_size = net.layers[0].layerSize;
      JobHandle eval_handle = evalNetwork_Job.Schedule<Noedify_Solver.EvaluateNetwork_Job>(1, 1);
      this.activeJob = eval_handle;
      net.trainingInProgress = true;
      int timeoutCounter = 0;
      do
      {
        yield return (object) null;
        ++timeoutCounter;
        if (eval_handle.IsCompleted)
          goto label_11;
      }
      while (timeoutCounter <= 100000);
      if (!this.suppressMessages)
      {
        MonoBehaviour.print((object) "Error: Evaluation timeout reached!");
        goto label_12;
      }
      else
        goto label_12;
label_11:
      eval_handle.Complete();
label_12:
      this.prediction = netOutputs.ToArray();
      this.CleanupTrainingNativeArrays();
      net.trainingInProgress = false;
      this.evaluationInProgress = false;
      activationFunctionList = new NativeArray<Noedify.ActivationFunction>();
      poolingTypeList = new NativeArray<Noedify.PoolingType>();
      layerTypeList = new NativeArray<Noedify.LayerType>();
      layerSizeList = new NativeArray<int>();
      netInputs = new NativeArray<float>();
      netOutputs = new NativeArray<float>();
      JJR0000x18_par = new NativeArray<float>();
      ZO0000x18_par = new NativeArray<float>();
      CNN_nodes_per_filter_par = new NativeArray<int>();
      CNN_weights_per_filter_par = new NativeArray<int>();
      CNN_no_filters_par = new NativeArray<int>();
      CNN_no_conns_per_node = new NativeArray<int>();
      evalNetwork_Job = new Noedify_Solver.EvaluateNetwork_Job();
      eval_handle = new JobHandle();
    }
    else
    {
      if (!this.suppressMessages)
        MonoBehaviour.print((object) "Error: Evaluation initiated before previous job competed");
      this.evaluationInProgress = false;
      net.Cleanup_Par();
    }
    net.trainingInProgress = false;
    yield return (object) null;
  }

  private IEnumerator TrainNetwork_Par_Queue(
    Noedify.Net net,
    int batch_size,
    int no_training_sets,
    int no_epochs,
    float trainingRate,
    Noedify_Solver.CostFunction costFunction,
    List<float[,,]> trainingInputs,
    List<float[]> trainingOutputs,
    List<float> trainingSetWeighting,
    int N_threads)
  {
    if (!net.trainingInProgress)
    {
      Stopwatch sw = new Stopwatch();
      sw.Start();
      if (batch_size > no_training_sets)
      {
        if (!this.suppressMessages)
          MonoBehaviour.print((object) ("Warning: batch size (" + (object) batch_size + ") greater than the # of training sets (" + (object) no_training_sets + "). Reducing batch size to " + (object) no_training_sets));
        batch_size = no_training_sets;
      }
      this.CleanupTrainingNativeArrays();
      NativeArray<Noedify.ActivationFunction> activationFunctionList = new NativeArray<Noedify.ActivationFunction>(net.LayerCount(), Allocator.Persistent);
      NativeArray<Noedify.PoolingType> poolingTypeList = new NativeArray<Noedify.PoolingType>(net.LayerCount(), Allocator.Persistent);
      NativeArray<Noedify.LayerType> layerTypeList = new NativeArray<Noedify.LayerType>(net.LayerCount(), Allocator.Persistent);
      this.nativeArrayCleanupList.Add((object) activationFunctionList);
      this.nativeArrayCleanupList.Add((object) poolingTypeList);
      this.nativeArrayCleanupList.Add((object) layerTypeList);
      NativeArray<int> layerSizeList = this.NativeAllocInt(net.LayerCount());
      NativeArray<float> trainingInputs_batchSet = this.NativeAllocFloat(batch_size * net.layers[0].layerSize);
      NativeArray<float> trainingOutputs_batchSet = this.NativeAllocFloat(batch_size * net.layers[net.LayerCount() - 1].layerSize);
      NativeArray<float> batch_cost = this.NativeAllocFloat(batch_size);
      NativeArray<int> CNN_nodes_per_filter_par = this.NativeAllocInt(net.LayerCount() - 1);
      NativeArray<int> CNN_weights_per_filter_par = this.NativeAllocInt(net.LayerCount() - 1);
      NativeArray<int> CNN_no_filters_par = this.NativeAllocInt(net.LayerCount() - 1);
      net.PARA0000x3 = this.NativeAllocFloat(net.total_no_weights * batch_size);
      net.PARA0000x4 = this.NativeAllocFloat(net.total_no_biases * batch_size);
      NativeArray<float> JJR0000x18_par = this.NativeAllocFloat(batch_size * net.total_no_nodes);
      NativeArray<float> ZO0000x18_par = this.NativeAllocFloat(batch_size * net.total_no_nodes);
      NativeArray<float> deltas_par = this.NativeAllocFloat(batch_size * net.total_no_activeNodes);
      NativeArray<int> CNN_no_conns_per_node = this.NativeAllocInt(net.LayerCount() - 1);
      Noedify_Solver.PROC0001x71(net, ref activationFunctionList, ref layerTypeList, ref layerSizeList, ref CNN_nodes_per_filter_par, ref CNN_weights_per_filter_par, ref CNN_no_filters_par, ref CNN_no_conns_per_node, ref poolingTypeList);
      List<float[]> trainingInputs_flat = Noedify_Utils.FlattenDataset(trainingInputs);
      for (int epoch = 0; epoch < no_epochs; ++epoch)
      {
        int[] TMR0000x91 = new int[no_training_sets];
        for (int i = 0; i < no_training_sets; ++i)
          TMR0000x91[i] = i;
        TMR0000x91 = Noedify_Utils.Shuffle(TMR0000x91);
        for (int batch = 0; batch < batch_size; ++batch)
        {
          for (int i = 0; i < net.layers[0].layerSize; ++i)
            trainingInputs_batchSet[batch * net.layers[0].layerSize + i] = trainingInputs_flat[TMR0000x91[batch]][i];
          for (int i = 0; i < net.layers[net.LayerCount() - 1].layerSize; ++i)
            trainingOutputs_batchSet[batch * net.layers[net.LayerCount() - 1].layerSize + i] = trainingOutputs[TMR0000x91[batch]][i];
        }
        for (int batch = 0; batch < batch_size; ++batch)
          batch_cost[batch] = 0.0f;
        for (int i = 0; i < net.total_no_weights * batch_size; ++i)
          net.PARA0000x3[i] = 0.0f;
        for (int i = 0; i < net.total_no_biases * batch_size; ++i)
          net.PARA0000x4[i] = 0.0f;
        Noedify_Solver.TrainNetwork_Job trainNetwork_Job = new Noedify_Solver.TrainNetwork_Job();
        trainNetwork_Job.activationFunction = activationFunctionList;
        trainNetwork_Job.costFunction = costFunction;
        trainNetwork_Job.layerTypes = layerTypeList;
        trainNetwork_Job.layerPooling = poolingTypeList;
        trainNetwork_Job.layerSize = layerSizeList;
        trainNetwork_Job.networkWeights = net.PARA0000x1;
        trainNetwork_Job.networkBiases = net.PARA0000x2;
        trainNetwork_Job.PARA0000x5 = net.PARA0000x5;
        trainNetwork_Job.PARA0000x6 = net.PARA0000x6;
        trainNetwork_Job.PARA0000x9 = net.PARA0000x9;
        trainNetwork_Job.nodeIndeces_start = net.nodeIndeces_start;
        trainNetwork_Job.ZO0000x18 = ZO0000x18_par;
        trainNetwork_Job.JJR0000x18 = JJR0000x18_par;
        trainNetwork_Job.deltas = deltas_par;
        trainNetwork_Job.networkWeightGradients = net.PARA0000x3;
        trainNetwork_Job.networkBiasGradients = net.PARA0000x4;
        trainNetwork_Job.VG0000x61 = net.VG0000x61_par;
        trainNetwork_Job.VG0000x61Start_index = net.VG0000x61Indeces_start;
        trainNetwork_Job.CNN_nodes_per_filter = CNN_nodes_per_filter_par;
        trainNetwork_Job.CNN_weights_per_filter = CNN_weights_per_filter_par;
        trainNetwork_Job.CNN_no_filters = CNN_no_filters_par;
        trainNetwork_Job.CNN_conns_per_node = CNN_no_conns_per_node;
        trainNetwork_Job.trainingInputs = trainingInputs_batchSet;
        trainNetwork_Job.trainingOutputs = trainingOutputs_batchSet;
        trainNetwork_Job.total_no_active_nodes = net.total_no_activeNodes;
        trainNetwork_Job.total_no_nodes = net.total_no_nodes;
        trainNetwork_Job.network_input_size = net.layers[0].layerSize;
        trainNetwork_Job.batch_cost = batch_cost;
        trainNetwork_Job.debug_job = this.debug.ConvertForPar();
        trainNetwork_Job.fineTuningLayerLimit_job = this.fineTuningLayerLimit;
        int batched_per_thread = Mathf.CeilToInt((float) (batch_size / N_threads));
        JobHandle training_handle = trainNetwork_Job.Schedule<Noedify_Solver.TrainNetwork_Job>(batch_size, batched_per_thread);
        this.activeJob = training_handle;
        net.trainingInProgress = true;
        int timeoutCounter = 0;
        do
        {
          yield return (object) null;
          ++timeoutCounter;
          if (training_handle.IsCompleted)
            goto label_32;
        }
        while (timeoutCounter <= 100000);
        if (!this.suppressMessages)
        {
          MonoBehaviour.print((object) "Error: Training timeout reached!");
          break;
        }
        break;
label_32:
        training_handle.Complete();
        float epoch_cost = 0.0f;
        for (int batch = 0; batch < batch_size; ++batch)
          epoch_cost += batch_cost[batch];
        epoch_cost /= (float) batch_size;
        if (epoch % 10 == 0 | epoch == no_epochs - 1 && !this.suppressMessages)
          MonoBehaviour.print((object) ("epoch " + (object) (epoch + 1) + "/" + (object) no_epochs + ", cost: " + (object) epoch_cost));
        this.cost_report[epoch] = epoch_cost;
        float[] weighting = new float[batch_size];
        for (int batch = 0; batch < batch_size; ++batch)
          weighting[batch] = trainingSetWeighting[TMR0000x91[batch]];
        Noedify.NN_LayerGradients[] KLA0000x71 = this.PROC0000x20(net, trainingRate, net.PARA0000x3, net.PARA0000x4, batch_size, weighting);
        if (this.debug.print_bias_Gradients && epoch % 10 == 0 | epoch == no_epochs - 1)
        {
          for (int l = 1; l < net.LayerCount(); ++l)
          {
            string IH0000x41 = "Epoch " + (object) epoch + " bias gradients layer " + (object) l + ": ";
            if (layerTypeList[l] == Noedify.LayerType.Convolutional2D)
            {
              for (int f = 0; f < CNN_no_filters_par[l - 1]; ++f)
                IH0000x41 = IH0000x41 + "(b" + (object) f + "=" + (object) net.PARA0000x2[net.PARA0000x6[l - 1] + f] + ")" + (object) KLA0000x71[l - 1].bias_gradients.valuesConv2D[f] + " ";
            }
            else if (net.layers[l].layer_type != Noedify.LayerType.Pool2D)
            {
              for (int j = 0; j < net.layers[l].layerSize; ++j)
                IH0000x41 = IH0000x41 + "(b" + (object) j + "=" + (object) net.PARA0000x2[net.PARA0000x6[l - 1] + j] + ")" + (object) KLA0000x71[l - 1].bias_gradients.values[j] + " ";
            }
            MonoBehaviour.print((object) IH0000x41);
            IH0000x41 = (string) null;
          }
        }
        if (this.debug.print_weight_Gradients && epoch % 10 == 0 | epoch == no_epochs - 1)
        {
          for (int l = 1; l < net.LayerCount(); ++l)
          {
            if (net.layers[l].layer_type == Noedify.LayerType.Convolutional2D)
            {
              for (int f = 0; f < net.layers[l].conv2DLayer.no_filters; ++f)
              {
                string IH0000x14 = "Epoch " + (object) epoch + " layer " + (object) l + " from filter " + (object) f + ": ";
                for (int j = 0; j < net.layers[l].conv2DLayer.N_weights_per_filter; ++j)
                  IH0000x14 = IH0000x14 + "(w_" + (object) f + "_" + (object) j + "=" + (object) net.PARA0000x1[net.PARA0000x5[l - 1] + f * net.layers[l].conv2DLayer.N_weights_per_filter + j] + ")" + (object) KLA0000x71[l - 1].weight_gradients.valuesConv2D[f, j] + " ";
                MonoBehaviour.print((object) IH0000x14);
                IH0000x14 = (string) null;
              }
            }
            else if (net.layers[l].layer_type != Noedify.LayerType.Pool2D)
            {
              for (int i = 0; i < net.layers[l - 1].layerSize && i <= 10; ++i)
              {
                string IH0000x14 = "Epoch " + (object) epoch + " layer " + (object) l + " from node " + (object) i + ": ";
                for (int j = 0; j < net.layers[l].layerSize; ++j)
                  IH0000x14 = IH0000x14 + "(w_" + (object) i + "_" + (object) j + "=" + (object) net.PARA0000x1[net.PARA0000x5[l - 1] + i * net.layers[l].layerSize + j] + ")" + (object) KLA0000x71[l - 1].weight_gradients.values[i, j] + " ";
                MonoBehaviour.print((object) IH0000x14);
                IH0000x14 = (string) null;
              }
            }
          }
        }
        net.ApplyGradients(KLA0000x71, batch_size, true, this.fineTuningLayerLimit);
        if ((double) epoch_cost < (double) this.costThreshold)
        {
          if (!this.suppressMessages)
          {
            MonoBehaviour.print((object) ("Cost Threshold reached (" + (object) this.costThreshold + "). Training Complete."));
            break;
          }
          break;
        }
        TMR0000x91 = (int[]) null;
        trainNetwork_Job = new Noedify_Solver.TrainNetwork_Job();
        training_handle = new JobHandle();
        weighting = (float[]) null;
        KLA0000x71 = (Noedify.NN_LayerGradients[]) null;
      }
      net.OffloadNativeParameterArrays();
      this.CleanupTrainingNativeArrays();
      net.Cleanup_Par();
      net.trainingInProgress = false;
      this.trainingInProgress = false;
      sw = (Stopwatch) null;
      activationFunctionList = new NativeArray<Noedify.ActivationFunction>();
      poolingTypeList = new NativeArray<Noedify.PoolingType>();
      layerTypeList = new NativeArray<Noedify.LayerType>();
      layerSizeList = new NativeArray<int>();
      trainingInputs_batchSet = new NativeArray<float>();
      trainingOutputs_batchSet = new NativeArray<float>();
      batch_cost = new NativeArray<float>();
      CNN_nodes_per_filter_par = new NativeArray<int>();
      CNN_weights_per_filter_par = new NativeArray<int>();
      CNN_no_filters_par = new NativeArray<int>();
      JJR0000x18_par = new NativeArray<float>();
      ZO0000x18_par = new NativeArray<float>();
      deltas_par = new NativeArray<float>();
      CNN_no_conns_per_node = new NativeArray<int>();
      trainingInputs_flat = (List<float[]>) null;
    }
    else
    {
      if (!this.suppressMessages)
        MonoBehaviour.print((object) "Error: Training initiated before previous job competed");
      this.trainingInProgress = false;
      net.Cleanup_Par();
    }
    yield return (object) null;
  }

  public void PROC001x32()
  {
    if (this.CD0000x1 != null)
      this.StopCoroutine(this.CD0000x1);
    this.activeJob.Complete();
  }

  private Noedify.NN_LayerGradients[] PROC0000x20(
    Noedify.Net net,
    float OU0000x1,
    List<Noedify.NN_LayerGradients[]> OU0000x2,
    int OU0001x3,
    float[] OU0000x3)
  {
    Noedify.NN_LayerGradients[] nnLayerGradientsArray = new Noedify.NN_LayerGradients[net.LayerCount() - 1];
    for (int index1 = 1; index1 < net.LayerCount(); ++index1)
    {
      if (index1 >= this.fineTuningLayerLimit)
      {
        Noedify.NN_LayerGradients nnLayerGradients = new Noedify.NN_LayerGradients(net.layers, index1);
        if (net.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
        {
          for (int index2 = 0; index2 < net.layers[index1].conv2DLayer.no_filters; ++index2)
          {
            for (int index3 = 0; index3 < net.layers[index1].conv2DLayer.N_weights_per_filter; ++index3)
            {
              for (int index4 = 0; index4 < OU0001x3; ++index4)
                nnLayerGradients.weight_gradients.valuesConv2D[index2, index3] += OU0000x2[index4][index1 - 1].weight_gradients.valuesConv2D[index2, index3] * OU0000x1 * OU0000x3[index4] / (float) OU0001x3;
            }
            for (int index5 = 0; index5 < OU0001x3; ++index5)
              nnLayerGradients.bias_gradients.valuesConv2D[index2] += OU0000x2[index5][index1 - 1].bias_gradients.valuesConv2D[index2] * OU0000x1 * OU0000x3[index5] / (float) OU0001x3;
          }
        }
        else if (net.layers[index1].layer_type != Noedify.LayerType.Pool2D)
        {
          for (int index6 = 0; index6 < net.layers[index1].layerSize; ++index6)
          {
            for (int index7 = 0; index7 < net.layers[index1 - 1].layerSize; ++index7)
            {
              for (int index8 = 0; index8 < OU0001x3; ++index8)
                nnLayerGradients.weight_gradients.values[index7, index6] += OU0000x2[index8][index1 - 1].weight_gradients.values[index7, index6] * OU0000x1 * OU0000x3[index8] / (float) OU0001x3;
            }
            for (int index9 = 0; index9 < OU0001x3; ++index9)
              nnLayerGradients.bias_gradients.values[index6] += OU0000x2[index9][index1 - 1].bias_gradients.values[index6] * OU0000x1 * OU0000x3[index9] / (float) OU0001x3;
          }
        }
        nnLayerGradientsArray[index1 - 1] = nnLayerGradients;
      }
    }
    return nnLayerGradientsArray;
  }

  private Noedify.NN_LayerGradients[] PROC0000x20(
    Noedify.Net net,
    float HU0000x2,
    NativeArray<float> HU0000x3,
    NativeArray<float> HU0000x6,
    int batch_size,
    float[] weighting)
  {
    Noedify.NN_LayerGradients[] nnLayerGradientsArray = new Noedify.NN_LayerGradients[net.LayerCount() - 1];
    for (int index1 = 1; index1 < net.LayerCount(); ++index1)
    {
      if (index1 >= this.fineTuningLayerLimit)
      {
        nnLayerGradientsArray[index1 - 1] = new Noedify.NN_LayerGradients(net.layers, index1);
        for (int index2 = 0; index2 < batch_size; ++index2)
        {
          if (net.layers[index1].layer_type == Noedify.LayerType.Convolutional2D)
          {
            for (int index3 = 0; index3 < net.layers[index1].conv2DLayer.no_filters; ++index3)
            {
              for (int index4 = 0; index4 < net.layers[index1].conv2DLayer.N_weights_per_filter; ++index4)
                nnLayerGradientsArray[index1 - 1].weight_gradients.valuesConv2D[index3, index4] += HU0000x3[index2 * net.total_no_weights + net.PARA0000x5[index1 - 1] + index3 * net.layers[index1].conv2DLayer.N_weights_per_filter + index4] * HU0000x2 * weighting[index2] / (float) batch_size;
              nnLayerGradientsArray[index1 - 1].bias_gradients.valuesConv2D[index3] += HU0000x6[index2 * net.total_no_biases + net.PARA0000x6[index1 - 1] + index3] * HU0000x2 * weighting[index2] / (float) batch_size;
            }
          }
          else if (net.layers[index1].layer_type != Noedify.LayerType.Pool2D)
          {
            for (int index5 = 0; index5 < net.layers[index1 - 1].layerSize; ++index5)
            {
              for (int index6 = 0; index6 < net.layers[index1].layerSize; ++index6)
                nnLayerGradientsArray[index1 - 1].weight_gradients.values[index5, index6] += HU0000x3[index2 * net.total_no_weights + net.PARA0000x5[index1 - 1] + index5 * net.layers[index1].layerSize + index6] * HU0000x2 * weighting[index2] / (float) batch_size;
            }
            for (int index7 = 0; index7 < net.layers[index1].layerSize; ++index7)
              nnLayerGradientsArray[index1 - 1].bias_gradients.values[index7] += HU0000x6[index2 * net.total_no_biases + net.PARA0000x6[index1 - 1] + index7] * HU0000x2 * weighting[index2] / (float) batch_size;
          }
        }
      }
    }
    return nnLayerGradientsArray;
  }

  private static void PROC0000x14_Job(
    int layer_no,
    Noedify.LayerType layerType,
    Noedify.ActivationFunction activationFunction,
    Noedify.PoolingType poolingType,
    NativeArray<int> TY0000x12,
    ref NativeArray<float> ZO0000x18,
    ref NativeArray<float> JJR0000x18,
    NativeArray<float> TY0000x31,
    NativeArray<float> TY0000x33,
    NativeArray<int> VG0000x61,
    NativeArray<int> TY0000x51,
    NativeArray<int> TY0000x52,
    NativeArray<int> PARA0000x6,
    NativeArray<int> VG0000x61Start_index,
    NativeArray<int> TY0000x53,
    NativeArray<int> TY0000x54,
    NativeArray<int> TY0000x57,
    int batch_nodes = 0)
  {
    switch (layerType)
    {
      case Noedify.LayerType.Convolutional2D:
        for (int index1 = 0; index1 < TY0000x12[layer_no]; ++index1)
        {
          int num1 = 0;
          ZO0000x18[batch_nodes + TY0000x51[layer_no] + index1] = 0.0f;
          int num2 = Mathf.FloorToInt((float) (index1 / TY0000x53[layer_no - 1]));
          for (int index2 = 0; index2 < TY0000x12[layer_no - 1]; ++index2)
          {
            int num3 = VG0000x61[VG0000x61Start_index[layer_no - 1] + index2 * TY0000x12[layer_no] + index1];
            if (num3 >= 0)
            {
              ++num1;
              ZO0000x18[batch_nodes + TY0000x51[layer_no] + index1] += JJR0000x18[batch_nodes + TY0000x51[layer_no - 1] + index2] * TY0000x31[TY0000x52[layer_no - 1] + num2 * TY0000x54[layer_no - 1] + num3];
            }
            if (num1 >= TY0000x57[layer_no - 1])
              break;
          }
          ZO0000x18[batch_nodes + TY0000x51[layer_no] + index1] += TY0000x33[PARA0000x6[layer_no - 1] + num2];
        }
        break;
      case Noedify.LayerType.Pool2D:
        for (int index3 = 0; index3 < TY0000x12[layer_no]; ++index3)
        {
          int num = 0;
          switch (poolingType)
          {
            case Noedify.PoolingType.Max:
              ZO0000x18[batch_nodes + TY0000x51[layer_no] + index3] = -100f;
              break;
            case Noedify.PoolingType.Avg:
              ZO0000x18[batch_nodes + TY0000x51[layer_no] + index3] = 0.0f;
              break;
          }
          for (int index4 = 0; index4 < TY0000x12[layer_no - 1]; ++index4)
          {
            if (VG0000x61[VG0000x61Start_index[layer_no - 1] + index4 * TY0000x12[layer_no] + index3] >= 0)
            {
              ++num;
              if (poolingType == Noedify.PoolingType.Avg)
                ZO0000x18[batch_nodes + TY0000x51[layer_no] + index3] += JJR0000x18[batch_nodes + TY0000x51[layer_no - 1] + index4];
              else if (poolingType == Noedify.PoolingType.Max && (double) JJR0000x18[batch_nodes + TY0000x51[layer_no - 1] + index4] > (double) ZO0000x18[batch_nodes + TY0000x51[layer_no] + index3])
                ZO0000x18[batch_nodes + TY0000x51[layer_no] + index3] = JJR0000x18[batch_nodes + TY0000x51[layer_no - 1] + index4];
            }
            if (num >= TY0000x57[layer_no - 1])
              break;
          }
          if (poolingType == Noedify.PoolingType.Avg)
            ZO0000x18[batch_nodes + TY0000x51[layer_no] + index3] /= (float) TY0000x54[layer_no - 1];
        }
        break;
      default:
        for (int index5 = 0; index5 < TY0000x12[layer_no]; ++index5)
        {
          ZO0000x18[batch_nodes + TY0000x51[layer_no] + index5] = 0.0f;
          for (int index6 = 0; index6 < TY0000x12[layer_no - 1]; ++index6)
            ZO0000x18[batch_nodes + TY0000x51[layer_no] + index5] += JJR0000x18[batch_nodes + TY0000x51[layer_no - 1] + index6] * TY0000x31[TY0000x52[layer_no - 1] + index6 * TY0000x12[layer_no] + index5];
          ZO0000x18[batch_nodes + TY0000x51[layer_no] + index5] += TY0000x33[PARA0000x6[layer_no - 1] + index5];
        }
        break;
    }
    Noedify_Solver.PROC0000x39(ref JJR0000x18, ZO0000x18, batch_nodes + TY0000x51[layer_no], TY0000x12[layer_no], activationFunction);
  }

  private static void BackPropagateLayer_Job(
    int JK0000x12,
    int JK0000x17,
    int outputLayerNo,
    int batch,
    int JK0000x21,
    int total_no_nodes,
    Noedify.ActivationFunction activationFunction,
    Noedify.PoolingType JK0000x26,
    NativeArray<Noedify.LayerType> layerTypes,
    NativeArray<int> layerSize,
    NativeArray<int> VG0000x61,
    NativeArray<int> PARA0000x9,
    NativeArray<int> nodeIndeces_start,
    NativeArray<float> ZO0000x18,
    NativeArray<float> JJR0000x18,
    NativeArray<float> KI0000x31,
    NativeArray<int> KI0020x32,
    NativeArray<int> VG0000x61Start_index,
    NativeArray<int> CNN_nodes_per_filter,
    NativeArray<int> CNN_weights_per_filter,
    NativeArray<int> CNN_conns_per_node,
    ref NativeArray<float> deltas)
  {
    for (int index1 = 0; index1 < layerSize[JK0000x12]; ++index1)
    {
      deltas[batch * JK0000x21 + PARA0000x9[JK0000x12 - 1] + index1] = 0.0f;
      for (int index2 = 0; index2 < layerSize[JK0000x17]; ++index2)
      {
        if (layerTypes[JK0000x17] == Noedify.LayerType.Convolutional2D)
        {
          int num1 = VG0000x61[VG0000x61Start_index[JK0000x17 - 1] + index1 * layerSize[JK0000x17] + index2];
          if (num1 >= 0)
          {
            int num2 = Mathf.FloorToInt((float) (index2 / CNN_nodes_per_filter[JK0000x17 - 1]));
            deltas[batch * JK0000x21 + PARA0000x9[JK0000x12 - 1] + index1] += KI0000x31[KI0020x32[JK0000x17 - 1] + num2 * CNN_weights_per_filter[JK0000x17 - 1] + num1] * deltas[batch * JK0000x21 + PARA0000x9[JK0000x17 - 1] + index2];
          }
        }
        else if (layerTypes[JK0000x17] == Noedify.LayerType.Pool2D)
        {
          if (VG0000x61[VG0000x61Start_index[JK0000x17 - 1] + index1 * layerSize[JK0000x17] + index2] >= 0)
          {
            if (JK0000x26 == Noedify.PoolingType.Avg)
              deltas[batch * JK0000x21 + PARA0000x9[JK0000x12 - 1] + index1] += deltas[batch * JK0000x21 + PARA0000x9[JK0000x17 - 1] + index2] / (float) CNN_weights_per_filter[JK0000x17 - 1];
            else if (JK0000x26 == Noedify.PoolingType.Max && (double) JJR0000x18[batch * total_no_nodes + nodeIndeces_start[JK0000x12] + index1] == (double) ZO0000x18[batch * total_no_nodes + nodeIndeces_start[JK0000x17] + index2])
              deltas[batch * JK0000x21 + PARA0000x9[JK0000x12 - 1] + index1] += deltas[batch * JK0000x21 + PARA0000x9[JK0000x17 - 1] + index2];
          }
        }
        else
          deltas[batch * JK0000x21 + PARA0000x9[JK0000x12 - 1] + index1] += KI0000x31[KI0020x32[JK0000x17 - 1] + index1 * layerSize[JK0000x17] + index2] * deltas[batch * JK0000x21 + PARA0000x9[JK0000x17 - 1] + index2];
      }
    }
    if (layerTypes[JK0000x12] != Noedify.LayerType.Convolutional2D)
      return;
    NativeArray<float> IXA0000x241 = new NativeArray<float>(layerSize[JK0000x12], Allocator.Temp);
    Noedify_Solver.PROC0000x38(ref IXA0000x241, 0, ZO0000x18, batch * total_no_nodes + nodeIndeces_start[JK0000x12], layerSize[JK0000x12], activationFunction);
    for (int index = 0; index < layerSize[JK0000x12]; ++index)
      deltas[batch * JK0000x21 + PARA0000x9[JK0000x12 - 1] + index] *= IXA0000x241[index];
  }

  private static void CalculateGradientsLayer_Job(
    int UY0000x1,
    int UY0000x2,
    int UY0000x3,
    int UY0000x4,
    int UY0000x5,
    int UY0000x6,
    Noedify.LayerType layerType,
    NativeArray<int> layerSize,
    NativeArray<float> JJR0000x18,
    ref NativeArray<float> UY0000x7,
    ref NativeArray<float> CC0000x12,
    NativeArray<int> VG0000x61,
    NativeArray<int> CC0000x13,
    NativeArray<int> CC0000x16,
    NativeArray<int> CC0000x19,
    NativeArray<int> CC0000x20,
    NativeArray<int> VG0000x61Start_index,
    NativeArray<int> CNN_nodes_per_filter,
    NativeArray<int> CC0000x25,
    NativeArray<int> CNN_conns_per_node,
    NativeArray<float> CC0000x27)
  {
    int index1 = UY0000x1 - 1;
    for (int index2 = 0; index2 < layerSize[UY0000x1]; ++index2)
    {
      int num1 = 0;
      int num2 = 0;
      if (layerType == Noedify.LayerType.Convolutional2D)
        num2 = Mathf.FloorToInt((float) (index2 / CNN_nodes_per_filter[UY0000x1 - 1]));
      for (int index3 = 0; index3 < layerSize[index1]; ++index3)
      {
        switch (layerType)
        {
          case Noedify.LayerType.Convolutional2D:
            int num3 = VG0000x61[VG0000x61Start_index[UY0000x1 - 1] + index3 * layerSize[UY0000x1] + index2];
            if (num3 >= 0)
            {
              ++num1;
              UY0000x7[UY0000x2 * UY0000x5 + CC0000x19[UY0000x1 - 1] + num2 * CC0000x25[UY0000x1 - 1] + num3] += CC0000x27[UY0000x2 * UY0000x3 + CC0000x13[UY0000x1 - 1] + index2] * JJR0000x18[UY0000x2 * UY0000x4 + CC0000x16[index1] + index3];
              break;
            }
            break;
          case Noedify.LayerType.Pool2D:
            goto label_12;
          default:
            UY0000x7[UY0000x2 * UY0000x5 + CC0000x19[UY0000x1 - 1] + index3 * layerSize[UY0000x1] + index2] = CC0000x27[UY0000x2 * UY0000x3 + CC0000x13[UY0000x1 - 1] + index2] * JJR0000x18[UY0000x2 * UY0000x4 + CC0000x16[index1] + index3];
            break;
        }
        if (layerType == Noedify.LayerType.Convolutional2D && num1 >= CNN_nodes_per_filter[UY0000x1 - 1])
          break;
      }
label_12:
      if (layerType == Noedify.LayerType.Convolutional2D)
      {
        CC0000x12[UY0000x2 * UY0000x6 + CC0000x20[UY0000x1 - 1] + num2] += CC0000x27[UY0000x2 * UY0000x3 + CC0000x13[UY0000x1 - 1] + index2];
      }
      else
      {
        if (layerType == Noedify.LayerType.Pool2D)
          break;
        CC0000x12[UY0000x2 * UY0000x6 + CC0000x20[UY0000x1 - 1] + index2] = CC0000x27[UY0000x2 * UY0000x3 + CC0000x13[UY0000x1 - 1] + index2];
      }
    }
  }

  private static void PROC0001x71(
    Noedify.Net net,
    ref NativeArray<Noedify.ActivationFunction> activationFunctionList,
    ref NativeArray<Noedify.LayerType> IAY0000x1,
    ref NativeArray<int> IAY0000x2,
    ref NativeArray<int> IAY0000x3,
    ref NativeArray<int> IAY0000x4,
    ref NativeArray<int> IAY0000x5,
    ref NativeArray<int> IAY0000x6,
    ref NativeArray<Noedify.PoolingType> IAY0000x7)
  {
    for (int index = 1; index < net.LayerCount(); ++index)
    {
      if (net.layers[index].layer_type == Noedify.LayerType.Convolutional2D)
      {
        IAY0000x3[index - 1] = net.layers[index].layerSize2D[0] * net.layers[index].layerSize2D[1];
        IAY0000x4[index - 1] = net.layers[index].conv2DLayer.N_weights_per_filter;
        IAY0000x5[index - 1] = net.layers[index].conv2DLayer.no_filters;
        IAY0000x6[index - 1] = net.layers[index].conv2DLayer.N_connections_per_node;
      }
      else if (net.layers[index].layer_type == Noedify.LayerType.Pool2D)
      {
        IAY0000x7[index] = net.layers[index].pool_type;
        IAY0000x4[index - 1] = net.layers[index].conv2DLayer.filterSize[0] * net.layers[index].conv2DLayer.filterSize[1];
        IAY0000x6[index - 1] = net.layers[index].conv2DLayer.N_connections_per_node;
      }
      else
      {
        IAY0000x3[index - 1] = 0;
        IAY0000x4[index - 1] = 0;
        IAY0000x5[index - 1] = 0;
        IAY0000x6[index - 1] = 0;
      }
    }
    for (int index = 0; index < net.LayerCount(); ++index)
    {
      activationFunctionList[index] = net.layers[index].activationFunction;
      IAY0000x1[index] = net.layers[index].layer_type;
      IAY0000x2[index] = net.layers[index].layerSize;
    }
  }

  private static float Cost(
    float[] JU0000x31,
    float[] EQ0000x91,
    Noedify_Solver.CostFunction costFunction)
  {
    float num = 0.0f;
    int length = JU0000x31.Length;
    switch (costFunction)
    {
      case Noedify_Solver.CostFunction.MeanSquare:
        for (int index = 0; index < length; ++index)
          num += 0.5f * Mathf.Pow(EQ0000x91[index] - JU0000x31[index], 2f);
        break;
      case Noedify_Solver.CostFunction.CrossEntropy:
        for (int index = 0; index < length; ++index)
          num += (float) (-1.0 / (double) length * ((double) EQ0000x91[index] * (double) Mathf.Log(JU0000x31[index]) + (1.0 - (double) EQ0000x91[index]) * (double) Mathf.Log(1f - JU0000x31[index])));
        break;
      default:
        for (int index = 0; index < length; ++index)
          num += 0.5f * Mathf.Pow(EQ0000x91[index] - JU0000x31[index], 2f);
        break;
    }
    return num;
  }

  private static float Cost(
    NativeArray<float> HY0000x51,
    int OFA0000x771,
    NativeArray<float> TY0000x51,
    int OFA0000x12,
    int JU0000x31,
    Noedify_Solver.CostFunction costFunction)
  {
    float num = 0.0f;
    switch (costFunction)
    {
      case Noedify_Solver.CostFunction.MeanSquare:
        for (int index = 0; index < JU0000x31; ++index)
          num += 0.5f * Mathf.Pow(TY0000x51[OFA0000x12 + index] - HY0000x51[OFA0000x771 + index], 2f);
        break;
      case Noedify_Solver.CostFunction.CrossEntropy:
        for (int index = 0; index < JU0000x31; ++index)
          num += (float) (-1.0 / (double) JU0000x31 * ((double) TY0000x51[OFA0000x12 + index] * (double) Mathf.Log(HY0000x51[OFA0000x771 + index]) + (1.0 - (double) TY0000x51[OFA0000x12 + index]) * (double) Mathf.Log(1f - HY0000x51[OFA0000x771 + index])));
        break;
      default:
        for (int index = 0; index < JU0000x31; ++index)
          num += 0.5f * Mathf.Pow(TY0000x51[OFA0000x12 + index] - HY0000x51[OFA0000x771 + index], 2f);
        break;
    }
    return num;
  }

  private static float[] PROC0004x91(
    float[] JSP0000x72,
    float[] JSP0000x73,
    Noedify_Solver.CostFunction costFunction)
  {
    if (JSP0000x72.Length != JSP0000x73.Length)
      MonoBehaviour.print((object) ("ERROR: network output size (" + (object) JSP0000x72.Length + ") does not match training data label size (" + (object) JSP0000x73.Length + ")"));
    int length = JSP0000x72.Length;
    float[] numArray = new float[length];
    switch (costFunction)
    {
      case Noedify_Solver.CostFunction.MeanSquare:
        for (int index = 0; index < length; ++index)
          numArray[index] = JSP0000x73[index] - JSP0000x72[index];
        break;
      case Noedify_Solver.CostFunction.CrossEntropy:
        for (int index = 0; index < length; ++index)
          numArray[index] = (float) (1.0 / (double) length * ((double) JSP0000x73[index] / (1.0 / 1000.0 + (double) JSP0000x72[index]) + ((double) JSP0000x73[index] - 1.0) / (0.999000012874603 - (double) JSP0000x72[index])));
        break;
      default:
        for (int index = 0; index < length; ++index)
          numArray[index] = JSP0000x73[index] - JSP0000x72[index];
        break;
    }
    return numArray;
  }

  private static void PROC0004x91(
    ref NativeArray<float> BBA0000x13,
    int BBA0000x14,
    NativeArray<float> JSP0000x72,
    int OFA0000x771,
    NativeArray<float> JSA0000x12,
    int OFA0000x12,
    int JJA0000x41,
    Noedify_Solver.CostFunction costFunction)
  {
    switch (costFunction)
    {
      case Noedify_Solver.CostFunction.MeanSquare:
        for (int index = 0; index < JJA0000x41; ++index)
          BBA0000x13[BBA0000x14 + index] = JSA0000x12[OFA0000x12 + index] - JSP0000x72[OFA0000x771 + index];
        break;
      case Noedify_Solver.CostFunction.CrossEntropy:
        for (int index = 0; index < JJA0000x41; ++index)
          BBA0000x13[BBA0000x14 + index] = (float) (1.0 / (double) JJA0000x41 * ((double) JSA0000x12[OFA0000x12 + index] / (1.0 / 1000.0 + (double) JSP0000x72[OFA0000x771 + index]) + ((double) JSA0000x12[OFA0000x12 + index] - 1.0) / (0.999000012874603 - (double) JSP0000x72[OFA0000x771 + index])));
        break;
      default:
        for (int index = 0; index < JJA0000x41; ++index)
          BBA0000x13[BBA0000x14 + index] = JSA0000x12[OFA0000x12 + index] - JSP0000x72[OFA0000x771 + index];
        break;
    }
  }

  private static float[] PROC0000x39(
    float[] IXAx0031,
    Noedify.ActivationFunction activationFunction)
  {
    float[] numArray = new float[IXAx0031.Length];
    float num = 0.0f;
    for (int index = 0; index < IXAx0031.Length; ++index)
    {
      switch (activationFunction)
      {
        case Noedify.ActivationFunction.Sigmoid:
          numArray[index] = Noedify_Solver.Sigmoid(IXAx0031[index]);
          break;
        case Noedify.ActivationFunction.ReLU:
          numArray[index] = (double) IXAx0031[index] >= 0.0 ? IXAx0031[index] : 1f / 1000f;
          break;
        case Noedify.ActivationFunction.LeakyReLU:
          numArray[index] = (double) IXAx0031[index] >= 0.0 ? IXAx0031[index] : 0.05f * IXAx0031[index];
          break;
        case Noedify.ActivationFunction.Linear:
          numArray[index] = IXAx0031[index];
          break;
        case Noedify.ActivationFunction.SoftMax:
          numArray[index] = Mathf.Exp(IXAx0031[index]);
          num += numArray[index];
          break;
        case Noedify.ActivationFunction.ELU:
          numArray[index] = (double) IXAx0031[index] <= 0.0 ? Mathf.Exp(IXAx0031[index]) - 1f : IXAx0031[index];
          break;
        case Noedify.ActivationFunction.Hard_sigmoid:
          numArray[index] = (double) IXAx0031[index] <= -2.5 ? ((double) IXAx0031[index] <= 2.5 ? (float) (0.200000002980232 * (double) IXAx0031[index] + 0.5) : 1f) : 0.0f;
          break;
        case Noedify.ActivationFunction.Tanh:
          numArray[index] = Noedify_Solver.Tanh(IXAx0031[index]);
          break;
        default:
          numArray[index] = (float) (1.0 / (1.0 + (double) Mathf.Exp(-IXAx0031[index])));
          break;
      }
    }
    if (activationFunction == Noedify.ActivationFunction.SoftMax)
    {
      for (int index = 0; index < IXAx0031.Length; ++index)
        numArray[index] /= num + 1f / 1000f;
    }
    return numArray;
  }

  private static void PROC0000x39(
    ref NativeArray<float> IXA0000x241,
    NativeArray<float> IXA0000x661,
    int layerStartIndex,
    int layerSize,
    Noedify.ActivationFunction activationFunction)
  {
    float num = 0.0f;
    for (int index = layerStartIndex; index < layerStartIndex + layerSize; ++index)
    {
      switch (activationFunction)
      {
        case Noedify.ActivationFunction.Sigmoid:
          IXA0000x241[index] = Noedify_Solver.Sigmoid(IXA0000x661[index]);
          break;
        case Noedify.ActivationFunction.ReLU:
          IXA0000x241[index] = (double) IXA0000x661[index] >= 0.0 ? IXA0000x661[index] : 1f / 1000f;
          break;
        case Noedify.ActivationFunction.LeakyReLU:
          IXA0000x241[index] = (double) IXA0000x661[index] >= 0.0 ? IXA0000x661[index] : 0.05f * IXA0000x661[index];
          break;
        case Noedify.ActivationFunction.Linear:
          IXA0000x241[index] = IXA0000x661[index];
          break;
        case Noedify.ActivationFunction.SoftMax:
          IXA0000x241[index] = Mathf.Exp(IXA0000x661[index]);
          num += IXA0000x241[index];
          break;
        case Noedify.ActivationFunction.ELU:
          IXA0000x241[index] = (double) IXA0000x661[index] <= 0.0 ? Mathf.Exp(IXA0000x661[index]) - 1f : IXA0000x661[index];
          break;
        case Noedify.ActivationFunction.Hard_sigmoid:
          IXA0000x241[index] = (double) IXA0000x661[index] <= -2.5 ? ((double) IXA0000x661[index] <= 2.5 ? (float) (0.200000002980232 * (double) IXA0000x661[index] + 0.5) : 1f) : 0.0f;
          break;
        case Noedify.ActivationFunction.Tanh:
          IXA0000x241[index] = Noedify_Solver.Tanh(IXA0000x661[index]);
          break;
        default:
          IXA0000x241[index] = (float) (1.0 / (1.0 + (double) Mathf.Exp(-IXA0000x661[index])));
          break;
      }
    }
    if (activationFunction != Noedify.ActivationFunction.SoftMax)
      return;
    for (int index = layerStartIndex; index < layerStartIndex + layerSize; ++index)
      IXA0000x241[index] /= num + 1f / 1000f;
  }

  private static float[] PROC0000x38(
    float[] IXA0000x661,
    Noedify.ActivationFunction activationFunction)
  {
    float[] numArray = new float[IXA0000x661.Length];
    float num1 = 0.0f;
    for (int index = 0; index < IXA0000x661.Length; ++index)
    {
      switch (activationFunction)
      {
        case Noedify.ActivationFunction.Sigmoid:
          numArray[index] = Noedify_Solver.Sigmoid(IXA0000x661[index]) * (1f - Noedify_Solver.Sigmoid(IXA0000x661[index]));
          break;
        case Noedify.ActivationFunction.ReLU:
          numArray[index] = (double) IXA0000x661[index] >= 0.0 ? 1f : 1f / 1000f;
          break;
        case Noedify.ActivationFunction.LeakyReLU:
          numArray[index] = (double) IXA0000x661[index] >= 0.0 ? 1f : 0.05f;
          break;
        case Noedify.ActivationFunction.Linear:
          numArray[index] = 1f;
          break;
        case Noedify.ActivationFunction.SoftMax:
          numArray[index] = Mathf.Exp(IXA0000x661[index]);
          num1 += numArray[index];
          break;
        case Noedify.ActivationFunction.ELU:
          numArray[index] = (double) IXA0000x661[index] <= 0.0 ? Mathf.Exp(IXA0000x661[index]) : 1f;
          break;
        case Noedify.ActivationFunction.Hard_sigmoid:
          numArray[index] = (double) IXA0000x661[index] <= -2.5 ? ((double) IXA0000x661[index] <= 2.5 ? 0.2f : 0.0f) : 0.0f;
          break;
        case Noedify.ActivationFunction.Tanh:
          numArray[index] = 1f - Mathf.Pow(Noedify_Solver.Tanh(IXA0000x661[index]), 2f);
          break;
        default:
          numArray[index] = Noedify_Solver.Sigmoid(IXA0000x661[index]) * (1f - Noedify_Solver.Sigmoid(IXA0000x661[index]));
          break;
      }
    }
    if (activationFunction == Noedify.ActivationFunction.SoftMax)
    {
      float num2 = (float) ((double) num1 * (double) num1 + 1.0 / 1000.0);
      for (int index = 0; index < IXA0000x661.Length; ++index)
        numArray[index] = numArray[index] * (num1 - numArray[index]) / num2;
    }
    return numArray;
  }

  private static void PROC0000x38(
    ref NativeArray<float> IXA0000x241,
    int IXO0000x551,
    NativeArray<float> IXA0000x661,
    int IXO0000x621,
    int layerSize,
    Noedify.ActivationFunction activationFunction)
  {
    float num1 = 0.0f;
    for (int index = 0; index < layerSize; ++index)
    {
      switch (activationFunction)
      {
        case Noedify.ActivationFunction.Sigmoid:
          IXA0000x241[IXO0000x551 + index] = Noedify_Solver.Sigmoid(IXA0000x661[IXO0000x621 + index]) * (1f - Noedify_Solver.Sigmoid(IXA0000x661[IXO0000x621 + index]));
          break;
        case Noedify.ActivationFunction.ReLU:
          IXA0000x241[IXO0000x551 + index] = (double) IXA0000x661[IXO0000x621 + index] >= 0.0 ? 1f : 1f / 1000f;
          break;
        case Noedify.ActivationFunction.LeakyReLU:
          IXA0000x241[IXO0000x551 + index] = (double) IXA0000x661[IXO0000x621 + index] >= 0.0 ? 1f : 0.05f;
          break;
        case Noedify.ActivationFunction.Linear:
          IXA0000x241[IXO0000x551 + index] = 1f;
          break;
        case Noedify.ActivationFunction.SoftMax:
          IXA0000x241[IXO0000x551 + index] = Mathf.Exp(IXA0000x661[IXO0000x621 + index]);
          num1 += IXA0000x241[IXO0000x551 + index];
          break;
        case Noedify.ActivationFunction.ELU:
          IXA0000x241[IXO0000x551 + index] = (double) IXA0000x661[IXO0000x621 + index] <= 0.0 ? Mathf.Exp(IXA0000x661[IXO0000x621 + index]) : 1f;
          break;
        case Noedify.ActivationFunction.Hard_sigmoid:
          IXA0000x241[IXO0000x551 + index] = (double) IXA0000x661[IXO0000x621 + index] <= -2.5 ? ((double) IXA0000x661[IXO0000x621 + index] <= 2.5 ? 0.2f : 0.0f) : 0.0f;
          break;
        case Noedify.ActivationFunction.Tanh:
          IXA0000x241[IXO0000x551 + index] = 1f - Mathf.Pow(Noedify_Solver.Tanh(IXA0000x661[IXO0000x621 + index]), 2f);
          break;
        default:
          IXA0000x241[IXO0000x551 + index] = Noedify_Solver.Sigmoid(IXA0000x661[IXO0000x621 + index]) * (1f - Noedify_Solver.Sigmoid(IXA0000x661[IXO0000x621 + index]));
          break;
      }
    }
    if (activationFunction != Noedify.ActivationFunction.SoftMax)
      return;
    float num2 = (float) ((double) num1 * (double) num1 + 1.0 / 1000.0);
    for (int index = 0; index < layerSize; ++index)
      IXA0000x241[IXO0000x551 + index] = IXA0000x241[IXO0000x551 + index] * (num1 - IXA0000x241[IXO0000x551 + index]) / num2;
  }

  private NativeArray<float> NativeAllocFloat(int size, Allocator allocator = Allocator.Persistent)
  {
    NativeArray<float> nativeArray = new NativeArray<float>(size, allocator);
    this.nativeArrayCleanupList.Add((object) nativeArray);
    return nativeArray;
  }

  private NativeArray<int> NativeAllocInt(int size, Allocator allocator = Allocator.Persistent)
  {
    NativeArray<int> nativeArray = new NativeArray<int>(size, allocator);
    this.nativeArrayCleanupList.Add((object) nativeArray);
    return nativeArray;
  }

  private void CleanupTrainingNativeArrays()
  {
    if (this.nativeArrayCleanupList != null && this.nativeArrayCleanupList.Count > 0)
    {
      for (int index = 0; index < this.nativeArrayCleanupList.Count; ++index)
      {
        if (this.nativeArrayCleanupList[index] is NativeArray<int>)
          ((NativeArray<int>) this.nativeArrayCleanupList[index]).Dispose();
        else if (this.nativeArrayCleanupList[index] is NativeArray<float>)
          ((NativeArray<float>) this.nativeArrayCleanupList[index]).Dispose();
        else if (this.nativeArrayCleanupList[index] is NativeArray<Noedify.ActivationFunction>)
          ((NativeArray<Noedify.ActivationFunction>) this.nativeArrayCleanupList[index]).Dispose();
        else if (this.nativeArrayCleanupList[index] is NativeArray<Noedify.LayerType>)
          ((NativeArray<Noedify.LayerType>) this.nativeArrayCleanupList[index]).Dispose();
        else if (this.nativeArrayCleanupList[index] is NativeArray<Noedify.PoolingType>)
          ((NativeArray<Noedify.PoolingType>) this.nativeArrayCleanupList[index]).Dispose();
        else if (!this.suppressMessages)
          MonoBehaviour.print((object) ("ERROR (CleanupTrainingNativeArrays): Unknown memory object type " + this.nativeArrayCleanupList[index].ToString()));
      }
    }
    this.nativeArrayCleanupList = new List<object>();
  }

  private void OnApplicationQuit()
  {
    this.PROC001x32();
    this.CleanupTrainingNativeArrays();
    if (this.nets_cleanup_list.Count <= 0)
      return;
    for (int index = 0; index < this.nets_cleanup_list.Count; ++index)
    {
      if (this.nets_cleanup_list[index] != null && this.nets_cleanup_list[index].nativeArraysInitialized)
        this.nets_cleanup_list[index].Cleanup_Par();
    }
  }

  private static float Tanh(float x) => (float) (((double) Mathf.Exp(2f * x) - 1.0) / ((double) Mathf.Exp(2f * x) + 1.0));

  private static float Sigmoid(float x) => (float) (1.0 / (1.0 + (double) Mathf.Exp(-x)));

  public enum SolverMethod
  {
    MainThread,
    Background,
  }

  public enum CostFunction
  {
    MeanSquare,
    CrossEntropy,
  }

  [Serializable]
  public class DebugReport
  {
    public bool print_nodeOutputs;
    public bool print_nodeInputs;
    public bool print_weight_Gradients;
    public bool print_bias_Gradients;
    public bool print_deltas;
    public bool print_outputError;

    public DebugReport()
    {
      this.print_nodeOutputs = false;
      this.print_nodeInputs = false;
      this.print_weight_Gradients = false;
      this.print_bias_Gradients = false;
      this.print_deltas = false;
      this.print_outputError = false;
    }

    public Noedify_Solver.DebugReport_Par ConvertForPar() => new Noedify_Solver.DebugReport_Par()
    {
      print_nodeOutputs = this.print_nodeOutputs,
      print_nodeInputs = this.print_nodeInputs,
      print_weight_Gradients = this.print_weight_Gradients,
      print_bias_Gradients = this.print_bias_Gradients,
      print_deltas = this.print_deltas,
      print_outputError = this.print_outputError
    };
  }

  public struct DebugReport_Par
  {
    public bool print_nodeOutputs;
    public bool print_nodeInputs;
    public bool print_weight_Gradients;
    public bool print_bias_Gradients;
    public bool print_deltas;
    public bool print_outputError;
  }

  private struct TrainNetwork_Job : IJobParallelFor
  {
    [NativeDisableParallelForRestriction]
    public NativeArray<Noedify.ActivationFunction> activationFunction;
    public Noedify_Solver.CostFunction costFunction;
    [NativeDisableParallelForRestriction]
    public NativeArray<Noedify.LayerType> layerTypes;
    [NativeDisableParallelForRestriction]
    public NativeArray<Noedify.PoolingType> layerPooling;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> layerSize;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> networkWeights;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> networkBiases;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> PARA0000x5;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> PARA0000x6;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> PARA0000x9;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> nodeIndeces_start;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> ZO0000x18;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> JJR0000x18;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> deltas;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> networkWeightGradients;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> networkBiasGradients;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> VG0000x61;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> VG0000x61Start_index;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> CNN_nodes_per_filter;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> CNN_weights_per_filter;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> CNN_no_filters;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> CNN_conns_per_node;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> trainingInputs;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> trainingOutputs;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> batch_cost;
    public Noedify_Solver.DebugReport_Par debug_job;
    public int total_no_active_nodes;
    public int total_no_nodes;
    public int network_input_size;
    public int fineTuningLayerLimit_job;

    public void Execute(int batch)
    {
      for (int index = 0; index < this.network_input_size; ++index)
        this.JJR0000x18[batch * this.total_no_nodes + index] = this.trainingInputs[batch * this.network_input_size + index];
      for (int index = 1; index < this.layerSize.Length; ++index)
        Noedify_Solver.PROC0000x14_Job(index, this.layerTypes[index], this.activationFunction[index], this.layerPooling[index], this.layerSize, ref this.ZO0000x18, ref this.JJR0000x18, this.networkWeights, this.networkBiases, this.VG0000x61, this.nodeIndeces_start, this.PARA0000x5, this.PARA0000x6, this.VG0000x61Start_index, this.CNN_nodes_per_filter, this.CNN_weights_per_filter, this.CNN_conns_per_node, batch * this.total_no_nodes);
      if (this.debug_job.print_nodeOutputs)
      {
        for (int index = 0; index < this.layerSize.Length; ++index)
          Noedify.PrintArrayLine("(Job " + (object) batch + ") layer " + (object) index + " node outputs", this.JJR0000x18, new int[2]
          {
            batch * this.total_no_nodes + this.nodeIndeces_start[index],
            batch * this.total_no_nodes + this.nodeIndeces_start[index] + this.layerSize[index]
          }, true);
      }
      if (this.debug_job.print_nodeInputs)
      {
        for (int index = 0; index < this.layerSize.Length; ++index)
          Noedify.PrintArrayLine("(Job " + (object) batch + ") layer " + (object) index + " node inputs", this.ZO0000x18, new int[2]
          {
            batch * this.total_no_nodes + this.nodeIndeces_start[index],
            batch * this.total_no_nodes + this.nodeIndeces_start[index] + this.layerSize[index]
          }, true);
      }
      int num1 = this.layerSize.Length - 1;
      int num2 = this.layerSize[num1];
      Noedify_Solver.PROC0004x91(ref this.deltas, batch * this.total_no_active_nodes + this.PARA0000x9[num1 - 1], this.JJR0000x18, batch * this.total_no_nodes + this.nodeIndeces_start[num1], this.trainingOutputs, batch * num2, num2, this.costFunction);
      this.batch_cost[batch] = Noedify_Solver.Cost(this.JJR0000x18, batch * this.total_no_nodes + this.nodeIndeces_start[this.layerSize.Length - 1], this.trainingOutputs, batch * num2, num2, this.costFunction);
      if (this.debug_job.print_outputError)
      {
        string message = "(Job " + (object) batch + ") Cost Calculation: ";
        for (int index = 0; index < this.layerSize[num1]; ++index)
        {
          float num3 = this.JJR0000x18[batch * this.total_no_nodes + this.nodeIndeces_start[num1] + index];
          float trainingOutput = this.trainingOutputs[batch * num2 + index];
          float delta = this.deltas[batch * this.total_no_active_nodes + this.PARA0000x9[num1 - 1] + index];
          message = message + "(" + (object) index + ")[" + (object) num3 + " - " + (object) trainingOutput + " = " + (object) delta + "] , ";
        }
        MonoBehaviour.print((object) message);
      }
      NativeArray<float> IXA0000x241 = new NativeArray<float>(this.layerSize[num1], Allocator.Temp);
      Noedify_Solver.PROC0000x38(ref IXA0000x241, 0, this.ZO0000x18, batch * this.total_no_nodes + this.nodeIndeces_start[num1], num2, this.activationFunction[num1]);
      for (int index = 0; index < this.layerSize[num1]; ++index)
        this.deltas[batch * this.total_no_active_nodes + this.PARA0000x9[num1 - 1] + index] *= IXA0000x241[index];
      if (this.debug_job.print_deltas)
      {
        string message = "Output layer deltas: ";
        for (int index = 0; index < this.layerSize[num1]; ++index)
          message = message + "(" + (object) IXA0000x241[index] + ")" + (object) this.deltas[batch * this.total_no_active_nodes + this.PARA0000x9[num1 - 1] + index] + ", ";
        MonoBehaviour.print((object) message);
      }
      Noedify_Solver.CalculateGradientsLayer_Job(num1, batch, this.total_no_active_nodes, this.total_no_nodes, this.networkWeights.Length, this.networkBiases.Length, this.layerTypes[num1], this.layerSize, this.JJR0000x18, ref this.networkWeightGradients, ref this.networkBiasGradients, this.VG0000x61, this.PARA0000x9, this.nodeIndeces_start, this.PARA0000x5, this.PARA0000x6, this.VG0000x61Start_index, this.CNN_nodes_per_filter, this.CNN_weights_per_filter, this.CNN_conns_per_node, this.deltas);
      for (int index1 = this.layerSize.Length - 2; index1 > 0; --index1)
      {
        int num4 = index1;
        int num5 = index1 + 1;
        int num6 = index1 - 1;
        Noedify_Solver.BackPropagateLayer_Job(num4, num5, num1, batch, this.total_no_active_nodes, this.total_no_nodes, this.activationFunction[num4], this.layerPooling[num5], this.layerTypes, this.layerSize, this.VG0000x61, this.PARA0000x9, this.nodeIndeces_start, this.ZO0000x18, this.JJR0000x18, this.networkWeights, this.PARA0000x5, this.VG0000x61Start_index, this.CNN_nodes_per_filter, this.CNN_weights_per_filter, this.CNN_conns_per_node, ref this.deltas);
        if (this.debug_job.print_deltas)
        {
          string message = "Layer " + (object) index1 + " deltas: ";
          for (int index2 = 0; index2 < this.layerSize[num4]; ++index2)
            message = message + (object) this.deltas[batch * this.total_no_active_nodes + this.PARA0000x9[num4 - 1] + index2] + ", ";
          MonoBehaviour.print((object) message);
        }
        Noedify_Solver.CalculateGradientsLayer_Job(num4, batch, this.total_no_active_nodes, this.total_no_nodes, this.networkWeights.Length, this.networkBiases.Length, this.layerTypes[num4], this.layerSize, this.JJR0000x18, ref this.networkWeightGradients, ref this.networkBiasGradients, this.VG0000x61, this.PARA0000x9, this.nodeIndeces_start, this.PARA0000x5, this.PARA0000x6, this.VG0000x61Start_index, this.CNN_nodes_per_filter, this.CNN_weights_per_filter, this.CNN_conns_per_node, this.deltas);
        if (index1 == this.fineTuningLayerLimit_job)
          break;
      }
    }
  }

  private struct EvaluateNetwork_Job : IJobParallelFor
  {
    [NativeDisableParallelForRestriction]
    public NativeArray<Noedify.ActivationFunction> activationFunction;
    [NativeDisableParallelForRestriction]
    public NativeArray<Noedify.LayerType> KJU0000x441;
    [NativeDisableParallelForRestriction]
    public NativeArray<Noedify.PoolingType> layerPooling;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> layerSize;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> KJU0000x313;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> KJU0000x21;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> KJU0000x241;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> PARA0000x6;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> KJU0000x71;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> KJU0000x23;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> ZO0000x18;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> JJR0000x18;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> VG0000x61;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> VG0000x61Start_index;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> CNN_nodes_per_filter;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> JUO0000x91;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> JUO0000x90;
    [NativeDisableParallelForRestriction]
    public NativeArray<int> JUO0000x92;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> networkInputs;
    [NativeDisableParallelForRestriction]
    public NativeArray<float> networkOutputs;
    public int total_no_active_nodes;
    public int network_input_size;

    public void Execute(int batch)
    {
      int index1 = this.layerSize.Length - 1;
      for (int index2 = 0; index2 < this.network_input_size; ++index2)
        this.JJR0000x18[index2] = this.networkInputs[batch * this.network_input_size + index2];
      for (int index3 = 1; index3 < this.layerSize.Length; ++index3)
        Noedify_Solver.PROC0000x14_Job(index3, this.KJU0000x441[index3], this.activationFunction[index3], this.layerPooling[index3], this.layerSize, ref this.ZO0000x18, ref this.JJR0000x18, this.KJU0000x313, this.KJU0000x21, this.VG0000x61, this.KJU0000x23, this.KJU0000x241, this.PARA0000x6, this.VG0000x61Start_index, this.CNN_nodes_per_filter, this.JUO0000x91, this.JUO0000x92);
      for (int index4 = 0; index4 < this.layerSize[index1]; ++index4)
        this.networkOutputs[index4] = this.JJR0000x18[this.KJU0000x23[index1] + index4];
    }
  }
}

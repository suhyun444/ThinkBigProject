// Decompiled with JetBrains decompiler
// Type: Noedify_Convolutional2D
// Assembly: Noedify, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 8080F0DD-0A3F-460C-80E2-226F05F6E4A2
// Assembly location: D:\Unity\Github\RaisingDragons\RecognizeHandWrittenDigitsTest\Assets\Noedify\dll\Noedify.dll

using System;
using UnityEngine;

public class Noedify_Convolutional2D : MonoBehaviour
{
    [Serializable]
    public class Convolutional2DLayer
    {
        public int[] stride;
        public int no_filters;
        public int[] filterSize;
        public int[] padding;
        public int[,] connections;
        public int[,] CPAR000x6;
        public bool[,,,] CPAR000x7;
        public int[,] VG0000x61;
        public int[] CPAR0000x7;
        public int[] CPAR0000x8;
        public int[] CPAR0000x9;
        public int N_weights_per_filter;
        public int N_connections_per_node;

        public Convolutional2DLayer()
        {
            this.stride = new int[2];
            this.padding = new int[2];
            this.filterSize = new int[2];
        }

        public void BuildConnections(Noedify.Layer previousLayer, Noedify.Layer layer)
        {
            if (previousLayer.layer_type == Noedify.LayerType.Input2D)
                this.N_weights_per_filter = this.filterSize[0] * this.filterSize[1];
            else if (previousLayer.layer_type == Noedify.LayerType.Convolutional2D)
                this.N_weights_per_filter = this.filterSize[0] * this.filterSize[1] * previousLayer.conv2DLayer.no_filters;
            else if (previousLayer.layer_type == Noedify.LayerType.Pool2D)
                this.N_weights_per_filter = this.filterSize[0] * this.filterSize[1] * previousLayer.conv2DLayer.no_filters;
            this.VG0000x61 = new int[previousLayer.layerSize, layer.layerSize];
            this.CPAR0000x7 = new int[layer.layerSize];
            this.CPAR0000x8 = new int[layer.layerSize];
            this.CPAR0000x9 = new int[layer.layerSize];
            int num1 = layer.layerSize2D[0] * layer.layerSize2D[1] * this.no_filters;
            int num2 = layer.layerSize2D[0] * layer.layerSize2D[1];
            int num3 = 0;
            int num4 = 0;
            int num5 = 0;
            this.N_connections_per_node = this.filterSize[0] * this.filterSize[1];
            if (previousLayer.layer_type == Noedify.LayerType.Convolutional2D | previousLayer.layer_type == Noedify.LayerType.Pool2D)
                this.N_connections_per_node *= previousLayer.conv2DLayer.no_filters;
            for (int index = 0; index < layer.layerSize; ++index)
            {
                this.CPAR0000x7[index] = num4;
                this.CPAR0000x8[index] = num3;
                this.CPAR0000x9[index] = num5;
                ++num5;
                if (num5 == num2)
                {
                    num5 = 0;
                    ++num4;
                    if (num4 == layer.conv2DLayer.no_filters)
                    {
                        num4 = 0;
                        ++num3;
                    }
                }
            }
            this.connections = new int[layer.layerSize, this.N_connections_per_node];
            this.CPAR000x6 = new int[layer.layerSize, this.N_connections_per_node];
            if (previousLayer.layer_type == Noedify.LayerType.Input2D)
            {
                int num6 = previousLayer.layerSize2D[0] * previousLayer.layerSize2D[1];
                for (int index1 = 0; index1 < layer.layerSize; ++index1)
                {
                    for (int index2 = 0; index2 < this.filterSize[0] * this.filterSize[1]; ++index2)
                    {
                        this.connections[index1, index2] = -1;
                        this.CPAR000x6[index1, index2] = -1;
                    }
                }
                int num7 = this.no_filters / previousLayer.channels;
                for (int index3 = 0; index3 < previousLayer.channels; ++index3)
                {
                    int[,] numArray1 = new int[previousLayer.layerSize2D[0], previousLayer.layerSize2D[1]];
                    for (int index4 = 0; index4 < previousLayer.layerSize2D[0]; ++index4)
                    {
                        for (int index5 = 0; index5 < previousLayer.layerSize2D[1]; ++index5)
                            numArray1[index4, index5] = index3 * previousLayer.layerSize2D[1] * previousLayer.layerSize2D[0] + index4 * previousLayer.layerSize2D[1] + index5;
                    }
                    for (int index6 = index3 * num7; index6 < (index3 + 1) * num7; ++index6)
                    {
                        int index7 = 0;
                        int num8 = -this.padding[1];
                        int num9 = -this.padding[0];
                        int[] numArray2 = new int[2];
                        int[] layerSize2D = layer.layerSize2D;
                        for (int index8 = 0; index8 < layerSize2D[0]; ++index8)
                        {
                            for (int index9 = 0; index9 < layerSize2D[1]; ++index9)
                            {
                                for (int index10 = num9; index10 < num9 + this.filterSize[0]; ++index10)
                                {
                                    for (int index11 = num8; index11 < num8 + this.filterSize[1]; ++index11)
                                    {
                                        if (index10 < previousLayer.layerSize2D[0] & index11 < previousLayer.layerSize2D[1] & index10 >= 0 & index11 >= 0)
                                        {
                                            this.connections[index6 * (layerSize2D[0] * layerSize2D[1]) + index8 * layerSize2D[1] + index9, index7] = numArray1[index10, index11];
                                            this.CPAR000x6[index6 * (layerSize2D[0] * layerSize2D[1]) + index8 * layerSize2D[1] + index9, index7] = (index10 - num9) * this.filterSize[1] + (index11 - num8);
                                            ++index7;
                                        }
                                    }
                                }
                                num8 += this.stride[1];
                                index7 = 0;
                            }
                            num9 += this.stride[0];
                            num8 = -this.padding[1];
                        }
                    }
                }
            }
            else if (previousLayer.layer_type == Noedify.LayerType.Convolutional2D | previousLayer.layer_type == Noedify.LayerType.Pool2D)
            {
                for (int index12 = 0; index12 < layer.layerSize; ++index12)
                {
                    for (int index13 = 0; index13 < previousLayer.conv2DLayer.no_filters * this.filterSize[0] * this.filterSize[1]; ++index13)
                    {
                        this.connections[index12, index13] = -1;
                        this.CPAR000x6[index12, index13] = -1;
                    }
                }
                int num10 = 0;
                for (int index14 = 0; index14 < previousLayer.conv2DLayer.no_filters; ++index14)
                {
                    int[,] numArray3 = new int[previousLayer.layerSize2D[0], previousLayer.layerSize2D[1]];
                    for (int index15 = 0; index15 < previousLayer.layerSize2D[0]; ++index15)
                    {
                        for (int index16 = 0; index16 < previousLayer.layerSize2D[1]; ++index16)
                            numArray3[index15, index16] = index14 * previousLayer.layerSize2D[1] * previousLayer.layerSize2D[0] + index15 * previousLayer.layerSize2D[1] + index16;
                    }
                    for (int index17 = 0; index17 < this.no_filters; ++index17)
                    {
                        int index18 = index14 * this.filterSize[0] * this.filterSize[1];
                        int num11 = -this.padding[1];
                        int num12 = -this.padding[0];
                        int[] numArray4 = new int[2];
                        int[] layerSize2D = layer.layerSize2D;
                        for (int index19 = 0; index19 < layerSize2D[0]; ++index19)
                        {
                            for (int index20 = 0; index20 < layerSize2D[1]; ++index20)
                            {
                                for (int index21 = num12; index21 < num12 + this.filterSize[0]; ++index21)
                                {
                                    for (int index22 = num11; index22 < num11 + this.filterSize[1]; ++index22)
                                    {
                                        if (index21 < previousLayer.layerSize2D[0] & index22 < previousLayer.layerSize2D[1] & index22 >= 0 & index21 >= 0)
                                        {
                                            this.connections[index17 * (layerSize2D[0] * layerSize2D[1]) + index19 * layerSize2D[1] + index20, index18] = numArray3[index21, index22];
                                            this.CPAR000x6[index17 * (layerSize2D[0] * layerSize2D[1]) + index19 * layerSize2D[1] + index20, index18] = index14 * this.filterSize[0] * this.filterSize[1] + (index21 - num12) * this.filterSize[1] + (index22 - num11);
                                            ++index18;
                                            ++num10;
                                        }
                                    }
                                }
                                num11 += this.stride[1];
                                index18 = index14 * this.filterSize[0] * this.filterSize[1];
                            }
                            num12 += this.stride[0];
                            num11 = -this.padding[1];
                        }
                    }
                }
            }
            for (int index23 = 0; index23 < previousLayer.layerSize; ++index23)
            {
                for (int index24 = 0; index24 < layer.layerSize; ++index24)
                    this.VG0000x61[index23, index24] = -1;
            }
            for (int index25 = 0; index25 < layer.layerSize; ++index25)
            {
                for (int index26 = 0; index26 < this.N_connections_per_node; ++index26)
                {
                    if (this.connections[index25, index26] >= 0)
                        this.VG0000x61[this.connections[index25, index26], index25] = this.CPAR000x6[index25, index26];
                }
            }
        }

        public void BuildConnectionsPool2D(Noedify.Layer previousLayer, Noedify.Layer layer)
        {
            this.VG0000x61 = new int[previousLayer.layerSize, layer.layerSize];
            this.CPAR0000x9 = new int[layer.layerSize];
            this.CPAR0000x7 = new int[layer.layerSize];
            this.no_filters = previousLayer.conv2DLayer.no_filters;
            int num1 = 0;
            int num2 = 0;
            int num3 = layer.layerSize2D[0] * layer.layerSize2D[1];
            this.N_connections_per_node = this.filterSize[0] * this.filterSize[1];
            for (int index = 0; index < layer.layerSize; ++index)
            {
                this.CPAR0000x9[index] = num1++;
                if (num1 == num3)
                {
                    this.CPAR0000x7[index] = num2;
                    ++num2;
                }
            }
            this.connections = new int[layer.layerSize, this.N_connections_per_node];
            this.CPAR000x6 = new int[layer.layerSize, this.N_connections_per_node];
            for (int index1 = 0; index1 < layer.layerSize; ++index1)
            {
                for (int index2 = 0; index2 < this.N_connections_per_node; ++index2)
                {
                    this.connections[index1, index2] = -1;
                    this.CPAR000x6[index1, index2] = -1;
                }
            }
            int num4 = 0;
            for (int index3 = 0; index3 < previousLayer.conv2DLayer.no_filters; ++index3)
            {
                int[,] numArray1 = new int[previousLayer.layerSize2D[0], previousLayer.layerSize2D[1]];
                for (int index4 = 0; index4 < previousLayer.layerSize2D[0]; ++index4)
                {
                    for (int index5 = 0; index5 < previousLayer.layerSize2D[1]; ++index5)
                        numArray1[index4, index5] = index3 * previousLayer.layerSize2D[1] * previousLayer.layerSize2D[0] + index4 * previousLayer.layerSize2D[1] + index5;
                }
                int index6 = 0;
                int num5 = -this.padding[1];
                int num6 = -this.padding[0];
                int[] numArray2 = new int[2]
                {
          Mathf.CeilToInt((float) (((double) previousLayer.layerSize2D[0] - (double) this.filterSize[0]) / (double) this.stride[0] + 1.0)),
          Mathf.CeilToInt((float) (((double) previousLayer.layerSize2D[1] - (double) this.filterSize[1]) / (double) this.stride[1] + 1.0))
                };
                for (int index7 = 0; index7 < numArray2[0]; ++index7)
                {
                    for (int index8 = 0; index8 < numArray2[1]; ++index8)
                    {
                        for (int index9 = num6; index9 < num6 + this.filterSize[0]; ++index9)
                        {
                            for (int index10 = num5; index10 < num5 + this.filterSize[1]; ++index10)
                            {
                                if (index9 < previousLayer.layerSize2D[0] & index10 < previousLayer.layerSize2D[1] & index10 >= 0 & index9 >= 0)
                                {
                                    this.connections[index3 * (numArray2[0] * numArray2[1]) + index7 * numArray2[1] + index8, index6] = numArray1[index9, index10];
                                    this.CPAR000x6[index3 * (numArray2[0] * numArray2[1]) + index7 * numArray2[1] + index8, index6] = index3 * this.filterSize[0] * this.filterSize[1] + (index9 - num6) * this.filterSize[1] + (index10 - num5);
                                    ++num4;
                                    ++index6;
                                }
                            }
                        }
                        num5 += this.stride[1];
                        index6 = 0;
                    }
                    num6 += this.stride[0];
                    num5 = -this.padding[1];
                }
            }
            for (int index11 = 0; index11 < previousLayer.layerSize; ++index11)
            {
                for (int index12 = 0; index12 < layer.layerSize; ++index12)
                    this.VG0000x61[index11, index12] = -1;
            }
            for (int index13 = 0; index13 < layer.layerSize; ++index13)
            {
                for (int index14 = 0; index14 < this.N_connections_per_node; ++index14)
                {
                    if (this.connections[index13, index14] >= 0)
                        this.VG0000x61[this.connections[index13, index14], index13] = this.CPAR000x6[index13, index14];
                }
            }
        }
    }
}

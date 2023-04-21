//
// using System.Numerics;
// using FftSharp;
// using UnityEngine;
// using Complex = FftSharp.Complex;
// using Vector2 = UnityEngine.Vector2;
//
// namespace VectorTools
// {
//     public class RiverFFT
//     {
//         // Vector2[] Do(Vector2[] riverData)
//         // {
//         //     int n = riverData.Length;
//         //     Complex[] input = new Complex[n];
//         //     for (int i = 0; i < n; i++)
//         //     {
//         //         input[i] = new Complex(riverData[i].x, riverData[i].y);
//         //     }
//         //
//         //     FftSharp.Transform.FFT(input);
//         //     
//         //     FftSharp.Transform.IFFT(input);
//         //     
//         //     Vector2[] newData = new Vector2[n];
//         //     for (int i = 0; i < n; i++)
//         //     {
//         //         newData[i] = new Vector2((float)input[i].Real, (float)input[i].Imaginary);
//         //     }
//         //
//         //     return newData;
//         // }
//         
//         
//         // take a vector2[] and apply an FFT to it, then invert it and return
//         
//         public static Vector2[] Do(Vector2[] riverData,float sampleRate, float cutoff)
//         {
//             Debug.Log("do");
//             int n = riverData.Length;
//             int paddedLength = (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(n, 2)));
//             Complex[] input = new Complex[paddedLength];
//             for (int i = 0; i < n; i++)
//             {
//                 input[i] = new Complex(riverData[i].x, riverData[i].y);
//             }
//
//             FftSharp.Transform.FFT(input);
//
//
//             input = ApplyHighPass(input, sampleRate, cutoff);
//             
//             
//             FftSharp.Transform.IFFT(input);
//             
//             Vector2[] newData = new Vector2[n];
//             for (int i = 0; i < n; i++)
//             {
//                 newData[i] = new Vector2((float)input[i].Real, (float)input[i].Imaginary);
//             }
//
//             return newData;
//         }
//         
//         public static Complex[] ApplyHighPass(Complex[] fftOutput, float sampleRate, float cutoff)
//         {
//             int n = fftOutput.Length;
//             float nyquistFreq = sampleRate / 2f;
//             int cutoffIndex = Mathf.RoundToInt(cutoff * n / nyquistFreq);
//
//             if (cutoffIndex >= n / 2)
//             {
//                 cutoffIndex = n / 2 - 1;
//             }
//
//             for (int i = 0; i < cutoffIndex; i++)
//             {
//                 fftOutput[i] = Complex.Zero;
//                 fftOutput[n - i - 1] = Complex.Zero;
//             }
//
//             return fftOutput;
//         }
//
//         public static Vector2[] DoHighPass(Vector2[] input, float sampleRate, float cutoff)
//         {
//             int n = input.Length;
//             int paddedLength = (int)Mathf.Pow(2, Mathf.Ceil(Mathf.Log(n, 2)));
//
//             // Separate x and y coordinates into two arrays
//             double[] x = new double[paddedLength];
//             double[] y = new double[paddedLength];
//             for (int i = 0; i < n; i++)
//             {
//                 x[i] = input[i].x;
//                 y[i] = input[i].y;
//             }
//
//             // Compute FFT of x and y coordinates separately
//             Complex[] xFftOutput = FftSharp.Transform.FFT(x);
//             Complex[] yFftOutput = FftSharp.Transform.FFT(y);
//
//             // Compute cutoff index based on specified cutoff frequency
//             float nyquistFreq = sampleRate / 2f;
//             int cutoffIndex = Mathf.RoundToInt(cutoff * n / nyquistFreq);
//
//             // Apply high-pass filter by setting low-frequency components to zero
//             for (int i = 0; i < cutoffIndex; i++)
//             {
//                 xFftOutput[i] = Complex.Zero;
//                 xFftOutput[n - i - 1] = Complex.Zero;
//
//                 yFftOutput[i] = Complex.Zero;
//                 yFftOutput[n - i - 1] = Complex.Zero;
//             }
//
//             // Convert filtered frequency components back to time-domain signal using IFFT
//             FftSharp.Transform.IFFT(xFftOutput);
//             FftSharp.Transform.IFFT(yFftOutput);
//
//             // Combine filtered x and y coordinates back into Vector2[] array
//             Vector2[] output = new Vector2[n];
//             for (int i = 0; i < n; i++)
//             {
//                 output[i] = new Vector2((float)xFftOutput[i].Real, (float)yFftOutput[i].Real);
//             }
//
//             return output;
//         }
//         
//     }
// }
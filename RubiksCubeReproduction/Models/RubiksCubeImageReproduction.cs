using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Drawing;
using System.Windows.Controls.Ribbon.Primitives;
using System.Threading;
using System.Timers;
using System.Runtime.InteropServices;

namespace RubiksCubeReproduction.Models
{
    public class RubiksCubeImageReproduction
    {
        //Argument: 3 bytes = pixel's RGB values
        [DllImport(@"C:\Users\Marcin\source\repos\JALab1\x64\Debug\DLLJALAB1.dll")]
        static unsafe extern int PS_2(byte* pixelRGB, byte* colorRGBs, long* tempPtr);

        public static int miliseconds = 0;
        public byte[] OriginalImage { get; private set; }
        public byte[,,] PixelRGBs { get; private set; }
        public byte[] ImageReproduction { get; private set; }
        private Bitmap Bitmap;
        public RubiksCubeImageReproduction(string filename)
        {
            OriginalImage = File.ReadAllBytes(filename);

            ImageReproduction = new byte[OriginalImage.Length];
            Bitmap = new Bitmap(filename, true);

            //make threedimensional array of RGB values.
            PixelRGBs = new byte[Bitmap.Width, Bitmap.Height, 3];
            for (int x = 0; x < Bitmap.Width; x++)
            {
                for (int y = 0; y < Bitmap.Height; y++)
                {
                    Color pixelColor = Bitmap.GetPixel(x, y);
                    PixelRGBs[x, y, 0] = pixelColor.R;
                    PixelRGBs[x, y, 1] = pixelColor.G;
                    PixelRGBs[x, y, 2] = pixelColor.B;
                }
            }
        }

        private void SaveBitmapToBitArray()
        {
            using (var ms = new MemoryStream())
            {
                Bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                //OriginalImage.Length - (int)ms.Length
                for (int ImageReproductionIterator = 0; ImageReproductionIterator < ImageReproduction.Length; ImageReproductionIterator++)
                    ImageReproduction[ImageReproductionIterator] = (byte)ms.ReadByte();

                //OriginalImage[0] = OriginalImage[0];
            }
        }

        private static void Add1Milisecond(Object source, ElapsedEventArgs e)
        {
            miliseconds += 1;
        }

        public int GenerateImageReproduction(bool isAssemblerLibraryActive, int numberOfThreads)
        {
            System.Timers.Timer _computationTimeTimer = new System.Timers.Timer(1);
            _computationTimeTimer.Elapsed += Add1Milisecond;
            _computationTimeTimer.AutoReset = true;
            _computationTimeTimer.Start();

            List<ThreadSettings> threadSettings = divideImageForThreads(numberOfThreads);
            List<Thread> threads = new List<Thread>();

            foreach (ThreadSettings settings in threadSettings)
            {
                if (isAssemblerLibraryActive)
                {
                    threads.Add(new Thread(() =>
                        GenerateImageReproductionInAssembly(settings.ImgColStart, settings.ImgColStopBefore, settings.ImgHeight)
                    ));
                }
                else //C# library is active
                {
                    threads.Add(new Thread(() =>
                        GenerateImageReproductionInCSharp(settings.ImgColStart, settings.ImgColStopBefore, settings.ImgHeight)
                    ));
                }
            }

            foreach (Thread thread in threads)
            {
                thread.Start();
            }
            foreach (Thread thread in threads)
            {
                thread.Join();
            }
            _computationTimeTimer.Stop();
            _computationTimeTimer.Close();
            SavePixelRGBsToBitmap();
            SaveBitmapToBitArray();
            int milisecondsCopy = miliseconds;
            miliseconds = 0;
            return milisecondsCopy;
        }

        private unsafe (byte, byte, byte) ModifyOnePixelInAssembly(byte r, byte g, byte b)
        {
            fixed (byte* pixelPtr = new byte[3] { r,g,b })
            {
                fixed (byte* colorPtr = new byte[18] { 0, 155, 72, 255, 255, 255, 183, 18, 52, 255, 213, 0, 0, 70, 173, 255, 88, 0 })
                {
                    fixed (long* tempPtr = new long[6] { 0, 0, 0, 0, 0, 0 })
                    {
                        int a = PS_2(pixelPtr, colorPtr, tempPtr);
                        //passing values back thru pointer works
                        return (pixelPtr[0], pixelPtr[1], pixelPtr[2]);
                    }
                }
            }
        }

        private List<ThreadSettings> divideImageForThreads(int numberOfThreads)
        {
            List<ThreadSettings> threadSettings = new List<ThreadSettings>();
            int colsToDivide = Bitmap.Width;
            int colsPerThread = colsToDivide / numberOfThreads;
            int currentCol = 0;
            for (int i = 0; i < numberOfThreads; i++)
            {
                ThreadSettings temp = new ThreadSettings
                {
                    ProcessId = i,
                    ImgWidth = Bitmap.Width,
                    ImgHeight = Bitmap.Height,
                    ImgColStart = currentCol
                };
                currentCol += colsPerThread;
                temp.ImgColStopBefore =
                    i != numberOfThreads - 1
                    ? currentCol
                    //last thread needs ImgColStopBefore to be last row+1, in case of dividing with a rest.
                    : Bitmap.Width;
                threadSettings.Add(temp);
            }

            return threadSettings;
        }

        private void GenerateImageReproductionInCSharp(int imgColStart, int imgColEndBefore, int imgHeight)
        {
            //Refactor for modifying PixelRGB, not Bitmap
            for (int x = imgColStart; x < imgColEndBefore; x++)
            {
                for (int y = 0; y < imgHeight; y++)
                {
                    Color rubiksColor = FindClosestColor(PixelRGBs[x, y, 0], PixelRGBs[x, y, 1], PixelRGBs[x, y, 2]);
                    PixelRGBs[x, y, 0] = rubiksColor.R;
                    PixelRGBs[x, y, 1] = rubiksColor.G;
                    PixelRGBs[x, y, 2] = rubiksColor.B;
                }
            }
            
        }

        private void GenerateImageReproductionInAssembly(int imgColStart, int imgColEndBefore, int imgHeight)
        {
            //Refactor for modifying PixelRGB, not Bitmap
            for (int x = imgColStart; x < imgColEndBefore; x++)
            {
                for (int y = 0; y < imgHeight; y++)
                {
                    var foundRGBValues = ModifyOnePixelInAssembly(PixelRGBs[x, y, 0], PixelRGBs[x, y, 1], PixelRGBs[x, y, 2]);
                    PixelRGBs[x, y, 0] = foundRGBValues.Item1;
                    PixelRGBs[x, y, 1] = foundRGBValues.Item2;
                    PixelRGBs[x, y, 2] = foundRGBValues.Item3;
                }
            }

        }

        private void SavePixelRGBsToBitmap()
        {
            for (int x = 0; x < Bitmap.Width; x++)
            {
                for (int y = 0; y < Bitmap.Height; y++)
                {
                    Color rubiksColor = Color.FromArgb(PixelRGBs[x, y, 0], PixelRGBs[x, y, 1], PixelRGBs[x, y, 2]);
                    Bitmap.SetPixel(x, y, rubiksColor);
                }
            }
        }
        private Color FindClosestColor(int r, int g, int b)
        {
            Color Green = Color.FromArgb(0, 155, 72);
            Color White = Color.FromArgb(255, 255, 255);
            Color Red = Color.FromArgb(183, 18, 52);
            Color Yellow = Color.FromArgb(255, 213, 0);
            Color Blue = Color.FromArgb(0, 70, 173);
            Color Orange = Color.FromArgb(255, 88, 0);
            List<Color> Colors= new List<Color>{ Green, White, Red, Yellow, Blue, Orange };
            List<int> Distances = new List<int>(6);
            for (int i = 0; i < Colors.Count; i++)
            {
                Distances.Add(
                    Math.Abs(Colors[i].R - r)
                        + Math.Abs(Colors[i].G - g)
                        + Math.Abs(Colors[i].B - b)
                );
            }
            int maxFound = 9999;
            int indexFound = 9999;
            for(int i = 0; i < Distances.Count; i++)
            {
                if (Distances[i] < maxFound)
                {
                    maxFound = Distances[i];
                    indexFound = i;
                }   
            }
            return Colors[indexFound];
        }
    }
}

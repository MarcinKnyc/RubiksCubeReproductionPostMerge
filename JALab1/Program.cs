using System;
using System.Runtime.InteropServices;
namespace JaProj
{
    class Program
    {
        [DllImport(@"D:\Polsl\sem IV\JA\JALab1\x64\Debug\DLLJALAB1.dll")]
        static extern int MyProc1(int a, int b);
        static void Main(string[] args)
        {
            int x = 5, y = 3;
            int retVal = MyProc1(x, y);
            Console.Write("Moja pierwsza wartość obliczona w asm to:");
            Console.WriteLine(retVal);
            Console.ReadLine();
        }
    }
}
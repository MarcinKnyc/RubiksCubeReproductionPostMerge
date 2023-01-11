using System;
using System.Runtime.InteropServices;
namespace JaProj
{
    class Program
    {
        [DllImport(@"H:\Kopia z dysku D\Polsl\sem IV\JA\JALab1\x64\Debug\DLLJALAB1.dll")]
        static extern int PS_2();
        static void Main(string[] args)
        {
            //BENOPA, A
            char[] szString = { 'A', 'G', 'I', 'J', 'K', 'S', (char)0xFF };
            int retVal = PS_2();
            Console.Write("Moja pierwsza wartość obliczona w asm to:");
            Console.WriteLine(retVal);
            Console.ReadLine();
        }
    }
}
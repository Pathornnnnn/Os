using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using CalculatingFunctions;
//400 ms result +- 27 
class Program
{
    static decimal[] data = new decimal[11000001];
    static decimal finalResult = 0;

    private static decimal ProcessChunk(int startIndex, int endIndex)
    {
        CalClass CF = new CalClass();
        decimal localResult = 0;
        int idx = startIndex;

        while (idx < endIndex)
        {
            localResult += CF.Calculate1(ref data, ref idx);
        }

        return localResult;
    }

    private static void LoadData()
    {
        Console.WriteLine("Loading data...");
        using (FileStream fs = new FileStream("data.bin", FileMode.Open))
        using (BinaryReader br = new BinaryReader(fs))
        {
            for (int i = 0; i < data.Length; i++)
            {
                Single f = br.ReadSingle();
                data[i] = (decimal)(f * 36);
            }
        }
        Console.WriteLine("Data loaded successfully.\n");
    }

    private static void Main(string[] args)
    {
        LoadData();
        Console.WriteLine("Calculation start...");

        decimal result1 = 0, result2 = 0;
        int mid = data.Length / 2;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Thread t1 = new Thread(() => { result1 = ProcessChunk(0, mid); });
        Thread t2 = new Thread(() => { result2 = ProcessChunk(mid, data.Length); });

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        finalResult = result1 + result2;

        sw.Stop();
        Console.WriteLine($"Calculation finished in {sw.ElapsedMilliseconds} ms. Result: {finalResult:F25}");
    }
}

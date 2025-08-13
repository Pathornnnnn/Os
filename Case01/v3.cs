// Case Study 01 - Multithreading v.3 (Hybrid Fast + Accurate)
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using CalculatingFunctions;

class Program
{
    static decimal[] data = new decimal[11000001];
    static decimal finalResult = 0;

    // Thread-safe: แต่ละ thread ทำงานกับ chunk ของตัวเอง
    private static decimal ThreadWork(int startIndex, int endIndex, int loopCount)
    {
        CalClass CF = new CalClass();
        decimal localResult = 0;

        for (int loop = 0; loop < loopCount; loop++)
        {
            for (int idx = startIndex; idx < endIndex; idx++)
            {
                int tempIdx = idx; // pass by ref
                localResult += CF.Calculate1(ref data, ref tempIdx);
            }
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

        int mid = data.Length / 2;
        int loopsPerThread = 15; // ครึ่งหนึ่งของ v.1

        decimal result1 = 0, result2 = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Thread t1 = new Thread(() => { result1 = ThreadWork(0, mid, loopsPerThread); });
        Thread t2 = new Thread(() => { result2 = ThreadWork(mid, data.Length, loopsPerThread); });

        t1.Start();
        t2.Start();
        t1.Join();
        t2.Join();

        finalResult = result1 + result2;

        sw.Stop();
        Console.WriteLine($"Calculation finished in {sw.ElapsedMilliseconds} ms. Result: {finalResult:F25}");
    }
}

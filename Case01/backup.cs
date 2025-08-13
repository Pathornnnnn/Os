// Case Study 01 - Multithreading (Fixed for Reproducible Results)
using System;
using System.IO;
using System.Threading;
using System.Diagnostics;
using CalculatingFunctions;
//8000 ms +- 0.01
class Program
{
    static decimal[] data = new decimal[11000001];
    static decimal finalResult = 0; // ใช้รวมผลหลังจบทุก thread

    private static decimal ThreadWork(int startLoop, int endLoop)
    {
        CalClass CF = new CalClass();
        decimal localResult = 0; // เก็บผลเฉพาะ thread นี้

        for (int i = startLoop; i < endLoop; i++)
        {
            int localIndex = 0;
            while (localIndex < 10000000)
            {
                localResult += CF.Calculate1(ref data, ref localIndex);
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

        decimal result1 = 0, result2 = 0;

        Stopwatch sw = new Stopwatch();
        sw.Start();

        Thread Th1 = new Thread(() => { result1 = ThreadWork(0, 5); });
        Thread Th2 = new Thread(() => { result2 = ThreadWork(6, 10); });

        Th1.Start();
        Th2.Start();
        Th1.Join();
        Th2.Join();

        finalResult = result1 + result2; // รวมผล

        sw.Stop();
        Console.WriteLine($"Calculation finished in {sw.ElapsedMilliseconds} ms. Result: {finalResult:F25}");
    }
}

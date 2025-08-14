using System;
using System.IO;
using CalculatingFunctions;
using System.Threading;
using System.Diagnostics;

class Program
{
    static decimal[] data = new decimal[11000001];
    static readonly int NumThreads = Environment.ProcessorCount; // 8 cores for M3
    static decimal[] threadResults = new decimal[NumThreads]; // Each thread gets its own result slot

    private static void ThreadWork(object threadIdObj)
    {
        int threadId = (int)threadIdObj;
        CalClass CF = new CalClass();
        decimal localResult = 0; // Local result for this thread
        int localIndex = 0; // Local index for this thread
        
        // Each thread processes a portion of the data to avoid race conditions
        int totalElements = 10000000;
        int elementsPerThread = totalElements / NumThreads;
        int startIndex = threadId * elementsPerThread;
        int endIndex = (threadId == NumThreads - 1) ? totalElements : startIndex + elementsPerThread;
        
        int i = 0;
        while (i < 30)
        {
            localIndex = startIndex; // Start from this thread's portion
            while (localIndex < endIndex) // Process only this thread's portion
            {
                localResult += CF.Calculate1(ref data, ref localIndex);
            }
            i++;
        }
        
        // Store result in thread-safe manner (each thread has its own index)
        threadResults[threadId] = localResult;
    }

    private static void LoadData()
    {
        Console.WriteLine("Loading data...");
        FileStream fs = new FileStream("data.bin", FileMode.Open);
        BinaryReader br = new BinaryReader(fs);
        for (int i = 0; i < data.Length; i++)
        {
            Single f = br.ReadSingle();
            data[i] = (decimal)(f * 36);
        }
        br.Close();
        fs.Close();
        Console.WriteLine("Data loaded successfully.\n\n");
    }

    private static void Main(string[] args)
    {
        LoadData();
        Console.WriteLine($"Calculation start with {NumThreads} threads optimized for M3...");

        Thread[] threads = new Thread[NumThreads];
        Stopwatch _st = new Stopwatch();
        _st.Start();

        // Create and start all threads
        for (int i = 0; i < NumThreads; i++)
        {
            threads[i] = new Thread(ThreadWork);
            threads[i].Start(i); // Pass thread ID as parameter
        }

        // Wait for all threads to finish
        for (int i = 0; i < NumThreads; i++)
        {
            threads[i].Join();
        }

        // Combine results from all threads
        decimal finalResult = 0;
        for (int i = 0; i < NumThreads; i++)
        {
            finalResult += threadResults[i];
        }

        _st.Stop();
        Console.WriteLine($"Calculation finished in {_st.ElapsedMilliseconds} ms. Result: {finalResult.ToString("F25")}");
        Console.WriteLine($"Performance optimized for M3 with {NumThreads} threads");
    }
}
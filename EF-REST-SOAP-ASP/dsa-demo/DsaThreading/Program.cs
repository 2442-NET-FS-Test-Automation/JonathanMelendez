using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Principal;
using DsaThreading;

Console.WriteLine("Hello, World!");

await ThreadingDemo();

static async Task ThreadingDemo()
{
    // C# Thread (OS ones not CPU ones)
    // Typically they are managed by the runtime

    Console.WriteLine($"Main runs on thread: {Environment.CurrentManagedThreadId}");
    // We can create our own threads
    // The class has one argument, a delegate to execute (lambda or a function)
    // List<Thread> explosion = [];
    // while (true)
    // {
    //     var worker = new Thread(() => {
    //         Console.WriteLine($"Hello from thread: {Environment.CurrentManagedThreadId}");
    //         Thread.Sleep(30000);
    //     });
    //     explosion.Add(worker);
    //     worker.Start();
    // }
    var workerThread = new Thread(() => {
        Console.WriteLine($"Hello from thread: {Environment.CurrentManagedThreadId}");
    });

    Console.WriteLine($"Before Start() call, isAlive = {workerThread.IsAlive}");

    workerThread.Start();
    Console.WriteLine($"After Start() call, isAlive = {workerThread.IsAlive}");

    workerThread.Join();
        
    Console.WriteLine($"After Join() call, isAlive = {workerThread.IsAlive}");

    var threads = new List<Thread>();

    for (int i = 1; i <= 5; i++)
    {
        int id = i;
        var th = new Thread(() =>
        {
            Thread.Sleep(Random.Shared.Next(5,40));
            Console.WriteLine($"Worker {id} finished on thread: {Environment.CurrentManagedThreadId}"); 
        });

        threads.Add(th);
        th.Start();
    }
    foreach(Thread th in threads) th.Join();

    // Thread safe collections
    var counts = new ConcurrentDictionary<int, int>();

    var threadPool = new List<Thread>();
    for (int i = 1; i <= 4; i++)
    {
        int id = i;
        var th = new Thread(() =>
        {
            for (int k = 0; k < 1000; k++)
            {
                counts.AddOrUpdate(id, 1, (_, prev) => prev + 1);
                // takes key, value, and a delegate to execute if the key already exists
                // _ = C# discard - indicates the parameter is intentionaly ignored
                // prev - the existing integer value currently stored for the key
            }
        });

        threadPool.Add(th);
        th.Start();
    }
    foreach(Thread th in threadPool) th.Join();
    Console.WriteLine($"Recorded {counts.Values.Sum()} increments across 4 threads");
    // foreach(KeyValuePair<int, int> entry in counts) Console.WriteLine($"{entry.Key} - {entry.Value}"); 

    // ConcurrentQueue
    var done = new ConcurrentQueue<int>();
    for (int i = 0; i < 5; i++)
    {
        int n = i;
        // Instead of creating a thread manually, you can ask for one to the background ThreadPool
        ThreadPool.QueueUserWorkItem(_ => done.Enqueue(n * n));
    }

    // Crude wait, cause we dont have access to the threads like this
    while (done.Count < 5) Thread.Sleep(5);

    Console.WriteLine($"Threadpool finished. {string.Join(", ", done.OrderBy(x => x))}");
    
    
    ParallelSum();

    static void ParallelSum()
    {
        int[] data = Enumerable.Range(1, 8000000).ToArray();

        // First sequentially
        var sw = Stopwatch.StartNew();
        long sequential = SumRange(data, 0, data.Length);
        sw.Stop();
        Console.WriteLine($"Sequential sum = {sequential}. {sw.ElapsedTicks} ticks, 1 thread");

        sw.Restart();
        // Parallelize with 2 tasks
        Task<long> half1 = Task.Run(() => SumRange(data, 0, data.Length/2));
        Task<long> half2 = Task.Run(() => SumRange(data, data.Length/2, data.Length));

        long total = half1.Result + half2.Result;
        sw.Stop();
        Console.WriteLine($"Two task sum: {total}. {sw.ElapsedTicks} ticks, 2-thread");

        long parallelTotal = 0;
        sw.Restart();
        Parallel.For(0, data.Length, 
            // We give it an acumulator
            () => 0L,
            // body: for each iteration do something
            // i is the loop index; _ ParallelLoopState; local the current threads subtotal for the sum
            (i, _, local) => local + data[i],
            // localFinally: After a thread finishes all of it assigned work this is called
            local => Interlocked.Add(ref parallelTotal, local)
        );
        sw.Stop();

        Console.WriteLine($"Parallel sum = {parallelTotal}. {sw.ElapsedTicks} ticks, multi-thread");
    }
    static long SumRange(int[] a, int start, int end)
    {
        long sum = 0;
        for (int i = start; i < end; i++)
        {
            sum+=a[i];
        }
        return sum;
    }

    // No concurrent protection
    RaceDemo();
    static void RaceDemo()
    {
        var bank =  new Bank();
        Parallel.For(0, 100000, _ => bank.DepositUnsafe(1));
        Console.WriteLine($"Unsafe balance: {bank.Balance}, should be 100000");
    }

    // using lock to protect againt concurrency
    SafeDemo();
    static void SafeDemo()
    {
        var bank = new Bank();
        Parallel.For(0, 100000, _ => bank.DepositSafe(1));
        Console.WriteLine($"Safe balance: {bank.Balance}, should be 100000");
    }

    // Interlocked - lock free atomic operations against one variable
    // Faster than lock for single atmic operations
    InterlockedDemo();
    static void InterlockedDemo()
    {
        long counter = 0;

        Parallel.For(0, 100000, _ => Interlocked.Increment(ref counter));
        Console.WriteLine($"Interlocked: {counter}, should be 100000");
    }

    CancellationDemo();
    static void CancellationDemo()
    {
        using var cts = new CancellationTokenSource(TimeSpan.FromMilliseconds(100));

        CancellationToken token = cts.Token;

        var work = Task.Run(() => 
        {
            for (long i = 0; ; i++){
                token.ThrowIfCancellationRequested();
                if (i % 50000000 == 0) Console.WriteLine("Doing work");
            }
        });

        try
        {
            work.Wait();
        }
        catch (AggregateException e) when (e.InnerException is OperationCanceledException)
        {
            Console.WriteLine("Work was cancelled cooperatively");
        }
    }

    ExceptionsThreadDemo();
    static void ExceptionsThreadDemo()
    {
        var t = Task.Run(() => throw new InvalidOperationException("oops your task messed up"));
        try
        {
            t.Wait();
        }
        catch (AggregateException e)
        {
            Console.WriteLine($"Caught: {e.InnerException!.Message}");
        }
    }
    await AsyncDemo();
    static async Task AsyncDemo()
    {
        Console.WriteLine($"Before await on Thread #{Environment.CurrentManagedThreadId}");
        await Task.Delay(50); // Non blocking wait, thread is freed
        Console.WriteLine($"After await on Thread #{Environment.CurrentManagedThreadId}");
    }
}
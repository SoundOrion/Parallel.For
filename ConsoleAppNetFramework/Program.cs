using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


internal class Program
{
    static void Main(string[] args)
    {
        int total = 1000;

        Stopwatch sw = new Stopwatch();

        // 計測開始
        sw.Start();

        for (int i = 0; i < total; i++)
        {
            //何らかの処理
            Thread.Sleep(1);

            Console.WriteLine("進捗" + (i + 1) + "/" + total);
        }

        // 計測終了
        sw.Stop();

        // 結果表示
        Console.WriteLine($"処理時間: {sw.ElapsedMilliseconds / 1000} 秒");



        int progress = 0;

        // 計測開始
        sw.Restart();

        //var options = new ParallelOptions
        //{
        //    //MaxDegreeOfParallelism = 4 // 最大並列数を4に制限
        //};

        int logicalProcessors = Environment.ProcessorCount;
        // 例えばCPUコア数の75%くらいを使うようにする
        int maxDegree = (int)(logicalProcessors * 0.75);

        var options = new ParallelOptions
        {
            MaxDegreeOfParallelism = maxDegree
        };

        Parallel.For(0, total, options, i =>
        {
            // 何らかの処理
            DoSomething(i);

            // 進捗をスレッドセーフに加算
            int current = Interlocked.Increment(ref progress);
            Console.WriteLine("進捗 " + current + "/" + total);
        });

        // 計測終了
        sw.Stop();

        // 結果表示
        Console.WriteLine($"処理時間: {sw.ElapsedMilliseconds / 1000} 秒");
    }

    static void DoSomething(int i)
    {
        // 処理の例（ちょっと待つだけ）
        Thread.Sleep(1);
    }
}


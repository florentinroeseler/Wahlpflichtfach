using System;
using System.Text;
using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Reports;

[MemoryDiagnoser]
public class StringConcatBenchmark
{
    [Params(100, 1000, 10000)]
    public int NumberOfItems;

    private List<string> data;

    [GlobalSetup]
    public void Setup()
    {
        data = new List<string>(NumberOfItems);
        var random = new Random();
        for (int i = 0; i < NumberOfItems; i++)
        {
            // Hätte vermutlich kürzere Strings nehmen sollen - hat lange gedauert
            int length = random.Next(5, 21);
            data.Add(new string('x', length));
        }
    }

    [Benchmark]
    public string ConcatUsingStringBuilder()
    {
        var sb = new StringBuilder();
        foreach (var s in data)
        {
            sb.Append(s);
        }
        return sb.ToString();
    }

    [Benchmark]
    public string ConcatUsingPlusOperator()
    {
        string result = string.Empty;
        foreach (var s in data)
        {
            result += s;
        }
        return result;
    }
}

public class Uebung4
{
    public static void Main(string[] args)
    {
        // Benchmark durchführen + Summary
        var summary = BenchmarkRunner.Run<StringConcatBenchmark>();

        // Ergebnisse in Textdatei
        ExportSummaryToTextFile(summary, "BenchmarkResults.txt");
    }

    private static void ExportSummaryToTextFile(Summary summary, string filePath)
    {
        // Erzeugt eine Tabelle mit den wichtigsten Werten: Method, NumberOfItems, Mean
        using var writer = new StreamWriter(filePath);

        writer.WriteLine("Method\t\tNumberOfItems\t\tMean (ns)");
        foreach (var report in summary.Reports)
        {
            string methodName = report.BenchmarkCase.Descriptor.WorkloadMethod.Name;
            object numberOfItems = report.BenchmarkCase.Parameters["NumberOfItems"];
            var meanNs = report.ResultStatistics?.Mean ?? 0.0;

            writer.WriteLine($"{methodName}\t{numberOfItems}\t{meanNs:F2}");
        }
    }
}

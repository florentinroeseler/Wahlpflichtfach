using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace ProducerConsumerDemo
{
    public class DataHandler
    {
        private readonly ConcurrentQueue<int> _queue = new();
        private readonly SemaphoreSlim _semaphore = new(initialCount: 2, maxCount: 2);
        private readonly int _capacity;
        private readonly int _totalItems;       
        private volatile bool _producingCompleted;

        public DataHandler(int capacity, int totalItems)
        {
            _capacity = capacity;
            _totalItems = totalItems;
        }

        public async Task ProduceDataAsync()
        {
            await Task.Run(() =>
            {
                Parallel.For(0, _totalItems, i =>
                {
                    while (_queue.Count >= _capacity)
                        Thread.Sleep(10);

                    _queue.Enqueue(i);
                    Console.WriteLine($"[Producer] Zahl {i} erzeugt.");
                    Thread.Sleep(250);   
                });

                _producingCompleted = true;
                Console.WriteLine("[Producer] Fertig – keine weiteren Daten.");
            });
        }

        public async Task ConsumeDataAsync()
        {
            var active = new List<Task>();

            while (!_producingCompleted || !_queue.IsEmpty)
            {
                if (_queue.TryDequeue(out int value))
                {
                    await _semaphore.WaitAsync(); // maximal 2 Tasks gleichzeitig

                    var t = Task.Run(async () =>
                    {
                        try
                        {
                            Console.WriteLine(
                                $"    [Consumer {Thread.CurrentThread.ManagedThreadId}] verarbeite {value}");
                            await Task.Delay(150);
                        }
                        finally
                        {
                            _semaphore.Release();
                        }
                    });

                    active.Add(t);
                    active.RemoveAll(x => x.IsCompleted);
                }
                else
                {
                    await Task.Delay(25);
                }
            }

            await Task.WhenAll(active);
            Console.WriteLine("[Consumer] Alle Daten abgearbeitet.");
        }
    }

    internal class Program
    {
        private const int NUMBERS_TO_PRODUCE = 100;
        private const int QUEUE_CAPACITY = 100; // Puffergröße

        private static async Task Main()
        {
            var handler = new DataHandler(QUEUE_CAPACITY, NUMBERS_TO_PRODUCE);

            Task consumer = handler.ConsumeDataAsync();
            Task producer = handler.ProduceDataAsync();

            await Task.WhenAll(producer, consumer);
            Console.WriteLine("== Programm beendet ==");
        }
    }
}

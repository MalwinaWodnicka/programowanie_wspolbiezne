using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Data;
using System.Threading.Tasks;
using System.Threading;

namespace Dane
{
    public class Logger : IDisposable
    {
        private readonly ConcurrentQueue<string> _logQueue = new ConcurrentQueue<string>();
        private readonly string _logDirectory;
        private readonly string _logFilePrefix = "ball_log_";
        private readonly TimeSpan _flushInterval = TimeSpan.FromSeconds(5);
        private readonly int _maxRetryAttempts = 3;
        private readonly TimeSpan _retryDelay = TimeSpan.FromMilliseconds(100);

        private StreamWriter _writer;
        private Task _flushTask;
        private CancellationTokenSource _cts;
        private readonly object _fileLock = new object();
        private bool _disposed = false;

        public Logger(string logDirectory = "Logs")
        {
            _logDirectory = logDirectory ?? throw new ArgumentNullException(nameof(logDirectory));
            Directory.CreateDirectory(_logDirectory);

            CreateNewLogFile();
            StartFlushTask();
        }

        private void CreateNewLogFile()
        {
            lock (_fileLock)
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmssfff");
                string uniqueId = Guid.NewGuid().ToString("N").Substring(0, 6);
                string logFilePath = Path.Combine(_logDirectory, $"{_logFilePrefix}{timestamp}_{uniqueId}.txt");
                _writer?.Dispose();
                _writer = new StreamWriter(logFilePath, true, Encoding.ASCII)
                {
                    AutoFlush = false
                };
            }
        }

        private void StartFlushTask()
        {
            _cts = new CancellationTokenSource();
            _flushTask = Task.Run(async () =>
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    try
                    {
                        await Task.Delay(_flushInterval, _cts.Token);
                        await FlushQueueToFileAsync();
                    }
                    catch (TaskCanceledException)
                    {
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Błąd podczas zapisywania logów: {ex.Message}");
                    }
                }
            }, _cts.Token);
        }

        public void LogCollision(Kula ball1, Kula ball2)
        {
            if (ball1 == null || ball2 == null) return;

            var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
            string logEntry = $"{timestamp}|Kolizja pomiedzy kula o promieniu R:{Math.Round(ball1.Promien, 2)} i masie M:{Math.Round(ball1.Masa, 2)} i kula o promieniu R:{Math.Round(ball2.Promien, 2)} i masie M:{Math.Round(ball2.Masa, 2)}";
            _logQueue.Enqueue(logEntry);
        }

        private async Task FlushQueueToFileAsync()
        {
            int attempts = 0;
            bool success = false;

            while (attempts < _maxRetryAttempts && !success)
            {
                try
                {
                    lock (_fileLock)
                    {
                        while (_logQueue.TryDequeue(out string logEntry))
                        {
                            _writer.WriteLine(logEntry);
                        }
                        _writer.Flush();
                    }
                    success = true;
                }
                catch (IOException ex) when (attempts < _maxRetryAttempts - 1)
                {
                    attempts++;
                    Console.WriteLine($"Błąd zapisu (próba {attempts}): {ex.Message}");
                    await Task.Delay(_retryDelay);
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Krytyczny błąd zapisu: {ex.Message}");
                    break;
                }
            }
        }

        public void Flush()
        {
            try
            {
                FlushQueueToFileAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas wymuszonego zapisu: {ex.Message}");
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _disposed = true;
            _cts?.Cancel();

            try
            {
                _flushTask?.Wait();
                FlushQueueToFileAsync().GetAwaiter().GetResult();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zamykania loggera: {ex.Message}");
            }
            finally
            {
                _writer?.Dispose();
                _cts?.Dispose();
                GC.SuppressFinalize(this);
            }
        }
    }
}
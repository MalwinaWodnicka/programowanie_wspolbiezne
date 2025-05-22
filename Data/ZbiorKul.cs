using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using Dane;

namespace Data
{
    public class Kula
    {
        private double _x;
        private double _y;
        private double _predkoscX;
        private double _predkoscY;
        private readonly double _promien;
        private readonly double _masa;
        private readonly object _lock = new object();

        public double X
        {
            get { lock (_lock) { return _x; } }
            set { lock (_lock) { _x = value; } }
        }

        public double Y
        {
            get { lock (_lock) { return _y; } }
            set { lock (_lock) { _y = value; } }
        }

        public double PredkoscX
        {
            get { lock (_lock) { return _predkoscX; } }
            set { lock (_lock) { _predkoscX = value; } }
        }

        public double PredkoscY
        {
            get { lock (_lock) { return _predkoscY; } }
            set { lock (_lock) { _predkoscY = value; } }
        }

        public double Promien => _promien;
        public double Masa => _masa;

        public Kula(double x, double y, double predkoscX, double predkoscY, double promien)
        {
            _x = x;
            _y = y;
            _predkoscX = predkoscX;
            _predkoscY = predkoscY;
            _promien = promien;
            _masa = promien * promien; 
        }
    }

    public class ZbiorKul : IZbiorKul
    {
        private readonly List<Kula> _kule = new List<Kula>();
        private readonly object _lock = new object();

        public IEnumerable<Kula> GetKule()
        {
            lock (_lock)
            {
                return new List<Kula>(_kule);
            }
        }

        public void AddKula(Kula kula)
        {
            if (kula != null)
            {
                lock (_lock)
                {
                    _kule.Add(kula);
                }
            }
        }

        public void ClearKule()
        {
            lock (_lock)
            {
                _kule.Clear();
            }
        }
    }

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
                string logFilePath = Path.Combine(_logDirectory, $"{_logFilePrefix}{timestamp}.txt");

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
                        // Normalne zakończenie
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
                FlushQueueToFileAsync().Wait();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Błąd podczas zamykania loggera: {ex.Message}");
            }
            finally
            {
                _writer?.Dispose();
                _cts?.Dispose();
            }
        }
    }
}

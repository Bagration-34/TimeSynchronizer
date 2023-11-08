using System;
using System.Diagnostics;
using System.Net.NetworkInformation;
using System.Threading;

namespace TimeSynchronizer
{
    internal class Program
    {

        private static string[] hosts = { "google.com", "ya.ru" };

        static void Main(string[] args)
        {

            // Итератор
            int i = 0;

            while (true)
            {
                // Если попытка соединения с хостом неудачная, то пробуем соединиться с другим хостом из массива
                if (CheckForInternetConnection(hosts[(i++)%hosts.Length]))
                {
                    break;
                }
                Thread.Sleep(3000); // Задержка между попытками соединения
            }
            StartW32Time();
            Thread.Sleep(1000); // Даём время службе запуститься
            ResyncForce();

        }

        // Запускаем службу
        private static void StartW32Time()
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = @"C:\Windows\System32\net.exe";    // путь к приложению, которое будем запускать
                process.StartInfo.Arguments = "start w32time";                  // аргументы (параметры)
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;      // скрыть окно
                process.Start();
            }
            finally
            {
                // Всегда вызывать Dispose() возникла ошибка или нет
                if (process is IDisposable)
                    process.Dispose();
            }
        }

        // Синхронизируем время
        private static void ResyncForce()
        {
            Process process = new Process();
            try
            {
                process.StartInfo.FileName = @"C:\Windows\System32\w32tm.exe";   // путь к приложению, которое будем запускать
                process.StartInfo.Arguments = "/resync /force";                  // аргументы  (параметры)
                process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;       // скрыть окно
                process.Start();
            }
            finally
            {
                // Всегда вызывать Dispose() возникла ошибка или нет
                if (process is IDisposable)
                    process.Dispose();
            }
        }

        // Провереяем интернет соединение
        private static bool CheckForInternetConnection(string host)
        {
            var ping = new Ping();
            byte[] buffer = new byte[32];
            int timeout = 1000;
            var options = new PingOptions();
            try
            {
                var reply = ping.Send(host, timeout, buffer, options);
                return reply.Status == IPStatus.Success;
            }
            catch (PingException)
            {
                return false;
            }
        }
    }
}

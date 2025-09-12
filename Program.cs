using System;
using System.Linq;
using System.Threading.Tasks;
using System.Diagnostics;

class Program
{
    static void Main(string[] args)
    {
        //Входные данные
        long points = ParseArg(args, "--points", 100_000_000L);
        int workers = ParseArg(args, "--workers", Environment.ProcessorCount);

        Stopwatch sw = Stopwatch.StartNew();

        double pi = ComputePiMonteCarlo(points, workers);

        sw.Stop();

        // Вывод результатов
        Console.WriteLine($"Оценка π: {pi:F6}");
        Console.WriteLine($"Время выполнения: {sw.ElapsedMilliseconds} мс");
        Console.WriteLine($"Точек: {points:N0}");
        Console.WriteLine($"Воркеров: {workers}");
        Console.WriteLine($"Физических ядер: {Environment.ProcessorCount}");

        static T ParseArg<T>(string[] args, string key, T defaultValue)
        {
            for (int i = 0; i < args.Length - 1; i++)
            {
                if (args[i] == key)
                {
                    try
                    {
                        return (T)Convert.ChangeType(args[i + 1], typeof(T));
                    }
                    catch (Exception)
                    {
                        Console.WriteLine($"Ошибка: Неверный формат значения для {key}: '{args[i + 1]}'. Use default value.");
                        return defaultValue;
                    }
                }
            }
            return defaultValue;
        }


        static double ComputePiMonteCarlo(long totalPoints, int numWorkers)
        {
            long pointsPerWorker = totalPoints / numWorkers;
            long remainder = totalPoints % numWorkers;

            long totalInside = 0;

            Parallel.For(0, numWorkers, i =>
            {
                long localPoints = pointsPerWorker + (i < remainder ? 1 : 0);
                long inside = CountPointsInsideCircle(localPoints);

                Interlocked.Add(ref totalInside, inside);
            });

            return 4.0 * totalInside / totalPoints;
        }

        static long CountPointsInsideCircle(long points)
        {
            Random rng = new();
            long count = 0;

            for (long i = 0; i < points; i++)
            {
                double x = rng.NextDouble();
                double y = rng.NextDouble();

                if (x * x + y * y <= 1.0)
                    count++;
            }

            return count;
        }
    }
}
using System.Globalization;

namespace RestaurantOrders.Ui;

public static class ConsoleIO
{
    public static void Header(string title)
    {
        Console.WriteLine();
        Console.WriteLine(new string('=', 60));
        Console.WriteLine($"  {title}");
        Console.WriteLine(new string('=', 60));
    }

    public static string ReadString(string prompt, bool allowEmpty = false)
    {
        while (true)
        {
            Console.Write(prompt);
            var s = (Console.ReadLine() ?? string.Empty).Trim();
            if (s.Length > 0 || allowEmpty) return s;
            Console.WriteLine("  Значення не може бути порожнім.");
        }
    }

    public static int ReadInt(string prompt, int min = int.MinValue, int max = int.MaxValue)
    {
        while (true)
        {
            var s = ReadString(prompt);
            if (int.TryParse(s, NumberStyles.Integer, CultureInfo.InvariantCulture, out var v) && v >= min && v <= max)
                return v;
            Console.WriteLine($"  Введіть ціле число від {(min == int.MinValue ? "-∞" : min)} до {(max == int.MaxValue ? "+∞" : max)}.");
        }
    }

    public static decimal ReadDecimal(string prompt, decimal min = 0)
    {
        while (true)
        {
            var s = ReadString(prompt).Replace(',', '.');
            if (decimal.TryParse(s, NumberStyles.Number, CultureInfo.InvariantCulture, out var v) && v >= min)
                return v;
            Console.WriteLine($"  Введіть число не менше {min}.");
        }
    }

    public static double ReadDouble(string prompt, double min = 0)
    {
        while (true)
        {
            var s = ReadString(prompt).Replace(',', '.');
            if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var v) && v >= min)
                return v;
            Console.WriteLine($"  Введіть число не менше {min}.");
        }
    }

    public static bool Confirm(string prompt)
    {
        var s = ReadString($"{prompt} (т/н): ");
        return s.Equals("т", StringComparison.OrdinalIgnoreCase)
            || s.Equals("y", StringComparison.OrdinalIgnoreCase)
            || s.Equals("так", StringComparison.OrdinalIgnoreCase);
    }

    public static void Error(Exception ex)
    {
        Console.WriteLine($"  [Помилка] {ex.InnerException?.Message ?? ex.Message}");
    }

    public static void Pause()
    {
        Console.WriteLine();
        Console.Write("Натисніть Enter, щоб продовжити...");
        Console.ReadLine();
    }

    public static string Money(decimal value) => value.ToString("F2", CultureInfo.InvariantCulture);
}

namespace Backup.Services;

public static class ConsoleService
{
    public static int ReadInt(string msg)
    {
        try
        {
            while (true)
            {
                Console.Write(msg);
                if (int.TryParse(Console.ReadLine()?.Trim(), out var integer))
                {
                    return integer;
                }
            }
        }
        catch
        {
            return -1;
        }
    }


    public static string ReadString(string msg)
    {
        try
        {
            while (true)
            {
                Console.Write(msg);
                var str = Console.ReadLine()?.Trim();
                if (!string.IsNullOrWhiteSpace(str))
                {
                    return str;
                }
            }
        }
        catch (Exception)
        {
            return "";
        }
    }
}
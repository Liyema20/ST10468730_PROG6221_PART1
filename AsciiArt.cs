using System;

public static class AsciiArt
{
    public static void DisplayLogo()
    {
        Console.ForegroundColor = ConsoleColor.Cyan;

        Console.WriteLine(@"
   ███╗   ███╗███╗   ███╗██╗  ██╗███╗   ███╗
   ████╗ ████║████╗ ████║╚██╗██╔╝████╗ ████║
   ██╔████╔██║██╔████╔██║ ╚███╔╝ ██╔████╔██║
   ██║╚██╔╝██║██║╚██╔╝██║ ██╔██╗ ██║╚██╔╝██║
   ██║ ╚═╝ ██║██║ ╚═╝ ██║██╔╝ ██╗██║ ╚═╝ ██║
   ╚═╝     ╚═╝╚═╝     ╚═╝╚═╝  ╚═╝╚═╝     ╚═╝
");

        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("        🔐 CYBERSECURITY AWARENESS BOT 🔐");

        Console.ForegroundColor = ConsoleColor.DarkGray;
        Console.WriteLine("        Stay Smart. Stay Safe. Stay Secure.\n");

        Console.ResetColor();
    }
}
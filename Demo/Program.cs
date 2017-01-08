using System;

namespace GemGuiTest
{
    public static class Program
    {
        [STAThread]
        static void Main()
        {
            using (var game = new GemGuiTest.Main())
                game.Run();
        }
    }
}

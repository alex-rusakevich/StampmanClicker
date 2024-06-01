using Microsoft.Xna.Framework;
using System.IO;
using System;

namespace StampmanClicker
{
    public static class StampmanClickerGame
    {
        public static readonly Game instance = new StampmanClicker.MainGame();
        public readonly static string gameFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "My Games", "Alerus", "Stampman");

        static void Main()
        {
            if (!Directory.Exists(gameFolder)) Directory.CreateDirectory(gameFolder);
            instance.Run();
        }
    }
}

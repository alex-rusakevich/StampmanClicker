using Microsoft.Xna.Framework;

namespace StampmanClicker
{
    public static class StampmanClickerGame
    {
        public static readonly Game instance = new StampmanClicker.MainGame();

        static void Main()
        {
            instance.Run();
        }
    }
}

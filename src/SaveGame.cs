using System.IO;
using System.IO.Compression;
using System.Text;
using System.Text.Json;

namespace StampmanClicker
{
    internal static class SaveGame
    {
        readonly static string gameSavePath = Path.Combine(StampmanClickerGame.gameFolder, "Game.sav");

        static readonly SaveGameData defaultSaveData = new();

        public static void Save(SaveGameData saveGameData)
        {
            using GZipStream gzipStreamWriter = new(File.Open(gameSavePath, FileMode.OpenOrCreate), CompressionMode.Compress);
            gzipStreamWriter.Write(
                Encoding.UTF8.GetBytes(JsonSerializer.Serialize(saveGameData)));
        }

        /// <summary>
        /// Load data from save file
        /// </summary>
        /// <returns>The first number is current level, the second is money, the third is total books number</returns>
        /// <exception cref="System.Exception">Wrong savefile format magic string</exception>
        public static SaveGameData Load()
        {
            if (!File.Exists(gameSavePath)) return defaultSaveData;

            using GZipStream gzipStream = new(File.Open(gameSavePath, FileMode.Open), CompressionMode.Decompress);
            using StreamReader fileReader = new(gzipStream);

            var saveGame = JsonSerializer.Deserialize<SaveGameData>(fileReader.ReadToEnd());

            return saveGame;
        }
    }
}

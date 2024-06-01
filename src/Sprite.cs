using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace StampmanClicker
{
    internal class Sprite 
    {
        public Vector2 position;
        public readonly Texture2D texture;

        public Rectangle Rectangle
        {
            get {
                return new Rectangle((int)position.X, (int)position.Y, texture.Width, texture.Height); 
            }
        }

        public Sprite(Texture2D texture, Vector2 position)
        {
            this.texture = texture;
            this.position = position;
        }

        public void Draw(SpriteBatch sp)
        {
            sp.Draw(texture, position, null, Color.White);
        }
    }
}

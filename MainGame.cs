using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;

namespace StampmanClicker
{
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private Texture2D bookTexture;
        private Texture2D stampTexture;
        private Texture2D stampMakerTexture;
        private Texture2D tableTexture;

        MouseState ms, oldms;
        KeyboardState ks, oldks;

        private int score;

        private DateTime lastClick;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 400;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

            score = 0;

            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _font = Content.Load<SpriteFont>(@"Font\Press Start 2P");

            bookTexture = Content.Load<Texture2D>(@"Image\book");
            stampTexture = Content.Load<Texture2D>(@"Image\stamp");
            stampMakerTexture = Content.Load<Texture2D>(@"Image\stampMaker");
            tableTexture = Content.Load<Texture2D>(@"Image\table");
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ms = Mouse.GetState();
            ks = Keyboard.GetState();

            if ((ms.LeftButton == ButtonState.Pressed && oldms.LeftButton != ButtonState.Pressed)
                || (ks.IsKeyDown(Keys.Space) && !oldks.IsKeyDown(Keys.Space)))
            {
                if(lastClick == default || DateTime.Now.Subtract(lastClick).TotalMilliseconds >= 100)
                {
                    lastClick = DateTime.Now;
                    score++;
                }
            }

            oldms = ms;
            oldks = ks;

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            #region Draw textures
            _spriteBatch.Draw(tableTexture, new Vector2(
                (Window.ClientBounds.Width/2) - (tableTexture.Width / 2),
                Window.ClientBounds.Height - tableTexture.Height
                ), Color.White);

            _spriteBatch.Draw(bookTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (bookTexture.Width * 0.7f / 2),
                (Window.ClientBounds.Height / 2) - (bookTexture.Height * 0.7f / 2) + 60
                ), null, Color.White, 0, Vector2.Zero, new Vector2(0.7f, 0.7f), SpriteEffects.None, 0);

            _spriteBatch.Draw(stampTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (stampTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (stampTexture.Height / 2)
                ), Color.White);

            _spriteBatch.Draw(stampMakerTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (stampMakerTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (stampMakerTexture.Height / 2)
                ), Color.White);
            #endregion

            string scoreRepr = $"{score}K$";
            var scoreRect = _font.MeasureString(scoreRepr);

            _spriteBatch.DrawString(_font, scoreRepr, new Vector2(
                (Window.ClientBounds.Width / 2) - scoreRect.X / 2,
                20
                ), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

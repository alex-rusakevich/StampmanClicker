using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace StampmanClicker
{
    public class MainGame : Game
    {
        private readonly GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        private SpriteFont _font;

        private Texture2D bookTexture;
        private Texture2D stampMakerTexture;
        private Texture2D tableTexture;
        private Texture2D stampTexture;

        private MouseState ms, oldms;
        private KeyboardState ks, oldks;

        private uint money;
        private uint moneyDelta;
        private uint level;
        private uint booksRecommended;

        private bool isBeingPressed = false;

        private DateTime lastClick;

        private bool isRedrawPending = true;

        public MainGame()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            _graphics.IsFullScreen = false;
            _graphics.PreferredBackBufferWidth = 800;
            _graphics.PreferredBackBufferHeight = 600;
            _graphics.ApplyChanges();

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

            var saveGameData = SaveGame.Load();
            money = saveGameData.Money;
            moneyDelta = saveGameData.MoneyDelta;
            level = saveGameData.Level;
            booksRecommended = saveGameData.BooksRecommended;
        }

        protected override void UnloadContent()
        {
            SaveGame.Save(new SaveGameData() { 
                Money = money,
                MoneyDelta = moneyDelta,
                Level = level,
                BooksRecommended = booksRecommended,
            });
            base.UnloadContent();
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
                    isBeingPressed = true;
                    lastClick = DateTime.Now;

                    money += moneyDelta;
                    booksRecommended += 1;

                    isRedrawPending = true;
                }
            }

            if ((ms.LeftButton != ButtonState.Pressed && oldms.LeftButton == ButtonState.Pressed)
                || (!ks.IsKeyDown(Keys.Space) && oldks.IsKeyDown(Keys.Space)))
            {
                isBeingPressed = false;
                isRedrawPending = true;
            }

            oldms = ms;
            oldks = ks;

            if (!isRedrawPending)
                SuppressDraw();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin();

            #region Draw textures
            double tableScale = Math.Round((float) Window.ClientBounds.Width / tableTexture.Width, 1);

            _spriteBatch.Draw(tableTexture, new Vector2(
                0,
                -40
                ), null, Color.White, 0, Vector2.Zero, new Vector2((float) tableScale, (float) tableScale), SpriteEffects.None, 0);

            // Center book
            _spriteBatch.Draw(bookTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (bookTexture.Width * 0.7f / 2),
                (Window.ClientBounds.Height / 2) - (bookTexture.Height * 0.7f / 2) + 60
                ), null, Color.White, 0, Vector2.Zero, new Vector2(0.7f, 0.7f), SpriteEffects.None, 0);

            // Left book
            _spriteBatch.Draw(bookTexture, new Vector2(
                0 - (bookTexture.Width * 0.7f / 2),
                (Window.ClientBounds.Height / 2) - (bookTexture.Height * 0.7f / 2) + 60
                ), null, Color.White, 0, Vector2.Zero, new Vector2(0.7f, 0.7f), SpriteEffects.None, 0);

            // Right book
            if(booksRecommended > 0)
            { 
                _spriteBatch.Draw(bookTexture, new Vector2(
                    Window.ClientBounds.Width - (bookTexture.Width * 0.7f / 2),
                    (Window.ClientBounds.Height / 2) - (bookTexture.Height * 0.7f / 2) + 60
                    ), null, Color.White, 0, Vector2.Zero, new Vector2(0.7f, 0.7f), SpriteEffects.None, 0);

                _spriteBatch.Draw(stampTexture, new Vector2(
                    Window.ClientBounds.Width - (stampTexture.Width * 0.3f / 2),
                    (Window.ClientBounds.Height / 2) - (stampTexture.Height * 0.3f / 2) + 60
                    ), null, Color.White, 0, Vector2.Zero, new Vector2(0.3f, 0.3f), SpriteEffects.None, 0);
            }

            int stampMakerPixelDelta = 0;
            if (isBeingPressed) stampMakerPixelDelta = 20;

            _spriteBatch.Draw(stampMakerTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (stampMakerTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (stampMakerTexture.Height / 2) - 20 + stampMakerPixelDelta
                ), Color.White);
            #endregion

            string moneyRepr = $"{money}$";
            string booksRecommendedRepr = $"Рек-но: {booksRecommended} кн.";
            string moneyDeltaRepr = $"{moneyDelta}$ за рек.";

            _spriteBatch.DrawString(_font, moneyRepr, new Vector2(20, 20), Color.White);
            _spriteBatch.DrawString(_font, booksRecommendedRepr, new Vector2(20, 40), Color.White);
            _spriteBatch.DrawString(_font, moneyDeltaRepr, new Vector2(20, 60), Color.White);

            _spriteBatch.End();

            base.Draw(gameTime);

            isRedrawPending = false;
        }
    }
}

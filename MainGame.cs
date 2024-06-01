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

        private Sprite bookCenterSprite;
        private Sprite bookLeftSprite;
        private Sprite bookRightSprite;
        private Sprite tableSprite;
        private Sprite stampSprite;
        private Sprite stampMakerSprite;

        private MouseState ms, oldms;
        private KeyboardState ks, oldks;

        private uint money;
        private uint moneyDelta;
        private uint level;
        private uint booksRecommended;

        private DateTime lastClick;
        private bool isStampMakerPressed = false;
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

            #region Create sprites
            tableSprite = new Sprite(tableTexture, new Vector2(
                (Window.ClientBounds.Width / 2 - tableTexture.Width / 2),
                (Window.ClientBounds.Height - tableTexture.Height)
                ));
            stampMakerSprite = new Sprite(stampMakerTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (stampMakerTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (stampMakerTexture.Height / 2) - 40));
            bookCenterSprite = new Sprite(bookTexture, new Vector2(
                (Window.ClientBounds.Width / 2) - (bookTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (bookTexture.Height / 2) + 60
                ));

            bookLeftSprite = new Sprite(bookTexture, new Vector2(
                0 - (bookTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (bookTexture.Height / 2) + 60
                ));
            bookRightSprite = new Sprite(bookTexture, new Vector2(
                Window.ClientBounds.Width - (bookTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (bookTexture.Height / 2) + 60
                ));
            stampSprite = new Sprite(stampTexture, new Vector2(
                Window.ClientBounds.Width - (stampTexture.Width / 2),
                (Window.ClientBounds.Height / 2) - (stampTexture.Height / 2) + 60
                ));
            #endregion

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

        private void StampClicked()
        {
            if (lastClick == default || DateTime.Now.Subtract(lastClick).TotalMilliseconds >= 100)
            {
                stampMakerSprite.position.Y += 20;

                lastClick = DateTime.Now;

                money += moneyDelta;
                booksRecommended += 1;

                isStampMakerPressed = true;
                isRedrawPending = true;
            }
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            ms = Mouse.GetState();
            Rectangle mouseRect = new((int)ms.X, (int)ms.Y, 1, 1);
            ks = Keyboard.GetState();

            if (ms.LeftButton == ButtonState.Pressed && oldms.LeftButton != ButtonState.Pressed)
            {
                if (stampMakerSprite.Rectangle.Intersects(mouseRect)
                    || bookCenterSprite.Rectangle.Intersects(mouseRect))
                {
                    StampClicked();
                }
            }
            else if (ks.IsKeyDown(Keys.Space) && !oldks.IsKeyDown(Keys.Space))
            {
                StampClicked();
            }

            if (((ms.LeftButton != ButtonState.Pressed && oldms.LeftButton == ButtonState.Pressed)
                || (!ks.IsKeyDown(Keys.Space) && oldks.IsKeyDown(Keys.Space))) && isStampMakerPressed)
            {
                stampMakerSprite.position.Y -= 20;
                isRedrawPending = true;
                isStampMakerPressed = false;
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
            // double tableScale = Math.Round((float) Window.ClientBounds.Width / tableTexture.Width, 1);

            tableSprite.Draw(_spriteBatch);
            bookCenterSprite.Draw(_spriteBatch);
            stampMakerSprite.Draw(_spriteBatch);

            bookLeftSprite.Draw(_spriteBatch);

            if(booksRecommended > 0) {
                bookRightSprite.Draw(_spriteBatch);
                stampSprite.Draw(_spriteBatch);
            }
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

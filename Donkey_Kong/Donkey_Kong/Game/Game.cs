using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Donkey_Kong
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game : Microsoft.Xna.Framework.Game
    {
        enum GameState
        {
            isOnMenu,
            isPlaying,
            isPaused,
            isDead,
            isWon
        }

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        GameState myGameState;
        Player myPlayer;
        Level myLevel;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1120;
            graphics.PreferredBackBufferHeight = 740;
            graphics.ApplyChanges();

            ResourceManager.Initialize();
            myGameState = GameState.isPlaying;

            myPlayer = new Player(new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height - 96), new Point(64, 48), 130.0f, 4.1f, -160.0f);
            myLevel = new Level(@"../../../../Levels/Level01.txt");

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.AddTexture("Mario_Walking", this.Content.Load<Texture2D>("Sprites/Mario_Walking"));
            ResourceManager.AddTexture("Mario_Jumping", this.Content.Load<Texture2D>("Sprites/Mario_Jumping"));
            ResourceManager.AddTexture("Bridge", this.Content.Load<Texture2D>("Sprites/bridge"));
            ResourceManager.AddTexture("Ladder", this.Content.Load<Texture2D>("Sprites/ladder"));
            ResourceManager.AddTexture("BridgeLadder", this.Content.Load<Texture2D>("Sprites/bridgeLadder"));
            ResourceManager.AddTexture("Empty", this.Content.Load<Texture2D>("Sprites/empty"));
            ResourceManager.AddTexture("Pole", this.Content.Load<Texture2D>("Sprites/pole"));
            ResourceManager.AddFont("8-bit_Font", this.Content.Load<SpriteFont>("Fonts/8-bit"));

            myPlayer.SetTexture("Mario_Walking");
            myLevel.SetTileTexture();
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            switch (myGameState)
            {
                case GameState.isOnMenu:

                    break;
                case GameState.isPlaying:
                    myPlayer.Update(gameTime, myLevel.Tiles);
                    myLevel.Update();
                    break;
                case GameState.isPaused:

                    break;
                case GameState.isDead:

                    break;
                case GameState.isWon:

                    break;
            }

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();

            switch (myGameState)
            {
                case GameState.isOnMenu:

                    break;
                case GameState.isPlaying:
                    myLevel.Draw(spriteBatch);
                    myPlayer.Draw(spriteBatch, gameTime);
                    break;
                case GameState.isPaused:

                    break;
                case GameState.isDead:

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}

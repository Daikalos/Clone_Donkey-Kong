using System;
using System.IO;
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
        Random myRNG;

        private Texture2D 
            myMenuTexture,
            myDKTexture;
        private Animation myMenuAnimation;

        private Player myPlayer;
        private Level myLevel;
        private EnemyManager myEnemyManager;

        private GameState myGameState;
        private SpriteFont my8BitFont;

        private int 
            myLives,
            myScore,
            myHighScore;

        public Game()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize()
        {
            graphics.PreferredBackBufferWidth = 1120;
            graphics.PreferredBackBufferHeight = 700;
            graphics.ApplyChanges();

            myRNG = new Random();

            ResourceManager.Initialize();
            myGameState = GameState.isOnMenu;

            myPlayer = new Player(new Vector2(Window.ClientBounds.Width / 6, Window.ClientBounds.Height - 60), new Point(40), 170.0f, 120.0f, 15.5f, -320.0f);
            myLevel = new Level(@"../../../../Levels/Level01.txt");
            myEnemyManager = new EnemyManager(5.0f, 5);
            myMenuAnimation = new Animation();

            myLives = 3;
            myScore = 0;
            myHighScore = 0;

            string tempFilePath = @"../../../../High-Score/High-Score.txt";
            if (File.Exists(tempFilePath))
            {
                if (new FileInfo(tempFilePath).Length > 0)
                {
                    string tempReadFile = File.ReadAllText(tempFilePath);
                    string[] tempSplitText = tempReadFile.Split('=');

                    for (int i = 0; i < tempSplitText.Length; i++)
                    {
                        if (tempSplitText[i] == "HighScore")
                        {
                            myHighScore = Convert.ToInt32(tempSplitText[i + 1]);
                        }
                    }
                }
            }

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.AddTexture("Mario_Walking", this.Content.Load<Texture2D>("Sprites/Mario_Walking"));
            ResourceManager.AddTexture("Mario_Jumping", this.Content.Load<Texture2D>("Sprites/Mario_Jumping"));
            ResourceManager.AddTexture("Mario_Lives", this.Content.Load<Texture2D>("Sprites/mario_lives"));
            ResourceManager.AddTexture("Bridge", this.Content.Load<Texture2D>("Sprites/bridge"));
            ResourceManager.AddTexture("Ladder", this.Content.Load<Texture2D>("Sprites/ladder"));
            ResourceManager.AddTexture("BridgeLadder", this.Content.Load<Texture2D>("Sprites/bridgeLadder"));
            ResourceManager.AddTexture("Empty", this.Content.Load<Texture2D>("Sprites/empty"));
            ResourceManager.AddTexture("Sprint", this.Content.Load<Texture2D>("Sprites/sprint"));
            ResourceManager.AddTexture("Pole", this.Content.Load<Texture2D>("Sprites/pole"));
            ResourceManager.AddTexture("Enemy", this.Content.Load<Texture2D>("Sprites/enemy"));
            ResourceManager.AddTexture("Menu", this.Content.Load<Texture2D>("Sprites/start"));
            ResourceManager.AddTexture("Donkey_Kong", this.Content.Load<Texture2D>("Sprites/donkey_kong"));

            ResourceManager.AddFont("8-bit", this.Content.Load<SpriteFont>("Fonts/8-bit"));

            myPlayer.SetTexture("Mario_Walking");
            myLevel.SetTileTexture();

            myMenuTexture = ResourceManager.RequestTexture("Menu");
            myDKTexture = ResourceManager.RequestTexture("Donkey_Kong");

            my8BitFont = ResourceManager.RequestFont("8-bit");
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
                    if (KeyMouseReader.KeyPressed(Keys.Enter))
                    {
                        myGameState = GameState.isPlaying;
                    }
                    break;
                case GameState.isPlaying:
                    myPlayer.Update(Window, gameTime, myLevel);
                    myEnemyManager.Update(Window, gameTime, myRNG, myLevel, myPlayer);
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
                    spriteBatch.Draw(myMenuTexture, new Rectangle(Window.ClientBounds.Width / 4, 20, (Window.ClientBounds.Width / 2), (Window.ClientBounds.Height / 2)), null, Color.White);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "Press ENTER to start", new Vector2(Window.ClientBounds.Width / 2, (Window.ClientBounds.Height / 2) + 80), Color.DarkOrange, 1.2f);
                    myMenuAnimation.DrawSpriteSheet(spriteBatch, gameTime, myDKTexture, new Vector2(
                        (Window.ClientBounds.Width / 2) - (myDKTexture.Width / 4) * 4,
                        (Window.ClientBounds.Height / 2) + (myDKTexture.Height / 2) * 9), myDKTexture.Width / 2, myDKTexture.Height, (myDKTexture.Width / 2) * 4, myDKTexture.Height * 4, 2, 1, 1.0f, SpriteEffects.None, true);
                    break;
                case GameState.isPlaying:
                    myLevel.Draw(spriteBatch);
                    myEnemyManager.Draw(spriteBatch);
                    myPlayer.Draw(spriteBatch, gameTime);

                    StringManager.DrawStringLeft(spriteBatch, ResourceManager.RequestFont("8-bit"), "SCORE", new Vector2(20, 20), Color.White, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, ResourceManager.RequestFont("8-bit"), "BONUS", new Vector2(20, 80), Color.Blue, 0.8f);
                    StringManager.DrawStringMid(spriteBatch, ResourceManager.RequestFont("8-bit"), "HIGH SCORE", new Vector2(Window.ClientBounds.Width - 130, 20), Color.Red, 1.0f);
                    StringManager.DrawStringMid(spriteBatch, ResourceManager.RequestFont("8-bit"), myHighScore.ToString(), new Vector2(Window.ClientBounds.Width - 130, 50), Color.White, 1.1f);
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

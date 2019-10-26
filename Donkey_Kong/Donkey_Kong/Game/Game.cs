using System;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Audio;

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
            myDKTexture,
            myMarioHPTexture;
        private Animation myDKIdleAnimation;

        private Player myPlayer;
        private Level myLevel;
        private EnemyManager myEnemyManager;

        private GameState myGameState;
        private SpriteFont my8BitFont;

        private int
            myBonusScore,
            myHighScore;
        private float
            myReduceBonus,
            myReduceBonusMax;
        private int[] myHighScores;

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

            GameInfo.Initialize(1.2f);
            ResourceManager.Initialize();
            myGameState = GameState.isOnMenu;

            myPlayer = new Player(new Vector2(Window.ClientBounds.Width / 6, Window.ClientBounds.Height - 60), new Point(40), 3, 170.0f, 120.0f, 15.5f, -320.0f);
            myLevel = new Level(@"../../../../Levels/Level01.txt");
            myEnemyManager = new EnemyManager(new Point(140, 180), 6.0f, 6);
            myDKIdleAnimation = new Animation();

            myBonusScore = 6000;
            myReduceBonusMax = 1.5f;
            myHighScore = 0;

            string tempFilePath = @"../../../../High-Score/High-Score.txt"; //Load HighScore
            string[] tempScores = FileReader.FindInfo(tempFilePath, "HighScore", '=');
            myHighScores = Array.ConvertAll(tempScores, s => Int32.Parse(s));
            myHighScore = myHighScores.Max();

            base.Initialize();
        }

        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);

            ResourceManager.AddFont("8-bit", this.Content.Load<SpriteFont>("Fonts/8-bit"));

            ResourceManager.AddTexture("Mario_Walking", this.Content.Load<Texture2D>("Sprites/mario_walking"));
            ResourceManager.AddTexture("Mario_Jumping", this.Content.Load<Texture2D>("Sprites/mario_jumping"));
            ResourceManager.AddTexture("Mario_Lives", this.Content.Load<Texture2D>("Sprites/mario_lives"));
            ResourceManager.AddTexture("Mario_Death", this.Content.Load<Texture2D>("Sprites/mario_death"));
            ResourceManager.AddTexture("Bridge", this.Content.Load<Texture2D>("Sprites/bridge"));
            ResourceManager.AddTexture("Ladder", this.Content.Load<Texture2D>("Sprites/ladder"));
            ResourceManager.AddTexture("BridgeLadder", this.Content.Load<Texture2D>("Sprites/bridgeLadder"));
            ResourceManager.AddTexture("BridgePole", this.Content.Load<Texture2D>("Sprites/bridgePole"));
            ResourceManager.AddTexture("Empty", this.Content.Load<Texture2D>("Sprites/empty"));
            ResourceManager.AddTexture("Sprint", this.Content.Load<Texture2D>("Sprites/sprint"));
            ResourceManager.AddTexture("Pole", this.Content.Load<Texture2D>("Sprites/pole"));
            ResourceManager.AddTexture("Items", this.Content.Load<Texture2D>("Sprites/items"));
            ResourceManager.AddTexture("Enemy", this.Content.Load<Texture2D>("Sprites/enemy"));
            ResourceManager.AddTexture("Menu", this.Content.Load<Texture2D>("Sprites/start"));
            ResourceManager.AddTexture("DK_Idle", this.Content.Load<Texture2D>("Sprites/dk_idle"));

            ResourceManager.AddSound("Walking", this.Content.Load<SoundEffect>("Sounds/walking"));
            ResourceManager.AddSound("Death", this.Content.Load<SoundEffect>("Sounds/death"));
            ResourceManager.AddSound("Item_Get", this.Content.Load<SoundEffect>("Sounds/itemget"));
            ResourceManager.AddSound("Jump", this.Content.Load<SoundEffect>("Sounds/jump"));
            ResourceManager.AddSound("BacMusic", this.Content.Load<SoundEffect>("Sounds/bacmusic"));
            ResourceManager.AddSound("Win", this.Content.Load<SoundEffect>("Sounds/win1"));

            myPlayer.SetTexture("Mario_Walking");
            myLevel.SetTileTexture();

            myMenuTexture = ResourceManager.RequestTexture("Menu");
            myDKTexture = ResourceManager.RequestTexture("DK_Idle");
            myMarioHPTexture = ResourceManager.RequestTexture("Mario_Lives");

            my8BitFont = ResourceManager.RequestFont("8-bit");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            Pausing();

            switch (myGameState)
            {
                case GameState.isOnMenu:
                    ResourceManager.PlaySound("BacMusic");
                    if (KeyMouseReader.KeyPressed(Keys.Enter))
                    {
                        myGameState = GameState.isPlaying;
                    }
                    break;
                case GameState.isPlaying:
                    ResourceManager.PlaySound("BacMusic");
                    GameInfo.Update(gameTime);

                    myReduceBonus += (float)gameTime.ElapsedGameTime.TotalSeconds;
                    if (myReduceBonus >= myReduceBonusMax)
                    {
                        myBonusScore -= 100;
                        myReduceBonus = 0;
                    }

                    myPlayer.Update(Window, gameTime, myLevel);
                    myEnemyManager.Update(Window, gameTime, myRNG, myLevel, myPlayer);
                    break;
                case GameState.isPaused:
                    ResourceManager.PlaySound("BacMusic");
                    break;
                case GameState.isDead:
                    if (KeyMouseReader.KeyPressed(Keys.Enter))
                    {
                        myGameState = GameState.isPlaying;
                    }
                    break;
                case GameState.isWon:
                    if (KeyMouseReader.KeyPressed(Keys.Enter))
                    {
                        myGameState = GameState.isPlaying;
                    }
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
                    myDKIdleAnimation.DrawSpriteSheet(spriteBatch, gameTime, myDKTexture, new Vector2(
                        (Window.ClientBounds.Width / 2) - (myDKTexture.Width / 4),
                        (Window.ClientBounds.Height / 2) + (myDKTexture.Height) * 1.6f), 
                        new Point(myDKTexture.Width / 4, myDKTexture.Height), 
                        new Point(myDKTexture.Width / 2, myDKTexture.Height * 2), 
                        new Point(4, 1), 1.2f, Color.White, SpriteEffects.None, true);
                    break;
                case GameState.isPlaying:
                    myDKIdleAnimation.DrawSpriteSheet(spriteBatch, gameTime, myDKTexture, new Vector2(
                        (Window.ClientBounds.Width / 2) - (myDKTexture.Width / 8),
                        (Window.ClientBounds.Height / 2) - ((myDKTexture.Height) * 2) - 70),
                        new Point(myDKTexture.Width / 4, myDKTexture.Height),
                        new Point((myDKTexture.Width / 4), myDKTexture.Height),
                        new Point(4, 1), 1.3f, Color.White, SpriteEffects.None, true);

                    myLevel.Draw(spriteBatch);
                    GameInfo.Draw(spriteBatch, my8BitFont);
                    myEnemyManager.Draw(spriteBatch);
                    myPlayer.Draw(spriteBatch, gameTime);

                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "SCORE", new Vector2(20, 20), Color.White, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, GameInfo.Score.ToString(), new Vector2(26, 44), Color.White, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "BONUS", new Vector2(20, 80), Color.Blue, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, myBonusScore.ToString(), new Vector2(26, 104), Color.DarkOrange, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "LIVES", new Vector2(20, 140), Color.Red, 0.8f);
                    spriteBatch.Draw(myMarioHPTexture, new Vector2(26, 160),
                        new Rectangle(0, 0, myMarioHPTexture.Width - ((myMarioHPTexture.Width / 3) * (3 - myPlayer.Health)), myMarioHPTexture.Height), Color.White);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "HIGH SCORE", new Vector2(Window.ClientBounds.Width - 130, 20), Color.Red, 1.0f);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, myHighScore.ToString(), new Vector2(Window.ClientBounds.Width - 130, 50), Color.White, 1.1f);
                    break;
                case GameState.isPaused:
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "PAUSED", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2), Color.DarkOrange, 1.4f);
                    break;
                case GameState.isDead:
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "GAME OVER", new Vector2(40, 40), Color.Red, 1.4f);
                    break;
                case GameState.isWon:

                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void Pausing()
        {
            if (KeyMouseReader.KeyPressed(Keys.Escape)) //Pausing
            {
                if (myGameState == GameState.isPlaying || myGameState == GameState.isPaused)
                {
                    if (myGameState == GameState.isPlaying)
                    {
                        myGameState = GameState.isPaused;
                    }
                    else
                    {
                        myGameState = GameState.isPlaying;
                    }
                }
            }
        }
    }
}

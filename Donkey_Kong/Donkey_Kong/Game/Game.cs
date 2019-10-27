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

        private Texture2D myMenuTexture;

        private Player myPlayer;

        private GameState myGameState;
        private SpriteFont my8BitFont;

        private bool temp;

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

            EnemyManager.Initialize(new Point(140, 180), 6.0f, 6);
            GameInfo.Initialize(1.2f, 1.5f, 6000);
            ResourceManager.Initialize();
            myGameState = GameState.isOnMenu;

            myPlayer = new Player(new Vector2(Window.ClientBounds.Width / 6, Window.ClientBounds.Height - 60), new Point(40), 3, 170.0f, 120.0f, 15.5f, -320.0f);
            Level.LoadLevel(@"../../../../Levels/Level01.txt");

            GameInfo.LoadHighScore(@"../../../../High-Score/High-Score.txt");

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
            ResourceManager.AddTexture("DK_Laughing", this.Content.Load<Texture2D>("Sprites/dk_laughing"));
            ResourceManager.AddTexture("DK_Falling", this.Content.Load<Texture2D>("Sprites/dk_falling"));

            ResourceManager.AddSound("Walking", this.Content.Load<SoundEffect>("Sounds/walking"));
            ResourceManager.AddSound("Death", this.Content.Load<SoundEffect>("Sounds/death"));
            ResourceManager.AddSound("Item_Get", this.Content.Load<SoundEffect>("Sounds/itemget"));
            ResourceManager.AddSound("Jump", this.Content.Load<SoundEffect>("Sounds/jump"));
            ResourceManager.AddSound("BacMusic", this.Content.Load<SoundEffect>("Sounds/bacmusic"));
            ResourceManager.AddSound("Win", this.Content.Load<SoundEffect>("Sounds/win1"));

            myPlayer.SetTexture("Mario_Walking");
            myPlayer.SetMarioHPTexture();

            Level.SetTileTexture();
            Level.SetDKTexture("DK_Idle");

            myMenuTexture = ResourceManager.RequestTexture("Menu");

            my8BitFont = ResourceManager.RequestFont("8-bit");
        }

        protected override void UnloadContent()
        {

        }

        protected override void Update(GameTime gameTime)
        {
            KeyMouseReader.Update();

            Pausing();
            BackgroundMusic();

            switch (myGameState)
            {
                case GameState.isOnMenu:
                    if (KeyMouseReader.KeyPressed(Keys.Enter))
                    {
                        myGameState = GameState.isPlaying;
                    }
                    break;
                case GameState.isPlaying:
                    GameInfo.Update(gameTime);
                    Level.Update();

                    if (KeyMouseReader.KeyPressed(Keys.Enter))
                    {
                        temp = true;
                    }
                    if (temp)
                    {
                        Level.WinCondition(Window, gameTime);
                    }

                    if (!Level.LevelCleared)
                    {
                        myPlayer.Update(Window, gameTime);
                        EnemyManager.Update(Window, gameTime, myRNG, myPlayer);
                    }
                    break;
                case GameState.isPaused:
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
                    Level.DrawDK(spriteBatch, gameTime, new Vector2(
                        (Window.ClientBounds.Width / 2),
                        (Window.ClientBounds.Height / 2) + 160));
                    break;
                case GameState.isPlaying:
                    Level.DrawDK(spriteBatch, gameTime, 
                        Level.GetTileAtPos(new Vector2(Window.ClientBounds.Width / 2, 120)).Position);

                    Level.Draw(spriteBatch);
                    GameInfo.Draw(spriteBatch, my8BitFont);
                    EnemyManager.Draw(spriteBatch);
                    myPlayer.Draw(spriteBatch, gameTime);

                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "SCORE", new Vector2(20, 20), Color.White, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, GameInfo.Score.ToString(), new Vector2(26, 44), Color.White, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "BONUS", new Vector2(20, 80), Color.Blue, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, GameInfo.BonusScore.ToString(), new Vector2(26, 104), Color.DarkOrange, 0.8f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "LIVES", new Vector2(20, 140), Color.Red, 0.8f);
                    spriteBatch.Draw(myPlayer.MarioHPTexture, new Vector2(26, 160),
                        new Rectangle(0, 0, myPlayer.MarioHPTexture.Width - ((myPlayer.MarioHPTexture.Width / 3) * (3 - myPlayer.Health)), myPlayer.MarioHPTexture.Height), Color.White);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "HIGH SCORE", new Vector2(Window.ClientBounds.Width - 130, 20), Color.Red, 1.0f);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, GameInfo.HighScore.ToString(), new Vector2(Window.ClientBounds.Width - 130, 50), Color.White, 1.1f);
                    break;
                case GameState.isPaused:
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "PAUSED", new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height / 2), Color.DarkOrange, 1.4f);
                    break;
                case GameState.isDead:
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "GAME OVER", new Vector2(40, 40), Color.Red, 1.4f);
                    StringManager.DrawStringRight(spriteBatch, my8BitFont, "High Score", new Vector2(Window.ClientBounds.Width - 40, 40), Color.Red, 1.2f);
                    for (int i = 0; i < 10; i++)
                    {
                        StringManager.DrawStringRight(spriteBatch, my8BitFont, GameInfo.HighScores[i].ToString(), new Vector2(Window.ClientBounds.Width - 40, 40 + (20 * i)), Color.White, 0.7f);
                    }
                    break;
                case GameState.isWon:
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "YOU WIN", new Vector2(40, 40), Color.Red, 1.4f);
                    StringManager.DrawStringRight(spriteBatch, my8BitFont, "High Score", new Vector2(Window.ClientBounds.Width - 40, 40), Color.Red, 1.2f);
                    for (int i = 0; i < 10; i++)
                    {
                        StringManager.DrawStringRight(spriteBatch, my8BitFont, GameInfo.HighScores[i].ToString(), new Vector2(Window.ClientBounds.Width - 40, 40 + (20 * i)), Color.White, 0.7f);
                    }
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private static void BackgroundMusic()
        {
            if (!Level.LevelCleared)
            {
                ResourceManager.StopSound("Win");
                ResourceManager.PlaySound("BacMusic");
            }
            else
            {
                ResourceManager.StopSound("BacMusic");
                ResourceManager.PlaySound("Win");
            }
        }
        private void Exit()
        {

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

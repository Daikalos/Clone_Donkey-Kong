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
            myDKFalling,
            myDKLaughing;

        private Player myPlayer;

        private GameState myGameState;
        private SpriteFont my8BitFont;

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

            EnemyManager.Initialize(new Vector2(Window.ClientBounds.Width / 2, Window.ClientBounds.Height), new Point(140, 180), 3.5f, 7);
            GameInfo.Initialize(1.2f, 1.2f, 100, 6000);
            ResourceManager.Initialize();
            myGameState = GameState.isOnMenu;

            myPlayer = new Player(new Vector2(Window.ClientBounds.Width / 6, Window.ClientBounds.Height - 60), new Point(40), 3, 170.0f, 120.0f, 15.5f, -320.0f, 4.0f);
            Level.Initialize();
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
            ResourceManager.AddTexture("Mario_Climbing", this.Content.Load<Texture2D>("Sprites/mario_climbing"));
            ResourceManager.AddTexture("Mario_Hammer", this.Content.Load<Texture2D>("Sprites/mario_hammer"));
            ResourceManager.AddTexture("Mario_Lives", this.Content.Load<Texture2D>("Sprites/mario_lives"));
            ResourceManager.AddTexture("Mario_Death", this.Content.Load<Texture2D>("Sprites/mario_death"));
            ResourceManager.AddTexture("Pauline_Walking", this.Content.Load<Texture2D>("Sprites/pauline_walking"));
            ResourceManager.AddTexture("Pauline_Jumping", this.Content.Load<Texture2D>("Sprites/pauline_jumping"));
            ResourceManager.AddTexture("Pauline_Climbing", this.Content.Load<Texture2D>("Sprites/pauline_climbing"));
            ResourceManager.AddTexture("Pauline_Death", this.Content.Load<Texture2D>("Sprites/pauline_death"));
            ResourceManager.AddTexture("Bridge", this.Content.Load<Texture2D>("Sprites/bridge"));
            ResourceManager.AddTexture("Ladder", this.Content.Load<Texture2D>("Sprites/ladder"));
            ResourceManager.AddTexture("BridgeLadder", this.Content.Load<Texture2D>("Sprites/bridgeLadder"));
            ResourceManager.AddTexture("BridgePole", this.Content.Load<Texture2D>("Sprites/bridgePole"));
            ResourceManager.AddTexture("Empty", this.Content.Load<Texture2D>("Sprites/empty"));
            ResourceManager.AddTexture("Sprint", this.Content.Load<Texture2D>("Sprites/sprint"));
            ResourceManager.AddTexture("Pole", this.Content.Load<Texture2D>("Sprites/pole"));
            ResourceManager.AddTexture("Items", this.Content.Load<Texture2D>("Sprites/items"));
            ResourceManager.AddTexture("Hammer", this.Content.Load<Texture2D>("Sprites/hammer"));
            ResourceManager.AddTexture("Enemy", this.Content.Load<Texture2D>("Sprites/enemy"));
            ResourceManager.AddTexture("Menu", this.Content.Load<Texture2D>("Sprites/start"));
            ResourceManager.AddTexture("DK_Idle", this.Content.Load<Texture2D>("Sprites/dk_idle"));
            ResourceManager.AddTexture("DK_Laughing", this.Content.Load<Texture2D>("Sprites/dk_laughing"));
            ResourceManager.AddTexture("DK_Falling", this.Content.Load<Texture2D>("Sprites/dk_falling"));
            ResourceManager.AddTexture("Pauline", this.Content.Load<Texture2D>("Sprites/pauline"));
            ResourceManager.AddTexture("Heart", this.Content.Load<Texture2D>("Sprites/heart"));

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
            Level.SetPaulineTexture("Pauline");
            Level.SetHeartTexture("Heart");

            myMenuTexture = ResourceManager.RequestTexture("Menu");
            myDKFalling = ResourceManager.RequestTexture("DK_Falling");
            myDKLaughing = ResourceManager.RequestTexture("DK_Laughing");

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
                    SelectCharacter();
                    Quit();
                    break;
                case GameState.isPlaying:
                    Level.WinCondition(Window, gameTime, myPlayer);
                    if (!Level.LevelCleared)
                    {
                        myPlayer.Update(Window, gameTime);
                        GameInfo.Update(gameTime);
                        EnemyManager.Update(Window, gameTime, myRNG, myPlayer);

                        if (myPlayer.IsDead)
                        {
                            myGameState = GameState.isDead;
                        }
                    }
                    else
                    {
                        Level.Update();
                        LevelCleared();
                    }
                    break;
                case GameState.isPaused:
                    break;
                case GameState.isDead:
                    Reset();
                    break;
                case GameState.isWon:
                    Reset();
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
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "Press BACK to quit", new Vector2(20, Window.ClientBounds.Height - 20), Color.DarkOrange, 0.7f);
                    Level.DrawDK(spriteBatch, gameTime, new Vector2(
                        (Window.ClientBounds.Width / 2),
                        (Window.ClientBounds.Height / 2) + 160));
                    StringManager.DrawStringRight(spriteBatch, my8BitFont, myPlayer.Character, new Vector2(Window.ClientBounds.Width - 20, Window.ClientBounds.Height - 20), Color.DarkOrange, 0.7f);
                    break;
                case GameState.isPlaying:
                    Level.DrawTiles(spriteBatch);
                    Level.DrawDK(spriteBatch, gameTime,
                        Level.GetTileAtPos(new Vector2(Window.ClientBounds.Width / 2, Level.TileSize.Y * 3)).Position);
                    Level.DrawPauline(spriteBatch);
                    Level.DrawHeart(spriteBatch);

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
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "Press ENTER to continue", new Vector2(Window.ClientBounds.Width / 2, (Window.ClientBounds.Height / 2) + 60), Color.DarkOrange, 1.2f);
                    break;
                case GameState.isDead:
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "GAME OVER", new Vector2(40, 40), Color.Red, 1.4f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "Score", new Vector2(40, 80), Color.White, 1.1f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, (GameInfo.Score + GameInfo.BonusScore).ToString(), new Vector2(40, 110), Color.White, 1.0f);

                    StringManager.DrawStringRight(spriteBatch, my8BitFont, "High Score", new Vector2(Window.ClientBounds.Width - 40, 40), Color.Red, 1.2f);
                    for (int i = 0; i < GameInfo.HighScores.Length; i++)
                    {
                        if (i < 10)
                        {
                            StringManager.DrawStringRight(spriteBatch, my8BitFont, GameInfo.HighScores[i].ToString(), new Vector2(Window.ClientBounds.Width - 40, 70 + (25 * i)), Color.White, 0.7f);
                        }
                    }

                    spriteBatch.Draw(myDKLaughing, new Vector2((Window.ClientBounds.Width / 2) - myDKLaughing.Width / 2, (Window.ClientBounds.Height / 2) - 30), null, Color.White);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "Press ENTER to try again", new Vector2(Window.ClientBounds.Width / 2, (Window.ClientBounds.Height / 2) + 150), Color.DarkOrange, 1.1f);
                    break;
                case GameState.isWon:
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "YOU WIN", new Vector2(40, 40), Color.Yellow, 1.4f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, "Score", new Vector2(40, 80), Color.White, 1.1f);
                    StringManager.DrawStringLeft(spriteBatch, my8BitFont, (GameInfo.Score + GameInfo.BonusScore).ToString(), new Vector2(40, 110), Color.White, 1.0f);

                    StringManager.DrawStringRight(spriteBatch, my8BitFont, "High Score", new Vector2(Window.ClientBounds.Width - 40, 40), Color.Red, 1.2f);
                    for (int i = 0; i < GameInfo.HighScores.Length; i++)
                    {
                        if (i < 10)
                        {
                            StringManager.DrawStringRight(spriteBatch, my8BitFont, GameInfo.HighScores[i].ToString(), new Vector2(Window.ClientBounds.Width - 40, 70 + (25 * i)), Color.White, 0.9f);
                        }
                    }

                    spriteBatch.Draw(myDKFalling, new Vector2((Window.ClientBounds.Width / 2) - myDKLaughing.Width / 2, (Window.ClientBounds.Height / 2) - 30), 
                        new Rectangle(myDKFalling.Width / 2, 3, myDKFalling.Width, myDKFalling.Height), Color.White, 0.0f, Vector2.Zero, 1.0f, SpriteEffects.FlipVertically, 0.0f);
                    StringManager.DrawStringMid(spriteBatch, my8BitFont, "Press ENTER to play again", new Vector2(Window.ClientBounds.Width / 2, (Window.ClientBounds.Height / 2) + 150), Color.DarkOrange, 1.1f);
                    break;
            }

            spriteBatch.End();

            base.Draw(gameTime);
        }

        private void SelectCharacter()
        {
            if (KeyMouseReader.KeyPressed(Keys.Left))
            {
                myPlayer.Character = "Mario";
            }
            if (KeyMouseReader.KeyPressed(Keys.Right))
            {
                myPlayer.Character = "Pauline";
            }
        }

        private void BackgroundMusic()
        {
            if (myPlayer.Health > 0 || myGameState != GameState.isPlaying) //Dislike this solution but tired
            {
                if (!Level.LevelCleared)
                {
                    ResourceManager.StopSound("Win");
                    ResourceManager.PlaySound("BacMusic");
                }
                else if (!Level.LevelFinished)
                {
                    ResourceManager.StopSound("BacMusic");
                    ResourceManager.PlaySound("Win");
                }
            }
        }

        private void LevelCleared()
        {
            if (Level.LevelFinished && ResourceManager.RequestSoundEffect("Win").State == SoundState.Stopped)
            {
                Level.Initialize();
                GameInfo.SaveHighScore(@"../../../../High-Score/High-Score.txt");

                myGameState = GameState.isWon;
            }
        }

        private void Reset()
        {
            if (KeyMouseReader.KeyPressed(Keys.Enter))
            {
                EnemyManager.RemoveAll();
                GameInfo.Initialize(1.2f, 1.2f, 100, 6000);

                myPlayer = new Player(new Vector2(Window.ClientBounds.Width / 6, Window.ClientBounds.Height - 60), new Point(40), 3, 170.0f, 120.0f, 15.5f, -320.0f, 4.0f);
                Level.LoadLevel(@"../../../../Levels/Level01.txt");

                GameInfo.LoadHighScore(@"../../../../High-Score/High-Score.txt");

                myPlayer.SetTexture("Mario_Walking");
                myPlayer.SetMarioHPTexture();

                Level.SetTileTexture();
                Level.SetDKTexture("DK_Idle");
                Level.SetPaulineTexture("Pauline");
                Level.SetHeartTexture("Heart");

                myMenuTexture = ResourceManager.RequestTexture("Menu");

                myGameState = GameState.isOnMenu;
            }
        }
        private void Quit()
        {
            if (KeyMouseReader.KeyPressed(Keys.Back) && myGameState != GameState.isPlaying)
            {
                Exit();
            }
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

using System;
using System.IO;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    static class Level
    {
        private static string[] myLevelBuilder;
        private static Tile[,] myTiles;
        private static Point myTileSize;

        private static Tuple<Vector2, Vector2> myDropBridgeArea;
        private static bool myLevelCleared;
        private static bool myLevelFinished;
        private static int myStackBridge;

        private static Texture2D myDKTexture;
        private static Animation myDKIdleAnimation;
        private static Vector2 myDKPosition;
        private static Rectangle myDKSourceRect;

        private static Texture2D 
            myPaulineTexture,
            myHeartTexture;
        private static Vector2 
            myPaulinePosition,
            myHeartPosition;

        public static void SetDKPosition(Vector2 aDKPosition)
        {
            myDKPosition = new Vector2(aDKPosition.X - (myDKTexture.Width / 8), aDKPosition.Y);

            myPaulinePosition = Level.GetTileAtPos(
                new Vector2(myDKPosition.X + Level.TileSize.X, 
                myDKPosition.Y - Level.TileSize.Y)).Position;
        }
        public static bool LevelCleared
        {
            get => myLevelCleared;
            set => myLevelCleared = value;
        }
        public static bool LevelFinished
        {
            get => myLevelFinished;
        }
        public static Point TileSize
        {
            get => myTileSize;
        }

        public static Tile GetTileAtPos(Vector2 aPos)
        {
            if (((int)aPos.X / myTileSize.X) >= 0 && ((int)aPos.Y / myTileSize.Y) >= 0)
            {
                if (((int)aPos.X / myTileSize.X) < myTiles.GetLength(0) && ((int)aPos.Y / myTileSize.Y) < myTiles.GetLength(1))
                {
                    return myTiles[(int)aPos.X / myTileSize.X, (int)aPos.Y / myTileSize.Y];
                }
            }
            return myTiles[0, 0];
        }

        public static void Initialize()
        {
            myLevelCleared = false;
            myLevelFinished = false;
            myStackBridge = 0;
            myDKIdleAnimation = new Animation(new Point(4, 1), 1.3f, true, false);
        }
        public static void LoadLevel(string aFilePath)
        {
            myLevelBuilder = File.ReadAllLines(aFilePath);

            int tempSizeX = myLevelBuilder[0].Length;
            int tempSizeY = myLevelBuilder.Length;

            myTiles = new Tile[tempSizeX, tempSizeY];

            for (int x = 0; x < tempSizeX; x++)
            {
                for (int y = 0; y < tempSizeY; y++)
                {
                    myTileSize = new Point(40);
                    myTiles[x, y] = new Tile(
                        new Vector2(x * myTileSize.X, y * myTileSize.Y),
                        myTileSize);
                    myTiles[x, y].TileType = myLevelBuilder[y][x];
                }
            }

            Vector2 tempTopLeftCorner = Vector2.Zero;
            Vector2 tempBotRightCorner = Vector2.Zero;
            for (int y = 0; y < myTiles.GetLength(1); y++)
            {
                for (int x = 0; x < myTiles.GetLength(0); x++)
                {
                    if (myTiles[x, y].TileType == '#' && tempTopLeftCorner == Vector2.Zero)
                    {
                        tempTopLeftCorner = myTiles[x, y].BoundingBox.Center.ToVector2();
                    }
                    if (myTiles[x, y].TileType == '?')
                    {
                        tempBotRightCorner = myTiles[x + 1, y + 1].BoundingBox.Center.ToVector2();
                    }
                }
            }
            myDropBridgeArea = new Tuple<Vector2, Vector2>(tempTopLeftCorner, tempBotRightCorner);
        }

        public static void Update()
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].Update();
                }
            }
        }

        public static void DrawTiles(SpriteBatch aSpriteBatch)
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].Draw(aSpriteBatch);
                }
            }
        }
        public static void DrawDK(SpriteBatch aSpriteBatch, GameTime aGameTime, Vector2 aDKPos)
        {
            if (!LevelCleared)
            {
                SetDKPosition(aDKPos);
                myDKIdleAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myDKTexture, myDKPosition, new Point(myDKTexture.Width / 4, myDKTexture.Height),
                    new Point((myDKTexture.Width / 4), myDKTexture.Height), Color.White, SpriteEffects.None);
            }
            else
            {
                aSpriteBatch.Draw(myDKTexture, myDKPosition, myDKSourceRect, Color.White);
            }
        }
        public static void DrawPauline(SpriteBatch aSpriteBatch)
        {
            aSpriteBatch.Draw(myPaulineTexture, myPaulinePosition, null, Color.White, 0.0f, new Vector2(0, myPaulineTexture.Height), 1.0f, SpriteEffects.None, 0.0f);
        }
        public static void DrawHeart(SpriteBatch aSpriteBatch)
        {
            aSpriteBatch.Draw(myHeartTexture, myHeartPosition, null, Color.White);
        }

        public static void WinCondition(GameWindow aWindow, GameTime aGameTime, Player aPlayer)
        {
            bool tempNoPin = true;
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    if (myTiles[i, j].TileType == '?')
                    {
                        tempNoPin = false;
                    }
                }
            }
            if (tempNoPin)
            {
                EnemyManager.RemoveAll();
                myDKSourceRect = new Rectangle(0, 0, myDKTexture.Width / 4, myDKTexture.Height);
                myLevelCleared = true;

                bool tempDropDK = DropBridge(aWindow, aGameTime);

                if (tempDropDK)
                {
                    DropDK(aGameTime, aPlayer);
                }
            }
        }
        private static bool DropBridge(GameWindow aWindow, GameTime aGameTime)
        {
            bool tempDropDK = true;
            float tempWidth = myDropBridgeArea.Item2.X - myDropBridgeArea.Item1.X;
            float tempHeight = myDropBridgeArea.Item2.Y - myDropBridgeArea.Item1.Y;

            for (int i = 0; i < tempWidth / Level.TileSize.X; i++)
            {
                for (int j = 0; j < tempHeight / Level.TileSize.Y; j++)
                {
                    Tile tempTile = Level.GetTileAtPos(new Vector2((i * Level.TileSize.X) + myDropBridgeArea.Item1.X, ((j + 2) * Level.TileSize.Y) + myDropBridgeArea.Item1.Y));
                    if (tempTile.TileType == '%')
                    {
                        tempTile.TileType = '#';
                        tempTile.SetTexture();
                    }
                    if (tempTile.TileType == '@' || tempTile.TileType == '/')
                    {
                        tempTile.TileType = '.';
                        tempTile.SetTexture();
                    }
                    float tempSpeed = 120 * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    if (tempTile.TileType == '#' && tempTile.Position.Y + (tempTile.BoundingBox.Size.Y / 2) + tempSpeed < ((aWindow.ClientBounds.Height - 20) - (20 * myStackBridge)))
                    {
                        tempDropDK = false;
                        tempTile.Position = new Vector2(tempTile.Position.X, tempTile.Position.Y + tempSpeed);
                    }
                    if (tempTile.TileType == '#' && tempTile.Position.Y + (tempTile.BoundingBox.Size.Y / 2) + tempSpeed >= ((aWindow.ClientBounds.Height - 20) - (20 * myStackBridge)))
                    {
                        myStackBridge++;

                        for (int k = 0; k < tempWidth / 40; k++)
                        {
                            Tile tempStopTile = Level.GetTileAtPos(new Vector2(
                                (k * Level.TileSize.X) + myDropBridgeArea.Item1.X, 
                                ((j + 2) * Level.TileSize.Y) + myDropBridgeArea.Item1.Y));

                            tempStopTile.Position = new Vector2(tempStopTile.Position.X, ((aWindow.ClientBounds.Height - 20) - (20 * myStackBridge)));
                            tempStopTile.TileType = '.';
                        }
                    }
                }
            }
            return tempDropDK;
        }
        private static void DropDK(GameTime aGameTime, Player aPlayer)
        {
            SetDKTexture("DK_Falling");

            float tempFallSpeed = 250;
            int tempFallHeight = (10 * 50) + 5;

            if (myDKPosition.Y + tempFallSpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds <= tempFallHeight)
            {
                myDKPosition.Y += tempFallSpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myDKSourceRect = new Rectangle(0, 0, myDKTexture.Width / 2, myDKTexture.Height);
            }
            else
            {
                myDKPosition.Y = tempFallHeight;
                myDKSourceRect = new Rectangle(myDKTexture.Width / 2, 0, myDKTexture.Width / 2, myDKTexture.Height);

                DropPlatform(aPlayer);
            }
        }
        private static void DropPlatform(Player aPlayer)
        {
            float tempWidth = myDropBridgeArea.Item2.X - myDropBridgeArea.Item1.X;
            for (int i = 0; i < tempWidth / Level.TileSize.X; i++)
            {
                Tile tempDropTile = Level.GetTileAtPos(new Vector2((i * Level.TileSize.X) + myDropBridgeArea.Item1.X, myDropBridgeArea.Item1.Y));
                Tile tempDropToTile = Level.GetTileAtPos(new Vector2(myDropBridgeArea.Item1.X, myDropBridgeArea.Item1.Y + Level.TileSize.Y * 2));

                tempDropTile.Position = new Vector2(tempDropTile.Position.X, tempDropToTile.Position.Y + Level.TileSize.Y);
                myPaulinePosition = new Vector2(myPaulinePosition.X, tempDropTile.Position.Y);
                aPlayer.BoundingBox = new Rectangle(
                    (int)myPaulinePosition.X + 120, 
                    (int)tempDropToTile.Position.Y, 
                    aPlayer.BoundingBox.Width, aPlayer.BoundingBox.Height);
                aPlayer.LevelFinished();
                myHeartPosition = new Vector2(myPaulinePosition.X + 65, myPaulinePosition.Y - 60);

                myLevelFinished = true;

                tempDropTile.TileType = '#';
                tempDropTile.SetTexture();

            }
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    if (myTiles[i, j].TileType == '=')
                    {
                        myTiles[i, j].TileType = '.';
                        myTiles[i, j].SetTexture();
                    }
                }
            }
        }

        public static void SetTileTexture()
        {
            int tempItem = 0;
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].SetTexture();
                    if (myTiles[i, j].TileType == '/')
                    {
                        myTiles[i, j].SetItemSourceRect(tempItem);
                        tempItem++;
                    }
                }
            }
        }
        public static void SetDKTexture(string aTextureName)
        {
            myDKTexture = ResourceManager.RequestTexture(aTextureName);
        }
        public static void SetPaulineTexture(string aTextureName)
        {
            myPaulineTexture = ResourceManager.RequestTexture(aTextureName);
        }
        public static void SetHeartTexture(string aTextureName)
        {
            myHeartTexture = ResourceManager.RequestTexture(aTextureName);
            myHeartPosition = new Vector2(-myHeartTexture.Width, -myHeartTexture.Height); //ritar den utanför, snabb lösning
        }
    }
}

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

        private static Tuple<Vector2, Vector2> myDropBridgeArea;
        private static bool myLevelCleared;
        private static int myStackBridge;

        private static Texture2D myDKTexture;
        private static Animation myDKIdleAnimation;
        private static Vector2 myDKPosition;
        private static Rectangle myDKSourceRect;

        public static void SetDKPosition(Vector2 aDKPosition)
        {
            myDKPosition = new Vector2(aDKPosition.X - (myDKTexture.Width / 8), aDKPosition.Y);
        }
        public static bool LevelCleared
        {
            get => myLevelCleared;
            set => myLevelCleared = value;
        }

        public static Tile GetTileAtPos(Vector2 aPos)
        {
            if (((int)aPos.X / 40) >= 0 && ((int)aPos.Y / 40) >= 0)
            {
                if (((int)aPos.X / 40) < myTiles.GetLength(0) && ((int)aPos.Y / 40) < myTiles.GetLength(1))
                {
                    return myTiles[(int)aPos.X / 40, (int)aPos.Y / 40];
                }
            }
            return myTiles[0, 0];
        }

        public static void LoadLevel(string aFilePath)
        {
            myLevelCleared = false;
            myStackBridge = 0;
            myDKIdleAnimation = new Animation();

            myLevelBuilder = File.ReadAllLines(aFilePath);

            int tempSizeX = myLevelBuilder[0].Length;
            int tempSizeY = myLevelBuilder.Length;

            myTiles = new Tile[tempSizeX, tempSizeY];

            for (int x = 0; x < tempSizeX; x++)
            {
                for (int y = 0; y < tempSizeY; y++)
                {
                    int tempTileSize = 40;
                    myTiles[x, y] = new Tile(
                        new Vector2(x * tempTileSize, y * tempTileSize),
                        new Point(tempTileSize));
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

        public static void Draw(SpriteBatch aSpriteBatch)
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
                myDKIdleAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myDKTexture, myDKPosition,
                    new Point(myDKTexture.Width / 4, myDKTexture.Height),
                    new Point((myDKTexture.Width / 4), myDKTexture.Height),
                    new Point(4, 1), 1.3f, Color.White, SpriteEffects.None, true);
            }
            else
            {
                aSpriteBatch.Draw(myDKTexture, myDKPosition, myDKSourceRect, Color.White);
            }
        }

        public static void WinCondition(GameWindow aWindow, GameTime aGameTime)
        {
            bool tempNoPin = true;
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    if (myTiles[i, j].TileType == '?')
                    {
                        //tempNoPin = false;
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
                    DropDK(aGameTime);
                }
            }
        }

        private static bool DropBridge(GameWindow aWindow, GameTime aGameTime)
        {
            bool tempDropDK = true;
            float tempWidth = myDropBridgeArea.Item2.X - myDropBridgeArea.Item1.X;
            float tempHeight = myDropBridgeArea.Item2.Y - myDropBridgeArea.Item1.Y;

            for (int i = 0; i < tempWidth / 40; i++)
            {
                for (int j = 0; j < tempHeight / 40; j++)
                {
                    Tile tempTile = Level.GetTileAtPos(new Vector2((i * 40) + myDropBridgeArea.Item1.X, ((j + 2) * 40) + myDropBridgeArea.Item1.Y));
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
                            Tile tempStopTile = Level.GetTileAtPos(new Vector2((k * 40) + myDropBridgeArea.Item1.X, ((j + 2) * 40) + myDropBridgeArea.Item1.Y));

                            tempStopTile.Position = new Vector2(tempStopTile.Position.X, ((aWindow.ClientBounds.Height - 20) - (20 * myStackBridge)));
                            tempStopTile.TileType = '.';
                        }
                    }
                }
            }
            return tempDropDK;
        }
        private static void DropDK(GameTime aGameTime)
        {
            SetDKTexture("DK_Falling");

            bool tempDropTopBridge = true;
            float tempFallSpeed = 250;
            int tempFallHeight = 10 * 51;

            if (myDKPosition.Y + tempFallSpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds <= tempFallHeight)
            {
                myDKPosition.Y += tempFallSpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                myDKSourceRect = new Rectangle(0, 0, myDKTexture.Width / 2, myDKTexture.Height);
            }
            else if (tempDropTopBridge)
            {
                myDKPosition.Y = tempFallHeight;
                myDKSourceRect = new Rectangle(myDKTexture.Width / 2, 0, myDKTexture.Width / 2, myDKTexture.Height);

                float tempWidth = myDropBridgeArea.Item2.X - myDropBridgeArea.Item1.X;
                for (int i = 0; i < tempWidth / 40; i++)
                {
                    Tile tempDropTile = Level.GetTileAtPos(new Vector2((i * 40) + myDropBridgeArea.Item1.X, myDropBridgeArea.Item1.Y));
                    Tile tempDropToTile = Level.GetTileAtPos(new Vector2(myDropBridgeArea.Item1.X, myDropBridgeArea.Item1.Y + 80));

                    tempDropTile.Position = new Vector2(tempDropTile.Position.X, tempDropToTile.Position.Y + 40);

                    tempDropTopBridge = false;
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
    }
}

using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    class Level
    {
        private string[] myLevelBuilder;
        private Tile[,] myTiles;

        public Tile[,] Tiles
        {
            get => myTiles;
        }

        public Tile GetTileAtPos(Vector2 aPos)
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

        public Level(string aFilePath)
        {
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
        }

        public void Draw(SpriteBatch aSpriteBatch)
        {
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].Draw(aSpriteBatch);
                }
            }
        }

        public void SetTileTexture()
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
    }
}

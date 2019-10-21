using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Donkey_Kong
{
    class Level
    {
        string[] myLevelBuilder;
        Tile[,] myTiles;

        public Tile[,] Tiles
        {
            get => myTiles;
        }

        public Tile GetTileAtPos(Vector2 aPos)
        {
            return myTiles[(int)aPos.X / 40, (int)aPos.Y / 40];
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
                        new Vector2(x * tempTileSize, (y + 1) * tempTileSize), 
                        new Point(tempTileSize));
                    myTiles[x, y].TileType = myLevelBuilder[y][x];
                }
            }
        }

        public void Update()
        {

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
            for (int i = 0; i < myTiles.GetLength(0); i++)
            {
                for (int j = 0; j < myTiles.GetLength(1); j++)
                {
                    myTiles[i, j].SetTexture();
                }
            }
        }
    }
}

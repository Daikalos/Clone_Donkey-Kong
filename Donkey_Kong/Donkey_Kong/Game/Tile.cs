using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Donkey_Kong
{
    class Tile
    {
        Texture2D myTexture;
        Vector2 myPosition;
        Rectangle myBoundingBox;
        Point mySize;
        char myTileType;

        public char TileType
        {
            get => myTileType;
            set => myTileType = value;
        }

        public Rectangle BoundingBox
        {
            get => myBoundingBox;
        }

        public Tile(Vector2 aPosition, Point aSize)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
            this.myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, aSize.X, aSize.Y);
        }

        public void Update()
        {

        }

        public void Draw(SpriteBatch aSpriteBatch)
        {
            if (myTexture != null)
            {
                aSpriteBatch.Draw(myTexture, new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y), null, Color.White);
            }
        }

        public void SetTexture()
        {
            switch (myTileType)
            {
                case '#':
                    myTexture = ResourceManager.RequestTexture("Bridge");
                    break;
                case '%':
                    myTexture = ResourceManager.RequestTexture("BridgeLadder");
                    break;
                case '@':
                    myTexture = ResourceManager.RequestTexture("Ladder");
                    break;
                case '=':
                    myTexture = ResourceManager.RequestTexture("Pole");
                    break;
                case '.':
                    myTexture = ResourceManager.RequestTexture("Empty");
                    break;
            }
        }
    }
}

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    class Tile
    {
        Texture2D myTexture;
        Vector2 myPosition;
        Rectangle 
            myBoundingBox,
            mySourceRect;
        Point mySize;
        char myTileType;

        /// <summary>
        /// # = Block; 
        /// @ = Ladder;
        /// % = BridgeLadder;
        /// ? = Pins; 
        /// / = Item;
        /// . = Empty;
        /// </summary>
        public char TileType
        {
            get => myTileType;
            set => myTileType = value;
        }

        public Rectangle BoundingBox
        {
            get => myBoundingBox;
        }

        public Vector2 Position
        {
            get => myPosition;
        }

        public Tile(Vector2 aPosition, Point aSize)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;

            this.myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, aSize.X, aSize.Y);
        }

        public void Draw(SpriteBatch aSpriteBatch)
        {
            aSpriteBatch.Draw(myTexture, myBoundingBox, mySourceRect, Color.White);
        }

        public void SetItemSourceRect(int aXPos)
        {
            mySourceRect = new Rectangle((myTexture.Width / 3) * aXPos, 0, myTexture.Width / 3, myTexture.Height);
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
                case '?':
                    myTexture = ResourceManager.RequestTexture("Sprint");
                    myBoundingBox = new Rectangle((int)myPosition.X - 6, (int)myPosition.Y - 4, 52, 42);
                    break;
                case '/':
                    myTexture = ResourceManager.RequestTexture("Items");
                    break;
                case '.':
                    myTexture = ResourceManager.RequestTexture("Empty");
                    break;
            }
            mySourceRect = new Rectangle(0, 0, myTexture.Width, myTexture.Height);
        }
    }
}

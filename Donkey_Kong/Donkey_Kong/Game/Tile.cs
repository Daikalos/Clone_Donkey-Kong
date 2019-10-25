using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    class Tile
    {
        Texture2D myTexture;
        Vector2 myPosition;
        Rectangle myBoundingBox;
        Point mySize;
        char myTileType;

        /// <summary>
        /// # = Block; 
        /// @ = Ladder;
        /// % = BridgeLadder;
        /// ? = Pins; 
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

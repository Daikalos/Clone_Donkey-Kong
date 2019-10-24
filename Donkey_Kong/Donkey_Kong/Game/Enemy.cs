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
    class Enemy
    {
        enum EnemyState
        {
            isClimbing,
            isWalking,
        }

        private Texture2D myTexture;

        private Vector2 
            myPosition,
            myDirection,
            myDestination;
        private Point mySize;
        private Rectangle myBoundingBox;
        private EnemyState myEnemyState;
        private SpriteEffects myFlipSprite;

        private float 
            mySpeed,
            mySwitchDestTimer,
            mySwitchDestTimerMax;
        private bool myIsAlive;

        public bool IsAlive
        {
            get => myIsAlive;
            set => myIsAlive = value;
        }

        public Enemy(Vector2 aPosition, Point aSize, float aSpeed, float aSwitchDestTimerMax)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
            this.mySpeed = aSpeed;
            this.mySwitchDestTimerMax = aSwitchDestTimerMax;

            this.mySwitchDestTimer = 0;
            myEnemyState = EnemyState.isWalking;
            this.myIsAlive = true;
        }

        public void Update(GameTime aGameTime, Random aRNG, Level aLevel)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myEnemyState)
            {
                case EnemyState.isWalking:
                    Movement(aGameTime, aRNG, aLevel);
                    break;
                case EnemyState.isClimbing:
                    Climbing(aGameTime, aLevel);
                    break;
            }
        }

        public void Draw(SpriteBatch aSpriteBatch)
        {
            aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
        }

        private void Climbing(GameTime aGameTime, Level aLevel)
        {
            if (Math.Abs(myBoundingBox.Center.ToVector2().Y - myDestination.Y) > 1.0f)
            {
                myDirection.Y = myDestination.Y - myBoundingBox.Center.ToVector2().Y;
                myDirection.Normalize();

                myPosition.Y += myDirection.Y * mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                myPosition.Y = myDestination.Y - mySize.Y / 2;
                myEnemyState = EnemyState.isWalking;
            }
        }
        private void Movement(GameTime aGameTime, Random aRNG, Level aLevel)
        {
            mySwitchDestTimer += (float)aGameTime.ElapsedGameTime.TotalSeconds;

            if (mySwitchDestTimer >= mySwitchDestTimerMax)
            {
                if (aRNG.Next(0, 2) == 0)
                {
                    MoveTo(aRNG, aLevel, 1);
                    myFlipSprite = SpriteEffects.None;
                }
                else
                {
                    MoveTo(aRNG, aLevel, -1);
                    myFlipSprite = SpriteEffects.FlipHorizontally;
                }

                if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).TileType == '%')
                {
                    if (aRNG.Next(0, 100) > 40)
                    {
                        myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 120)).BoundingBox.Center.ToVector2();
                        myPosition.X = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).Position.X;
                        myEnemyState = EnemyState.isClimbing;
                    }
                }
                if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType == '@')
                {
                    if (aRNG.Next(0, 100) > 60)
                    {
                        myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 120)).BoundingBox.Center.ToVector2();
                        myPosition.X = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).Position.X;
                        myEnemyState = EnemyState.isClimbing;
                    }
                }

                mySwitchDestTimer = 0;
            }

            if (Math.Abs(myBoundingBox.Center.ToVector2().X - myDestination.X) > 1.0f)
            {
                myDirection.X = myDestination.X - myBoundingBox.Center.ToVector2().X;
                myDirection.Normalize();

                myPosition.X += myDirection.X * mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                myPosition.X = myDestination.X - mySize.X / 2;
            }
        }
        public void MoveTo(Random aRNG, Level aLevel, int aDirection)
        {
            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
            for (int i = 1; i < aRNG.Next(0, 7) + 1; i++)
            {
                tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - (40 * i * aDirection), myBoundingBox.Center.Y + 40));
                if (tempTile.TileType == '.')
                {
                    tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - (40 * i * aDirection) + (40 * aDirection), myBoundingBox.Center.Y + 40));
                    break;
                }
            }

            myDestination = tempTile.BoundingBox.Center.ToVector2();
        }

        public void SetTexture(string aTextureName)
        {
            myTexture = ResourceManager.RequestTexture(aTextureName);
        }
    }
}

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

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

        public Rectangle BoundingBox
        {
            get => myBoundingBox;
        }
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

        public void Update(GameTime aGameTime, Random aRNG)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myEnemyState)
            {
                case EnemyState.isWalking:
                    Movement(aGameTime, aRNG);
                    break;
                case EnemyState.isClimbing:
                    Climbing(aGameTime);
                    break;
            }
        }

        public void Draw(SpriteBatch aSpriteBatch)
        {
            aSpriteBatch.Draw(myTexture, myBoundingBox, null, Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
        }

        private void Climbing(GameTime aGameTime)
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
        private void Movement(GameTime aGameTime, Random aRNG)
        {
            mySwitchDestTimer += (float)aGameTime.ElapsedGameTime.TotalSeconds;

            if (mySwitchDestTimer >= mySwitchDestTimerMax)
            {
                if (aRNG.Next(0, 100) < 50)
                {
                    MoveTo(aRNG, 1);
                    myFlipSprite = SpriteEffects.None;
                }
                else
                {
                    MoveTo(aRNG, -1);
                    myFlipSprite = SpriteEffects.FlipHorizontally;
                }

                if (Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + Level.TileSize.Y)).TileType == '%')
                {
                    if (aRNG.Next(0, 100) > 70)
                    {
                        myDestination = Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + (Level.TileSize.Y * 3))).BoundingBox.Center.ToVector2();
                        myPosition.X = Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).Position.X;
                        myEnemyState = EnemyState.isClimbing;
                    }
                }
                if (Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType == '@')
                {
                    if (aRNG.Next(0, 100) > 80)
                    {
                        myDestination = Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - (Level.TileSize.Y * 3))).BoundingBox.Center.ToVector2();
                        myPosition.X = Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).Position.X;
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
        public void MoveTo(Random aRNG, int aDirection)
        {
            Tile tempTile = Level.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
            for (int i = 0; i < aRNG.Next(0, 6) + 1; i++)
            {
                tempTile = Level.GetTileAtPos(new Vector2(
                    myBoundingBox.Center.X - (Level.TileSize.X * i * aDirection), 
                    myBoundingBox.Center.Y + Level.TileSize.Y));
                if (tempTile.TileType == '.')
                {
                    tempTile = Level.GetTileAtPos(new Vector2(
                        myBoundingBox.Center.X - (Level.TileSize.X * i * aDirection) + (Level.TileSize.X * aDirection), 
                        myBoundingBox.Center.Y + Level.TileSize.Y));
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

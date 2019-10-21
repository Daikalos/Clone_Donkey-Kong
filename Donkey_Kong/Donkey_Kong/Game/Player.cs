using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Donkey_Kong
{
    class Player
    {
        private enum PlayerState
        {
            isWalking,
            isClimbing,
            isJumping,
            isFalling,
            isDead
        }

        private Texture2D myTexture;
        private Vector2 
            myPosition,
            myDirection,
            myDestination;
        private PlayerState myPlayerState;

        private Animation myWalkingAnimation;

        private Point mySize;
        private Rectangle myBoundingBox;
        private SpriteEffects myFlipSprite;
        private int myLives;
        private float
            mySpeed,
            myClimbSpeed,
            myVelocity,
            myGravity,
            myJumpHeight;
        private bool
            myIsMoving,
            myJumpAvailable;

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aClimbSpeed, float aGravity, float aJumpHeight)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
            this.mySpeed = aSpeed;
            this.myClimbSpeed = aClimbSpeed;
            this.myGravity = aGravity;
            this.myJumpHeight = aJumpHeight;

            this.myIsMoving = false;
            this.myJumpAvailable = true;
            this.myWalkingAnimation = new Animation();
            this.myPlayerState = PlayerState.isFalling;
            this.myDestination = new Vector2(myPosition.X + mySize.X / 2, myPosition.Y);
            this.myDirection = Vector2.Zero;
        }

        public void Update(GameWindow aWindow, GameTime aGameTime, Level myLevel)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    Movement(aWindow, aGameTime, myLevel);
                    break;
                case PlayerState.isClimbing:
                    Climbing(aGameTime, myLevel);
                    break;
                case PlayerState.isJumping:
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;

                    if (myIsMoving && !OutsideBounds(aWindow, aGameTime))
                    {
                        if (myFlipSprite == SpriteEffects.FlipHorizontally)
                        {
                            myPosition.X -= mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds * 0.6f;
                        }
                        else
                        {
                            myPosition.X += mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds * 0.6f;
                        }
                    }
                    break;
                case PlayerState.isFalling:
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case PlayerState.isDead:

                    break;
            }

            CollisionChecking(myLevel, aGameTime);
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, myTexture.Width / 3, myTexture.Height, mySize.X, mySize.Y, 3, 1, 0.035f, myFlipSprite, true);
                    }
                    else
                    {
                        aSpriteBatch.Draw(myTexture,
                            new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y),
                            new Rectangle(16, 0, myTexture.Width / 3, myTexture.Height), Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    }
                    break;
                case PlayerState.isClimbing:
                    aSpriteBatch.Draw(myTexture, new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y), null, Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    break;
                case PlayerState.isJumping:
                    aSpriteBatch.Draw(myTexture, new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y), null, Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    break;
                case PlayerState.isFalling:
                    aSpriteBatch.Draw(myTexture, new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y), null, Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    break;
                case PlayerState.isDead:

                    break;
            }
        }

        private void Movement(GameWindow aWindow, GameTime aGameTime, Level myLevel)
        {
            if (KeyMouseReader.KeyHold(Keys.Left) && !OutsideBounds(aWindow, aGameTime))
            {
                myDestination = myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - 40, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();

                myFlipSprite = SpriteEffects.FlipHorizontally;
                myIsMoving = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Right) && !OutsideBounds(aWindow, aGameTime))
            {
                myDestination = myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + 40, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();

                myFlipSprite = SpriteEffects.None;
                myIsMoving = true;
            }
            if (!KeyMouseReader.KeyHold(Keys.Left) && !KeyMouseReader.KeyHold(Keys.Right))
            {
                myIsMoving = false;
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

            if (KeyMouseReader.KeyPressed(Keys.Space) && myJumpAvailable)
            {
                myPlayerState = PlayerState.isJumping;
                myVelocity = myJumpHeight;
                myJumpAvailable = false;

                SetTexture("Mario_Jumping");
            }
        }
        private void Climbing(GameTime aGameTime, Level myLevel)
        {
            if (KeyMouseReader.KeyHold(Keys.Up))
            {
                myDestination = myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).BoundingBox.Center.ToVector2();
            }
            if (KeyMouseReader.KeyHold(Keys.Down))
            {
                myDestination = myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).BoundingBox.Center.ToVector2();
            }

            if (Math.Abs(myBoundingBox.Center.ToVector2().Y - myDestination.Y) > 1.0f)
            {
                myDirection.Y = myDestination.Y - myBoundingBox.Center.ToVector2().Y;
                myDirection.Normalize();

                myPosition.Y -= myDirection.Y * mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                myPosition.Y = myDestination.Y - mySize.Y / 2;
            }
        }

        private void CollisionChecking(Level myLevel, GameTime aGameTime)
        {
            IsFalling(myLevel);
            CollisionBlock(myLevel);
            CollisionLadder(myLevel, aGameTime);
        }
        private void IsFalling(Level myLevel)
        {
            if (myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType == '.')
            {
                myPlayerState = PlayerState.isFalling;
                SetTexture("Mario_Jumping");
            }
        }
        private void CollisionBlock(Level myLevel)
        {
            Tile tempTile = myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
            if (myPlayerState != PlayerState.isClimbing)
            {
                if (tempTile.TileType == '#' || tempTile.TileType == '%')
                {
                    myVelocity = 0.0f;

                    myPosition.Y = myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).BoundingBox.Y - myBoundingBox.Height;
                    myJumpAvailable = true;

                    myPlayerState = PlayerState.isWalking;
                    SetTexture("Mario_Walking");
                }
            }
        }
        private void CollisionLadder(Level myLevel, GameTime aGameTime)
        {
            if (myLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).TileType == '@')
            {
                if (KeyMouseReader.KeyHold(Keys.Up))
                {
                    myPlayerState = PlayerState.isClimbing;
                    SetTexture("Mario_Jumping");
                }
            }
        }
        private bool OutsideBounds(GameWindow aWindow, GameTime aGameTime)
        {
            if (myPosition.X < 0)
            {
                myPosition.X = 0;
                return true;
            }
            if (myPosition.X + mySize.X > aWindow.ClientBounds.Width)
            {
                myPosition.X = aWindow.ClientBounds.Width - mySize.X;
                return true;
            }
            if (myPosition.Y > aWindow.ClientBounds.Height) //Safety measure
            {
                myPosition = new Vector2(aWindow.ClientBounds.Width / 6, aWindow.ClientBounds.Height - 720);
                myPlayerState = PlayerState.isFalling;
            }
            return false;
        }

        public void SetTexture(string aTextureName)
        {
            myTexture = ResourceManager.RequestTexture(aTextureName);
        }
    }
}
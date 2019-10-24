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

        public void Update(GameWindow aWindow, GameTime aGameTime, Level aLevel)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    Movement(aWindow, aGameTime, aLevel);
                    break;
                case PlayerState.isClimbing:
                    Climbing(aGameTime, aLevel);
                    break;
                case PlayerState.isJumping:
                    Jumping(aGameTime, aLevel);
                    break;
                case PlayerState.isFalling:
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case PlayerState.isDead:

                    break;
            }

            CollisionChecking(aGameTime, aLevel);
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
                            new Rectangle(myTexture.Width / 3, 0, myTexture.Width / 3, myTexture.Height), Color.White, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
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

        private void Movement(GameWindow aWindow, GameTime aGameTime, Level aLevel)
        {
            if (KeyMouseReader.KeyHold(Keys.Left) && OutsideBounds(aWindow, aLevel) != -1)
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - 40, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();

                myFlipSprite = SpriteEffects.FlipHorizontally;
                myIsMoving = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Right) && OutsideBounds(aWindow, aLevel) != 1)
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + 40, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();

                myFlipSprite = SpriteEffects.None;
                myIsMoving = true;
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
                myIsMoving = false;
            }

            if (KeyMouseReader.KeyPressed(Keys.Space) && myJumpAvailable && aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType != '.')
            {
                myPlayerState = PlayerState.isJumping;
                myVelocity = myJumpHeight;
                myJumpAvailable = false;

                if (myIsMoving)
                {
                    for (int i = 0; i < 4; i++)
                    {
                        if (myFlipSprite == SpriteEffects.None)
                        {
                            if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + (40 * i), myBoundingBox.Center.Y)).TileType == '.')
                            {
                                break;
                            }
                            myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + (40 * i), myBoundingBox.Center.Y)).BoundingBox.Center.ToVector2();
                        }
                        else
                        {
                            if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - (40 * i), myBoundingBox.Center.Y)).TileType == '.')
                            {
                                break;
                            }
                            myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - (40 * i), myBoundingBox.Center.Y)).BoundingBox.Center.ToVector2();
                        }
                    }
                }

                SetTexture("Mario_Jumping");
            }
        }
        private void Climbing(GameTime aGameTime, Level aLevel)
        {
            if (KeyMouseReader.KeyHold(Keys.Up))
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 80)).BoundingBox.Center.ToVector2();
            }
            if (KeyMouseReader.KeyHold(Keys.Down) && aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType != '#')
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).BoundingBox.Center.ToVector2();
            }

            if (Math.Abs(myBoundingBox.Center.ToVector2().Y - myDestination.Y) > 1.0f)
            {
                myDirection.Y = myDestination.Y - myBoundingBox.Center.ToVector2().Y;
                myDirection.Normalize();

                myPosition.Y += myDirection.Y * myClimbSpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                myPosition.Y = myDestination.Y - mySize.Y / 2;
            }
        }
        private void Jumping(GameTime aGameTime, Level aLevel)
        {
            myVelocity += myGravity;
            myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;

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

        private void CollisionChecking(GameTime aGameTime, Level aLevel)
        {
            IsFalling(aLevel);
            CollisionBlock(aGameTime, aLevel);
            CollisionLadder(aLevel);
        }
        private void IsFalling(Level aLevel)
        {
            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
            if (Math.Abs(myBoundingBox.Center.X - tempTile.BoundingBox.Center.X) <= 1.0f && myPlayerState != PlayerState.isJumping)
            {
                if (tempTile.TileType == '.' || tempTile.TileType == '?')
                {
                    myPosition.X = tempTile.Position.X;

                    myPlayerState = PlayerState.isFalling;
                    SetTexture("Mario_Jumping");
                }
            }
        }
        private void CollisionBlock(GameTime aGameTime, Level aLevel)
        {
            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
            if (myPlayerState != PlayerState.isClimbing && myPosition.Y + (myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds) >= tempTile.BoundingBox.Y - mySize.Y)
            {
                if (tempTile.TileType == '#' || tempTile.TileType == '%')
                {
                    myVelocity = 0.0f;

                    myPosition.Y = tempTile.BoundingBox.Y - mySize.Y;
                    myJumpAvailable = true;

                    myPlayerState = PlayerState.isWalking;
                    SetTexture("Mario_Walking");
                }
            }
        }
        private void CollisionLadder(Level aLevel)
        {
            if (!myIsMoving)
            {
                if (myPlayerState != PlayerState.isClimbing)
                {
                    if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).TileType == '@')
                    {
                        if (KeyMouseReader.KeyHold(Keys.Up))
                        {
                            myPlayerState = PlayerState.isClimbing;
                            SetTexture("Mario_Jumping");
                        }
                    }
                    if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType == '%')
                    {
                        if (KeyMouseReader.KeyHold(Keys.Down))
                        {
                            myPlayerState = PlayerState.isClimbing;
                            SetTexture("Mario_Jumping");
                        }
                    }
                }
                else
                {
                    if (Math.Abs(myBoundingBox.Center.ToVector2().Y - myDestination.Y) < 1.0f)
                    {
                        Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
                        if (tempTile.TileType == '%' || tempTile.TileType == '#')
                        {
                            myPosition.Y = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).BoundingBox.Y - myBoundingBox.Height;
                            myJumpAvailable = true;

                            myPlayerState = PlayerState.isWalking;
                            SetTexture("Mario_Walking");
                        }
                    }
                }
            }
        }
        private int OutsideBounds(GameWindow aWindow, Level aLevel)
        {
            if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - 40, myBoundingBox.Center.Y)).Position == new Vector2(0, 40)) //Will return myTiles[0, 0] position if outside grid
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();
                return -1;
            }
            if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + 40, myBoundingBox.Center.Y)).Position == new Vector2(0, 40))
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();
                return 1;
            }
            if (myPosition.Y > aWindow.ClientBounds.Height) //Safety measure
            {
                myPosition = new Vector2(aWindow.ClientBounds.Width / 6, aWindow.ClientBounds.Height - 720);
                myPlayerState = PlayerState.isFalling;
            }
            return 0;
        }

        public void SetTexture(string aTextureName)
        {
            myTexture = ResourceManager.RequestTexture(aTextureName);
        }
    }
}
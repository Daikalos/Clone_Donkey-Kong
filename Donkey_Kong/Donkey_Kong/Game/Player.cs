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
        private Animation 
            myWalkingAnimation,
            myDeathAnimation;

        private Vector2 
            myPosition,
            myDirection,
            myDestination;
        private Point mySize;
        private Rectangle myBoundingBox;
        private Color myMarioColor; //For invincibility
        private PlayerState myPlayerState;
        private SpriteEffects myFlipSprite;

        private int myHealth;
        private float
            mySpeed,
            myClimbSpeed,
            myVelocity,
            myGravity,
            myJumpHeight,
            myInvincibilityFrames;
        private bool
            myIsMoving,
            myJumpAvailable,
            myIsDead;

        public Vector2 Position
        {
            get => myPosition;
        }
        public int Health
        {
            get => myHealth;
        }

        public Player(Vector2 aPosition, Point aSize, int aHealth, float aSpeed, float aClimbSpeed, float aGravity, float aJumpHeight)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
            this.myHealth = aHealth;
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
            this.myMarioColor = Color.White;
            this.myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);
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
                    Jumping(aGameTime);
                    break;
                case PlayerState.isFalling:
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;
                    break;
                case PlayerState.isDead:

                    break;
            }

            Invincibility(aGameTime);
            CollisionChecking(aGameTime, aLevel);
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, new Point(myTexture.Width / 3, myTexture.Height), mySize, new Point(3, 1), 0.035f, myMarioColor, myFlipSprite, true);
                    }
                    else
                    {
                        aSpriteBatch.Draw(myTexture, myBoundingBox,
                            new Rectangle(myTexture.Width / 3, 0, myTexture.Width / 3, myTexture.Height), myMarioColor, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    }
                    break;
                case PlayerState.isClimbing:
                    aSpriteBatch.Draw(myTexture, myBoundingBox, null, myMarioColor, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    break;
                case PlayerState.isJumping:
                    aSpriteBatch.Draw(myTexture, myBoundingBox, null, myMarioColor, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    break;
                case PlayerState.isFalling:
                    aSpriteBatch.Draw(myTexture, myBoundingBox, null, myMarioColor, 0.0f, Vector2.Zero, myFlipSprite, 0.0f);
                    break;
                case PlayerState.isDead:

                    break;
            }
        }

        private void Movement(GameWindow aWindow, GameTime aGameTime, Level aLevel)
        {
            if (KeyMouseReader.KeyHold(Keys.Left) && OutsideBounds(aWindow, aLevel) != -1)
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - 40, myBoundingBox.Center.Y)).BoundingBox.Center.ToVector2();

                myFlipSprite = SpriteEffects.FlipHorizontally;
                myIsMoving = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Right) && OutsideBounds(aWindow, aLevel) != 1)
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + 40, myBoundingBox.Center.Y)).BoundingBox.Center.ToVector2();

                myFlipSprite = SpriteEffects.None;
                myIsMoving = true;
            }

            if (Math.Abs(myBoundingBox.Center.ToVector2().X - myDestination.X) > 1.0f)
            {
                myDirection.X = myDestination.X - myBoundingBox.Center.ToVector2().X;
                myDirection.Normalize();

                myPosition.X += myDirection.X * mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;

                ResourceManager.PlaySound("Walking");
            }
            else
            {
                myPosition.X = myDestination.X - mySize.X / 2;
                myIsMoving = false;

                ResourceManager.StopSound("Walking");
            }

            if (KeyMouseReader.KeyPressed(Keys.Space) && myJumpAvailable && aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).TileType != '.')
            {
                myPlayerState = PlayerState.isJumping;
                myVelocity = myJumpHeight;
                myJumpAvailable = false;

                ResourceManager.StopSound("Walking");
                ResourceManager.PlaySound("Jump");

                if (myIsMoving)
                {
                    for (int i = 4; i > 0; i--)
                    {
                        if (myFlipSprite == SpriteEffects.None)
                        {
                            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + (40 * (i - 1)), myBoundingBox.Center.Y + 40));
                            if (tempTile.TileType == '#' || tempTile.TileType == '%')
                            {
                                myDestination = tempTile.BoundingBox.Center.ToVector2();
                                break;
                            }
                        }
                        else
                        {
                            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - (40 * (i - 1)), myBoundingBox.Center.Y + 40));
                            if (tempTile.TileType == '#' || tempTile.TileType == '%')
                            {
                                myDestination = tempTile.BoundingBox.Center.ToVector2();
                                break;
                            }
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
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();
            }
            if (KeyMouseReader.KeyHold(Keys.Down) && aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).TileType != '#')
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).BoundingBox.Center.ToVector2();
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
        private void Jumping(GameTime aGameTime)
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
        private void Invincibility(GameTime aGameTime)
        {
            if (myInvincibilityFrames > 0)
            {
                myInvincibilityFrames -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
            else
            {
                myInvincibilityFrames = 0;
                myMarioColor = Color.White;
            }
        }

        private void CollisionChecking(GameTime aGameTime, Level aLevel)
        {
            IsFalling(aLevel);
            CollisionBlock(aGameTime, aLevel);
            CollisionLadder(aLevel);
            CollisionSprint(aLevel);
            CollisionItem(aLevel);
        }
        private void IsFalling(Level aLevel)
        {
            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40));
            if (Math.Abs(myBoundingBox.Center.X - tempTile.BoundingBox.Center.X) <= 1.0f && myPlayerState != PlayerState.isJumping)
            {
                if (tempTile.TileType == '.')
                {
                    myPosition.X = tempTile.Position.X;

                    myPlayerState = PlayerState.isFalling;
                    SetTexture("Mario_Jumping");
                }
            }
        }
        private void CollisionBlock(GameTime aGameTime, Level aLevel)
        {
            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40));
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
                    if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y)).TileType == '@')
                    {
                        if (KeyMouseReader.KeyHold(Keys.Up))
                        {
                            myPlayerState = PlayerState.isClimbing;
                            SetTexture("Mario_Jumping");
                        }
                    }
                    if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).TileType == '%')
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
                        Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40));
                        if (tempTile.TileType == '%' || tempTile.TileType == '#')
                        {
                            myPosition.Y = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40)).BoundingBox.Y - myBoundingBox.Height;
                            myJumpAvailable = true;

                            myPlayerState = PlayerState.isWalking;
                            SetTexture("Mario_Walking");
                        }
                    }
                }
            }
        }
        private void CollisionSprint(Level aLevel)
        {
            for (int i = 1; i <= 2; i++)
            {
                Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y + 40 * i));
                if (tempTile.TileType == '?')
                {
                    tempTile.TileType = '.';
                    tempTile.SetTexture();

                    ResourceManager.PlaySound("Item_Get");
                    GameInfo.AddScore(100);
                }
            }
        }
        private void CollisionItem(Level aLevel)
        {
            Tile tempTile = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y));
            if (tempTile.TileType == '/')
            {
                tempTile.TileType = '.';
                tempTile.SetTexture();

                ResourceManager.PlaySound("Item_Get");
                GameInfo.AddScore(100);
            }
        }
        private int OutsideBounds(GameWindow aWindow, Level aLevel)
        {
            if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X - 40, myBoundingBox.Center.Y)).Position == Vector2.Zero) //Will return myTiles[0, 0] position if outside grid
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();
                return -1;
            }
            if (aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X + 40, myBoundingBox.Center.Y)).Position == Vector2.Zero)
            {
                myDestination = aLevel.GetTileAtPos(new Vector2(myBoundingBox.Center.X, myBoundingBox.Center.Y - 40)).BoundingBox.Center.ToVector2();
                return 1;
            }
            if (myPosition.Y > aWindow.ClientBounds.Height) //Safety measure
            {
                myPosition = new Vector2(aWindow.ClientBounds.Width / 6, aWindow.ClientBounds.Height - 60);
                myPlayerState = PlayerState.isFalling;
            }
            return 0;
        }

        public void TakeDamage()
        {
            if (myInvincibilityFrames <= 0)
            {
                myHealth--;
                myMarioColor = new Color(0, 60, 120, 210.0f);
                myInvincibilityFrames = 2.0f;
            }
        }
        public void SetTexture(string aTextureName)
        {
            myTexture = ResourceManager.RequestTexture(aTextureName);
        }
    }
}
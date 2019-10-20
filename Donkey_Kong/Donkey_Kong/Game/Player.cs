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
        private Vector2 myPosition;
        private PlayerState myPlayerState;

        private Animation myWalkingAnimation;

        private Point mySize;
        private Rectangle myBoundingBox;
        private SpriteEffects myFlipSprite;
        private float
            mySpeed,
            myVelocity,
            myGravity,
            myJumpHeight;
        private bool
            myIsMoving,
            myJumpAvailable;

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aJumpHeight)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
            this.mySpeed = aSpeed;
            this.myGravity = aGravity;
            this.myJumpHeight = aJumpHeight;

            this.myIsMoving = false;
            this.myJumpAvailable = true;
            this.myWalkingAnimation = new Animation();
            this.myPlayerState = PlayerState.isFalling;
        }

        public void Update(GameTime aGameTime, Tile[,] someTiles)
        {
            myBoundingBox = new Rectangle((int)myPosition.X, (int)myPosition.Y, mySize.X, mySize.Y);

            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    Movement(aGameTime);
                    break;
                case PlayerState.isClimbing:

                    break;
                case PlayerState.isJumping:
                    myVelocity += myGravity;
                    myPosition.Y += myVelocity * (float)aGameTime.ElapsedGameTime.TotalSeconds;

                    if (myIsMoving)
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

            CollisionChecking(someTiles);
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

        private void Movement(GameTime aGameTime)
        {
            if (KeyMouseReader.KeyHold(Keys.Left))
            {
                myPosition.X -= mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;

                myFlipSprite = SpriteEffects.FlipHorizontally;
                myIsMoving = true;
            }
            if (KeyMouseReader.KeyHold(Keys.Right))
            {
                myPosition.X += mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds;

                myFlipSprite = SpriteEffects.None;
                myIsMoving = true;
            }
            if (!KeyMouseReader.KeyHold(Keys.Left) && !KeyMouseReader.KeyHold(Keys.Right))
            {
                myIsMoving = false;
            }

            if (KeyMouseReader.KeyPressed(Keys.Space) && myJumpAvailable)
            {
                myPlayerState = PlayerState.isJumping;
                myVelocity = myJumpHeight;
                myJumpAvailable = false;

                SetTexture("Mario_Jumping");
            }
        }

        private void CollisionChecking(Tile[,] someTiles)
        {
            IsFalling(someTiles);
            Collision(someTiles);
        }
        private void IsFalling(Tile[,] someTiles)
        {
            bool tempNoCollisionFound = false;
            bool tempBreakLoop = false;
            for (int i = 0; i < someTiles.GetLength(0); i++)
            {
                for (int j = 0; j < someTiles.GetLength(1); j++)
                {
                    if (someTiles[i, j].TileType == '#' || someTiles[i, j].TileType == '%')
                    {
                        Rectangle tempCollisionRect = new Rectangle((int)myPosition.X, (int)myPosition.Y + mySize.Y, mySize.X, mySize.Y / 6); //Custom hitbox for checking below
                        if (!tempCollisionRect.Intersects(someTiles[i, j].BoundingBox))
                        {
                            tempNoCollisionFound = true;
                        }
                        else
                        {
                            tempNoCollisionFound = false;
                            tempBreakLoop = true;
                            break;
                        }
                    }
                }
                if (tempBreakLoop)
                {
                    break;
                }
            }
            if (tempNoCollisionFound && myPlayerState != PlayerState.isJumping)
            {
                myPlayerState = PlayerState.isFalling;
                SetTexture("Mario_Jumping");
            }
        }
        private void Collision(Tile[,] someTiles)
        {
            for (int i = 0; i < someTiles.GetLength(0); i++)
            {
                for (int j = 0; j < someTiles.GetLength(1); j++)
                {
                    if (myBoundingBox.Intersects(someTiles[i, j].BoundingBox))
                    {
                        if (someTiles[i, j].TileType == '#' || someTiles[i, j].TileType == '%')
                        {
                            Rectangle tempIntersection = Rectangle.Intersect(myBoundingBox, someTiles[i, j].BoundingBox);
                            if (tempIntersection.Y == someTiles[i, j].BoundingBox.Y && tempIntersection.Width >= tempIntersection.Height) //Top
                            {
                                myVelocity = 0;
                                myPosition.Y = someTiles[i, j].BoundingBox.Y - myBoundingBox.Height;
                                myJumpAvailable = true;

                                myPlayerState = PlayerState.isWalking;
                                SetTexture("Mario_Walking");
                            }
                        }
                        if (someTiles[i, j].TileType == '@' && KeyMouseReader.KeyPressed(Keys.Up))
                        {
                            myPlayerState = PlayerState.isClimbing;
                        }
                    }
                }
            }
        }

        public void SetTexture(string aTextureName)
        {
            myTexture = ResourceManager.RequestTexture(aTextureName);
        }
    }
}
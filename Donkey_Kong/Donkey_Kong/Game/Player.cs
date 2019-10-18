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
    class Player
    {
        enum PlayerState
        {
            isWalking,
            isClimbing,
            isJumping,
            isDead
        }

        Texture2D myTexture;
        Vector2 myPosition;
        PlayerState myPlayerState;

        Animation myWalkingAnimation;

        Point mySize;
        float 
            mySpeed,
            myVelocity,
            myGravity,
            myJumpHeight;
        bool myIsMoving;
        SpriteEffects myFlipSprite;

        public Player(Vector2 aPosition, Point aSize, float aSpeed, float aGravity, float aJumpHeight)
        {
            this.myPosition = aPosition;
            this.mySize = aSize;
            this.mySpeed = aSpeed;
            this.myGravity = aGravity;
            this.myJumpHeight = aJumpHeight;

            myIsMoving = false;
            myWalkingAnimation = new Animation();
            myPlayerState = PlayerState.isWalking;
        }

        public void Update(GameTime aGameTime, Tile[,] someTiles)
        {
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
                            myPosition.X -= mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds * 0.7f;
                        }
                        else
                        {
                            myPosition.X += mySpeed * (float)aGameTime.ElapsedGameTime.TotalSeconds * 0.7f;
                        }
                    }
                    break;
                case PlayerState.isDead:

                    break;
            }

            for (int i = 0; i < someTiles.GetLength(0); i++)
            {
                for (int j = 0; j < someTiles.GetLength(1); j++)
                {

                }
            }
        }

        public void Draw(SpriteBatch aSpriteBatch, GameTime aGameTime)
        {
            switch (myPlayerState)
            {
                case PlayerState.isWalking:
                    if (myIsMoving)
                    {
                        myWalkingAnimation.DrawSpriteSheet(aSpriteBatch, aGameTime, myTexture, myPosition, myTexture.Width / 3, myTexture.Height, mySize.X, mySize.Y, 3, 1, 0.1f, myFlipSprite, true);
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

            if (KeyMouseReader.KeyPressed(Keys.Space))
            {
                myPlayerState = PlayerState.isJumping;
                myVelocity = myJumpHeight;

                SetTexture("Mario_Jumping");
            }
        }

        public void SetTexture(string aTextureName)
        {
            myTexture = ResourceManager.RequestTexture(aTextureName);
        }
    }
}

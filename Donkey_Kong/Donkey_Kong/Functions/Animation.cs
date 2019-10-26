using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    class Animation
    {
        int myCurrentFrame;
        Point myCurrentFramePos;
        bool myIsFinished;
        float myTimer;

        public bool IsFinished
        {
            get => myIsFinished;
            set => myIsFinished = value;
        }

        public Animation()
        {
            this.myCurrentFrame = 0;
            this.myIsFinished = false;
        }

        public void DrawSpriteSheet(SpriteBatch aSpriteBatch, GameTime aGameTime, Texture2D aTexture, Vector2 aPos, Point aFrameSize, Point aDestSize, Point aFrameAmount, float aAnimationSpeed, Color aColor, SpriteEffects aSpriteEffect, bool aLoop)
        {
            if (myIsFinished) return;

            myTimer += (float)aGameTime.ElapsedGameTime.TotalSeconds;
            if (myTimer > aAnimationSpeed) 
            {
                myCurrentFrame++;
                myCurrentFramePos.X++;
                if (myCurrentFrame >= (aFrameAmount.X * aFrameAmount.Y))
                {
                    if (aLoop)
                    {
                        myCurrentFrame = 0;
                        myCurrentFramePos = new Point(0, 0);
                    }
                    else
                    {
                        myCurrentFrame = (aFrameAmount.X * aFrameAmount.Y) - 1;
                        myIsFinished = true;
                    }
                }
                if (myCurrentFramePos.X >= aFrameAmount.X) //Animation
                {
                    myCurrentFramePos.Y++;
                    myCurrentFramePos.X = 0;
                }
                myTimer = 0;
            }

            aSpriteBatch.Draw(aTexture,
                new Rectangle((int)aPos.X, (int)aPos.Y, aDestSize.X, aDestSize.Y),
                new Rectangle(aFrameSize.X * myCurrentFramePos.X, aFrameSize.Y * myCurrentFramePos.Y, aFrameSize.X, aFrameSize.Y),
                aColor, 0.0f, Vector2.Zero, aSpriteEffect, 0.0f);
        }
    }
}

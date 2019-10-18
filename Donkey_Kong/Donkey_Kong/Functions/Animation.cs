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

        public void DrawSpriteSheet(SpriteBatch aSpriteBatch, GameTime aGameTime, Texture2D aTexture, Vector2 aPos, int aFrameWidth, int aFrameHeight, int aDestWidth, int aDestHeight, int someFramesInX, int someFramesInY, float aAnimationSpeed, SpriteEffects aSpriteEffect, bool aLoop)
        {
            if (myIsFinished) return;

            myTimer += (float)aGameTime.ElapsedGameTime.TotalSeconds;
            if (myTimer > aAnimationSpeed) 
            {
                myCurrentFrame++;
                myCurrentFramePos.X++;
                if (myCurrentFrame >= (someFramesInX * someFramesInY))
                {
                    if (aLoop)
                    {
                        myCurrentFrame = 0;
                        myCurrentFramePos = new Point(0, 0);
                    }
                    else
                    {
                        myCurrentFrame = (someFramesInX * someFramesInY) - 1;
                        myIsFinished = true;
                    }
                }
                if (myCurrentFramePos.X >= someFramesInX) //Animation
                {
                    myCurrentFramePos.Y++;
                    myCurrentFramePos.X = 0;
                }
                myTimer = 0;
            }

            aSpriteBatch.Draw(aTexture,
                new Rectangle((int)aPos.X, (int)aPos.Y, aDestWidth, aDestHeight),
                new Rectangle(aFrameWidth * myCurrentFramePos.X, aFrameHeight * myCurrentFramePos.Y, aFrameWidth, aFrameHeight),
                Color.White, 0.0f, Vector2.Zero, aSpriteEffect, 0.0f);
        }
    }
}

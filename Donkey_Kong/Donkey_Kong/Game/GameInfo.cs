using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    static class GameInfo
    {
        static Vector2 myDrawPos;
        static int 
            myScore,
            myDrawScore;
        static float 
            myDSTimer,
            myDSTimerMax; //Draw Score

        public static int Score
        {
            get => myScore;
        }
        public static Vector2 DrawPos
        {
            set => myDrawPos = value;
        }

        public static void Initialize(float aDSTimerMax)
        {
            myDSTimerMax = aDSTimerMax;

            myScore = 0;
            myDSTimer = 0;
            myDrawPos = Vector2.Zero;
        }

        public static void Update(GameTime aGameTime)
        {
            if (myDSTimer >= 0)
            {
                myDSTimer -= (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
        }

        public static void Draw(SpriteBatch aSpriteBatch, SpriteFont aFont)
        {
            if (myDSTimer >= 0)
            {
                StringManager.DrawStringMid(aSpriteBatch, aFont, myDrawScore.ToString(), myDrawPos, Color.White, 0.5f);
            }
        }

        public static void AddScore(Vector2 aPos, int someScore)
        {
            myDrawPos = new Vector2(aPos.X, aPos.Y - 40);
            myScore += someScore;
            myDrawScore = someScore;
            myDSTimer = myDSTimerMax;
        }
    }
}

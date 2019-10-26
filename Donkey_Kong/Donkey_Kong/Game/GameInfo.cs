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
        static int myScore;
        static float 
            myDSTimer,
            myDSTimerMax; //Draw Score

        public static int Score
        {
            get => myScore;
        }

        public static void Initialize(float aDSTimerMax)
        {
            myDSTimerMax = aDSTimerMax;

            myScore = 0;
            myDSTimer = 0;
        }

        public static void Update(GameTime aGameTime)
        {

        }

        public static void Draw(SpriteBatch aSpriteBatch)
        {

        }

        public static void AddScore(int someScore)
        {
            myScore += someScore;
        }
    }
}

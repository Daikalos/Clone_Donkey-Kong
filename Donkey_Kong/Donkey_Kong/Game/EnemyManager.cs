using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    static class EnemyManager
    {
        private static List<Enemy> myEnemies;
        private static Point myEnemySpeed;
        private static int myMaxEnemies;
        private static float
            mySpawnTimer,
            mySpawnTimerMax;

        public static void Initialize(Point aEnemySpeed, float aSpawnTimerMax, int someMaxEnemies)
        {
            mySpawnTimerMax = aSpawnTimerMax;
            myEnemySpeed = aEnemySpeed;
            myMaxEnemies = someMaxEnemies;

            mySpawnTimer = 0;
            myEnemies = new List<Enemy>();
        }

        public static void Update(GameWindow aWindow, GameTime aGameTime, Random aRNG, Player aPlayer)
        {
            if (myEnemies.Count < myMaxEnemies)
            {
                mySpawnTimer += (float)aGameTime.ElapsedGameTime.TotalSeconds;
            }
            if (mySpawnTimer >= mySpawnTimerMax)
            {
                mySpawnTimer = 0;
                AddEnemy(aWindow, aRNG, aPlayer);
            }

            for (int i = myEnemies.Count; i > 0; i--)
            {
                myEnemies[i - 1].Update(aGameTime, aRNG);
                if (!myEnemies[i - 1].IsAlive)
                {
                    myEnemies.RemoveAt(i - 1);
                }
            }
        }

        public static void Draw(SpriteBatch aSpriteBatch)
        {
            for (int i = myEnemies.Count; i > 0; i--)
            {
                myEnemies[i - 1].Draw(aSpriteBatch);
            }
        }

        public static void AddEnemy(GameWindow aWindow, Random aRNG, Player aPlayer)
        {
            float tempSpeed = aRNG.Next(myEnemySpeed.X, myEnemySpeed.Y);
            Vector2 tempSpawnPos = new Vector2((aWindow.ClientBounds.Width / 2) - 40, aWindow.ClientBounds.Height - (120 * (aRNG.Next(0, 4) + 1) + 60));

            if (Vector2.Distance(tempSpawnPos, aPlayer.Position) < 100.0f)
            {
                AddEnemy(aWindow, aRNG, aPlayer);
            }
            else
            {
                Enemy tempEnemy = new Enemy(tempSpawnPos, new Point(40), tempSpeed, 1.5f);
                tempEnemy.SetTexture("Enemy");
                myEnemies.Add(tempEnemy);
            }
        }
        public static void RemoveAll()
        {
            myEnemies.RemoveAll(x => x.IsAlive);
        }
    }
}

using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Donkey_Kong
{
    class EnemyManager
    {
        private List<Enemy> myEnemies;
        private int myMaxEnemies;
        private float
            mySpawnTimer,
            mySpawnTimerMax;

        public EnemyManager(float aSpawnTimerMax, int someMaxEnemies)
        {
            this.mySpawnTimerMax = aSpawnTimerMax;
            this.myMaxEnemies = someMaxEnemies;

            mySpawnTimer = 0;
            myEnemies = new List<Enemy>();
        }

        public void Update(GameWindow aWindow, GameTime aGameTime, Random aRNG, Level aLevel, Player aPlayer)
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
                myEnemies[i - 1].Update(aGameTime, aRNG, aLevel);
                if (!myEnemies[i - 1].IsAlive)
                {
                    myEnemies.RemoveAt(i - 1);
                }
            }
        }

        public void Draw(SpriteBatch aSpriteBatch)
        {
            for (int i = myEnemies.Count; i > 0; i--)
            {
                myEnemies[i - 1].Draw(aSpriteBatch);
            }
        }

        public void AddEnemy(GameWindow aWindow, Random aRNG, Player aPlayer)
        {
            float tempSpeed = aRNG.Next(120, 160);
            Vector2 tempSpawnPos = new Vector2((aWindow.ClientBounds.Width / 2) - 40, aWindow.ClientBounds.Height - (120 * (aRNG.Next(0, 4) + 1) + 60));

            if (Vector2.Distance(tempSpawnPos, aPlayer.Position) < 100.0f)
            {
                AddEnemy(aWindow, aRNG, aPlayer);
            }

            Enemy tempEnemy = new Enemy(tempSpawnPos, new Point(40), tempSpeed, 1.5f);
            tempEnemy.SetTexture("Enemy");
            myEnemies.Add(tempEnemy);
        }
    }
}

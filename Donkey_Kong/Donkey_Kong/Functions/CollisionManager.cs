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
    static class CollisionManager
    {
        public static bool CheckIfCollision(Rectangle aRectangle1, Rectangle aRectangle2) //Does not serve a direct purpose atm 
        {
            if (aRectangle1.Intersects(aRectangle2))
            {
                return true;
            }
            return false;
        }
    }
}

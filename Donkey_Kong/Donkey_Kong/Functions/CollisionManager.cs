using Microsoft.Xna.Framework;

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

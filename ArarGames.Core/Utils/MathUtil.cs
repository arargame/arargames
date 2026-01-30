using Microsoft.Xna.Framework;
using System;

namespace ArarGames.Core.Utils
{
    public static class MathUtil
    {
        public static float ToRadians(float degrees)
        {
            return degrees * (float)(Math.PI / 180.0);
        }

        public static float ToDegrees(float radians)
        {
            return radians * (float)(180.0 / Math.PI);
        }

        // Returns distance between two vectors
        public static float Distance(Vector2 a, Vector2 b)
        {
            return Vector2.Distance(a, b);
        }

        public static Vector2 Direction(Vector2 from, Vector2 to)
        {
            Vector2 dir = to - from;
            if (dir != Vector2.Zero) dir.Normalize();
            return dir;
        }
    }
}

using UnityEngine;

namespace VectorTools
{
    public static class ExtensionMethods
    {
        //Vector2 extension method that calculates cross product of two vectors
        public static float Cross(this Vector2 v1, Vector2 v2)
        {
            return v1.x * v2.y - v1.y * v2.x;
        }
    }
}
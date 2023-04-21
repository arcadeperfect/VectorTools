
using System.Collections.Generic;
using UnityEngine;

namespace VectorTools
{
    public static class Extensions
    {
        /// <summary>
        /// Extension method to lerp two arrays of vector2's
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2[] Lerp(this Vector2[] a, Vector2[] b, float t)
        {
            Vector2[] result = new Vector2[a.Length];
            for (int i = 0; i < a.Length; i++)
            {
                result[i] = Vector2.Lerp(a[i], b[i], t);
            }
            return result;
        }

        /// <summary>
        /// Extension method to lerp two lists of vector2's
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        
        public static List<Vector2> Lerp(this List<Vector2> a, List<Vector2> b, float t)
        {
            List<Vector2> result = new List<Vector2>();
            for (int i = 0; i < a.Count; i++)
            {
                result.Add(Vector2.Lerp(a[i], b[i], t));
            }
            return result;
        }
        
        
        
        /// <summary>
        /// Extension method to rotate a Vector2 in degrees
        /// </summary>
        /// <param name="v"></param>
        /// <param name="degrees"></param>
        /// <returns></returns>
        
        public static Vector2 RotateDegrees(this Vector2 v, float degrees)
        {
            float radians = -degrees * Mathf.Deg2Rad; // Negative sign for clockwise rotation

            float cosAngle = Mathf.Cos(radians);
            float sinAngle = Mathf.Sin(radians);

            float newX = v.x * cosAngle - v.y * sinAngle;
            float newY = v.x * sinAngle + v.y * cosAngle;

            return new Vector2(newX, newY);
        }
        
        
        /// <summary>
        /// Extension method to rotate a Vector2 in revolutions
        /// </summary>
        /// <param name="v"></param>
        /// <param name="rotation: 1 = a complete revolution"></param>
        /// <returns></returns>
        public static Vector2 RotateNormalized(this Vector2 v, float rotation)
        {
            float angleInDegrees = rotation * 360.0f;
            float angleInRadians = -angleInDegrees * Mathf.Deg2Rad; // Negative sign for clockwise rotation

            float cosAngle = Mathf.Cos(angleInRadians);
            float sinAngle = Mathf.Sin(angleInRadians);

            float newX = v.x * cosAngle - v.y * sinAngle;
            float newY = v.x * sinAngle + v.y * cosAngle;

            return new Vector2(newX, newY);
        }
        
        public static Vector2 RotateAroundDegrees(this Vector2 v, float rotation, Vector2 pivot)
        {
            return (v - pivot).RotateDegrees(rotation) + pivot;
        }
        
        /// <summary>
        /// Extension method to get the total distance of a path
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float TotalDistance(this Vector2[] input)
        {
            float totalDistance = 0;
            for (int i = 0; i < input.Length - 1; i++)
            {
                totalDistance += Vector2.Distance(input[i], input[i + 1]);
            }

            return totalDistance;
        }
        
        /// <summary>
        /// Extension method to get the total distance of a path
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static float TotalDistance(this List<Vector2> input)
        {
            float totalDistance = 0;
            for (int i = 0; i < input.Count - 1; i++)
            {
                totalDistance += Vector2.Distance(input[i], input[i + 1]);
            }

            return totalDistance;
        }
        
        public static Vector2 PointAlongPathAbsolute(this Vector2[] input, float t)
        {
            return Utilities.PointAlongPathAbsolute(input, t);
        }
        
        public static Vector2 PointAlongPathAbsolute(this List<Vector2> input, float t)
        {
            return Utilities.PointAlongPathAbsolute(input, t);
        }
        
    }
}
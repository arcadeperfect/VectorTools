// using System;
// using System.Collections.Generic;
// using System.Linq;
// using ProceduralToolkit.Samples.Primitives;
// using UnityEngine;
// using static VectorTools.Utilities;
//
// namespace VectorTools
// {
//     public class RiverUnwrap
//     {
//         public static Vector2[] UnWrap(Vector2[] input, int numSegments, float t1, float u)
//         {
//
//         }
//
//       
//         
//         private static float AlignAngle(Vector2 first, Vector2 second)
//         {
//             Vector2 delta = second - first;
//             float currentAngle = Mathf.Atan2(delta.y, delta.x);
//             float targetAngle = Mathf.Atan2(0, delta.x);
//             float angleDifference = targetAngle - currentAngle;
//             float angleInDegrees = angleDifference * Mathf.Rad2Deg;
//             return angleInDegrees;
//         }
//     }
// }
using System.Collections.Generic;
using Clipper2Lib;
using ProceduralToolkit.ClipperLib;
using UnityEngine;
using ClipperOffset = Clipper2Lib.ClipperOffset;

namespace VectorTools
{
    public class ClipperOffsetTools
    {
        public static int ClipperScale = 100000;
        
        
        // private Clipper2Lib.ClipperOffset _clipperOffset;
        
        // public ClipperOffset()
        // {
        //     _clipperOffset = new Clipper2Lib.ClipperOffset();
        // }
        
        
        public static Path64 ToPath64(Vector2[] path)
        {
            var path64 = new Path64(path.Length);
            foreach (var vertex in path)
            {
                path64.Add(ToPoint64(vertex));
            }
            return path64;
        }
        
        public static Path64 ToPath64(List<Vector2> path)
        {
            var path64 = new Path64(path.Count);
            foreach (var vertex in path)
            {
                path64.Add(ToPoint64(vertex));
            }
            return path64;
        }
        
        public static List<Vector2> ToVector2Path(Path64 path64)
        {
            return path64.ConvertAll(ToVector2);
        }
        
        public static Point64 ToPoint64(Vector2 vector2)
        {
            return new Point64(vector2.x * ClipperScale, vector2.y * ClipperScale);
        }
        
        public static Vector2 ToVector2(Point64 intPoint)
        {
            return new Vector2(intPoint.X / ClipperScale, intPoint.Y / ClipperScale);
        }
    }
}
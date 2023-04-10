using System;
using System.Collections.Generic;
using System.Drawing;
using UnityEngine;



/// <summary>
/// Very simple polyline offset algorithm
/// </summary>

class _PolylineOffset
{
    public static List<Vector2> OffsetPolyline(List<Vector2> input, float offset)
    {
        List<Vector2> output = new List<Vector2>();

        for (int i = 0; i < input.Count - 1; i++)
        {
            Vector2 A = input[i];
            Vector2 B = input[i + 1];

            _Vector2 AB = new _Vector2(B.x - A.x, B.y - A.y);
            _Vector2 normal = AB.Rotate(90).Normalized();

            Vector2 AOffset = A + normal * offset;
            Vector2 BOffset = B + normal * offset;

            output.Add(AOffset);

            if (i == input.Count - 2)
            {
                output.Add(BOffset);
            }
        }

        return output;
    }
    
    private struct _Vector2
    {
        public float X;
        public float Y;

        public _Vector2(float x, float y)
        {
            X = x;
            Y = y;
        }

        public float Magnitude() => (float)Math.Sqrt(X * X + Y * Y);

        public _Vector2 Normalized()
        {
            float mag = Magnitude();
            return new _Vector2(X / mag, Y / mag);
        }

        public _Vector2 Rotate(float degrees)
        {
            float radians = degrees * (float)Math.PI / 180;
            float sin = (float)Math.Sin(radians);
            float cos = (float)Math.Cos(radians);

            return new _Vector2(X * cos - Y * sin, X * sin + Y * cos);
        }

        public static _Vector2 operator *(_Vector2 a, float scalar) => new _Vector2(a.X * scalar, a.Y * scalar);
        public static Vector2 operator +(Vector2 a, _Vector2 b) => new Vector2(a.x + b.X, a.y + b.Y);
    }
    
}


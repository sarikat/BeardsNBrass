using System;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Nkg
{
    public static class NMath
    {
        static NMath()
        {
            squared = new List<int>();
        }

        public static int Clamp(int val, int min, int max)
        {
            return (val < min) ? min : (val > max) ? max : val;
        }

        public static float ManhattanDist(Vector3 l, Vector3 r)
        {
            return Mathf.Abs(l.x - r.x) + Mathf.Abs(l.y - r.y) + Mathf.Abs(l.z - r.z);
        }

        private static readonly int[] powersOfTwo = { 16, 8, 4, 2, 1 };
        public static int Int32pos_log2(int n)
        {
            if (n <= 0) return -1;
            int i = 0;
            for (int kIdx = 0; kIdx < powersOfTwo.Length; ++kIdx)
            {
                int k = powersOfTwo[kIdx];
                if (n >= (1 << k))
                {
                    i += k;
                    n >>= k;
                }
            }
            return i;
        }

        // Memoizes squared computations for hashtables with quadratic probing
        private static List<int> squared;
        public static int GetSquared(int i)
        {
            int count = squared.Count;
            while (i >= count)
            {
                squared.Add(count * count);
                count = squared.Count;
            }
            if (squared.Count > 100000)
            {
                throw new Exception("Likely error in a hashtable or sparseset");
            }
            return squared[i];
        }

        /// <summary>
        /// Returns true if point is within the circle defined by its center and a point on the edge.
        /// </summary>
        public static bool CircleContains(float2 circleCenter, float2 circleEdge, float2 point)
        {
            float2 pDistTemp = circleCenter - point;
            float pDist = pDistTemp.x * pDistTemp.x + pDistTemp.y * pDistTemp.y;
            float2 radTemp = circleCenter - circleEdge;
            float cRadius = radTemp.x * radTemp.x + radTemp.y * radTemp.y;
            //Debug.Log("pDistTemp: " + pDistTemp + ", pDist: " + pDist + ", radTemp: " + radTemp + ", cRadius: " + cRadius);
            return pDist < cRadius;
        }

        /// <summary>
        /// Rotate the specified vector by radians.
        /// </summary>
        /// <returns>The rotate.</returns>
        /// <param name="vector">Vector.</param>
        /// <param name="radians">Radians.</param>
        public static float2 Rotate(float2 vector, float radians)
        {
            float cs = Mathf.Cos(radians);
            float sn = Mathf.Sin(radians);
            float x = vector.x;
            float y = vector.y;
            return new float2(
                x * cs - y * sn,
                x * sn + y * cs
            );
        }

        public static Vector2 Rotate(Vector2 vector, float radians)
        {
            float cs = Mathf.Cos(radians);
            float sn = Mathf.Sin(radians);
            float x = vector.x;
            float y = vector.y;
            return new Vector2(
                x * cs - y * sn,
                x * sn + y * cs
            );
        }

        /// <summary>
        /// Finds the nearest larger power of10.
        /// </summary>
        /// <returns>The nearest larger power of10.</returns>
        /// <param name="number">Number.</param>
        public static ulong FindNearestLargerPowerOf10(int number)
        {
            uint zeroes = 0;
            while (number >= 10)
            {
                ++zeroes;
                number /= 10;
            }
            return (ulong)math.pow(10, zeroes + 1);
        }
    }
}

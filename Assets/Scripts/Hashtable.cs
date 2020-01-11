using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.Profiling;
using UnityEngine;

namespace Nkg
{

    public static class HashtableHelpers
    {
        public static readonly int[] Primes = {
            11,
            17,
            29,
            53,
            97,
            193,
            389,
            769,
            1543,
            3079,
            6151,
            12289,
            24593,
            49157,
            98317,
            196613,
            393241,
            786433,
            1572869,
            3145739,
            6291469,
            12582917,
            25165843,
            50331653,
            100663319,
            201326611,
            402653189,
            805306457,
            1610612741,
        };
    }
    /// <summary>
    /// Standard hashtable with linear probing and Robin hood hashing.
    /// Uses prime modulo.
    /// Power-of-two hashtable would be faster but can result in serious issues in 
    /// select cases, i.e. when all the values are multiples of 2, odd indices
    /// in the hashtable would never be used.
    /// </summary>
    public class Hashtable<TKey, TValue> where TKey : IEquatable<TKey>
    {
        public Hashtable(Func<TKey, int> hash_in)
        {

            // For power of two: 
            //curPrimeIdx = 3;
            //bitmask = (1 << curPrimeIdx) - 1;
            //modFactor = 1 << curPrimeIdx;

            // For prime modulo
            curPrimeIdx = 0;
            modFactor = HashtableHelpers.Primes[curPrimeIdx];
            maxProbeDepth = NMath.Int32pos_log2(modFactor);

            int initialLength = modFactor + maxProbeDepth;

            arr = new (TKey key, TValue value, sbyte distFromIdeal, sbyte status)[initialLength];
            numDeleted = 0;
            Count = 0;
            maxLoadFactor = 0.75F;
            hashFunc = hash_in;
        }

        public int Count { get; private set; }

        public TValue this[TKey key]
        {
            get { return arr[GetIdx(key)].value; }
            set
            {
                int index = GetIdx(key);
                arr[index] = (key, value, arr[index].distFromIdeal, -1);
            }
        }

        public void Add(TKey key, TValue value, int numTries = 0)
        {
            if (Count > maxLoadFactor * modFactor)
            {
                Resize();
            }

            int hash = hashFunc(key);

            // For prime modulo
            int idx = hash % modFactor;
            if (idx < 0)
            {
                throw new Exception("Hashtable.Add key error: hashFunc may not return a negative index.");
            }

            // For powers of two
            //int idx = hash & bitmask

            ++Count;
            for (int myDistFromIdeal = 0; myDistFromIdeal < maxProbeDepth; ++myDistFromIdeal, ++idx)
            {
                // if(idx < 0) throw Error // Caused by supplying negative key

                (_, _, sbyte thisDistFromIdeal, sbyte thisStatus) = arr[idx];
                if (thisStatus != -1)
                {
                    // Found a free spot 
                    arr[idx] = (key, value, (sbyte)myDistFromIdeal, -1);
                    return;
                }

                if (myDistFromIdeal > thisDistFromIdeal)
                {
                    // Swap and continue
                    (TKey tempKey, TValue tempValue, _, _) = arr[idx];
                    arr[idx] = (key, value, (sbyte)myDistFromIdeal, -1);
                    myDistFromIdeal = thisDistFromIdeal;
                    key = tempKey;
                    value = tempValue;
                }
            }

            // Exceeded max probing depth, resize and try again.
            // Make sure we haven't accidentally entered an infinite loop
            if (numTries > 10) throw new Exception("Internal error in hashtable. Could be caused by a terrible hashing function.");
            Resize();
            Add(key, value, numTries + 1);
        }

        public void Clear()
        {
            for (int i = 0; i < arr.Length; ++i)
            {
                arr[i].status = 0;
            }
            Count = 0;
            numDeleted = 0;
        }

        public bool ContainsKey(TKey key)
        {
            int idx = GetIdx(key);
            return idx >= 0;
        }

        /// <summary>
        /// Remove the key value pair. Item must exist in hashtable.
        /// </summary>
        /// <param name="key">Key.</param>
        public void Remove(TKey key)
        {
            int idx = GetIdx(key);
            RemoveAtIndex(idx);
        }

        /// <summary>
        /// Removes the key safely. If key doesn't exist, nothing happens.
        /// Faster than using both ContainsKey and Remove.
        /// </summary>
        /// <param name="key">Key.</param>
        public void TryRemoveKey(TKey key)
        {
            int idx = GetIdx(key);
            if (idx < 0) return;
            RemoveAtIndex(idx);
        }

        private void RemoveAtIndex(int idx)
        {
            arr[idx].status = 1;
            ++numDeleted;
            --Count;

            // If there are a lot of ghost elements, rehash the dictionary
            // This should happen extremely rarely, and in most cases never
            if (numDeleted > 0.3 * modFactor)
            {
                Resize(false);
            }
        }

        private int GetIdx(TKey key)
        {
            int hash = hashFunc(key);

            // For prime modulo
            int idx = hash % modFactor;

            // For powers of two
            //int idx = hash & bitmask;

            // Linear probing with maximum depth
            for (int curDistFromIdeal = 0; curDistFromIdeal < maxProbeDepth; ++curDistFromIdeal, ++idx)
            {
                sbyte status = arr[idx].status;
                if (status == 0) return -1; // Uninitialized
                if (status == -1 && arr[idx].key.Equals(key))
                {
                    return idx;
                }
            }
            return -1;
        }

        private void Resize(bool increaseSize = true, int numAttempts = 0)
        {
            int newModFactor = modFactor;
            int newMaxProbeDepth = maxProbeDepth;
            if (increaseSize)
            {
                // For prime modulo
                newModFactor = HashtableHelpers.Primes[++curPrimeIdx];

                // For powers of two
                //newModFactor = 1 << (++curPrimeIdx);

                newMaxProbeDepth = NMath.Int32pos_log2(newModFactor);
            }

            var newArr = new (TKey key, TValue value, sbyte distFromIdeal, sbyte status)[newModFactor + newMaxProbeDepth];

            // Rehash keys
            bool errorDuringResize = false;
            for (int idx = 0; idx < arr.Length; ++idx)
            {
                if (arr[idx].status != -1) continue;

                (TKey key, TValue value, sbyte distFromIdeal, _) = arr[idx];
                int hash = hashFunc(key);

                // For prime modulo
                int newIdx = hash % newModFactor;

                // For powers of two
                //int newIdx = hash & bitmask;

                // Linear probing with robin hood 
                bool foundASpot = false;
                for (int newDistFromIdeal = 0; newDistFromIdeal < newMaxProbeDepth; ++newDistFromIdeal, ++newIdx)
                {
                    (_, _, sbyte nextDistFromIdeal, sbyte nextStatus) = newArr[newIdx];

                    if (nextStatus != -1)
                    {
                        // Found a spot
                        newArr[newIdx] = (key, value, (sbyte)newDistFromIdeal, -1);
                        foundASpot = true;
                        break;
                    }

                    // If an existing element is closer to ideal than us, swap with them and continue
                    if (nextStatus == -1 && nextDistFromIdeal < newDistFromIdeal)
                    {
                        (TKey tempKey, TValue tempValue, _, _) = newArr[newIdx];
                        newArr[newIdx] = (key, value, (sbyte)newDistFromIdeal, -1);
                        key = tempKey;
                        value = tempValue;
                        newDistFromIdeal = nextDistFromIdeal;
                    }
                }

                if (!foundASpot)
                {
                    // Exceeded max probe depth, try again. This should be very rare but possible on occasion.
                    errorDuringResize = true;
                    break;
                }
            }

            if (errorDuringResize)
            {
                if (numAttempts > 5) throw new Exception("Hash table error: failed to resize. This should not happen.");
                Resize(true, numAttempts + 1);
            }
            else
            {
                arr = newArr;
                modFactor = newModFactor;
                maxProbeDepth = newMaxProbeDepth;
                numDeleted = 0;
            }
        }

        // status: In Use: -1, Uninitalized: 0, Deleted: 1
        private (TKey key, TValue value, sbyte distFromIdeal, sbyte status)[] arr;

        private readonly Func<TKey, int> hashFunc;

        private float maxLoadFactor;
        private int curPrimeIdx;
        private int numDeleted;

        /// <summary>
        /// If probing exceeds this, the hashtable will resize itself.
        /// </summary>
        private int maxProbeDepth;

        /// <summary>
        /// Either a prime number or power of two. The underlying array size is
        /// this plus the the maxProbeDepth;
        /// </summary>
        private int modFactor;

        // For power of two:
        //private int bitmask;

        private void PrintState()
        {
            UnityEngine.Debug.Log("curPrimeIdx: " + curPrimeIdx + ", modFactor: " + modFactor + ", maxProbeDepth: " + maxProbeDepth + ", arrlength: " + arr.Length + ", Count: " + Count + ", maxLoadFactor: " + maxLoadFactor);
            for (int i = 0; i < arr.Length; ++i)
            {
                (TKey k, TValue v, sbyte d, sbyte s) = arr[i];
                UnityEngine.Debug.Log("kvds: " + k + "," + v + "," + d + "," + s);
            }
            UnityEngine.Debug.Log("mlF * mF: " + (maxLoadFactor * modFactor) + ", count is greater? " + (Count > maxLoadFactor * modFactor));
        }
    }
}

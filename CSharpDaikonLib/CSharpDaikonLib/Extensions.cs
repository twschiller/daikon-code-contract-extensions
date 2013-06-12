using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

namespace CSharpDaikonLib
{
    public static class Extensions
    {
        /// <summary>
        /// Returns true if <paramref name="antecedent"/> is <code>false</code>, or if both arguments
        /// are <code>true</code>.
        /// </summary>
        /// <param name="antecedant">the antecedent of the implication</param>
        /// <param name="consequent">the consequent of the implication</param>
        /// <returns>true if <paramref name="antecedent"/> is <code>false</code>, or if both arguments are <code>true</code></returns>
        [Pure]
        public static bool Implies(this bool antecedent, Func<bool> consequent)
        {
            return !antecedent || consequent();
        }

        [Pure]
        private static bool BothNullOrEqual(object x, object y)
        {
            return (x == null && y == null) || (x != null && x.Equals(y));
        }

        /// <summary>
        /// Returns true if <paramref name="query"/> is equal to <paramref name="first"/> or one of <paramref name="rest"/>. 
        /// </summary>
        /// <typeparam name="T">the element type</typeparam>
        /// <param name="query">the query</param>
        /// <param name="first">the first object to check</param>
        /// <param name="rest">the other objects to check</param>
        /// <returns></returns>
        [Pure]
        public static bool OneOf<T>(this T query, T first, params T[] rest)
        {
            return BothNullOrEqual(query, first) || rest.Any(x => BothNullOrEqual(query, x));
        }

        /// <summary>
        /// Returns <code>true</code> if <paramref name="target"/> is a subsequence of <paramref name="parent"/>.
        /// </summary>
        /// <typeparam name="T">the element type</typeparam>
        /// <param name="target">the query subsequence</param>
        /// <param name="parent">the sequence</param>
        /// <returns><code>true</code> if <paramref name="target"/> is a subsequence of <paramref name="parent"/></returns>
        [Pure]
        public static bool IsSubsequence<T>(this IEnumerable<T> target, IEnumerable<T> parent)
        {
            // Taken from http://stackoverflow.com/questions/7324177/finding-a-subsequence-in-longer-sequence

            bool foundOneMatch = false;
            var enumeratedTarget = target.ToList();
            int enumPos = 0;

            using (IEnumerator<T> parentEnum = parent.GetEnumerator())
            {
                while (parentEnum.MoveNext())
                {
                    if (enumeratedTarget[enumPos].Equals(parentEnum.Current))
                    {
                        // Match, so move the target enum forward
                        foundOneMatch = true;
                        if (enumPos == enumeratedTarget.Count - 1)
                        {
                            // We went through the entire target, so we have a match
                            return true;
                        }

                        enumPos++;
                    }
                    else if (foundOneMatch)
                    {
                        foundOneMatch = false;
                        enumPos = 0;

                        if (enumeratedTarget[enumPos].Equals(parentEnum.Current))
                        {
                            foundOneMatch = true;
                            enumPos++;
                        }
                    }
                }

                return false;
            }
        }

        /// <summary>
        /// Returns <code>true</code> if the argument is a power of two. Zero is not considered to 
        /// be a power of two.
        /// </summary>
        /// <param name="x">a positive integer</param>
        /// <returns><code>true</code> if argument is a power of two</returns>
        [Pure]
        public static bool IsPowerOfTwo(this uint x)
        {
            if (x == 0)
            {
                return false;
            }
            else
            {
                return (x & (x - 1)) == 0;
            }
        }


        [Pure]
        public static bool IsPowerOfTwo(this int x)
        {
            return IsPowerOfTwo((uint) x);
        }

        [Pure]
        public static int Pow(this int x, int y)
        {
            return (int)Math.Pow((double)x, (double)y);
        }

        /// <summary>
        /// Returns the greatest common denominator of two positive integers.
        /// </summary>
        /// <param name="a">a positive integer</param>
        /// <param name="b">a positive integer</param>
        /// <remarks>Implementation uses Euclid's method</remarks>
        /// <returns>the greatest common denominator of two positive integers.</returns>
        [Pure]
        public static int GCD(int a, int b)
        {
            return b == 0 ? a : GCD(b, a % b);
        }

        /// <summary>
        /// Compare two enumerables lexically (element-by-element). If shared elements are the same, 
        /// but lengths differ the shorter enumerable is considered less.
        /// </summary>
        /// <typeparam name="T">a comparable element type</typeparam>
        /// <remarks><code>null</code> is considered the smallest element</remarks>
        /// <returns><code>true</code> if <paramref name="a"/> is strictly less than <paramref name="b"/></returns>
        [Pure]
        public static bool LexLT<T>(this IEnumerable<T> a, IEnumerable<T> b) where T : IComparable<T>
        {
            return LexHelper(a, b, false);
        }

        /// <summary>
        /// Compare two enumerables lexically (element-by-element). If shared elements are the same, 
        /// but lengths differ the shorter enumerable is considered less.
        /// </summary>
        /// <typeparam name="T">a comparable element type</typeparam>
        /// <remarks><code>null</code> is considered the smallest element</remarks>
        /// <returns><code>true</code> if <paramref name="a"/> is less than or equal to <paramref name="b"/></returns>
        [Pure]
        public static bool LexLTE<T>(this IEnumerable<T> a, IEnumerable<T> b) where T : IComparable<T>
        {
            return LexHelper(a, b, true);
        }

        /// <summary>
        /// Compare two enumerables lexically (element-by-element). If shared elements are the same, 
        /// but lengths differ the shorter enumerable is considered less.
        /// </summary>
        /// <typeparam name="T">a comparable element type</typeparam>
        /// <remarks><code>null</code> is considered the smallest element</remarks>
        /// <returns><code>true</code> if <paramref name="a"/> is strictly greater than <paramref name="b"/></returns>
        [Pure]
        public static bool LexGT<T>(this IEnumerable<T> a, IEnumerable<T> b) where T : IComparable<T>
        {
            a.All(x => x is string);
            // compute LT for swapped arguments
            return LexHelper(b, a, false);
        }

        /// <summary>
        /// Compare two enumerables lexically (element-by-element). If shared elements are the same, 
        /// but lengths differ the shorter enumerable is considered less.
        /// </summary>
        /// <typeparam name="T">a comparable element type</typeparam>
        /// <remarks><code>null</code> is considered the smallest element</remarks>
        /// <returns><code>true</code> if <paramref name="a"/> is greater than or equal to <paramref name="b"/></returns>
        [Pure]
        public static bool LexGTE<T>(this IEnumerable<T> a, IEnumerable<T> b) where T : IComparable<T>
        {
            // compute LTE for swapped arguments
            return LexHelper(b, a, true);
        }

        [Pure]
        private static bool LexHelper<T>(IEnumerable<T> a, IEnumerable<T> b, bool eq) where T : IComparable<T>
        {
            if (a == b)
            {
                return false;
            }
            else
            {
                using (IEnumerator<T> ae = a.GetEnumerator(), be = b.GetEnumerator())
                {
                    while (ae.MoveNext())
                    {
                        if (be.MoveNext())
                        {
                            // null is considered smaller than other elements
                            if ((ae.Current == null && ae.Current != null)
                                || ae.Current.CompareTo(be.Current) < 0)
                            {
                                return true;
                            }
                        }
                        else
                        {
                            // a has more elements
                            return false;
                        }
                    }

                    // a and b are equal, or b has more elements
                    if (be.MoveNext())
                    {
                        // b has more elements
                        return true;
                    }
                    else
                    {
                        // the lists are equal
                        return eq;
                    }
                }
            }
        }

        /// <summary>
        /// Performs a logical shift right wherein high-order empty bit positions are always set to zero.
        /// </summary>
        /// <remarks>See http://msdn.microsoft.com/en-us/library/aa691377(v=vs.71).aspx</remarks>
        [Pure]
        public static int UnsignedRightShift(this int x, int count)
        {
            return unchecked((int)(((uint)x) >> count));
        }

        /// <summary>
        /// Performs a logical shift right wherein high-order empty bit positions are always set to zero. Equivalent
        /// to x >> count.
        /// </summary>
        [Pure]
        public static uint UnsignedRightShift(this uint x, int count)
        {
            return x >> count;
        }

        /// <summary>
        /// Performs a logical shift right wherein high-order empty bit positions are always set to zero.
        /// </summary>
        /// <remarks>See http://msdn.microsoft.com/en-us/library/aa691377(v=vs.71).aspx</remarks>
        [Pure]
        public static long UnsignedRightShift(this long x, int count)
        {
            return unchecked((long)(((ulong)x) >> count));
        }

        /// <summary>
        /// Performs a logical shift right wherein high-order empty bit positions are always set to zero. Equivalent
        /// to x >> count.
        /// </summary>
        [Pure]
        public static ulong UnsignedRightShift(this ulong x, int count)
        {
            return x >> count;
        }
    }
}

// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.
// Copyright © Tanner Gooding and Contributors. Licensed under the MIT License (MIT). See License.md in the repository root for more information.
// Based on: https://github.com/terrafx/terrafx/blob/main/sources/Core/Utilities/MathUtilities.cs

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.Intrinsics;
using System.Runtime.Intrinsics.Arm;
using System.Runtime.Intrinsics.X86;

namespace XUI._Maths
{
    /// <summary>
    /// Defines a math helper class.
    /// </summary>
    public static class Maths
    {
        public static bool Is64BitProcess { get; } = Unsafe.SizeOf<nuint>() == 8;

        /// <summary>Gets a value used to represent all bits set.</summary>
        public static float AllBitsSet
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return BitConverter.UInt32BitsToSingle(0xFFFFFFFF);
            }
        }

        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const float ZeroTolerance = 1e-6f; // Value a 8x higher than 1.19209290E-07F

        /// <summary>
        /// The value for which all absolute numbers smaller than are considered equal to zero.
        /// </summary>
        public const double ZeroToleranceDouble = double.Epsilon * 8;

        /// <summary>
        /// Represents the mathematical constant e.
        /// </summary>
        public const float E = 2.71828175f;

        /// <summary>
        /// Represents the log base two of e.
        /// </summary>
        public const float Log2E = 1.442695f;

        /// <summary>
        /// Represents the log base ten of e.
        /// </summary>
        public const float Log10E = 0.4342945f;

        /// <summary>
        /// Represents the value of pi.
        /// </summary>
        public const float Pi = (float)System.Math.PI;

        /// <summary>
        /// Represents the value of pi times two.
        /// </summary>
        public const float TwoPi = (float)(2 * System.Math.PI);

        /// <summary>
        /// Represents the value of pi divided by two.
        /// </summary>
        public const float PiOver2 = (float)(System.Math.PI / 2);

        /// <summary>
        /// Represents the value of pi divided by four.
        /// </summary>
        public const float PiOver4 = (float)(System.Math.PI / 4);

        /// <summary>Compares two 32-bit floats to determine approximate equality.</summary>
        /// <param name="left">The float to compare with <paramref name="right" />.</param>
        /// <param name="right">The float to compare with <paramref name="left" />.</param>
        /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> differ by no more than <see cref="ZeroTolerance"/>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareEqual(float left, float right) => MathF.Abs(left - right) <= ZeroTolerance;

        /// <summary>Compares two 64-bit floats to determine approximate equality.</summary>
        /// <param name="left">The float to compare with <paramref name="right" />.</param>
        /// <param name="right">The float to compare with <paramref name="left" />.</param>
        /// <param name="epsilon">The maximum (inclusive) difference between <paramref name="left" /> and <paramref name="right" /> for which they should be considered equivalent.</param>
        /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> differ by no more than <paramref name="epsilon" />; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareEqual(double left, double right, double epsilon) => System.Math.Abs(left - right) <= epsilon;

        /// <summary>Compares two 32-bit floats to determine approximate equality.</summary>
        /// <param name="left">The float to compare with <paramref name="right" />.</param>
        /// <param name="right">The float to compare with <paramref name="left" />.</param>
        /// <param name="epsilon">The maximum (inclusive) difference between <paramref name="left" /> and <paramref name="right" /> for which they should be considered equivalent.</param>
        /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> differ by no more than <paramref name="epsilon" />; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareEqual(float left, float right, float epsilon) => MathF.Abs(left - right) <= epsilon;

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
        public static bool IsZero(float a) => MathF.Abs(a) < ZeroTolerance;

        /// <summary>
        /// Determines whether the specified value is close to zero (0.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to zero (0.0f); otherwise, <c>false</c>.</returns>
        public static bool IsZero(double a) => System.Math.Abs(a) < ZeroToleranceDouble;

        /// <summary>
        /// Determines whether the specified value is close to one (1.0f).
        /// </summary>
        /// <param name="a">The floating value.</param>
        /// <returns><c>true</c> if the specified value is close to one (1.0f); otherwise, <c>false</c>.</returns>
        public static bool IsOne(float a) => IsZero(a - 1.0f);

        /// <summary>
        /// Checks if a - b are almost equals within a float epsilon.
        /// </summary>
        /// <param name="a">The left value to compare.</param>
        /// <param name="b">The right value to compare.</param>
        /// <param name="epsilon">Epsilon value</param>
        /// <returns><c>true</c> if a almost equal to b within a float epsilon, <c>false</c> otherwise</returns>
        public static bool WithinEpsilon(float a, float b, float epsilon)
        {
            float diff = a - b;
            return -epsilon <= diff && diff <= epsilon;
        }

        /// <summary>
        /// Converts radians to degrees.
        /// </summary>
        /// <param name="radians">The angle in radians.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToDegrees(float radians) => radians * (180.0f / Pi);

        /// <summary>
        /// Converts degrees to radians.
        /// </summary>
        /// <param name="degree">Converts degrees to radians.</param>
        /// <returns>The converted value.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float ToRadians(float degree) => degree * (Pi / 180.0f);

        /// <summary>Clamps a 32-bit float to be between a minimum and maximum value.</summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (inclusive).</param>
        /// <returns><paramref name="value" /> clamped to be between <paramref name="min" /> and <paramref name="max" />.</returns>
        /// <remarks>This method does not account for <paramref name="min" /> being greater than <paramref name="max" />.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float Clamp(float value, float min, float max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        /// <summary>Clamps a 64-bit float to be between a minimum and maximum value.</summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (inclusive).</param>
        /// <returns><paramref name="value" /> clamped to be between <paramref name="min" /> and <paramref name="max" />.</returns>
        /// <remarks>This method does not account for <paramref name="min" /> being greater than <paramref name="max" />.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double Clamp(double value, double min, double max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        /// <summary>Clamps a 32-bit signed integer to be between a minimum and maximum value.</summary>
        /// <param name="value">The value to restrict.</param>
        /// <param name="min">The minimum value (inclusive).</param>
        /// <param name="max">The maximum value (inclusive).</param>
        /// <returns><paramref name="value" /> clamped to be between <paramref name="min" /> and <paramref name="max" />.</returns>
        /// <remarks>This method does not account for <paramref name="min" /> being greater than <paramref name="max" />.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static int Clamp(int value, int min, int max)
        {
            value = value > max ? max : value;
            value = value < min ? min : value;
            return value;
        }

        /// <summary>
        /// Performs smooth (cubic Hermite) interpolation between 0 and 1.
        /// </summary>
        /// <remarks>
        /// See https://en.wikipedia.org/wiki/Smoothstep
        /// </remarks>
        /// <param name="amount">Value between 0 and 1 indicating interpolation amount.</param>
        public static float SmoothStep(float amount)
        {
            return amount <= 0 ? 0 : amount >= 1 ? 1 : amount * amount * (3 - 2 * amount);
        }

        /// <summary>
        /// Linearly interpolates between two values.
        /// </summary>
        /// <param name="value1">Source value1.</param>
        /// <param name="value2">Source value2.</param>
        /// <param name="amount">Value between 0 and 1 indicating the weight of value2.</param>
        /// <returns>The result of linear interpolation of values based on the amount.</returns>
        public static float Lerp(float value1, float value2, float amount) => value1 + (value2 - value1) * amount;

        /// <summary>
        /// Calculates the absolute value of the difference of two values.
        /// </summary>
        /// <param name="value1">Source value1.</param>
        /// <param name="value2">Source value2.</param>
        /// <returns>The distance value.</returns>
        public static float Distance(float value1, float value2) => MathF.Abs(value1 - value2);

        /// <summary>
        /// Determines whether a given value is a power of two.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if <paramref name="value" /> is a power of two; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(uint value)
        {
            return BitOperations.IsPow2(value);
        }

        /// <summary>Determines whether a given value is a power of two.</summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if <paramref name="value" /> is a power of two; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(ulong value)
        {
            return BitOperations.IsPow2(value);
        }

        /// <summary>Determines whether a given value is a power of two.</summary>
        /// <param name="value">The value to check.</param>
        /// <returns><c>true</c> if <paramref name="value" /> is a power of two; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsPow2(nuint value)
        {
            return Is64BitProcess ? BitOperations.IsPow2(value) : BitOperations.IsPow2((uint)value);
        }

        // <summary>Rounds a given address down to the nearest alignment.</summary>
        /// <param name="address">The address to be aligned.</param>
        /// <param name="alignment">The target alignment, which should be a power of two.</param>
        /// <returns><paramref name="address" /> rounded down to the specified <paramref name="alignment" />.</returns>
        /// <remarks>This method does not account for an <paramref name="alignment" /> which is not a <c>power of two</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AlignDown(uint address, uint alignment)
        {
            System.Diagnostics.Debug.Assert(IsPow2(alignment));
            return address & ~(alignment - 1);
        }

        /// <summary>Rounds a given address down to the nearest alignment.</summary>
        /// <param name="address">The address to be aligned.</param>
        /// <param name="alignment">The target alignment, which should be a power of two.</param>
        /// <returns><paramref name="address" /> rounded down to the specified <paramref name="alignment" />.</returns>
        /// <remarks>This method does not account for an <paramref name="alignment" /> which is not a <c>power of two</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AlignDown(ulong address, ulong alignment)
        {
            System.Diagnostics.Debug.Assert(IsPow2(alignment));
            return address & ~(alignment - 1);
        }

        /// <summary>Rounds a given address down to the nearest alignment.</summary>
        /// <param name="address">The address to be aligned.</param>
        /// <param name="alignment">The target alignment, which should be a power of two.</param>
        /// <returns><paramref name="address" /> rounded down to the specified <paramref name="alignment" />.</returns>
        /// <remarks>This method does not account for an <paramref name="alignment" /> which is not a <c>power of two</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint AlignDown(nuint address, nuint alignment)
        {
            System.Diagnostics.Debug.Assert(IsPow2(alignment));
            return address & ~(alignment - 1);
        }

        // <summary>Rounds a given address up to the nearest alignment.</summary>
        /// <param name="address">The address to be aligned.</param>
        /// <param name="alignment">The target alignment, which should be a power of two.</param>
        /// <returns><paramref name="address" /> rounded up to the specified <paramref name="alignment" />.</returns>
        /// <remarks>This method does not account for an <paramref name="alignment" /> which is not a <c>power of two</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint AlignUp(uint address, uint alignment)
        {
            System.Diagnostics.Debug.Assert(IsPow2(alignment));

            return address + (alignment - 1) & ~(alignment - 1);
        }

        /// <summary>Rounds a given address up to the nearest alignment.</summary>
        /// <param name="address">The address to be aligned.</param>
        /// <param name="alignment">The target alignment, which should be a power of two.</param>
        /// <returns><paramref name="address" /> rounded up to the specified <paramref name="alignment" />.</returns>
        /// <remarks>This method does not account for an <paramref name="alignment" /> which is not a <c>power of two</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong AlignUp(ulong address, ulong alignment)
        {
            System.Diagnostics.Debug.Assert(IsPow2(alignment));

            return address + (alignment - 1) & ~(alignment - 1);
        }

        /// <summary>Rounds a given address up to the nearest alignment.</summary>
        /// <param name="address">The address to be aligned.</param>
        /// <param name="alignment">The target alignment, which should be a power of two.</param>
        /// <returns><paramref name="address" /> rounded up to the specified <paramref name="alignment" />.</returns>
        /// <remarks>This method does not account for an <paramref name="alignment" /> which is not a <c>power of two</c>.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static nuint AlignUp(nuint address, nuint alignment)
        {
            System.Diagnostics.Debug.Assert(IsPow2(alignment));

            return address + (alignment - 1) & ~(alignment - 1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static uint DivideByMultiple(uint value, uint alignment)
        {
            return (value + alignment - 1) / alignment;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ulong DivideByMultiple(ulong value, ulong alignment)
        {
            return (value + alignment - 1) / alignment;
        }

        /// <summary>
        /// Converts a float value from sRGB to linear.
        /// </summary>
        /// <param name="sRgbValue">The sRGB value.</param>
        /// <returns>A linear value.</returns>
        public static float SRgbToLinear(float sRgbValue)
        {
            if (sRgbValue <= 0.04045f)
                return sRgbValue / 12.92f;
            return MathF.Pow((sRgbValue + 0.055f) / 1.055f, 2.4f);
        }

        /// <summary>
        /// Converts a float value from linear to sRGB.
        /// </summary>
        /// <param name="linearValue">The linear value.</param>
        /// <returns>The encoded sRGB value.</returns>
        public static float LinearToSRgb(float linearValue)
        {
            if (linearValue <= 0.0031308f)
                return 12.92f * linearValue;

            return 1.055f * MathF.Pow(linearValue, 0.4166667f) - 0.055f;
        }
    }

    public static class MathsVec128
    {

        /// <summary>Gets a vector where the x-component is one and all other components are zero.</summary>
        public static Vector128<float> UnitX
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(1.0f, 0.0f, 0.0f, 0.0f);
            }
        }

        /// <summary>Gets a vector where the y-component is one and all other components are zero.</summary>
        public static Vector128<float> UnitY
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0.0f, 1.0f, 0.0f, 0.0f);
            }
        }

        /// <summary>Gets a vector where the z-component is one and all other components are zero.</summary>
        public static Vector128<float> UnitZ
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0.0f, 0.0f, 1.0f, 0.0f);
            }
        }

        /// <summary>Gets a vector where the w-component is one and all other components are zero.</summary>
        public static Vector128<float> UnitW
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0.0f, 0.0f, 0.0f, 1.0f);
            }
        }

        /// <summary>Gets a vector where all components are one.</summary>
        public static Vector128<float> One
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
#if NET8_0_OR_GREATER
                return Vector128<float>.One;
#else
            return Vector128.Create(1.0f, 1.0f, 1.0f, 1.0f);
#endif
            }
        }

        public static Vector128<float> OneHalf
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0.5f, 0.5f, 0.5f, 0.5f);
            }
        }

        public static Vector128<float> ByteMin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(-127.0f, -127.0f, -127.0f, -127.0f);
            }
        }

        public static Vector128<float> ByteMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(127.0f, 127.0f, 127.0f, 127.0f);
            }
        }

        public static Vector128<float> UByteMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(255.0f, 255.0f, 255.0f, 255.0f);
            }
        }

        public static Vector128<float> ScaleUByteN4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(255.0f, 255.0f * 256.0f * 0.5f, 255.0f * 256.0f * 256.0f, 255.0f * 256.0f * 256.0f * 256.0f * 0.5f);
            }
        }

        public static Vector128<int> MaskUByteN4
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0xFF, 0xFF << 8 - 1, 0xFF << 16, 0xFF << 24 - 1);
            }
        }

        public static Vector128<float> ShortMin
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(-32767.0f, -32767.0f, -32767.0f, -32767.0f);
            }
        }

        public static Vector128<float> ShortMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(32767.0f, 32767.0f, 32767.0f, 32767.0f);
            }
        }


        public static Vector128<float> UShortMax
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(65535.0f, 65535.0f, 65535.0f, 65535.0f);
            }
        }

        public static Vector128<int> AbsMask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0x7FFFFFFF, 0x7FFFFFFF, 0x7FFFFFFF, 0x7FFFFFFF);
            }
        }

        public static Vector128<uint> NegativeZero
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(0x80000000, 0x80000000, 0x80000000, 0x80000000);
            }
        }

        public static Vector128<float> NegativeOne
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(-1.0f, -1.0f, -1.0f, -1.0f);
            }
        }

        public static Vector128<float> NoFraction
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return Vector128.Create(8388608.0f, 8388608.0f, 8388608.0f, 8388608.0f);
            }
        }

        public static byte Shuffle(byte fp3, byte fp2, byte fp1, byte fp0)
        {
            return (byte)(fp3 << 6 | fp2 << 4 | fp1 << 2 | fp0);
        }

        /// <summary>Gets the x-component of the vector.</summary>
        /// <param name="self">The vector.</param>
        /// <returns>The x-component of <paramref name="self" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetX(this Vector128<float> self) => self.ToScalar();

        /// <summary>Gets the y-component of the vector.</summary>
        /// <param name="self">The vector.</param>
        /// <returns>The y-component of <paramref name="self" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetY(this Vector128<float> self) => self.GetElement(1);

        /// <summary>Gets the z-component of the vector.</summary>
        /// <param name="self">The vector.</param>
        /// <returns>The z-component of <paramref name="self" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetZ(this Vector128<float> self) => self.GetElement(2);

        /// <summary>Gets the w-component of the vector.</summary>
        /// <param name="self">The vector.</param>
        /// <returns>The w-component of <paramref name="self" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static float GetW(this Vector128<float> self) => self.GetElement(3);

        /// <summary>Compares two vectors to determine which elements equivalent.</summary>
        /// <param name="left">The vector to compare with <paramref name="right" />.</param>
        /// <param name="right">The vector to compare with <paramref name="left" />.</param>
        /// <returns>A vector that contains the element-wise comparison of <paramref name="left" /> and <paramref name="right" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> CompareEqual(Vector128<float> left, Vector128<float> right)
        {
            if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.CompareEqual(left, right);
            }
            else if (Sse.IsSupported)
            {
                return Sse.CompareEqual(left, right);
            }
            else
            {
                return SoftwareFallback(left, right);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    left.GetX() == right.GetX() ? Maths.AllBitsSet : 0.0f,
                    left.GetY() == right.GetY() ? Maths.AllBitsSet : 0.0f,
                    left.GetZ() == right.GetZ() ? Maths.AllBitsSet : 0.0f,
                    left.GetW() == right.GetW() ? Maths.AllBitsSet : 0.0f
                );
            }
        }

        /// <summary>Compares two vectors to determine approximate equality.</summary>
        /// <param name="left">The vector to compare with <paramref name="right" />.</param>
        /// <param name="right">The vector to compare with <paramref name="left" />.</param>
        /// <param name="epsilon">The maximum (inclusive) difference between <paramref name="left" /> and <paramref name="right" /> for which they should be considered equivalent.</param>
        /// <returns>A vector that contains the element-wise approximate comparison of <paramref name="left" /> and <paramref name="right" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> CompareEqual(Vector128<float> left, Vector128<float> right, Vector128<float> epsilon)
        {
            if (Sse.IsSupported)
            {
                Vector128<float> result = Sse.Subtract(left, right);
                result = Sse.And(result, Vector128.Create(0x7FFFFFFF).AsSingle());
                return Sse.CompareLessThanOrEqual(result, epsilon);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                Vector128<float> result = AdvSimd.Subtract(left, right);
                result = AdvSimd.Abs(result);
                return AdvSimd.CompareLessThanOrEqual(result, epsilon);
            }
            else
            {
                return SoftwareFallback(left, right, epsilon);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right, Vector128<float> epsilon)
            {
                return Vector128.Create(
                    MathF.Abs(left.GetX() - right.GetX()) <= epsilon.GetX() ? Maths.AllBitsSet : 0.0f,
                    MathF.Abs(left.GetY() - right.GetY()) <= epsilon.GetY() ? Maths.AllBitsSet : 0.0f,
                    MathF.Abs(left.GetZ() - right.GetZ()) <= epsilon.GetZ() ? Maths.AllBitsSet : 0.0f,
                    MathF.Abs(left.GetW() - right.GetW()) <= epsilon.GetW() ? Maths.AllBitsSet : 0.0f
                );
            }
        }

        /// <summary>Compares two vectors to determine equality.</summary>
        /// <param name="left">The vector to compare with <paramref name="right" />.</param>
        /// <param name="right">The vector to compare with <paramref name="left" />.</param>
        /// <returns><c>true</c> if <paramref name="left" /> and <paramref name="right" /> are equal; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareNotEqualAny(Vector128<float> left, Vector128<float> right)
        {
            if (Sse41.IsSupported)
            {
                Vector128<float> result = Sse.CompareNotEqual(left, right);
                return Sse.MoveMask(result) != 0x00;
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                Vector128<float> result = AdvSimd.CompareEqual(left, right);
                return AdvSimd.Arm64.MaxAcross(result).ToScalar() == 0;
            }
            else
            {
                return SoftwareFallback(left, right);
            }

            static bool SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return left.GetX() != right.GetX()
                    || left.GetY() != right.GetY()
                    || left.GetZ() != right.GetZ()
                    || left.GetW() != right.GetW();
            }
        }

        /// <summary>Computes the sum of two vectors.</summary>
        /// <param name="left">The vector to which to add <paramref name="right" />.</param>
        /// <param name="right">The vector which is added to <paramref name="left" />.</param>
        /// <returns>The sum of <paramref name="right" /> added to <paramref name="left" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Add(Vector128<float> left, Vector128<float> right)
        {
            if (Sse41.IsSupported)
            {
                return Sse.Add(left, right);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.Add(left, right);
            }
            else
            {
                return SoftwareFallback(left, right);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    left.GetX() + right.GetX(),
                    left.GetY() + right.GetY(),
                    left.GetZ() + right.GetZ(),
                    left.GetW() + right.GetW()
                );
            }
        }

        /// <summary>Computes the difference of two vectors.</summary>
        /// <param name="left">The vector from which to subtract <paramref name="right" />.</param>
        /// <param name="right">The vector which is subtracted from <paramref name="left" />.</param>
        /// <returns>The difference of <paramref name="right" /> subtracted from <paramref name="left" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Subtract(Vector128<float> left, Vector128<float> right)
        {
            if (Sse41.IsSupported)
            {
                return Sse.Subtract(left, right);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.Subtract(left, right);
            }
            else
            {
                return SoftwareFallback(left, right);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    left.GetX() - right.GetX(),
                    left.GetY() - right.GetY(),
                    left.GetZ() - right.GetZ(),
                    left.GetW() - right.GetW()
                );
            }
        }

        /// <summary>Computes the product of two vectors and then adds a third.</summary>
        /// <param name="addend">The vector which is added to the product of <paramref name="left" /> and <paramref name="right" />.</param>
        /// <param name="left">The vector to multiply by <paramref name="right" />.</param>
        /// <param name="right">The vector which is used to multiply <paramref name="left" />.</param>
        /// <returns>The sum of <paramref name="addend" /> and the product of <paramref name="left" /> multipled by <paramref name="right" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> MultiplyAdd(Vector128<float> addend, Vector128<float> left, Vector128<float> right)
        {
            if (Sse41.IsSupported)
            {
                Vector128<float> result = Sse.Multiply(left, right);
                return Sse.Add(addend, result);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                Vector128<float> result = AdvSimd.Multiply(left, right);
                return AdvSimd.Add(addend, result);
            }
            else
            {
                return SoftwareFallback(addend, left, right);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> addend, Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    addend.GetX() + left.GetX() * right.GetX(),
                    addend.GetY() + left.GetY() * right.GetY(),
                    addend.GetZ() + left.GetZ() * right.GetZ(),
                    addend.GetW() + left.GetW() * right.GetW()
                );
            }
        }

        /// <summary>Checks a vector to determine if all elements represent <c>true</c>.</summary>
        /// <param name="value">The vector to check.</param>
        /// <returns><c>true</c> if all elements in <paramref name="value" /> represent <c>true</c>; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool CompareTrueAll(Vector128<float> value)
        {
            if (Sse41.IsSupported)
            {
                return Sse.MoveMask(value) == 0x0F;
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.Arm64.MinAcross(value).ToScalar() != 0;
            }
            else
            {
                return SoftwareFallback(value.AsUInt32());
            }

            static bool SoftwareFallback(Vector128<uint> value)
            {
                return value.GetElement(0) != 0
                    && value.GetElement(1) != 0
                    && value.GetElement(2) != 0
                    && value.GetElement(3) != 0;
            }
        }

        /// <summary>Compares two vectors to determine which elements are lesser.</summary>
        /// <param name="left">The vector to compare with <paramref name="right" />.</param>
        /// <param name="right">The vector to compare with <paramref name="left" />.</param>
        /// <returns>A vector that contains the element-wise comparison of <paramref name="left" /> and <paramref name="right" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> CompareLessThan(Vector128<float> left, Vector128<float> right)
        {
            if (Sse41.IsSupported)
            {
                return Sse.CompareLessThan(left, right);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.CompareLessThan(left, right);
            }
            else
            {
                return SoftwareFallback(left, right);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    left.GetX() < right.GetX() ? Maths.AllBitsSet : 0.0f,
                    left.GetY() < right.GetY() ? Maths.AllBitsSet : 0.0f,
                    left.GetZ() < right.GetZ() ? Maths.AllBitsSet : 0.0f,
                    left.GetW() < right.GetW() ? Maths.AllBitsSet : 0.0f
                );
            }
        }

        /// <summary>Compares two vectors to determine which elements are lesser or equivalent.</summary>
        /// <param name="left">The vector to compare with <paramref name="right" />.</param>
        /// <param name="right">The vector to compare with <paramref name="left" />.</param>
        /// <returns>A vector that contains the element-wise comparison of <paramref name="left" /> and <paramref name="right" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> CompareLessThanOrEqual(Vector128<float> left, Vector128<float> right)
        {
            if (Sse41.IsSupported)
            {
                return Sse.CompareLessThanOrEqual(left, right);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.CompareLessThanOrEqual(left, right);
            }
            else
            {
                return SoftwareFallback(left, right);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> left, Vector128<float> right)
            {
                return Vector128.Create(
                    left.GetX() <= right.GetX() ? Maths.AllBitsSet : 0.0f,
                    left.GetY() <= right.GetY() ? Maths.AllBitsSet : 0.0f,
                    left.GetZ() <= right.GetZ() ? Maths.AllBitsSet : 0.0f,
                    left.GetW() <= right.GetW() ? Maths.AllBitsSet : 0.0f
                );
            }
        }

        /// <summary>Compares two vectors to determine which elements are lesser or equivalent.</summary>
        /// <param name="left">The vector to compare with <paramref name="right" />.</param>
        /// <param name="right">The vector to compare with <paramref name="left" />.</param>
        /// <returns>A vector that contains the element-wise comparison of <paramref name="left" /> and <paramref name="right" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool LessThanOrEqual(Vector128<float> left, Vector128<float> right)
        {
            return CompareTrueAll(CompareLessThanOrEqual(left, right));
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Round(Vector128<float> vector)
        {
            if (Sse41.IsSupported)
            {
                return Sse41.RoundToNearestInteger(vector);
            }
            else if (Sse.IsSupported)
            {
                Vector128<float> sign = Sse.And(vector, NegativeZero.AsSingle());
                Vector128<float> sMagic = Sse.Or(NoFraction, sign);
                Vector128<float> R1 = Sse.Add(vector, sMagic);
                R1 = Sse.Subtract(R1, sMagic);
                Vector128<float> R2 = Sse.And(vector, AbsMask.AsSingle());
                Vector128<float> mask = Sse.CompareLessThanOrEqual(R2, NoFraction);
                R2 = Sse.AndNot(mask, vector);
                R1 = Sse.And(R1, mask);
                Vector128<float> result = Sse.Xor(R1, R2);
                return result;
            }
            else if (AdvSimd.IsSupported)
            {
                return AdvSimd.RoundToNearest(vector);
            }
            else
            {
                return SoftwareFallback(vector);
            }

            static Vector128<float> SoftwareFallback(Vector128<float> vector)
            {
                return Vector128.Create(
                    MathF.Round(vector.GetX()),
                    MathF.Round(vector.GetY()),
                    MathF.Round(vector.GetZ()),
                    MathF.Round(vector.GetW())
                );
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Truncate(Vector128<float> vector)
        {
            if (Sse.IsSupported)
            {
                return Sse41.RoundToZero(vector);
            }
            else if (AdvSimd.IsSupported)
            {
                return AdvSimd.RoundToZero(vector);
            }
            else
            {
                return Clamp(vector, Vector128<float>.Zero, One);
            }
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Clamp(Vector128<float> vector, Vector128<float> min, Vector128<float> max)
        {
            System.Diagnostics.Debug.Assert(LessThanOrEqual(min, max));

            Vector128<float> result = Vector128.Max(min, vector);
            result = Vector128.Min(max, result);
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> Saturate(Vector128<float> vector)
        {
            return Clamp(vector, Vector128<float>.Zero, One);
        }

        /// <summary>
        /// Replicate the X component of the vector.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> VectorSplatX(Vector128<float> value)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, 0b00_00_00_00);
            }
            else if (Sse41.IsSupported)
            {
                return Sse.Shuffle(value, value, 0b00_00_00_00);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.DuplicateSelectedScalarToVector128(value, 0);
            }
            else
            {
                float x = value.GetX();
                return Vector128.Create(x);
            }
        }

        /// <summary>
        /// Replicate the Y component of the vector.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> VectorSplatY(Vector128<float> value)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, 0b01_01_01_01);
            }
            else if (Sse41.IsSupported)
            {
                return Sse.Shuffle(value, value, 0b01_01_01_01);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.DuplicateSelectedScalarToVector128(value, 1);
            }
            else
            {
                float y = value.GetY();
                return Vector128.Create(y);
            }
        }

        /// <summary>
        /// Replicate the Z component of the vector.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> VectorSplatZ(Vector128<float> value)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, 0b10_10_10_10);
            }
            else if (Sse41.IsSupported)
            {
                return Sse.Shuffle(value, value, 0b10_10_10_10);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.DuplicateSelectedScalarToVector128(value, 2);
            }
            else
            {
                float z = value.GetZ();
                return Vector128.Create(z);
            }
        }

        /// <summary>
        /// Replicate the W component of the vector.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Vector128<float> VectorSplatW(Vector128<float> value)
        {
            if (Avx.IsSupported)
            {
                return Avx.Permute(value, 0b11_11_11_11);
            }
            else if (Sse41.IsSupported)
            {
                return Sse.Shuffle(value, value, 0b11_11_11_11);
            }
            else if (AdvSimd.Arm64.IsSupported)
            {
                return AdvSimd.DuplicateSelectedScalarToVector128(value, 3);
            }
            else
            {
                float w = value.GetW();
                return Vector128.Create(w);
            }
        }
    }
}
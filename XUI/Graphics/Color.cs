﻿// Copyright (c) Amer Koleci and contributors.
// Licensed under the MIT License (MIT). See LICENSE in the repository root for more information.

using System;
using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Runtime.Intrinsics;
using XUI._Maths;

namespace XUI.Graphics
{
    /// <summary>
    /// Represents a 32-bit RGBA color (4 bytes).
    /// </summary>
    /// <remarks>Equivalent of XMUBYTEN4.</remarks>
    [StructLayout(LayoutKind.Explicit)]
    public readonly struct Color : IPackedVector<uint>, IEquatable<Color>
    {
        [FieldOffset(0)]
        private readonly uint _packedValue;

        /// <summary>
        /// The red component of the color.
        /// </summary>
        [FieldOffset(0)]
        public readonly byte R;

        /// <summary>
        /// The green component of the color.
        /// </summary>
        [FieldOffset(1)]
        public readonly byte G;

        /// <summary>
        /// The blue component of the color.
        /// </summary>
        [FieldOffset(2)]
        public readonly byte B;

        /// <summary>
        /// The alpha component of the color.
        /// </summary>
        [FieldOffset(3)]
        public readonly byte A;

        /// <summary>
        /// Gets or Sets the current color as a packed value.
        /// </summary>
        public uint PackedValue => _packedValue;

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="packedValue">The packed value to assign.</param>
        public Color(uint packedValue)
        {
            _packedValue = packedValue;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="value">The value that will be assigned to all components.</param>
        public Color(float value)
            : this(value, value, value, value)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="r">Red component.</param>
        /// <param name="g">Green component.</param>
        /// <param name="b">Blue component.</param>
        /// <param name="a">Alpha component.</param>
        public Color(float r, float g, float b, float a = 1.0f)
        {
            Vector128<float> result = MathsVec128.Saturate(Vector128.Create(r, g, b, a));
            result = Vector128.Multiply(result, MathsVec128.UByteMax);
            result = MathsVec128.Truncate(result);

            R = (byte)result.GetX();
            G = (byte)result.GetY();
            B = (byte)result.GetZ();
            A = (byte)result.GetW();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Passed values are clamped within byte range.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        public Color(byte red, byte green, byte blue)
        {
            R = red;
            G = green;
            B = blue;
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Passed values are clamped within byte range.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color</param>
        public Color(byte red, byte green, byte blue, byte alpha)
        {
            R = red;
            G = green;
            B = blue;
            A = alpha;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Passed values are clamped within byte range.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        public Color(int red, int green, int blue)
        {
            R = PackHelpers.ToByte(red);
            G = PackHelpers.ToByte(green);
            B = PackHelpers.ToByte(blue);
            A = 255;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.  Passed values are clamped within byte range.
        /// </summary>
        /// <param name="red">The red component of the color.</param>
        /// <param name="green">The green component of the color.</param>
        /// <param name="blue">The blue component of the color.</param>
        /// <param name="alpha">The alpha component of the color</param>
        public Color(int red, int green, int blue, int alpha)
        {
            R = PackHelpers.ToByte(red);
            G = PackHelpers.ToByte(green);
            B = PackHelpers.ToByte(blue);
            A = PackHelpers.ToByte(alpha);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="vector">The red, green, and blue components of the color.</param>
        /// <param name="alpha">The alpha component of the color.</param>
        public Color(in Vector3 vector, float alpha = 1.0f)
            : this(vector.X, vector.Y, vector.Z, alpha)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Color"/> struct.
        /// </summary>
        /// <param name="vector">A four-component color.</param>
        public Color(Vector4 vector)
            : this(vector.X, vector.Y, vector.Z, vector.W)
        {
        }

        public readonly void Deconstruct(out byte red, out byte green, out byte blue, out byte alpha)
        {
            red = R;
            green = G;
            blue = B;
            alpha = A;
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public int ToBgra()
        {
            int value = B;
            value |= G << 8;
            value |= R << 16;
            value |= A << 24;

            return value;
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public int ToRgba()
        {
            int value = R;
            value |= G << 8;
            value |= B << 16;
            value |= A << 24;
            return value;
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public int ToArgb()
        {
            int value = A;
            value |= R << 8;
            value |= G << 16;
            value |= B << 24;

            return value;
        }

        /// <summary>
        /// Converts the color into a packed integer.
        /// </summary>
        /// <returns>A packed integer containing all four color components.</returns>
        public int ToAbgr()
        {
            int value = A;
            value |= B << 8;
            value |= G << 16;
            value |= R << 24;

            return value;
        }

        /// <summary>
        /// Converts the color into a three component vector.
        /// </summary>
        /// <returns>A three component vector containing the red, green, and blue components of the color.</returns>
        public Vector3 ToVector3()
        {
            return new Vector3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        /// <summary>
        /// Converts the color into a three component color.
        /// </summary>
        /// <returns>A three component color containing the red, green, and blue components of the color.</returns>
        public Color3 ToColor3()
        {
            return new Color3(R / 255.0f, G / 255.0f, B / 255.0f);
        }

        /// <summary>Reinterprets the current instance as a new <see cref="Vector128{Single}" />.</summary>
        /// <returns>The current instance reinterpreted as a new <see cref="Vector128{Single}" />.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Vector128<float> AsVector128() => Vector128.Create(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);

        /// <summary>
        /// Gets a four-component vector representation for this object.
        /// </summary>
        public Vector4 ToVector4()
        {
            return new(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }

        /// <summary>
        /// Convert this instance to a <see cref="Color4"/>
        /// </summary>
        public Color4 ToColor4()
        {
            return new(R / 255.0f, G / 255.0f, B / 255.0f, A / 255.0f);
        }

        /// <summary>
        /// Performs an implicit conversion from <see cref="Color"/> to <see cref="Color4"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color4(Color value) => value.ToColor4();

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector3"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color(in Vector3 value) => new(value.X, value.Y, value.Z, 1.0f);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Vector4"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color(in Vector4 value) => new(value.X, value.Y, value.Z, value.W);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Color4"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color(in Color4 value) => new(value.R, value.G, value.B, value.A);

        /// <summary>
        /// Performs an explicit conversion from <see cref="Color"/> to <see cref="int"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator int(in Color value) => value.ToRgba();

        /// <summary>
        /// Performs an explicit conversion from <see cref="int"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static explicit operator Color(int value) => new(value);

        /// <summary>
        /// Performs an explicit conversion from <see cref="int"/> to <see cref="Color"/>.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The result of the conversion.</returns>
        public static implicit operator Color(uint value) => new(value);

        /// <inheritdoc/>
        public override bool Equals(object obj) => obj is Color color && Equals(ref color);

        /// <summary>
        /// Determines whether the specified <see cref="Color"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare with this instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(Color other) => Equals(ref other);

        /// <summary>
        /// Determines whether the specified <see cref="Color"/> is equal to this instance.
        /// </summary>
        /// <param name="other">The <see cref="Color"/> to compare with this instance.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool Equals(ref Color other)
        {
            return R == other.R
                && G == other.G
                && B == other.B
                && A == other.A;
        }

        /// <summary>
        /// Compares two <see cref="Color"/> objects for equality.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Color"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is equal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator ==(Color left, Color right) => left.Equals(ref right);

        /// <summary>
        /// Compares two <see cref="Color"/> objects for inequality.
        /// </summary>
        /// <param name="left">The <see cref="Color"/> on the left hand of the operand.</param>
        /// <param name="right">The <see cref="Color"/> on the right hand of the operand.</param>
        /// <returns>
        /// True if the current left is unequal to the <paramref name="right"/> parameter; otherwise, false.
        /// </returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool operator !=(Color left, Color right) => !left.Equals(ref right);

        /// <inheritdoc/>
        public override int GetHashCode() => PackedValue.GetHashCode();

        /// <inheritdoc/>
        public override string ToString()
        {
            return $"R={R}, G={G}, B={B}, A={A}";
        }
    }
}
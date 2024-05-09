using SkiaSharp;
using System;

namespace GeneXus.Drawing.Common;

[Serializable]
public struct Size : IEquatable<Size>
{
	internal SKSize m_size;

	private Size(SKSize size)
	{
		m_size = size;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Size'/> class from the specified dimensions.
	/// </summary>
	public Size(float width, float height)
		: this(new SKSize(width, height)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Size'/> class from the specified
	/// <see cref='Point'/>.
	/// </summary>
	public Size(Point pt)
		: this(pt.X, pt.Y) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Size'/>.
	/// </summary>
	public override readonly string ToString() => $"{{Width={Width},Height={Height}}}";


	#region Operators

	/// <summary>
	/// Converts the specified <see cref='Size'/> to a <see cref='SKSize'/>.
	/// </summary>
	public static explicit operator SKSize(Size point) => point.m_size;

	/// <summary>
	/// Converts the specified <see cref='Size'/> to a <see cref='Point'/>.
	/// </summary>
	public static explicit operator Point(Size size) => new(size.Width, size.Height);

	/// <summary>
	/// Tests whether two <see cref='Size'/> objects are identical.
	/// </summary>
	public static bool operator ==(Size sz1, Size sz2) => sz1.m_size == sz2.m_size;

	/// <summary>
	/// Tests whether two <see cref='Size'/> objects are different.
	/// </summary>
	public static bool operator !=(Size sz1, Size sz2) => sz1.m_size != sz2.m_size;

	/// <summary>
	/// Performs vector addition of two <see cref='Size'/> objects.
	/// </summary>
	public static Size operator +(Size sz1, Size sz2) => Add(sz1, sz2);

	/// <summary>
	/// Contracts a <see cref='Size'/> by another <see cref='Size'/>
	/// </summary>
	public static Size operator -(Size sz1, Size sz2) => Subtract(sz1, sz2);

	/// <summary>
	/// Multiplies <see cref="Size"/> by an <see cref="float"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="left">Multiplicand of type <see cref="Size"/>.</param>
	/// <param name="right">Multiplier of type <see cref="float"/>.</param>
	/// <returns>Product of type <see cref="Size"/>.</returns>
	public static Size operator *(Size left, float right) => Multiply(left, right);

	/// <summary>
	/// Multiplies a <see cref="Size"/> by an <see cref="float"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="left">Multiplier of type <see cref="float"/>.</param>
	/// <param name="right">Multiplicand of type <see cref="Size"/>.</param>
	/// <returns>Product of type <see cref="Size"/>.</returns>
	public static Size operator *(float left, Size right) => Multiply(right, left);

	/// <summary>
	/// Divides <see cref="Size"/> by an <see cref="float"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="left">Dividend of type <see cref="Size"/>.</param>
	/// <param name="right">Divisor of type <see cref="float"/>.</param>
	/// <returns>Result of type <see cref="Size"/>.</returns>
	public static Size operator /(Size left, float right) => Divide(left, right);

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='Size'/> has the same dimensions
	/// as this Size.
	/// </summary>
	public readonly bool Equals(Size other) => m_size == other.m_size;

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='Size'/> with the same dimensions
	/// as this Size.
	/// </summary>
	public override readonly bool Equals(object obj) => m_size.Equals(obj);

	/// <summary>
	/// Returns a hash code.
	/// </summary>
	public override readonly int GetHashCode() => m_size.GetHashCode();

	#endregion


	#region Fields

	/// <summary>
	/// Initializes a new instance of the <see cref='Size'/> class.
	/// </summary>
	public static readonly Size Empty = default;

	#endregion


	#region Properties

	/// <summary>
	/// Represents the horizontal component of this <see cref='Size'/>.
	/// </summary>
	public float Width
	{
		readonly get => m_size.Width;
		set => m_size.Width = value;
	}

	/// <summary>
	/// Represents the vertical component of this <see cref='Size'/>.
	/// </summary>
	public float Height
	{
		readonly get => m_size.Height;
		set => m_size.Height = value;
	}

	/// <summary>
	/// Tests whether this <see cref='Size'/> has zero width and height.
	/// </summary>
	public readonly bool IsEmpty => m_size.IsEmpty;

	#endregion


	#region Methods

	/// <summary>
	/// Performs vector addition of two <see cref='Size'/> objects.
	/// </summary>
	public static Size Add(Size sz1, Size sz2) => new(sz1.m_size + sz2.m_size);

	/// <summary>
	/// Performs vector substraction of two <see cref='Size'/> objects.
	/// </summary>
	public static Size Subtract(Size sz1, Size sz2) => new(sz1.m_size - sz2.m_size);

	/// <summary>
	/// Converts a <see cref='Size'/> by performing a ceiling operation on all the coordinates.
	/// </summary>
	public static Size Ceiling(Size value) => new(unchecked((int)Math.Ceiling(value.Width)), unchecked((int)Math.Ceiling(value.Height)));

	/// <summary>
	/// Converts a <see cref='Size'/> by performing a truncate operation on all the coordinates.
	/// </summary>
	public static Size Truncate(Size value) => new(unchecked((int)value.Width), unchecked((int)value.Height));

	/// <summary>
	/// Converts a <see cref='Size'/> by performing a round operation on all the coordinates.
	/// </summary>
	public static Size Round(Size value) => new(unchecked((int)Math.Round(value.Width)), unchecked((int)Math.Round(value.Height)));

	/// <summary>
	/// Multiplies <see cref="Size"/> by an <see cref="float"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="size">Multiplicand of type <see cref="Size"/>.</param>
	/// <param name="multiplier">Multiplier of type <see cref='float'/>.</param>
	/// <returns>Product of type <see cref="Size"/>.</returns>
	public static Size Multiply(Size size, float multiplier) => new(unchecked(size.Width * multiplier), unchecked(size.Height * multiplier));

	/// <summary>
	/// Divides <see cref="Size"/> by an <see cref="float"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="size">Divident of type <see cref="Size"/>.</param>
	/// <param name="denominator">Denominator of type <see cref='float'/>.</param>
	/// <returns>Quotient of type <see cref="Size"/>.</returns>
	public static Size Divide(Size size, float denominator) => new(unchecked(size.Width / denominator), unchecked(size.Height / denominator));

	#endregion
}

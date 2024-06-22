using System;
using SkiaSharp;

namespace GeneXus.Drawing;

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
	public Size(int width, int height)
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
	/// Multiplies <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="left">Multiplicand of type <see cref="Size"/>.</param>
	/// <param name="right">Multiplier of type <see cref="int"/>.</param>
	/// <returns>Product of type <see cref="Size"/>.</returns>
	public static Size operator *(Size left, int right) => Multiply(left, right);

	/// <summary>
	/// Multiplies a <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="left">Multiplier of type <see cref="int"/>.</param>
	/// <param name="right">Multiplicand of type <see cref="Size"/>.</param>
	/// <returns>Product of type <see cref="Size"/>.</returns>
	public static Size operator *(int left, Size right) => Multiply(right, left);

	/// <summary>
	/// Divides <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="left">Dividend of type <see cref="Size"/>.</param>
	/// <param name="right">Divisor of type <see cref="int"/>.</param>
	/// <returns>Result of type <see cref="Size"/>.</returns>
	public static Size operator /(Size left, int right) => Divide(left, right);

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
	public int Width
	{
		readonly get => (int)m_size.Width;
		set => m_size.Width = value;
	}

	/// <summary>
	/// Represents the vertical component of this <see cref='Size'/>.
	/// </summary>
	public int Height
	{
		readonly get => (int)m_size.Height;
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
	/// Performs vector subtraction of two <see cref='Size'/> objects.
	/// </summary>
	public static Size Subtract(Size sz1, Size sz2) => new(sz1.m_size - sz2.m_size);

	/// <summary>
	/// Multiplies <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="size">Multiplicand of type <see cref="Size"/>.</param>
	/// <param name="multiplier">Multiplier of type <see cref='int'/>.</param>
	/// <returns>Product of type <see cref="Size"/>.</returns>
	public static Size Multiply(Size size, int multiplier) => new(size.Width * multiplier, size.Height * multiplier);

	/// <summary>
	/// Divides <see cref="Size"/> by an <see cref="int"/> producing <see cref="Size"/>.
	/// </summary>
	/// <param name="size">Dividend of type <see cref="Size"/>.</param>
	/// <param name="divisor">Divisor of type <see cref='int'/>.</param>
	/// <returns>Quotient of type <see cref="Size"/>.</returns>
	public static Size Divide(Size size, int divisor) => new(size.Width / divisor, size.Height / divisor);

	#endregion
}

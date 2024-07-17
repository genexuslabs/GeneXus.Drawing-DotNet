using System;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public struct Point : IEquatable<Point>
{
	internal SKPoint m_point;

	internal Point(SKPoint point)
	{
		m_point = point;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Point'/> class with the specified coordinates.
	/// </summary>
	public Point(int x, int y)
		: this(new SKPoint(x, y)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Point'/> class from a <see cref='Size'/> .
	/// </summary>
	public Point(Size sz)
		: this(sz.Width, sz.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Point'/> class using coordinates specified by an integer value.
	/// </summary>
	public Point(int dw)
		: this(unchecked((short)((dw >> 0) & 0xFFFF)), unchecked((short)((dw >> 16) & 0xFFFF))) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Point'/>.
	/// </summary>
	public override readonly string ToString() => $"{{X={X},Y={Y}}}";


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKPoint'/> with the coordinates of the specified <see cref='Point'/> .
	/// </summary>
	public static explicit operator SKPoint(Point point) => point.m_point;

	/// <summary>
	/// Creates a <see cref='Size'/> with the coordinates of the specified <see cref='Point'/> .
	/// </summary>
	public static explicit operator Size(Point p) => new(p.X, p.Y);

	/// <summary>
	/// Compares two <see cref='Point'/> objects. The result specifies whether the values of the
	/// <see cref='Point.X'/> and <see cref='Point.Y'/> properties of the two
	/// <see cref='Point'/> objects are equal.
	/// </summary>
	public static bool operator ==(Point left, Point right) => left.m_point == right.m_point;

	/// <summary>
	/// Compares two <see cref='Point'/> objects. The result specifies whether the values of the
	/// <see cref='Point.X'/> or <see cref='Point.Y'/> properties of the two
	/// <see cref='Point'/>  objects are unequal.
	/// </summary>
	public static bool operator !=(Point left, Point right) => left.m_point != right.m_point;

	/// <summary>
	/// Translates a <see cref='Point'/> by a given <see cref='Size'/> .
	/// </summary>
	public static Point operator +(Point pt, Size sz) => Add(pt, sz);

	/// <summary>
	/// Translates a <see cref='Point'/> by the negative of a given <see cref='Size'/> .
	/// </summary>
	public static Point operator -(Point pt, Size sz) => Subtract(pt, sz);

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='Point'/> has the same coordinates
	/// as this Point.
	/// </summary>
	public readonly bool Equals(Point other) => m_point == other.m_point;

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='Point'/> with the same coordinates
	/// as this Point.
	/// </summary>
	public override readonly bool Equals(object obj) => m_point.Equals(obj);

	/// <summary>
	/// Returns a hash code.
	/// </summary>
	public override readonly int GetHashCode() => m_point.GetHashCode();

	#endregion


	#region Fields

	/// <summary>
	/// Creates a new instance of the <see cref='Point'/> class with member data left uninitialized.
	/// </summary>
	public static readonly Point Empty = default;

	#endregion


	#region Properties

	/// <summary>
	/// Gets the x-coordinate of this <see cref='Point'/>.
	/// </summary>
	public int X
	{
		readonly get => (int)m_point.X;
		set => m_point.X = value;
	}

	/// <summary>
	/// Gets the y-coordinate of this <see cref='Point'/>.
	/// </summary>
	public int Y
	{
		readonly get => (int)m_point.Y;
		set => m_point.Y = value;
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref='Point'/> is empty.
	/// </summary>
	public readonly bool IsEmpty => m_point.IsEmpty;

	#endregion


	#region Methods

	/// <summary>
	/// Translates a <see cref='Point'/> by a given <see cref='Size'/> .
	/// </summary>
	public static Point Add(Point pt, Size sz) => new(pt.m_point + sz.m_size);

	/// <summary>
	/// Translates a <see cref='Point'/> by the negative of a given <see cref='Size'/> .
	/// </summary>
	public static Point Subtract(Point pt, Size sz) => new(pt.m_point - sz.m_size);

	/// <summary>
	/// Translates this <see cref='Point'/> by the specified amount.
	/// </summary>
	public void Offset(int dx, int dy) => m_point.Offset(dx, dy);

	/// <summary>
	/// Translates this <see cref='Point'/> by the specified amount.
	/// </summary>
	public void Offset(Point p) => Offset(p.X, p.Y);

	#endregion
}

using System;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public struct PointF : IEquatable<PointF>
{
	internal SKPoint m_point;

	internal PointF(SKPoint point)
	{
		m_point = point;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='PointF'/> class with the specified coordinates.
	/// </summary>
	public PointF(float x, float y)
		: this(new SKPoint(x, y)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='PointF'/> class from a <see cref='SizeF'/> .
	/// </summary>
	public PointF(SizeF sz)
		: this(sz.Width, sz.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='PointF'/> class using coordinates specified by an integer value.
	/// </summary>
	public PointF(int dw)
		: this(unchecked((short)((dw >> 0) & 0xFFFF)), unchecked((short)((dw >> 16) & 0xFFFF))) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='PointF'/>.
	/// </summary>
	public override readonly string ToString() => $"{{X={X},Y={Y}}}";


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKPoint'/> with the coordinates of the specified <see cref='PointF'/> .
	/// </summary>
	public static explicit operator SKPoint(PointF point) => point.m_point;

	/// <summary>
	/// Creates a <see cref='SizeF'/> with the coordinates of the specified <see cref='PointF'/> .
	/// </summary>
	public static explicit operator SizeF(PointF p) => new(p.X, p.Y);

	/// <summary>
	/// Compares two <see cref='PointF'/> objects. The result specifies whether the values of the
	/// <see cref='PointF.X'/> and <see cref='PointF.Y'/> properties of the two
	/// <see cref='PointF'/> objects are equal.
	/// </summary>
	public static bool operator ==(PointF left, PointF right) => left.m_point == right.m_point;

	/// <summary>
	/// Compares two <see cref='PointF'/> objects. The result specifies whether the values of the
	/// <see cref='PointF.X'/> or <see cref='PointF.Y'/> properties of the two
	/// <see cref='PointF'/>  objects are unequal.
	/// </summary>
	public static bool operator !=(PointF left, PointF right) => left.m_point != right.m_point;

	/// <summary>
	/// Translates a <see cref='PointF'/> by a given <see cref='SizeF'/> .
	/// </summary>
	public static PointF operator +(PointF pt, SizeF sz) => Add(pt, sz);

	/// <summary>
	/// Translates a <see cref='PointF'/> by the negative of a given <see cref='SizeF'/> .
	/// </summary>
	public static PointF operator -(PointF pt, SizeF sz) => Subtract(pt, sz);

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='PointF'/> has the same coordinates
	/// as this Point.
	/// </summary>
	public readonly bool Equals(PointF other) => m_point == other.m_point;

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='PointF'/> with the same coordinates
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
	/// Creates a new instance of the <see cref='PointF'/> class with member data left uninitialized.
	/// </summary>
	public static readonly PointF Empty = default;

	#endregion


	#region Properties

	/// <summary>
	/// Gets the x-coordinate of this <see cref='PointF'/>.
	/// </summary>
	public float X
	{
		readonly get => m_point.X;
		set => m_point.X = value;
	}

	/// <summary>
	/// Gets the y-coordinate of this <see cref='PointF'/>.
	/// </summary>
	public float Y
	{
		readonly get => m_point.Y;
		set => m_point.Y = value;
	}

	/// <summary>
	/// Gets a value indicating whether this <see cref='PointF'/> is empty.
	/// </summary>
	public readonly bool IsEmpty => m_point.IsEmpty;

	#endregion


	#region Methods

	/// <summary>
	/// Translates a <see cref='PointF'/> by a given <see cref='SizeF'/> .
	/// </summary>
	public static PointF Add(PointF pt, SizeF sz) => new(pt.m_point + sz.m_size);

	/// <summary>
	/// Translates a <see cref='PointF'/> by the negative of a given <see cref='SizeF'/> .
	/// </summary>
	public static PointF Subtract(PointF pt, SizeF sz) => new(pt.m_point - sz.m_size);

	/// <summary>
	/// Translates this <see cref='PointF'/> by the specified amount.
	/// </summary>
	public void Offset(float dx, float dy) => m_point.Offset(dx, dy);

	/// <summary>
	/// Translates this <see cref='PointF'/> by the specified amount.
	/// </summary>
	public void Offset(PointF p) => Offset(p.X, p.Y);

	#endregion
}

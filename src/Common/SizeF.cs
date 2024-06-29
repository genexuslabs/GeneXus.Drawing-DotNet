using System;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public struct SizeF : IEquatable<SizeF>
{
	internal SKSize m_size;

	private SizeF(SKSize size)
	{
		m_size = size;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='SizeF'/> class from the specified dimensions.
	/// </summary>
	public SizeF(float width, float height)
		: this(new SKSize(width, height)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='SizeF'/> class from the specified
	/// <see cref='PointF'/>.
	/// </summary>
	public SizeF(PointF pt)
		: this(pt.X, pt.Y) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='SizeF'/>.
	/// </summary>
	public override readonly string ToString() => $"{{Width={Width},Height={Height}}}";


	#region Operators

	/// <summary>
	/// Converts the specified <see cref='SizeF'/> to a <see cref='SKSize'/>.
	/// </summary>
	public static explicit operator SKSize(SizeF point) => point.m_size;

	/// <summary>
	/// Converts the specified <see cref='SizeF'/> to a <see cref='PointF'/>.
	/// </summary>
	public static explicit operator PointF(SizeF size) => new(size.Width, size.Height);

	/// <summary>
	/// Tests whether two <see cref='SizeF'/> objects are identical.
	/// </summary>
	public static bool operator ==(SizeF sz1, SizeF sz2) => sz1.m_size == sz2.m_size;

	/// <summary>
	/// Tests whether two <see cref='SizeF'/> objects are different.
	/// </summary>
	public static bool operator !=(SizeF sz1, SizeF sz2) => sz1.m_size != sz2.m_size;

	/// <summary>
	/// Performs vector addition of two <see cref='SizeF'/> objects.
	/// </summary>
	public static SizeF operator +(SizeF sz1, SizeF sz2) => Add(sz1, sz2);

	/// <summary>
	/// Contracts a <see cref='SizeF'/> by another <see cref='SizeF'/>
	/// </summary>
	public static SizeF operator -(SizeF sz1, SizeF sz2) => Subtract(sz1, sz2);

	/// <summary>
	/// Multiplies <see cref="SizeF"/> by an <see cref="float"/> producing <see cref="SizeF"/>.
	/// </summary>
	/// <param name="left">Multiplicand of type <see cref="SizeF"/>.</param>
	/// <param name="right">Multiplier of type <see cref="float"/>.</param>
	/// <returns>Product of type <see cref="SizeF"/>.</returns>
	public static SizeF operator *(SizeF left, float right) => Multiply(left, right);

	/// <summary>
	/// Multiplies a <see cref="SizeF"/> by an <see cref="float"/> producing <see cref="SizeF"/>.
	/// </summary>
	/// <param name="left">Multiplier of type <see cref="float"/>.</param>
	/// <param name="right">Multiplicand of type <see cref="SizeF"/>.</param>
	/// <returns>Product of type <see cref="SizeF"/>.</returns>
	public static SizeF operator *(float left, SizeF right) => Multiply(right, left);

	/// <summary>
	/// Divides <see cref="SizeF"/> by an <see cref="float"/> producing <see cref="SizeF"/>.
	/// </summary>
	/// <param name="left">Dividend of type <see cref="SizeF"/>.</param>
	/// <param name="right">Divisor of type <see cref="float"/>.</param>
	/// <returns>Result of type <see cref="SizeF"/>.</returns>
	public static SizeF operator /(SizeF left, float right) => Divide(left, right);

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='SizeF'/> has the same dimensions
	/// as this Size.
	/// </summary>
	public readonly bool Equals(SizeF other) => m_size == other.m_size;

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='SizeF'/> with the same dimensions
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
	/// Initializes a new instance of the <see cref='SizeF'/> class.
	/// </summary>
	public static readonly SizeF Empty = default;

	#endregion


	#region Properties

	/// <summary>
	/// Represents the horizontal component of this <see cref='SizeF'/>.
	/// </summary>
	public float Width
	{
		readonly get => m_size.Width;
		set => m_size.Width = value;
	}

	/// <summary>
	/// Represents the vertical component of this <see cref='SizeF'/>.
	/// </summary>
	public float Height
	{
		readonly get => m_size.Height;
		set => m_size.Height = value;
	}

	/// <summary>
	/// Tests whether this <see cref='SizeF'/> has zero width and height.
	/// </summary>
	public readonly bool IsEmpty => m_size.IsEmpty;

	#endregion


	#region Methods

	/// <summary>
	/// Performs vector addition of two <see cref='SizeF'/> objects.
	/// </summary>
	public static SizeF Add(SizeF sz1, SizeF sz2) => new(sz1.m_size + sz2.m_size);

	/// <summary>
	/// Performs vector subtraction of two <see cref='SizeF'/> objects.
	/// </summary>
	public static SizeF Subtract(SizeF sz1, SizeF sz2) => new(sz1.m_size - sz2.m_size);

	/// <summary>
	/// Converts a <see cref='SizeF'/> by performing a ceiling operation on all the coordinates.
	/// </summary>
	public static SizeF Ceiling(SizeF value) => new(unchecked((int)Math.Ceiling(value.Width)), unchecked((int)Math.Ceiling(value.Height)));

	/// <summary>
	/// Converts a <see cref='SizeF'/> by performing a truncate operation on all the coordinates.
	/// </summary>
	public static SizeF Truncate(SizeF value) => new(unchecked((int)value.Width), unchecked((int)value.Height));

	/// <summary>
	/// Converts a <see cref='SizeF'/> by performing a round operation on all the coordinates.
	/// </summary>
	public static SizeF Round(SizeF value) => new(unchecked((int)Math.Round(value.Width)), unchecked((int)Math.Round(value.Height)));

	/// <summary>
	/// Multiplies <see cref="SizeF"/> by an <see cref="float"/> producing <see cref="SizeF"/>.
	/// </summary>
	/// <param name="size">Multiplicand of type <see cref="SizeF"/>.</param>
	/// <param name="multiplier">Multiplier of type <see cref='float'/>.</param>
	/// <returns>Product of type <see cref="SizeF"/>.</returns>
	public static SizeF Multiply(SizeF size, float multiplier) => new(unchecked(size.Width * multiplier), unchecked(size.Height * multiplier));

	/// <summary>
	/// Divides <see cref="SizeF"/> by an <see cref="float"/> producing <see cref="SizeF"/>.
	/// </summary>
	/// <param name="size">Dividend of type <see cref="SizeF"/>.</param>
	/// <param name="divisor">Divisor of type <see cref='float'/>.</param>
	/// <returns>Quotient of type <see cref="SizeF"/>.</returns>
	public static SizeF Divide(SizeF size, float divisor) => new(unchecked(size.Width / divisor), unchecked(size.Height / divisor));

	#endregion
}

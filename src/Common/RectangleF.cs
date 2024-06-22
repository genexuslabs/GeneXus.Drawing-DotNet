using System;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public struct RectangleF : IEquatable<RectangleF>
{
	internal SKRect m_rect;

	internal RectangleF(SKRect rect)
	{
		m_rect = rect;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='RectangleF'/> struct with the 
	/// specified location and size.
	/// </summary>
	public RectangleF(float x, float y, float width, float height)
		: this(new SKRect(x, y, width + x, height + y)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='RectangleF'/> struct with the specified location and size.
	/// </summary>
	public RectangleF(PointF location, SizeF size)
		: this(location.X, location.Y, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='RectangleF'/> struct with the specified location (x, y) and size.
	/// </summary>
	public RectangleF(float x, float y, SizeF size)
		: this(x, y, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='RectangleF'/> struct with the specified location and size (width, height).
	/// </summary>
	public RectangleF(PointF location, float width, float height)
		: this(location.X, location.Y, width, height) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='RectangleF'/>.
	/// </summary>
	public override readonly string ToString() => $"{{X={X},Y={Y},Width={Width},Height={Height}}}";


	#region Operators

	public static explicit operator SKRect(RectangleF rect) => rect.m_rect;

	/// <summary>
	/// Tests whether two <see cref='RectangleF'/> objects have equal location and size.
	/// </summary>
	public static bool operator ==(RectangleF left, RectangleF right) => left.m_rect == right.m_rect;

	/// <summary>
	/// Tests whether two <see cref='RectangleF'/> objects differ in location or size.
	/// </summary>
	public static bool operator !=(RectangleF left, RectangleF right) => left.m_rect != right.m_rect;

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='RectangleF'/> has the same location
	/// and size of this Rectangle.
	/// </summary>
	public readonly bool Equals(RectangleF other) => m_rect == other.m_rect;

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='RectangleF'/> with the same location
	/// and size as this Rectangle.
	/// </summary>
	public override readonly bool Equals(object obj) => m_rect.Equals(obj);

	/// <summary>
	/// Returns a hash code.
	/// </summary>
	public override readonly int GetHashCode() => m_rect.GetHashCode();

	#endregion


	#region Fields

	/// <summary>
	/// Initializes a new instance of the <see cref='RectangleF'/> struct.
	/// </summary>
	public static readonly RectangleF Empty = default;

	#endregion


	#region Properties

	/// <summary>
	/// Gets the x-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='RectangleF'/> .
	/// </summary>
	public readonly float Left => m_rect.Left;

	/// <summary>
	/// Gets the x-coordinate of the lower-right corner of the rectangular region defined by this
	/// <see cref='RectangleF'/>.
	/// </summary>
	public readonly float Right => m_rect.Right;

	/// <summary>
	/// Gets the y-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='RectangleF'/>.
	/// </summary>
	public readonly float Top => m_rect.Top;

	/// <summary>
	/// Gets the y-coordinate of the lower-right corner of the rectangular region defined by this
	/// <see cref='RectangleF'/>.
	/// </summary>
	public readonly float Bottom => m_rect.Bottom;

	/// <summary>
	/// Gets or sets the width of the rectangular region defined by this <see cref='RectangleF'/>.
	/// </summary>
	public float Width
	{
		readonly get => m_rect.Width;
		set => m_rect.Right = m_rect.Left + value;
	}

	/// <summary>
	/// Gets or sets the width of the rectangular region defined by this <see cref='RectangleF'/>.
	/// </summary>
	public float Height
	{
		readonly get => m_rect.Height;
		set => m_rect.Bottom = m_rect.Top + value;
	}

	/// <summary>
	/// Gets or sets the x-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='RectangleF'/>.
	/// </summary>
	public float X
	{
		readonly get => Left;
		set
		{
			m_rect.Right += value - m_rect.Left;
			m_rect.Left = value;
		}
	}

	/// <summary>
	/// Gets or sets the y-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='RectangleF'/>.
	/// </summary>
	public float Y
	{
		readonly get => Top;
		set
		{
			m_rect.Bottom += value - m_rect.Top;
			m_rect.Top = value;
		}
	}

	/// <summary>
	/// Gets or sets the size of this <see cref='RectangleF'/>.
	/// </summary>
	public SizeF Size
	{
		get => new(Width, Height);
		set => (Width, Height) = (value.Width, value.Height);
	}

	/// <summary>
	/// Gets or sets the coordinates of the upper-left corner of the rectangular region represented by this
	/// <see cref='RectangleF'/>.
	/// </summary>
	public PointF Location
	{
		get => new(X, Y);
		set => (X, Y) = (value.X, value.Y);
	}

	/// <summary>
	/// Tests whether this <see cref='RectangleF'/> has a <see cref='RectangleF.Width'/>
	/// or a <see cref='RectangleF.Height'/> of 0.
	/// </summary>
	public readonly bool IsEmpty => m_rect.IsEmpty;

	#endregion


	#region Factory

	/// <summary>
	/// Creates a new <see cref='RectangleF'/> with the specified location and size.
	/// </summary>
	public static RectangleF FromLTRB(float left, float top, float right, float bottom) => new(left, top, unchecked(right - left), unchecked(bottom - top));

	#endregion


	#region Methods

	/// <summary>
	/// Determines if the rectangular region represented by <paramref name="rect"/> is entirely contained within the
	/// rectangular region represented by this <see cref='RectangleF'/> .
	/// </summary>
	public readonly bool Contains(RectangleF rect) => m_rect.Contains(rect.m_rect);

	/// <summary>
	/// Determines if the specified point is contained within the rectangular region defined by this
	/// <see cref='RectangleF'/> .
	/// </summary>
	public readonly bool Contains(float x, float y) => m_rect.Contains(x, y);

	/// <summary>
	/// Determines if the specified point is contained within the rectangular region defined by this
	/// <see cref='RectangleF'/> .
	/// </summary>
	public readonly bool Contains(PointF pt) => Contains(pt.X, pt.Y);

	/// <summary>
	/// Creates a <see cref='RectangleF'/> that represents the intersection between this Rectangle and rect.
	/// </summary>
	public void Intersect(RectangleF rect) => m_rect.Intersect(rect.m_rect);

	/// <summary>
	/// Creates a <see cref='RectangleF'/> that represents the intersection between a and b. If there 
	/// is no intersection, an empty rectangle is returned.
	/// </summary>
	public static RectangleF Intersect(RectangleF a, RectangleF b)
	{
		var ret = SKRect.Intersect(a.m_rect, b.m_rect);
		return new RectangleF(ret);
	}

	/// <summary>
	/// Determines if this <see cref='RectangleF'/> intersects with rect.
	/// </summary>
	public readonly bool IntersectsWith(RectangleF rect) => m_rect.IntersectsWith(rect.m_rect);

	/// <summary>
	/// Creates a <see cref='RectangleF'/> that represents the union between this Rectangle and rect.
	/// </summary>
	public void Union(RectangleF rect) => m_rect.Union(rect.m_rect);

	/// <summary>
	/// Creates a <see cref='RectangleF'/> that represents the union between a and b.
	/// </summary>
	public static RectangleF Union(RectangleF a, RectangleF b)
	{
		var ret = SKRect.Union(a.m_rect, b.m_rect);
		return new RectangleF(ret);
	}

	/// <summary>
	/// Inflates this <see cref='RectangleF'/> by the specified amount.
	/// </summary>
	public void Inflate(float width, float height) => m_rect.Inflate(width, height);

	/// <summary>
	/// Inflates this <see cref='RectangleF'/> by the specified amount.
	/// </summary>
	public void Inflate(SizeF size) => Inflate(size.Width, size.Height);

	/// <summary>
	/// Creates a <see cref='RectangleF'/> that is inflated by the specified amount.
	/// </summary>
	public static RectangleF Inflate(RectangleF rect, float x, float y)
	{
		var ret = SKRect.Inflate(rect.m_rect, x, y);
		return new RectangleF(ret);
	}

	/// <summary>
	/// Adjusts the location of this <see cref='RectangleF'/> by the specified amount.
	/// </summary>
	public void Offset(float x, float y) => m_rect.Offset(x, y);

	/// <summary>
	/// Adjusts the location of this <see cref='RectangleF'/> by the specified amount.
	/// </summary>
	public void Offset(PointF pos) => Offset(pos.X, pos.Y);

	#endregion
}

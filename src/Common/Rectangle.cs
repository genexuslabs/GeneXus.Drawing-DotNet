using System;
using SkiaSharp;

namespace GeneXus.Drawing;

[Serializable]
public struct Rectangle : IEquatable<Rectangle>
{
	internal SKRect m_rect;

	internal Rectangle(SKRect rect)
	{
		m_rect = rect;
	}

	/// <summary>
	/// Initializes a new instance of the <see cref='Rectangle'/> struct with the 
	/// specified location and size.
	/// </summary>
	public Rectangle(int x, int y, int width, int height)
		: this(new SKRect(x, y, width + x, height + y)) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Rectangle'/> struct with the specified location and size.
	/// </summary>
	public Rectangle(Point location, Size size)
		: this(location.X, location.Y, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Rectangle'/> struct with the specified location (x, y) and size.
	/// </summary>
	public Rectangle(int x, int y, Size size)
		: this(x, y, size.Width, size.Height) { }

	/// <summary>
	/// Initializes a new instance of the <see cref='Rectangle'/> struct with the specified location and size (width, height).
	/// </summary>
	public Rectangle(Point location, int width, int height)
		: this(location.X, location.Y, width, height) { }

	/// <summary>
	/// Creates a human-readable string that represents this <see cref='Rectangle'/>.
	/// </summary>
	public override readonly string ToString() => $"{{X={X},Y={Y},Width={Width},Height={Height}}}";


	#region Operators

	/// <summary>
	/// Converts the specified <see cref='Rectangle'/> to a <see cref='SKRect'/>.
	/// </summary>
	public static explicit operator SKRect(Rectangle rect) => rect.m_rect;

	/// <summary>
	/// Creates a <see cref='RectangleF'/> with the coordinates of the specified <see cref='Rectangle'/> .
	/// </summary>
	public static implicit operator RectangleF(Rectangle rect) => new(rect.X, rect.Y, rect.Width, rect.Height);

	/// <summary>
	/// Tests whether two <see cref='Rectangle'/> objects have equal location and size.
	/// </summary>
	public static bool operator ==(Rectangle left, Rectangle right) => left.m_rect == right.m_rect;

	/// <summary>
	/// Tests whether two <see cref='Rectangle'/> objects differ in location or size.
	/// </summary>
	public static bool operator !=(Rectangle left, Rectangle right) => left.m_rect != right.m_rect;

	#endregion


	#region IEquatable

	/// <summary>
	/// Tests whether a <see cref='Rectangle'/> has the same location
	/// and size of this Rectangle.
	/// </summary>
	public readonly bool Equals(Rectangle other) => m_rect == other.m_rect;

	/// <summary>
	/// Tests whether <paramref name="obj"/> is a <see cref='Rectangle'/> with the same location
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
	/// Initializes a new instance of the <see cref='Rectangle'/> struct.
	/// </summary>
	public static readonly Rectangle Empty = default;

	#endregion


	#region Properties

	/// <summary>
	/// Gets the x-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='Rectangle'/> .
	/// </summary>
	public readonly int Left => (int)m_rect.Left;

	/// <summary>
	/// Gets the x-coordinate of the lower-right corner of the rectangular region defined by this
	/// <see cref='Rectangle'/>.
	/// </summary>
	public readonly int Right => (int)m_rect.Right;

	/// <summary>
	/// Gets the y-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='Rectangle'/>.
	/// </summary>
	public readonly int Top => (int)m_rect.Top;

	/// <summary>
	/// Gets the y-coordinate of the lower-right corner of the rectangular region defined by this
	/// <see cref='Rectangle'/>.
	/// </summary>
	public readonly int Bottom => (int)m_rect.Bottom;

	/// <summary>
	/// Gets or sets the width of the rectangular region defined by this <see cref='Rectangle'/>.
	/// </summary>
	public int Width
	{
		readonly get => (int)m_rect.Width;
		set => m_rect.Right = m_rect.Left + value;
	}

	/// <summary>
	/// Gets or sets the width of the rectangular region defined by this <see cref='Rectangle'/>.
	/// </summary>
	public int Height
	{
		readonly get => (int)m_rect.Height;
		set => m_rect.Bottom = m_rect.Top + value;
	}

	/// <summary>
	/// Gets or sets the x-coordinate of the upper-left corner of the rectangular region defined by this
	/// <see cref='Rectangle'/>.
	/// </summary>
	public int X
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
	/// <see cref='Rectangle'/>.
	/// </summary>
	public int Y
	{
		readonly get => Top;
		set
		{
			m_rect.Bottom += value - m_rect.Top;
			m_rect.Top = value;
		}
	}

	/// <summary>
	/// Gets or sets the size of this <see cref='Rectangle'/>.
	/// </summary>
	public Size Size
	{
		get => new(Width, Height);
		set => (Width, Height) = (value.Width, value.Height);
	}

	/// <summary>
	/// Gets or sets the coordinates of the upper-left corner of the rectangular region represented by this
	/// <see cref='Rectangle'/>.
	/// </summary>
	public Point Location
	{
		get => new(X, Y);
		set => (X, Y) = (value.X, value.Y);
	}

	/// <summary>
	/// Tests whether this <see cref='Rectangle'/> has a <see cref='Rectangle.Width'/>
	/// or a <see cref='Rectangle.Height'/> of 0.
	/// </summary>
	public readonly bool IsEmpty => m_rect.IsEmpty;

	/// <summary>
	/// Gets a secuencie of <see cref='Point'/> that defines this <see cref='Rectangle'/>.
	/// </summary>
	public readonly Point[] Points => new[]
	{
		new Point(Left, Top),
		new Point(Right, Top),
		new Point(Right, Bottom),
		new Point(Left, Bottom)
	};

	#endregion


	#region Factory

	/// <summary>
	/// Creates a new <see cref='Rectangle'/> with the specified location and size.
	/// </summary>
	public static Rectangle FromLTRB(int left, int top, int right, int bottom) => new(left, top, unchecked(right - left), unchecked(bottom - top));

	#endregion


	#region Methods

	/// <summary>
	/// Determines if the rectangular region represented by <paramref name="rect"/> is entirely contained within the
	/// rectangular region represented by this <see cref='Rectangle'/> .
	/// </summary>
	public readonly bool Contains(Rectangle rect) => m_rect.Contains(rect.m_rect);

	/// <summary>
	/// Determines if the specified point is contained within the rectangular region defined by this
	/// <see cref='Rectangle'/> .
	/// </summary>
	public readonly bool Contains(int x, int y) => m_rect.Contains(x, y);

	/// <summary>
	/// Determines if the specified point is contained within the rectangular region defined by this
	/// <see cref='Rectangle'/> .
	/// </summary>
	public readonly bool Contains(Point pt) => Contains(pt.X, pt.Y);

	/// <summary>
	/// Creates a <see cref='Rectangle'/> that represents the intersection between this Rectangle and rect.
	/// </summary>
	public void Intersect(Rectangle rect) => m_rect.Intersect(rect.m_rect);

	/// <summary>
	/// Creates a <see cref='Rectangle'/> that represents the intersection between a and b. If there 
	/// is no intersection, an empty rectangle is returned.
	/// </summary>
	public static Rectangle Intersect(Rectangle a, Rectangle b)
	{
		var ret = SKRect.Intersect(a.m_rect, b.m_rect);
		return new Rectangle(ret);
	}

	/// <summary>
	/// Determines if this <see cref='Rectangle'/> intersects with rect.
	/// </summary>
	public readonly bool IntersectsWith(Rectangle rect) => m_rect.IntersectsWith(rect.m_rect);

	/// <summary>
	/// Creates a <see cref='Rectangle'/> that represents the union between this Rectangle and rect.
	/// </summary>
	public void Union(Rectangle rect) => m_rect.Union(rect.m_rect);

	/// <summary>
	/// Creates a <see cref='Rectangle'/> that represents the union between a and b.
	/// </summary>
	public static Rectangle Union(Rectangle a, Rectangle b)
	{
		var ret = SKRect.Union(a.m_rect, b.m_rect);
		return new Rectangle(ret);
	}

	/// <summary>
	/// Inflates this <see cref='Rectangle'/> by the specified amount.
	/// </summary>
	public void Inflate(int width, int height) => m_rect.Inflate(width, height);

	/// <summary>
	/// Inflates this <see cref='Rectangle'/> by the specified amount.
	/// </summary>
	public void Inflate(Size size) => Inflate(size.Width, size.Height);

	/// <summary>
	/// Creates a <see cref='Rectangle'/> that is inflated by the specified amount.
	/// </summary>
	public static Rectangle Inflate(Rectangle rect, int x, int y)
	{
		var ret = SKRect.Inflate(rect.m_rect, x, y);
		return new Rectangle(ret);
	}

	/// <summary>
	/// Adjusts the location of this <see cref='Rectangle'/> by the specified amount.
	/// </summary>
	public void Offset(int x, int y) => m_rect.Offset(x, y);

	/// <summary>
	/// Adjusts the location of this <see cref='Rectangle'/> by the specified amount.
	/// </summary>
	public void Offset(Point pos) => Offset(pos.X, pos.Y);

	#endregion
}

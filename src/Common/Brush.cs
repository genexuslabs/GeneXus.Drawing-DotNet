using System;
using SkiaSharp;

namespace GeneXus.Drawing;

public abstract class Brush : IDisposable, ICloneable
{
	internal readonly SKPaint m_paint;

	internal Brush(SKPaint paint)
	{
		m_paint = paint ?? throw new ArgumentNullException(nameof(paint));
		m_paint.Style = SKPaintStyle.Fill;
	}

	/// <summary>
	///  Cleans up resources for this <see cref='Brush'/>.
	/// </summary>
	~Brush() => Dispose(false);


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Brush'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	protected virtual void Dispose(bool disposing) => m_paint.Dispose();

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Pen'/>.
	/// </summary>
	public abstract object Clone();

	#endregion


	#region Operators

	/// <summary>
	///  Creates a <see cref='SKPaint'/> with the coordinates of the specified <see cref='Brush'/>.
	/// </summary>
	public static explicit operator SKPaint(Brush brush) => brush.m_paint;

	#endregion


	#region Utilities

	protected static Color ApplyFactor(Color color, float factor)
		=> Color.FromArgb(
			(int)(color.A * factor),
			(int)(color.R * factor),
			(int)(color.G * factor),
			(int)(color.B * factor));

	#endregion
}

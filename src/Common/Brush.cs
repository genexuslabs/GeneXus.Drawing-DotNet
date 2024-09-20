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

	protected static Drawing2D.Blend GetBlendTriangularShape(float focus, float scale)
	{
		if (focus < 0 || focus > 1)
			throw new ArgumentException("Invalid focus value", nameof(focus));
		if (scale < 0 || scale > 1)
			throw new ArgumentException("Invalid scale value", nameof(scale));
		
		int count = focus == 0 || focus == 1 ? 2 : 3;
		Drawing2D.Blend blend = new(count);

		if (focus == 0)
		{
			blend.Positions[0] = 0;
			blend.Factors[1] = scale;
			blend.Positions[1] = 1;
			blend.Factors[1] = 0;
		}
		else if (focus == 1)
		{
			blend.Positions[0] = 0;
			blend.Factors[1] = 0;
			blend.Positions[1] = 1;
			blend.Factors[1] = scale;
		}
		else
		{
			blend.Positions[0] = 0;
			blend.Factors[0] = 0;
			blend.Positions[1] = focus;
			blend.Factors[1] = scale;
			blend.Positions[2] = 1;
			blend.Factors[2] = 0;
		}
		return blend;
	}

	protected static Drawing2D.Blend GetSigmaBellShape(float focus, float scale = 1.0f)
	{
		if (focus < 0 || focus > 1)
			throw new ArgumentException("Invalid focus value", nameof(focus));
		if (scale < 0 || scale > 1)
			throw new ArgumentException("Invalid scale value", nameof(scale));
		
		int count = focus == 0 || focus == 1 ? 256 : 511;
		Drawing2D.Blend m_blend = new(count);

		// TODO: clear preset colors

		float fallOffLenght = 2.0f;
		if (focus == 0)
		{
			m_blend.Positions[0] = focus;
			m_blend.Factors[0] = scale;

			SigmaBellBlend(ref m_blend, focus, scale, 1 / fallOffLenght, 1f / 2, 1f / 255, 1, count - 1, true);

			m_blend.Positions[count - 1] = 1f;
			m_blend.Factors[count - 1] = 0f;
		}
		else if (focus == 1)
		{
			m_blend.Positions[0] = 0f;
			m_blend.Factors[0] = 0f;

			SigmaBellBlend(ref m_blend, focus, scale, 1 / fallOffLenght, 1f / 2, 1f / 255, 1, count - 1, false);

			m_blend.Positions[count - 1] = focus;
			m_blend.Factors[count - 1] = scale;
		}
		else
		{
			int middle = count / 2;

			// left part of the sigma bell
			m_blend.Positions[0] = 0f;
			m_blend.Factors[0] = 0f;

			SigmaBellBlend(ref m_blend, focus, scale, focus / (2 * fallOffLenght), focus / 2, focus / 255, 1, middle, false);

			// middle part of the sigma bell
			m_blend.Positions[middle] = focus;
			m_blend.Factors[middle] = scale;

			// right part of the sigma bell
			SigmaBellBlend(ref m_blend, focus, scale, (1 - focus) / (2 * fallOffLenght), (1 + focus) / 2, (1 - focus) / 255, middle + 1, count - 1, true);

			m_blend.Positions[count - 1] = 1f;
			m_blend.Factors[count - 1] = 0f;
		}
		return m_blend;

		static void SigmaBellBlend(ref Drawing2D.Blend blend, float focus, float scale, float sigma, float mean, float delta, int startIndex, int endIndex, bool invert)
		{
			float sg = invert ? -1 : 1;
			float x0 = invert ? 1f : 0f;

			float cb = (1 + sg * Erf(x0, sigma, mean)) / 2;
			float ct = (1 + sg * Erf(focus, sigma, mean)) / 2;
			float ch = ct - cb;

			float offset = invert ? focus : 0;
			float pos = delta + offset;

			for (int index = startIndex; index < endIndex; index++, pos += delta)
			{
				blend.Positions[index] = pos;
				blend.Factors[index] = scale / ch * ((1 + sg * Erf(pos, sigma, mean)) / 2 - cb);
			}
		}

		static float Erf(float x, float sigma, float mean, int terms = 6)
		{
			/*
			* Error function (Erf) for Gaussian distribution by Maclaurin series:
			* erf (z) = (2 / sqrt (pi)) * infinite sum of [(pow (-1, n) * pow (z, 2n+1))/(n! * (2n+1))]
			*/
			float constant = 2 / (float)Math.Sqrt(Math.PI);
			float z = (x - mean) / (sigma * (float)Math.Sqrt(2));

			float series = z;
			for (int n = 1, fact = 1; n < terms; n++, fact *= n)
			{
				int sign = (int)Math.Pow(-1, n);
				int step = 2 * n + 1;
				series += sign * (float)Math.Pow(z, step) / (fact * step);
			}

			return constant * series;
		}
	}

	#endregion
}

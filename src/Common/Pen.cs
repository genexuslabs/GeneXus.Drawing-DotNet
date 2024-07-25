
using System;
using SkiaSharp;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing;

public class Pen : ICloneable, IDisposable
{
	internal SKPaint m_paint;
	private Brush m_brush = Brushes.Transparent;

	internal Pen(SKPaint paint, float width)
	{
		m_paint = paint ?? throw new ArgumentNullException(nameof(paint));
		m_paint.Style = SKPaintStyle.Stroke;
		m_paint.StrokeWidth = width;
		m_paint.StrokeMiter = 10;
		m_paint.TextAlign = SKTextAlign.Center;
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='Pen'/> class with the 
	///  specified <see cref='Drawing.Color'/> and <see cref='Width'/> .
	/// </summary>
	public Pen(Color color, float width = 1.0f) 
		: this (new SKPaint() { Color = color.m_color }, width)
	{
		m_brush = new SolidBrush(color);
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='Pen'/> class with the 
	///  specified <see cref='Brush'/> and <see cref='Width'/> .
	/// </summary>
	public Pen(Brush brush, float width = 1.0f) 
		: this(brush.m_paint.Clone(), width) 
	{
		m_brush = brush;
	}

	/// <summary>
	///  Cleans up resources for this <see cref='Pen'/>.
	/// </summary>
	~Pen() => Dispose(false);

	/// <summary>
	///  Creates a human-readable string that represents this <see cref='Pen'/>.
	/// </summary>
	public override string ToString() => $"{GetType().Name}: [{Color}, Width: {Width}]";


	#region IDisposable

	/// <summary>
	///  Cleans up resources for this <see cref='Pen'/>.
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
	public object Clone() 
		=> new Pen(m_paint, m_paint.StrokeWidth);

	#endregion


	#region Operators

	/// <summary>
	///  Creates a <see cref='SKPaint'/> with the coordinates of the specified <see cref='Pen'/>.
	/// </summary>
	public static explicit operator SKPaint(Pen pen) => pen.m_paint;

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets the alignment for this <see cref='Pen'/>.
	/// </summary>
	public PenAlignment Alignment
	{ 
		get => m_paint.TextAlign switch
		{
			SKTextAlign.Center => PenAlignment.Center,
			SKTextAlign.Left => PenAlignment.Left,
			SKTextAlign.Right => PenAlignment.Right,
			_ => throw new NotSupportedException($"unsuported value {m_paint.TextAlign}")
		};
		set => m_paint.TextAlign = value switch
		{
			PenAlignment.Center => SKTextAlign.Center,
			PenAlignment.Left => SKTextAlign.Left,
			PenAlignment.Right => SKTextAlign.Right,
			_ => throw new NotSupportedException($"unsuported value {value}")
		}; 
	}

	/// <summary>
	///  Gets or sets the <see cref='Drawing.Brush'/> that determines attributes of this <see cref='Pen'/>.
	/// </summary>
	public Brush Brush
	{
		get => m_brush;
		set
		{
			m_paint.Color = value.m_paint.Color;
			m_brush = value;
		}
	}

	/// <summary>
	///  Gets or sets the color of this <see cref='Pen'/>.
	/// </summary>
	public Color Color
	{
		get => new(m_paint.Color);
		set => m_paint.Color = value.m_color;
	}

	/// <summary>
	///  Gets or sets an array of values that specifies a compound <see cref='Pen'/>. A compound 
	///  pen draws a compound line made up of parallel lines and spaces.
	/// </summary>
	public float[] CompoundArray
	{
		get => m_interval;
		set
		{
			m_interval = value;
			m_paint.PathEffect = SKPathEffect.CreateDash(m_interval, 0);
		}
	}

	private float[] m_interval = Array.Empty<float>();

	/// <summary>
	///  Gets or sets a custom cap to use at the end of lines drawn 
	///  with this <see cref='Pen'/>.
	/// </summary>
	public object CustomEndCap // TODO: implement CustomLineCap class
	{
		get => throw new NotImplementedException();
		set => throw new NotImplementedException();
	}

	/// <summary>
	///  Gets or sets a custom cap to use at the beginning of lines drawn 
	///  with this <see cref='Pen'/>.
	/// </summary>
	public object CustomStartCap // TODO: implement CustomLineCap class
	{
		get => throw new NotImplementedException();
		set => throw new NotImplementedException();
	}

	/// <summary>
	///  Gets or sets the cap style used at the end of the dashes that make 
	///  up dashed lines drawn with this <see cref='Pen'/>.
	/// </summary>
	public DashCap DashCap { get; set; } = DashCap.Flat;

	/// <summary>
	///  Gets or sets the distance from the start of a line to the beginning of a dash pattern.
	/// </summary>
	public float DashOffset { get; set; } = 0.0f;

	/// <summary>
	///  Gets or sets an array of custom dashes and spaces.
	/// </summary>
	public float[] DashPattern { get; set; } = Array.Empty<float>();

	/// <summary>
	///  Gets the style of lines drawn with this <see cref='Pen'/>.
	/// </summary>
	public PenType PenType => m_brush switch
	{
		SolidBrush => PenType.SolidColor,
		TextureBrush => PenType.TextureFill,
		LinearGradientBrush => PenType.LinearGradient,
		PathGradientBrush => PenType.PathGradient,
		HatchBrush => PenType.HatchFill,
		_ => throw new NotImplementedException($"the {m_brush.GetType().Name} pen type is not implemented.")
	};

	/// <summary>
	///  Gets or sets the cap style used at the beginning of lines drawn with this <see cref='Pen'/>.
	/// </summary>
	public LineCap StartCap { get; set; } = LineCap.Flat;

	/// <summary>
	///  Gets or sets the cap style used at the end of lines drawn with this <see cref='Pen'/>.
	/// </summary>
	public LineCap EndCap { get; set; } = LineCap.Flat;

	/// <summary>
	///  Gets or sets the join style for the ends of two consecutive lines drawn with this <see cref='Pen'/>.
	/// </summary>
	public LineJoin LineJoin
	{
		get => m_paint.StrokeJoin switch
		{
			SKStrokeJoin.Bevel => LineJoin.Bevel,
			SKStrokeJoin.Round => LineJoin.Round,
			SKStrokeJoin.Miter => LineJoin.Miter,
			_ => throw new NotImplementedException($"undefined map to {m_paint.StrokeJoin}.")
		};
		set => m_paint.StrokeJoin = value switch
		{
			LineJoin.Bevel => SKStrokeJoin.Bevel,
			LineJoin.Round => SKStrokeJoin.Round,
			LineJoin.Miter => SKStrokeJoin.Miter,
			_ => throw new ArgumentException($"undefined value {value}.", nameof(value))
		};
	}

	/// <summary>
	///  Gets or sets the limit of the thickness of the join on a mitered corner.
	/// </summary>
	public float MiterLimit
	{
		get => m_paint.StrokeMiter;
		set => m_paint.StrokeMiter = value;
	}

	/// <summary>
	///  Gets or sets a copy of the geometric transformation for this <see cref='Pen'/>.
	/// </summary>
	public Matrix Transform { get; set; } = new Matrix();

	/// <summary>
	///  Gets or sets the width of this <see cref='Pen'/>.
	/// </summary>
	public float Width
	{
		get => m_paint.StrokeWidth;
		set => m_paint.StrokeWidth = value;
	}

	#endregion


	#region Mathod

	/// <summary>
	///  Multiplies the transformation matrix for this <see cref='Pen'/> by the specified <see cref='Matrix'/>.
	/// </summary>
	public void MultiplyTransform(Matrix matrix)
		=> Transform.Multiply(matrix);

	/// <summary>
	///  Resets the geometric transformation matrix for this <see cref='Pen'/> to identity.
	/// </summary>
	public void ResetTransform()
		=> Transform.Reset();

	/// <summary>
	///  Rotates the local geometric transformation by the specified angle.
	/// </summary>
	public void RotateTransform(float degrees, MatrixOrder order = MatrixOrder.Prepend)
		=> Transform.Rotate(degrees, order);

	/// <summary>
	///  Scales the local geometric transformation by the specified factors.
	/// </summary>
	public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Prepend)
		=> Transform.Scale(sx, sy, order);

	/// <summary>
	///  Sets the values that determine the style of cap used to end lines drawn by this <see cref='Pen'/>.
	/// </summary>
	public void SetLineCap(LineCap startCap, LineCap endCap, DashCap dashCap)
		=> throw new NotImplementedException();

	/// <summary>
	///  Translates the local geometric transformation by the specified dimensions.
	/// </summary>
	public void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Prepend)
		=> Transform.Translate(dx, dy, order);

	#endregion
}

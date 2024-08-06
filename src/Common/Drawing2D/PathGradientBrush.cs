using System;
using System.ComponentModel;
using System.Linq;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class PathGradientBrush : Brush
{
	private GraphicsPath m_path;
	private WrapMode m_mode;
	private Matrix m_transform;
	private Blend m_blend;
	private ColorBlend m_interpolation;
	private PointF m_center, m_focus;
	private Color m_color;
	private Color[] m_surround;

	private PathGradientBrush(GraphicsPath path, WrapMode mode, Matrix transform)
		: base(new SKPaint {  })
	{	
		m_path = path;
		m_mode = mode;

		m_blend = null;
		m_color = new Color(255, 255, 255, 255);
		m_center = GetCentroid(m_path);
		m_focus = new PointF(0, 0);
		m_interpolation = new()
		{
			Colors = new[] { Color.Empty },
			Positions = new[] { 0f }
		};
		m_surround = new[] { CenterColor };
		m_transform = transform;

		UpdateShader(() => { });
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified path.
	/// </summary>
	/// <param name="path"></param>
	public PathGradientBrush(GraphicsPath path)
		: this(path, WrapMode.Clamp, new Matrix()) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified points and wrap mode.
	/// </summary>
	public PathGradientBrush(PointF[] points, WrapMode mode = WrapMode.Clamp)
		: this(CreatePath(points), mode, new Matrix()) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified points and wrap mode.
	/// </summary>
	public PathGradientBrush(Point[] points, WrapMode mode = WrapMode.Clamp)
		: this(Array.ConvertAll(points, point => new PointF(point.m_point)), mode) { }


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='PathGradientBrush'/>.
	/// </summary>
	public override object Clone()
		=> m_path.Clone() is GraphicsPath path ? new PathGradientBrush(path, m_mode, m_transform) : throw new Exception("path could not be cloned.");

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets a <see cref='Drawing.Blend'/> that specifies positions and factors that define a 
	///  custom falloff for the gradient.
	/// </summary>
	public Blend Blend 
	{
		get => m_blend; 
		set => UpdateShader(() => m_blend = value ?? throw new ArgumentNullException(nameof(value)));
	}

	/// <summary>
	///  Gets or sets the <see cref='Color'/> at the center of the path gradient.
	/// </summary>
	public Color CenterColor 
	{ 
		get => m_color;
		set => UpdateShader(() => m_color = value);
	}

	/// <summary>
	///  Gets or sets the center <see cref='Point'/> of the path gradient.
	/// </summary>
	public PointF CenterPoint
	{ 
		get => m_center;
		set => UpdateShader(() => m_center = value);
	}

	/// <summary>
	///  Gets or sets the focus <see cref='Point'/> for the gradient falloff.
	/// </summary>
	public PointF FocusScales 
	{ 
		get => m_focus;
		set => UpdateShader(() => m_focus = value);
	}

	/// <summary>
	///  Gets or sets a <see cref='ColorBlend'/> that defines a multicolor linear gradient.
	/// </summary>
	public ColorBlend InterpolationColors
	{
		get => m_interpolation;
		set => UpdateShader(() => m_interpolation = value);
	}

	/// <summary>
	///  Gets a bounding <see cref='Drawing.Rectangle'/> for this gradient.
	/// </summary>
	public RectangleF Rectangle => m_path.GetBounds();

	/// <summary>
	///  Gets or sets an array of <see cref='Color'/> structures that correspond to the 
	///  points in the path this gradient fills.
	/// </summary>
	public Color[] SurroundColors 
	{
		get => m_surround;
		set => UpdateShader(() => m_surround = value);
	}

	/// <summary>
	///  Gets or sets a copy of the <see cref='Matrix'/> that defines a local geometric 
	///  transform for this gradient.
	/// </summary>
	public Matrix Transform
	{
		get => m_transform;
		set => UpdateShader(() => m_transform = value);
	}

	/// <summary>
	///  Gets or sets a <see cref='Drawing.WrapMode'/> that indicates the wrap mode for this gradient.
	/// </summary>
	public WrapMode WrapMode
	{
		get => m_mode;
		set => UpdateShader(() =>
		{
			if (value is < WrapMode.Tile or > WrapMode.Clamp)
				throw new InvalidEnumArgumentException(nameof(value), (int)value, typeof(WrapMode));
			m_mode = value;
		});
	}

	#endregion


	#region Methods

	/// <summary>
	///  Multiplies the <see cref='Matrix'/> object that represents the local 
	///  geometric transformation of this <see cref='PathGradientBrush'/> object 
	///  by the specified <see cref='Matrix'/> object in the specified order.
	/// </summary>
	public void MultiplyTransform(Matrix matrix, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Multiply(matrix, order));

	/// <summary>
	///  Resets the Transform property of this <see cref='PathGradientBrush'/> object to identity.
	/// </summary>
	public void ResetTransform()
		=> UpdateShader(() => Transform.Reset());

	/// <summary>
	///  Rotates the local geometric transformation of this <see cref='PathGradientBrush'/> object 
	///  by the specified amount in the specified order.
	/// </summary>
	public void RotateTransform(float angle, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Rotate(angle, order));

	/// <summary>
	///  Scales the local geometric transformation of this <see cref='PathGradientBrush'/> object 
	///  by the specified amounts in the specified order.
	/// </summary>
	public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Scale(sx, sy, order));

	/// <summary>
	///  Creates a gradient with a center color and a linear falloff to each surrounding color.
	/// </summary>
	public void SetBlendTriangularShape(float focus, float scale = 1.0f)
		=> UpdateShader(() => m_blend = GetBlendTriangularShape(focus, scale));

	/// <summary>
	///  Creates a gradient brush that changes color starting from the center of the path outward 
	///  to the path's boundary. The transition from one color to another is based on a bell-shaped curve.
	/// </summary>
	public void SetSigmaBellShape(float focus, float scale = 1.0f)
		=> UpdateShader(() => m_blend = GetSigmaBellShape(focus, scale));

	/// <summary>
	///  Translates the local geometric transformation of this <see cref='PathGradientBrush'/> object
	///  by the specified dimensions in the specified order.
	/// </summary>
	public void TranslateTransform(float dx, float dy, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Translate(dx, dy, order));

	#endregion


	#region Utilities

	private static GraphicsPath CreatePath(PointF[] points)
	{
		var types = new byte[points.Length];
		types[0] = (byte)PathPointType.Start;
		for (int i = 1; i < types.Length - 1; i++)
			types[i] = (byte)PathPointType.Line;
		types[types.Length - 1] = (byte)PathPointType.CloseSubpath;
		return new GraphicsPath(points, types);
	}

	private static PointF GetCentroid(GraphicsPath path)
	{
		var bounds = path.GetBounds();
		return new PointF(bounds.X + bounds.Width / 2, bounds.Y + bounds.Height / 2);
	}

	private void UpdateShader(Action action)
	{
		action();

		switch (m_mode)
		{
			case WrapMode.TileFlipX:
				m_transform.Scale(-1, 1);
				break;
			case WrapMode.TileFlipY:
				m_transform.Scale(1, -1);
				break;
			case WrapMode.TileFlipXY:
				m_transform.Scale(-1, -1);
				break;
		}

		var center = m_center.m_point;
		var focus = Math.Max(m_focus.X, m_focus.Y);
		var factors = m_blend?.Factors ?? Enumerable.Repeat(1f, m_interpolation.Positions.Length).ToArray();
		var positions = m_interpolation.Positions
			.Prepend(0f)
			.Take(factors.Length)
			.ToArray();
		var colors = m_interpolation.Colors
			.Prepend(m_color)
			.Zip(factors, ApplyFactor)
			.Select(color => color.m_color)
			.ToArray();
		var mode = m_mode == WrapMode.Clamp ? SKShaderTileMode.Decal : SKShaderTileMode.Repeat;
		var matrix = m_transform.m_matrix;

		m_paint.Shader = SKShader.CreateRadialGradient(center, focus, colors, positions, mode, matrix);
	}

	#endregion
}

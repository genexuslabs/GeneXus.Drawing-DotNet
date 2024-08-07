using System;
using System.Collections.Generic;
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
	private ColorBlend m_colors;
	private PointF m_center, m_focus;
	private Color m_color;
	private Color[] m_surround;

	private PathGradientBrush(GraphicsPath path, WrapMode mode, Matrix transform)
		: base(new SKPaint { })
	{
		var color = Color.White;
		var points = path.PathPoints;
		if (points.First() == points.Last())
			points = points.Take(points.Length - 1).ToArray();

		m_path = path;
		m_mode = mode;
		m_transform = transform;

		m_blend = new();
		Array.Copy(new[] { 1f }, m_blend.Factors, 1);
		Array.Copy(new[] { 0f }, m_blend.Positions, 1);

		m_colors = new();
		Array.Copy(new[] { Color.Empty }, m_colors.Colors, 1);
		Array.Copy(new[] { 0f }, m_colors.Positions, 1);

		m_center = new PointF(0, 0);
		foreach (var point in points)
		{
			m_center.X += point.X;
			m_center.Y += point.Y;
		}
		m_center.X /= points.Length;
		m_center.Y /= points.Length;

		m_color = color;
		m_focus = new PointF(0, 0);
		m_surround = new[] { color };

		UpdateShader(() => { });
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified path.
	/// </summary>
	/// <param name="path"></param>
	public PathGradientBrush(GraphicsPath path)
		: this(path, WrapMode.Clamp, new Matrix()) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified points.
	/// </summary>
	public PathGradientBrush(params PointF[] points)
		: this(points, WrapMode.Clamp) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified points and wrap mode.
	/// </summary>
	public PathGradientBrush(PointF[] points, WrapMode mode)
		: this(CreatePath(points), mode, new Matrix()) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified points.
	/// </summary>
	public PathGradientBrush(params Point[] points)
		: this(points, WrapMode.Clamp) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='PathGradientBrush'/> class with the specified points and wrap mode.
	/// </summary>
	public PathGradientBrush(Point[] points, WrapMode mode)
		: this(Array.ConvertAll(points, point => new PointF(point.m_point)), mode) { }


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='PathGradientBrush'/>.
	/// </summary>
	public override object Clone()
	{
		var path = new GraphicsPath(m_path.PathPoints, m_path.PathTypes, m_path.FillMode);
		var transform = new Matrix(m_transform.MatrixElements);
		return new PathGradientBrush(path, WrapMode, transform)
		{
			Blend = Blend,
			CenterColor = CenterColor,
			CenterPoint = CenterPoint,
			FocusScales = FocusScales,
			InterpolationColors = InterpolationColors,
			SurroundColors = SurroundColors,
		};
	}

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
		get => m_colors;
		set => UpdateShader(() =>
		{
			var interpolation = value ?? throw new ArgumentNullException(nameof(value));
			if (Enumerable.SequenceEqual(m_colors.Positions, interpolation.Positions) && Enumerable.SequenceEqual(m_colors.Colors, interpolation.Colors))
				return;
			if (interpolation.Positions[0] != 0 )
				throw new ArgumentException("first element must be equal to 0.", nameof(value));
			if (interpolation.Positions[value.Positions.Length - 1] != 1)
				throw new ArgumentException("last element must be equal to 1.", nameof(value));
			m_colors = interpolation;
		});
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

	private void UpdateShader(Action action)
	{
		action();

		using var transform = new Matrix(m_transform.MatrixElements);
		switch (m_mode)
		{
			case WrapMode.TileFlipX:
				transform.Scale(-1, 1);
				break;
			case WrapMode.TileFlipY:
				transform.Scale(1, -1);
				break;
			case WrapMode.TileFlipXY:
				transform.Scale(-1, -1);
				break;
		}

		var points = new PointF[] { m_center, m_focus };
		transform.TransformPoints(points);
		transform.Reset();

		var matrix = transform.m_matrix;
		var mode = m_mode == WrapMode.Clamp ? SKShaderTileMode.Decal : SKShaderTileMode.Repeat;

		var center = points[0].m_point;
		var focus = Math.Max(points[1].X, points[1].Y);

		int index;

		var blend = new Dictionary<float, object>() { [0] = m_color, [1] = Color.Empty };
		for (index = 0; index < m_blend.Positions.Length; index++)
		{
			var pos = m_blend.Positions[index];
			var fac = m_blend.Factors[index];
			blend[pos] = fac; // blend factor
		}
		for (index = 0; index < m_colors.Positions.Length && m_colors.Positions.Length > 1; index++)
		{
			var pos = m_colors.Positions[index];
			var col = m_colors.Colors[index];
			blend[pos] = col; // specific color
		}

		var positions = blend.Keys.OrderBy(key => key).ToArray();
		var colors = new SKColor[positions.Length];
	
		var lastColor = Color.Empty;
		for (index = 0; index < positions.Length; index++)
		{
			var key = positions[index];
			var value = blend[key];
			if (value is Color currColor)
			{
				colors[index] = currColor.m_color;
				lastColor = currColor;
				continue;
			}
			if (value is float factor)
			{
				var color = ApplyFactor(lastColor, factor);
				colors[index] = color.m_color;
				continue;
			}
		}

		var bounds = m_path.GetBounds();
		float scaleX = bounds.Width < bounds.Height ? bounds.Width / bounds.Height : 1;
		float scaleY = bounds.Height < bounds.Width ? bounds.Height / bounds.Width : 1;
		transform.Scale(scaleX, scaleY); // make an ellipse

		float radius = Math.Max(bounds.Width, bounds.Height) / 2;

		m_paint.Shader = SKShader.CreateRadialGradient(center, radius, colors, positions, mode, matrix);
	}

	#endregion
}

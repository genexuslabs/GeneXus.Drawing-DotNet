using System;
using System.ComponentModel;
using System.Linq;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class LinearGradientBrush : Brush 
{
	private RectangleF m_rect;
	private WrapMode m_mode;
	private Matrix m_transform;
	private Blend m_blend;
	private ColorBlend m_colors;
	private bool m_gamma;

	private LinearGradientBrush(GradientVector vector, Color[] colors, WrapMode mode, Matrix transform)
		: base(new SKPaint { })
	{
		m_rect = new RectangleF(vector.BegPoint.X, vector.BegPoint.Y, vector.BegPoint.X + vector.EndPoint.X, vector.BegPoint.Y + vector.EndPoint.Y);
		m_mode = mode;
		m_transform = transform;

		m_gamma = false;
		
		m_blend = new();
		Array.Copy(new[] { 1f }, m_blend.Factors, 1);
		Array.Copy(new[] { 0f }, m_blend.Positions, 1);
		
		var uniform = CreateUniformArray(colors.Length);

		m_colors = new(colors.Length);
		Array.Copy(colors, m_colors.Colors, colors.Length);
		Array.Copy(uniform, m_colors.Positions, colors.Length);

		UpdateShader(() => { });
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='LinearGradientBrush'/> class with the specified 
	///  points and colors.
	/// </summary>
	public LinearGradientBrush(PointF point1, PointF point2, Color color1, Color color2)
		: this(GetGradientVector(point1, point2), new[] { color1, color2 }, WrapMode.Tile, new Matrix()) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='LinearGradientBrush'/> class with the specified 
	///  points and colors.
	/// </summary>
	public LinearGradientBrush(Point point1, Point point2, Color color1, Color color2)
		: this(new PointF(point1.m_point), new PointF(point2.m_point), color1, color2) { }

	/// <summary>
	///  Creates a new instance of the <see cref='LinearGradientBrush'/> class based on 
	///  a <see cref='RectangleF'/>, starting and ending colors, and orientation.
	/// </summary>
	public LinearGradientBrush(RectangleF rect, Color color1, Color color2, LinearGradientMode mode)
		: this(GetGradientVector(rect, mode), new[] { color1, color2 }, WrapMode.Tile, new Matrix()) { }

	/// <summary>
	///  Creates a new instance of the <see cref='LinearGradientBrush'/> class based on 
	///  a <see cref='Rectangle'/>, starting and ending colors, and orientation.
	/// </summary>
	public LinearGradientBrush(Rectangle rect, Color color1, Color color2, LinearGradientMode mode)
		: this(new RectangleF(rect.m_rect), color1, color2, mode) { }

	/// <summary>
	///  Creates a new instance of the <see cref='LinearGradientBrush'/> class based on 
	///  a <see cref='RectangleF'/>, starting and ending colors, and an orientation angle.
	/// </summary>
	public LinearGradientBrush(RectangleF rect, Color color1, Color color2, float angle, bool isAngleScaleable = false)
		: this(GetGradientVector(rect, angle, isAngleScaleable), new[] { color1, color2 }, WrapMode.Tile, new Matrix()) { }

	/// <summary>
	///  Creates a new instance of the <see cref='LinearGradientBrush'/> class based on
	///  a <see cref='Rectangle'/>, starting and ending colors, and an orientation angle.
	/// </summary>
	public LinearGradientBrush(Rectangle rect, Color color1, Color color2, float angle, bool isAngleScaleable = false)
		: this(new RectangleF(rect.m_rect), color1, color2, angle, isAngleScaleable) { }


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='LinearGradientBrush'/>.
	/// </summary>
	public override object Clone()
	{
		var vector = GetGradientVector(m_rect, LinearGradientMode.ForwardDiagonal);
		return new LinearGradientBrush(vector, m_colors.Colors, m_mode, m_transform);
	}

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets a <see cref='Drawing.Blend'/> that specifies positions and factors 
	///  that define a custom falloff for the gradient.
	/// </summary>
	public Blend Blend 
	{ 
		get => m_blend;
		set => UpdateShader(() => m_blend = value ?? throw new ArgumentNullException(nameof(value)));
	}

	/// <summary>
	///  Gets or sets a value indicating whether gamma correction is enabled for 
	///  this <see cref='LinearGradientBrush'/>.
	/// </summary>
	public bool GammaCorrection 
	{
		get => m_gamma;
		set => UpdateShader(() => m_gamma = value);
	}

	/// <summary>
	///  Gets or sets a <see cref='Drawing.ColorBlend'/> that defines a multicolor 
	///  linear gradient.
	/// </summary>
	public ColorBlend InterpolationColors
	{
		get => m_colors;
		set => UpdateShader(() => 
		{
			var colors = value ?? throw new ArgumentNullException(nameof(value));
			if (colors.Positions[0] != 0 )
				throw new ArgumentException("first element must be equal to 0.", nameof(value));
			if (colors.Positions[value.Positions.Length - 1] != 1)
				throw new ArgumentException("last element must be equal to 1.", nameof(value));
			m_colors = colors;
		});
	}

	/// <summary>
	///  Gets or sets the starting and ending colors of the gradient.
	/// </summary>
	public Color[] LinearColors
	{
		get => m_colors.Colors;
		set => UpdateShader(() => m_colors = new()
		{
			Colors = value,
			Positions = CreateUniformArray(value.Length)
		});
	}

	/// <summary>
	///  Gets a rectangular region that defines the starting and ending points of the gradient.
	/// </summary>
	public RectangleF Rectangle => m_rect;

	/// <summary>
	///  Gets or sets a copy of the <see cref='Matrix'/>  object 
	///  that defines a local geometric transformation for the image associated 
	///  with this <see cref='LinearGradientBrush'/> object.
	/// </summary>
	public Matrix Transform 
	{
		get => m_transform;
		set => UpdateShader(() => m_transform = value);
	}

	/// <summary>
	///  Gets or sets a WrapMode enumeration that indicates the wrap mode for this LinearGradientBrush.
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
	///  geometric transformation of this <see cref='LinearGradientBrush'/> object 
	///  by the specified <see cref='Matrix'/> object in the specified order.
	/// </summary>
	public void MultiplyTransform(Matrix matrix, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Multiply(matrix, order));

	/// <summary>
	///  Resets the Transform property of this <see cref='LinearGradientBrush'/> object to identity.
	/// </summary>
	public void ResetTransform()
		=> UpdateShader(() => Transform.Reset());

	/// <summary>
	///  Rotates the local geometric transformation of this <see cref='LinearGradientBrush'/> object 
	///  by the specified amount in the specified order.
	/// </summary>
	public void RotateTransform(float angle, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Rotate(angle, order));

	/// <summary>
	///  Scales the local geometric transformation of this <see cref='LinearGradientBrush'/> object 
	///  by the specified amounts in the specified order.
	/// </summary>
	public void ScaleTransform(float sx, float sy, MatrixOrder order = MatrixOrder.Prepend)
		=> UpdateShader(() => Transform.Scale(sx, sy, order));

	/// <summary>
	///  Translates the local geometric transformation of this <see cref='LinearGradientBrush'/> object
	///  by the specified dimensions in the specified order.
	/// </summary>
	public void TranslateTransform(float dx, float dy, MatrixOrder order)
		=> UpdateShader(() => Transform.Translate(dx, dy, order));

	#endregion


	#region Utilities

	private struct GradientVector
	{
		public PointF BegPoint { get; internal set; }
		public PointF EndPoint { get; internal set; }
	}

	private static GradientVector GetGradientVector(PointF start, PointF end)
		=> new() { BegPoint = start, EndPoint = end };

	private static GradientVector GetGradientVector(RectangleF rect, LinearGradientMode mode)
	{
		(var begPoint, var endPoint) = mode switch
		{
			LinearGradientMode.Horizontal => ( new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Top) ),
			LinearGradientMode.Vertical => ( new PointF(rect.Left, rect.Top), new PointF(rect.Left, rect.Bottom) ),
			LinearGradientMode.ForwardDiagonal => ( new PointF(rect.Left, rect.Top), new PointF(rect.Right, rect.Bottom) ),
			LinearGradientMode.BackwardDiagonal => ( new PointF(rect.Right, rect.Top), new PointF(rect.Left, rect.Bottom) ),
			_ => throw new ArgumentException($"{mode} mode is not supported.", nameof(mode))
		};
		return GetGradientVector(begPoint, endPoint);
	}

	private static GradientVector GetGradientVector(RectangleF rect, float angle, bool scale)
	{
		double radians = angle * (Math.PI / 180);
		float midWidth = rect.Width / 2f;
		float midHeight = rect.Height / 2f;

		float centerX = rect.Left + midWidth;
		float centerY = rect.Top + midHeight;

		float lengthX = scale ? midWidth : 1f;
		float lengthY = scale ? midHeight : 1f;

		var begPoint = new PointF(
			centerX - lengthX * (float)Math.Cos(radians),
			centerY - lengthY * (float)Math.Sin(radians));

		var endPoint = new PointF(
			centerX + lengthX * (float)Math.Cos(radians),
			centerY + lengthY * (float)Math.Sin(radians));

		return GetGradientVector(begPoint, endPoint);
	}

	private static Color ApplyGamma(Color color, float gamma) 
		=> Color.FromArgb(
			color.A,
			(int)(Math.Pow(color.R / 255.0, gamma) * 255),
			(int)(Math.Pow(color.G / 255.0, gamma) * 255),
			(int)(Math.Pow(color.B / 255.0, gamma) * 255));

	private static float[] CreateUniformArray(int length)
		=> length < 2
			? throw new ArgumentException("at least two items are required.", nameof(length))
			: Enumerable.Range(0, length).Select(i => 1f * i / (length - 1)).ToArray();

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

		var vector = GetGradientVector(m_rect, LinearGradientMode.ForwardDiagonal);
		var gamma = m_gamma ? 2.2f : 1.0f;

		var start = vector.BegPoint.m_point;
		var end = vector.EndPoint.m_point;
		var mode = m_mode == WrapMode.Clamp ? SKShaderTileMode.Decal : SKShaderTileMode.Repeat;
		var matrix = m_transform.m_matrix;

		int index;

		var blend = new Dictionary<float, object>();
		for (index = 0; index < m_blend.Positions.Length; index++)
		{
			var pos = m_blend.Positions[index];
			var fac = m_blend.Factors[index];
			blend[pos] = fac; // blend factor
		}
		for (index = 0; index < m_colors.Positions.Length; index++)
		{
			var pos = m_colors.Positions[index];
			var col = m_colors.Colors[index];
			blend[pos] = col; // specific color
		}

		var lastColor = Color.Empty;
		var blendKeys = blend.Keys.OrderBy(key => key);

		var positions = blendKeys.ToArray();
		var colors = new SKColor[positions.Length];
		
		index = 0;
		foreach (var key in blendKeys)
		{
			var value = blend[key];
			if (value is Color currColor)
			{
				var color = ApplyGamma(currColor, gamma);
				colors[index++] = color.m_color;
				lastColor = currColor;
				continue;
			}
			if (value is float factor)
			{
				var color = ApplyFactor(lastColor, factor);
				colors[index++] = color.m_color;
				continue;
			}
		}

		m_paint.Shader = SKShader.CreateLinearGradient(start, end, colors, positions, mode, matrix);
	}

	#endregion
}

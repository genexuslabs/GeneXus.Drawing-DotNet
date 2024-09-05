using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class LinearGradientBrush : Brush 
{
	private RectangleF m_rect;
	private WrapMode m_mode;
	private Matrix m_transform;
	private Blend m_factors;
	private ColorBlend m_colors;
	private bool m_gamma;

	private LinearGradientBrush(RectangleF rect, Color[] colors, WrapMode mode, Matrix transform)
		: base(new SKPaint { })
	{
		m_rect = rect;
		m_mode = mode;
		m_transform = transform;

		m_gamma = false;
		
		m_factors = new();
		Array.Copy(new[] { 1f }, m_factors.Factors, 1);
		Array.Copy(new[] { 0f }, m_factors.Positions, 1);

		var uniform = GetUniformArray(colors.Length);

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
		: this(GetRectangle(point1, point2), color1, color2, 0, false) { }

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
		: this(rect, color1, color2, GetAngle(mode), false) { }

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
		: this(rect, new[] { color1, color2 }, WrapMode.Tile, GetTransform(rect, angle, isAngleScaleable)) { }

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
		=> new LinearGradientBrush(Rectangle, LinearColors, WrapMode, Transform)
		{
			Blend = Blend,
			InterpolationColors = InterpolationColors,
			GammaCorrection = GammaCorrection
		};

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets a <see cref='Drawing.Blend'/> that specifies positions and factors 
	///  that define a custom falloff for the gradient.
	/// </summary>
	public Blend Blend 
	{ 
		get => m_factors;
		set => UpdateShader(() => m_factors = value ?? throw new ArgumentNullException(nameof(value)));
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
			if (colors.Positions[colors.Positions.Length - 1] != 1 && colors.Positions.Length > 1)
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
			Positions = GetUniformArray(value.Length)
		});
	}

	/// <summary>
	///  Gets a rectangular region that defines the starting and ending points of the gradient.
	/// </summary>
	public RectangleF Rectangle => m_rect;

	/// <summary>
	///  Gets or sets a copy of the <see cref='Matrix'/> object 
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

	/// <summary>
	///  Creates a gradient falloff based on a bell-shaped curve.
	/// </summary>
	public void SetSigmaBellShape(float focus, float scale = 1.0f)
		=> UpdateShader(() => m_factors = GetSigmaBellShape(focus, scale));

	/// <summary>
	///  Creates a linear gradient with a center color and a linear falloff to a single color on both ends.
	/// </summary>
	public void SetBlendTriangularShape(float focus, float scale = 1.0f)
		=> UpdateShader(() => m_factors = GetBlendTriangularShape(focus, scale));

	#endregion


	#region Utilities

	private static RectangleF GetRectangle(PointF point1, PointF point2)
		=> new(point1.X, point1.Y, point1.X + point2.X, point1.Y + point2.Y);

	private static float GetAngle(LinearGradientMode mode) => mode switch
	{
		LinearGradientMode.Horizontal => 0,
		LinearGradientMode.Vertical => 90,
		LinearGradientMode.ForwardDiagonal => 45,
		LinearGradientMode.BackwardDiagonal => 135,
		_ => throw new ArgumentException($"{mode} mode is not supported.", nameof(mode))
	};

	private static float[] GetUniformArray(int length)
		=> length < 2
			? throw new ArgumentException("at least two items are required.", nameof(length))
			: Enumerable.Range(0, length).Select(i => 1f * i / (length - 1)).ToArray();

	static private Matrix GetTransform(RectangleF rect, float angle, bool scale)
	{
		float radians = angle % 360 * (float)(Math.PI / 180);

		float cos = (float)Math.Cos(radians);
		float sin = (float)Math.Sin(radians);

		float absCos = Math.Abs(cos);
		float absSin = Math.Abs(sin);

		float wRatio = (absCos * rect.Width + absSin * rect.Height) / rect.Width;
		float hRatio = (absSin * rect.Width + absCos * rect.Height) / rect.Height;

		float transX = rect.X + rect.Width / 2;
		float transY = rect.Y + rect.Height / 2;

		var transform = new Matrix();
		transform.Translate(transX, transY);
		transform.Rotate(angle);
		transform.Scale(wRatio, hRatio);
		transform.Translate(-transX, -transY);

		if (scale && absCos > 5e-4 && absSin > 5e-4)
		{
			var points = new PointF[3]
			{
				new(rect.Left, rect.Top),
				new(rect.Right, rect.Top),
				new(rect.Left, rect.Bottom),
			};

			transform.TransformPoints(points);

			float ratio = rect.Width /rect.Height;
			if (sin > 0 && cos > 0) 
			{
				float slope = GetSlope(radians, ratio);
				points[0].Y = (slope * (points[0].X - rect.Left)) + rect.Top;
				points[1].X = ((points[1].Y - rect.Bottom) / slope) + rect.Right;
				points[2].X = ((points[2].Y - rect.Top) / slope) + rect.Left;
			}
			else if (sin > 0 && cos < 0)
			{
				float slope = GetSlope(radians - 1 / 2f * Math.PI, ratio);
				points[0].X = ((points[0].Y - rect.Bottom) / slope) + rect.Right;
				points[1].Y = (slope * (points[1].X - rect.Right)) + rect.Bottom;
				points[2].Y = (slope * (points[2].X - rect.Left)) + rect.Top;
			}
			else if (sin < 0 && cos < 0)
			{
				float slope = GetSlope(radians, ratio);
				points[0].Y = (slope * (points[0].X - rect.Right)) + rect.Bottom;
				points[1].X = ((points[1].Y - rect.Top) / slope) + rect.Left;
				points[2].X = ((points[2].Y - rect.Bottom) / slope) + rect.Right;
			}
			else
			{
				float slope = GetSlope(radians - 3 / 2f * Math.PI, ratio);
				points[0].X = ((points[0].Y - rect.Y) / slope) + rect.X;
				points[1].Y = (slope * (points[1].X - rect.Left)) + rect.Top;
				points[2].Y = (slope * (points[2].X - rect.Right)) + rect.Bottom;
			}

			float m11 = (points[1].X - points[0].X) / rect.Width;
			float m12 = (points[1].Y - points[0].Y) / rect.Width;
			float m21 = (points[2].X - points[0].X) / rect.Height;
			float m22 = (points[2].Y - points[0].Y) / rect.Height;

			transform = new Matrix(m11, m12, m21, m22, 0, 0);
			transform.Translate(-rect.X, -rect.Y);
		}

		return transform;

		static float GetSlope(double angleRadians, float aspectRatio)
			=> -1.0f / (aspectRatio * (float)Math.Tan(angleRadians));
	}

	private static Color ApplyGamma(Color color, float gamma) 
		=> Color.FromArgb(
			color.A,
			(int)(Math.Pow(color.R / 255.0, gamma) * 255),
			(int)(Math.Pow(color.G / 255.0, gamma) * 255),
			(int)(Math.Pow(color.B / 255.0, gamma) * 255));

	private void UpdateShader(Action action)
	{
		action();

		var points = new PointF[] { new(m_rect.Left, 0), new(m_rect.Right, 0) };

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
		transform.TransformPoints(points);
		transform.Reset();

		var start = points[0].m_point;
		var end = points[1].m_point;
		var matrix = transform.m_matrix;
		var gamma = m_gamma ? 2.2f : 1.0f;
		var mode = m_mode == WrapMode.Clamp ? SKShaderTileMode.Decal : SKShaderTileMode.Repeat;

		var blend = new Dictionary<float, object>();
		if (m_factors.Positions.Length > 1)
		{
			for (int index = 0; index < m_factors.Positions.Length; index++)
			{
				var pos = m_factors.Positions[index];
				var fac = m_factors.Factors[index];
				blend[pos] = fac; // blend factor
			}
		}
		if (m_colors.Positions.Length > 1)
		{
			for (int index = 0; index < m_colors.Positions.Length; index++)
			{
				var pos = m_colors.Positions[index];
				var col = m_colors.Colors[index];
				blend[pos] = col; // specific color
			}
		}

		var positions = blend.Keys.OrderBy(key => key).ToArray();
		var colors = new SKColor[positions.Length];
	
		var lastColor = m_colors.Colors[0];
		for (int index = 0; index < positions.Length; index++)
		{
			var key = positions[index];
			var value = blend[key];
			if (value is Color currColor)
			{
				var color = ApplyGamma(currColor, gamma);
				colors[index] = color.m_color;
				lastColor = currColor;
				continue;
			}
			if (value is float factor)
			{
				var nextColor = m_colors.Colors[m_colors.Colors.Length - 1];
				for (int i = index + 1; i < positions.Length; i++)
				{
					key = positions[i];
					value = blend[key];
					if (value is Color foundColor)
					{
						nextColor = foundColor;
						break;
					}
				}
				var color = Color.Blend(lastColor, nextColor, factor);
				colors[index] = color.m_color;
				continue;
			}
		}

		m_paint.Shader = SKShader.CreateLinearGradient(start, end, colors, positions, mode, matrix);
	}

	#endregion
}

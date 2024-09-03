using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class PathGradientBrush : Brush
{
	private GraphicsPath m_path;
	private WrapMode m_mode;
	private Matrix m_transform;
	private Blend m_factors;
	private ColorBlend m_colors;
	private PointF m_center, m_focus;
	private Color? m_color;
	private Color[] m_surround;

	private PathGradientBrush(GraphicsPath path, WrapMode mode, Matrix transform)
		: base(new SKPaint { })
	{
		var points = path.PathPoints;
		if (points.First() == points.Last())
			points = points.Take(points.Length - 1).ToArray();

		m_path = path;
		m_mode = mode;
		m_transform = transform;

		m_factors = new();
		Array.Copy(new[] { 1f }, m_factors.Factors, 1);
		Array.Copy(new[] { 0f }, m_factors.Positions, 1);

		m_colors = new();
		Array.Copy(new[] { Color.Empty }, m_colors.Colors, 1);
		Array.Copy(new[] { 0f }, m_colors.Positions, 1);

		m_center = new(
			points.Average(pt => pt.X),
			points.Average(pt => pt.Y));

		m_focus = new(0, 0);
		m_surround = new[] { Color.White };

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
		get => m_factors;
		set => UpdateShader(() => m_factors = value ?? throw new ArgumentNullException(nameof(value)));
	}

	/// <summary>
	///  Gets or sets the <see cref='Color'/> at the center of the path gradient.
	/// </summary>
	public Color CenterColor
	{
		get => m_color ?? (m_surround.Length > 1 ? Color.Black : Color.White);
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
			if (interpolation.Positions[0] != 0)
				throw new ArgumentException("first element must be equal to 0.", nameof(value));
			if (interpolation.Positions[interpolation.Positions.Length - 1] != 1 && interpolation.Positions.Length > 1)
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
		set => UpdateShader(() =>
		{
			if (value.Length > m_path.PointCount)
				throw new ArgumentException("parameter is not valid.", nameof(value));
			m_surround = value;
		});
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
		=> UpdateShader(() => m_factors = GetBlendTriangularShape(focus, scale));

	/// <summary>
	///  Creates a gradient brush that changes color starting from the center of the path outward
	///  to the path's boundary. The transition from one color to another is based on a bell-shaped curve.
	/// </summary>
	public void SetSigmaBellShape(float focus, float scale = 1.0f)
		=> UpdateShader(() => m_factors = GetSigmaBellShape(focus, scale));

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
		for (int i = 1; i < types.Length; i++)
			types[i] = (byte)PathPointType.Line;
		types[types.Length - 1] |= (byte)PathPointType.CloseSubpath;
		return new GraphicsPath(points, types);
	}

	private static Vector2 Intersect(Vector2 p0, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		Vector2 d1 = p1 - p0; // line p0-p1
		Vector2 d2 = p3 - p2; // line p2-p3

		float det = d1.X * d2.Y - d1.Y * d2.X;
		if (Math.Abs(det) < 1e-6) // check if lines are parallel
			return new Vector2(float.MaxValue, float.MaxValue);

		// point the in the intersection
		float t = ((p2.X - p0.X) * d2.Y - (p2.Y - p0.Y) * d2.X) / det;
		return p0 + t * d1;
	}

	private static bool Triangulated(Vector2 pt, Vector2 p0, Vector2 p1, Vector2 p2)
	{
		bool EdgeCheck(Vector2 a, Vector2 b)
			=> (a.Y <= pt.Y && pt.Y < b.Y || b.Y <= pt.Y && pt.Y < a.Y)
			&& (pt.X + 1e-5 < (b.X - a.X) * (pt.Y - a.Y) / (b.Y - a.Y) + a.X);

		// check if point is in the triangle by checking the edges
		return EdgeCheck(p0, p2) ^ EdgeCheck(p1, p0) ^ EdgeCheck(p2, p1);
	}

	private static Vector2 Project(Vector2 pt, Vector2 p0, Vector2 p1)
	{
		var e0 = p1 - p0;
		var e1 = pt - p0;

		// point projection in the line p0-p1
		float t = Vector2.Dot(e0, e1) / Vector2.Dot(e0, e0);
		return p0 + e0 * Math.Min(1, Math.Max(0, t));
	}

	private static Vector2 Bezier(Vector2 p0, Vector2 p1, Vector2 p2, float percent)
	{
		var c0 = Vector2.Lerp(p0, p1, percent);
		var c1 = Vector2.Lerp(p1, p2, percent);

		// point in the bezier curve p0-p1-p2 at percent
		return Vector2.Lerp(c0, c1, percent);
	}

	private Color ComputeColor(int x, int y, Vector2 center, Vector2[] outer, Vector2[] inner, PathPointType[] types, Color[] colors, float[] positions)
	{
		var color = Color.Transparent;
		if (m_path.IsVisible(x, y))
		{
			var target = new Vector2(x, y);
			if (m_surround.Length > 1)
			{
				// determine the weights of this point to the corners of the path
				var weight = new float[outer.Length];
				for (int i = 0; i < outer.Length; i++)
				{
					var p0 = outer[(i + 0) % outer.Length];
					var p1 = outer[(i + 1) % outer.Length];

					var ep = Project(target, p0, p1);

					int j = (i + outer.Length - 1) % outer.Length;
					weight[j] = Vector2.Distance(target, ep);
				}

				float total = weight.Sum(); // for normalizing in 0..1

				// determine the blended color for the given point by weights
				color = colors[colors.Length - 1];
				for (int i = 0; i < weight.Length; i++)
				{
					float amount = weight[i] / total;
					int j = Math.Min(i, colors.Length - 1); // NOTE: there could be less colors than vertices
					color = Color.Blend(color, colors[j], amount);
				}

				// change the colors and positions according to surrounded colors
				colors = new[] { color, colors[colors.Length - 1] };
				positions = new[] { 0f, 1f };
			}

			// determine the distance of this point to the edge of the path
			float dist = float.MaxValue, dmax = 0f;
			for (int i = 0; i < outer.Length; i++)
			{
				var p0 = outer[(i + 0) % outer.Length];
				var p1 = outer[(i + 1) % outer.Length];
				var p2 = outer[(i + 2) % outer.Length];

				var c0 = inner[(i + 0) % inner.Length];
				var c1 = inner[(i + 1) % inner.Length];
				var c2 = inner[(i + 2) % inner.Length];

				var type = types[i % types.Length];
				switch(type)
				{
					case PathPointType.Line:
						var ep = Intersect(target, center, p0, p1);
						if (Triangulated(target, p0, p1, center))
						{
							var ip = Intersect(target, center, c0, c1);
							var pp = Project(ip, c0, c1);

							dist = Vector2.Distance(target, ep);
							dmax = Vector2.Distance(pp, ep);

							i = outer.Length; // break the loop
						}
						break;

					case PathPointType.Bezier:
						for (float t = 0; t <= 1; t += 0.1f)
						{
							var bp = Bezier(p0, p1, p2, t);
							var cp = Bezier(c0, c1, c2, t);

							var distCp = Vector2.Distance(target, bp);
							var dmaxCp = Vector2.Distance(cp, bp);

							if (distCp < dist)
							{
								dist = distCp;
								dmax = dmaxCp;
							}
						}
						i++;
						break;

					default:
						throw new ArgumentException($"unknown type 0x{type:X2} at index {i}", nameof(types));
				}
			}

			// normalized in 0..1
			dist /= dmax;

			// determine the blended color for the given point by positions
			color = colors[colors.Length - 1];
			for (int i = 0; i < positions.Length - 1; i++)
			{
				if (dist >= positions[i] && dist < positions[i + 1])
				{
					float amount = (dist - positions[i]) / (positions[i + 1] - positions[i]);
					color = Color.Blend(colors[i], colors[i + 1], amount);
					break;
				}
			}
		}
		return color;
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

		var bounds = m_path.GetBounds();
		var center = m_center.ToVector2();
		
		var outer = m_path.PathPoints
			.Select(point => point.ToVector2())
			.ToArray();
		var types = m_path.PathTypes
			.Select(type => (PathPointType)(type & (byte)PathPointType.PathTypeMask))
			.Skip(1) // skip PathPointType.Start
			.ToArray();

		var focus = Matrix3x2.CreateScale(m_focus.X, m_focus.Y);
		var inner = outer
			.Select(point => Vector2.Transform(point - center, focus) + center)
			.ToArray();

		var blend = new Dictionary<float, object>() { [0] = m_surround[0], [1] = m_color };
		if (m_surround.Length > 1)
		{
			blend[1] ??= Color.Black;
			for (int index = 0; index < m_surround.Length; index++)
			{
				var pos = 1f * index / m_surround.Length;
				var col = m_surround[index];
				blend[pos] = col; // corner color
			}
		}
		else
		{
			blend[1] ??= Color.White;
			if (m_factors.Positions.Length > 1)
			{
				for (int index = 0; index < m_factors.Positions.Length; index++)
				{
					var pos = m_factors.Positions[index];
					var fac = m_factors.Factors[index];
					blend[pos] = fac; // edge factor
				}
			}
			if (m_colors.Positions.Length > 1)
			{
				for (int index = 0; index < m_colors.Positions.Length; index++)
				{
					var pos = m_colors.Positions[index];
					var col = m_colors.Colors[index];
					blend[pos] = col; // edge color
				}
			}
		}

		var positions = blend.Keys.OrderBy(key => key).ToArray();
		var colors = new Color[positions.Length];

		var lastColor = m_surround[0];
		for (int index = 0; index < positions.Length; index++)
		{
			var key = positions[index];
			var value = blend[key];
			if (value is Color currColor)
			{
				colors[index] = currColor;
				lastColor = currColor;
				continue;
			}
			if (value is float factor)
			{
				var nextColor = m_color ?? Color.White;
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
				colors[index] = color;
				continue;
			}
		}

		// NOTE: Skia does not offers path gradient shader, that's why we use a bitmap
		using var bitmap = new Bitmap(bounds.Width + bounds.Left, bounds.Height + bounds.Top);
		for (int x = 0; x < bitmap.Width; x++)
		{
			for (int y = 0; y < bitmap.Height; y++)
			{
				var color = ComputeColor(x, y, center, outer, inner, types, colors, positions);
				bitmap.SetPixel(x, y, color);
			}
		}

		var source = bitmap.m_bitmap;
		var matrix = transform.m_matrix;
		var mode = m_mode == WrapMode.Clamp ? SKShaderTileMode.Decal : SKShaderTileMode.Repeat;

		m_paint.Shader = SKShader.CreateBitmap(source, mode, mode, matrix);
	}

	#endregion
}

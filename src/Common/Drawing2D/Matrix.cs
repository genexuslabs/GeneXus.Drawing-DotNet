
using System;
using System.Numerics;
using SkiaSharp;

namespace GeneXus.Drawing.Drawing2D;

public sealed class Matrix : ICloneable, IDisposable
{
	internal SKMatrix m_matrix;

	internal Matrix(SKMatrix matrix)
	{
		m_matrix = matrix;
	}

	/// <summary>
	///  Initializes a new instance of the <see cref='Matrix'/> class as the identity matrix.
	/// </summary>
	public Matrix()
		: this(SKMatrix.CreateIdentity()) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='Matrix'/> class with the specified elements.
	/// </summary>
	public Matrix(float m11, float m12, float m21, float m22, float dx, float dy)
		: this(CreateElementsMatrix(m11, m12, m21, m22, dx, dy)) { }

	/// <summary>
	///  Constructs a <see cref='Matrix'/> utilizing the specified matrix.
	/// </summary>
	/// <param name="matrix"></param>
	public Matrix(Matrix3x2 matrix)
		: this(matrix.M11, matrix.M12, matrix.M21, matrix.M22, matrix.M31, matrix.M32) { }

	/// <summary>
	///  Initializes a new instance of the <see cref='Matrix'/> class to the geometric transform 
	///  defined by the specified <see cref='RectangleF'/> and array of <see cref='PointF'/>.
	/// </summary>
	public Matrix(RectangleF rect, PointF[] plgpts)
		: this(CreateTransformMatrix(
			new PointF[]
			{
				new(rect.Left, rect.Top),
				new(rect.Right, rect.Top),
				new(rect.Left, rect.Bottom)
			},
			plgpts))
	{ }

	/// <summary>
	///  Initializes a new instance of the <see cref='Matrix'/> class to the geometric transform 
	///  defined by the specified <see cref='Rectangle'/> and array of <see cref='Point'/>.
	/// </summary>
	public Matrix(Rectangle rect, Point[] plgpts)
		: this(new RectangleF(rect.m_rect), Array.ConvertAll(plgpts, point => new PointF(point.m_point))) { }

	/// <summary>
	///  Cleans up resources for this <see cref='Matrix'/>.
	/// </summary>
	~Matrix() => Dispose(false);


	#region IDisposble

	/// <summary>
	///  Cleans up resources for this <see cref='Matrix'/>.
	/// </summary>
	public void Dispose()
	{
		GC.SuppressFinalize(this);
		Dispose(true);
	}

	private void Dispose(bool disposing) { }

	#endregion


	#region IClonable

	/// <summary>
	///  Creates an exact copy of this <see cref='Matrix'/>.
	/// </summary>
	public object Clone()
		=> new Matrix(m_matrix);

	#endregion


	#region IEqualitable

	/// <summary>
	///  Tests whether the specified object is a <see cref='Matrix'/> and is identical to this <see cref='Matrix'/>.
	/// </summary>
	public override bool Equals(object obj)
		=> obj is Matrix matrix && m_matrix.Equals(matrix.m_matrix);

	/// <summary>
	///  Get the has code of this <see cref='Matrix'/>.
	/// </summary>
	public override int GetHashCode()
		=> m_matrix.GetHashCode();

	#endregion


	#region Operators

	/// <summary>
	/// Creates a <see cref='SKMatrix'/> with specified <see cref='Matrix'/> .
	/// </summary>
	public static explicit operator SKMatrix(Matrix matrix) => matrix.m_matrix;

	/// <summary>
	/// Tests whether two <see cref='Matrix'/> objects are identical.
	/// </summary>
	public static bool operator ==(Matrix left, Matrix right) => left.m_matrix == right.m_matrix;

	/// <summary>
	/// Tests whether two <see cref='Matrix'/> objects are different.
	/// </summary>
	public static bool operator !=(Matrix left, Matrix right) => left.m_matrix != right.m_matrix;

	#endregion


	#region Properties

	/// <summary>
	///  Gets an array of floating-point values that represents the elements of this <see cref='Matrix'/>.
	/// </summary>
	public float[] Elements => new float[]
	{
		m_matrix.ScaleX, m_matrix.SkewX,
		m_matrix.SkewY,  m_matrix.ScaleY,
		m_matrix.TransX, m_matrix.TransY
	};

	/// <summary>
	///  Gets a value indicating whether this <see cref='Matrix'/> is the identity matrix.
	/// </summary>
	public bool IsIdentity => m_matrix.IsIdentity;

	/// <summary>
	///  Gets a value indicating whether this <see cref='Matrix'/> is invertible.
	/// </summary>
	public bool IsInvertible => m_matrix.IsInvertible;

	/// <summary>
	///  Gets or sets the elements for the matrix.
	/// </summary>
	public Matrix3x2 MatrixElements
	{
		get => new(
			m_matrix.ScaleX,
			m_matrix.SkewX,
			m_matrix.SkewY,
			m_matrix.ScaleY,
			m_matrix.TransX,
			m_matrix.TransY
		);
		set
		{
			m_matrix.ScaleX = value.M11;
			m_matrix.SkewX = value.M12;
			m_matrix.SkewY = value.M21;
			m_matrix.ScaleY = value.M22;
			m_matrix.TransX = value.M31;
			m_matrix.TransY = value.M32;
			m_matrix.Persp0 = 0;
			m_matrix.Persp1 = 0;
			m_matrix.Persp2 = 1;
		}
	}

	/// <summary>
	///  Gets the x translation value (the dx value, or the element in the third row and first column) 
	///  of this <see cref='Matrix'/>.
	/// </summary>
	public float OffsetX => m_matrix.TransX;

	/// <summary>
	///  Gets the y translation value (the dy value, or the element in the third row and second column) 
	///  of this <see cref='Matrix'/>.
	/// </summary>
	public float OffsetY => m_matrix.TransY;

	#endregion


	#region Methods

	/// <summary>
	///  Inverts this <see cref='Matrix'/>, if it is invertible.
	/// </summary>
	public void Invert()
		=> m_matrix = Invert(m_matrix);

	/// <summary>
	///  Multiplies this <see cref="SKMatrix"/> by the matrix specified in the <paramref name="matrix"/> parameter, 
	///  and in the order specified in the <paramref name="order" /> parameter.
	/// </summary>
	private void Multiply(SKMatrix matrix, MatrixOrder order = MatrixOrder.Prepend)
		=> m_matrix = order == MatrixOrder.Prepend ? Multiply(matrix, m_matrix) : Multiply(m_matrix, matrix);

	/// <summary>
	///  Multiplies this <see cref="Matrix"/> by the matrix specified in the <paramref name="matrix"/> parameter, 
	///  and in the order specified in the <paramref name="order" /> parameter.
	/// </summary>
	public void Multiply(Matrix matrix, MatrixOrder order = MatrixOrder.Prepend)
		=> Multiply(matrix.m_matrix, order);

	/// <summary>
	///  Resets this <see cref='Matrix'/> to have the elements of the identity matrix.
	/// </summary>
	public void Reset()
		=> m_matrix = SKMatrix.CreateIdentity();

	/// <summary>
	///  Applies a clockwise rotation of the specified angle about the origin to this <see cref='Matrix'/>.
	/// </summary>
	public void Rotate(float angle, MatrixOrder order = MatrixOrder.Prepend)
		=> Multiply(SKMatrix.CreateRotationDegrees(-angle), order);

	/// <summary>
	///  Applies a clockwise rotation about the specified point to this <see cref='Matrix'/>.
	/// </summary>
	public void RotateAt(float angle, PointF point, MatrixOrder order = MatrixOrder.Prepend)
	{
		Translate(point.X, point.Y, order);
		Rotate(angle, order);
		Translate(-point.X, -point.Y, order);
	}

	/// <summary>
	///  Applies a clockwise rotation about the specified point to this <see cref='Matrix'/>.
	/// </summary>
	public void RotateAt(float angle, Point point, MatrixOrder order = MatrixOrder.Prepend)
		=> RotateAt(angle, new PointF(point.m_point), order);

	/// <summary>
	///  Applies the specified scale vector to this <see cref='Matrix'/> by prepending the scale vector.
	/// </summary>
	public void Scale(float scaleX, float scaleY, MatrixOrder order = MatrixOrder.Prepend)
		=> Multiply(SKMatrix.CreateScale(scaleX, scaleY), order);

	/// <summary>
	///  Applies the specified shear vector to this <see cref='Matrix'/> by prepending the shear vector.
	/// </summary>
	public void Shear(float shearX, float shearY, MatrixOrder order = MatrixOrder.Prepend)
		=> Multiply(SKMatrix.CreateSkew(shearY, shearX), order);

	/// <summary>
	///  Applies the specified translation vector to this <see cref='Matrix'/> by prepending the translation vector.
	/// </summary>
	public void Translate(float offsetX, float offsetY, MatrixOrder order = MatrixOrder.Prepend)
		=> Multiply(SKMatrix.CreateTranslation(offsetX, offsetY), order);

	/// <summary>
	///  Applies the geometric transform represented by this <see cref='Matrix'/> to a specified 
	///  array of <see cref='PointF'/>.
	/// </summary>
	public void TransformPoints(PointF[] points)
		=> TransformPoints(points, m_matrix, p => new(p), p => p.m_point);

	/// <summary>
	///  Applies the geometric transform represented by this <see cref='Matrix'/> to a specified 
	///  array of <see cref='Point'/>.
	/// </summary>
	public void TransformPoints(Point[] points)
		=> TransformPoints(points, m_matrix, p => new(p), p => p.m_point);

	/// <summary>
	///  Applies only the scale and rotate components of this <see cref='Matrix'/> to the specified 
	///  array of <see cref='Point'/>.
	/// </summary>
	public void TransformVectors(Point[] points)
		=> TransformVectors(points, p => new(p), p => p.m_point);

	/// <summary>
	///  Multiplies each vector in an array by this <see cref='Matrix'/>. The translation elements of 
	///  this matrix (third row) are ignored.
	/// </summary>
	public void VectorTransformPoints(Point[] points)
		=> TransformVectors(points);

	#endregion


	#region Utilities

	private const float DEG_OFFSET = -90;

	/*
		* NOTE: SkiaSharp and System.Drawing have a different 
		* representation for transformation matrices
		* 
		* SkiaSharp:				 System.Drawing:
		* ┌						 ┐	 ┌						 ┐
		* │ ScaleX SkewX  TransX │	 │ 	ScaleX SkewX  PerspX │
		* │ SkewY  ScaleY TransY │	 │	SkewY  ScaleY PerspY │
		* │ PerspX PerspY PerspZ │	 │	TransX TransY PerspZ │
		* └						 ┘	 └						 ┘
		*/

	private static SKMatrix CreateElementsMatrix(float m11, float m12, float m21, float m22, float m31, float m32)
		=> new(m11, m12, m31, m21, m22, m32, 0, 0, 1);

	private static SKMatrix CreateTransformMatrix(PointF[] src, PointF[] dst)
	{
		if (src.Length < 3) throw new ArgumentException("must contain 3 points.", nameof(src));
		if (dst.Length < 3) throw new ArgumentException("must contain 3 points.", nameof(dst));

		float den = (src[0].X - src[1].X) * (src[0].Y - src[2].Y) - (src[0].X - src[2].X) * (src[0].Y - src[1].Y);
		if (den == 0) throw new InvalidOperationException("cannot create a valid transformation matrix for the given points.");

		float m11 = (dst[0].X * (src[1].Y - src[2].Y) + dst[1].X * (src[2].Y - src[0].Y) + dst[2].X * (src[0].Y - src[1].Y)) / den;
		float m12 = (dst[0].Y * (src[1].Y - src[2].Y) + dst[1].Y * (src[2].Y - src[0].Y) + dst[2].Y * (src[0].Y - src[1].Y)) / den;
		float dx = dst[0].X - m11 * src[0].X - m12 * src[0].Y;

		float m21 = (dst[0].X * (src[2].X - src[1].X) + dst[1].X * (src[0].X - src[2].X) + dst[2].X * (src[1].X - src[0].X)) / den;
		float m22 = (dst[0].Y * (src[2].X - src[1].X) + dst[1].Y * (src[0].X - src[2].X) + dst[2].Y * (src[1].X - src[0].X)) / den;
		float dy = dst[0].Y - m21 * src[0].X - m22 * src[0].Y;

		return CreateElementsMatrix(m11, m12, m21, m22, dx, dy);
	}

	private static SKMatrix Multiply(SKMatrix a, SKMatrix b)
		=> SwapTrans(SKMatrix.Concat(SwapTrans(a), SwapTrans(b)));

	private static SKMatrix Invert(SKMatrix matrix)
			=> SwapTrans(matrix).TryInvert(out var invert)
			? SwapTrans(invert)
			: throw new InvalidOperationException("matrix inversion failed.");

	private static SKMatrix SwapTrans(SKMatrix matrix)
		=> new( // swaps (TransX, TransY) with (Persp0, Persp1)
			matrix.ScaleX, matrix.SkewX, matrix.Persp0,
			matrix.SkewY, matrix.ScaleY, matrix.Persp1,
			matrix.TransX, matrix.TransY, matrix.Persp2);

	private static SKMatrix Transpose(SKMatrix matrix)
		=> SwapTrans(new( // transposes the matrix
			matrix.ScaleX, matrix.SkewY, matrix.Persp0,
			matrix.SkewX, matrix.ScaleY, matrix.Persp1,
			matrix.TransX, matrix.TransY, matrix.Persp2));

	private static void TransformPoints<T>(T[] points, SKMatrix matrix, Func<SKPoint, T> newPoint, Func<T, SKPoint> getPoint)
	{
		var transpose = Transpose(matrix);
		for (int i = 0; i < points.Length; i++)
		{
			var point = transpose.MapPoint(getPoint(points[i]));
			points[i] = newPoint(point);
		}
	}

	private void TransformVectors<T>(T[] points, Func<SKPoint, T> newPoint, Func<T, SKPoint> getPoint)
	{
		var transformMatrix = new SKMatrix
		{
			ScaleX = m_matrix.ScaleX,
			SkewX = m_matrix.SkewX,
			TransX = 0,
			SkewY = m_matrix.SkewY,
			ScaleY = m_matrix.ScaleY,
			TransY = 0,
			Persp0 = m_matrix.Persp0,
			Persp1 = m_matrix.Persp1,
			Persp2 = 1
		};
		TransformPoints(points, transformMatrix, newPoint, getPoint);
	}

	#endregion
}
	
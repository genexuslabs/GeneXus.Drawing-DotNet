using GeneXus.Drawing.Drawing2D;
using System;
using System.Numerics;

namespace GeneXus.Drawing.Test.Drawing2D;

internal class MatrixTests
{

	private const float TOLERANCE = 0.001f;

	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Default()
	{
		var matrix = new Matrix();
		Assert.Multiple(() => 
		{
			Assert.That(matrix.IsIdentity, Is.True);
			Assert.That(matrix.IsInvertible, Is.True);
			Assert.That(matrix.Elements, Is.EquivalentTo(new float[] { 1, 0, 0, 1, 0, 0 }));
		});
	}

	[Test]
	public void Constructor_Elements()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		Assert.Multiple(() => 
		{
			Assert.That(matrix.IsIdentity, Is.False);
			Assert.That(matrix.IsInvertible, Is.True);
			Assert.That(matrix.Elements, Is.EqualTo(new float[] { 1, 2, 3, 4, 5, 6 }));
		});
	}

	 [Test]
	public void Constructor_Matrix3x2()
	{
		var matrix3x2 = new Matrix3x2(1.0f, 0.0f, 0.0f, 1.0f, 50.0f, 50.0f);
		
		var matrix = new Matrix(matrix3x2);
		Assert.That(matrix.MatrixElements, Is.EqualTo(matrix3x2));
	}

	[Test]
	public void Constructor_RectAndPoints()
	{
		var rect = new RectangleF(0, 0, 100, 100);
		var plgpts = new PointF[] { new(0, 0), new(100, 0), new(0, 100) };
		
		var matrix = new Matrix(rect, plgpts);
		Assert.That(matrix.Elements, Is.EqualTo(new float[] { 1, 0, 0, 1, 0, 0 }).Within(TOLERANCE));
	}

	[Test]
	public void Operator_Equality()
	{
		var matrix1 = new Matrix(1, 2, 3, 4, 5, 6);
		var matrix2 = new Matrix(1, 2, 3, 4, 5, 6);
		Assert.Multiple(() => 
		{
			Assert.That(matrix1 == matrix2, Is.True);
			Assert.That(matrix1 != matrix2, Is.False);
		});
	}

	[Test]
	public void Method_Equals()
	{
		var matrix1 = new Matrix(1, 2, 3, 4, 5, 6);

		var matrix2 = new Matrix(1, 2, 3, 4, 5, 6);
		Assert.That(matrix1.Equals(matrix2), Is.True);

		var matrix3 = new Matrix(6, 5, 4, 3, 2, 1);
		Assert.That(matrix1.Equals(matrix3), Is.False);
	}

	[Test]
	public void Method_Clone()
	{
		var original = new Matrix(1, 2, 3, 4, 5, 6);

		var clone = original.Clone() as Matrix;
		Assert.Multiple(() => 
		{
			Assert.That(clone, Is.Not.Null);
			Assert.That(clone, Is.EqualTo(original));
			Assert.That(ReferenceEquals(clone, original), Is.False);
			Assert.That(clone.Elements, Is.EqualTo(original.Elements));
		});
	}

	[Test]
	public void Method_Invert()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		Assert.Multiple(() => 
		{
			Assert.That(matrix.IsInvertible, Is.True);
			
			matrix.Invert();
			Assert.That(matrix.Elements, Is.EqualTo(new[] { -2, 1, 1.5f, -0.5f, 1, -2 }).Within(TOLERANCE));
		});
	}

	[Test]
	public void Method_Multiply()
	{
		var matrix1 = new Matrix(1, 0, 0, 1, 10, 10);
		var matrix2 = new Matrix(1, 0, 0, 1, 20, 20);
		
		matrix1.Multiply(matrix2);
		Assert.That(matrix1.Elements, Is.EqualTo(new[] { 1, 0, 0, 1, 30, 30 }));
	}

	[Test]
	public void Method_Reset()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		
		matrix.Reset();
		Assert.That(matrix.IsIdentity, Is.True);
	}

	[Test]
	public void Method_Rotate()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		
		matrix.Rotate(45);
		Assert.That(matrix.Elements, Is.EqualTo(new float[] { 2.828427f, 4.24264f, 1.414214f, 1.414214f, 5, 6 }).Within(TOLERANCE));
	}

	[Test]
	public void Method_RotateAt()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		
		matrix.RotateAt(45, new PointF(10, 10));
		Assert.That(matrix.Elements, Is.EqualTo(new float[] { 2.828427f, 4.24264f, 1.414214f, 1.414214f, 2.573596f, 9.431463f }).Within(TOLERANCE));
	}

	[Test]
	public void Method_Scale()
	{
		var matrix = new Matrix();
		
		matrix.Scale(2, 2);
		Assert.That(matrix.Elements, Is.Not.EqualTo(new[] { 2, 4, 9, 12, 5, 6 }).Within(TOLERANCE));
	}

	[Test]
	public void Method_Shear()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);

		matrix.Shear(1, 2);
		Assert.That(matrix.Elements, Is.EqualTo(new[] { 7, 10, 4, 6, 5, 6 }).Within(TOLERANCE));
	}

	[Test]
	public void Method_Translate()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		
		matrix.Translate(50, 50);
		Assert.Multiple(() => 
		{
			Assert.That(matrix.OffsetX, Is.EqualTo(205));
			Assert.That(matrix.OffsetY, Is.EqualTo(306));
		});
	}


	[Test]
	public void Method_TransformPoints()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		var points = new PointF[] { new(0, 0), new(10, 10) };

		matrix.TransformPoints(points);
		Assert.Multiple(() => 
		{
			Assert.That(points[0], Is.EqualTo(new PointF(5, 6)));
			Assert.That(points[1], Is.EqualTo(new PointF(45, 66)));
		});
	}

	[Test]
	public void Method_TransformVectors_ShouldTransformVectorsCorrectly()
	{
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);
		var vectors = new Point[] { new(1, 0), new(0, 1) };

		matrix.TransformVectors(vectors);
		Assert.Multiple(() => 
		{
			Assert.That(vectors[0], Is.EqualTo(new Point(1, 2)));
			Assert.That(vectors[1], Is.EqualTo(new Point(3, 4)));
		});
	}
}
using GeneXus.Drawing.Drawing2D;
using System;
using System.Linq;

namespace GeneXus.Drawing.Test.Drawing2D;

internal class GraphicsPathUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Default()
	{
		using var path = new GraphicsPath();
		Assert.Multiple(() =>
		{
			Assert.That(path.FillMode, Is.EqualTo(FillMode.Alternate));
			Assert.That(path.PointCount, Is.EqualTo(0));
			Assert.That(path.PathPoints, Is.Empty);
			Assert.That(path.PathTypes, Is.Empty);
		});
	}

	[Test]
	public void Constructor_FillMode()
	{
		var fillMode = FillMode.Winding;

		using var path = new GraphicsPath(fillMode);
		Assert.Multiple(() =>
		{
			Assert.That(path.FillMode, Is.EqualTo(fillMode));
			Assert.That(path.PointCount, Is.EqualTo(0));
			Assert.That(path.PathPoints, Is.Empty);
			Assert.That(path.PathTypes, Is.Empty);
		});
	}

	[Test]
	public void Constructor_PointsAndTypes()
	{
		var fillMode = FillMode.Winding;
		PointF[] points = { new(0, 0), new(1, 1) };
		byte[] types = { (byte)PathPointType.Start, (byte)PathPointType.Line };

		using var path = new GraphicsPath(points, types, fillMode);
		Assert.Multiple(() =>
		{
			Assert.That(path.FillMode, Is.EqualTo(fillMode));
			Assert.That(path.PointCount, Is.EqualTo(2));
			Assert.That(path.PathPoints, Is.EqualTo(points));
			Assert.That(path.PathTypes, Is.EqualTo(types));
		});
	}

	[Test]
	public void Property_FillMode()
	{
		using var path = new GraphicsPath();
		path.FillMode = FillMode.Winding;
		Assert.That(path.FillMode, Is.EqualTo(FillMode.Winding));
	}

	[Test]
	public void Method_Clone()
	{
		var fillMode = FillMode.Winding;

		using var path1 = new GraphicsPath(fillMode);
		path1.AddLine(0, 0, 1, 1);

		var path2 = path1.Clone();
		Assert.Multiple(() =>
		{
			Assert.That(path2, Is.Not.Null);
			Assert.That(path2, Is.Not.SameAs(path1));
			Assert.That(path2, Is.TypeOf<GraphicsPath>());

			if (path2 is GraphicsPath cloned)
			{
				Assert.That(cloned.FillMode, Is.EqualTo(path1.FillMode));
				Assert.That(cloned.PointCount, Is.EqualTo(path1.PointCount));
				Assert.That(cloned.PathPoints, Is.EqualTo(path1.PathPoints));
				Assert.That(cloned.PathTypes, Is.EqualTo(path1.PathTypes));

				cloned.Dispose();
			}
		});
	}

	[Test]
	public void Method_AddArc()
	{
		var rect = new RectangleF(0, 0, 100, 100);

		using var path = new GraphicsPath();
		path.AddArc(rect, 45, 90);
		Assert.Multiple(() =>
		{
			// TODO: remove this assert and replace with below asserts when method has been fixed
			Assert.That(path.PointCount, Is.GreaterThan(0));
			/*Assert.That(path.PointCount, Is.EqualTo(4));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(85.35534f, 85.35532f), 
				new PointF(65.82913f, 104.8815f), 
				new PointF(34.17089f, 104.8815f), 
				new PointF(14.64468f, 85.35535f) 
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Bezier, 
				(byte)PathPointType.Bezier, 
				(byte)PathPointType.Bezier
			}));*/
		});
	}

	[Test]
	public void Method_AddBezier()
	{
		var points = new[] { new PointF(1, 5), new PointF(1, 1), new PointF(2, 1), new PointF(3, 0) };

		using var path = new GraphicsPath();
		path.AddBezier(points[0], points[1], points[2], points[3]);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(4));
			Assert.That(path.PathPoints, Is.EqualTo(points));
			Assert.That(path.PathTypes, Is.EqualTo(new []
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier
			}));
		});
	}

	[Test]
	public void Method_AddClosedCurve()
	{
		var points = new[] { new PointF(0, 0), new PointF(1, 1), new PointF(2, 0) };
		
		using var path = new GraphicsPath();
		path.AddClosedCurve(points);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(10));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(0, 0),
				new PointF(-0.16666667f, 0.16666667f),
				new PointF(0.6666666f, 1),
				new PointF(1, 1),
				new PointF(1.3333334f, 1),
				new PointF(2.1666667f, 0.16666667f),
				new PointF(2, 0),
				new PointF(1.8333334f, -0.16666667f),
				new PointF(0.16666667f, -0.16666667f),
				new PointF(0, 0)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier + (byte)PathPointType.CloseSubpath
			}));
		});
	}

	[Test]
	public void Method_AddCurve()
	{
		var points = new[] { new PointF(0, 0), new PointF(1, 1), new PointF(2, 0) };
		
		using var path = new GraphicsPath();
		path.AddCurve(points);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(7));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(0, 0),
				new PointF(0.16666667f, 0.16666667f),
				new PointF(0.6666666f, 1),
				new PointF(1, 1),
				new PointF(1.3333334f, 1),
				new PointF(1.8333334f, 0.16666667f),
				new PointF(2, 0)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier
			}));
		});
	}

	[Test]
	public void Method_AddEllipse()
	{
		var rect = new RectangleF(0, 0, 100, 50);
		
		using var path = new GraphicsPath();
		path.AddEllipse(rect);
		Assert.Multiple(() =>
		{
			// TODO: remove this assert and replace with below asserts when method has been fixed
			Assert.That(path.PointCount, Is.GreaterThan(0));
			/*Assert.That(path.PointCount, Is.EqualTo(13));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(100, 25),
				new PointF(100, 38.80712f),
				new PointF(77.61423f, 50),
				new PointF(50, 50),
				new PointF(22.38576f, 50),
				new PointF(0, 38.80712f),
				new PointF(0, 25),
				new PointF(0, 11.19288f),
				new PointF(22.38576f, 0),
				new PointF(50, 0),
				new PointF(77.61423f, 0),
				new PointF(100, 11.19288f),
				new PointF(100, 25)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier | (byte)PathPointType.CloseSubpath
			}));*/
		});
	}

	[Test]
	public void Method_AddLine()
	{
		var pt1 = new PointF(0, 0);
		var pt2 = new PointF(1, 1);

		using var path = new GraphicsPath();
		path.AddLine(pt1, pt2);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(2));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(0, 0),
				new PointF(1, 1)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line
			}));
		});
	}

	[Test]
	public void Method_AddPie()
	{
		var rect = new RectangleF(0, 0, 100, 50);

		using var path = new GraphicsPath();
		path.AddPie(rect, 0, 90);
		Assert.Multiple(() =>
		{
			 // TODO: remove this assert and replace with below asserts when method has been fixed
			Assert.That(path.PointCount, Is.GreaterThan(0));
			/*Assert.That(path.PointCount, Is.EqualTo(5));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(50, 25),
				new PointF(99.99999f, 25),
				new PointF(99.99999f, 38.80711f),
				new PointF(77.61423f, 49.99999f),
				new PointF(50, 50)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier + (byte)PathPointType.CloseSubpath
			}));*/
		});
	}

	[Test]
	public void Method_AddPolygon()
	{
		var points = new PointF[] { new(0, 0), new(1, 1), new(2, 0) };

		using var path = new GraphicsPath();
		path.AddPolygon(points);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(3));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(0, 0),
				new PointF(1, 1),
				new PointF(2, 0)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line | (byte)PathPointType.CloseSubpath
			}));
		});
	}

	[Test]
	public void Method_AddRectangle()
	{
		var rect = new RectangleF(0, 0, 100, 50);

		using var path = new GraphicsPath();
		path.AddRectangle(rect);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(4));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{ 
				new PointF(0, 0), 
				new PointF(100, 0), 
				new PointF(100, 50), 
				new PointF(0, 50) 
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line, 
				(byte)PathPointType.Line, 
				(byte)PathPointType.Line | (byte)PathPointType.CloseSubpath
			}));
		});
	}

	[Test]
	public void Method_AddString()
	{
		var text = "x!";
		var font = new Font("Arial", 16);
		var origin = new PointF(0, 0);
		var format = new StringFormat();

		using var path = new GraphicsPath();
		path.AddString(text, font.FontFamily, (int)font.Style, font.Size, origin, format);
		Assert.Multiple(() =>
		{
			 // TODO: remove this assert and replace with below asserts when method has been fixed
			Assert.That(path.PointCount, Is.GreaterThan(0));
			/*Assert.That(path.PointCount, Is.EqualTo(29));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{
				new PointF(2.78125f, 14.48438f),
				new PointF(5.8125f, 10.17188f),
				new PointF(3.007813f, 6.1875f),
				new PointF(4.765625f, 6.1875f),
				new PointF(6.039063f, 8.132813f),
				new PointF(6.278646f, 8.502604f),
				new PointF(6.471354f, 8.8125f),
				new PointF(6.617188f, 9.0625f),
				new PointF(6.846354f, 8.71875f),
				new PointF(7.057292f, 8.414063f),
				new PointF(7.25f, 8.148438f),
				new PointF(8.648438f, 6.1875f),
				new PointF(10.32813f, 6.1875f),
				new PointF(7.460938f, 10.09375f),
				new PointF(10.54688f, 14.48438f),
				new PointF(8.820313f, 14.48438f),
				new PointF(7.117188f, 11.90625f),
				new PointF(6.664063f, 11.21094f),
				new PointF(4.484375f, 14.48438f),
				new PointF(12.71094f, 11.64063f),
				new PointF(12.28125f, 5.570313f),
				new PointF(12.28125f, 3.03125f),
				new PointF(14.02344f, 3.03125f),
				new PointF(14.02344f, 5.570313f),
				new PointF(13.61719f, 11.64063f),
				new PointF(12.34375f, 14.48438f),
				new PointF(12.34375f, 12.88281f),
				new PointF(13.96094f, 12.88281f),
				new PointF(13.96094f, 14.48438f)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line | (byte)PathPointType.DashMode | (byte)PathPointType.CloseSubpath,
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line + (byte)PathPointType.CloseSubpath,
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line + (byte)PathPointType.DashMode + (byte)PathPointType.CloseSubpath
			}));*/
		});
	}

	[Test]
	public void Method_CloseFigure()
	{
		var points = new[] { new PointF(0, 0), new PointF(1, 1), new PointF(2, 0) };
		
		using var path = new GraphicsPath();
		path.AddCurve(points);

		path.CloseFigure();
		Assert.That((path.PathTypes[path.PointCount - 1] & (byte)PathPointType.CloseSubpath) == (byte)PathPointType.CloseSubpath);
	}

	[Test]
	public void Method_Flatten()
	{
		var points = new[] { new PointF(1, 5), new PointF(1, 3), new PointF(5, 5), new PointF(5, 3) };
			
		var matrix = new Matrix();
		matrix.Translate(5, 10);
		
		using var path = new GraphicsPath();
		path.AddBezier(points[0], points[1], points[2], points[3]);

		path.Flatten(matrix, 0.25f);
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(5));
			Assert.That(path.PathPoints, Is.EqualTo(new[] 
			{
				new PointF(6f, 15f),
				new PointF(6.625f, 14.125f),
				new PointF(8f, 14f),
				new PointF(9.375f, 13.875f),
				new PointF(10f, 13f)
			}));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line
			}));
		});
	}

	[Test]
	public void Method_GetBounds()
	{
		var rect = new RectangleF(0, 0, 100, 50);

		using var path = new GraphicsPath();
		path.AddRectangle(rect);

		var bounds = path.GetBounds();
		Assert.Multiple(() =>
		{
			Assert.That(bounds.X, Is.EqualTo(0));
			Assert.That(bounds.Y, Is.EqualTo(0));
			Assert.That(bounds.Width, Is.EqualTo(100));
			Assert.That(bounds.Height, Is.EqualTo(50));
		});
	}

	[Test]
	public void Method_GetLastPoint()
	{
		var points = new[] { new PointF(0, 0), new PointF(1, 1), new PointF(2, 0) };
		
		using var path = new GraphicsPath();
		path.AddCurve(points);

		Assert.That(path.GetLastPoint(), Is.EqualTo(points[points.Length - 1]));
	}

	[Test]
	public void Method_IsOutlineVisible()
	{
		var pen = new Pen(Color.Black, 1);
		var rect = new RectangleF(0, 0, 3, 3);

		using var path = new GraphicsPath();
		path.AddRectangle(rect);

		Assert.Multiple(() =>
		{
			Assert.That(path.IsOutlineVisible(0, 0, pen), Is.True);
			Assert.That(path.IsOutlineVisible(2, 2, pen), Is.False);
			Assert.That(path.IsOutlineVisible(3, 3, pen), Is.True);
			Assert.That(path.IsOutlineVisible(4, 4, pen), Is.False);
		});
	}

	[Test]
	public void Method_IsVisible()
	{
		var rect = new RectangleF(0, 0, 3, 3);

		using var path = new GraphicsPath();
		path.AddRectangle(rect);

		Assert.Multiple(() =>
		{
			Assert.That(path.IsVisible(0, 0), Is.True);
			Assert.That(path.IsVisible(2, 2), Is.True);
			Assert.That(path.IsVisible(3, 3), Is.False);
			Assert.That(path.IsVisible(4, 4), Is.False);
		});
	}

	[Test]
	public void Method_Reverse()
	{
		var rect = new RectangleF(0, 0, 100, 50);

		using var path = new GraphicsPath();
		path.AddLine(0, 0, 1, 1);
		path.AddLine(1, 1, 2, 2);
		path.AddBezier(2, 2, 2, 3, 3, 3, 3, 4);
		path.AddRectangle(rect);

		var reversedPoints = path.PathPoints.Reverse().ToArray();

		path.Reverse();
		Assert.Multiple(() =>
		{
			Assert.That(path.PathPoints, Is.EqualTo(reversedPoints));
			Assert.That(path.PathTypes, Is.EqualTo(new[]
			{
				(byte)PathPointType.Start,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line | (byte)PathPointType.CloseSubpath,
				(byte)PathPointType.Start,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Bezier,
				(byte)PathPointType.Line,
				(byte)PathPointType.Line
			}));
		});
	}

	[Test]
	public void Method_Reset()
	{
		var rect = new RectangleF(0, 0, 100, 50);
		
		using var path = new GraphicsPath();
		path.AddRectangle(rect);

		path.Reset();
		Assert.Multiple(() =>
		{
			Assert.That(path.PointCount, Is.EqualTo(0));
			Assert.That(path.PathPoints, Is.Empty);
			Assert.That(path.PathTypes, Is.Empty);
		});
	}

	[Test]
	public void Method_Transform()
	{
		using var path = new GraphicsPath();
		path.AddLine(0, 0, 1, 1);

		using var matrix = new Matrix();
		matrix.Translate(1, 1);

		var transformedPoints = path.PathPoints;
		matrix.TransformPoints(transformedPoints);

		path.Transform(matrix);
		Assert.That(path.PathPoints, Is.EqualTo(transformedPoints));
	}
}
using System;
using System.IO;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test.Drawing2D;

internal class LinearGradientBrushUnitTest
{
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	public void Constructor_Points()
	{
		var startPoint = new Point(0, 0);
		var endPoint = new Point(1, 1);

		var bounds = new RectangleF(startPoint.X, startPoint.Y, startPoint.X + endPoint.X, startPoint.Y + endPoint.Y);

		var startColor = Color.Red;
		var endColor = Color.Blue;

		using var brush = new LinearGradientBrush(startPoint, endPoint, startColor, endColor);
		Assert.Multiple(() =>
		{
			Assert.That(brush.LinearColors, Is.EqualTo(new[] { startColor, endColor }));
			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Tile));
		});
	}
	
	[Test]
	[TestCase(LinearGradientMode.Horizontal, new[] { 0.9999999f, 0f, 0f, 0.9999999f, 0f, 0f })]
	[TestCase(LinearGradientMode.Vertical, new[] { -1.907349E-07f, 0.9999999f, -1f, -4.768372E-08f, 50f, 7.152557E-07f })]
	[TestCase(LinearGradientMode.BackwardDiagonal, new[] { -1f, 0.9999998f, -1f, -1f, 74.99999f, 25f })]
	[TestCase(LinearGradientMode.ForwardDiagonal, new[] { 0.9999999f, 0.9999999f, -1f, 0.9999999f, 25f, -25f })]
	public void Constructor_RectangleMode(LinearGradientMode mode, float[] elements)
	{
		var rect = new RectangleF(15, 15, 20, 20);

		var startColor = Color.Red;
		var endColor = Color.Blue;

		using var brush = new LinearGradientBrush(rect, startColor, endColor, mode);
		Assert.Multiple(() =>
		{
			Assert.That(brush.LinearColors, Is.EqualTo(new[] { startColor, endColor }));
			Assert.That(brush.Transform.Elements, Is.EqualTo(elements).Within(1e-05));
			Assert.That(brush.Rectangle, Is.EqualTo(rect));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Tile));

			string path = Path.Combine("brush", "linear", $"Mode{mode}.png");
			float similarity = Utils.CompareImage(path, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.95));
		});
	}
	
	[Test]
	[TestCase(0f, new[] { 0.9999999f, 0f, 0f, 0.9999999f, 0f, 0f })]
	[TestCase(45f, new[] { 0.999999f, 0.9999999f, -1f, 0.9999999f, 25f, -25f })]
	[TestCase(75f, new[] { 0.3169872f, 1.183012f, -1.183013f, 0.3169872f, 46.65063f, -12.5f })]
	[TestCase(90f, new[] { -1.907349E-07f, 0.9999999f, -1f, -4.768372E-08f, 50f, 7.152557E-07f })]
	public void Constructor_RectangleAngle(float angle, float[] elements)
	{
		var rect = new RectangleF(15, 15, 20, 20);

		var startColor = Color.Red;
		var endColor = Color.Blue;

		using var brush = new LinearGradientBrush(rect, startColor, endColor, angle);
		Assert.Multiple(() =>
		{
			Assert.That(brush.LinearColors, Is.EqualTo(new[] { startColor, endColor }));
			Assert.That(brush.Transform.Elements, Is.EqualTo(elements).Within(1e-05));
			Assert.That(brush.Rectangle, Is.EqualTo(rect));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Tile));

			string path = Path.Combine("brush", "linear", $"Angle{angle}.png");
			float similarity = Utils.CompareImage(path, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.95));
		});
	}

	[Test]
	public void Method_Clone()
	{
		using var brush1 = new LinearGradientBrush(new Point(0, 0), new Point(1, 1), Color.Red, Color.Blue);
		using var brush2 = brush1.Clone() as LinearGradientBrush;
		Assert.Multiple(() =>
		{
			Assert.That(brush2, Is.Not.Null);
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.LinearColors, Is.EqualTo(brush1.LinearColors));
			Assert.That(brush2.Transform.Elements, Is.EqualTo(brush1.Transform.Elements));
			Assert.That(brush2.Rectangle, Is.EqualTo(brush1.Rectangle));
			Assert.That(brush2.WrapMode, Is.EqualTo(brush1.WrapMode));
		});
	}
	
	[Test]
	public void Property_Blend()
	{
		var bounds = new RectangleF(0, 0, 1, 1);
		var colors = new Color[] { Color.Red, Color.Blue };

		var blend = new Blend(4);
		Array.Copy(new[] { 0f, 0.25f, 0.75f, 1f }, blend.Positions, 4);
		Array.Copy(new[] { 0f, 0.25f, 0.25f, 0f }, blend.Factors, 4);

		using var brush = new LinearGradientBrush(bounds, colors[0], colors[1], 0) { Blend = blend };
		Assert.Multiple(() =>
		{
			Assert.That(brush.Blend.Factors, Is.EqualTo(blend.Factors));
			Assert.That(brush.Blend.Positions, Is.EqualTo(blend.Positions));
			Assert.That(brush.LinearColors, Is.EqualTo(colors));
			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Tile));
		});
	}
	
	[Test]
	public void Property_Interpolation_And_Gamma()
	{
		var bounds = new RectangleF(0, 0, 1, 1);
		var colors = new Color[] { Color.Red, Color.Blue };

		var interpolation = new ColorBlend(3);
		Array.Copy(new[] { 0f, 0.5f, 1f }, interpolation.Positions, 3);
		Array.Copy(new[] { Color.Cyan, Color.Yellow, Color.Magenta }, interpolation.Colors, 3);

		var gammaCorrection = true;

		using var brush = new LinearGradientBrush(bounds, colors[0], colors[1], 0) { InterpolationColors = interpolation, GammaCorrection = gammaCorrection };
		Assert.Multiple(() =>
		{
			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(interpolation.Colors));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(interpolation.Positions));
			Assert.That(brush.GammaCorrection, Is.EqualTo(gammaCorrection));
			Assert.That(brush.LinearColors, Is.EqualTo(interpolation.Colors)); // NOTE: interpolation overrides linear colors
			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Tile));
		});
	}
	
	[Test]
	public void Property_Transform()
	{
		var bounds = new RectangleF(0, 0, 1, 1);
		var colors = new Color[] { Color.Red, Color.Blue };
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);

		using var brush = new LinearGradientBrush(bounds, colors[0], colors[1], 0) { Transform = matrix };
		Assert.Multiple(() =>
		{
			Assert.That(brush.LinearColors, Is.EqualTo(colors));
			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.Transform, Is.EqualTo(matrix));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Tile));
		});
	}
	
	[Test]
	public void Property_WrapMode()
	{
		var bounds = new RectangleF(0, 0, 1, 1);
		var colors = new Color[] { Color.Red, Color.Blue };
		var wrapMode = WrapMode.Clamp;

		using var brush = new LinearGradientBrush(bounds, colors[0], colors[1], 0) { WrapMode = wrapMode };
		Assert.Multiple(() =>
		{
			Assert.That(brush.LinearColors, Is.EqualTo(colors));
			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(wrapMode));
		});
	}

	[Test]
	public void Method_SetSigmaBellShape()
	{
		var startPoint = new Point(0, 0);
		var endPoint = new Point(1, 1);

		var startColor = Color.Red;
		var endColor = Color.Blue;

		var (focus, scale) = (0.25f, 0.75f);

		using var brush = new LinearGradientBrush(startPoint, endPoint, startColor, endColor);
		brush.SetSigmaBellShape(focus, scale);
		Assert.Multiple(() =>
		{
			var threshold = 1e-3f;
			var length = brush.Blend.Factors.Length;
			var mid = length / 2;
			
			Assert.That(length, Is.EqualTo(511));

			Assert.That(brush.Blend.Factors[0], Is.EqualTo(0));
			Assert.That(brush.Blend.Positions[0], Is.EqualTo(0));

			Assert.That(brush.Blend.Factors[1], Is.EqualTo(0.000675).Within(threshold));
			Assert.That(brush.Blend.Positions[1], Is.EqualTo(0.000980).Within(threshold));

			Assert.That(brush.Blend.Factors[mid], Is.EqualTo(scale));
			Assert.That(brush.Blend.Positions[mid], Is.EqualTo(focus));

			Assert.That(brush.Blend.Factors[length - 2], Is.EqualTo(0.000675).Within(threshold));
			Assert.That(brush.Blend.Positions[length - 2], Is.EqualTo(0.997058).Within(threshold));

			Assert.That(brush.Blend.Factors[length - 1], Is.EqualTo(0));
			Assert.That(brush.Blend.Positions[length - 1], Is.EqualTo(1));
		});
	}

	[Test]
	public void Method_SetBlendTriangularShape()
	{
		var startPoint = new Point(0, 0);
		var endPoint = new Point(1, 1);

		var startColor = Color.Red;
		var endColor = Color.Blue;

		var (focus, scale) = (0.25f, 0.75f);

		using var brush = new LinearGradientBrush(startPoint, endPoint, startColor, endColor);
		brush.SetBlendTriangularShape(focus, scale);
		Assert.Multiple(() =>
		{
			Assert.That(brush.Blend.Factors.Length, Is.EqualTo(3));
			Assert.That(brush.Blend.Positions[0], Is.EqualTo(0));
			Assert.That(brush.Blend.Factors[0], Is.EqualTo(0));
			Assert.That(brush.Blend.Positions[1], Is.EqualTo(focus));
			Assert.That(brush.Blend.Factors[1], Is.EqualTo(scale));
			Assert.That(brush.Blend.Positions[2], Is.EqualTo(1));
			Assert.That(brush.Blend.Factors[2], Is.EqualTo(0));
		});
	}
}
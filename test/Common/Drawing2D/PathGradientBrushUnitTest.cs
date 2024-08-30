using System.IO;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test.Drawing2D;

internal class PathGradientBrushUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Path()
	{
		var rect = new RectangleF(5, 5, 40, 40);
		var color = Color.Red;
		// var center = new PointF(15, 15); // NOT WORKING
		
		using var path = new GraphicsPath();
		path.AddEllipse(rect);

		using var brush = new PathGradientBrush(path) { CenterColor = color };
		Assert.Multiple(() =>
		{
			var bounds = path.GetBounds();
			var center = new PointF(rect.X + rect.Width / 2, rect.Y + rect.Height / 2);

			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Clamp));
			Assert.That(brush.Transform.IsIdentity, Is.True);
			Assert.That(brush.FocusScales, Is.EqualTo(new PointF(0, 0)));
			Assert.That(brush.SurroundColors, Is.EqualTo(new[] { Color.White }));

			Assert.That(brush.CenterColor, Is.EqualTo(color));
			Assert.That(brush.CenterPoint, Is.EqualTo(center));

			Assert.That(brush.Blend.Factors, Is.EqualTo(new[] { 1f }));
			Assert.That(brush.Blend.Positions, Is.EqualTo(new[] { 0f }));

			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(new[] { Color.Empty }));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(new[] { 0f }));

			string filepath = Path.Combine("brush", "path", $"Circle.png");
			float similarity = Utils.CompareImage(filepath, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	[TestCase(WrapMode.Tile)]
	[TestCase(WrapMode.Clamp)]
	public void Constructor_Points(WrapMode mode)
	{
		var points = new PointF[] {  new(0, 0), new(50, 0), new(25, 25) };
		var color = Color.Red;

		using var brush = new PathGradientBrush(points, mode) { CenterColor = color };
		Assert.Multiple(() =>
		{
			var bounds = Utils.GetBoundingRectangle(points);
			var center = Utils.GetCenterPoint(points);
			
			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(mode));
			Assert.That(brush.Transform.IsIdentity, Is.True);
			Assert.That(brush.FocusScales, Is.EqualTo(PointF.Empty));
			Assert.That(brush.SurroundColors, Is.EqualTo(new[] { Color.White }));

			Assert.That(brush.CenterColor, Is.EqualTo(color));
			Assert.That(brush.CenterPoint, Is.EqualTo(center));

			Assert.That(brush.Blend.Factors, Is.EqualTo(new[] { 1f }));
			Assert.That(brush.Blend.Positions, Is.EqualTo(new[] { 0f }));

			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(new[] { Color.Empty }));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(new[] { 0f }));

			string filepath = Path.Combine("brush", "path", $"Triangle{mode}.png");
			float similarity = Utils.CompareImage(filepath, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Property_Blend()
	{
		var points = new PointF[] {  new(0, 0), new(50, 0), new(25, 25) };
		var color = Color.Red;
		var blend = new Blend()
		{
			Factors = new float[] { 0.1f, 0.9f, 0.1f },
			Positions = new float[] { 0.0f, 0.4f, 1.0f }
		};

		using var brush = new PathGradientBrush(points) { CenterColor = color, Blend = blend };
		Assert.Multiple(() => 
		{
			var bounds = Utils.GetBoundingRectangle(points);
			var center = Utils.GetCenterPoint(points);

			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Clamp));
			Assert.That(brush.Transform.IsIdentity, Is.True);
			Assert.That(brush.FocusScales, Is.EqualTo(PointF.Empty));
			Assert.That(brush.SurroundColors, Is.EqualTo(new[] { Color.White }));

			Assert.That(brush.CenterColor, Is.EqualTo(color));
			Assert.That(brush.CenterPoint, Is.EqualTo(center));

			Assert.That(brush.Blend.Factors, Is.EqualTo(blend.Factors));
			Assert.That(brush.Blend.Positions, Is.EqualTo(blend.Positions));

			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(new[] { Color.Empty }));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(new[] { 0f }));

			string filepath = Path.Combine("brush", "path", $"TriangleBlended.png");
			float similarity = Utils.CompareImage(filepath, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Property_CenterPoint()
	{
		var rect = new RectangleF(5, 5, 40, 40);
		var color = Color.Red;
		var center = new PointF(15, 15);
		
		using var path = new GraphicsPath();
		path.AddEllipse(rect);

		using var brush = new PathGradientBrush(path) { CenterColor = color, CenterPoint = center };
		Assert.Multiple(() =>
		{
			var bounds = path.GetBounds();

			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Clamp));
			Assert.That(brush.Transform.IsIdentity, Is.True);
			Assert.That(brush.FocusScales, Is.EqualTo(new PointF(0, 0)));
			Assert.That(brush.SurroundColors, Is.EqualTo(new[] { Color.White }));

			Assert.That(brush.CenterColor, Is.EqualTo(color));
			Assert.That(brush.CenterPoint, Is.EqualTo(center));

			Assert.That(brush.Blend.Factors, Is.EqualTo(new[] { 1f }));
			Assert.That(brush.Blend.Positions, Is.EqualTo(new[] { 0f }));

			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(new[] { Color.Empty }));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(new[] { 0f }));

			string filepath = Path.Combine("brush", "path", $"CircleRecentered.png");
			float similarity = Utils.CompareImage(filepath, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Property_FocusScales()
	{
		Assert.Fail("Not implemented");
	}

	[Test]
	public void Property_InterpolationColors()
	{
		var points = new PointF[] {  new(0, 0), new(50, 0), new(25, 25) };
		var blend = new ColorBlend()
		{
			Colors = new Color[] { Color.DarkGreen, Color.Aqua, Color.Blue },
			Positions = new float[] { 0.0f, 0.4f, 1.0f }
		};

		using var brush = new PathGradientBrush(points) { InterpolationColors = blend };
		Assert.Multiple(() => 
		{
			var bounds = Utils.GetBoundingRectangle(points);
			var center = Utils.GetCenterPoint(points);

			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Clamp));
			Assert.That(brush.Transform.IsIdentity, Is.True);
			Assert.That(brush.FocusScales, Is.EqualTo(PointF.Empty));
			Assert.That(brush.SurroundColors, Is.EqualTo(new[] { Color.White }));

			Assert.That(brush.CenterColor, Is.EqualTo(Color.White));
			Assert.That(brush.CenterPoint, Is.EqualTo(center));

			Assert.That(brush.Blend.Factors, Is.EqualTo(new[] { 1f }));
			Assert.That(brush.Blend.Positions, Is.EqualTo(new[] { 0f }));

			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(blend.Colors));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(blend.Positions));

			string filepath = Path.Combine("brush", "path", $"TriangleInterpolated.png");
			float similarity = Utils.CompareImage(filepath, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
 	}

	[Test]
	public void Property_SurroundedColors()
	{
		var points = new PointF[] {  new(0, 0), new(50, 0), new(25, 25) };
		var surrounded = new Color[] { Color.Green, Color.Yellow, Color.Blue };

		using var brush = new PathGradientBrush(points) { SurroundColors = surrounded };
		Assert.Multiple(() => 
		{
			var bounds = Utils.GetBoundingRectangle(points);
			var center = Utils.GetCenterPoint(points);

			Assert.That(brush.Rectangle, Is.EqualTo(bounds));
			Assert.That(brush.WrapMode, Is.EqualTo(WrapMode.Clamp));
			Assert.That(brush.Transform.IsIdentity, Is.True);
			Assert.That(brush.FocusScales, Is.EqualTo(PointF.Empty));
			Assert.That(brush.SurroundColors, Is.EqualTo(surrounded));

			Assert.That(brush.CenterColor, Is.EqualTo(Color.Black));
			Assert.That(brush.CenterPoint, Is.EqualTo(center));

			Assert.That(brush.Blend.Factors, Is.EqualTo(new[] { 1f }));
			Assert.That(brush.Blend.Positions, Is.EqualTo(new[] { 0f }));

			Assert.That(brush.InterpolationColors.Colors, Is.EqualTo(new[] { Color.Empty }));
			Assert.That(brush.InterpolationColors.Positions, Is.EqualTo(new[] { 0f }));

			string filepath = Path.Combine("brush", "path", $"TriangleSurrounded.png");
			float similarity = Utils.CompareImage(filepath, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
 	}

	[Test]
	public void Property_Transform()
	{
		Assert.Fail("Not implemented");
	}

	[Test]
	public void Method_Clone()
	{
		var rect = new RectangleF(15, 15, 20, 20);
		
		using var path = new GraphicsPath();
		path.AddEllipse(rect);

		using var brush1 = new PathGradientBrush(path) { WrapMode = WrapMode.Tile };
		using var brush2 = brush1.Clone() as PathGradientBrush;
		Assert.Multiple(() =>
		{
			Assert.That(brush2, Is.Not.Null);
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.Transform.Elements, Is.EqualTo(brush1.Transform.Elements));
			Assert.That(brush2.Rectangle, Is.EqualTo(brush1.Rectangle));
			Assert.That(brush2.WrapMode, Is.EqualTo(brush1.WrapMode));
		});
	}
}
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test;

internal class PenUnitTest
{
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	public void Constructor_Color()
	{
		var color = Color.Red;

		using var pen = new Pen(color);
		Assert.Multiple(() =>
		{
			Assert.That(pen.Color, Is.EqualTo(color));
			Assert.That(pen.Width, Is.EqualTo(1.0f));
			Assert.That(pen.Alignment, Is.EqualTo(PenAlignment.Center));
			Assert.That(pen.CompoundArray, Is.Empty);
			Assert.That(pen.DashCap, Is.EqualTo(DashCap.Flat));
			Assert.That(pen.DashOffset, Is.EqualTo(0));
			Assert.That(pen.DashPattern, Is.Empty);
			Assert.That(pen.PenType, Is.EqualTo(PenType.SolidColor));
			Assert.That(pen.StartCap, Is.EqualTo(LineCap.Flat));
			Assert.That(pen.EndCap, Is.EqualTo(LineCap.Flat));
			Assert.That(pen.LineJoin, Is.EqualTo(LineJoin.Miter));
			Assert.That(pen.MiterLimit, Is.EqualTo(10));
			Assert.That(pen.Transform.IsIdentity, Is.True);
		});
	}

	[Test]
	public void Constructor_ColorAndWidth()
	{
		var color = Color.Green;
		var width = 5.0f;

		using var pen = new Pen(color, width);
		Assert.Multiple(() =>
		{
			Assert.That(pen.Color, Is.EqualTo(color));
			Assert.That(pen.Width, Is.EqualTo(width));
		});
	}

	[Test]
	public void Constructor_Brush()
	{
		var color = Color.Green;
		var brush = new SolidBrush(color);
		var width = 5.0f;

		using var pen = new Pen(brush, width);
		Assert.Multiple(() =>
		{
			Assert.That(pen.Color, Is.EqualTo(color));
			Assert.That(pen.Width, Is.EqualTo(width));
		});
	}

	[Test]
	public void Constructor_Properties()
	{
		var color = Color.Red;
		var width = 10.0f;
		var alignment = PenAlignment.Left;
		var brush = new LinearGradientBrush(new Point(0, 0), new Point(1, 1), Color.Black, Color.White);
		var compund = new[] { 1.0f, 2.0f };
		var dashCap = DashCap.Round;
		var dashOffset = 3.0f;
		var dashPattern = new[] { 0.0f, 5.0f, 10.0f };
		var lineCap1 = LineCap.Round;
		var lineCap2 = LineCap.Square;
		var lineJoin = LineJoin.Bevel;
		var mitterLimit = 5.0f;
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);

		using var pen = new Pen(Color.Green, 5.0f)
		{
			Color = color,
			Width = width,
			Alignment = alignment,
			Brush = brush,
			CompoundArray = compund,
			DashCap = dashCap,
			DashOffset = dashOffset,
			DashPattern = dashPattern,
			StartCap = lineCap1,
			EndCap = lineCap2,
			LineJoin = lineJoin,
			MiterLimit = mitterLimit,
			Transform = matrix,
		};

		Assert.Multiple(() =>
		{
			Assert.That(pen.Color, Is.EqualTo(color));
			Assert.That(pen.Width, Is.EqualTo(width));
			Assert.That(pen.Alignment, Is.EqualTo(alignment));
			Assert.That(pen.Brush, Is.TypeOf<LinearGradientBrush>());
			Assert.That(pen.PenType, Is.EqualTo(PenType.LinearGradient));
			Assert.That(pen.CompoundArray, Is.EqualTo(compund));
			Assert.That(pen.DashCap, Is.EqualTo(dashCap));
			Assert.That(pen.DashOffset, Is.EqualTo(dashOffset));
			Assert.That(pen.DashPattern, Is.EqualTo(dashPattern));
			Assert.That(pen.StartCap, Is.EqualTo(lineCap1));
			Assert.That(pen.EndCap, Is.EqualTo(lineCap2));
			Assert.That(pen.LineJoin, Is.EqualTo(lineJoin));
			Assert.That(pen.MiterLimit, Is.EqualTo(mitterLimit));
			Assert.That(pen.Transform, Is.EqualTo(matrix));
		});
	}

	[Test]
	public void Method_Clone()
	{
		using var pen1 = new Pen(Color.Blue, 2.0f);
		using var pen2 = pen1.Clone() as Pen;
		Assert.Multiple(() =>
		{
			Assert.That(pen1, Is.Not.Null);
			Assert.That(pen1, Is.Not.SameAs(pen2));
			Assert.That(pen2.Color, Is.EqualTo(pen1.Color));
			Assert.That(pen2.Width, Is.EqualTo(pen1.Width));
			Assert.That(pen2.Brush, Is.TypeOf<SolidBrush>());
		});
	}
}
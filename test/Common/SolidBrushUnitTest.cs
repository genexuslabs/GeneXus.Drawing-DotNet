namespace GeneXus.Drawing.Test;

internal class SolidBrushUnitTest
{
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	public void Constructor_Color()
	{
		var color = Color.Red;

		using var brush = new SolidBrush(color);
		Assert.That(brush.Color, Is.EqualTo(color));
	}
	
	[Test]
	public void Property_Color()
	{
		var color = Color.Blue;

		using var brush = new SolidBrush(Color.Red);
		brush.Color = color;
		Assert.That(brush.Color, Is.EqualTo(color));
	}

	[Test]
	public void Method_Clone()
	{
		using var brush1 = new SolidBrush(Color.Blue);
		using var brush2 = brush1.Clone() as SolidBrush;
		Assert.Multiple(() =>
		{
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.Color, Is.EqualTo(brush1.Color));
		});
	}
}
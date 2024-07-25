using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test.Drawing2D;

internal class HatchBrushUnitTest
{
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	public void Constructor_StyleForeBack()
	{
		var style = HatchStyle.Horizontal;
		var back = Color.Red;
		var fore = Color.Blue;

		using var brush = new HatchBrush(style, fore, back);
		Assert.Multiple(() =>
		{
			Assert.That(brush.BackgroundColor, Is.EqualTo(back));
			Assert.That(brush.ForegroundColor, Is.EqualTo(fore));
			Assert.That(brush.HatchStyle, Is.EqualTo(style));
		});
	}

	[Test]
	public void Method_Clone()
	{
		using var brush1 = new HatchBrush(HatchStyle.Vertical, Color.Red, Color.Blue);
		using var brush2 = brush1.Clone() as HatchBrush;
		Assert.Multiple(() =>
		{
			Assert.That(brush2, Is.Not.Null);
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.HatchStyle, Is.EqualTo(brush1.HatchStyle));
			Assert.That(brush2.ForegroundColor, Is.EqualTo(brush1.ForegroundColor));
			Assert.That(brush2.BackgroundColor, Is.EqualTo(brush1.BackgroundColor));
		});
	}
}
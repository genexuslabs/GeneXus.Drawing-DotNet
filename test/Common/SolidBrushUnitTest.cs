using System.IO;

namespace GeneXus.Drawing.Test;

internal class SolidBrushUnitTest
{
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	[TestCase("Red")]
	[TestCase("Green")]
	[TestCase("Blue")]
	public void Constructor_Color(string name)
	{
		var color = Color.FromName(name);

		using var brush = new SolidBrush(color);
		Assert.Multiple(() =>
		{
			Assert.That(brush.Color, Is.EqualTo(color));

			string path = Path.Combine("brush", "solid", $"Color{name}.png");
			float similarity = Utils.CompareImage(path, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
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
			Assert.That(brush2, Is.Not.Null);
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.Color, Is.EqualTo(brush1.Color));
		});
	}
}
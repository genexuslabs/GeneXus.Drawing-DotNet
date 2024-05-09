using GeneXus.Drawing;

namespace GeneXus.Drawing.Test;

internal class ColorUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_RGBA()
	{
		int alpha = 255, red = 100, green = 150, blue = 200;
		var color = new Color(alpha, red, green, blue);
		Assert.Multiple(() =>
		{
			Assert.That(color.A, Is.EqualTo(alpha));
			Assert.That(color.R, Is.EqualTo(red));
			Assert.That(color.G, Is.EqualTo(green));
			Assert.That(color.B, Is.EqualTo(blue));
		});
	}

	[Test]
	public void Constructor_Hex()
	{
		string hex = "#6496C8";
		var color = new Color(hex);
		Assert.Multiple(() =>
		{
			Assert.That(color.Hex, Is.EqualTo(hex).IgnoreCase);
		});
	}

	[Test]
	public void Operator_Equality()
	{
		var color1 = new Color(255, 100, 150, 200);
		var color2 = new Color(255, 100, 150, 200);
		Assert.That(color1 == color2, Is.True);
	}

	[Test]
	public void Operator_Inequality()
	{
		var color1 = new Color(255, 100, 150, 200);
		var color2 = new Color(255, 200, 150, 100);
		Assert.That(color1 != color2, Is.True);
	}

	[Test]
	public void Method_GetBrightness()
	{
		var color = new Color(255, 100, 150, 200);
		Assert.That(color.GetBrightness(), Is.EqualTo(0.5882f).Within(0.0001f));
	}

	[Test]
	public void Method_GetSaturation()
	{
		var color = new Color(255, 100, 150, 200);
		Assert.That(color.GetSaturation(), Is.EqualTo(0.4761f).Within(0.0001f));
	}

	[Test]
	public void Method_GetHue()
	{
		var color = new Color(255, 100, 150, 200);
		Assert.That(color.GetHue(), Is.EqualTo(210.00001f).Within(0.0001f));
	}

	[Test]
	public void Method_ToArgb()
	{
		var color = new Color(255, 100, 150, 200);
		int expectedArgb = unchecked((int)0xFF6496C8); // Expected ARGB value
		Assert.That(color.ToArgb(), Is.EqualTo(expectedArgb));
	}

	[Test]
	public void Method_FromName()
	{
		var colorName = "LightSkyBlue";
		var color = Color.FromName(colorName);
		Assert.That(color.Name, Is.EqualTo(colorName).IgnoreCase);
	}
}

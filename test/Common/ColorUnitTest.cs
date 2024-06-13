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
		Assert.That(color.Hex, Is.EqualTo(hex).IgnoreCase);
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
	public void Property_IsEmpty()
	{
		var color1 = Color.Empty;
		var color2 = Color.FromArgb(0, 0, 0, 0);
		Assert.Multiple(() =>
		{
			Assert.That(color1.IsEmpty, Is.True);
			Assert.That(color2.IsEmpty, Is.False);
		});
	}

	[Test]
	public void Property_Name()
	{
		const string COLOR_NAME = "red";

		var color0 = Color.Empty;
		var color1 = Color.FromName(COLOR_NAME);
		var color2 = Color.FromArgb(255, 0, 0);
		var color3 = Color.FromArgb(255, 128, 64);
		Assert.Multiple(() =>
		{
			Assert.That(color0.Name, Is.EqualTo("0"));
			Assert.That(color1.Name, Is.EqualTo(COLOR_NAME).IgnoreCase);
			Assert.That(color2.Name, Is.EqualTo(COLOR_NAME).IgnoreCase);
			Assert.That(color3.Name, Is.EqualTo("ffff8040").IgnoreCase);
		});
	}

	[Test]
	public void Method_IsNamedColor()
	{
		const string COLOR_NAME = "red";

		var color1 = Color.FromName(COLOR_NAME);
		var color2 = Color.FromHex("#ff0000");
		Assert.Multiple(() =>
		{
			Assert.That(color1.IsNamedColor, Is.True);
			Assert.That(color2.IsNamedColor, Is.False);
		});
	}

	[Test]
	public void Method_IsKnownColor()
	{
		var knwonColor = KnownColor.Red;

		var color1 = Color.FromKnownColor(knwonColor);
		var color2 = Color.FromHex("#ff0000");
		Assert.Multiple(() =>
		{
			Assert.That(color1.IsKnownColor, Is.True);
			Assert.That(color2.IsKnownColor, Is.False);
		});
	}

	[Test]
	public void Method_IsSystemColor()
	{
		var knwonColor = KnownColor.Control;

		var color1 = Color.FromKnownColor(knwonColor);
		var color2 = Color.FromHex("#ff0000");
		Assert.Multiple(() =>
		{
			Assert.That(color1.IsSystemColor, Is.True);
			Assert.That(color2.IsSystemColor, Is.False);
		});
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
	public void Method_ToKnownColor()
	{
		var knownColor = KnownColor.Red;

		var color1 = Color.FromKnownColor(knownColor);
		var color2 = new Color(color1.A, color1.R, color1.G, color1.B);

		Assert.Multiple(() =>
		{
			Assert.That(color1.ToKnownColor(), Is.EqualTo(knownColor));
			Assert.That(color2.ToKnownColor(), Is.EqualTo(knownColor));
		});
	}

	[Test]
	public void Method_FromName()
	{
		const string COLOR_NAME = "red";
		var color = Color.FromName(COLOR_NAME);
		Assert.Multiple(() =>
		{
			Assert.That(color.Name, Is.EqualTo(COLOR_NAME).IgnoreCase);
			Assert.That(color.R, Is.EqualTo(255));
			Assert.That(color.G, Is.EqualTo(0));
			Assert.That(color.B, Is.EqualTo(0));
		});
	}

	[Test]
	public void Method_FromKnownColor([Values] KnownColor knownColor)
	{
		var name = knownColor.ToString();
		var color = Color.FromKnownColor(knownColor);
		Assert.Multiple(() =>
		{
			Assert.That(color.Name, Is.EqualTo(name).IgnoreCase);
			Assert.That(color.IsKnownColor, Is.True);
			Assert.That(color.ToKnownColor(), Is.EqualTo(knownColor));
		});
	}
}

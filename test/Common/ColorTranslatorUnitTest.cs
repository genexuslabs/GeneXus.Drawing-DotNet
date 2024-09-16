namespace GeneXus.Drawing.Test;

internal class ColorTranslatorUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Method_ToHtml()
	{
		var color = Color.Red;
		var htmlColor = ColorTranslator.ToHtml(color);

		Assert.That(htmlColor, Is.EqualTo("#F00"));
	}

	[Test]
	public void Method_FromHtml()
	{
		var htmlColor = "#FF0000";
		var color = ColorTranslator.FromHtml(htmlColor);

		Assert.That(color, Is.EqualTo(Color.Red));
	}

	[Test]
	public void Method_ToOle()
	{
		 var color = Color.Blue;
        var oleColor = ColorTranslator.ToOle(color);

		Assert.That(oleColor, Is.EqualTo(0xFF0000));
	}

	[Test]
	public void Method_FromOle()
	{
		var oleColor = 0xFF0000;
        var color = ColorTranslator.FromOle(oleColor);

		Assert.That(color, Is.EqualTo(Color.Blue));
	}

	[Test]
	public void Method_ToWin32()
	{
		var color = Color.Lime;
        var win32Color = ColorTranslator.ToWin32(color);

		Assert.That(win32Color, Is.EqualTo(0x00FF00));
	}

	[Test]
	public void Method_FromWin32()
	{
		var win32Color = 0x00FF00;
        var color = ColorTranslator.FromWin32(win32Color);

        Assert.That(color, Is.EqualTo(Color.Lime));
	}
}
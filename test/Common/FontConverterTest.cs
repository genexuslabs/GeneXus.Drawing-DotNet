using System;
using System.ComponentModel;

namespace GeneXus.Drawing.Test;

internal class FontConverterTest
{
	[Test]
	public void Method_ConvertFromString()
	{
		Font font = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString("Verdana,12") as Font;
		Assert.Multiple(() =>
		{
			Assert.That(font?.Name, Is.EqualTo("Verdana"));
			Assert.That(Math.Abs(font.Size - 12f), Is.LessThan(0.01));
			Assert.That(font.Unit, Is.EqualTo(GraphicsUnit.Point));
			Assert.That(font.Style, Is.EqualTo(FontStyle.Regular));
		});
	}
	
	[Test]
	public void Method_ConvertToString()
	{
		Font font = new("Verdana"); // default size is 12
		string text = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(font);
		Assert.That(text, Is.EqualTo("Verdana, 12pt"));
	}
	
	[Test]
	public void Method_ConvertFromString_Unknown()
	{
		Font font = TypeDescriptor.GetConverter(typeof(Font)).ConvertFromString("UnknownFont,14") as Font;
		Assert.Multiple(() =>
		{
			Assert.That(font?.Name, Is.EqualTo("UnknownFont"));
			Assert.That(Math.Abs(font.Size - 14f), Is.LessThan(0.01));
			Assert.That(font.Unit, Is.EqualTo(GraphicsUnit.Point));
			Assert.That(font.Style, Is.EqualTo(FontStyle.Regular));
		});
	}
	
	[Test]
	public void Method_ConvertToString_Unknown()
	{
		Font font = new("UnknownFont"); // default size is 12
		string text = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(font);
		Assert.That(text, Is.EqualTo("UnknownFont, 12pt"));
	}
}

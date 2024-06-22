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
		});
	}
	
	[Test]
	public void Method_ConvertToString()
	{
		Font font = new("Verdana"); // default size is 12
		string text = TypeDescriptor.GetConverter(typeof(Font)).ConvertToString(font);
		Assert.That(text, Is.EqualTo("Verdana, 12pt"));
	}
}

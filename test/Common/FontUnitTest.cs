using System;
using System.IO;
using System.Linq;

namespace GeneXus.Drawing.Test;

internal class FontUnitTest
{
	private static readonly string FONT_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "fonts");

	[SetUp]
	public void Setup()
	{
	}

	[Test]
	[TestCase("Montserrat-Regular.ttf", "Montserrat", "Regular", 400, 5, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("Montserrat-Italic.ttf", "Montserrat", "Italic", 400, 5, 15, SlantType.Italic, FontStyle.Regular | FontStyle.Italic)]
	[TestCase("Montserrat-Bold.ttf", "Montserrat", "Bold", 700, 5, 15, SlantType.Normal, FontStyle.Regular | FontStyle.Bold)]
	[TestCase("Graphik-Regular.otf", "Graphik", "Regular", 400, 5, 14, SlantType.Normal, FontStyle.Regular)]
	[TestCase("Graphik-Italic.otf", "Graphik", "Italic", 400, 5, 14, SlantType.Italic, FontStyle.Regular | FontStyle.Italic)]
	[TestCase("Graphik-Bold.otf", "Graphik", "Bold", 700, 5, 14, SlantType.Normal, FontStyle.Regular | FontStyle.Bold)]
	[TestCase("EncodeSans-Regular.ttf", "Encode Sans", "Regular", 400, 5, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("EncodeSans-Condensed.ttf", "Encode Sans", "Condensed", 400, 3, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("EncodeSans-Expanded.ttf", "Encode Sans", "Expanded", 400, 7, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Regular", 400, 5, 17, SlantType.Normal, FontStyle.Regular, 7)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Italic", 400, 5, 17, SlantType.Italic, FontStyle.Regular | FontStyle.Italic, 4)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Bold", 700, 5, 17, SlantType.Normal, FontStyle.Regular | FontStyle.Bold, 0)]
	public void Constructor_Family(string fileName, string familyName, string faceName, int fontWeight, int fontWidth, int fontHeight, SlantType fontSlant, FontStyle fontStyle, int fontIndex = 0)
	{
		var fontPath = Path.Combine(FONT_PATH, fileName);
		using var family = FontFamilyFactory.Create(fontPath, fontIndex);

		using var font = new Font(family);
		Assert.Multiple(() =>
		{
			Assert.That(font, Is.Not.Null);
			Assert.That(font.Name, Is.EqualTo($"{familyName} {faceName}"));
			Assert.That(font.Weight, Is.EqualTo(fontWeight));
			Assert.That(font.Width, Is.EqualTo(fontWidth));
			Assert.That(font.Height, Is.EqualTo(fontHeight));
			Assert.That(font.Slant, Is.EqualTo(fontSlant));
			Assert.That(font.Italic, Is.EqualTo(fontSlant != SlantType.Normal));
			Assert.That(font.Bold, Is.EqualTo(fontWeight >= 600));
			Assert.That(font.Style, Is.EqualTo(fontStyle));
			Assert.That(font.Underline, Is.False);
			Assert.That(font.Strikeout, Is.False);
		});
	}

	[Test]
	public void Constructor_FamilyName()
	{
		var sysFont = Font.SystemFonts.FirstOrDefault();
		Assert.That(sysFont, Is.Not.Null);

		using var font = new Font(sysFont.Name);
		Assert.Multiple(() =>
		{
			Assert.That(font, Is.Not.Null);
			Assert.That(font.Name, Is.EqualTo(sysFont.Name));
			Assert.That(font.Weight, Is.EqualTo(sysFont.Weight));
			Assert.That(font.Width, Is.EqualTo(sysFont.Width));
			Assert.That(font.Height, Is.EqualTo(sysFont.Height));
			Assert.That(font.Slant, Is.EqualTo(sysFont.Slant));
			Assert.That(font.Italic, Is.EqualTo(sysFont.Italic));
			Assert.That(font.Bold, Is.EqualTo(sysFont.Bold));
			Assert.That(font.Underline, Is.EqualTo(sysFont.Underline));
			Assert.That(font.Strikeout, Is.EqualTo(sysFont.Strikeout));
		});
	}

	/*
	#TODO #Issue: font.IsSystemFont is throwing a
	"Fatal error. System.AccessViolationException: Attempted to read or write protected memory."
	that finished the execution of test. Disabled for now.
	[Test]
	public void Property_IsSystemFont()
	{
		try
		{
			var font = Font.SystemFonts.FirstOrDefault();
			Assert.Multiple(() =>
			{
				Assert.That(font, Is.Not.Null);
				Assert.That(font.IsSystemFont, Is.True);
			});
		}
		catch (Exception ex)
		{
			Assert.Fail(ex.Message);
		}
	}
	*/

	[Test]
	public void Method_Clone()
	{
		var fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");
		using var family = FontFamilyFactory.Create(fontPath);

		using var font1 = new Font(family);
		using var font2 = font1.Clone() as Font;
		Assert.That(font1, Is.Not.SameAs(font2));
	}

	[Test]
	public void Method_GetFonts()
	{
		var dirpath = Environment.GetFolderPath(Environment.SpecialFolder.Fonts);
		var fonts = Font.GetFonts(dirpath);
		Assert.That(fonts, Is.Not.Empty);
	}

	[Test]
	public void Extra_GetFontCount_FileName()
	{
		var fontpath = Path.Combine(FONT_PATH, "AvenirNext-Collection.ttc");
		var count = Font.GetFontCount(fontpath);
		Assert.That(count, Is.EqualTo(12));
	}

	[Test]
	public void Extra_GetFontCount_Stream()
	{
		var fontpath = Path.Combine(FONT_PATH, "AvenirNext-Collection.ttc");
		using var stream = File.OpenRead(fontpath);
		var count = Font.GetFontCount(stream);
		Assert.That(count, Is.EqualTo(12));
	}
}

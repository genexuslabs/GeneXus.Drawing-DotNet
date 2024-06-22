using GeneXus.Drawing.Text;
using NUnit.Framework;
using System;
using System.IO;

namespace GeneXus.Drawing.Test;

internal class FontFamilyUnitTest
{
	private static readonly string FONT_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "fonts");

	[SetUp]
	public void Setup()
	{
	}

	[Test]
	[TestCase("Montserrat-Regular.ttf", "Montserrat", 968, 251, 1219, 1000 )]
	[TestCase("Montserrat-Italic.ttf", "Montserrat", 968, 251, 1219, 1000 )]
	[TestCase("Montserrat-Bold.ttf", "Montserrat", 968, 251, 1219, 1000 )]
	[TestCase("Graphik-Regular.otf", "Graphik", 818, 182, 1100, 1000 )]
	[TestCase("Graphik-Italic.otf", "Graphik", 818, 182, 1100, 1000 )]
	[TestCase("Graphik-Bold.otf", "Graphik", 825, 175, 1100, 1000 )]
	[TestCase("EncodeSans-Regular.ttf", "Encode Sans", 2060, 440, 2500, 2000)]
	[TestCase("EncodeSans-Condensed.ttf", "Encode Sans", 2060, 440, 2500, 2000)]
	[TestCase("EncodeSans-Expanded.ttf", "Encode Sans", 2060, 440, 2500, 2000)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", 1000, 366, 1366, 1000 , 7)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", 1000, 366, 1366, 1000 , 4)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", 1000, 366, 1366, 1000 , 0)]
	public void Constructor_FileName(string fileName, string familyName, int ascent, int descent, int lineSpacing, int emHeight, int fontIndex = 0)
	{
		var fontPath = Path.Combine(FONT_PATH, fileName);
		using var family = FontFamilyFactory.Create(fontPath, fontIndex);
		Assert.Multiple(() =>
		{
			Assert.That(family.Name, Is.EqualTo(familyName));
			Assert.That(family.GetCellAscent(), Is.EqualTo(ascent));
			Assert.That(family.GetCellDescent(), Is.EqualTo(descent));
			Assert.That(family.GetLineSpacing(), Is.EqualTo(lineSpacing));
			Assert.That(family.GetEmHeight(), Is.EqualTo(emHeight));
		});
	}

	[Test]
	[TestCase("Montserrat-Regular.ttf", "Montserrat", 968, 251, 1219, 1000)]
	[TestCase("Montserrat-Italic.ttf", "Montserrat", 968, 251, 1219, 1000)]
	[TestCase("Montserrat-Bold.ttf", "Montserrat", 968, 251, 1219, 1000)]
	[TestCase("Graphik-Regular.otf", "Graphik", 818, 182, 1100, 1000)]
	[TestCase("Graphik-Italic.otf", "Graphik", 818, 182, 1100, 1000)]
	[TestCase("Graphik-Bold.otf", "Graphik", 825, 175, 1100, 1000)]
	[TestCase("EncodeSans-Regular.ttf", "Encode Sans", 2060, 440, 2500, 2000)]
	[TestCase("EncodeSans-Condensed.ttf", "Encode Sans", 2060, 440, 2500, 2000)]
	[TestCase("EncodeSans-Expanded.ttf", "Encode Sans", 2060, 440, 2500, 2000)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", 1000, 366, 1366, 1000, 7)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", 1000, 366, 1366, 1000, 4)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", 1000, 366, 1366, 1000, 0)]
	public void Constructor_Stream(string fileName, string familyName, int ascent, int descent, int lineSpacing, int emHeight, int fontIndex = 0)
	{
		var fontPath = Path.Combine(FONT_PATH, fileName);
		using var fontStream = File.OpenRead(fontPath);
		using var family = FontFamilyFactory.Create(fontStream, fontIndex);
		Assert.Multiple(() =>
		{
			Assert.That(family.Name, Is.EqualTo(familyName));
			Assert.That(family.GetCellAscent(), Is.EqualTo(ascent));
			Assert.That(family.GetCellDescent(), Is.EqualTo(descent));
			Assert.That(family.GetLineSpacing(), Is.EqualTo(lineSpacing));
			Assert.That(family.GetEmHeight(), Is.EqualTo(emHeight));
		});
	}

	[Test]
	public void Constructor_FontCollection()
	{
		using var pfc = new PrivateFontCollection();
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Regular.ttf"));
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Italic.ttf"));
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Bold.ttf"));
		Assert.That(pfc.Families, Has.Length.EqualTo(3));

		using var font = new Font(pfc.Families[1]);
		using var family = FontFamilyFactory.Create(font.Name, pfc);
		Assert.Multiple(() =>
		{
			Assert.That(family.Name, Is.EqualTo("Montserrat"));
			Assert.That(family.IsStyleAvailable(FontStyle.Regular), Is.True);
			Assert.That(family.IsStyleAvailable(FontStyle.Italic), Is.EqualTo((font.Style & FontStyle.Italic) == FontStyle.Italic));
			Assert.That(family.IsStyleAvailable(FontStyle.Bold), Is.EqualTo((font.Style & FontStyle.Bold) == FontStyle.Bold));
		});
	}

	[Test]
	public void Constructor_GenericFont()
	{
		using var monospace = FontFamilyFactory.Create(GenericFontFamilies.Monospace);
		Assert.That(monospace.Name, Is.AnyOf("Courier New", "Consolas", "Courier", "Menlo", "Monaco", "Lucida Console").IgnoreCase);

		using var sanserif = FontFamilyFactory.Create(GenericFontFamilies.SansSerif);
		Assert.That(sanserif.Name, Is.AnyOf("Arial", "Helvetica", "Verdana", "Tahoma", "Trebuchet MS", "Gill Sans").IgnoreCase);

		using var justserif = FontFamilyFactory.Create(GenericFontFamilies.Serif);
		Assert.That(justserif.Name, Is.AnyOf("Times New Roman", "Georgia", "Garamond", "Palatino", "Book Antiqua", "Baskerville").IgnoreCase);
	}
}

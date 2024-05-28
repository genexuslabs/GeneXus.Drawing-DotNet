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
		using var family = new FontFamily(fontPath, fontIndex);
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
		using var family = new FontFamily(fontStream, fontIndex);
		Assert.Multiple(() =>
		{
			Assert.That(family.Name, Is.EqualTo(familyName));
			Assert.That(family.GetCellAscent(), Is.EqualTo(ascent));
			Assert.That(family.GetCellDescent(), Is.EqualTo(descent));
			Assert.That(family.GetLineSpacing(), Is.EqualTo(lineSpacing));
			Assert.That(family.GetEmHeight(), Is.EqualTo(emHeight));
		});
	}

	public void Constructor_FontCollection()
	{
		using var pfc = new PrivateFontCollection();
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Regular.ttf"));
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Italic.ttf"));
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Bold.ttf"));
		Assert.That(pfc.Families.Length, Is.EqualTo(3));

		using var family = new FontFamily("Montserrat-Regular", pfc);
		Assert.Multiple(() =>
		{
			Assert.That(family, Is.Not.EqualTo(pfc.Families[0])); // not the same instance
			Assert.That(family.Name, Is.EqualTo(pfc.Families[0].Name));
		});
	}
}

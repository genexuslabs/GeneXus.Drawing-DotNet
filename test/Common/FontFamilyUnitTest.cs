using GeneXus.Drawing.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;

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
	[TestCase("Montserrat-Regular.ttf", "Montserrat", FontStyle.Regular, 968, 251, 1219, 1000 )]
	[TestCase("Montserrat-Italic.ttf", "Montserrat", FontStyle.Italic, 968, 251, 1219, 1000 )]
	[TestCase("Montserrat-Bold.ttf", "Montserrat", FontStyle.Bold, 968, 251, 1219, 1000 )]
	[TestCase("Graphik-Regular.otf", "Graphik", FontStyle.Regular, 818, 182, 1100, 1000 )]
	[TestCase("Graphik-Italic.otf", "Graphik", FontStyle.Italic, 818, 182, 1100, 1000 )]
	[TestCase("Graphik-Bold.otf", "Graphik", FontStyle.Bold, 825, 175, 1100, 1000 )]
	[TestCase("EncodeSans-Regular.ttf", "Encode Sans", FontStyle.Regular, 2060, 440, 2500, 2000)]
	[TestCase("EncodeSans-Condensed.ttf", "Encode Sans Condensed", FontStyle.Regular, 2060, 440, 2500, 2000)]
	[TestCase("EncodeSans-Expanded.ttf", "Encode Sans Expanded", FontStyle.Regular, 2060, 440, 2500, 2000)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", FontStyle.Regular, 1000, 366, 1366, 1000)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", FontStyle.Italic, 1000, 366, 1366, 1000)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", FontStyle.Bold, 1000, 366, 1366, 1000)]
	public void Constructor_FileName(string fileName, string familyName, FontStyle style, int ascent, int descent, int lineSpacing, int emHeight)
	{
		if (fileName.StartsWith("EncodeSans-") && RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			familyName = "Encode Sans"; // in Windows it doesn't read the correct family name for this font with skia

		string fontPath = Path.Combine(FONT_PATH, fileName);
		using PrivateFontCollection fontCollection = new();
		fontCollection.AddFontFile(fontPath);

		FontFamily family = fontCollection.Families[0];
		Assert.Multiple(() =>
		{
			Assert.That(family.Name, Is.EqualTo(familyName));
			Assert.That(family.GetCellAscent(style), Is.EqualTo(ascent));
			Assert.That(family.GetCellDescent(style), Is.EqualTo(descent));
			Assert.That(family.GetLineSpacing(style), Is.EqualTo(lineSpacing));
			Assert.That(family.GetEmHeight(style), Is.EqualTo(emHeight));
		});
	}

	[Test]
	public void Constructor_FontCollection()
	{
		using PrivateFontCollection pfc = new();
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Regular.ttf"));
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Italic.ttf"));
		pfc.AddFontFile(Path.Combine(FONT_PATH, "Montserrat-Bold.ttf"));
		Assert.That(pfc.Families, Has.Length.EqualTo(3));

		using FontFamily family = new("Montserrat", pfc);
		Assert.Multiple(() =>
		{
			Assert.That(family.Name, Is.EqualTo("Montserrat"));
			Assert.That(family.IsStyleAvailable(FontStyle.Regular), Is.True);
			Assert.That(family.IsStyleAvailable(FontStyle.Italic), Is.True);
			Assert.That(family.IsStyleAvailable(FontStyle.Bold), Is.True);
		});
	}
	
	[Test]
	public void Constructor_GenericFont()
	{
		using FontFamily monospace = new(GenericFontFamilies.Monospace);
		Assert.That(monospace.Name, Is.AnyOf("Courier New", "Consolas", "Courier", "Menlo", "Monaco", "Lucida Console", "DejaVu Sans Mono").IgnoreCase);

		using FontFamily sanserif = new(GenericFontFamilies.SansSerif);
		Assert.That(sanserif.Name, Is.AnyOf("Arial", "Helvetica", "Verdana", "Tahoma", "Trebuchet MS", "Gill Sans", "DejaVu Sans").IgnoreCase);

		using FontFamily serif = new(GenericFontFamilies.Serif);
		Assert.That(serif.Name, Is.AnyOf("Times New Roman", "Georgia", "Garamond", "Palatino", "Book Antiqua", "Baskerville", "DejaVu Serif").IgnoreCase);
	}
}

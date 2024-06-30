using GeneXus.Drawing.Text;
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
	[TestCase("Montserrat-Italic.ttf", "Montserrat", "Italic", 400, 5, 15, SlantType.Italic, FontStyle.Italic)]
	[TestCase("Montserrat-Bold.ttf", "Montserrat", "Bold", 700, 5, 15, SlantType.Normal, FontStyle.Bold)]
	[TestCase("Graphik-Regular.otf", "Graphik", "Regular", 400, 5, 14, SlantType.Normal, FontStyle.Regular)]
	[TestCase("Graphik-Italic.otf", "Graphik", "Italic", 400, 5, 14, SlantType.Italic, FontStyle.Italic)]
	[TestCase("Graphik-Bold.otf", "Graphik", "Bold", 700, 5, 14, SlantType.Normal, FontStyle.Bold)]
	[TestCase("EncodeSans-Regular.ttf", "Encode Sans", "Regular", 400, 5, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("EncodeSans-Condensed.ttf", "Encode Sans Condensed", "Condensed", 400, 3, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("EncodeSans-Expanded.ttf", "Encode Sans Expanded", "Expanded", 400, 7, 15, SlantType.Normal, FontStyle.Regular)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Regular", 400, 5, 17, SlantType.Normal, FontStyle.Regular)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Italic", 400, 5, 17, SlantType.Italic, FontStyle.Italic)]
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Bold", 700, 5, 17, SlantType.Normal, FontStyle.Bold)]
	public void Constructor_Family(string fileName, string familyName, string faceName, int fontWeight, int fontWidth, int fontHeight, SlantType fontSlant, FontStyle fontStyle)
	{
		// this test is expected to fail in Mac with AvenirNext-Collection.ttc since it only loads the first typeface of a file
		// the ttcIndex is no supported in mac implementation
		// https://github.com/google/skia/blob/main/src/ports/SkTypeface_mac_ct.cpp
		//   sk_sp<SkTypeface> SkTypeface_Mac::MakeFromStream(std::unique_ptr<SkStreamAsset> stream, const SkFontArguments& args) {
		//	    int ttcIndex = args.getCollectionIndex();
		//	    if (ttcIndex != 0)
		//		   return nullptr;
		
		string fontPath = Path.Combine(FONT_PATH, fileName);
		using FontFamily family = FontFamily.FromFile(fontPath);

		using Font font = new(family, 12, fontStyle);
		Assert.Multiple(() =>
		{
			Assert.That(font, Is.Not.Null);
			Assert.That(font.Name, Is.EqualTo(familyName));
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
		FontFamily family = FontFamily.Families.FirstOrDefault();
		Assert.That(family, Is.Not.Null);

		using Font font = new(family.Name);
		Assert.Multiple(() =>
		{
			Assert.That(font, Is.Not.Null);
			Assert.That(font.Name, Is.EqualTo(family.Name));
			Assert.That(font.FontFamily, Is.EqualTo(family));
			Assert.That(font.FamilyName, Is.EqualTo(family.Name));
		});
	}

	[Test]
	public void Method_Clone()
	{
		string fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");
		using FontFamily family = FontFamily.FromFile(fontPath);

		using Font font1 = new(family);
		using Font font2 = font1.Clone() as Font;
		Assert.That(font1, Is.Not.SameAs(font2));
	}

	[Test]
	public void Extra_GetFontCount_FileName()
	{
		string fontPath = Path.Combine(FONT_PATH, "AvenirNext-Collection.ttc");
		int count = Font.GetFontCount(fontPath);
		Assert.That(count, Is.EqualTo(12));
	}

	[Test]
	public void Extra_GetFontCount_Stream()
	{
		string fontPath = Path.Combine(FONT_PATH, "AvenirNext-Collection.ttc");
		using FileStream stream = File.OpenRead(fontPath);
		int count = Font.GetFontCount(stream);
		Assert.That(count, Is.EqualTo(12));
	}
}

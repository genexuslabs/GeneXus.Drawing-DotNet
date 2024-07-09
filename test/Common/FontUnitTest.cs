using GeneXus.Drawing.Text;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

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
	[TestCase("AvenirNext-Collection.ttc", "Avenir Next", "Regular", 500, 5, 17, SlantType.Normal, FontStyle.Regular)]
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
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && fileName.Equals("AvenirNext-Collection.ttc") && fontStyle != FontStyle.Bold) // bold is index 0 so it works
			return;

		if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && fileName.StartsWith("EncodeSans-"))
			familyName = "Encode Sans"; // in Windows it doesn't read the correct family name for this font with skia

		string fontPath = Path.Combine(FONT_PATH, fileName);
		using PrivateFontCollection fontCollection = new();
		fontCollection.AddFontFile(fontPath);

		using Font font = new(fontCollection.Families[0], 12, fontStyle, GraphicsUnit.Pixel);
		Assert.Multiple(() =>
		{
			Assert.That(font, Is.Not.Null);
			Assert.That(font.Name, Is.EqualTo(familyName));
			Assert.That(font.Weight, Is.EqualTo(fontWeight));
			Assert.That(font.Width, Is.EqualTo(fontWidth));
			Assert.That(font.Height, Is.EqualTo(fontHeight));
			Assert.That(font.Size, Is.EqualTo(12));
			Assert.That(font.SizeInPoints, Is.EqualTo(9));
			Assert.That(font.Unit, Is.EqualTo(GraphicsUnit.Pixel));
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
		using PrivateFontCollection fontCollection = new();
		fontCollection.AddFontFile(fontPath);

		using Font font1 = new(fontCollection.Families[0]);
		using Font font2 = font1.Clone() as Font;
		Assert.That(font1, Is.Not.SameAs(font2));
	}

	[Test]
	[TestCase("Montserrat-Regular.ttf", FontStyle.Regular, "[Font: Name=Montserrat, Size=12, Style=Regular, Unit=Point]")]
	[TestCase("Montserrat-Italic.ttf", FontStyle.Italic, "[Font: Name=Montserrat, Size=12, Style=Italic, Unit=Point]")]
	[TestCase("Montserrat-Bold.ttf", FontStyle.Bold, "[Font: Name=Montserrat, Size=12, Style=Bold, Unit=Point]")]
	[TestCase("Graphik-Regular.otf", FontStyle.Regular, "[Font: Name=Graphik, Size=12, Style=Regular, Unit=Point]")]
	[TestCase("Graphik-Italic.otf", FontStyle.Italic, "[Font: Name=Graphik, Size=12, Style=Italic, Unit=Point]")]
	[TestCase("Graphik-Bold.otf", FontStyle.Bold, "[Font: Name=Graphik, Size=12, Style=Bold, Unit=Point]")]
	[TestCase("EncodeSans-Regular.ttf", FontStyle.Regular, "[Font: Name=Encode Sans, Size=12, Style=Regular, Unit=Point]")]
	[TestCase("EncodeSans-Condensed.ttf", FontStyle.Regular, "[Font: Name=Encode Sans Condensed, Size=12, Style=Regular, Unit=Point]")]
	[TestCase("EncodeSans-Expanded.ttf", FontStyle.Regular, "[Font: Name=Encode Sans Expanded, Size=12, Style=Regular, Unit=Point]")]
	[TestCase("AvenirNext-Collection.ttc", FontStyle.Regular, "[Font: Name=Avenir Next #5, Size=12, Style=Regular, Unit=Point]")]
	[TestCase("AvenirNext-Collection.ttc", FontStyle.Italic, "[Font: Name=Avenir Next #4, Size=12, Style=Italic, Unit=Point]")]
	[TestCase("AvenirNext-Collection.ttc", FontStyle.Bold, "[Font: Name=Avenir Next, Size=12, Style=Bold, Unit=Point]")]
	public void Method_ToString(string fileName, FontStyle fontStyle, string expected)
	{
		// skip problematic cases, details in Constructor_Family()
		if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) && fileName.Equals("AvenirNext-Collection.ttc") && fontStyle != FontStyle.Bold
			|| RuntimeInformation.IsOSPlatform(OSPlatform.Windows) && fileName.StartsWith("EncodeSans-"))
			return;

		string fontPath = Path.Combine(FONT_PATH, fileName);
		using PrivateFontCollection fontCollection = new();
		fontCollection.AddFontFile(fontPath);

		using Font font = new(fontCollection.Families[0], 12, fontStyle);
		Assert.That(font.ToString(), Is.EqualTo(expected));
	}

	[Test]
	[TestCase(GraphicsUnit.Pixel, 13.79883f, 13.79883f)]
	[TestCase(GraphicsUnit.Point, 18.39844f, 15.33203f)]
	[TestCase(GraphicsUnit.Inch, 1324.688f, 1103.906f)]
	[TestCase(GraphicsUnit.Millimeter, 52.15305f, 43.46087f)]
	[TestCase(GraphicsUnit.Document, 4.415625f, 3.679687f)]
	public void Method_GetHeight(GraphicsUnit unit, float height, float height80)
	{
		using Font font = new("Arial", 12, unit);
		Assert.Multiple(() => 
		{
			Assert.That(font.GetHeight(), Is.EqualTo(height).Within(0.001f));
			Assert.That(font.GetHeight(80), Is.EqualTo(height80).Within(0.001f));
		});
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

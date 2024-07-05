using GeneXus.Drawing.Text;
using System;
using System.IO;
using System.Runtime.InteropServices;

namespace GeneXus.Drawing.Test.Text;

internal class PrivateFontCollectionUnitTest
{
	private static readonly string FONT_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "fonts");

	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Default()
	{
		using var pfc = new PrivateFontCollection();
		Assert.That(pfc.Families, Is.Empty);
	}

	[Test]
	public void Method_AddFontFile()
	{
		using var pfc = new PrivateFontCollection();
		string[] fileNames = new[] { "Montserrat-Regular.ttf", "Montserrat-Italic.ttf", "Montserrat-Bold.ttf" };
		foreach (string fileName in fileNames)
		{
			string fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");
			pfc.AddFontFile(fontPath);
		}

		Assert.That(pfc.Families, Has.Length.EqualTo(3));
	}

	[Test]
	public void Method_AddMemoryFont()
	{
		using var pfc = new PrivateFontCollection();

		string[] fileNames = new[] { "Montserrat-Regular.ttf", "Montserrat-Italic.ttf", "Montserrat-Bold.ttf" };
		foreach (string fileName in fileNames)
		{
			string fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");
			byte[] fontData = File.ReadAllBytes(fontPath);
			IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
			Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

			pfc.AddMemoryFont(fontPtr, fontData.Length);

			Marshal.FreeCoTaskMem(fontPtr);
		}

		Assert.That(pfc.Families, Has.Length.EqualTo(3));
	}
}

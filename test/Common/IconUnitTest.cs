using System;
using System.IO;
using GeneXus.Drawing.Common;

namespace GeneXus.Drawing.Test;

internal class IconUnitTest
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");

	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_FileName()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var icon = new Icon(filePath);
		Assert.Multiple(() =>
		{
			Assert.That(icon, Is.Not.Null);
			Assert.That(icon.Width, Is.EqualTo(32));
			Assert.That(icon.Height, Is.EqualTo(32));
			Assert.That(icon.Size.Width, Is.EqualTo(32));
			Assert.That(icon.Size.Height, Is.EqualTo(32));
		});
	}

	[Test] // the 'Sample.ico' icon contains 5 embedded icons (16x16, 20x20, 24x24, 28x28, 32x32) and constuctor must find the best match
	[TestCase(00, 00, 16, 16)]
	[TestCase(19, 19, 20, 20)]
	[TestCase(25, 25, 24, 24)]
	[TestCase(26, 29, 28, 28)]
	[TestCase(34, 30, 32, 32)]
	[TestCase(00, 34, 32, 32)]
	[TestCase(18, 34, 20, 20)]
	public void Constructor_FileName(int width, int height, int eWidth, int eHeight)
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var icon = new Icon(filePath, width, height);
		Assert.Multiple(() =>
		{
			Assert.That(icon, Is.Not.Null);
			Assert.That(icon.Width, Is.EqualTo(eWidth));
			Assert.That(icon.Height, Is.EqualTo(eHeight));
			Assert.That(icon.Size.Width, Is.EqualTo(eWidth));
			Assert.That(icon.Size.Height, Is.EqualTo(eHeight));
		});
	}

	[Test]
	public void Constructor_Stream()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var stream = File.OpenRead(filePath);
		using var icon = new Icon(stream);
		Assert.Multiple(() =>
		{
			Assert.That(icon, Is.Not.Null);
			Assert.That(icon.Width, Is.EqualTo(32));
			Assert.That(icon.Height, Is.EqualTo(32));
			Assert.That(icon.Size.Width, Is.EqualTo(32));
			Assert.That(icon.Size.Height, Is.EqualTo(32));
		});
	}

	[Test]
	public void Constructor_Bitmap()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var bitmap = new Bitmap(filePath);
		using var icon = new Icon(bitmap);
		Assert.Multiple(() =>
		{
			Assert.That(icon, Is.Not.Null);
			Assert.That(icon.Width, Is.EqualTo(32));
			Assert.That(icon.Height, Is.EqualTo(32));
			Assert.That(icon.Size.Width, Is.EqualTo(32));
			Assert.That(icon.Size.Height, Is.EqualTo(32));
		});
	}

	[Test]
	public void Constructor_Icon()
	{
		int width = 16, height = 16;
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var icon1 = new Icon(filePath);
		using var icon2 = new Icon(icon1, width, height);
		Assert.Multiple(() =>
		{
			Assert.That(icon2, Is.Not.Null);
			Assert.That(icon2, Is.Not.EqualTo(icon1));
			Assert.That(icon2.Width, Is.EqualTo(width));
			Assert.That(icon2.Height, Is.EqualTo(height));
			Assert.That(icon2.Size.Width, Is.EqualTo(width));
			Assert.That(icon2.Size.Height, Is.EqualTo(height));
		});
	}

	[Test]
	public void Method_Clone()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var icon1 = new Icon(filePath);
		using var icon2 = icon1.Clone() as Icon;
		Assert.That(icon1, Is.Not.SameAs(icon2));
	}

	[Test]
	public void Method_ToBitmap()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var icon = new Icon(filePath);
		using var bitmap = icon.ToBitmap();
		Assert.Multiple(() =>
		{
			Assert.That(bitmap, Is.Not.Null);
			Assert.That(icon.Width, Is.EqualTo(bitmap.Width));
			Assert.That(icon.Height, Is.EqualTo(bitmap.Height));
		});
	}

	[Test]
	public void Method_Save()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.ico");
		using var icon = new Icon(filePath);
		using var stream = new MemoryStream();
		icon.Save(stream);
		Assert.That(stream.Length, Is.GreaterThan(0));

		// read the first 8 bytes and compare with PNG header
		byte[] streamHeader = new byte[8];
		stream.Seek(0, SeekOrigin.Begin);
		stream.Read(streamHeader, 0, 8);

		byte[] pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
		Assert.That(streamHeader, Is.EqualTo(pngHeader));
	}
}

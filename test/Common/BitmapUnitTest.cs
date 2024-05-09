using System;
using System.IO;
using GeneXus.Drawing.Common;

namespace GeneXus.Drawing.Test;

internal class BitmapUnitTest
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
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var bitmap = new Bitmap(filePath);
		Assert.Multiple(() =>
		{
			Assert.That(bitmap, Is.Not.Null);
			Assert.That(bitmap.Width, Is.EqualTo(64));
			Assert.That(bitmap.Height, Is.EqualTo(64));
		});
	}

	[Test]
	public void Constructor_Stream()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var stream = File.OpenRead(filePath);
		using var bitmap = new Bitmap(stream);
		Assert.Multiple(() =>
		{
			Assert.That(bitmap, Is.Not.Null);
			Assert.That(bitmap.Width, Is.EqualTo(64));
			Assert.That(bitmap.Height, Is.EqualTo(64));
		});
	}

	[Test]
	public void Constructor_WidthHeight()
	{
		using var bitmap = new Bitmap(200, 150);
		Assert.Multiple(() =>
		{
			Assert.That(bitmap, Is.Not.Null);
			Assert.That(bitmap.Width, Is.EqualTo(200));
			Assert.That(bitmap.Height, Is.EqualTo(150));
		});
	}

	[Test]
	public void Constructor_Image()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);
		using var bitmap = new Bitmap(image);
		Assert.Multiple(() =>
		{
			Assert.That(bitmap, Is.Not.Null);
			Assert.That(bitmap.Width, Is.EqualTo(64));
			Assert.That(bitmap.Height, Is.EqualTo(64));
		});
	}

	[Test]
	public void Constructor_ImageWidthHeight()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);
		using var bitmap = new Bitmap(image, 200, 150);
		Assert.Multiple(() =>
		{
			Assert.That(bitmap, Is.Not.Null);
			Assert.That(bitmap.Width, Is.EqualTo(200));
			Assert.That(bitmap.Height, Is.EqualTo(150));
		});
	}

	[Test]
	public void Method_Clone()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var bitmap1 = new Bitmap(filePath);
		using var bitmap2 = bitmap1.Clone() as Bitmap;
		Assert.That(bitmap1, Is.Not.SameAs(bitmap2));
	}

	[Test]
	public void GetPixel_SetGetPixel()
	{
		using var bitmap = new Bitmap(100, 100);
		bitmap.SetPixel(50, 50, Color.Red);
		var pixelColor = bitmap.GetPixel(50, 50);
		Assert.That(pixelColor, Is.EqualTo(Color.Red));
	}
}

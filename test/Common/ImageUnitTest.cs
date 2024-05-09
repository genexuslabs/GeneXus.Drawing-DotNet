using System;
using System.IO;
using GeneXus.Drawing.Imaging;

namespace GeneXus.Drawing.Test;

internal class ImageUnitTest
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");

	[SetUp]
	public void Setup()
	{
	}

	[Test]
	[TestCase("Sample.bmp", ImageFormat.Bmp)]
	[TestCase("Sample.gif", ImageFormat.Gif)]
	[TestCase("Sample.png", ImageFormat.Png)]
	[TestCase("Sample.jpg", ImageFormat.Jpeg)]
	[TestCase("Sample.webp", ImageFormat.Webp)]
	[TestCase("Sample.svg", ImageFormat.Svg)]
	public void Method_FromFile(string fileName, ImageFormat format)
	{
		var filePath = Path.Combine(IMAGE_PATH, fileName);
		using var image = Image.FromFile(filePath);
		Assert.Multiple(() =>
		{
			Assert.That(image, Is.Not.Null);
			Assert.That(image.Width, Is.EqualTo(64));
			Assert.That(image.Height, Is.EqualTo(64));
			Assert.That(image.Size.Width, Is.EqualTo(64));
			Assert.That(image.Size.Height, Is.EqualTo(64));
			Assert.That(image.RawFormat, Is.EqualTo(format));
		});
	}

	[Test]
	[TestCase("Sample.png")]
	public void Method_FromStream(string fileName)
	{
		var filePath = Path.Combine(IMAGE_PATH, fileName);
		using var stream = File.OpenRead(filePath);
		using var image = Image.FromStream(stream);
		Assert.Multiple(() =>
		{
			Assert.That(image, Is.Not.Null);
			Assert.That(image.Width, Is.EqualTo(64));
			Assert.That(image.Height, Is.EqualTo(64));
			Assert.That(image.RawFormat, Is.EqualTo(ImageFormat.Png));
		});
	}

	[Test]
	public void Method_Clone()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image1 = Image.FromFile(filePath);
		using var image2 = image1.Clone() as Image;
		Assert.That(image1, Is.Not.SameAs(image2));
	}

	[Test]
	public void Method_Save_Stream()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);
		using var stream = new MemoryStream();
		image.Save(stream, ImageFormat.Png);
		Assert.That(stream.Length, Is.GreaterThan(0));
	}

	[Test]
	public void Method_Save_FileName()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		var savePath = Path.Combine(IMAGE_PATH, "Sample.out.png");
		using var image = Image.FromFile(filePath);
		image.Save(savePath);
		Assert.That(File.Exists(savePath), Is.True);

		// ensure saved file is a PNG
		using (var fileStream = File.OpenRead(savePath))
		{
			var fileHeader = new byte[8];
			fileStream.Read(fileHeader, 0, fileHeader.Length);
			var pngHeader = new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A };
			Assert.That(fileHeader, Is.EqualTo(pngHeader));
		}

		// delete temp file
		File.Delete(savePath);
	}

	[Test]
	public void Method_Save_FileName_Format()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		var savePath = Path.Combine(IMAGE_PATH, "Sample.out.jpg");
		using var image = Image.FromFile(filePath);
		image.Save(savePath, ImageFormat.Jpeg);
		Assert.That(File.Exists(savePath), Is.True);

		// ensure saved file is a JPEG
		using (var fileStream = File.OpenRead(savePath))
		{
			var fileHeader = new byte[2];
			fileStream.Read(fileHeader, 0, fileHeader.Length);
			var jpegHeader = new byte[] { 0xFF, 0xD8 };
			Assert.That(fileHeader, Is.EqualTo(jpegHeader));
		}

		// delete temp file
		File.Delete(savePath);
	}

	[Test]
	public void Method_GetFrameCount()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.gif");
		using var image = Image.FromFile(filePath);
		Assert.That(image.GetFrameCount(), Is.EqualTo(6));
	}

	[Test]
	public void Method_SelectActiveFrame()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.gif");
		using var image = Image.FromFile(filePath);
		image.SelectActiveFrame(4);

		// check color change on selected frame (red instead of blue)
		using var bitmap = new Bitmap(image);
		Assert.That(bitmap.GetPixel(40, 10), Is.EqualTo(Color.Red));
	}
}

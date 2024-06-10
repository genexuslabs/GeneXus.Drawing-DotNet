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

	[Test]
	[TestCase(RotateFlipType.RotateNoneFlipNone, 0, 1, 1)]
	[TestCase(RotateFlipType.RotateNoneFlipX, 0, -1, 1)]
	[TestCase(RotateFlipType.RotateNoneFlipY, 0, 1, -1)]
	[TestCase(RotateFlipType.RotateNoneFlipXY, 0, -1, -1)]
	[TestCase(RotateFlipType.Rotate90FlipNone, 90, 1, 1)]
	[TestCase(RotateFlipType.Rotate90FlipX, 90, -1, 1)]
	[TestCase(RotateFlipType.Rotate90FlipY, 90, 1, -1)]
	[TestCase(RotateFlipType.Rotate90FlipXY, 90, -1, -1)]
	public void Method_RotateFlip(RotateFlipType type, int deg, int sx, int sy)
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);

		using var rotated = image.Clone() as Image;
		rotated.RotateFlip(type);

		(int pivW, int pivH) = (image.Width, image.Height);
		(int newW, int newH) = deg switch // calculate size after rotation and flipping
		{
			 0 or 180 => (image.Width, image.Height),
			90 or 270 => (image.Height, image.Width),
			_ => throw new ArgumentException($"unrecognized angle {deg}", nameof(deg))
		};

		Assert.Multiple(() =>
		{
			Assert.That(rotated.Width, Is.EqualTo(newW));
			Assert.That(rotated.Height, Is.EqualTo(newH));
		});

		(int pivX, int pivY) = (17, 23);
		(int newX, int newY) = deg switch // calculate coordinates after rotation and flipping
		{
			  0 => (sx == -1 ? pivW - 1 - pivX : pivX, sy == -1 ? pivH - 1 - pivY : pivY),
			 90 => (sx == -1 ? pivY : pivW - 1 - pivY, sy == -1 ? pivH - 1 - pivX : pivX),
			180 => (sx == -1 ? pivX : pivW - 1 - pivX, sy == -1 ? pivY : pivH - 1 - pivY),
			270 => (sx == -1 ? pivH - 1 - pivY : pivY, sy == -1 ? pivX : pivW - 1 - pivX),
			_ => throw new ArgumentException($"unrecognized angle {deg}", nameof(deg))
		};

		using var expected = new Bitmap(image);
		using var obtained = new Bitmap(rotated);
		Assert.That(obtained.GetPixel(newX, newY), Is.EqualTo(expected.GetPixel(pivX, pivY)));
	}

	[Test]
	public void Method_GetThumbnailImage()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);
		using var thumb = image.GetThumbnailImage(20, 20, new Image.GetThumbnailImageAbort(() => false), IntPtr.Zero);
		Assert.Multiple(() =>
		{
			Assert.That(thumb.Width, Is.EqualTo(20));
			Assert.That(thumb.Height, Is.EqualTo(20));
		});
	}

	[Test]
	public void Method_GetBounds()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);
		var unit = GraphicsUnit.Pixel;
		var bounds = image.GetBounds(ref unit);
		Assert.Multiple(() =>
		{
			Assert.That(unit, Is.EqualTo(GraphicsUnit.Pixel));
			Assert.That(bounds.X, Is.Zero);
			Assert.That(bounds.Y, Is.Zero);
			Assert.That(bounds.Width, Is.EqualTo(image.Width));
			Assert.That(bounds.Height, Is.EqualTo(image.Height));
		});
	}
}

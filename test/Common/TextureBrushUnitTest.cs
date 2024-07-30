using System;
using System.IO;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test;

internal class TextureBrushUnitTest
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");

	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	[TestCase(WrapMode.Tile, "wrap-tile.png")]
	[TestCase(WrapMode.Clamp, "wrap-clamp.png")]
	public void Constructor_BitmapRectWrap(WrapMode mode, string expected)
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var bitmap = new Bitmap(filePath);
		var rect = new RectangleF(15, 15, 20, 20);

		var matrix = new Matrix();
		matrix.Rotate(45);
		matrix.Translate(10, 10);

		using var brush = new TextureBrush(bitmap, mode, rect) { Transform = matrix };
		Assert.Multiple(() =>
		{
			Assert.That(brush.Image, Is.EqualTo(bitmap));
			Assert.That(brush.WrapMode, Is.EqualTo(mode));
			Assert.That(brush.Transform, Is.EqualTo(matrix));

			string path = Path.Combine("brush", "textured", expected);
			float similarity = Utils.CompareImage(path, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.95));
		});
	}

	[Test]
	public void Method_Clone()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var bitmap = new Bitmap(filePath);

		using var brush1 = new TextureBrush(bitmap, WrapMode.Tile, new Rectangle(0, 0, 100, 50)) { Transform = new Matrix(1, 2, 3, 4, 5, 6) };
		using var brush2 = brush1.Clone() as TextureBrush;
		Assert.Multiple(() =>
		{
			Assert.That(brush2, Is.Not.Null);
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.Image, Is.EqualTo(brush1.Image));
			Assert.That(brush2.WrapMode, Is.EqualTo(brush1.WrapMode));
			Assert.That(brush2.Transform, Is.EqualTo(brush1.Transform));
		});
	}
}
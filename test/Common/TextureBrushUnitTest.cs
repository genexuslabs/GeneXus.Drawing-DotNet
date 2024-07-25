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
	public void Constructor_BitmapRectWrap()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var bitmap = new Bitmap(filePath);
		var rect = new Rectangle(0, 0, 100, 50);
		var wrapMode = WrapMode.Tile;
		var matrix = new Matrix(1, 2, 3, 4, 5, 6);

		using var brush = new TextureBrush(bitmap, rect, wrapMode) { Transform = matrix };
		Assert.Multiple(() =>
		{
			Assert.That(brush.Image, Is.EqualTo(bitmap));
			Assert.That(brush.WrapMode, Is.EqualTo(wrapMode));
			Assert.That(brush.Transform, Is.EqualTo(matrix));
		});
	}

	[Test]
	public void Method_Clone()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var bitmap = new Bitmap(filePath);

		using var brush1 = new TextureBrush(bitmap, new Rectangle(0, 0, 100, 50), WrapMode.Tile) { Transform = new Matrix(1, 2, 3, 4, 5, 6) };
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
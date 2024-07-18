using System;
using System.IO;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test;

internal class RecgionUnitTest
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");
		
	[Test]
	public void Constructor_Default()
	{
		using var region = new Region();
		Assert.Multiple(() =>
		{
			Assert.That(region, Is.Not.Null);
			Assert.That(region.GetBounds(null), Is.EqualTo(RectangleF.Empty));
		});
	}

	[Test]
	public void Constructor_RectangleF()
	{
		var rect = new RectangleF(0, 0, 100, 100);
		
		using var region = new Region(rect);
		Assert.Multiple(() =>
		{
			Assert.That(region, Is.Not.Null);
			Assert.That(region.GetBounds(null), Is.EqualTo(rect));
		});
	}

	[Test]
	public void Constructor_Rectangle()
	{
		var rectF = new RectangleF(0, 0, 100, 100);
		var rectI = RectangleF.Truncate(rectF);
		
		using var region = new Region(rectI);
		Assert.Multiple(() =>
		{
			Assert.That(region, Is.Not.Null);
			Assert.That(region.GetBounds(null), Is.EqualTo(rectF));
		});
	}

	[Test]
	public void Constructor_GraphicsPath()
	{
		var rect = new RectangleF(0, 0, 100, 100);

		var path = new GraphicsPath();
		path.AddRectangle(rect);
		
		using var region = new Region(path);
		Assert.Multiple(() =>
		{
			Assert.That(region, Is.Not.Null);
			Assert.That(region.GetBounds(null), Is.EqualTo(rect));
		});
	}

	[Test]
	public void Constructor_RegionData()
	{
		var rect = new RectangleF(0, 0, 100, 100);
		
		using var initialRegion = new Region(rect);
		var data = initialRegion.GetRegionData();
		
		using var region = new Region(data);
		Assert.Multiple(() =>
		{
			Assert.That(region, Is.Not.Null);
			Assert.That(region.GetBounds(null), Is.EqualTo(rect));
		});
	}

	[Test]
	public void Method_Clone()
	{
		var rect = new RectangleF(0, 0, 100, 100);

		using var region = new Region(rect);
		using var clone = region.Clone();

		Assert.Multiple(() =>
		{
			Assert.That(clone, Is.Not.Null);
			Assert.That(clone, Is.TypeOf<Region>());
			Assert.That(clone, Is.Not.SameAs(region));
			Assert.That(clone.GetBounds(null), Is.EqualTo(rect));
		});
	}

	[Test]
	public void Method_Complement_RectangleF()
	{
		using var region1 = new Region(new RectangleF(0, 0, 100, 100));
		using var region2 = new Region(new RectangleF(50, 50, 100, 100));

		region1.Complement(region2);
		Assert.That(region1.IsVisible(50, 50), Is.False);
	}

	[Test]
	public void Method_Complement_GraphicsPath()
	{
		var path = new GraphicsPath();
		path.AddRectangle(new RectangleF(50, 50, 100, 100));
		
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Complement(path);
		Assert.That(region.IsVisible(50, 50), Is.False);
	}

	[Test]
	public void Method_Intersect_Region()
	{
		using var region1 = new Region(new RectangleF(0, 0, 100, 100));
		using var region2 = new Region(new RectangleF(50, 50, 100, 100));

		region1.Intersect(region2);
		Assert.That(region1.IsVisible(75, 75), Is.True);
	}

	[Test]
	public void Method_Intersect_RectangleF()
	{
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Intersect(new RectangleF(50, 50, 100, 100));
		Assert.That(region.IsVisible(75, 75), Is.True);
	}

	[Test]
	public void Method_Intersect_GraphicsPath()
	{
		var path = new GraphicsPath();
		path.AddRectangle(new RectangleF(50, 50, 100, 100));

		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Intersect(path);
		Assert.That(region.IsVisible(75, 75), Is.True);
	}

	[Test]
	public void Method_Union_Region()
	{
		using var region1 = new Region(new RectangleF(0, 0, 100, 100));
		using var region2 = new Region(new RectangleF(50, 50, 100, 100));

		region1.Union(region2);
		Assert.That(region1.IsVisible(75, 75), Is.True);
		Assert.That(region1.IsVisible(25, 25), Is.True);
	}

	[Test]
	public void Method_Union_RectangleF()
	{
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Union(new RectangleF(50, 50, 100, 100));
		Assert.That(region.IsVisible(75, 75), Is.True);
		Assert.That(region.IsVisible(25, 25), Is.True);
	}

	[Test]
	public void Method_Union_GraphicsPath()
	{
		var path = new GraphicsPath();
		path.AddRectangle(new RectangleF(50, 50, 100, 100));

		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Union(path);
		Assert.That(region.IsVisible(75, 75), Is.True);
		Assert.That(region.IsVisible(25, 25), Is.True);
	}

	[Test]
	public void Method_Xor_Region()
	{
		using var region1 = new Region(new RectangleF(0, 0, 100, 100));
		using var region2 = new Region(new RectangleF(50, 50, 100, 100));

		region1.Xor(region2);
		Assert.That(region1.IsVisible(75, 75), Is.False);
		Assert.That(region1.IsVisible(25, 25), Is.True);
		Assert.That(region1.IsVisible(125, 125), Is.True);
	}

	[Test]
	public void Method_Xor_RectangleF()
	{
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Xor(new RectangleF(50, 50, 100, 100));
		Assert.That(region.IsVisible(75, 75), Is.False);
		Assert.That(region.IsVisible(25, 25), Is.True);
		Assert.That(region.IsVisible(125, 125), Is.True);
	}

	[Test]
	public void Method_Xor_GraphicsPath()
	{
		var path = new GraphicsPath();
		path.AddRectangle(new RectangleF(50, 50, 100, 100));
		
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Xor(path);
		Assert.That(region.IsVisible(75, 75), Is.False);
		Assert.That(region.IsVisible(25, 25), Is.True);
		Assert.That(region.IsVisible(125, 125), Is.True);
	}

	[Test]
	public void Method_GetBounds_Graphics()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);
		using var graphics = Graphics.FromImage(image);
		
		var rect = new RectangleF(0, 0, 100, 100);
		using var region = new Region(rect);

		var bounds = region.GetBounds(graphics);
		Assert.That(bounds, Is.EqualTo(rect));
	}

	[Test]
	public void Method_GetRegionData()
	{
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		var data = region.GetRegionData();

		Assert.That(data, Is.Not.Null);
	}

	[Test]
	public void Method_Transform_Matrix()
	{
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		var matrix = new Matrix();
		matrix.Translate(50, 50);
		
		region.Transform(matrix);
		Assert.That(region.IsVisible(75, 75), Is.True);
	}

	[Test]
	public void Method_Translate()
	{
		using var region = new Region(new RectangleF(0, 0, 100, 100));
		
		region.Translate(50, 50);
		Assert.That(region.IsVisible(75, 75), Is.True);
	}

	[Test]
	public void Method_GetRegionScans()
	{
		var rect = new RectangleF(0, 0, 100, 100);

		using var region = new Region(rect);
		var scans = region.GetRegionScans(new Matrix());

		Assert.That(scans, Is.Not.Null);
		Assert.That(scans.Length, Is.EqualTo(1));
		Assert.That(scans[0], Is.EqualTo(rect));
	}
}
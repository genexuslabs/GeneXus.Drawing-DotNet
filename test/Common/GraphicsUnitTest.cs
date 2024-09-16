using System;
using System.Diagnostics;
using System.IO;
using GeneXus.Drawing.Drawing2D;
using GeneXus.Drawing.Text;

namespace GeneXus.Drawing.Test;

internal class GraphicsUnitTest
{
	private static readonly string IMAGE_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "images");

	private static readonly string FONT_PATH = Path.Combine(
		Directory.GetParent(Environment.CurrentDirectory).Parent.FullName,
		"res", "fonts");

	[SetUp]
	public void Setup()
	{
	}


	#region Factory methods

	[Test]
	public void Method_FromImage()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);
		Assert.That(bitmap.GetPixel(25, 25), Is.EqualTo(Color.Empty));
	}

	#endregion


	#region Propeties

	[Test]
	[TestCase(CompositingMode.SourceCopy, "#FF000080")]
	[TestCase(CompositingMode.SourceOver, "#80007FFF")]
	public void Property_CompositingMode(CompositingMode mode, string hex)
	{
		var color = Color.FromArgb(128, 255, 0, 0);
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.Clear(Color.Blue);
		g.CompositingMode = mode;
		g.FillRectangle(brush, 10, 10, 30, 30);

		Assert.Multiple(() =>
		{
			var expected = Color.FromHex(hex);
			Assert.That(bitmap.GetPixel(0, 0), Is.EqualTo(Color.Blue));
			Assert.That(bitmap.GetPixel(25, 25), Is.EqualTo(expected));
		});
	}

	[Test]
	[TestCase(0, 0)]
	[TestCase(25, 25)]
	public void Property_RenderingOrigin(int x, int y)
	{
		using var brush = new HatchBrush(HatchStyle.DiagonalCross, Color.Green, Color.Orange);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.Clear(Color.Blue);
		g.RenderingOrigin = new Point(x, y);
		g.FillRectangle(brush, 10, 10, 30, 30);

		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"RenderingOrigin_{x:D2}-{y:D2}.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	#endregion


	#region Utilities methods

	[Test]
	public void Method_Clear()
	{
		var color = Color.Red;

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.Clear(color);
		Assert.Multiple(() =>
		{
			for (int i = 0; i < bitmap.Width; i++)
				for (int j = 0; j < bitmap.Height; j++)
					Assert.That(bitmap.GetPixel(i, j), Is.EqualTo(color));
		});
	}

	[Test]
	public void Method_CopyFromScreen()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var size = new Size(25, 25);
		Assert.Throws<NotSupportedException>(() =>
		{
			g.CopyFromScreen(0, 0, bitmap.Width, bitmap.Height, size);
		});
	}

	[Test]
	public void Method_GetNearestColor()
	{
        var color = Color.FromArgb(123, 200, 123);
		
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var nearest = g.GetNearestColor(color);
		Assert.Multiple(() =>
        {
            Assert.That(nearest.R, Is.EqualTo(color.R).Within(5));
            Assert.That(nearest.G, Is.EqualTo(color.G).Within(5));
            Assert.That(nearest.B, Is.EqualTo(color.B).Within(5));
        });
	}

	[Test]
	public void Method_IsVisible()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.FillRectangle(brush, 10, 10, 30, 30);
		Assert.Multiple(() =>
		{
			Assert.That(g.IsVisible(25, 25), Is.True);
			Assert.That(g.IsVisible(50, 50), Is.False);
		});
	}

	#endregion


	#region State methods

	[Test]
	public void Method_SaveAndRestore()
	{
		// NOTE: Save/Restore only works over transform matrix in both System.Drawing and SkiaSharp
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.TranslateTransform(10, 10);

		var state = g.Save();
		Assert.Multiple(() =>
		{
			Assert.That(g.Transform.OffsetX, Is.EqualTo(10));
			Assert.That(g.Transform.OffsetY, Is.EqualTo(10));
		});

		g.TranslateTransform(5, 5);
		Assert.Multiple(() =>
		{
			Assert.That(g.Transform.OffsetX, Is.EqualTo(15));
			Assert.That(g.Transform.OffsetY, Is.EqualTo(15));
		});

		g.Restore(state);
		Assert.Multiple(() =>
		{
			Assert.That(g.Transform.OffsetX, Is.EqualTo(10));
			Assert.That(g.Transform.OffsetY, Is.EqualTo(10));
		});
	}


	[Test]
	public void Method_BeginAndEndContainer()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.TranslateTransform(10, 10);

		var state = g.BeginContainer();
		Assert.Multiple(() =>
		{
			Assert.That(g.Transform.OffsetX, Is.EqualTo(0));
			Assert.That(g.Transform.OffsetY, Is.EqualTo(0));
		});

		g.TranslateTransform(5, 5);
		Assert.Multiple(() =>
		{
			Assert.That(g.Transform.OffsetX, Is.EqualTo(5));
			Assert.That(g.Transform.OffsetY, Is.EqualTo(5));
		});

		g.EndContainer(state);
		Assert.Multiple(() =>
		{
			Assert.That(g.Transform.OffsetX, Is.EqualTo(10));
			Assert.That(g.Transform.OffsetY, Is.EqualTo(10));
		});
	}

	#endregion


	#region Draw methods

	[Test]
	public void Method_DrawArc()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawArc(pen, 0, 0, 50, 50, 0, 45);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawArc.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawBezier()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawBezier(pen, 0, 25, 5, 15, 15, 5, 25, 0);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawBezier.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawClosedCurve()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var points = new Point[] { new(0, 25), new(25, 0), new(25, 25) };
		var tension = 0.75f;
		var mode = FillMode.Winding;

		g.DrawClosedCurve(pen, points, tension, mode);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawClosedCurve.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawCurve()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);
		
		var points = new Point[] { new(0, 25), new(25, 0), new(25, 25) };
		var tension = 0.75f;

		g.DrawCurve(pen, points, tension);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawCurve.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawEllipse()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawEllipse(pen, 10, 10, 30, 30);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawEllipse.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawIcon()
	{
		// TODO: test DrawIcon method
	}

	[Test]
	public void Method_DrawImage()
	{
		var filePath = Path.Combine(IMAGE_PATH, "Sample.png");
		using var image = Image.FromFile(filePath);

		var portion = new Rectangle(10, 10, 30, 30);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawImage(image, 10, 10, portion, GraphicsUnit.Pixel);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawImage.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawLine()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawLine(pen, 10, 10, 40, 40);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawLine.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawPath()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		using var path = new GraphicsPath();
		path.AddLine(0, 0, 50, 0);
		path.AddLine(50, 0, 25, 25);
		path.AddLine(25, 25, 0, 0);
		path.CloseFigure(); // defines a triangle

		g.DrawPath(pen, path);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawPath.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawPie()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawPie(pen, 10, 10, 30, 30, 0, 45);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawPie.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawPolygon()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var points = new[] { new PointF(0, 0), new PointF(25, 25), new PointF(50, 0), new PointF(25, 50) };

		g.DrawPolygon(pen, points);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawPolygon.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawRectangle()
	{
		var color = Color.Red;
		using var pen = new Pen(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.DrawRectangle(pen, 10, 10, 30, 30);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawRectangle.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_DrawString()
	{
		string fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");

		using var pfc = new PrivateFontCollection();
		pfc.AddFontFile(fontPath);

		var font = new Font(pfc.Families[0], 10);

		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var bounds = new RectangleF(0, 0, 40, 40);

		var flags = StringFormatFlags.NoWrap;
		var format = new StringFormat(flags)
		{
			HotkeyPrefix = HotkeyPrefix.Show,
			Trimming = StringTrimming.EllipsisCharacter,
		};

		g.TextRenderingHint = TextRenderingHint.SingleBitPerPixel;
		g.DrawString("&hello\nworld\n123", font, brush, bounds, format);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"DrawString.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	#endregion


	#region Fill methods

	[Test]
	public void Method_FillClosedCurve()
	{
		// TODO: test FillClosedCurve method	
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var points = new Point[] { new(0, 25), new(25, 0), new(25, 25) };
		var tension = 0.75f;
		var mode = FillMode.Winding;

		g.FillClosedCurve(brush, points, mode, tension);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillClosedCurve.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9).Within(0.1));
		});
	}

	[Test]
	public void Method_FillEllipse()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.FillEllipse(brush, 10, 10, 30, 30);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillEllipse.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_FillPath()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		using var path = new GraphicsPath();
		path.AddLine(0, 0, 50, 0);
		path.AddLine(50, 0, 25, 25);
		path.AddLine(25, 25, 0, 0);
		path.CloseFigure(); // defines a triangle

		g.FillPath(brush, path);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillPath.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_FillPie()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.FillPie(brush, 10, 10, 30, 30, 0, 45);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillPie.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_FillPolygon()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var points = new[] { new PointF(0, 0), new PointF(25, 25), new PointF(50, 0), new PointF(25, 50) };

		g.FillPolygon(brush, points);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillPolygon.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_FillRectangle()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.FillRectangle(brush, 10, 10, 30, 30);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillRectangle.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_FillRegion()
	{
		var color = Color.Red;
		using var brush = new SolidBrush(color);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var path = new GraphicsPath();
		path.AddLine(10, 10, 40, 10);
		path.AddBezier(40, 10, 30, 40, 20, 40, 10, 10);
		path.CloseFigure();

		using var region = new Region(path);

		g.FillRegion(brush, region);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"FillRegion.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	#endregion


	#region Text methods

	[Test]
	public void Method_MeasureCharacterRanges()
	{
		string fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");

		using var pfc = new PrivateFontCollection();
		pfc.AddFontFile(fontPath);

		var font = new Font(pfc.Families[0], 10);

		var flags = StringFormatFlags.NoWrap;
		var format = new StringFormat(flags)
		{
			HotkeyPrefix = HotkeyPrefix.Show,
			Trimming = StringTrimming.EllipsisCharacter,
		};

		var charRanges = new[] { new CharacterRange(0, 4) };
		format.SetMeasurableCharacterRanges(charRanges);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		var bounds = new RectangleF(0, 0, bitmap.Width, bitmap.Height);

		var regions = g.MeasureCharacterRanges("Hello World", font, bounds, format);
		Assert.Multiple(() =>
		{
			Assert.That(regions, Has.Length.EqualTo(charRanges.Length));
			var bounds = regions[0].GetBounds(g);
			Assert.That(bounds.X, Is.EqualTo(3).Within(1));
			Assert.That(bounds.Y, Is.EqualTo(0).Within(1));
			Assert.That(bounds.Width, Is.EqualTo(27).Within(5));  // NOTE: huge difference but skia draws text's path in pixel, that's why this value is smaller than expected
			Assert.That(bounds.Height, Is.EqualTo(17).Within(1));
		});
	}

	[Test]
	public void Method_MeasureString()
	{
		string fontPath = Path.Combine(FONT_PATH, "Montserrat-Regular.ttf");

		using var pfc = new PrivateFontCollection();
		pfc.AddFontFile(fontPath);

		var font = new Font(pfc.Families[0], 10);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);
		
		var measure = g.MeasureString("Hello World", font);
		Assert.Multiple(() =>
		{
			Assert.That(measure.Width, Is.EqualTo(85).Within(11)); // NOTE: huge difference but skia draws text's path in pixel, that's why this value is smaller than expected
			Assert.That(measure.Height, Is.EqualTo(17).Within(1));
		});
	}

	#endregion


	#region Clip methods

	[Test]
	public void Method_ExcludeClip()
	{
		var color = Color.Red;
		var clip1 = new Rectangle(0, 0, 25, 25);
		var clip2 = new Rectangle(10, 10, 30, 30);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.SetClip(clip1);
		g.ExcludeClip(clip2);
		g.Clear(color);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"ClipExclude.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_IntersectClip()
	{
		var color = Color.Red;
		var clip1 = new Rectangle(0, 0, 25, 25);
		var clip2 = new Rectangle(10, 10, 10, 10);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.SetClip(clip1);
		g.IntersectClip(clip2);
		g.Clear(color);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"ClipIntersect.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_TranslateClip()
	{
		var color = Color.Red;
		var clip = new Rectangle(0, 0, 25, 25);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.SetClip(clip);
		g.TranslateClip(10, 10);
		g.Clear(color);
		Assert.Multiple(() =>
		{
			string filepath = Path.Combine("graphics", $"ClipTranslate.png");
			float similarity = Utils.CompareImage(filepath, bitmap, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_SetClip()
	{
		var color = Color.Red;
		var clip = new Rectangle(0, 0, 25, 25);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);
		
		g.SetClip(clip);
		g.Clear(color);
		Assert.Multiple(() =>
		{
			for (int i = 0; i < bitmap.Width; i++)
				for (int j = 0; j < bitmap.Height; j++)
					Assert.That(bitmap.GetPixel(i, j), Is.EqualTo(clip.Contains(i, j) ? color : Color.Empty));
		});
	}

	[Test]
	public void Method_ResetClip()
	{
		var color = Color.Red;
		var clip = new Rectangle(0, 0, 25, 25);

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);
		
		g.SetClip(clip);
		g.Clear(color);
		g.ResetClip();
		Assert.Multiple(() =>
		{
			for (int i = 0; i < bitmap.Width; i++)
				for (int j = 0; j < bitmap.Height; j++)
					Assert.That(bitmap.GetPixel(i, j), Is.EqualTo(color));
		});
	}

	#endregion


	#region Transform methods

	[Test]
	public void Method_MultiplyTransform()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		using var matrix = new Matrix(6, 5, 4, 3, 2, 1);

		g.Transform = new Matrix(1, 2, 3, 4, 5, 6);
		g.MultiplyTransform(matrix);
		Assert.That(g.Transform.Elements, Is.EqualTo(new[] { 21, 32, 13, 20, 10, 14 }).Within(0.001f));
	}

	[Test]
	public void Method_ResetTransform()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.TranslateTransform(20, 30);
    	g.ResetTransform();
		Assert.That(g.Transform.IsIdentity, Is.True);
	}

	[Test]
	public void Method_RotateTransform()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.RotateTransform(45);
		Assert.That(g.Transform.Elements, Is.EqualTo(new[] { 0.707f, 0.707f, -0.707f, 0.707f, 0, 0 }).Within(0.001f));
	}

	[Test]
	public void Method_ScaleTransform()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.ScaleTransform(0.50f, 0.25f);
		Assert.That(g.Transform.Elements, Is.EqualTo(new[] { 0.5f, 0, 0, 0.25f, 0, 0 }).Within(0.001f));
	}

	[Test]
	public void Method_TranslateTransform()
	{
		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.TranslateTransform(50, 50f);
		Assert.That(g.Transform.Elements, Is.EqualTo(new[] { 1, 0, 0, 1, 50, 50 }).Within(0.001f));
	}

	[Test]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.World, CoordinateSpace.World, 10f, 10f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.World, CoordinateSpace.Page, 20f, 5f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.World, CoordinateSpace.Device, 40f, 10f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.Page, CoordinateSpace.Page, 10f, 10f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.Page, CoordinateSpace.World, 5f, 20f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.Page, CoordinateSpace.Device, 20f, 20f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.Device, CoordinateSpace.Device, 10f, 10f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.Device, CoordinateSpace.World, 2.5f, 10f)]
	[TestCase(GraphicsUnit.Pixel, 0.5f, CoordinateSpace.Device, CoordinateSpace.Page, 5f, 5f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.World, CoordinateSpace.World, 10f, 10f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.World, CoordinateSpace.Page, 20f, 5f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.World, CoordinateSpace.Device, 0.4166666f, 0.1041667f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.Page, CoordinateSpace.Page, 10f, 10f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.Page, CoordinateSpace.World, 5f, 20f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.Page, CoordinateSpace.Device, 0.2083333f, 0.2083333f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.Device, CoordinateSpace.Device, 10f, 10f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.Device, CoordinateSpace.World, 240f, 960f)]
	[TestCase(GraphicsUnit.Inch, 0.5f, CoordinateSpace.Device, CoordinateSpace.Page, 480f, 480f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.World, CoordinateSpace.World, 10f, 10f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.World, CoordinateSpace.Page, 20f, 5f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.World, CoordinateSpace.Device, 10.58333f, 2.645833f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.Page, CoordinateSpace.Page, 10f, 10f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.Page, CoordinateSpace.World, 5f, 20f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.Page, CoordinateSpace.Device, 5.291666f, 5.291666f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.Device, CoordinateSpace.Device, 10f, 10f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.Device, CoordinateSpace.World, 9.448818f, 37.79527f)]
	[TestCase(GraphicsUnit.Millimeter, 0.5f, CoordinateSpace.Device, CoordinateSpace.Page, 18.89764f, 18.89764f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.World, CoordinateSpace.World, 10f, 10f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.World, CoordinateSpace.Page, 20f, 5f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.World, CoordinateSpace.Device, 30f, 7.5f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.Page, CoordinateSpace.Page, 10f, 10f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.Page, CoordinateSpace.World, 5f, 20f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.Page, CoordinateSpace.Device, 15f, 15f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.Device, CoordinateSpace.Device, 10f, 10f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.Device, CoordinateSpace.World, 3.333333f, 13.33333f)]
	[TestCase(GraphicsUnit.Point, 0.5f, CoordinateSpace.Device, CoordinateSpace.Page, 6.666666f, 6.666666f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.World, CoordinateSpace.World, 10f, 10f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.World, CoordinateSpace.Page, 20f, 5f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.World, CoordinateSpace.Device, 125f, 31.25f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.Page, CoordinateSpace.Page, 10f, 10f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.Page, CoordinateSpace.World, 5f, 20f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.Page, CoordinateSpace.Device, 62.5f, 62.5f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.Device, CoordinateSpace.Device, 10f, 10f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.Device, CoordinateSpace.World, 0.8f, 3.2f)]
	[TestCase(GraphicsUnit.Document, 0.5f, CoordinateSpace.Device, CoordinateSpace.Page, 1.6f, 1.6f)]
	public void Method_TransformPoints(GraphicsUnit unit, float scale, CoordinateSpace destination, CoordinateSpace source, float expectedX, float expectedY)
	{
		var points = new PointF[] { new(10, 10) };

		using var bitmap = new Bitmap(50, 50);
		using var g = Graphics.FromImage(bitmap);

		g.ScaleTransform(0.5f, 2);

		g.PageUnit = unit;
		g.PageScale = scale;

		g.TransformPoints(destination, source, points);
		Assert.Multiple(() =>
		{
			Assert.That(points[0].X, Is.EqualTo(expectedX).Within(0.001f));
			Assert.That(points[0].Y, Is.EqualTo(expectedY).Within(0.001f));
		});
	}

	#endregion
}
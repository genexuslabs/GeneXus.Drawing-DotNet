namespace GeneXus.Drawing.Test;

internal class RectangleUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Int()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_PointSize()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var point = new Point(x, y);
		var size = new Size(w, h);
		var rect = new Rectangle(point, size);
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_FloatSize()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var size = new Size(w, h);
		var rect = new Rectangle(x, y, size);
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_PointFloat()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var point = new Point(x, y);
		var rect = new Rectangle(point, w, h);
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Operator_Equality()
	{
		var rect1 = new Rectangle(5, 10, 100, 200);
		var rect2 = new Rectangle(5, 10, 100, 200);
		Assert.That(rect1 == rect2, Is.True);
	}

	[Test]
	public void Operator_Inequality()
	{
		var rect1 = new Rectangle(5, 10, 100, 200);
		var rect2 = new Rectangle(10, 5, 200, 100);
		Assert.That(rect1 != rect2, Is.True);
	}

	[Test]
	public void Property_X()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		rect.X += 5;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x + 5));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Property_Y()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		rect.Y += 5;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y + 5));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Property_Width()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		rect.Width += 50;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w + 50));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Property_Height()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		rect.Height += 50;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h + 50));
		});
	}

	[Test]
	public void Property_Left()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.That(rect.Left, Is.EqualTo(x));
	}

	[Test]
	public void Property_Right()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.That(rect.Right, Is.EqualTo(x + w));
	}

	[Test]
	public void Property_Top()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.That(rect.Top, Is.EqualTo(y));
	}

	[Test]
	public void Property_Bottom()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.That(rect.Bottom, Is.EqualTo(y + h));
	}

	[Test]
	public void Property_Location()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Location.X, Is.EqualTo(x));
			Assert.That(rect.Location.Y, Is.EqualTo(y));
		});
	}

	[Test]
	public void Property_Size()
	{
		int x = 5, y = 10, w = 100, h = 200;
		var rect = new Rectangle(x, y, w, h);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Size.Width, Is.EqualTo(w));
			Assert.That(rect.Size.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Method_FromLTRB()
	{
		int l = 5, t = 10, r = 105, b = 210;
		var rect = Rectangle.FromLTRB(l, t, r,  b);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(l));
			Assert.That(rect.Top, Is.EqualTo(t));
			Assert.That(rect.Right, Is.EqualTo(r));
			Assert.That(rect.Bottom, Is.EqualTo(b));
		});
	}

	[Test]
	public void Method_Contains()
	{
		var rect1 = new Rectangle(10, 20, 100, 50);
		var rect2 = new Rectangle(20, 30, 50, 20);
		Assert.That(rect1.Contains(rect2), Is.True);
	}

	[Test]
	public void Method_Contains_Point()
	{
		var rect = new Rectangle(10, 20, 100, 50);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Contains(50, 40), Is.True);
			Assert.That(rect.Contains(5, 15), Is.False);
		});
	}

	[Test]
	public void Method_IntersectWith()
	{
		var rect1 = new Rectangle(10, 20, 100, 50);
		var rect2 = new Rectangle(50, 30, 80, 70);
		Assert.That(rect1.IntersectsWith(rect2), Is.True);
	}

	[Test]
	public void Method_Union()
	{
		var rect1 = new Rectangle(10, 20, 100, 50);
		var rect2 = new Rectangle(50, 30, 80, 70);
		rect1.Union(rect2);
		Assert.Multiple(() =>
		{
			Assert.That(rect1.Left, Is.EqualTo(10));
			Assert.That(rect1.Top, Is.EqualTo(20));
			Assert.That(rect1.Right, Is.EqualTo(130));
			Assert.That(rect1.Bottom, Is.EqualTo(100));
		});
	}

	[Test]
	public void Method_Inflate()
	{
		var rect = new Rectangle(10, 20, 100, 50);
		rect.Inflate(5, 10);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(5));
			Assert.That(rect.Top, Is.EqualTo(10));
			Assert.That(rect.Right, Is.EqualTo(115));
			Assert.That(rect.Bottom, Is.EqualTo(80));
		});
	}

	[Test]
	public void Method_Inflate_Size()
	{
		var size = new Size(5, 18);
		var rect = new Rectangle(10, 20, 100, 50);
		rect.Inflate(size);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(5));
			Assert.That(rect.Top, Is.EqualTo(2));
			Assert.That(rect.Right, Is.EqualTo(115));
			Assert.That(rect.Bottom, Is.EqualTo(88));
		});
	}

	[Test]
	public void Method_Offset()
	{
		var rect = new Rectangle(10, 20, 100, 50);
		rect.Offset(5, 10);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(15));
			Assert.That(rect.Top, Is.EqualTo(30));
			Assert.That(rect.Right, Is.EqualTo(115));
			Assert.That(rect.Bottom, Is.EqualTo(80));
		});
	}

	[Test]
	public void Method_Offset_Point()
	{
		var point = new Point(5, 10);
		var rect = new Rectangle(10, 20, 100, 50);
		rect.Offset(point);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(15));
			Assert.That(rect.Top, Is.EqualTo(30));
			Assert.That(rect.Right, Is.EqualTo(115));
			Assert.That(rect.Bottom, Is.EqualTo(80));
		});
	}
}

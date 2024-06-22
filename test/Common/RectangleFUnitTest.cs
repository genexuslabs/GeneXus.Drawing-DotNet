namespace GeneXus.Drawing.Test;

internal class RectangleFUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Float()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
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
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var point = new PointF(x, y);
		var size = new SizeF(w, h);
		var rect = new RectangleF(point, size);
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
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var size = new SizeF(w, h);
		var rect = new RectangleF(x, y, size);
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
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var point = new PointF(x, y);
		var rect = new RectangleF(point, w, h);
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
		var rect1 = new RectangleF(5f, 10f, 100f, 200f);
		var rect2 = new RectangleF(5f, 10f, 100f, 200f);
		Assert.That(rect1 == rect2, Is.True);
	}

	[Test]
	public void Operator_Inequality()
	{
		var rect1 = new RectangleF(5f, 10f, 100f, 200f);
		var rect2 = new RectangleF(10f, 5f, 200f, 100f);
		Assert.That(rect1 != rect2, Is.True);
	}

	[Test]
	public void Property_X()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
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
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		rect.Y += 5f;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y + 5f));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Property_Width()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		rect.Width += 50;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w + 50f));
			Assert.That(rect.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Property_Height()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		rect.Height += 50f;
		Assert.Multiple(() =>
		{
			Assert.That(rect.X, Is.EqualTo(x));
			Assert.That(rect.Y, Is.EqualTo(y));
			Assert.That(rect.Width, Is.EqualTo(w));
			Assert.That(rect.Height, Is.EqualTo(h + 50f));
		});
	}

	[Test]
	public void Property_Left()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		Assert.That(rect.Left, Is.EqualTo(x));
	}

	[Test]
	public void Property_Right()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		Assert.That(rect.Right, Is.EqualTo(x + w));
	}

	[Test]
	public void Property_Top()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		Assert.That(rect.Top, Is.EqualTo(y));
	}

	[Test]
	public void Property_Bottom()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		Assert.That(rect.Bottom, Is.EqualTo(y + h));
	}

	[Test]
	public void Property_Location()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Location.X, Is.EqualTo(x));
			Assert.That(rect.Location.Y, Is.EqualTo(y));
		});
	}

	[Test]
	public void Property_Size()
	{
		float x = 5f, y = 10f, w = 100f, h = 200f;
		var rect = new RectangleF(x, y, w, h);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Size.Width, Is.EqualTo(w));
			Assert.That(rect.Size.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Method_FromLTRB()
	{
		float l = 5f, t = 10f, r = 105f, b = 210f;
		var rect = RectangleF.FromLTRB(l, t, r,  b);
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
		var rect1 = new RectangleF(10f, 20f, 100f, 50f);
		var rect2 = new RectangleF(20f, 30f, 50f, 20f);
		Assert.That(rect1.Contains(rect2), Is.True);
	}

	[Test]
	public void Method_Contains_Point()
	{
		var rect = new RectangleF(10f, 20f, 100f, 50f);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Contains(50f, 40f), Is.True);
			Assert.That(rect.Contains(5f, 15f), Is.False);
		});
	}

	[Test]
	public void Method_IntersectWith()
	{
		var rect1 = new RectangleF(10f, 20f, 100f, 50f);
		var rect2 = new RectangleF(50f, 30f, 80f, 70f);
		Assert.That(rect1.IntersectsWith(rect2), Is.True);
	}

	[Test]
	public void Method_Union()
	{
		var rect1 = new RectangleF(10f, 20f, 100f, 50f);
		var rect2 = new RectangleF(50f, 30f, 80f, 70f);
		rect1.Union(rect2);
		Assert.Multiple(() =>
		{
			Assert.That(rect1.Left, Is.EqualTo(10f));
			Assert.That(rect1.Top, Is.EqualTo(20f));
			Assert.That(rect1.Right, Is.EqualTo(130f));
			Assert.That(rect1.Bottom, Is.EqualTo(100f));
		});
	}

	[Test]
	public void Method_Inflate()
	{
		var rect = new RectangleF(10f, 20f, 100f, 50f);
		rect.Inflate(5f, 10f);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(5f));
			Assert.That(rect.Top, Is.EqualTo(10f));
			Assert.That(rect.Right, Is.EqualTo(115f));
			Assert.That(rect.Bottom, Is.EqualTo(80f));
		});
	}

	[Test]
	public void Method_Inflate_Size()
	{
		var size = new SizeF(5f, 18);
		var rect = new RectangleF(10f, 20f, 100f, 50f);
		rect.Inflate(size);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(5f));
			Assert.That(rect.Top, Is.EqualTo(2f));
			Assert.That(rect.Right, Is.EqualTo(115f));
			Assert.That(rect.Bottom, Is.EqualTo(88f));
		});
	}

	[Test]
	public void Method_Offset()
	{
		var rect = new RectangleF(10f, 20f, 100f, 50f);
		rect.Offset(5f, 10f);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(15f));
			Assert.That(rect.Top, Is.EqualTo(30f));
			Assert.That(rect.Right, Is.EqualTo(115f));
			Assert.That(rect.Bottom, Is.EqualTo(80f));
		});
	}

	[Test]
	public void Method_Offset_Point()
	{
		var point = new PointF(5f, 10f);
		var rect = new RectangleF(10f, 20f, 100f, 50f);
		rect.Offset(point);
		Assert.Multiple(() =>
		{
			Assert.That(rect.Left, Is.EqualTo(15f));
			Assert.That(rect.Top, Is.EqualTo(30f));
			Assert.That(rect.Right, Is.EqualTo(115f));
			Assert.That(rect.Bottom, Is.EqualTo(80f));
		});
	}
}

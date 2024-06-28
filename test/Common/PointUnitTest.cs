namespace GeneXus.Drawing.Test;

internal class PointUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_None()
	{
		var point = new Point();
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(0f));
			Assert.That(point.Y, Is.EqualTo(0f));
		});
	}

	[Test]
	public void Constructor_Properties()
	{
		const int x = 10, y = 20;
		Point point = new() { X = x, Y = y };
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(x));
			Assert.That(point.Y, Is.EqualTo(y));
		});
	}

	[Test]
	public void Constructor_Int()
	{
		const int x = 10, y = -20;
		Point point = new(x, y);
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(x));
			Assert.That(point.Y, Is.EqualTo(y));
		});
	}

	[Test]
	public void Constructor_Size()
	{
		Size size = new(30, 40);
		Point point = new(size);
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(size.Width));
			Assert.That(point.Y, Is.EqualTo(size.Height));
		});
	}

	[Test]
	public void Constructor_Int()
	{
		const int x = 100;
		const int y = 200;
		const int dw = y << 16 | x << 0;
		Point point = new(dw);
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(x));
			Assert.That(point.Y, Is.EqualTo(y));
		});
	}

	[Test]
	public void Operator_Equality()
	{
		Point point1 = new(10, 20);
		Point point2 = new(10, 20);
		Assert.That(point1 == point2, Is.True);
	}

	[Test]
	public void Operator_Inequality()
	{
		Point point1 = new(10, 20);
		Point point2 = new(20, 30);
		Assert.That(point1, Is.Not.EqualTo(point2));
	}

	[Test]
	public void Operator_Addition()
	{
		Point point = new(10, 20);
		Size size = new(30, 40);
		Point result = point + size;
		Assert.Multiple(() =>
		{
			Assert.That(result.X, Is.EqualTo(40));
			Assert.That(result.Y, Is.EqualTo(60));
		});
	}

	[Test]
	public void Operator_Subtraction()
	{
		Point point = new(50, 60);
		Size size = new(20, 30);
		Point result = point - size;
		Assert.Multiple(() =>
		{
			Assert.That(result.X, Is.EqualTo(30f));
			Assert.That(result.Y, Is.EqualTo(30f));
		});
	}

	[Test]
	public void Method_Equals()
	{
		Point point1 = new(10, 20);
		Point point2 = new(10, 20);
		Assert.That(point1.Equals(point2), Is.True);
	}

	[Test]
	public void Method_GetHashCode()
	{
		Point point1 = new(10, 20);
		Point point2 = new(10, 20);
		Assert.That(point2.GetHashCode(), Is.EqualTo(point1.GetHashCode()));
	}

	[Test]
	public void Method_Offset()
	{
		Point point = new(10, 20);
		point.Offset(5, -5);
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(15f));
			Assert.That(point.Y, Is.EqualTo(15f));
		});
	}

	[Test]
	public void Method_Offset_Point()
	{
		Point point = new(10, 20);
		Point offsetPoint = new(5, -5);
		point.Offset(offsetPoint);
		Assert.Multiple(() =>
		{
			Assert.That(point.X, Is.EqualTo(15f));
			Assert.That(point.Y, Is.EqualTo(15f));
		});
	}
}
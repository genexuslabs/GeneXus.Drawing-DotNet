namespace GeneXus.Drawing.Test;

internal class SizeUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_None()
	{
		var size = new Size();
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(0f));
			Assert.That(size.Height, Is.EqualTo(0f));
		});
	}

	[Test]
	public void Constructor_Properties()
	{
		const int w = 10;
		const int h = 20;
		Size size = new() { Width = w, Height = h };
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(w));
			Assert.That(size.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_Float()
	{
		const int w = 10;
		const int h = 20;
		Size size = new(w, h);
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(w));
			Assert.That(size.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_Point()
	{
		Point point = new(30, 40);
		Size size = new(point);
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(point.X));
			Assert.That(size.Height, Is.EqualTo(point.Y));
		});
	}

	[Test]
	public void Operator_Equality()
	{
		Size size1 = new(10, 20);
		Size size2 = new(10, 20);
		Assert.That(size1 == size2, Is.True);
	}

	[Test]
	public void Operator_Inequality()
	{
		Size size1 = new(10, 20);
		Size size2 = new(20, 30);
		Assert.That(size1 != size2, Is.True);
	}

	[Test]
	public void Operator_Addition()
	{
		Size size1 = new(10, 20);
		Size size2 = new(5, 10);
		Size result = size1 + size2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(15));
			Assert.That(result.Height, Is.EqualTo(30));
		});
	}

	[Test]
	public void Operator_Substraction()
	{
		Size size1 = new(10, 20);
		Size size2 = new(5, 10);
		Size result = size1 - size2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(5));
			Assert.That(result.Height, Is.EqualTo(10));
		});
	}

	[Test]
	public void Operator_Multiplication_Right()
	{
		Size size = new(10, 20);
		Size result = size * 2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(20));
			Assert.That(result.Height, Is.EqualTo(40));
		});
	}

	[Test]
	public void Operator_Multiplication_Left()
	{
		Size size = new(10, 20);
		Size result = 2 * size;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(20));
			Assert.That(result.Height, Is.EqualTo(40));
		});
	}

	[Test]
	public void Operator_Division()
	{
		Size size1 = new(10, 20);
		Size result = size1 / 2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(5));
			Assert.That(result.Height, Is.EqualTo(10));
		});
	}

	[Test]
	public void Method_Equals()
	{
		Size size1 = new(10, 20);
		Size size2 = new(10, 20);
		Assert.That(size1, Is.EqualTo(size2));
	}

	[Test]
	public void Method_GetHashCode()
	{
		Size size1 = new(10, 20);
		Size size2 = new(10, 20);
		Assert.That(size2.GetHashCode(), Is.EqualTo(size1.GetHashCode()));
	}
}

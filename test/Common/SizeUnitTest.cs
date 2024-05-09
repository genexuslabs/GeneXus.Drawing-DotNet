using GeneXus.Drawing.Common;

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
		float w = 10f, h = 20f;
		var size = new Size() { Width = w, Height = h };
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(w));
			Assert.That(size.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_Float()
	{
		float w = 10f, h = 20f;
		var size = new Size(w, h);
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(w));
			Assert.That(size.Height, Is.EqualTo(h));
		});
	}

	[Test]
	public void Constructor_Point()
	{
		var point = new Point(30f, 40f);
		var size = new Size(point);
		Assert.Multiple(() =>
		{
			Assert.That(size.Width, Is.EqualTo(point.X));
			Assert.That(size.Height, Is.EqualTo(point.Y));
		});
	}

	[Test]
	public void Operator_Equality()
	{
		var size1 = new Size(10f, 20f);
		var size2 = new Size(10f, 20f);
		Assert.That(size1 == size2, Is.True);
	}

	[Test]
	public void Operator_Inequality()
	{
		var size1 = new Size(10f, 20f);
		var size2 = new Size(20f, 30f);
		Assert.That(size1 != size2, Is.True);
	}

	[Test]
	public void Operator_Addition()
	{
		var size1 = new Size(10f, 20f);
		var size2 = new Size(5f, 10f);
		var result = size1 + size2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(15f));
			Assert.That(result.Height, Is.EqualTo(30f));
		});
	}

	[Test]
	public void Operator_Substraction()
	{
		var size1 = new Size(10f, 20f);
		var size2 = new Size(5f, 10f);
		var result = size1 - size2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(5f));
			Assert.That(result.Height, Is.EqualTo(10f));
		});
	}

	[Test]
	public void Operator_Multiplication_Right()
	{
		var size = new Size(10f, 20f);
		var result = size * 2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(20f));
			Assert.That(result.Height, Is.EqualTo(40f));
		});
	}

	[Test]
	public void Operator_Multiplication_Left()
	{
		var size = new Size(10f, 20f);
		var result = 2 * size;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(20f));
			Assert.That(result.Height, Is.EqualTo(40f));
		});
	}

	[Test]
	public void Operator_Division()
	{
		var size1 = new Size(10f, 20f);
		var result = size1 / 2;
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(5f));
			Assert.That(result.Height, Is.EqualTo(10f));
		});
	}

	[Test]
	public void Operator_Round()
	{
		var size1 = new Size(10.2f, 20.8f);
		var result = Size.Round(size1);
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(10f));
			Assert.That(result.Height, Is.EqualTo(21f));
		});
	}

	[Test]
	public void Operator_Truncate()
	{
		var size1 = new Size(10.9f, 20.1f);
		var result = Size.Truncate(size1);
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(10f));
			Assert.That(result.Height, Is.EqualTo(20f));
		});
	}

	[Test]
	public void Operator_Ceiling()
	{
		var size1 = new Size(10.2f, 20.6f);
		var result = Size.Ceiling(size1);
		Assert.Multiple(() =>
		{
			Assert.That(result.Width, Is.EqualTo(11f));
			Assert.That(result.Height, Is.EqualTo(21f));
		});
	}

	[Test]
	public void Method_Equals()
	{
		var size1 = new Size(10f, 20f);
		var size2 = new Size(10f, 20f);
		Assert.That(size1.Equals(size2), Is.True);
	}

	[Test]
	public void Method_GetHashCode()
	{
		var size1 = new Size(10f, 20f);
		var size2 = new Size(10f, 20f);
		Assert.That(size2.GetHashCode(), Is.EqualTo(size1.GetHashCode()));
	}
}

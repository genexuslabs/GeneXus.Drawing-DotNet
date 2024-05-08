using GeneXus.Drawing.Common;

namespace GeneXus.Drawing.Test
{
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
			float x = 10f, y = 20f;
			var point = new Point() { X = x, Y = y };
			Assert.Multiple(() =>
			{
				Assert.That(point.X, Is.EqualTo(x));
				Assert.That(point.Y, Is.EqualTo(y));
			});
		}

		[Test]
		public void Constructor_Float()
		{
			float x = 10f, y = -20f;
			var point = new Point(x, y);
			Assert.Multiple(() =>
			{
				Assert.That(point.X, Is.EqualTo(x));
				Assert.That(point.Y, Is.EqualTo(y));
			});
		}

		[Test]
		public void Constructor_Size()
		{
			var size = new Size(30f, 40f);
			var point = new Point(size);
			Assert.Multiple(() =>
			{
				Assert.That(point.X, Is.EqualTo(size.Width));
				Assert.That(point.Y, Is.EqualTo(size.Height));
			});
		}

		[Test]
		public void Constructor_Int()
		{
			int x = 100, y = 200;
			int dw = unchecked(y << 16) | unchecked(x << 0);
			var point = new Point(dw);
			Assert.Multiple(() =>
			{
				Assert.That(point.X, Is.EqualTo(x));
				Assert.That(point.Y, Is.EqualTo(y));
			});
		}

		[Test]
		public void Operator_Equality()
		{
			var point1 = new Point(10f, 20f);
			var point2 = new Point(10f, 20f);
			Assert.That(point1 == point2, Is.True);
		}

		[Test]
		public void Operator_Inequality()
		{
			var point1 = new Point(10f, 20f);
			var point2 = new Point(20f, 30f);
			Assert.That(point1 != point2, Is.True);
		}

		[Test]
		public void Operator_Addition()
		{
			var point = new Point(10f, 20f);
			var size = new Size(30f, 40f);
			var result = point + size;
			Assert.Multiple(() =>
			{
				Assert.That(result.X, Is.EqualTo(40f));
				Assert.That(result.Y, Is.EqualTo(60f));
			});
		}

		[Test]
		public void Operator_Subtraction()
		{
			var point = new Point(50f, 60f);
			var size = new Size(20f, 30f);
			var result = point - size;
			Assert.Multiple(() =>
			{
				Assert.That(result.X, Is.EqualTo(30f));
				Assert.That(result.Y, Is.EqualTo(30f));
			});
		}

		[Test]
		public void Method_Equals()
		{
			var point1 = new Point(10f, 20f);
			var point2 = new Point(10f, 20f);
			Assert.That(point1.Equals(point2), Is.True);
		}

		[Test]
		public void Method_GetHashCode()
		{
			var point1 = new Point(10f, 20f);
			var point2 = new Point(10f, 20f);
			Assert.That(point2.GetHashCode(), Is.EqualTo(point1.GetHashCode()));
		}

		[Test]
		public void Method_Offset()
		{
			var point = new Point(10f, 20f);
			point.Offset(5f, -5f);
			Assert.Multiple(() =>
			{
				Assert.That(point.X, Is.EqualTo(15f));
				Assert.That(point.Y, Is.EqualTo(15f));
			});
		}

		[Test]
		public void Method_Offset_Point()
		{
			var point = new Point(10f, 20f);
			var offsetPoint = new Point(5f, -5f);
			point.Offset(offsetPoint);
			Assert.Multiple(() =>
			{
				Assert.That(point.X, Is.EqualTo(15f));
				Assert.That(point.Y, Is.EqualTo(15f));
			});
		}

		[Test]
		public void Static_Method_Ceiling()
		{
			var point = new Point(10.4f, 20.6f);
			var result = Point.Ceiling(point);
			Assert.Multiple(() =>
			{
				Assert.That(result.X, Is.EqualTo(11f));
				Assert.That(result.Y, Is.EqualTo(21f));
			});
		}

		[Test]
		public void Static_Method_Truncate()
		{
			var point = new Point(10.9f, 20.6f);
			var result = Point.Truncate(point);
			Assert.Multiple(() =>
			{
				Assert.That(result.X, Is.EqualTo(10f));
				Assert.That(result.Y, Is.EqualTo(20f));
			});
		}

		[Test]
		public void Static_Method_Round()
		{
			var point = new Point(10.4f, 20.6f);
			var result = Point.Round(point);
			Assert.Multiple(() =>
			{
				Assert.That(result.X, Is.EqualTo(10f));
				Assert.That(result.Y, Is.EqualTo(21f));
			});
		}
	}
}
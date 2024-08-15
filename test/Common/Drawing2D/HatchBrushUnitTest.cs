using System.IO;
using GeneXus.Drawing.Drawing2D;

namespace GeneXus.Drawing.Test.Drawing2D;

internal class HatchBrushUnitTest
{
	[SetUp]
	public void Setup()
	{
	}
	
	[Test]
	[TestCase(HatchStyle.Horizontal)]
	[TestCase(HatchStyle.Vertical)]
	[TestCase(HatchStyle.ForwardDiagonal)]
	[TestCase(HatchStyle.BackwardDiagonal)]
	[TestCase(HatchStyle.Cross)]
	[TestCase(HatchStyle.DiagonalCross)]
	[TestCase(HatchStyle.Percent05)]
	[TestCase(HatchStyle.Percent10)]
	[TestCase(HatchStyle.Percent20)]
	[TestCase(HatchStyle.Percent25)]
	[TestCase(HatchStyle.Percent30)]
	[TestCase(HatchStyle.Percent40)]
	[TestCase(HatchStyle.Percent50)]
	[TestCase(HatchStyle.Percent60)]
	[TestCase(HatchStyle.Percent70)]
	[TestCase(HatchStyle.Percent75)]
	[TestCase(HatchStyle.Percent80)]
	[TestCase(HatchStyle.Percent90)]
	[TestCase(HatchStyle.LightDownwardDiagonal)]
	[TestCase(HatchStyle.LightUpwardDiagonal)]
	[TestCase(HatchStyle.DarkDownwardDiagonal)]
	[TestCase(HatchStyle.DarkUpwardDiagonal)]
	[TestCase(HatchStyle.WideDownwardDiagonal)]
	[TestCase(HatchStyle.WideUpwardDiagonal)]
	[TestCase(HatchStyle.LightVertical)]
	[TestCase(HatchStyle.LightHorizontal)]
	[TestCase(HatchStyle.NarrowVertical)]
	[TestCase(HatchStyle.NarrowHorizontal)]
	[TestCase(HatchStyle.DarkVertical)]
	[TestCase(HatchStyle.DarkHorizontal)]
	[TestCase(HatchStyle.DashedDownwardDiagonal)]
	[TestCase(HatchStyle.DashedUpwardDiagonal)]
	[TestCase(HatchStyle.DashedHorizontal)]
	[TestCase(HatchStyle.DashedVertical)]
	[TestCase(HatchStyle.SmallConfetti)]
	[TestCase(HatchStyle.LargeConfetti)]
	[TestCase(HatchStyle.ZigZag)]
	[TestCase(HatchStyle.Wave)]
	[TestCase(HatchStyle.DiagonalBrick)]
	[TestCase(HatchStyle.HorizontalBrick)]
	[TestCase(HatchStyle.Weave)]
	[TestCase(HatchStyle.Plaid)]
	[TestCase(HatchStyle.Divot)]
	[TestCase(HatchStyle.DottedGrid)]
	[TestCase(HatchStyle.DottedDiamond)]
	[TestCase(HatchStyle.Shingle)]
	[TestCase(HatchStyle.Trellis)]
	[TestCase(HatchStyle.Sphere)]
	[TestCase(HatchStyle.SmallGrid)]
	[TestCase(HatchStyle.SmallCheckerBoard)]
	[TestCase(HatchStyle.LargeCheckerBoard)]
	[TestCase(HatchStyle.OutlinedDiamond)]
	[TestCase(HatchStyle.SolidDiamond)]
	public void Constructor_StyleForeBack(HatchStyle style)
	{
		var back = Color.Red;
		var fore = Color.Blue;

		using var brush = new HatchBrush(style, fore, back);
		Assert.Multiple(() =>
		{
			Assert.That(brush.BackgroundColor, Is.EqualTo(back));
			Assert.That(brush.ForegroundColor, Is.EqualTo(fore));
			Assert.That(brush.HatchStyle, Is.EqualTo(style));

			string name = style switch
			{
				HatchStyle.Cross => "Cross", // defines LargeGrid and Max aliases
				HatchStyle.Horizontal => "Horizontal", // defines Min alias
				_ => style.ToString()
			};
			string path = Path.Combine("brush", "hatch", $"Style{name}.png");
			float similarity = Utils.CompareImage(path, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.9));
		});
	}

	[Test]
	public void Method_Clone()
	{
		using var brush1 = new HatchBrush(HatchStyle.Vertical, Color.Red, Color.Blue);
		using var brush2 = brush1.Clone() as HatchBrush;
		Assert.Multiple(() =>
		{
			Assert.That(brush2, Is.Not.Null);
			Assert.That(brush2, Is.Not.SameAs(brush1));
			Assert.That(brush2.HatchStyle, Is.EqualTo(brush1.HatchStyle));
			Assert.That(brush2.ForegroundColor, Is.EqualTo(brush1.ForegroundColor));
			Assert.That(brush2.BackgroundColor, Is.EqualTo(brush1.BackgroundColor));
		});
	}
}
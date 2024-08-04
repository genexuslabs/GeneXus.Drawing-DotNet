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
	[TestCase(HatchStyle.Horizontal, "style-horizontal.png")]
	[TestCase(HatchStyle.Vertical, "style-vertical.png")]
	[TestCase(HatchStyle.ForwardDiagonal, "style-forward-diagonal.png")]
	[TestCase(HatchStyle.BackwardDiagonal, "style-backward-diagonal.png")]
	[TestCase(HatchStyle.Cross, "style-cross.png")]
	[TestCase(HatchStyle.DiagonalCross, "style-diagonal-cross.png")]
	[TestCase(HatchStyle.Percent05, "style-percent05.png")]
	[TestCase(HatchStyle.Percent10, "style-percent10.png")]
	[TestCase(HatchStyle.Percent20, "style-percent20.png")]
	[TestCase(HatchStyle.Percent25, "style-percent25.png")]
	[TestCase(HatchStyle.Percent30, "style-percent30.png")]
	[TestCase(HatchStyle.Percent40, "style-percent40.png")]
	[TestCase(HatchStyle.Percent50, "style-percent50.png")]
	[TestCase(HatchStyle.Percent60, "style-percent60.png")]
	[TestCase(HatchStyle.Percent70, "style-percent70.png")]
	[TestCase(HatchStyle.Percent75, "style-percent75.png")]
	[TestCase(HatchStyle.Percent80, "style-percent80.png")]
	[TestCase(HatchStyle.Percent90, "style-percent90.png")]
	[TestCase(HatchStyle.LightDownwardDiagonal, "style-light-downward-diagonal.png")]
	[TestCase(HatchStyle.LightUpwardDiagonal, "style-light-upward-diagonal.png")]
	[TestCase(HatchStyle.DarkDownwardDiagonal, "style-dark-downward-diagonal.png")]
	[TestCase(HatchStyle.DarkUpwardDiagonal, "style-dark-upward-diagonal.png")]
	[TestCase(HatchStyle.WideDownwardDiagonal, "style-wide-downward-diagonal.png")]
	[TestCase(HatchStyle.WideUpwardDiagonal, "style-wide-upward-diagonal.png")]
	[TestCase(HatchStyle.LightVertical, "style-light-vertical.png")]
	[TestCase(HatchStyle.LightHorizontal, "style-light-horizontal.png")]
	[TestCase(HatchStyle.NarrowVertical, "style-narrow-vertical.png")]
	[TestCase(HatchStyle.NarrowHorizontal, "style-narrow-horizontal.png")]
	[TestCase(HatchStyle.DarkVertical, "style-dark-vertical.png")]
	[TestCase(HatchStyle.DarkHorizontal, "style-dark-horizontal.png")]
	[TestCase(HatchStyle.DashedDownwardDiagonal, "style-dashed-downward-diagonal.png")]
	[TestCase(HatchStyle.DashedUpwardDiagonal, "style-dashed-upward-diagonal.png")]
	[TestCase(HatchStyle.DashedHorizontal, "style-dashed-horizontal.png")]
	[TestCase(HatchStyle.DashedVertical, "style-dashed-vertical.png")]
	[TestCase(HatchStyle.SmallConfetti, "style-small-confetti.png")]
	[TestCase(HatchStyle.LargeConfetti, "style-large-confetti.png")]
	[TestCase(HatchStyle.ZigZag, "style-zigzag.png")]
	[TestCase(HatchStyle.Wave, "style-wave.png")]
	[TestCase(HatchStyle.DiagonalBrick, "style-diagonal-brick.png")]
	[TestCase(HatchStyle.HorizontalBrick, "style-horizontal-brick.png")]
	[TestCase(HatchStyle.Weave, "style-weave.png")]
	[TestCase(HatchStyle.Plaid, "style-plaid.png")]
	[TestCase(HatchStyle.Divot, "style-divot.png")]
	[TestCase(HatchStyle.DottedGrid, "style-dotted-grid.png")]
	[TestCase(HatchStyle.DottedDiamond, "style-dotted-diamond.png")]
	[TestCase(HatchStyle.Shingle, "style-shingle.png")]
	[TestCase(HatchStyle.Trellis, "style-trellis.png")]
	[TestCase(HatchStyle.Sphere, "style-sphere.png")]
	[TestCase(HatchStyle.SmallGrid, "style-small-grid.png")]
	[TestCase(HatchStyle.SmallCheckerBoard, "style-small-checker-board.png")]
	[TestCase(HatchStyle.LargeCheckerBoard, "style-large-checker-board.png")]
	[TestCase(HatchStyle.OutlinedDiamond, "style-outlined-diamond.png")]
	[TestCase(HatchStyle.SolidDiamond, "style-solid-diamond.png")]
	public void Constructor_StyleForeBack(HatchStyle style, string expected)
	{
		var back = Color.Red;
		var fore = Color.Blue;

		using var brush = new HatchBrush(style, fore, back);
		Assert.Multiple(() =>
		{
			Assert.That(brush.BackgroundColor, Is.EqualTo(back));
			Assert.That(brush.ForegroundColor, Is.EqualTo(fore));
			Assert.That(brush.HatchStyle, Is.EqualTo(style));

			string path = Path.Combine("brush", "hatch", expected);
			float similarity = Utils.CompareImage(path, brush, true);
			Assert.That(similarity, Is.GreaterThan(0.95));
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
namespace GeneXus.Drawing.Test;

internal class StringFormatUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Default()
	{
		var format = new StringFormat();
		Assert.Multiple(() => 
		{
			Assert.That(format.FormatFlags, Is.EqualTo((StringFormatFlags)0));
			Assert.That(format.Alignment, Is.EqualTo(StringAlignment.Near));
			Assert.That(format.LineAlignment, Is.EqualTo(StringAlignment.Near));
			Assert.That(format.Trimming, Is.EqualTo(StringTrimming.None));
			Assert.That(format.DigitSubstitutionMethod, Is.EqualTo(StringDigitSubstitute.None));
			Assert.That(format.DigitSubstitutionLanguage, Is.EqualTo(0));
		});
	}

	[Test]
	public void Constructor_FormatFlags()
	{
		var format = new StringFormat(StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft);
		Assert.Multiple(() =>
		{
			Assert.That(format.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical));
			Assert.That(format.FormatFlags.HasFlag(StringFormatFlags.DirectionRightToLeft));
			Assert.That(!format.FormatFlags.HasFlag(StringFormatFlags.NoClip));
			Assert.That(format.DigitSubstitutionLanguage, Is.EqualTo(0));
		});
	}

	[Test]
	public void Constructor_FormatFlagsAndLanguage()
	{
		var format = new StringFormat(StringFormatFlags.DirectionVertical | StringFormatFlags.DirectionRightToLeft, 1033);
		Assert.Multiple(() =>
		{
			Assert.That(format.FormatFlags.HasFlag(StringFormatFlags.DirectionVertical));
			Assert.That(format.FormatFlags.HasFlag(StringFormatFlags.DirectionRightToLeft));
			Assert.That(!format.FormatFlags.HasFlag(StringFormatFlags.NoClip));
			Assert.That(format.DigitSubstitutionLanguage, Is.EqualTo(1033));
		});
	}

	[Test]
	public void Method_GetSetDigitSubstitution()
	{
		var format = new StringFormat();
		
		format.SetDigitSubstitution(1033, StringDigitSubstitute.Traditional);
		Assert.Multiple(() =>
		{
			Assert.That(format.DigitSubstitutionLanguage, Is.EqualTo(1033));
			Assert.That(format.DigitSubstitutionMethod, Is.EqualTo(StringDigitSubstitute.Traditional));
		});
	}

	[Test]
	public void Method_GetSetTabStops()
	{
		var format = new StringFormat();

		float expTabOffset = 2.0f;
		float[] expTabStops = { 4.0f, 8.0f, 12.0f };
		format.SetTabStops(expTabOffset, expTabStops);

		float[] tabStops = format.GetTabStops(out float tabOffset);
		Assert.Multiple(() =>
		{
			Assert.That(tabOffset, Is.EqualTo(expTabOffset));
			Assert.That(tabStops, Is.EqualTo(expTabStops));
		});
	}	
}
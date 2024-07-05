using GeneXus.Drawing.Text;
using System;
using System.IO;

namespace GeneXus.Drawing.Test.Text;

internal class InstalledFontCollectionUnitTest
{
	[SetUp]
	public void Setup()
	{
	}

	[Test]
	public void Constructor_Default()
	{
		using var ifc = new InstalledFontCollection();
		Assert.That(ifc.Families, Is.Not.Empty);
	}
}

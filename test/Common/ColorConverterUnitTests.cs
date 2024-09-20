namespace GeneXus.Drawing.Test;

internal class ColorConverterTests
{
    private ColorConverter converter;

    [SetUp]
    public void SetUp()
    {
        converter = new ColorConverter();
    }

    [Test]
    public void Method_CanConvertFrom_String()
    {
        Assert.That(converter.CanConvertFrom(typeof(string)), Is.True);
    }

    [Test]
    public void Method_CanConvertFrom_Color()
    {
        Assert.That(converter.CanConvertFrom(typeof(Color)), Is.False);
    }

    [Test]
    public void Method_CanConvertTo_String()
    {
        Assert.That(converter.CanConvertTo(typeof(string)), Is.True);
    }

    [Test]
    public void Method_CanConvertTo_Int()
    {
        Assert.That(converter.CanConvertTo(typeof(int)), Is.False);
    }

    [Test]
    public void Method_ConvertFrom_ColorName()
    {
        var result = converter.ConvertFrom("Red");

        Assert.That(result, Is.EqualTo(Color.Red));
    }

    [Test]
    public void Method_ConvertFrom_HexString()
    {
        var result = converter.ConvertFrom("#FF0000");

        Assert.That(result, Is.EqualTo(Color.Red));
    }

    [Test]
    public void Method_ConvertFrom_Empty()
    {
        var result = converter.ConvertFrom("");

        Assert.That(result, Is.EqualTo(Color.Empty));
    }

    [Test]
    public void Method_ConvertTo_String_Named()
    {
        var result = converter.ConvertTo(Color.Blue, typeof(string));

        Assert.That(result, Is.EqualTo("Blue"));
    }

    [Test]
    public void Method_ConvertTo_String_Empty()
    {
        var result = converter.ConvertTo(Color.Empty, typeof(string));

        Assert.That(result, Is.EqualTo(string.Empty));
    }

    [Test]
    public void Method_ConvertFromInvariantString_Named()
    {
        var result = converter.ConvertFromInvariantString("Green");

        Assert.That(result, Is.EqualTo(Color.Green));
    }

    [Test]
    public void Method_ConvertFromInvariantString_Hex()
    {
        var result = converter.ConvertFromInvariantString("#00FF00");

        Assert.That(result, Is.EqualTo(Color.Lime));
    }

    [Test]
    public void Method_ConvertFromInvariantString_Empty()
    {
        var result = converter.ConvertFromInvariantString("");

        Assert.That(result, Is.EqualTo(Color.Empty));
    }
}
namespace GeneXus.Drawing.Drawing2D;

public sealed class ColorBlend
{
	/// <summary>
	///  Initializes a new instance of the <see cref='ColorBlend'/> class with the specified number of colors and positions.
	/// </summary>
	public ColorBlend(int count = 1)
	{
		Colors = new Color[count];
		Positions = new float[count];
	}

	#region Properties

	/// <summary>
	///  Gets or sets an array of colors that represents the colors to use at corresponding positions along a gradient.
	/// </summary>
	public Color[] Colors { get; set; }

	/// <summary>
	///  Gets or sets the positions along a gradient line.
	/// </summary>
	public float[] Positions { get; set; }

	#endregion
}

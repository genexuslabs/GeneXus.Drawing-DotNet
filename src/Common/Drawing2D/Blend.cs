namespace GeneXus.Drawing.Drawing2D;

public sealed class Blend
{

	/// <summary>
	///  Initializes a new instance of the <see cref='Blend'/> class with the specified number of factors and positions.
	/// </summary>
	public Blend(int count = 1)
	{
		Factors = new float[count];
		Positions = new float[count];
	}

	#region Properties

	/// <summary>
	///  Gets or sets an array of blend factors for the gradient.
	/// </summary>
	public float[] Factors { get; set; }

	/// <summary>
	///  Gets or sets an array of blend positions for the gradient.
	/// </summary>
	public float[] Positions { get; set; }

	#endregion
}

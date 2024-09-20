namespace GeneXus.Drawing.Drawing2D;

public sealed class PathData
{

	/// <summary>
	///  Initializes a new instance of the <see cref="PathData"> class.
	/// </summary>
	public PathData() { }


	#region Properties

	/// <summary>
	///  Gets or sets an array of <see cref="PointF"> structures that represents the points through which the path is constructed.
	/// </summary>
	public PointF[] Points { get; set; }

	/// <summary>
	///  Gets or sets the types of the corresponding points in the path.
	/// </summary>
	public byte[] Types { get; set; }

	#endregion
}
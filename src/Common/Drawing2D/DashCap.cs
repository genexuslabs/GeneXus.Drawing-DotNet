namespace GeneXus.Drawing.Drawing2D;

/// <summary>
/// Specifies the available dash cap styles with which a <see cref='Pen'/> can end a line.
/// </summary>
public enum DashCap
{
	/// <summary>
	///  Specifies a square cap that squares off both ends of each dash.
	/// </summary>
	Flat = 0,

	/// <summary>
	///  Specifies a circular cap that rounds off both ends of each dash.
	/// </summary>
	Round = 2,

	/// <summary>
	///  Specifies a triangular cap that points both ends of each dash.
	/// </summary>
	Triangle = 3
}

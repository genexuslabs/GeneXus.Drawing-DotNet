namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies how the interior of a closed path is filled.
/// </summary>
public enum FillMode
{
	/// <summary>
	///  Specifies the alternate fill mode.
	/// </summary>
	Alternate = 0, // Odd-even fill rule

	/// <summary>
	///  Specifies the winding fill mode.
	/// </summary>
	Winding = 1, // Non-zero winding fill rule
}

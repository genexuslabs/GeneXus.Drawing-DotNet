namespace GeneXus.Drawing.Drawing2D;

/// <summary>
///  Specifies the available cap styles with which a <see cref='Pen'/> object can end a line.
/// </summary>
public enum LineCap
{
	Flat = 0,
	Square = 1,
	Round = 2,
	Triangle = 3,
	NoAnchor = 16,		// corresponds to flat cap
	SquareAnchor = 17,	// corresponds to square cap
	RoundAnchor = 18,	// corresponds to round cap
	DiamondAnchor = 19,	// corresponds to triangle cap
	ArrowAnchor = 20,	// no correspondence
	Custom = 255,		// custom cap
	AnchorMask = 40		// mask to check for anchor or not.
}

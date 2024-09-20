namespace GeneXus.Drawing;

/// <summary>
/// Specifies the Copy Pixel (ROP) operation.
/// </summary>
public enum CopyPixelOperation
{
	/// <summary>
	///  Fills the Destination Rectangle using the color associated with the index 0 in the physical palette. 
	///  (This color is black for the default physical palette.)
	/// </summary>
	Blackness = 0x00000042,

	/// <summary>
	///  Includes any windows that are layered on top.
	/// </summary>
	CaptureBlt = 0x40000000,

	/// <summary>
	///  The destination area is inverted.
	/// </summary>
	DestinationInvert = 0x00550009,

	/// <summary>
	///  The colors of the source area are merged with the colors of the selected brush of the destination device 
	///  context using the Boolean AND operator.
	/// </summary>
	MergeCopy = 0x00C000CA,

	/// <summary>
	///  The colors of the inverted source area are merged with the colors of the destination area by using the Boolean OR operator.
	/// </summary>
	MergePaint = 0x00BB0226,

	/// <summary>
	///  The bitmap is not mirrored.
	/// </summary>
	NoMirrorBitmap = unchecked((int)0x80000000),

	/// <summary>
	///  The inverted source area is copied to the destination.
	/// </summary>
	NotSourceCopy = 0x00330008,

	/// <summary>
	///  The source and destination colors are combined using the Boolean OR operator, and then the resultant color is inverted.
	/// </summary>
	NotSourceErase = 0x001100A6,

	/// <summary>
	///  The brush currently selected in the destination device context is copied to the destination bitmap.
	/// </summary>
	PatCopy = 0x00F00021,

	/// <summary>
	///  The colors of the brush currently selected in the destination device context are combined with the colors of 
	///  the destination area using the Boolean XOR operator.
	/// </summary>
	PatInvert = 0x005A0049,

	/// <summary>
	///  The colors of the brush currently selected in the destination device context are combined with the colors of 
	///  the inverted source area using the Boolean OR operator. The result of this operation is combined with the colors of the destination area using the Boolean OR operator.
	/// </summary>
	PatPaint = 0x00FB0A09,

	/// <summary>
	///  The colors of the source and destination areas are combined using the Boolean AND operator.
	/// </summary>
	SourceAnd = 0x008800C6,

	/// <summary>
	///  The source area is copied directly to the destination area.
	/// </summary>
	SourceCopy = 0x00CC0020,

	/// <summary>
	///  The inverted colors of the destination area are combined with the colors of the source area using the Boolean AND operator.
	/// </summary>
	SourceErase = 0x00440328,

	/// <summary>
	///  The colors of the source and destination areas are combined using the Boolean XOR operator.
	/// </summary>
	SourceInvert = 0x00660046,

	/// <summary>
	///  The colors of the source and destination areas are combined using the Boolean OR operator.
	/// </summary>
	SourcePaint = 0x00EE0086,

	/// <summary>
	///  Fills the destination area using the color associated with index 1 in the physical palette. 
	///  (This color is white for the default physical palette.)
	/// </summary>
	Whiteness = 0x00FF0062,
}
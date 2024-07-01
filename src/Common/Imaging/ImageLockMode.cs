namespace GeneXus.Drawing.Imaging;

/// <summary>
///  Indicates the access mode for an <see cref='Image'/>.
/// </summary>
public enum ImageLockMode
{
	/// <summary>
	///  Specifies the image is read-only.
	/// </summary>
	ReadOnly = 1,

	/// <summary>
	///  Specifies the image is write-only.
	/// </summary>
	WriteOnly = 2,

	/// <summary>
	///  Specifies the image is read-write.
	/// </summary>
	ReadWrite = ReadOnly | WriteOnly,

	/// <summary>
	///  Indicates the image resides in a user input buffer, to which the user controls access.
	/// </summary>
	UserInputBuffer = 4
}

namespace GeneXus.Drawing;

/// <summary>
///  Specifies style information applied to String Digit Substitute.
/// </summary>
public enum StringDigitSubstitute
{
	/// <summary>
	///  Specifies a user-defined substitution scheme.
	/// </summary>
	User = 0,

	/// <summary>
	///  Specifies to disable substitutions.
	/// </summary>
	None = 1,

	/// <summary>
	///  Specifies substitution digits that correspond with the official national language of the user's locale.
	/// </summary>
	National = 2,

	/// <summary>
	///  Specifies substitution digits that correspond with the user's native script or language, which may be 
	///  different from the official national language of the user's locale.
	/// </summary>
	Traditional = 3
}

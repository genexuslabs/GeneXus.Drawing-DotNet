using System;

namespace GeneXus.Drawing;

public struct CharacterRange : IEquatable<CharacterRange>
{
	public CharacterRange(int first, int length)
	{
		First = first;
		Length = length;
	}


	#region Operators

	/// <summary>
	///  Compares two <see cref='CharacterRange'/> objects. Gets a value indicating 
	///  whether the <see cref='First'/> and <see cref='Lenght'/> values of the 
	///  two <see cref='CharacterRange'/> objects are equal.
	/// </summary>
	public static bool operator ==(CharacterRange cr1, CharacterRange cr2) => cr1.Equals(cr2);

	/// <summary>
	///  Compares two <see cref='CharacterRange'/> objects. Gets a value indicating 
	///  whether the <see cref='First'/> and <see cref='Lenght'/> values of the 
	///  two <see cref='CharacterRange'/> objects are not equal.
	/// </summary>
	public static bool operator !=(CharacterRange cr1, CharacterRange cr2) => !cr1.Equals(cr2);

	#endregion


	#region IEqualitable

	/// <summary>
	///  Indicates whether the current instance is equal to another instance of the same type.
	/// </summary>
	public readonly bool Equals(CharacterRange other)
		=> First == other.First && Length == other.Length;

	/// <summary>
	///  Gets a value indicating whether this object is equivalent to the specified object.
	/// </summary>
	public override readonly bool Equals(object obj) 
		=> obj is CharacterRange other && Equals(other);

	/// <summary>
	///  Returns the hash code for this instance.
	/// </summary>
	public override readonly int GetHashCode()
        => Combine(First, Length);

	#endregion


	#region Properties

	/// <summary>
	///  Gets or sets the position in the string of the first character of this <see cref='CharacterRange'/>.
	/// </summary>
	public int First { get; set; }

	/// <summary>
	///  Gets or sets the number of positions in this <see cref='CharacterRange'/>.
	/// </summary>
	public int Length { get; set; }

	#endregion


    #region Utilities

    private const uint PRIME1 = 2654435761U, PRIME2 = 2246822519U;

    private static int Combine(params object[] objects)
    {
        uint hash = PRIME1;
        foreach (var obj in objects)
            hash = hash * PRIME2 + (uint)obj.GetHashCode();
        return Convert.ToInt32(hash);
    }

    #endregion
}


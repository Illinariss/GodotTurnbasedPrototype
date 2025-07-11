public class BasicAbility
{
    public string Name { get; set; }
    public AbilityType Type { get; set; }
    public CharacterStat Scalestat { get; set; }
    public float Faktor { get; set; }
    public int ResourceCost { get; set; }
    /// <summary>
    /// Some abilities do not require a manual target selection.
    /// </summary>
    public bool RequiresTarget { get; set; } = true;

    /// <summary>
    /// Calculates the damage this ability deals when used by the given attacker.
    /// The base implementation simply returns the scaled stat multiplied by
    /// <see cref="Faktor"/>.
    /// </summary>
    /// <param name="attacker">The character using the ability.</param>
    /// <returns>The damage amount.</returns>
    public virtual float CalculateDamage(CharacterData attacker)
    {
        return Faktor * attacker.GetStat(Scalestat, attacker.Turn);
    }
}

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
}
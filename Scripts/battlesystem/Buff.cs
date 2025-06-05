public class Buff
{
    public CharacterStat TargetStat { get; }
    public float Factor { get; }
    public int Value { get; }
    public int StartTurn { get; }
    public int EndTurn { get; }
    public string Icon { get; }

    public Buff(CharacterStat stat, float factor, int value, int startTurn, int endTurn, string icon = "")
    {
        TargetStat = stat;
        Factor = factor;
        Value = value;
        StartTurn = startTurn;
        EndTurn = endTurn;
        Icon = icon;
    }

    public bool IsActive(int round) => round >= StartTurn && round < EndTurn;
}

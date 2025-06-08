public interface ICombatAI
{
    void Initialize(CharacterData self);
    BattleAction DecideNextAction(BattleContext context);
}

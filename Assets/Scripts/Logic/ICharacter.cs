/// <summary>
/// Interface representing a character in the game, which can be a player or an enemy.
/// </summary>
public interface ICharacter: IIdentifiable
{
    int Health { get; set;}
    void Die();
}

/// <summary>
/// Interface representing an identifiable entity in the game, which has a unique ID.
/// </summary>
public interface IIdentifiable
{
    int ID { get; }
}
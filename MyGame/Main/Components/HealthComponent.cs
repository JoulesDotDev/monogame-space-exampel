using MyGame.Main.Managers;
using MyGame.Main.Messages;

namespace MyGame.Main.Components;

public class HealthComponent(int maxHealth, IdentifierComponent identifierComponent)
{
    public int MaxHealth { get; set; } = maxHealth;
    private int CurrentHealth { get; set; } = maxHealth;

    public void TakeDamage(int damage)
    {
        if (!IsAlive)
        {
            return;
        }

        CurrentHealth -= damage;

        if (CurrentHealth <= 0)
        {
            MessageManager.SendMessage(new DeathMessage(), identifierComponent.Id);
        }
    }

    public bool IsAlive => CurrentHealth > 0;
}
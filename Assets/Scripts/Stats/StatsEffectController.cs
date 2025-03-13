using UnityEngine;
using TMPro;
using static CardDataController;

public class StatsEffectController : MonoBehaviour
{
    public static StatsEffectController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void TriggerStatEffects(Stats oldStats, Stats newStats, Player targetPlayer)
    {
        var uiElements = StatsUIController.Instance.GetUIElements(targetPlayer);

        TriggerStatChange(oldStats.quarries, newStats.quarries, uiElements[Constants.Generators.Quarries], Constants.Sounds.GeneratorUp, Constants.Sounds.GeneratorDown);
        TriggerStatChange(oldStats.bricks, newStats.bricks, uiElements[Constants.Resources.Bricks], Constants.Sounds.ResourceUp, Constants.Sounds.ResourceDown);
        TriggerStatChange(oldStats.magic, newStats.magic, uiElements[Constants.Generators.Magic], Constants.Sounds.GeneratorUp, Constants.Sounds.GeneratorDown);
        TriggerStatChange(oldStats.gems, newStats.gems, uiElements[Constants.Resources.Gems], Constants.Sounds.ResourceUp, Constants.Sounds.ResourceDown);
        TriggerStatChange(oldStats.dungeons, newStats.dungeons, uiElements[Constants.Generators.Dungeons], Constants.Sounds.GeneratorUp, Constants.Sounds.GeneratorDown);
        TriggerStatChange(oldStats.recruits, newStats.recruits, uiElements[Constants.Resources.Recruits], Constants.Sounds.ResourceUp, Constants.Sounds.ResourceDown);
        TriggerStatChange(oldStats.wall, newStats.wall, uiElements[Constants.Structures.Wall], Constants.Sounds.WallUp, Constants.Sounds.Damage);
        TriggerStatChange(oldStats.tower, newStats.tower, uiElements[Constants.Structures.Tower], Constants.Sounds.TowerUp, Constants.Sounds.Damage);
    }

    void TriggerStatChange(int oldValue, int newValue, TextMeshProUGUI uiElement, string soundUp, string soundDown)
    {
        if (oldValue < newValue)
        {
            AudioManager.Instance.PlayGameplaySound(soundUp);
            ParticleEffectsController.Instance.PlayEffect(uiElement.transform, newValue - oldValue, Color.green);
        }
        else if (oldValue > newValue)
        {
            AudioManager.Instance.PlayGameplaySound(soundDown);
            ParticleEffectsController.Instance.PlayEffect(uiElement.transform, oldValue - newValue, Color.red);
        }
    }
}

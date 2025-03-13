using UnityEngine;

public class ParticleEffectsController : MonoBehaviour
{
    [SerializeField] ParticleSystem _particlesPrefab;

    public static ParticleEffectsController Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PlayEffect(Transform uiElement, int amount, Color color)
    {
        if (_particlesPrefab == null || uiElement == null)
        {
            Debug.LogError("Prefab or UI Element is null.");
            return;
        }

        Vector3 worldPosition = uiElement.position + new Vector3(-0.5f, 0.5f, 0);
        ParticleSystem particles = Instantiate(_particlesPrefab, worldPosition, Quaternion.identity, transform);

        ConfigureParticleSystem(particles, amount, color);
    }

    void ConfigureParticleSystem(ParticleSystem particles, int amount, Color color)
    {
        int maxParticles = amount * 10; // 10 particles for one unit

        var mainModule = particles.main;
        mainModule.maxParticles = maxParticles; 
        mainModule.startColor = color;
        mainModule.startSpeed = 2;
        mainModule.gravityModifier = 0.1f;
        mainModule.startLifetime = new ParticleSystem.MinMaxCurve(1, 3);

        var shapeModule = particles.shape;
        shapeModule.radius = amount / 10.0f;

        particles.Emit(maxParticles);
        Destroy(particles.gameObject, mainModule.duration);
    }
}

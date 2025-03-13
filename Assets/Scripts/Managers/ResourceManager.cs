using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    // Resources are organized all in a single folder since Unity cannot handle subdirectories with using Resources.LoadAll()
    // ...and System.IO has troubles in WebGL build
    public static ResourceManager Instance { get; private set; }
    private Dictionary<string, Sprite> _cache = new Dictionary<string, Sprite>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void PreloadResources(string path)
    {
        Sprite[] sprites = Resources.LoadAll<Sprite>(path);
        foreach (var sprite in sprites)
        {
            if (!_cache.ContainsKey(sprite.name))
            {
                _cache[sprite.name] = sprite;
            }
        }        
    }

    public Sprite GetSprite(string name)
    {
        if (_cache.TryGetValue(name, out var sprite))
        {
            return sprite;
        }
        Debug.LogWarning($"Sprite not found: {name}");
        return null;
    }
}

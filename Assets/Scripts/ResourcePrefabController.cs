using System.Collections.Generic;
using UnityEngine;

public class ResourcePrefabController: MonoBehaviour
{
    static List<ResourcePrefabController> _resourcePrefabInstances = new List<ResourcePrefabController>();
    
    [SerializeField] GameObject _generatorName;

    void Awake()
    {
        _resourcePrefabInstances.Add(this);
        _generatorName.SetActive(false);
    }

    void OnDestroy()
    {
        _resourcePrefabInstances.Remove(this);
    }

    public static void ToggleGeneratorNames(bool isOn)
    {
        foreach (ResourcePrefabController instance in _resourcePrefabInstances)
        {
            instance.ToggleGeneratorName(isOn);
        }
    }
        
    public void ToggleGeneratorName(bool isOn)
    {
        _generatorName.SetActive(isOn);
    }
}

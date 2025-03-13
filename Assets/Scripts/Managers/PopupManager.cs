using System;
using System.Collections.Generic;
using UnityEngine;

public class PopupManager : MonoBehaviour
{
    public static PopupManager Instance { get; private set; }

    public PopupType? ActivePopup;

    Dictionary<PopupType?, GameObject> _popups = new Dictionary<PopupType?, GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    public void RegisterPopup(PopupType type, GameObject popup)
    {        
        if (!_popups.ContainsKey(type))
        {
            _popups[type] = popup;
        }
        else
        {
            Debug.LogWarning($"Popup of type {type} is already registered.");
        }
    }

    public void OpenPopup(PopupType type)
    {
        if (_popups.TryGetValue(type, out GameObject popup))
        {
            SetPopupActive(type, true);            
        }
        else
        {
            Debug.LogError($"Popup of type {type} is not registered.");
        }
    }

    public void ClosePopup(PopupType type)
    {
        if (_popups.TryGetValue(type, out GameObject popup))
        {
            SetPopupActive(type, false);
        }
        else
        {
            Debug.LogError($"Popup of type {type} is not registered.");
        }
    }

    public void SetPopupActive(PopupType? type, bool isActive)
    {
        if (_popups.TryGetValue(type, out GameObject popup))
        {
            popup.SetActive(isActive);
            ActivePopup = isActive ? type : null;
        }
        else
        {
            Debug.LogError($"Popup of type {type} is not registered.");
        }
    }
}

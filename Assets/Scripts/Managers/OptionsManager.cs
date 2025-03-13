using UnityEngine;
using UnityEngine.UI;

public class OptionsManager : MonoBehaviour
{
    public static OptionsManager Instance;
    public Options options = new Options();

    [SerializeField] Toggle _lowFPS;
    [SerializeField] Toggle _originalImages;
    [SerializeField] Toggle _enableSound;
    [SerializeField] Toggle _enableMusic;
    [SerializeField] Toggle _showGeneratorIcons;
    [SerializeField] Toggle _showGeneratorNames;
    [SerializeField] Toggle _showNotEnoughResources;
    [SerializeField] Slider _soundVolume;
    [SerializeField] Slider _musicVolume;

    GameManager _gameManager;
    AudioManager _audioManager;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;        

        ApplyDefaultOptions();
        InitializeListeners();
    }

    void InitializeListeners()
    {
        _lowFPS.onValueChanged.AddListener(onLowFPSValueChanged);
        _originalImages.onValueChanged.AddListener(onOriginalImagesValueChanged);
        _enableSound.onValueChanged.AddListener(onEnableSoundValueChanged);
        _enableMusic.onValueChanged.AddListener(onEnableMusicValueChanged);
        _soundVolume.onValueChanged.AddListener(onSoundVolumeChanged);
        _musicVolume.onValueChanged.AddListener(onMusicVolumeChanged);
        _showGeneratorIcons.onValueChanged.AddListener(onShowGeneratorIconsValueChanged);
        _showGeneratorNames.onValueChanged.AddListener(onShowGeneratorNamesValueChanged);
        _showNotEnoughResources.onValueChanged.AddListener(onShowNotEnoughResourcesValueChanged);
    }

    void ApplyDefaultOptions()
    {
        options.lowFPS = _lowFPS.isOn;
        options.originalImages = _originalImages.isOn;
        options.enableSound = _enableSound.isOn;
        options.enableMusic = _enableMusic.isOn;
        options.showGeneratorIcons = _showGeneratorIcons.isOn;
        options.showGeneratorNames = _showGeneratorNames.isOn;
        options.showNotEnoughResources = _showNotEnoughResources.isOn;
        options.soundVolume = 100;
        options.musicVolume = 60;
    }    

    void onLowFPSValueChanged(bool isOn)
    {
        options.lowFPS = isOn;

        _gameManager.SetLowFPS(isOn);

        PlayClickSound();
    }

    void onOriginalImagesValueChanged(bool isOn)
    {
        options.originalImages = isOn;
        
        CardUIController.ToggleOriginalImages(isOn);

        PlayClickSound();
    }

    void onEnableSoundValueChanged(bool isOn)
    {
        options.enableSound = isOn;
        _soundVolume.interactable = isOn;

        _audioManager.ToggleSound(isOn);

        PlayClickSound();
    }

    void onEnableMusicValueChanged(bool isOn)
    {
        options.enableMusic = isOn;
        _musicVolume.interactable = isOn;

        _audioManager.ToggleMusic(isOn);

        PlayClickSound();
    }

    void onShowGeneratorIconsValueChanged(bool isOn)
    {                
        options.showGeneratorIcons = isOn;

        CardUIController.ToggleIcons(isOn);

        PlayClickSound();
    }

    void onShowGeneratorNamesValueChanged(bool isOn)
    {
        options.showGeneratorNames = isOn;

        ResourcePrefabController.ToggleGeneratorNames(isOn);

        PlayClickSound();
    }

    void onShowNotEnoughResourcesValueChanged(bool isOn)
    {
        options.showNotEnoughResources = isOn;

        CardUIController.ToggleNotEnoughResourcesInPlayerCards();

        PlayClickSound();
    }

    void onSoundVolumeChanged(float value)
    {
        options.soundVolume = value;

        _audioManager.SetSoundVolume(value);
    }

    void onMusicVolumeChanged(float value)
    {
        options.musicVolume = value;

        _audioManager.SetMusicVolume(value);
    }

    void PlayClickSound()
    {
        _audioManager.PlayOptionsSound(Constants.Sounds.Click);
    }    
}

[System.Serializable]
public class Options
{
    public bool lowFPS;
    public bool originalImages;
    public bool enableSound;
    public bool enableMusic;
    public bool showGeneratorIcons;
    public bool showGeneratorNames;
    public bool showNotEnoughResources;
    public float soundVolume;
    public float musicVolume;
}
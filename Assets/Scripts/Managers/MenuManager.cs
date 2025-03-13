using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{    
    public static MenuManager Instance { get; private set; }

    [SerializeField] GameObject _menu;
    [SerializeField] GameObject _pageOptions;
    [SerializeField] GameObject _pageHowToPlay;
    [SerializeField] GameObject _pageAbout;
    [SerializeField] GameObject _bookBackground;

    [SerializeField] Button _buttonMenu;
    [SerializeField] Button _buttonNewGame;
    [SerializeField] Button _buttonOptions;
    [SerializeField] Button _buttonHowToPlay;
    [SerializeField] Button _buttonAbout;
    [SerializeField] Button _buttonExitGame;
    [SerializeField] Button _buttonReturn;

    CanvasGroup _menuCanvasGroup;
    List<GameObject> _pages;

    GameManager _gameManager;
    AudioManager _audioManager;
    PopupManager _popupManager;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    void Start()
    {
        _gameManager = GameManager.Instance;
        _audioManager = AudioManager.Instance;
        _popupManager = PopupManager.Instance;
        _menuCanvasGroup = _menu.GetComponent<CanvasGroup>();                        

        InitializeListeners();
        InitializeMenu();

        _pages = new List<GameObject>
        {
            _pageOptions,
            _pageHowToPlay,
            _pageAbout
        };
    }

    void Update()
    {
        HandleInput();
    }

    public IEnumerator FadeInCanvas()
    {
        if (_menuCanvasGroup == null) yield break;

        yield return new WaitForSecondsRealtime(Constants.Durations.MenuFadeInDelay);

        float initialAlpha = _menuCanvasGroup.alpha;
        float targetAlpha = 1f;
        float elapsed = 0f;

        // Previous implementation works in Windows build but not in WebGL build:
        //while (elapsed < 1f)
        //{
        //    elapsed += Time.deltaTime;
        //    float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / 1f);
        //    menuCanvasGroup.alpha = newAlpha;
        //    yield return null;
        //}

        float startTime = Time.unscaledTime; // Use unscaled time to avoid issues with timeScale

        while (elapsed < Constants.Durations.MenuFadeIn)
        {
            elapsed = Time.unscaledTime - startTime;
            float newAlpha = Mathf.Lerp(initialAlpha, targetAlpha, elapsed / Constants.Durations.MenuFadeIn);
            _menuCanvasGroup.alpha = newAlpha;
            yield return null;
        }
        
        _menuCanvasGroup.alpha = targetAlpha; // Ensure the final alpha is set
        _menuCanvasGroup.interactable = true;

        OpenMenu();        
    }

    public void LaunchNewGame()
    {
        _gameManager.StartNewGame();
        _buttonReturn.gameObject.SetActive(true);
    }

    public void CloseMenu()
    {
        _audioManager.PauseGameplaySounds(false);
        _audioManager.PlayOptionsSound(Constants.Sounds.BookClose);
        _menu.SetActive(false);
        CloseAllPages();
        Time.timeScale = 1;
    }

    void InitializeListeners()
    {
        _buttonMenu.onClick.AddListener(OnButtonMenuClick);
        _buttonNewGame.onClick.AddListener(OnButtonNewGameClick);
        _buttonOptions.onClick.AddListener(OnButtonOptionsClick);
        _buttonHowToPlay.onClick.AddListener(OnButtonHowToPlayClick);
        _buttonAbout.onClick.AddListener(OnButtonAboutClick);
        _buttonExitGame.onClick.AddListener(OnButtonExitGameClick);
        _buttonReturn.onClick.AddListener(OnButtonReturnClick);
    }

    void InitializeMenu()
    {
        _menu.SetActive(true);
        
        _pageOptions.SetActive(false);
        _pageHowToPlay.SetActive(false);
        _pageAbout.SetActive(false);
        _bookBackground.SetActive(true);        

        _menuCanvasGroup.alpha = 0;
        _menuCanvasGroup.interactable = false;
    }    
    
    void OnButtonMenuClick()
    {
        OpenMenu();
    }

    void OnButtonNewGameClick()
    {
        if (_gameManager.GameStarted)
        {
            _popupManager.OpenPopup(PopupType.NewGame);            
        } else
        {
            CloseMenu();
            LaunchNewGame();
        }
            
    }    

    void OnButtonOptionsClick()
    {
        OpenPage(_pageOptions);
    }

    void OnButtonHowToPlayClick()
    {
        OpenPage(_pageHowToPlay);
    }

    void OnButtonAboutClick()
    {
        OpenPage(_pageAbout);
    }

    void OnButtonExitGameClick()
    {
        if (_gameManager.GameStarted)
        {
            _popupManager.OpenPopup(PopupType.ExitGame);            
        }
        else
        {
            _gameManager.ExitGame();
        }       
    }

    void OnButtonReturnClick()
    {
        CloseMenu();
    }    

    void OpenPage(GameObject page)
    {
        CloseAllPages();

        page.SetActive(true);
        _bookBackground.SetActive(false);

        _audioManager.PlayGameplaySound(Constants.Sounds.BookOpen);
    }

    void CloseAllPages()
    {
        foreach (var page in _pages)
        {
            page.SetActive(false);
        }

        _bookBackground.SetActive(true);
    }

    void ToggleMenu()
    {
        if (_menu.activeSelf)
        {
            CloseMenu();
        } else
        {
            OpenMenu();
        }
    }

    void OpenMenu()
    {        
        _audioManager.PauseGameplaySounds(true);
        if (_gameManager.GameStarted)
        {
            _audioManager.PlayOptionsSound(Constants.Sounds.BookOpen);
        } else
        {
            _audioManager.StartMusicLoop();
        }

        _menu.SetActive(true);
        Time.timeScale = 0;
    }       

    void HandleInput()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (!_gameManager.GameStarted || _popupManager.ActivePopup == PopupType.GameOver) return;

            if (_popupManager.ActivePopup != null)
            {
                _popupManager.SetPopupActive(_popupManager.ActivePopup, false);
            }
            else
            {
                ToggleMenu();
            }
        }
    }    
}

using UnityEngine;
using System.Collections.Generic;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    AudioSource _optionsSoundsSource;
    AudioSource _gameplaySoundsSource;
    AudioSource _musicSource;
    Dictionary<string, AudioClip> _audioClips = new Dictionary<string, AudioClip>();
    List<string> _musicTracks { get; set; } = new List<string>();
    List<string> _playlist = new List<string>();

    int _currentTrackIndex = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }        
    }

    private void Start()
    {
        _optionsSoundsSource = gameObject.AddComponent<AudioSource>();
        _gameplaySoundsSource = gameObject.AddComponent<AudioSource>();
        _musicSource = gameObject.AddComponent<AudioSource>();
        _musicSource.loop = false;

        LoadAudioClips(); 
    }

    void Update()
    {
        if (_playlist.Count == 0 || _musicSource.clip == null)
        {
            return;
        }

        // Check if the current track has finished playing
        if (_musicSource.time >= _musicSource.clip.length)
        {
            // Move to the next track and play it
            _currentTrackIndex = (_currentTrackIndex + 1) % _playlist.Count;
            PlayNextTrack();
        }        
    }

    public void StartMusicLoop()
    {
        if (IsMusicPlaying()) return;

        if (_musicTracks.Count == 0)
        {
            Debug.LogError("No music tracks available!");
            return;
        }

        _playlist = new List<string>(_musicTracks);
        ShufflePlaylist(_playlist);
        _currentTrackIndex = 0;

        PlayNextTrack();
    }

    public void PlayGameplaySound(string clipName)
    {
        if (_audioClips.TryGetValue(clipName, out var clip))
        {
            if (_gameplaySoundsSource.enabled)
            {
                _gameplaySoundsSource.PlayOneShot(clip);
            }
        }
        else
        {
            Debug.LogError($"AudioClip {clipName} not found!");
        }
    }

    public void PlayOptionsSound(string clipName)
    {
        if (_audioClips.TryGetValue(clipName, out var clip))
        {
            if (_optionsSoundsSource.enabled)
            {
                _optionsSoundsSource.PlayOneShot(clip);
            }
        }
        else
        {
            Debug.LogError($"AudioClip {clipName} not found!");
        }
    }

    public void PauseGameplaySounds(bool pause)
    {
        if (pause)
        {
            _gameplaySoundsSource.Pause();
        }
        else
        {
            _gameplaySoundsSource.UnPause();
        }
    }

    public void ToggleSound(bool isOn)
    {
        _gameplaySoundsSource.enabled = isOn;
        _optionsSoundsSource.enabled = isOn;
    }

    public void ToggleMusic(bool isOn)
    {
        if (isOn)
        {
            _musicSource.UnPause();
        }
        else
        {
            _musicSource.Pause();
        }
    }

    public void SetMusicVolume(float value)
    {
        _musicSource.volume = value;
    }

    public void SetSoundVolume(float value)
    {
        _gameplaySoundsSource.volume = value;
        _optionsSoundsSource.volume = value;
    }

    void PlayNextTrack()
    {
        if (_playlist.Count == 0) return;

        string trackName = _playlist[_currentTrackIndex];

        if (_audioClips.TryGetValue(trackName, out var clip))
        {
            _musicSource.clip = clip;
            _musicSource.Play();
        }
        else
        {
            Debug.LogError($"Track {trackName} not found!");
        }
    }

    void ShufflePlaylist(List<string> list)
    {
        for (int i = list.Count - 1; i > 0; i--)
        {
            int randomIndex = Random.Range(0, i + 1);
            string temp = list[i];
            list[i] = list[randomIndex];
            list[randomIndex] = temp;
        }
    }

    void LoadAudioClips()
    {
        string[] clipNames = {
            Constants.Sounds.BookClose,
            Constants.Sounds.BookOpen,
            Constants.Sounds.Click,
            Constants.Sounds.Damage,
            Constants.Sounds.GameOver,
            Constants.Sounds.GameWin,
            Constants.Sounds.GeneratorDown,
            Constants.Sounds.GeneratorUp,
            Constants.Sounds.MoveCard,
            Constants.Sounds.ResourceDown,
            Constants.Sounds.ResourceUp,
            Constants.Sounds.TowerUp,
            Constants.Sounds.VictoryIsOurs,
            Constants.Sounds.WallUp,
        };
        foreach (var name in clipNames)
        {
            _audioClips[name] = Resources.Load<AudioClip>($"{Paths.Sounds.Base}/{name}");
        }

        string[] musicNames = {
            Constants.Music.Track01,
            Constants.Music.Track02,
            Constants.Music.Track03,
            Constants.Music.Track04,
            Constants.Music.Track05,
            Constants.Music.Track06,
            Constants.Music.Track07,
            Constants.Music.Track08,
            Constants.Music.Track09,
            Constants.Music.Track10,
            Constants.Music.Track11,
            Constants.Music.Track12,
            Constants.Music.Track13,
            Constants.Music.Track14,
        };
        foreach (var name in musicNames)
        {
            _audioClips[name] = Resources.Load<AudioClip>($"{Paths.Music.Base}/{name}");
            _musicTracks.Add(name);
        }
    }    

    bool IsMusicPlaying()
    {
        return _musicSource.isPlaying || _musicSource.clip != null;
    }
}

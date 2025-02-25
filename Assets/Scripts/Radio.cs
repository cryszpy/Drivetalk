using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Radio : MonoBehaviour
{

    [Tooltip("Reference to the radio's waveform line renderer.")]
    [SerializeField] private LineRenderer lineRenderer;

    [Tooltip("Reference to the current song's audio clip component.")]
    public AudioClip currentSong;

    [Tooltip("Current song's index number in list of total songs.")]
    [HideInInspector] public int currentSongIndex;

    [Tooltip("Reference to the current song's color material.")]
    public Gradient currentColor;

    [Tooltip("Array of all radio song audio clip components.")]
    public AudioClip[] songs;

    [Tooltip("Array of all radio song material colors.")]
    public Gradient[] songColors;

    [Tooltip("Reference to the radio's audio source component.")]
    public AudioSource audioSource;

    [Tooltip("Boolean flag; Checks whether a song is currently playing.")]
    [HideInInspector] public bool isSongPlaying;

    [Tooltip("Timer to track progress through the current radio song.")]
    [HideInInspector] public float songTimer;

    void Start() {

        // Checks if the radio has a valid audio source, and adds one if not
        if (!audioSource && TryGetComponent<AudioSource>(out var source)) {
            audioSource = source;
        } else if (!audioSource) {
            audioSource = gameObject.AddComponent<AudioSource>();

            // Assigns radio song variables (volume, pitch, 2D/3D spatial blend, etc)
            audioSource.playOnAwake = false;
            audioSource.loop = false;

            audioSource.volume = 0f;
            audioSource.pitch = 1f;
            audioSource.spatialBlend = 0.75f;

            Debug.LogWarning("Radio audio source component was missing! Added.");
        }

        // If there are songs able to be played—
        if (songs.Length > 0) {

            // Chooses a random radio song to start out with
            currentSongIndex = Random.Range(0, songs.Length);

            // Sets the first radio song upon opening the game
            SetRadioSong(currentSongIndex);

            Debug.Log("First radio song is: " + currentSong.name);
        }
    }

    private void Update() {

        // Tracks length and progress of song
        if (isSongPlaying) {
            songTimer += Time.deltaTime;

            // Disables dash request when timer reaches limit
            if (songTimer > currentSong.length) {
                isSongPlaying = false;
                songTimer = 0;

                // Stop current song
                audioSource.Stop();

                // Increment song number
                IncrementSongNumber(1);

                // Play next song
                SetRadioSong(currentSongIndex);
            }
        }
    }

    // Increments song number
    public void IncrementSongNumber(int value) {

        // If the incremented song index would be out of the list bounds—
        if (currentSongIndex + value > songs.Length - 1) {

            // Reset to 0
            currentSongIndex = 0;
        } 
        // If the incremented song index would be under 0 (not in list)
        else if (currentSongIndex + value < 0) {

            // Reset to last song in the songs list
            currentSongIndex = songs.Length - 1;
        }
        else {

            // Increment song number
            currentSongIndex += value;
        }
    }

    // Starts playing a song on the radio
    public void SetRadioSong(int index) {

        // Sets current radio song and radio color
        currentSongIndex = index;

        currentSong = songs[currentSongIndex];
        currentColor = songColors[currentSongIndex];

        audioSource.clip = currentSong;

        lineRenderer.colorGradient = currentColor;

        isSongPlaying = true;
        audioSource.Play();

        Debug.Log("Currently playing: " + currentSong.name);
    }
}

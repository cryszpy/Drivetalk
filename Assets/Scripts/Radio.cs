using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Radio : MonoBehaviour
{

    [Header("SCRIPT REFERENCES")] // ---------------------------------------------------------------------------------

    [Tooltip("Reference to the radio's waveform line renderer.")]
    [HideInInspector] public LineRenderer lineRenderer;

    [SerializeField] private RadioChannel radioChannels;

    [Tooltip("Reference to the current song's audio clip component.")]
    [HideInInspector] public AudioClip currentSong;

    [Tooltip("Audio file of radio static to play when no channels are selected.")]
    public AudioClip radioStatic;

    [Tooltip("Gradient color for the static channels.")]
    public Gradient staticColor;

    [Tooltip("Reference to the current song's color material.")]
    [HideInInspector] public Gradient currentColor;

    [Tooltip("Array of all radio song audio clip components.")]
    public AudioClip[] songs;

    [Tooltip("Array of all radio song material colors.")]
    public Gradient[] songColors;

    [Tooltip("Reference to the radio's audio source component.")]
    public AudioSource audioSource;

    [Header("STATS")] // ---------------------------------------------------------------------------------------------------

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

            // Sets the first radio song upon opening the game
            //StartCoroutine(radioChannels.TurnDial(0));
            radioChannels.ChangeChannel(0);
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

                // Play next song
                IncrementSongNumber(1);
            }
        }
    }

    // Increments song number
    public void IncrementSongNumber(int value) {

        // If the incremented song index would be out of the list bounds—
        if (CarController.CurrentRadioChannel + value > songs.Length - 1) {

            // Reset to 0
            StartCoroutine(radioChannels.TurnDial(0, true));
        } 
        // If the incremented song index would be under 0 (not in list)
        else if (CarController.CurrentRadioChannel + value < 0) {

            // Reset to last song in the songs list
            StartCoroutine(radioChannels.TurnDial(songs.Length - 1, true));
        }
        else {

            // Increment song number
            StartCoroutine(radioChannels.TurnDial(CarController.CurrentRadioChannel + value));
        }
    }

    public void SetRadioStatic() {

        // Sets current radio song and radio color
        CarController.CurrentRadioChannel = -1;

        currentSong = radioStatic;
        currentColor = staticColor;

        audioSource.clip = currentSong;

        lineRenderer.colorGradient = currentColor;

        isSongPlaying = false;
        audioSource.Play();

        Debug.Log("Currently not tuned to a channel!");
    }
}

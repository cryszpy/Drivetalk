using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class Radio : MonoBehaviour
{
    public MeshRenderer meshRenderer;

    public AudioClip currentSong;
    [HideInInspector] public int currentSongIndex;
    public Material currentColor;

    public AudioClip[] songs;
    public Material[] songColors;

    public AudioSource audioSource;

    [HideInInspector] public bool isSongPlaying;

    [HideInInspector] public float songTimer;

    void Start() {

        // Checks if the radio has a valid audio source, and adds one if not
        if (TryGetComponent<AudioSource>(out var source)) {
            audioSource = source;
        } else {
            audioSource = gameObject.AddComponent<AudioSource>();

            audioSource.playOnAwake = false;
            audioSource.loop = false;

            audioSource.volume = 1f;
            audioSource.pitch = 1f;
            audioSource.spatialBlend = 0.75f;

            Debug.LogWarning("Radio audio source component was missing! Added.");
        }

        if (songs.Length > 0) {

            // Chooses a random radio song to start out with
            currentSongIndex = Random.Range(0, songs.Length);

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

        if (currentSongIndex + value > songs.Length - 1) {
            currentSongIndex = 0;
        } 
        else if (currentSongIndex + value < 0) {
            currentSongIndex = songs.Length - 1;
        }
        else {
            currentSongIndex += value;
        }
    }

    // Starts playing a song on the radio
    public void SetRadioSong(int index) {

        currentSongIndex = index;

        currentSong = songs[currentSongIndex];
        currentColor = songColors[currentSongIndex];

        audioSource.clip = currentSong;
        meshRenderer.material = currentColor;

        isSongPlaying = true;
        audioSource.Play();

        Debug.Log("Currently playing: " + currentSong.name);
    }
}

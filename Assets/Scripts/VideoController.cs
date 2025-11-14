using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using TMPro;

[RequireComponent(typeof(VideoPlayer))]
public class VideoController : MonoBehaviour
{
    public VideoPlayer videoPlayer;
    public RawImage videoDisplay;

    public Button playPauseButton;
    public Button forwardButton;
    public Button backwardButton;
    public Slider progressSlider;
    public TextMeshProUGUI timeText;

    private bool isDragging = false;

    void Awake() => videoPlayer ??= GetComponent<VideoPlayer>();

    void Start()
    {
        playPauseButton.onClick.AddListener(TogglePlayPause);
        forwardButton.onClick.AddListener(() => Seek(15));
        backwardButton.onClick.AddListener(() => Seek(-15));

        progressSlider.onValueChanged.AddListener(v => { if (isDragging) videoPlayer.time = v * videoPlayer.length; });
        progressSlider.minValue = 0; progressSlider.maxValue = 1;
    }

    void Update()
    {
        if (videoPlayer.isPlaying && !isDragging)
            progressSlider.value = (float)(videoPlayer.time / videoPlayer.length);
    }

    public void TogglePlayPause()
    {
        if (videoPlayer.isPlaying) videoPlayer.Pause();
        else videoPlayer.Play();
    }

    public void Seek(float seconds)
    {
        float t = (float)videoPlayer.time + seconds;
        videoPlayer.time = Mathf.Clamp(t, 0, (float)videoPlayer.length);
    }

    public void SetClip(VideoClip clip)
    {
        videoPlayer.clip = clip;
        videoPlayer.Play();
        progressSlider.value = 0;
    }

    public void OnSliderDragStart() => isDragging = true;
    public void OnSliderDragEnd() => isDragging = false;
}
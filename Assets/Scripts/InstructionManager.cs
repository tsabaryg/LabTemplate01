using UnityEngine;
using TMPro;
using UnityEngine.Video;
using UnityEngine.UI;

[System.Serializable]
public struct Instruction
{
    [TextArea(3, 10)]
    public string text;
    public Sprite image;
    public VideoClip video;
    public AudioClip audio;
}

public class InstructionManager : MonoBehaviour
{
    [Header("מקבצי הנחיות")]
    public Instruction[] instructions;  // גרור 3+ מקבצים ב-Inspector

    [Header("רכיבי UI")]
    // The lines below create empty boxes in the inspector of InstructionPanel under instruction manager script.
    // For each line added here, one must remember to drag and drop the relevant feature from the
    // hirarchy of InstructionPanel (this is the remarks // גרור xxxx)
    // Example: after I added the line "public Button nextButton" I grabed NextButton from the hirarchy the the new
    // empty box created in instruction manager script
    public TextMeshProUGUI textUI;  // גרור TextualInstruction
    public RawImage imageUI;  // גרור VisualInstruction
    public VideoPlayer videoPlayer;  // גרור Video Player
    public AudioSource audioSource;  // גרור VocalInstruction
    public Button nextButton;  // גרור NextButton
    public Button prevButton;  // גרור PrevButton
    public Button PlayPauseButton;  // גרור PlayPauseButton
    public Button ForwardButton;  // גרור SkipForwardButton
    public Button BackwardButton;  // גרור SkipBackwardButtonButton
    public float skipTime = 15.0f; // קפיצה של 15 שניות, אפשר לשנות באינספקטור
    
    private int currentIndex = 0;
    void Start()
    {
        if (instructions.Length == 0) return;  // אם אין מקבצים, אל תמשיך

        // קשר onClick לכפתורים (אם לא קושרת ב-Inspector)
        if (nextButton) nextButton.onClick.AddListener(Next);
        if (prevButton) prevButton.onClick.AddListener(Previous);
        if (PlayPauseButton) PlayPauseButton.onClick.AddListener(TogglePlayPause);
        if (ForwardButton) ForwardButton.onClick.AddListener(SkipForward);
        if (BackwardButton) BackwardButton.onClick.AddListener(SkipBackward);
        UpdateInstruction();
        UpdateButtons();
    }

    public void Next()
    {
        if (currentIndex < instructions.Length - 1)
        {
            currentIndex++;
            UpdateInstruction();
            UpdateButtons();
        }
    }

    public void Previous()
    {
        if (currentIndex > 0)
        {
            currentIndex--;
            UpdateInstruction();
            UpdateButtons();
        }
    }
    public void TogglePlayPause()
    {
        // הגנה מפני קריסה אם הנגן ריק
        if (videoPlayer == null) return;

        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause(); // אם מנגן -> תעצור
        }
        else
        {
            videoPlayer.Play(); // אם עומד -> תנגן
        }
    }
    public void SkipForward()
    {
        // בדיקת בטיחות
        if (videoPlayer == null) return;

        // הוספת זמן לנגן
        videoPlayer.time += skipTime;
    }
    public void SkipBackward()
    {
        // בדיקת בטיחות
        if (videoPlayer == null) return;

        // הוספת זמן לנגן
        videoPlayer.time -= skipTime;
    }
    void UpdateInstruction()
    {
        if (currentIndex >= instructions.Length) return;

        Instruction instr = instructions[currentIndex];

        // טקסט תמיד
        if (textUI) textUI.text = instr.text;

        // עצור מדיה קודמת
        StopMedia();

        // RawImage תמיד active (הוא מציג תמונה או וידאו)
        if (imageUI) imageUI.gameObject.SetActive(true);

        // תמונה
        if (instr.image != null && imageUI)
        {
            imageUI.texture = instr.image.texture;
            if (videoPlayer) videoPlayer.gameObject.SetActive(false);
        }

        // וידאו – הוסף עדכון texture מ-Render Texture
        if (instr.video != null && videoPlayer)
        {
            videoPlayer.clip = instr.video;
            videoPlayer.gameObject.SetActive(true);
            videoPlayer.isLooping = true; // מופעל בלופ
            videoPlayer.Play();
            // הוסף: קשר RawImage לפלט הווידאו
            if (imageUI && videoPlayer.targetTexture != null)
            {
                imageUI.texture = (RenderTexture)videoPlayer.targetTexture;
            }
        }

        // שמע – רק אם יש, ותמיד עצור קודם
        if (instr.audio != null && audioSource)
        {
            audioSource.clip = instr.audio;
            audioSource.gameObject.SetActive(true);
            audioSource.Play();
        }
        else if (audioSource)
        {
            audioSource.gameObject.SetActive(false);
        }
    }

    void UpdateButtons()
    {
        if (prevButton) prevButton.interactable = currentIndex > 0;
        if (nextButton) nextButton.interactable = currentIndex < instructions.Length - 1;
    }

    private void StopMedia()
    {
        if (videoPlayer && videoPlayer.isPlaying)
        {
            videoPlayer.Stop();
            videoPlayer.gameObject.SetActive(false);
        }
        if (audioSource && audioSource.isPlaying)
        {
            audioSource.Stop();
            audioSource.gameObject.SetActive(false);
        }
    }

    // פונקציה ציבורית להפעלה (קרא מסקריפט אחר, כמו Readme.cs)
    public void StartTutorial()
    {
        gameObject.SetActive(true);
        currentIndex = 0;
        UpdateInstruction();
        UpdateButtons();
    }

    void OnDestroy()
    {
        StopMedia();  // נקה בסיום
    }
}
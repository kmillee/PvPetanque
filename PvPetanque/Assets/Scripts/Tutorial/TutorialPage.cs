using UnityEngine;
using UnityEngine.Video;

public enum TutorialCategory
{
    Gameplay,
    Rules,
    Items,
    Credits
}

[CreateAssetMenu(fileName = "TutorialPage", menuName = "Scriptable Objects/TutorialPage")]
public class TutorialPage : ScriptableObject
{
    public TutorialCategory category;
    
    public string pageTitle;
    [TextArea] public string pageText;

    public Sprite pageImage;
    public VideoClip pageVideo;
}

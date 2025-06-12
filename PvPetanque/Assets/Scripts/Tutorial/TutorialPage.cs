using UnityEngine;
using UnityEngine.Video;

[CreateAssetMenu(fileName = "TutorialPage", menuName = "Scriptable Objects/TutorialPage")]
public class TutorialPage : ScriptableObject
{
    [TextArea]
    public string pageText;

    public Sprite pageImage;
    public VideoClip pageVideo;
    
}

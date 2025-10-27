using UnityEngine;
using UnityEngine.UI;

public class PlayerImageDisplay : MonoBehaviour
{
    public Image portraitImage;
    public Image frameImage;

    private void Start()
    {
        UpdateImages();
    }

    private void OnEnable()
    {
        PlayerImageEvents.OnImageChanged += UpdateImages;
    }

    private void OnDisable()
    {
        PlayerImageEvents.OnImageChanged -= UpdateImages;
    }

    public void UpdateImages()
    {
        var manager = PlayerImageManager.Instance;
        if (manager == null) return;

        var portrait = manager.GetSelectedPortrait();
        var frame = manager.GetSelectedFrame();

        if (portraitImage != null && portrait != null)
            portraitImage.sprite = portrait.portraitSprite;

        if (frameImage != null && frame != null)
            frameImage.sprite = frame.frameSprite;
    }
}

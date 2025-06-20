using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text tooltipText;

    private void Awake()
    {
        Instance = this;
        HideTooltip();
    }

    private void Update()
    {
        if (panel.activeSelf)
        {
            // Vector2 mousePos = Input.mousePosition;
            // panel.transform.position = mousePos + new Vector2(10f, -10f); // offset
        }
    }

    public void ShowTooltip(string description)
    {
        panel.SetActive(true);
        tooltipText.text = description;
        // tooltipText.text =
    }

    public void HideTooltip()
    {
        panel.SetActive(false);
    }
}

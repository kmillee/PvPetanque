using TMPro;
using UnityEngine;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance;

    [SerializeField] private GameObject panel;
    [SerializeField] private TMP_Text titleText;
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

    public void ShowTooltip(string title, string description)
    {
        panel.SetActive(true);
        titleText.text = title; // You can set a specific title if needed
        tooltipText.text = description;

        // tooltipText.text =
    }

    public void HideTooltip()
    {
        panel.SetActive(false);
    }
}

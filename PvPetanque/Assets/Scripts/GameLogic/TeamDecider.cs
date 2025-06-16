using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class TeamDecider : MonoBehaviour
{
    [Header("Decider Menu")]
    [SerializeField] private GameObject deciderMenu;

    [Header("Team Labels")]
    [SerializeField] private TMP_Text teamALabel;
    [SerializeField] private TMP_Text teamBLabel;

    [Header("Flip Settings")]
    [SerializeField] private float flipDuration = 4f; // Total duration for flipping
    [SerializeField] private float flipSpeed = 0.2f; // Speed of flipping

    [Header("Game Launch")]
    [SerializeField] private Button launchButton;
    [SerializeField] private GameObject settingsMenu; // Reference to the settings menu

    private string originalTeamAName;
    private string originalTeamBName;
    private Color teamAColor;
    private Color teamBColor;
    private bool teamAStarts;

    private void Start()
    {
        // Save original names so we can restore after flipping
        originalTeamAName = MatchSettingsData.teamNameA;
        originalTeamBName = MatchSettingsData.teamNameB;
        teamALabel.text = originalTeamAName;
        teamBLabel.text = originalTeamBName;

        // Set initial colors
        teamAColor = MatchSettingsData.teamColorA;
        teamBColor = MatchSettingsData.teamColorB;

        launchButton.onClick.AddListener(OnLetsGoClicked);
    }

    public void OnLetsGoClicked()
    {
        launchButton.interactable = false; // Prevent double click
        settingsMenu.SetActive(false); // Hide settings menu or any other UI
        deciderMenu.SetActive(true);
        teamALabel.gameObject.SetActive(true);
        StartCoroutine(FlipAndStart());
    }

    IEnumerator FlipAndStart()
    {
        float elapsed = 0f;
        bool isFlipped = false;

        while (elapsed < flipDuration)
        {
            // Lerp color back and forth between team colors
            float t = Mathf.PingPong(elapsed / flipDuration * 2, 1f);
            deciderMenu.GetComponent<Image>().color = Color.Lerp(teamAColor, teamBColor, t);

            // Swap names
            string temp = teamALabel.text;
            teamALabel.text = teamBLabel.text;
            teamBLabel.text = temp;

            isFlipped = !isFlipped;
            elapsed += flipSpeed;

            yield return new WaitForSeconds(flipSpeed);
        }

        // Final result: decide who starts
        teamAStarts = Random.value < 0.5f;

        if (!teamAStarts)
        {
            // Flip one more time if Team B should start
            string temp = teamALabel.text;
            teamALabel.text = teamBLabel.text;
            teamBLabel.text = temp;
        }
        // Restore original colors
        deciderMenu.GetComponent<Image>().color = teamAStarts ? teamAColor : teamBColor;
        teamALabel.text = teamAStarts ? originalTeamAName : originalTeamBName;
        Debug.Log(teamAStarts ? "Team A starts!" : "Team B starts!");

        MatchSettingsData.firstTeam = teamAStarts ? Team.TeamA : Team.TeamB;

        yield return new WaitForSeconds(2f); // Short pause before game starts

        LaunchGame();
    }

    public void LaunchGame()
    {
        SceneManager.LoadScene("GameScene");
    }
}

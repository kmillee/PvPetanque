using UnityEngine;

public class ConfettiController : MonoBehaviour
{
    public ParticleSystem confetti1;
    public ParticleSystem confetti2;

    public void Play(Color teamColor)
    {
        ApplyColors(confetti1, teamColor);
        ApplyColors(confetti2, teamColor);

        confetti1.Play();
        confetti2.Play();
    }

    private void ApplyColors(ParticleSystem ps, Color teamColor)
    {
        var main = ps.main;
        main.startColor = new ParticleSystem.MinMaxGradient(teamColor, Color.white); // 50% teamColor, 50% white

        // Optional: Clear color over lifetime if you don't want it to interfere
        var colorOverLifetime = ps.colorOverLifetime;
        colorOverLifetime.enabled = false;
    }
}

using UnityEngine;


[CreateAssetMenu(menuName = "Game Effects/Bonus Ball")]
public class BonusBall : GameEffect
{

    public override void Apply(GameObject target)
    {
        // Check if the target has a Team component
        TeamTag teamTag = target.GetComponent<TeamTag>();
        Debug.Log($"BonusBall: Applying bonus ball effect to target {target.name}.");

        if (teamTag == null)
        {
            Debug.LogWarning("BonusBall: Target does not have a TeamTag component.");
            return;
        }
        if (teamTag != null)
        {
            Team team = teamTag.team;
            Debug.Log($"BonusBall: Applying bonus ball effect to {team}.");
            GameManager.instance.IncreaseMaxBalls(team);

        }
    }
    

}


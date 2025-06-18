using UnityEngine;


[CreateAssetMenu(menuName = "Game Effects/Bonus Ball")]
public class BonusBall : GameEffect
{
    
    public override void Apply(GameObject target)
    {
        // Check if the target has a Team component
        TeamTag teamTag = target.GetComponent<TeamTag>();
        if (teamTag != null)
        {
            Team team = teamTag.team;
            GameManager.instance.IncreaseMaxBalls(team);

        }
    }
}


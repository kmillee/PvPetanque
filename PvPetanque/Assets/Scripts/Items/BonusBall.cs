using UnityEngine;


[CreateAssetMenu(menuName = "Game Effects/Bonus Ball")]
public class BonusBall : GameEffect
{

    public override void Apply(GameObject target)
    {
        Debug.Log($"BonusBall: Applying bonus ball effect to target {target.name}.");

        Ball ball = target.GetComponent<Ball>();
        if (ball != null)
        {
            Team team = ball.Team;
            if (team == null)
            {
                Debug.LogWarning("BonusBall: Ball does not have a Team assigned.");
                return;
            }
            Debug.Log($"BonusBall: Ball belongs to team {team}.");
            GameManager.instance.IncreaseMaxBalls(team);
        }
    
    }


    

}


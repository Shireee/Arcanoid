using UnityEngine;

public class Fire : BonusBase
{
    PlayerScript playerObj;


    public override void BonusActivate()
    {
        playerObj = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerScript>();
        playerObj.force = 4;

        var balls = GameObject.FindGameObjectsWithTag("Ball");

        foreach (GameObject ball in balls)
        {
            var BallSprite = ball.GetComponent<SpriteRenderer>();
            BallSprite.color = Color.red;
        }
    }
}
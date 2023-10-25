using UnityEngine;

// Script for storing game data
[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class GameDataScript : ScriptableObject
{
    public bool resetOnStart; // checkbox for Reset() 
    public int level = 1; // Initial lvl
    public int balls = 6; // Initial number of balls
    public int points = 0; // Initial number of points
    public int pointsToBall = 0; // Initial number of point needed for getting ball
    public bool music = true; // background music controller
    public bool sound = true; // hit music controller 

    // Restart state of GameData object when restarting game 
    public void Reset()
    {
        level = 1;
        balls = 6;
        points = 0;
        pointsToBall = 0;
    }
}

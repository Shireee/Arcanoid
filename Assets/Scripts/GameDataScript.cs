using UnityEngine;

// Script for storing game data
[CreateAssetMenu(fileName = "GameData", menuName = "Game Data", order = 51)]
public class GameDataScript : ScriptableObject
{
    public bool resetOnStart;
    public int level = 1;
    public int balls = 6;
    public int points = 0;
    public void Reset()
    {
        level = 1;
        balls = 6;
        points = 0;
    }
}
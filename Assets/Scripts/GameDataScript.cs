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
    public bool IsNewGame = false;

    // Restart state of GameData object when restarting game 
    public void Reset()
    {
        level = 1;
        balls = 6;
        points = 0;
        pointsToBall = 0;
    }

    // Seving game state
    public void Save()
    {
        PlayerPrefs.SetInt("level", level);
        PlayerPrefs.SetInt("balls", balls);
        PlayerPrefs.SetInt("points", points);
        PlayerPrefs.SetInt("pointsToBall", pointsToBall);
        PlayerPrefs.SetInt("music", music ? 1 : 0);
        PlayerPrefs.SetInt("sound", sound ? 1 : 0);
    }

    // Load saved game state
    public void Load()
    {
        level = PlayerPrefs.GetInt("level", 1);
        balls = PlayerPrefs.GetInt("balls", 6);
        points = PlayerPrefs.GetInt("points", 0);
        pointsToBall = PlayerPrefs.GetInt("pointsToBall", 0);
        music = PlayerPrefs.GetInt("music", 1) == 1;
        sound = PlayerPrefs.GetInt("sound", 1) == 1;
    }

    // Bonus probability 
    public int[] getProbab()
    {
        int[] probab = new int[3];
        probab[0] = 30; // Fire
        probab[1] = 30; // Steel
        probab[2] = 50; // Norm

        int[] probsum = new int[3];
        probsum[0] = probab[0];
        for (int i = 1; i < 3; i++)
            probsum[i] = probsum[i - 1] + probab[i];

        return probsum;
    }

}

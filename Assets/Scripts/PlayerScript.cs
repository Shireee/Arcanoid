using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerScript : MonoBehaviour
{
    const int maxLevel = 30;
    [Range(1, maxLevel)]
    public int level = 1;
    public float ballVelocityMult = 0.02f;
    public GameObject bluePrefab;
    public GameObject redPrefab;
    public GameObject greenPrefab;
    public GameObject yellowPrefab;
    public GameObject ballPrefab;

    static Collider2D[] colliders = new Collider2D[50];
    static ContactFilter2D contactFilter = new ContactFilter2D();
    public GameDataScript gameData;
    static bool gameStarted = false;

    // Bonus 
    public GameObject bonusPrefab;
    public int force = 1;

    // Audio and interface
    AudioSource audioSrc;
    public AudioClip pointSound;
    public Canvas canvas;
    public AudioMixerGroup mixerGroup;

    int requiredPointsToBall { get { return 400 + (level - 1) * 20; } }

    void Start()
    {
        
        audioSrc = Camera.main.GetComponent<AudioSource>(); // Getting audio source
        audioSrc.outputAudioMixerGroup = mixerGroup;
        Cursor.visible = false; // Off cursor
        if (!gameStarted)
        {
            // Menu when start
            canvas.enabled = true;
            Time.timeScale = 0;
            Cursor.visible = true;

            gameStarted = true;
            if (gameData.resetOnStart) gameData.Load(); // Load GameData object
        }
        level = gameData.level;
        SetMusic();
        StartLevel();
    }

    void Update()
    {
        // Checking for pause 
        if (Time.timeScale > 0)
        {
            canvas.enabled = false;
            Cursor.visible = false;
            var mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            var pos = transform.position;
            pos.x = mousePos.x;
            transform.position = pos;
        }

        // Listener of "Space" button
        if (Input.GetButtonDown("Pause"))
        {
            Text text = canvas.transform.Find("Header_text").GetComponent<Text>();
            if (text != null)
            {
                text.text = "Pause"; 
            }
            canvas.enabled = true; // Включаем канвас
            Cursor.visible = true;
            if (Time.timeScale > 0) Time.timeScale = 0;
            else Time.timeScale = 1;
        }

        // Listener of "M" button
        if (Input.GetKeyDown(KeyCode.M))
        {
            gameData.music = !gameData.music; // Off background sound
            SetMusic();
        }

        // Listener of "M" button
        if (Input.GetKeyDown(KeyCode.S))
        {
            gameData.sound = !gameData.sound; // Off hit sound
        }

        // Listener of "N" button
        if (Input.GetKeyDown(KeyCode.N))
        {
            gameData.Reset(); // Reseting game
            SceneManager.LoadScene("MainScene");
        }

        // Listener of "Esc" button
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit(); // Closing game
        }
    }
    // Saving data when quit from game
    void OnApplicationQuit()
    {
        gameData.Save();
    }

    // Function for setting the bg image corresponding to the level
    void SetBackground()
    {
        var bg = GameObject.Find("GameBackground").GetComponent<SpriteRenderer>();
        bg.sprite = Resources.Load(level.ToString("d2"), typeof(Sprite)) as Sprite;
    }

    // Function for setting the game field
    void StartLevel()
    {
        SetBackground();
        var yMax = Camera.main.orthographicSize * 0.8f;
        var xMax = Camera.main.orthographicSize * Camera.main.aspect * 0.85f;
        CreateBlocks(bluePrefab, xMax, yMax, level, 8);
        CreateBlocks(redPrefab, xMax, yMax, 1 + level, 10);
        CreateBlocks(greenPrefab, xMax, yMax, 1 + level, 12);
        CreateBlocks(yellowPrefab, xMax, yMax, 2 + level, 15);
        CreateBalls();
    }

    // Function for creating Blocks
    void CreateBlocks(GameObject prefab, float xMax, float yMax, int count, int maxCount)
    {
        {
            if (count > maxCount) count = maxCount;
            for (int i = 0; i < count; i++)
            {
                for (int k = 0; k < 20; k++)
                {
                    var obj = Instantiate(prefab,
                    new Vector3((Random.value * 2 - 1) * xMax, Random.value * yMax, 0), Quaternion.identity);
                    if (obj.GetComponent<Collider2D>().OverlapCollider(contactFilter.NoFilter(), colliders) == 0) break;
                    Destroy(obj);
                }
            }
        }
    }

    // Function for destroing ball
    public void BallDestroyed()
    {
        gameData.balls--;
        StartCoroutine(BallDestroyedCoroutine());
    }

    // Function for create balls
    void CreateBalls()
    {
        int count = 2;
        if (gameData.balls == 1) count = 1;

        for (int i = 0; i < count; i++)
        {
            var obj = Instantiate(ballPrefab);
            var ball = obj.GetComponent<BallScript>();
            ball.ballInitialForce += new Vector2(10 * i, 0);
            ball.ballInitialForce *= 1 + level * ballVelocityMult;
        }
    }

    // Corutin to check for playing ball getting sound
    IEnumerator BlockDestroyedCoroutine2()
    {
        for (int i = 0; i < 10; i++)
        {
            yield return new WaitForSeconds(0.2f);
            audioSrc.PlayOneShot(pointSound, 5);
        }
    }

    // Corutin to check for game objects "Block", if no such obj level++
    IEnumerator BlockDestroyedCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (GameObject.FindGameObjectsWithTag("Block").Length == 0)
        {
            if (level < maxLevel) gameData.level++;
            SceneManager.LoadScene("MainScene");
        }
    }

    // Function for destroying block and getting point
    public void BlockDestroyed(int points, string name, Vector3 pos)
    {
        gameData.points += points;
        if (gameData.sound) audioSrc.PlayOneShot(pointSound, 5);
        gameData.pointsToBall += points;

        if (gameData.pointsToBall >= requiredPointsToBall)
        {
            gameData.balls++;
            gameData.pointsToBall -= requiredPointsToBall;
            if (gameData.sound) StartCoroutine(BlockDestroyedCoroutine2());

        }
        // Create bonus if green block destroed 
        if (name == "Green Block(Clone)")
        {
            int[] probab = gameData.getProbab();
            CreateBonus(probab, pos);
        }

        StartCoroutine(BlockDestroyedCoroutine());
    }

    // Handle bounus creation
    public void CreateBonus(int[] probab, Vector3 pos)
    {
        int rand = Random.Range(1, 100);
        var obj = Instantiate(bonusPrefab, pos, Quaternion.identity);
        if (rand < probab[0])
        {
            obj.AddComponent<Fire>().gameData = gameData;
            obj.GetComponent<Fire>().textObject = obj.transform.Find("Canvas").gameObject.transform.Find("BlockText").gameObject;
            obj.GetComponent<Fire>().text = "Fire";
            obj.GetComponent<Fire>().bonusColor = Color.red;
            obj.GetComponent<Fire>().textColor = Color.black;
            return;
        }
        else if (rand < probab[1])
        {
            obj.AddComponent<Steel>().gameData = gameData;
            obj.GetComponent<Steel>().textObject = obj.transform.Find("Canvas").gameObject.transform.Find("BlockText").gameObject;
            obj.GetComponent<Steel>().text = "Steel";
            obj.GetComponent<Steel>().bonusColor = Color.gray;
            obj.GetComponent<Steel>().textColor = Color.black;
            return;
        }
        else if (rand < probab[2])
        {
            obj.AddComponent<Norm>().gameData = gameData;
            obj.GetComponent<Norm>().textObject = obj.transform.Find("Canvas").gameObject.transform.Find("BlockText").gameObject;
            obj.GetComponent<Norm>().text = "Norm";
            obj.GetComponent<Norm>().bonusColor = Color.white;
            obj.GetComponent<Norm>().textColor = Color.black;
            return;
        }

    }

    // Corutin to check for game objects "Ball"
    IEnumerator BallDestroyedCoroutine()
    {
        yield return new WaitForSeconds(0.1f);
        if (GameObject.FindGameObjectsWithTag("Ball").Length == 0)
            if (gameData.balls > 0) CreateBalls();
            else
            {
                gameData.Reset();
                SceneManager.LoadScene("MainScene");
            }
    }

    // Function for changing state of bg-music
    void SetMusic()
    {
        if (gameData.music) audioSrc.Play();
        else audioSrc.Stop();
    }
    
    // Displaying "Off" or "On"
    string OnOff(bool boolVal)
    {
        return boolVal ? "on" : "off";
    }


    // Draw simple UI
    void OnGUI()
    {
        GUI.Label(new Rect(5, 4, Screen.width - 10, 100),
        string.Format(
        "<color=yellow><size=30>Level <b>{0}</b> Balls <b>{1}</b>" +
        " Score <b>{2}</b></size></color>",
        gameData.level, gameData.balls, gameData.points));

        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.UpperRight;
        GUI.Label(new Rect(5, 14, Screen.width - 10, 100),
        string.Format(
         "<color=yellow><size=20><color=white>Space</color>-pause {0}" +
         " <color=white>N</color>-new" +
         " <color=white>J</color>-jump" +
         " <color=white>M</color>-music {1}" +
         " <color=white>S</color>-sound {2}" +
         " <color=white>Esc</color>-exit</size></color>",
         OnOff(Time.timeScale > 0), OnOff(!gameData.music),
         OnOff(!gameData.sound)), style);
    }
}

using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Audio;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public Canvas canvas; 
    public GameDataScript gameData;
    public AudioMixer audioMixer;
    public AudioSource audioSrc;

    void Start()
    {
        audioSrc = Camera.main.GetComponent<AudioSource>();
        Button btn_continue = canvas.transform.Find("Button_continue").GetComponent<Button>();
        Button btn_newGame = canvas.transform.Find("Button_newGame").GetComponent<Button>();
        Button btn_exit = canvas.transform.Find("Button_exit").GetComponent<Button>();
        Toggle toggle_background_music = canvas.transform.Find("toggle_background_music").GetComponent<Toggle>();
        Toggle toggle_sound_effects = canvas.transform.Find("toggle_sound_effects").GetComponent<Toggle>();
        Slider slider_background_music = canvas.transform.Find("slider_background_music").GetComponent<Slider>();
        Slider slider_sound_effects = canvas.transform.Find("slider_sound_effects").GetComponent<Slider>();

        btn_continue.onClick.AddListener(HideCanvas);
        btn_newGame.onClick.AddListener(NewGame);
        btn_exit.onClick.AddListener(Exit);

        toggle_background_music.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle_background_music, slider_background_music);
            setBgm(slider_background_music);
        });


        toggle_sound_effects.onValueChanged.AddListener(delegate {
            ToggleValueChanged(toggle_sound_effects, slider_sound_effects);
            setSfx(slider_sound_effects);
        });



        slider_background_music.onValueChanged.AddListener(delegate {
            setBgm(slider_background_music);
        });

        slider_sound_effects.onValueChanged.AddListener(delegate {
            setSfx(slider_sound_effects);
        });

    }

    void SetMusic()
    {
        if (gameData.music) audioSrc.Play();
        else audioSrc.Stop();
    }

    public void setBgm(Slider slider)
    {
        float volume = slider.value;
        if (volume != 0) audioMixer.SetFloat("bgm", Mathf.Log10(volume) * 20);
    }

    public void setSfx(Slider slider)
    {
        float volume = slider.value;
        if (volume != 0) audioMixer.SetFloat("sfx", Mathf.Log10(volume) * 20);
    }



    // Toggle controller 
    void ToggleValueChanged(Toggle toggle, Slider slider)
    {
        if (toggle == canvas.transform.Find("toggle_background_music").GetComponent<Toggle>())
        {
            if (toggle.isOn)
            {
                slider.interactable = true;
                if (!gameData.music) gameData.music = !gameData.music;
                Debug.Log("music on");
            }
            else
            {
                slider.interactable = false;
                if (gameData.music) gameData.music = !gameData.music;
                Debug.Log("music off");
            }
            SetMusic();
        }
        else if (toggle == canvas.transform.Find("toggle_sound_effects").GetComponent<Toggle>())
        {   
            if (toggle.isOn)
            {
                slider.interactable = true;
                if (!gameData.sound) gameData.sound = !gameData.sound;
                Debug.Log("sound_effects on");
            }
            else
            {
                slider.interactable = false;
                if (gameData.sound) gameData.sound = !gameData.sound;
                Debug.Log("sound_effects off");
            }
        }
    }


    // New game button logic
    void NewGame()
    {
        gameData.IsNewGame = false;
        canvas.enabled = false; // disable canvas
        Time.timeScale = 1; // turn on game
        gameData.Reset(); 
        SceneManager.LoadScene("MainScene");
    }
    
    // Continue button logic
    void HideCanvas()
    {
        canvas.enabled = false; 
        Time.timeScale = 1;
    }

    // Exit button logic
    void Exit()
    {
        Debug.Log("Game closed!");
        Application.Quit();
    }
}
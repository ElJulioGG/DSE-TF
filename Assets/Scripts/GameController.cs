using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    [SerializeField] private int startHP = 5;
    private bool lowHP = false;
    private bool oneHP = false;
    [SerializeField] private float musicVolume = 0.7f;
    [SerializeField] public Transform playerSpawn;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private void Awake()
    {
        GameManager.instance.playerspawn = playerSpawn;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }
    void Start()
    {
        SoundFXManager.instance.PlaySoundByName("bgm", gameObject.transform, musicVolume, 1f, true);
        GameManager.instance.playerDied = false;
        GameManager.instance.playerHP = startHP;
<<<<<<< Updated upstream
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = playerSpawn.position;
=======
        GameManager.instance.lastLevelSceneName = SceneManager.GetActiveScene().name;
>>>>>>> Stashed changes
    }

    // Update is called once per frame
    void Update()
    {
        if(GameManager.instance.playerHP == 2&& !lowHP)
        {
            SoundFXManager.instance.ChangePitchByName("bgm", 0.8f);
            lowHP = true;
            oneHP = false;
        }
        if (GameManager.instance.playerHP == 1&&!oneHP)
        {
            SoundFXManager.instance.ChangePitchByName("bgm", 0.6f);
            lowHP = false;
            oneHP = true;
        }
        if(GameManager.instance.playerHP > 2&& (lowHP||oneHP))
        {
            
            SoundFXManager.instance.ChangePitchByName("bgm",1f);
            lowHP = false;
            oneHP = true;
        }

        if (GameManager.instance.playerHP <= 0 && !GameManager.instance.playerDied)
        {
        // Marcar que ya murió para no ejecutar esto muchas veces
            GameManager.instance.playerDied = true;

        // Parar la música del nivel
            SoundFXManager.instance.StopSoundByName("bgm");

        // Ir a la escena de Game Over
            SceneManager.LoadScene("GAMEOVER");
        }

        if (Input.GetKeyDown(KeyCode.R) && !GameManager.instance.playerDied)
        {
            SoundFXManager.instance.StopSoundByName("bgm");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}

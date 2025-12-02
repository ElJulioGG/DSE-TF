using UnityEngine;
using UnityEngine.SceneManagement;


public class GameController : MonoBehaviour
{
    [SerializeField] private int startHP = 5;
    private bool lowHP = false;
    private bool oneHP = false;
    [SerializeField] private float musicVolume = 0.7f;

    private void Awake()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.None;
    }

    void Start()
    {
        SoundFXManager.instance.PlaySoundByName("bgm", transform, musicVolume, 1f, true);

        GameManager.instance.playerDied = false;
        GameManager.instance.playerHP = startHP;
<<<<<<< HEAD
<<<<<<< Updated upstream
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        player.transform.position = playerSpawn.position;
=======
        GameManager.instance.lastLevelSceneName = SceneManager.GetActiveScene().name;
>>>>>>> Stashed changes
=======

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        // Si hay un checkpoint guardado, respawn all�
        if (GameManager.instance.playerspawn != Vector3.zero)
        {
            player.transform.position = GameManager.instance.playerspawn;
        }
        
>>>>>>> 1ad302090f4f974e7efdf6d39f55a18c3036e688
    }

    void Update()
    {
        if (GameManager.instance.playerHP == 2 && !lowHP)
        {
            SoundFXManager.instance.ChangePitchByName("bgm", 0.8f);
            lowHP = true;
            oneHP = false;
        }

        if (GameManager.instance.playerHP == 1 && !oneHP)
        {
            SoundFXManager.instance.ChangePitchByName("bgm", 0.6f);
            lowHP = false;
            oneHP = true;
        }

        if (GameManager.instance.playerHP > 2 && (lowHP || oneHP))
        {
            SoundFXManager.instance.ChangePitchByName("bgm", 1f);
            lowHP = false;
            oneHP = false;
        }

<<<<<<< HEAD
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
=======
        if (GameManager.instance.playerHP <= 0)
        {
            SoundFXManager.instance.StopSoundByName("bgm");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

        if (Input.GetKeyDown(KeyCode.R))
>>>>>>> 1ad302090f4f974e7efdf6d39f55a18c3036e688
        {
            SoundFXManager.instance.StopSoundByName("bgm");
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }

    }
}

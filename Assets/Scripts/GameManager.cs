using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    [SerializeField] public int activeCamera = 0;
    [SerializeField] public int currentPlayerTool = 0;
    [SerializeField] public bool playerDied;
    [SerializeField] public bool playerCanInput = true;
    [SerializeField] public bool weaponChange = true;
    [SerializeField] public int ducksCollected = 0;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
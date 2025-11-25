using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] private Image[] fill;
    [SerializeField] private Image face;
    [SerializeField] private Sprite[] faces;

    [Header("HP Colors")]
    [SerializeField] private Color colorHP1 = Color.red;
    [SerializeField] private Color colorHP2 = new Color(1f, 0.5f, 0f);
    [SerializeField] private Color colorHP3 = Color.yellow;
    [SerializeField] private Color colorHP4 = Color.green;
    [SerializeField] private Color colorHP5 = Color.blue;

    [Header("Loop Shakes")]
    public bool orangeShakeOn = false; // HP 2
    public bool redShakeOn = false;    // HP 1

    [Header("Shake Settings (HP 2)")]
    [SerializeField] private float orangeDuration = 0.4f;
    [SerializeField] private float orangeStrength = 15f;
    [SerializeField] private int orangeVibrato = 10;
    [SerializeField] private float orangeRandomness = 90f;

    [Header("Shake Settings (HP 1)")]
    [SerializeField] private float redDuration = 0.6f;
    [SerializeField] private float redStrength = 30f;
    [SerializeField] private int redVibrato = 20;
    [SerializeField] private float redRandomness = 120f;

    private UITweens uiTweens;
    private int lastHP = -1;
    private bool lastOrange = false;
    private bool lastRed = false;

    private void Awake()
    {
        uiTweens = GetComponent<UITweens>();
    }

    private Color GetColorByHP(int hp)
    {
        return hp switch
        {
            1 => colorHP1,
            2 => colorHP2,
            3 => colorHP3,
            4 => colorHP4,
            5 => colorHP5,
            _ => Color.white
        };
    }

    private void Update()
    {
        int hp = GameManager.instance.playerHP;

        bool changed = hp != lastHP || orangeShakeOn != lastOrange || redShakeOn != lastRed;

        if (changed)
        {
            lastHP = hp;
            lastOrange = orangeShakeOn;
            lastRed = redShakeOn;

            uiTweens.StopShakeLoop(); // always stop first

            if (hp == 2 && orangeShakeOn)
                uiTweens.StartShakeLoop(orangeDuration, orangeStrength, orangeVibrato, orangeRandomness);

            else if (hp == 1 && redShakeOn)
                uiTweens.StartShakeLoop(redDuration, redStrength, redVibrato, redRandomness);
        }

        // Update hearts UI
        for (int i = 0; i < fill.Length; i++)
            fill[i].enabled = i < hp;

        if (hp > 0)
        {
            Color c = GetColorByHP(hp);
            for (int i = 0; i < hp; i++)
                fill[i].color = c;
        }

        face.sprite = faces[Mathf.Clamp(hp, 0, faces.Length - 1)];
    }

    private void OnDisable()
    {
        uiTweens.StopShakeLoop();
    }
}

using TMPro;
using UnityEngine;

public class CoinCounter : MonoBehaviour
{
    [SerializeField] private int coinCount = 0;
    [SerializeField] private TMP_Text coinText;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        coinCount = GameManager.instance.coins;
        coinText.text = "x"+coinCount.ToString();
    }
}

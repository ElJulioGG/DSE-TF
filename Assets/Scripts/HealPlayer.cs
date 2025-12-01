using UnityEngine;

public class HealPlayer : MonoBehaviour
{
    public float healRate = 4f; // hp por segundo
    private float healTimer = 0f;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            healTimer += Time.deltaTime;

            float healInterval = 1f / healRate;

            while (healTimer >= healInterval)
            {
                TryHeal();
                healTimer -= healInterval;
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
            healTimer = 0f;
    }

    private void TryHeal()
    {
        // Si ya tiene vida máxima no curar
        if (GameManager.instance.playerHP >= 5)
            return;

        // Si no tienes monedas no curar
        if (GameManager.instance.coins <= 0)
            return;

        // Curar 1 HP y gastar 1 moneda
        SoundFXManager.instance.PlaySoundByName("heal", gameObject.transform, 1f, 1f, false);
        GameManager.instance.playerHP++;
        GameManager.instance.coins--;

        // opcional: Debug.Log("HP: " + GameManager.instance.playerHP + " | Coins: " + GameManager.instance.coins);
    }
}

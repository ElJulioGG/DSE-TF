using UnityEngine;

public class PlayerStatus : MonoBehaviour
{
    private int lifes = 3;
    private int full_lifes = 3;

    public void damage()
    {
        lifes--;
    }

    public void total_damage()
    {
        full_lifes--;
    }

    public int get_lifes()
    {
        return lifes;
    }

    public int get_full_lifes()
    {
        return full_lifes;
    }

    public void restart_lifes()
    {
        lifes = 3;
    }

    public void restart_full_lifes()
    {
        full_lifes = 3;
    }
}

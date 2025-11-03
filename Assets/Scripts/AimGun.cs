using UnityEngine;

public class AimGun : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private PlayerMovement player; // reference to player

    [Header("Aiming Settings")]
    [SerializeField] private Vector2 aimDirection = Vector2.right;

    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;

        // Compute direction from gun to mouse
        Vector2 direction = mousePos - transform.position;

        // Rotate gun toward mouse
        SetAimDirection(direction);

        // Flip based on player's facing direction
        if (player != null)
        {
            Vector3 scale = transform.localScale;
            scale.x = !player.WatchRight ? Mathf.Abs(scale.x) : -Mathf.Abs(scale.x);
            transform.localScale = scale;
        }
    }

    public void SetAimDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.1f)
        {
            aimDirection = dir.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
}

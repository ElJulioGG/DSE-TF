using UnityEngine;

public class AimGun : MonoBehaviour
{
    [SerializeField] private Vector2 aimDirection = Vector2.right;

    void Update()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Compute direction from gun to mouse
        Vector2 direction = mousePos - transform.position;

        // Call your existing aim function
        SetAimDirection(direction);
    }

    public void SetAimDirection(Vector2 dir)
    {
        if (dir.sqrMagnitude > 0.1f)
        {
            aimDirection = dir.normalized;
            float angle = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Rotate gun to face the mouse
            transform.rotation = Quaternion.Euler(0, 0, angle);

            // Flip vertically when aiming left
            Vector3 scale = transform.localScale;
            scale.y = dir.x < 0 ? -Mathf.Abs(scale.y) : Mathf.Abs(scale.y);
            transform.localScale = scale;
        }
    }
}

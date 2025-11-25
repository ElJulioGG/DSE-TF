using UnityEngine;
using DG.Tweening;

public class CroshairFollowMouse : MonoBehaviour
{
    [SerializeField] private float rotationSpeed = 180f; // degrees per second
    private RectTransform rect;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    private void Start()
    {
        // Infinite rotation loop
        rect.DORotate(
            new Vector3(0, 0, -360f),
            360f / rotationSpeed,
            RotateMode.FastBeyond360
        )
        .SetLoops(-1)
        .SetEase(Ease.Linear);
    }

    private void Update()
    {
        // Follow the mouse position
        Vector2 mousePos = Input.mousePosition;

        // Works with all canvas types
        rect.position = mousePos;
    }
}

using UnityEngine;
using DG.Tweening;

public class UITweens : MonoBehaviour
{
    private RectTransform rect;
    private Tween shakeLoop;

    private void Awake()
    {
        rect = GetComponent<RectTransform>();
    }


    public void StartShakeLoop(float dur, float str, int vib, float rand)
    {
        // If a shake is already running, ignore
        if (shakeLoop != null && shakeLoop.IsActive() && shakeLoop.IsPlaying())
            return;

        // Kill any tween just in case
        shakeLoop?.Kill();

        shakeLoop = rect.DOShakeAnchorPos(dur, str, vib, rand, false) // fadeOut = false
                        .SetLoops(-1)
                        .SetAutoKill(false);
    }

 
    public void StopShakeLoop()
    {
        if (shakeLoop != null)
        {
            shakeLoop.Kill();
            shakeLoop = null;
        }
    }

    private void OnDisable()
    {
        StopShakeLoop();
    }

    private void OnDestroy()
    {
        StopShakeLoop();
    }
}

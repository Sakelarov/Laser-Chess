using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class UIEffects : MonoBehaviour
{
    public static float popUpScaleFactor = 0.7f;
    public static float popUpScaleTime = 0.7f;
    public static float fadeTime = 0.45f;
    public static float animationTime = 0.6f;
    public static float scaleAnimationTime = 0.3f;
    public static float closeFadeTime = 0.7f;

    public static Dictionary<GameObject, string> playingObjects = new Dictionary<GameObject, string>();

    public static void PanelOpenTransition(GameObject panel, int speed = 1, float delay = 0, Action onComplete = null, Vector3? oldScale = null, bool independentUpdate = false)
    {
        if (oldScale == null)
        {
            oldScale = new Vector3(1, 1, 1);
        }
        if (!playingObjects.ContainsKey(panel))
        {
            playingObjects.Add(panel, "opening");
        }
        playingObjects[panel] = "opening";
        RectTransform panelTransfrom = panel.GetComponent<RectTransform>();
        panelTransfrom.DOKill();
        if (panelTransfrom != null)
        {
            panelTransfrom.localScale = (Vector3)oldScale;

            panel.SetActive(true);
            if (panel.TryGetComponent(out CanvasGroup canvasGroup))
            {
                canvasGroup.DOKill();
                canvasGroup.alpha = 0;
                canvasGroup.DOFade(1, fadeTime / (2 * speed))
                    .SetDelay(delay).SetUpdate(independentUpdate)
                    .SetLink(panel, LinkBehaviour.KillOnDestroy);
            }

            panelTransfrom.DOScale(new Vector3(0.3f, 0.3f, 0.3f), animationTime / speed)
                .From()
                .SetEase(Ease.OutBack)
                .SetDelay(delay)
                .SetUpdate(independentUpdate)
                .OnComplete(() => onComplete?.Invoke())
                .SetLink(panel, LinkBehaviour.KillOnDestroy);
        }
    }

    public static void PanelCloseTransition(GameObject panel, int speed = 1, float delay = 0, Action onComplete = null, Vector3? oldScale = null, bool independentUpdate = false)
    {
        if (oldScale == null)
        {
            oldScale = new Vector3(1, 1, 1);
        }
        RectTransform panelTransfrom = panel.GetComponent<RectTransform>();
        if (panelTransfrom != null)
        {
            var canvasGroup = panel.GetComponent<CanvasGroup>();
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1;
                canvasGroup.DOFade(0, fadeTime / (2 * speed))
                    .SetDelay(delay).SetUpdate(independentUpdate)
                    .SetLink(panel, LinkBehaviour.KillOnDestroy);
            }
            if (!playingObjects.ContainsKey(panel))
            {
                playingObjects.Add(panel, "closing");
            }
            playingObjects[panel] = "closing";
           
            panelTransfrom.DOScale(new Vector3(0.5f, 0.5f, 0.5f), animationTime / speed)
                .SetEase(Ease.OutBack)
                .SetDelay(delay)
                .SetUpdate(independentUpdate)
                .OnComplete(() =>
                {
                    if (playingObjects[panel] == "opening") return;
                    panel.SetActive(false);
                    panelTransfrom.localScale = (Vector3)oldScale;
                    if (onComplete != null) onComplete();
                    playingObjects[panel] = "closed";
                })
                .SetLink(panel, LinkBehaviour.KillOnDestroy);
        }
    }
}
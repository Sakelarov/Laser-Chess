using System;
using DG.Tweening;
using UnityEngine;

public class Popup : MonoBehaviour
{
    [SerializeField] private CanvasGroup overlay;
    [SerializeField] private GameObject background;

    public void OpenPopup(Action onComplete)
    {
        overlay.gameObject.SetActive(true);
        DOVirtual.Float(0, 1, 0.2f, (value) => overlay.alpha = value).SetUpdate(true).OnComplete(() => onComplete?.Invoke());
        UIEffects.PanelOpenTransition(background,1, 0, null, null, true);
    }

    public void ClosePopup(Action onComplete)
    {
        DOVirtual.Float(1, 0, 0.2f, (value) => overlay.alpha = value).SetUpdate(true)
            .OnComplete(() =>
            {
                overlay.gameObject.SetActive(false);
                onComplete?.Invoke();
            });
        UIEffects.PanelCloseTransition(background, 1, 0, null, null, true);
    }
}

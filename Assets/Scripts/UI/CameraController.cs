using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CameraController : MonoBehaviour
{
    private static CameraController instance;

    public static CameraController Instance
    {
        get
        {
            if (instance == null)
            {
                var inst = FindObjectOfType<CameraController>();
                instance = inst;
            }

            return instance;
        }
    }

    [SerializeField] private GameObject mainMenu;

    private readonly Vector3 mainMenuPosition = new Vector3(0, 20, -40);
    private readonly float mainMenuXRot = 25;

    private readonly Vector3 gameViewPosition = new Vector3(0, 7.3f, -9.9f);
    private readonly float gameViewXRot = 40;

    public enum CameraPosition { MainMenu, GameView}

    private Sequence rotation;
    private Camera cam;
    
    private void Start()
    {
        cam = Camera.main;

        StartMainMenuAnimation();
    }

    private void StartMainMenuAnimation()
    {
        rotation = DOTween.Sequence();
        rotation.Append(DOVirtual.Float(0, 360, 40, value => { transform.rotation = Quaternion.Euler(0, value, 0); })
            .SetEase(Ease.Linear));
        rotation.SetLoops(-1, LoopType.Restart);
    }

    public void GoToPosition(CameraPosition position, Action onComplete)
    {
        switch (position)
        {
            case CameraPosition.MainMenu:
            {
                DOVirtual.Vector3(cam.transform.localPosition, mainMenuPosition, 1, value => { cam.transform.localPosition = value; })
                    .SetEase(Ease.InOutSine);

                DOVirtual.Float(cam.transform.eulerAngles.x, mainMenuXRot, 1, value => { cam.transform.eulerAngles = new Vector3(value, 0, 0); }).SetEase(Ease.InOutSine)
                    .OnComplete(() =>
                    {
                        StartMainMenuAnimation();
                        onComplete?.Invoke();
                    });
                break;
            }
            case CameraPosition.GameView:
            {
                rotation.Kill();
                
                DOVirtual.Vector3(cam.transform.localPosition, gameViewPosition, 1, value => { cam.transform.localPosition = value; })
                    .SetEase(Ease.InOutSine);

                var startAngle = transform.eulerAngles.y;
                if (startAngle > 180) startAngle -= 360;
                
                DOVirtual.Float(startAngle, 0, 0.75f, value => transform.eulerAngles = new Vector3(0, value, 0))
                    .SetEase(Ease.Linear)
                    .OnComplete(() =>
                    {
                        DOVirtual.Float(cam.transform.eulerAngles.x, gameViewXRot, 0.25f, value => { cam.transform.eulerAngles = new Vector3(value, 0, 0); })
                            .SetEase(Ease.Linear)
                            .OnComplete(() => onComplete?.Invoke());
                    });
                break;
            }
        }
    }
}

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
    
    private Camera cam;
    
    private void Start()
    {
        cam = Camera.main;

        StartMainMenuAnimation();
    }

    private void StartMainMenuAnimation()
    {
        DOVirtual.Float(0, 360, 40, value =>
            {
                transform.rotation = Quaternion.Euler(0, value, 0);
            }).SetEase(Ease.Linear)
            .SetLoops(-1, LoopType.Restart)
            .SetLink(mainMenu, LinkBehaviour.KillOnDisable);
    }
}

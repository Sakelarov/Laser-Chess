using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] private GameObject title;
    [SerializeField] private GameObject madeBy;
    
    [SerializeField] private Button[] levelBtns;
    [SerializeField] private Button customLevelsBtn;
    [SerializeField] private Button creditsBtn;
    [SerializeField] private Button exitBtn;

    private RectTransform[] leftBtns = new RectTransform[3];
    private RectTransform[] rightBtns = new RectTransform[3];
    private readonly Vector3 reducedScale = new Vector3(1, 0.5f, 1);
    private readonly float offset = 400;

    private void Start()
    {
        AttachListeners();

        leftBtns[0] = levelBtns[0].GetComponent<RectTransform>();
        leftBtns[1] = levelBtns[1].GetComponent<RectTransform>();
        leftBtns[2] = levelBtns[2].GetComponent<RectTransform>();
        
        rightBtns[0] = customLevelsBtn.GetComponent<RectTransform>();
        rightBtns[1] = creditsBtn.GetComponent<RectTransform>();
        rightBtns[2] = exitBtn.GetComponent<RectTransform>();
    }

    private void AttachListeners()
    {
        for (int i = 0; i < levelBtns.Length; i++)
        {
            var y = i;
            levelBtns[i].onClick.AddListener(() =>
            {
                CloseMainMenu(() =>  BoardManager.Instance.StartLevel((BoardManager.LevelType)(y + 1)));
            });
        }
        
        customLevelsBtn.onClick.AddListener(OpenCustomLevelsPopUp);
        creditsBtn.onClick.AddListener(OpenCredits);
        exitBtn.onClick.AddListener(Application.Quit);
    }

    private void CloseMainMenu(Action onComplete)
    {
        madeBy.SetActive(false);
        UIEffects.PanelCloseTransition(title, 2);

        DOVirtual.Vector3(Vector3.one, reducedScale, 0.15f, value =>
        {
            foreach (var tr in leftBtns) tr.localScale = value;
            foreach (var tr in rightBtns) tr.localScale = value;
        });

        DOVirtual.Float(0, offset, 0.3f, value =>
        {
            foreach (var tr in leftBtns)
            {
                tr.offsetMax = new Vector2(-value, 0);
                tr.offsetMin = new Vector2(-value, 0);
            }

            foreach (var tr in rightBtns)
            {
                tr.offsetMax = new Vector2(value, 0);
                tr.offsetMin = new Vector2(value, 0);
            }
        }).OnComplete(() =>
        {
            CameraController.Instance.GoToPosition(CameraController.CameraPosition.GameView, onComplete);
        });
    }

    private void OpenCustomLevelsPopUp()
    {
        
    }

    private void OpenCredits()
    {
        
    }
}
using UnityEngine;
using UnityEngine.UI;

public class CreditsPopup : Popup
{
    [SerializeField] private Button closeBtn;

    private void Start()
    {
        closeBtn.onClick.AddListener(() => ClosePopup(null));
    }
}

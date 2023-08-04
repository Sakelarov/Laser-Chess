using DG.Tweening;
using TMPro;
using UnityEngine;

public class DamageReduction : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI value;
    private Camera cam;
    private CanvasGroup cg;
    
    public void Setup(int amount)
    {
        cam = Camera.main;
        Canvas canvas = GetComponent<Canvas>();
        canvas.worldCamera = cam;

        cg = GetComponent<CanvasGroup>();
        value.text = $"-{amount}";

        Vector3 psn = transform.position;
        float yPos = psn.y;

        DOVirtual.Float(1, 0, 1.5f, v => cg.alpha = v)
            .SetEase(Ease.Linear);
        DOVirtual.Float(0,2, 1.5f, v => transform.position = new Vector3(psn.x, yPos + v, psn.z))
            .SetEase(Ease.Linear)
            .OnComplete(() => Destroy(gameObject));
    }
    private void LateUpdate()
    {
        transform.LookAt(transform.position + cam.transform.rotation * Vector3.forward, cam.transform.rotation * Vector3.up);
    }
}

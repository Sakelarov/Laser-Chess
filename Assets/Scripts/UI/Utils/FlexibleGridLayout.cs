using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class FlexibleGridLayout : GridLayoutGroup
{
    [SerializeField] private bool deactivateOnceSet = false;
    [SerializeField] private float childAspectRation = 1f;
    public int fixedRows = 0;
    public int fixedColumns = 0;

    private bool horizontalSet = false;
    private bool verticalSet = false;

    public override void SetLayoutHorizontal()
    {
        UpdateCellSize();
        base.SetLayoutHorizontal();
        horizontalSet = true;
        DeactivateComponent();
    }

    public override void SetLayoutVertical()
    {
        UpdateCellSize();
        base.SetLayoutVertical();
        verticalSet = true;
        DeactivateComponent();
    }

    private void DeactivateComponent()
    {
        if (deactivateOnceSet && verticalSet && horizontalSet)
        {
            if (TryGetComponent(out ContentSizeFitter fitter)) fitter.enabled = false;
            enabled = false;
        }
    }

    private void UpdateCellSize()
    {
        if (childAspectRation == 0) return;
        var width = 100f;
        var height = 100f;
        if (this.constraint == Constraint.FixedColumnCount)
        {
            width = (rectTransform.rect.size.x - padding.horizontal - spacing.x * (this.constraintCount - 1)) / this.constraintCount;
            height = width / childAspectRation;
        }
        else if (this.constraint == Constraint.FixedRowCount)
        {
            height = (rectTransform.rect.size.y - padding.vertical - spacing.y * (this.constraintCount - 1)) / (float)this.constraintCount;
            width = height * childAspectRation;
        }
        else
        {
            width = (rectTransform.rect.size.x - spacing.x - padding.horizontal * (this.fixedColumns - 1)) / this.fixedColumns;
            height = (rectTransform.rect.size.y - spacing.y - padding.vertical * (this.fixedRows - 1)) / (float)this.fixedRows;
        }

        this.cellSize = new Vector2(width, height);
    }
}

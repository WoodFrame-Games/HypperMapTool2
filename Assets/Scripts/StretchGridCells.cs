using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(GridLayoutGroup))]
public class StretchGridCells : MonoBehaviour
{
    [SerializeField] private int columns = 1;      // колонок в сетке
    [SerializeField] private float extraWidth = 0f; // насколько шире вьюпорта (за экран)

    private GridLayoutGroup grid;
    private RectTransform rect;

    private void Awake()
    {
        grid = GetComponent<GridLayoutGroup>();
        rect = (RectTransform)transform;
        UpdateCells();
    }

    // Вызывается при любом изменении размеров RectTransform (анкоры растяжки и т.п.)
    private void OnRectTransformDimensionsChange()
    {
        UpdateCells();
    }

    private void UpdateCells()
    {
        if (grid == null || rect == null || columns < 1)
            return;

        float width = rect.rect.width + extraWidth;

        RectOffset padding = grid.padding;
        float usable = width - padding.left - padding.right - grid.spacing.x * (columns - 1);

        grid.cellSize = new Vector2(usable / columns, grid.cellSize.y);
    }
}

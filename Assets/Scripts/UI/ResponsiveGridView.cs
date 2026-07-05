using UnityEngine;
using UnityEngine.UI;

public class ResponsiveGridView : MonoBehaviour
{
    [SerializeField] private GridLayoutGroup grid;
    [SerializeField] private RectTransform viewport;

    [Header("Columns")]
    [SerializeField] private int portraitColumns = 2;
    [SerializeField] private int landscapeColumns = 4;

    [Header("Cell")]
    [SerializeField] private float cellAspect = 1.6f; 
    // width / height. 1.6 = широкая карточка

    [SerializeField] private Vector2 spacing = new Vector2(8f, 8f);

    private Vector2 lastViewportSize;

    private void Reset()
    {
        grid = GetComponent<GridLayoutGroup>();
    }

    private void OnEnable()
    {
        Apply();
    }

    private void Update()
    {
        if (viewport == null)
            return;

        Vector2 size = viewport.rect.size;

        if (size == lastViewportSize)
            return;

        Apply();
    }

    private void Apply()
    {
        if (grid == null || viewport == null)
            return;

        lastViewportSize = viewport.rect.size;

        bool isLandscape = Screen.width > Screen.height;
        int columns = isLandscape ? landscapeColumns : portraitColumns;

        grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
        grid.constraintCount = columns;
        grid.spacing = spacing;

        float totalWidth = viewport.rect.width;

        float horizontalPadding =
            grid.padding.left + grid.padding.right;

        float totalSpacing =
            spacing.x * (columns - 1);

        float cellWidth =
            (totalWidth - horizontalPadding - totalSpacing) / columns;

        float cellHeight =
            cellWidth / cellAspect;

        grid.cellSize = new Vector2(cellWidth, cellHeight);
    }
}

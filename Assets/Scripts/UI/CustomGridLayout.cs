using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class CustomGridLayout : LayoutGroup
{
    protected override void OnEnable()
    {
        CalculateLayoutInputHorizontal();
    }
    public enum FixedType { Rows, Columns };
    public enum Alignments { Horizontal, Vertical };

    [Header("Properties")]
    //[SerializeField] bool fixedNumberOfRows;
    //[SerializeField] bool fixedNumberOfColumns;
    [SerializeField] FixedType fixedType;
    [SerializeField] int rowsCount = 1;
    [SerializeField] int columnsCount = 1;
    [SerializeField] bool fixedCellRatio;
    [SerializeField] Vector2 spacing;
    [SerializeField] Alignments alignment;

    [Header("Cell Ratio")]
    [Tooltip("(Width / Height) Used when cell ratio is fixed")]
    [SerializeField] float cellRatio = 1;
    [Space]
    [Tooltip("Used when cell ratio is not fixed")]
    [SerializeField] List<float> rowsRatio = new List<float>();
    [Tooltip("Used when cell ratio is not fixed")]
    [SerializeField] List<float> columnsRatio = new List<float>();

    private int rows, columns;
    public override void CalculateLayoutInputHorizontal()
    {
        base.CalculateLayoutInputHorizontal();
        if (!fixedCellRatio)
        {
            if (rowsRatio.Count == 0 || columnsRatio.Count == 0) return;
            foreach (var item in rowsRatio) if (item <= 0) return;
            foreach (var item in columnsRatio) if (item <= 0) return;
        }
        float TotalRowsRatio = 0, TotalColumsRatio = 0;
        if (!fixedCellRatio)
        {
            rows = rowsRatio.Count; columns = columnsRatio.Count;
            if (rows == 0 || columns == 0) return;
            foreach (var item in rowsRatio) TotalRowsRatio += item;
            foreach (var item in columnsRatio) TotalColumsRatio += item;
        }
        else
        {
            rows = 0; columns = 0;
            if (fixedType == FixedType.Rows) rows = rowsCount;
            else columns = columnsCount;
            if(rows != 0 && columns == 0)
            {
                columns = Mathf.CeilToInt((float)rectChildren.Count / rows);
            }
            else if(rows == 0 && columns != 0)
            {
                rows = Mathf.CeilToInt((float)rectChildren.Count / columns);
            }
            else
            {
                if(alignment == Alignments.Horizontal)
                {
                    columns = Mathf.CeilToInt(Mathf.Sqrt((float)rectChildren.Count));
                    rows = Mathf.CeilToInt((float)rectChildren.Count / columns);
                }
                else
                {
                    rows = Mathf.CeilToInt(Mathf.Sqrt((float)rectChildren.Count));
                    columns = Mathf.CeilToInt((float)rectChildren.Count / rows);
                }
            }
        }

        float parentWidth = rectTransform.rect.width, parentHeight = rectTransform.rect.height;
        float cellWidthDelta = spacing.x * (columns - 1)/columns + padding.left/columns + padding.right/columns, cellHeightDelta = spacing.y * (rows - 1)/rows + padding.top/rows + padding.bottom/rows;
        List<List<Vector2>>CellSize = new List<List<Vector2>>();
        if (!fixedCellRatio)
        {
            for(int i=0; i<rows; i++)
            {
                CellSize.Add(new List<Vector2>());
                for(int j=0; j<columns; j++)
                {
                    CellSize[i].Add(new Vector2(parentWidth / TotalColumsRatio * columnsRatio[j] - cellWidthDelta, parentHeight / TotalRowsRatio * rowsRatio[i] - cellHeightDelta)); //[width, height]
                }
            }
        }
        else
        {
            if (fixedType == FixedType.Rows)
            {
                rectTransform.sizeDelta = new Vector2(((parentHeight - padding.top - padding.bottom - spacing.y * (rows - 1)) / rows * cellRatio * columns + padding.left + padding.right + spacing.x * (columns - 1)), rectTransform.sizeDelta.y);
                float SameHeight = parentHeight / rows - cellHeightDelta;
                float SameWidth = SameHeight * cellRatio;
                for (int i = 0; i < rows; i++)
                {
                    CellSize.Add(new List<Vector2>());
                    for (int j = 0; j < columns; j++)
                    {
                        CellSize[i].Add(new Vector2(SameWidth, SameHeight)); //[width, height]
                    }
                }
            }
            else if (fixedType == FixedType.Columns)
            {
                rectTransform.sizeDelta = new Vector2(rectTransform.sizeDelta.x, ((((parentWidth - padding.left - padding.right - spacing.x * (columns - 1)) / columns) / cellRatio) * rows + padding.top + padding.bottom + spacing.y * (rows - 1)));
                float SameWidth = parentWidth / columns - cellWidthDelta;
                float SameHeight = SameWidth / cellRatio;
                for (int i = 0; i < rows; i++)
                {
                    CellSize.Add(new List<Vector2>());
                    for (int j = 0; j < columns; j++)
                    {
                        CellSize[i].Add(new Vector2(SameWidth, SameHeight)); //[width, height]
                    }
                }
            }
        }

        int rowIndex = 0, columnIndex = 0;

        for(int i=0; i<rectChildren.Count; i++)
        //for(int i=0; i<ObjectsInCell.Count; i++)
        {
            float CellOffsetX = 0; //parentWidth / TotalColumsRatio * ColumnsRatio[columnIndex] - cellWidthDelta;
            float CellOffsetY = 0; // parentHeight / TotalRowsRatio * RowsRatio[rowIndex] - cellHeightDelta;
            
            for(int j=0; j<columnIndex; j++)
            {
                CellOffsetX += CellSize[rowIndex][j].x;
            }
            for(int j=0; j<rowIndex; j++)
            {
                CellOffsetY += CellSize[j][columnIndex].y;
            }

            //var item = ObjectsInCell[i].GetComponent<RectTransform>();
            var item = rectChildren[i];
            float xPos = CellOffsetX + spacing.x * columnIndex + padding.left, yPos = CellOffsetY + spacing.y * rowIndex + padding.top;

            SetChildAlongAxis(item, 0, xPos, CellSize[rowIndex][columnIndex].x);
            SetChildAlongAxis(item, 1, yPos, CellSize[rowIndex][columnIndex].y);

            if (alignment == Alignments.Horizontal)
            {
                columnIndex++;
                if(columnIndex >= columns)
                {
                    rowIndex++; columnIndex %= columns;
                }
            }
            else
            {
                rowIndex++;
                if(rowIndex >= rows)
                {
                    columnIndex++; rowIndex %= rows;
                }
            }
        }
    }
    public override void CalculateLayoutInputVertical(){}

    public override void SetLayoutHorizontal(){}

    public override void SetLayoutVertical(){}
}

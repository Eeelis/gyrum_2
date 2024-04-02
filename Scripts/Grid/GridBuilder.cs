using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GridBuilder : MonoBehaviour
{
    [Space(10)]
    [Header("Variables")]
    [SerializeField] public int size;
    [Space(10)]
    [Header("References")]
    [SerializeField] private GameObject gridContainer;
    [SerializeField] private GameObject borderPrefab;
    [SerializeField] private Cell cellPrefab;
    
    private Cell[,] grid;

    public event Action<Cell[,]> OnGridGenerated;

    private void Start()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        grid = new Cell[size, size];

        for (int x = 0; x < size; x++)
        {
            for (int y = 0; y < size; y++)
            {
                Cell newCell = Instantiate(cellPrefab, new Vector3(x, y), Quaternion.identity);
                newCell.transform.SetParent(gridContainer.transform);
                newCell.name = $"Cell {x} {y}";

                grid[x, y] = newCell;
            }
        }

        PlaceBorders();

        OnGridGenerated?.Invoke(grid);
    }

    private void PlaceBorders()
    {
        borderPrefab.transform.localScale = new Vector2(size + 1f, 1f);

        Instantiate(borderPrefab, Vector2.zero + new Vector2(size / 2f - 1f, -1f), Quaternion.identity, transform);
        Instantiate(borderPrefab, Vector2.zero + new Vector2(size / 2f, size), Quaternion.identity, transform);
        Instantiate(borderPrefab, Vector2.zero + new Vector2(-1f, size / 2f), Quaternion.Euler(new Vector3(0, 0, 90)), transform);
        Instantiate(borderPrefab, Vector2.zero + new Vector2(size, size / 2f - 1f), Quaternion.Euler(new Vector3(0, 0, 90)), transform);
    }

    public Cell GetCellAtPosition(Vector2 pos)
    {
        Vector2 localPos = gridContainer.transform.InverseTransformPoint(pos);

        // Round the position to the nearest integer to get the grid indices
        int x = Mathf.RoundToInt(localPos.x);
        int y = Mathf.RoundToInt(localPos.y);

        // Check if the indices are within valid range
        if (x >= 0 && x < size && y >= 0 && y < size)
        {
            return grid[x, y];
        }
        else
        {
            return grid[0, 0];
        }
    }
}

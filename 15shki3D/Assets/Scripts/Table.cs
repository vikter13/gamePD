using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class Table : MonoBehaviour
{
    private const int SIZE = 4;

    public event System.Action OnMoveComplete;

    [SerializeField]

    private Cell cellPrefab;

    private Cell[,] table;

    private void Clear()
    {
        var cells = FindObjectsOfType<Cell>();
        foreach (var cell in cells)
            Destroy(cell.gameObject);
        table = new Cell[SIZE, SIZE];
    }

    private int[,] GenerateTable()
    {
        int[,] table = new int[SIZE, SIZE];

        do
        {
            List<int> numbers = Enumerable.Range(1, 15).ToList();
            for (int y = 0; y < SIZE; y++)
            {
                for (int x = 0; x < SIZE; x++)
                {
                    if (x == SIZE - 1 && y == SIZE - 1)
                        continue;

                    int index = Random.Range(0, numbers.Count);
                    table[x,y] = numbers[index];  
                    numbers.RemoveAt(index);
                }
            }
        }


        while (IsSolvable(table.Cast<int>().ToArray()));
        return table;
    }

    private bool IsSolvable(int[] table)
    {
        int countInversion = 0;

        for (int i = 0; i<table.Length; i++)
        {
            for (int j = 0; j<i; j++)
            {
                if (table[j] > table[i])
                    countInversion++;
            }
        }
        return countInversion % 2 == 0;
    }

    private void Generate()
    {
        Clear();
        int[,] table = GenerateTable();

        for (int y = 0; y < SIZE; y++)
        {
            for (int x = 0; x < SIZE; x++)
            {
                if (table[x,y] == 0)
                    break;

                var cell = Instantiate(cellPrefab);
                cell.transform.position = new Vector3(-x, 0f, y);
                cell.Number = table[x,y];
                this.table[x,y] = cell;
            }
        }
    }

    public bool TryMove(Cell cell)
    {
        int x = -Mathf.RoundToInt(cell.transform.position.x);
        int y = Mathf.RoundToInt(cell.transform.position.z);

        List<Vector2Int> dxdy = new List<Vector2Int>()
        {
            new Vector2Int(0,-1),
            new Vector2Int(1,0),
            new Vector2Int(0,1),
            new Vector2Int(-1,0)
        };

        for (int i = 0; i < dxdy.Count; i++) 
        { 
            int xx = x + dxdy[i].x;
            int yy = y + dxdy[i].y;

            if (xx >= 0 && xx < SIZE && yy >= 0 && yy < SIZE)
            {
                if (table[xx,yy] == null)
                {
                    cell.Move(-xx, yy);
                    cell.OnPositionChanged += Cell_OnPositionChanged;
                    return true;
                }
            }
        }
        return false;

    }

    private void Cell_OnPositionChanged(Cell cell, Vector3 prev, Vector3 curr)
    {
        cell.OnPositionChanged -= Cell_OnPositionChanged;

        int x = -Mathf.RoundToInt(prev.x);
        int y = Mathf.RoundToInt(prev.z);
        table[x,y] = null;
        x = -Mathf.RoundToInt(curr.x);
        y = Mathf.RoundToInt(curr.z);
        table[x, y] = cell;

        OnMoveComplete?.Invoke();
    }


    void Start()
    {
        Generate(); 
    }

   
}

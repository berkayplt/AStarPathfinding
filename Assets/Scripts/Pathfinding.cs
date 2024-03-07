using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using System.Linq;

public class Pathfinding: MonoBehaviour
{
    private class Cell
    {
        public int parentX = 0;
        public int parentY = 0;
        public int fCost = int.MaxValue;
        public int gCost = int.MaxValue;
        public int hCost = 0;
        public bool isPathFull = false;
    }

    private class CostCell
    {
        public int x;
        public int y;
        public int fCost;
    }

    public static Pathfinding Instance { get; private set; }
    
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
        else if(Instance != this)
        {
            Destroy(this);
        }
    }

    private bool IsValid(int height, int width, int row, int  col)
    {
        return (row >= 0) && (row < height) && (col >= 0) && (col < width);
        
    }
    private bool IsUnblocked(GridCell[,] grid, int row, int col)
    {
        Debug.Log("X= " + row +" " + "Y= " + col);
        return (grid[row, col].pathCase != Path.BlueSeat && grid[row, col].pathCase != Path.YellowSeat && grid[row, col].pathCase != Path.PinkSeat);

    }
   
    
    private bool IsDestination(int row, int col, GridCell destination)
    {
        return (row == destination.x) && (col == destination.y);
    }
  
    private int CalculateHValue(int row, int col, GridCell destination)
    {
        return (Mathf.Abs(row - destination.x) + Mathf.Abs(col - destination.y));
    }

    private List<GridCell> TracePath(Cell[,] cellList, GridCell destination)
    {
        List<GridCell> path = new List<GridCell>();
        int row = destination.x;
        int col = destination.y;

        while (!(cellList[row,col].parentX == row && cellList[row,col].parentY == col))
        {
           path.Add(new GridCell {x = row, y = col, pathCase = global::Path.None});
            int tempRow = cellList[row, col].parentX;
            int tempCol = cellList[row, col].parentY;
            row = tempRow;
            col = tempCol;
        }
        path.Add(new GridCell { x = row, y = col });
        path.Reverse();
        return path;

    }

    public List<GridCell> FindPath(Vector2Int currentPos, Vector2Int targetPos)
    {
       GridCell[,] grid = GridManager.Instance.GetGridCell();

        //Set current and target Positin with Vector2Int
        var source = new GridCell { x = currentPos.x, y = currentPos.y };
        var destination = new GridCell { x = targetPos.x, y = targetPos.y};

        bool[,] closedList = new bool[grid.GetLength(0), grid.GetLength(1)];

        Cell[,] cellDetail = new Cell[grid.GetLength(0), grid.GetLength(1)];
        for (int k = 0; k<cellDetail.GetLength(0);k++)
        {
            for (int l = 0; l<cellDetail.GetLength(1);l++)
            {
                cellDetail[k, l] = new Cell();
            }
        }

        int i = source.x;
        int j = source.y;

        cellDetail[i, j].fCost = 0;
        cellDetail[i, j].gCost = 0;
        cellDetail[i, j].hCost = 0;
        cellDetail[i, j].parentX = i;
        cellDetail[i, j].parentY = j;

        List<CostCell> openList = new List<CostCell>();
        
        openList.Add(new CostCell { x = i, y = j, fCost = cellDetail[i, j].fCost});

        while (openList.Count > 0)
        {
            int minF = openList.Min(obj => obj.fCost);
            CostCell minCell = openList.FirstOrDefault(obj => obj.fCost == minF);
            openList.Remove(minCell);
            i = minCell.x;
            j = minCell.y;
            closedList[i, j] = true;

            List<Vector2Int> directions = new List<Vector2Int>
            {
                new Vector2Int(0,1),
                new Vector2Int(0,-1),
                new Vector2Int(1,0),
                new Vector2Int(-1,0) 
            };
            foreach (var dir in directions)
            {
                int new_i = i + dir.x;
                int new_j = j + dir.y;
                
                if (IsValid(grid.GetLength(0), grid.GetLength(1), new_i, new_j) && IsUnblocked(grid, new_i, new_j) && !closedList[new_i, new_j])
                {
                   
                   if(IsDestination(new_i, new_j,destination))
                   {
                       
                       cellDetail[new_i, new_j].parentX = i;
                       cellDetail[new_i, new_j].parentY = j;
                        
                       return TracePath(cellDetail,destination);
                   }
                   else
                   {
                       int g_new = cellDetail[i, j].gCost + 1;
                       int h_new = CalculateHValue(new_i, new_j, destination);
                       int f_new = g_new + h_new;

                       if((cellDetail[new_i, new_j].fCost == int.MaxValue || cellDetail[new_i, new_j].fCost > f_new))
                       {
                           openList.Add(new CostCell { x = new_i, y= new_j, fCost = f_new});

                           cellDetail[new_i, new_j].fCost = f_new;
                           cellDetail[new_i, new_j].gCost = g_new;
                           cellDetail[new_i, new_j].hCost = h_new;
                           cellDetail[new_i, new_j].parentX = i; 
                           cellDetail[new_i, new_j].parentY = j;
                       }
                   }
                }
            }
        }
        return new List<GridCell>();

    }

  
}



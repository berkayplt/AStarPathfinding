using System.Collections.Generic;
using System;
using System.Collections;
using UnityEngine;
using _Main.Scripts.Utilities.Singletons;
using Object = UnityEngine.Object;

public class SeatedGrids
{
    public int x;
    public int y;
}

public class GridManager : MonoBehaviour
{
    public static GridManager Instance;
    
    [SerializeField] private int width = 10;
    [SerializeField] private int height = 10;
    [SerializeField] private float cellSize = 1f;
    [SerializeField] private GameObject cellPrefab;
    [SerializeField] private GridCell[,] gridArray;

    [SerializeField] private List<Vector3> blueSeatPositions;
    [SerializeField] private List<Vector3> pinkSeatPositions;
    [SerializeField] private List<Vector3> yellowSeatPositions;
  
    [SerializeField] private bool isMoveable = true;

    List<Vector2Int> directions = new List<Vector2Int>
    {
        new Vector2Int(0,1), //Up
        new Vector2Int(1,0), //Right
        new Vector2Int(-1,0) //Left
    };

    private void Awake()
    {
        Instance = this;
        gridArray = new GridCell[width, height];
    }

    void Start()
    {
        Application.targetFrameRate = 60;
        CreateGrid();
    }

    public void SetIsMoveable(bool ismove)
    {
        isMoveable = ismove;
    }

    public bool GetIsMoveable()
    {
        return isMoveable;
    }

    void CreateGrid()
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellPosition = new Vector3(x * cellSize, 0, y * cellSize);

                GridCell cellObject =
                    Instantiate(cellPrefab, cellPosition, Quaternion.identity).GetComponent<GridCell>();
                gridArray[x, y] = cellObject.InitializeGrid(x, y, Path.None);
                for (int i = 0; i < blueSeatPositions.Count; i++)
                {
                    if (x == blueSeatPositions[i].x && y == blueSeatPositions[i].z && blueSeatPositions[i] != null)
                    {
                        gridArray[x, y] = cellObject.InitializeGrid(x, y, Path.BlueSeat);
                    }
                }
                for (int i = 0; i < yellowSeatPositions.Count; i++)
                {
                    if (x == yellowSeatPositions[i].x && y == yellowSeatPositions[i].z && yellowSeatPositions[i] != null)
                    {
                        gridArray[x, y] = cellObject.InitializeGrid(x, y, Path.YellowSeat);
                    }
                }
                for (int i = 0; i < pinkSeatPositions.Count; i++)
                {
                    if (x == pinkSeatPositions[i].x && y == pinkSeatPositions[i].z && pinkSeatPositions[i] != null)
                    {
                        gridArray[x, y] = cellObject.InitializeGrid(x, y, Path.PinkSeat);
                    }
                }
            }
        }
        
        CalculateNeighbours();
    }

    private void CalculateNeighbours()
    {
        //Gridlerin butun yonlerine bak, eger baktigin yonlerde grid var ise bunlari listeye ekle
        var foundedNeighbours = new List<GridCell>();

        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                var grid = gridArray[x,y];
                var neighbours = new List<GridCell>();
                foreach (var direction in directions)
                {
                    Vector2Int gridCoordinates = new Vector2Int(grid.x, grid.y);
                    var targetNeighbourCoordinates = gridCoordinates + direction;
                    GridCell neighbourTile;
                    if (HasTile(targetNeighbourCoordinates, out neighbourTile))
                    {
                        neighbours.Add(neighbourTile);
                    }
                }
                grid.SetNeighbours(neighbours);
            }
        }
      
    }

    private bool HasTile(Vector2Int coordinates, out GridCell tile)
    {
        tile = null;
        for (int x = 0; x < gridArray.GetLength(0); x++)
        {
            for (int y = 0; y < gridArray.GetLength(1); y++)
            {
                var currentGridCoordinates = new Vector2Int(gridArray[x, y].x, gridArray[x, y].y);
                if (currentGridCoordinates == coordinates)
                {
                    tile = gridArray[x, y];
                    return true;
                }
            }
        }
        return false;
    }
    

    public GridCell[,] GetGridCell()
    {
        return gridArray;
    }

    public void SearchClosestGridForSeatDown(Vector3 _seatPos, bool isSeatFull, SeatScript seatObject, Path path)
    {
        GridCell closestObject = gridArray[(int)_seatPos.x, (int)_seatPos.z]; 
        float minDistance = float.MaxValue;

        closestObject.pathCase = path;
        closestObject.SetSeatGameObject(seatObject);
        closestObject.SetIsSeatFull(isSeatFull);
      
    }

    
    public void SetGridNoneForSeat(Vector3 _seatPos)
    {
    
     GridCell currentGrid = gridArray[(int)_seatPos.x, (int)_seatPos.z]; 
        
       currentGrid.pathCase = Path.None;
       if(currentGrid.GetSeatGameObject() != null)
       {
           currentGrid.SetIsSeatFull(false);
           currentGrid.SetSeatGameObject(null);
          
       }
    }

    
    //********************************************************TEST NEW***************************************************************************************
    
    private readonly Dictionary<Direction, Vector3Int> _directions = new()
    {
        { Direction.Left, new Vector3Int(-1, 0, 0) },
        { Direction.LeftUp, new Vector3Int(-1, 0, 1) },
        { Direction.Up, new Vector3Int(0, 0, 1) },
        { Direction.RightUp, new Vector3Int(1, 0, 1) },
        { Direction.Right, new Vector3Int(1, 0, 0) },
        { Direction.RightDown, new Vector3Int(1, 0, -1) },
        { Direction.Down, new Vector3Int(0, 0, -1) },
        { Direction.LeftDown, new Vector3Int(-1, 0, -1) }
    };
    
    public Vector3 AdjustPos(SeatScript seat, Vector3 pos)
    {
        var gridPos = PosToGrid(pos);

        if (HasSeat(seat, gridPos))
        {
            return seat.GetTargetPos();
        }

        foreach (var (direction, dirVector) in _directions)
        {
            var halfDir = (Vector3)dirVector;
            var dirPos = pos + halfDir;
            var checkPos = PosToGrid(dirPos);
            var foundSeat = gridArray[checkPos.x, checkPos.z];
            var hasSeat = foundSeat != null && foundSeat != seat;
            if(!hasSeat) continue;
            var offset = dirPos - (gridPos + halfDir);
            
            return pos - AdjustOffset(direction, offset);
          
        }

        return pos;
    }
    
    private bool HasSeat(Object seat, Vector3Int gridPos)
    {
        var foundSeat = gridArray[gridPos.x, gridPos.z];
        return foundSeat != null && foundSeat.pathCase != Path.None;
    }
    
    public static Vector3Int PosToGrid(Vector3 pos)
    {
        return new Vector3Int(Mathf.RoundToInt(pos.x), 0, Mathf.RoundToInt(pos.z));
    }
    private static Vector3 AdjustOffset(Direction direction, Vector3 offset)
    {
        switch (direction)
        {
            case Direction.Up or Direction.Down:
                offset.x = 0;
                break;
            case Direction.Left or Direction.Right:
                offset.z = 0;
                break;
            case Direction.LeftDown or Direction.RightDown:
                offset.x = 0;
                break;
            case Direction.LeftUp or Direction.RightUp:
                offset.x = 0;
                break;
        }

        return offset;
    }

    private enum Direction
    {
        Left,
        LeftUp,
        Up,
        RightUp,
        Right,
        RightDown,
        Down,
        LeftDown
    }
    
}
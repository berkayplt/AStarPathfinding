using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEditor;
using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    [SerializeField] private bool moveFlag = false;
    [SerializeField] private bool isOnSeat = false;
    [SerializeField] private  List<GridCell> path = new List<GridCell>();
    [SerializeField] private GridCell[,] gridCells;
    [SerializeField] private Vector2Int currentTargetPos;
    [SerializeField] private Path playerPathSelection;


    private void Awake()
    {
        Events.playerTriggerSetPosition += SetPosition;
        
    }

    

    void Update()
    {
        Movement();
    }


    private void SetPosition()
    {
        Vector2Int currentPos = new Vector2Int((int)transform.position.x, (int)transform.position.z);
        Vector2Int targetNeighborPos = new Vector2Int(0, 0);

        gridCells = GridManager.Instance.GetGridCell();

        foreach (GridCell item in gridCells)
        {
            if (item.pathCase == playerPathSelection && !item.GetIsSeatFull() && !isOnSeat)
            {
               
                currentTargetPos = new Vector2Int(item.x, item.y);
                
                var targetAvailableNeigbourCell = item.GetRandomNeighbourCell();
                if (targetAvailableNeigbourCell == null)
                {
                    
                }
                else
                {
                 
                    Events.playerStartMovement?.Invoke(false);
                    targetNeighborPos = new Vector2Int((int)targetAvailableNeigbourCell.transform.position.x, (int)targetAvailableNeigbourCell.transform.position.z);
                }
                
             
                Debug.Log(targetNeighborPos + " is target Position");
                moveFlag = true;

                break;
            }
            else
            {
                targetNeighborPos = currentPos;
                Debug.Log("Seat not found");
            }

        }

        path = Pathfinding.Instance.FindPath(currentPos, targetNeighborPos);
        
        if (path.Count > 0)
        {
            //Set seat full
            gridCells[currentTargetPos.x, currentTargetPos.y].SetIsSeatFull(true);
            gridCells[currentTargetPos.x, currentTargetPos.y].seatGameObject.SetSeatFull(true);
            
        }
        
        Debug.Log("Path Count is: " + path.Count);

    }



    private void Movement()
    {
        if (moveFlag == true && path.Count > 0 && !isOnSeat)
        {
            int x = path[0].x;
            int y = path[0].y;

            transform.position = Vector3.MoveTowards(transform.position, gridCells[x,y].transform.position, 4 * Time.deltaTime);
            GridManager.Instance.SetIsMoveable(false);
            if (transform.position == gridCells[x, y].transform.position)
            {
                path.RemoveAt(0);

            }
        }
        else
        {
            moveFlag = false;
        }
        
        //if player reaches the target grid
        if (moveFlag == true && path.Count == 0)
        {
            PlayerReachesTarget();
            moveFlag = false;

        }
    }


    private void PlayerReachesTarget()
    {
        GridManager.Instance.SetIsMoveable(true);
        var targetGrid = gridCells[currentTargetPos.x, currentTargetPos.y];
        var targetPosition = new Vector3(targetGrid.transform.position.x,1,targetGrid.transform.position.z);
        var playerTransform = transform;
            
        playerTransform.position = targetPosition;
        playerTransform.parent = targetGrid.GetSeatTransform();
        isOnSeat = true;
     
        Events.playerStopMovement?.Invoke(true);

    }
}
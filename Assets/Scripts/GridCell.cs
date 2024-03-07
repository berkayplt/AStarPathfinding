using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public enum Path
{
    BlueSeat,
    YellowSeat,
    PinkSeat,
    None
}

public class GridCell : MonoBehaviour
{
    public int x;
    public int y;
    public Path pathCase;

    [SerializeField] private int gCost;
    [SerializeField] private int hCost;
    [SerializeField] private int fCost;
    [SerializeField] private bool isSeatFull;
    [SerializeField] private bool isSeatBlocked;
    [SerializeField] private List<GridCell> _neighbours;

    [SerializeField] private Renderer renderer;
    [SerializeField] private Material[] seatMaterials;

   [SerializeField] private GameObject seatPrefab;
   public SeatScript seatGameObject;
   
   
   

    public GridCell InitializeGrid(int x, int y, Path gridBeh)
    {
        this.x = x;
        this.y = y;
        this.pathCase = gridBeh;

        if (pathCase == Path.BlueSeat)
        {
           seatGameObject =  Instantiate(seatPrefab, new Vector3(transform.position.x,transform.position.y + 0.35f,transform.position.z), transform.rotation).GetComponent<SeatScript>();
           seatGameObject.SetSeatColor(seatMaterials[0]);
           seatGameObject.path = Path.BlueSeat;
        }
        else if (pathCase == Path.YellowSeat)
        {
            seatGameObject =  Instantiate(seatPrefab, new Vector3(transform.position.x,transform.position.y + 0.35f,transform.position.z), transform.rotation).GetComponent<SeatScript>();
            seatGameObject.SetSeatColor(seatMaterials[1]);
            seatGameObject.path = Path.YellowSeat;
        }
        else if (pathCase == Path.PinkSeat)
        {
            seatGameObject =  Instantiate(seatPrefab, new Vector3(transform.position.x,transform.position.y + 0.35f,transform.position.z), transform.rotation).GetComponent<SeatScript>();
            seatGameObject.SetSeatColor(seatMaterials[2]);
            seatGameObject.path = Path.PinkSeat;
        }

        return this;
    }

    public GridCell GetRandomNeighbourCell()
    {
        if (_neighbours.Count == 0)
            return null;

        int availableNeighborIndex = 0;

        for (int i = 0; i < _neighbours.Count; i++)
        {
            availableNeighborIndex = Random.Range(0, _neighbours.Count);
            if (!_neighbours[availableNeighborIndex].isSeatFull && (_neighbours[availableNeighborIndex].pathCase != Path.BlueSeat || _neighbours[availableNeighborIndex].pathCase != Path.YellowSeat) || _neighbours[availableNeighborIndex].pathCase != Path.PinkSeat)
            {
                return _neighbours[availableNeighborIndex];
            }
        }


        return null;
    }
   
    
 
   
    public void SetNeighbours(List<GridCell> neighbours)
    {
        _neighbours = neighbours;
    }


    public bool GetIsSeatFull()
    {
        return isSeatFull;
    }

    public void SetIsSeatFull(bool _isSeatFull)
    {
        isSeatFull = _isSeatFull;
    }

    public bool GetSeatGameObject()
    {
        return seatGameObject;
    }

    public Transform GetSeatTransform()
    {
        return seatGameObject.transform;
    }

    public void SetSeatGameObject(SeatScript _seatObject)
    {
        seatGameObject = _seatObject;
    }
   
  

}
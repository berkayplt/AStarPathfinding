using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace;
using UnityEngine;
using UnityEngine.EventSystems;

public class SeatScript : MonoBehaviour,IDragHandler, IPointerDownHandler, IEndDragHandler, IPointerUpHandler
{
    [SerializeField] private bool isSeatFull;

    public Renderer seatRenderer;
    public Path path;
    private GridCell[,] _gridArray;

    private Vector3 screenPoint;
    private Vector3 offset;

    private bool isGoDown;
    private bool isGoUp;
    private bool isGoRight;
    private bool isGoLeft;

    public Vector3 _targetPos;
   
    
    private void Update()
    {
        if(GridManager.Instance.GetIsMoveable())
            transform.position = Vector3.Lerp(transform.position, _targetPos, Time.deltaTime * 20f);
    }
    private void Start()
    {
        _targetPos = transform.position;
    }
    
    public Vector3 GetTargetPos()
    {
        return _targetPos;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (GridManager.Instance.GetIsMoveable())
        {
          _targetPos = GridManager.PosToGrid(transform.position);
          GridManager.Instance.SetGridNoneForSeat(_targetPos);
        }
        
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (GridManager.Instance.GetIsMoveable())
        {
            _targetPos = GridManager.PosToGrid(transform.position);
            GridManager.Instance.SearchClosestGridForSeatDown(_targetPos,isSeatFull,this, path);
            Events.playerTriggerSetPosition?.Invoke();
        }
       
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (GridManager.Instance.GetIsMoveable())
        {
            var seatTransform = transform;
            var seatPos = seatTransform.position;

            var targetPos = CameraController.Instance.ScreenToWorld(eventData.position, seatPos.y);
            targetPos = GridManager.Instance.AdjustPos(this, targetPos);
      
            _targetPos = targetPos;
        }
      
       
    }
    public void OnEndDrag(PointerEventData eventData)
    {
        if (GridManager.Instance.GetIsMoveable())
            _targetPos = GridManager.PosToGrid(transform.position);
    }
    
    public void SetSeatFull(bool _isSeatFull)
    {
        isSeatFull = _isSeatFull;
    }

    public bool GetSeatFull()
    {
        return isSeatFull;
    }

    public void SetSeatColor(Material _material)
    {
        seatRenderer.material = _material;
    }
   
}
   


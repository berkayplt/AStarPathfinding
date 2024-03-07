using System;
using _Main.Scripts.Utilities.Singletons;
using UnityEngine;

namespace DefaultNamespace
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        public static CameraController Instance;
        private Camera _mainCam;

        private void Awake()
        {
            Instance = this;
        }

        private void Start()
        {
            _mainCam = GetComponent<Camera>();
        }

        public Vector3 ScreenToWorld(Vector3 pos, float yPos)
        {
            var plane = new Plane(Vector3.up, new Vector3(0, yPos, 0));
            var rayCast = _mainCam.ScreenPointToRay(pos);
            plane.Raycast(rayCast, out var enterDist);
            return rayCast.GetPoint(enterDist);
        }
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileLogic : MonoBehaviour
{
    public bool IsWalkable = true;
    public bool IsFight = false; // replace with universal "activity" system later
    public Transform PlayerPosition;
    public List<DetectionPoint> DetectionPoints;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void GetMovementMarkersReady()
    {
        foreach (var point in DetectionPoints)
        {
            point.DetectTile();
        }
    }

    public void DisableMovementMarkers()
    {
        foreach (var point in DetectionPoints)
        {
            point.Arrow.SetActive(false);
        }
    }
}

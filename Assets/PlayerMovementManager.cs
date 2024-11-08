using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    public static PlayerMovementManager Instance;
    
    public TileLogic StartingTile;
    private TileLogic currentTile;
    
    void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentTile = StartingTile;
        UpdateToCurrentTile();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTile(TileLogic tile)
    {
        currentTile.DisableMovementMarkers();
        currentTile = tile;
        UpdateToCurrentTile();
    }

    void UpdateToCurrentTile()
    {
        transform.position = currentTile.PlayerPosition.position;
        currentTile.GetMovementMarkersReady();
    }
    
}

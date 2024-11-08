using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class PlayerMovementManager : MonoBehaviour
{
    public static PlayerMovementManager Instance;
    
    public TileLogic StartingTile;
    private TileLogic currentTile;

    public GameObject Character;
    
    void Awake()
    {
        Instance = this;
    }
    
    // Start is called before the first frame update
    void Start()
    {
        currentTile = StartingTile;
        UpdateToCurrentTile(quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void MoveToTile(TileLogic tile, Quaternion lookDirection)
    {
        currentTile.DisableMovementMarkers();
        currentTile = tile;
        UpdateToCurrentTile(lookDirection);
    }

    void UpdateToCurrentTile(Quaternion lookDirection)
    {
        Character.transform.rotation = lookDirection;
        transform.position = currentTile.PlayerPosition.position;
        currentTile.GetMovementMarkersReady();
        
    }
    
}

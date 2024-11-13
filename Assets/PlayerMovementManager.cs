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
    private float movementSpeed = 3f;
    
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
        StartCoroutine(MoveToTileLerp(transform, Character.transform, tile.transform, movementSpeed, lookDirection));
        
    }

    void UpdateToCurrentTile(Quaternion lookDirection)
    {
        Character.transform.rotation = lookDirection;
        transform.position = currentTile.PlayerPosition.position;
        currentTile.GetMovementMarkersReady();
    }
    
    IEnumerator MoveToTileLerp(Transform character, Transform characterModel, Transform targetTile, float speed, Quaternion lookDirection)
    {
        Vector3 startPosition = character.position;
        Vector3 endPosition = targetTile.position;
        float journey = 0f;

        // Initial rotation
        Quaternion startRotation = characterModel.rotation;

        while (journey < 1f)
        {
            journey += Time.deltaTime * speed;

            // Interpolate position
            character.position = Vector3.Lerp(startPosition, endPosition, journey);

            // Interpolate rotation (of only the character model)
            characterModel.rotation = Quaternion.Slerp(startRotation, lookDirection, journey);

            yield return null;
        }

        // Snap to final position and rotation
        character.position = endPosition;
        characterModel.rotation = lookDirection;
        currentTile.GetMovementMarkersReady();
    }
    
}

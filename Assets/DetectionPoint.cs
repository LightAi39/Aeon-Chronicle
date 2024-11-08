using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DetectionPoint : MonoBehaviour
{
    public GameObject Arrow;
    private float radius = 0.01f;
    public bool CanBeWalkedThrough = true;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public GameObject DetectTile()
    {
        if (!CanBeWalkedThrough)
        {
            Arrow.SetActive(false);
            return null;
        }
        
        Collider[] results = new Collider[1];
        var size = Physics.OverlapSphereNonAlloc(transform.position, radius, results);
        
        if (size > 0)
        {
            var obj = results[0].gameObject;
            if (obj.GetComponentInParent<TileLogic>().IsWalkable)
            {
                Arrow.SetActive(true);
            }
            return obj;
        }
        else
        {
            Arrow.SetActive(false);
            return null;
        }
    }

    public void ArrowClicked()
    {
        var obj = DetectTile();
        
        // move character to new tile main point - inside character, set new active tile
        PlayerMovementManager.Instance.MoveToTile(obj.GetComponentInParent<TileLogic>(), transform.rotation);
        
        Debug.Log("moving to tile from collider at " + obj.transform.position);
    }
}

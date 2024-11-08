using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementArrowClickDetector : MonoBehaviour
{
    private DetectionPoint _detectionPoint;
    // Start is called before the first frame update
    void Start()
    {
        _detectionPoint = transform.GetComponentInParent<DetectionPoint>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private void OnMouseDown()
    {
        _detectionPoint.ArrowClicked();
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FindSizeTemp : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Debug.Log(GetComponent<Renderer>().bounds.size);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

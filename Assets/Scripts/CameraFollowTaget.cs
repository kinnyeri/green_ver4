using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollowTaget : MonoBehaviour
{
    public Transform target;
    public Simulation simulation;
    
    float offsetX = -0.03668215f, offsetY = 4.158187f, offsetZ = -6.558564f;

    Vector3 cameraPos;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (simulation.state == Progress.StateLevel.Roll)
        {
            cameraPos.x = target.position.x + offsetX;
            cameraPos.y = target.position.y + offsetY;
            cameraPos.z = target.position.z + offsetZ;
            transform.position = cameraPos;
        }
    }
}

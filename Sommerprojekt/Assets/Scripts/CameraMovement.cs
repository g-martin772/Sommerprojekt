using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public GameObject Player;

    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(Player.transform.position.x >= 5)
            transform.position = new Vector3(Player.transform.position.x - 5, 0, -5);

        if(Player.transform.position.x <= -5)
            transform.position = new Vector3(Player.transform.position.x + 5, 0, -5);


    }
}

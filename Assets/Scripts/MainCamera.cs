using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainCamera : MonoBehaviour
{
    public GameObject player;
    Vector3 distance = new Vector3(1, 1, -10);
    // Start is called before the first frame update

    private void Start()
    {
        Screen.SetResolution(Screen.width, (Screen.width * 16) / 9, true);
    }
    // Update is called once per frame
    void Update()
    {
        if(player)
            transform.position = player.transform.position + distance;
    }
}

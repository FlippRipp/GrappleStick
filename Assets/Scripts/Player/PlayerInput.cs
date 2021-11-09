using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }

    public bool leftMouseButtonPressedDown = false;
    public bool rightMouseButtonPressedDown = false;
    public float vertical;

    // Update is called once per frame
    void Update()
    {
        leftMouseButtonPressedDown = Input.GetButtonDown("Fire1");
        rightMouseButtonPressedDown = Input.GetButtonDown("Fire2");
        vertical = Input.GetAxis("Vertical");
    }
}
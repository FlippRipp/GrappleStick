using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
    }
    public bool useScrollWheel = false;
    public bool leftMouseButtonPressedDown = false;
    public bool rightMouseButtonPressedDown = false;
    public bool rightMouseButtonPressedUp = false;
    public bool leftMouseButtonPressedUp = false;

    public float vertical;

    // Update is called once per frame
    void Update()
    {
        leftMouseButtonPressedDown = Input.GetButtonDown("Fire1");
        leftMouseButtonPressedUp = Input.GetButtonUp("Fire1");
        rightMouseButtonPressedDown = Input.GetButtonDown("Fire2");
        rightMouseButtonPressedUp= Input.GetButtonUp("Fire2");
        vertical = Input.GetAxis("Vertical");
        if (useScrollWheel)
        {
            vertical = Input.GetAxis("Mouse ScrollWheel");
        }
    }
}
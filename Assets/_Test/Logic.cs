using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class Logic : MonoBehaviour 
{
    public float MovementSpeed;    
    public Color red = Color.red;
    public Color blue = Color.blue;
    public Text device;
    private Renderer rend;
    private Vector3 MoveDirection;

    void Awake()
    {
        AirConsole.instance.onMessage += OnMessage;
        AirConsole.instance.onConnect += OnConnect;
        AirConsole.instance.onDisconnect += OnDisconnect;
    }

    void Start()
    {        
        rend = GetComponent<Renderer>();
    }

    void FixedUpdate()
    {
        transform.Translate(MoveDirection * MovementSpeed * Time.deltaTime);
    }

    void OnMessage(int from, JToken data)
    {
        //Debug.Log(from);
        //Debug.Log(data["dpad-left"]);
        int activePlayer = AirConsole.instance.ConvertDeviceIdToPlayerNumber(from); //get active player
        //Debug.Log(activePlayer);
        if (activePlayer != -1)
        {
            if (activePlayer == 0)
            {
                rend.material.color = red; //change color of object
                device.text = "Active player: Red"; //display active player on UI
            }
            if (activePlayer == 1)
            {
                rend.material.color = blue; //change color of object
                device.text = "Active player: Blue"; //display active player on UI
            }
        }

        MoveDirection = CalculateDirection(data);           
    }

    void OnConnect(int device_id)
    {
        if (AirConsole.instance.GetActivePlayerDeviceIds.Count == 0)
        {
            if (AirConsole.instance.GetControllerDeviceIds().Count >= 2)
            {
                AirConsole.instance.SetActivePlayers(2);
            }
            else
            {
                device.text = "Need more players";
            }
        }
    }


    void OnDisconnect(int device_id)
    {
        int active_player = AirConsole.instance.ConvertDeviceIdToPlayerNumber(device_id);
        if (active_player != -1)
        {
            if (AirConsole.instance.GetControllerDeviceIds().Count >= 2)
            {
                AirConsole.instance.SetActivePlayers(2);
            }
            else
            {
                AirConsole.instance.SetActivePlayers(0);                
                device.text = "Player has left - need more players";
            }
        }
    }

    public Vector3 CalculateDirection(JToken data)
    {
        Vector3 direction = Vector3.zero;
        if ((bool)data["dpad-left"]["pressed"])
        {
            if ((string)data["dpad-left"]["message"]["direction"] == "up")
            {
                direction.y += 1.0f;
            }
            if ((string)data["dpad-left"]["message"]["direction"] == "left")
            {
                direction.x -= 1.0f;
            }
            if ((string)data["dpad-left"]["message"]["direction"] == "down")
            {
                direction.y -= 1.0f;
            }
            if ((string)data["dpad-left"]["message"]["direction"] == "right")
            {
                direction.x += 1.0f;
            }

            return direction.normalized;
        }
        else
        {
            return Vector3.zero;
        }
    }    
    
}

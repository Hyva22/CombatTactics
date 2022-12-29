using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    private static InputManager instance;

    private bool paused = false;

    public static bool Paused { get => instance.paused; set => instance.paused = value; }

    private Dictionary<KeyCode, Action> inputs = new();

    private void Awake()
    {
        InitializeSingleton();
        InitializeKeys();
    }

    private void Update()
    {
        if (paused)
            return;

        foreach(var item in inputs)
        {
            if(Input.GetKey(item.Key))
                item.Value();
        }
    }

    private void InitializeSingleton()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log($"Instance of {GetType()} already exists!");
        }
        Destroy(this);
    }

    private void InitializeKeys()
    {
        
    }
}

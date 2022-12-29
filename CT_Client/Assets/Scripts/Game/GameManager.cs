using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;

    [SerializeField] private Player player;

    public static Player Player { get => instance.player;}

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.Log($"Instance of {GetType()} already exists!");
        }
        Destroy(this);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Network.Game;

public class Player : MonoBehaviour
{
    [SerializeField] private PlayerAccount playerAccount;

    public PlayerAccount PlayerAccount { get => playerAccount; set => playerAccount = value; }
}

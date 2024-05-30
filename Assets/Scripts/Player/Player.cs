using System;
using UnityEngine;


public class Player : MonoBehaviour
{     
    public PlayerController controller;
    public PlayerCondition condition;
    public Equipment equipment;

    public ItemData ItemData;
    public Action addItem;

    public Transform dropPosition;

    private void Awake()
    {
        CharacterManager.Instance.Player = this;
        controller = GetComponent<PlayerController>();
        condition = GetComponent<PlayerCondition>();
        equipment = GetComponent<Equipment>();
    }
}

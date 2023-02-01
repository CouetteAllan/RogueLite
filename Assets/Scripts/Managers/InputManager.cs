using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    public static PlayerInputAction playerInputAction;
    public static event Action<InputActionMap> actionMapChange;

    [SerializeField] private bool playerDefaultMap = true;

    private void Awake()
    {
        playerInputAction = new PlayerInputAction();
    }

    private void Start()
    {
        if (playerDefaultMap)
            ToggleActionMap(playerInputAction.Player);
        else
            ToggleActionMap(playerInputAction.UI);
    }

    public static void ToggleActionMap(InputActionMap actionMap)
    {
        if (actionMap.enabled)
            return;

        playerInputAction.Disable();
        actionMapChange?.Invoke(actionMap);
        actionMap.Enable();
    }
}

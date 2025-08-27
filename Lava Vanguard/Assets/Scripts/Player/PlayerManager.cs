using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerManager : MonoBehaviour
{
    public PlayerView playerView;
    private bool initialized = false;
    public static PlayerManager Instance { get; private set; }
    private void Awake()
    {
        Instance = this;
    }

    public void Init(bool isContinue)
    {
        playerView.Init(isContinue);
        initialized = true;
    }
    private void Update()
    {
        if (!initialized) return;
        Move();
        Jump();
        Down();
        UpdateInvincible();
        
    }

    private void Move()
    {
        if (Input.GetKey(KeyCode.A))
        {
            playerView.MoveLeft();
        }
        else if (Input.GetKey(KeyCode.D))
        {
            playerView.MoveRight();
        }
        else
        {
            playerView.MoveStop();
        }
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.K) || Input.GetKeyDown(KeyCode.W) || Input.GetKeyDown(KeyCode.Space))
        {
            playerView.JumpStart();
        }
        else if (Input.GetKey(KeyCode.K) || Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.Space))
        {
            playerView.JumpMaintain();
        }
        else if (Input.GetKeyUp(KeyCode.K) || Input.GetKeyUp(KeyCode.W) || Input.GetKeyUp(KeyCode.Space))
        {
            playerView.JumpStop();
        }
    }

    private void Down()
    {
        if (Input.GetKey(KeyCode.S))
        {
            playerView.MoveDown();
        }
    }
    public void GetHurt(int damage,bool mustKilled=false)
    {
        playerView.UpdateHealth(damage,mustKilled);
    }

    private void UpdateInvincible()
    {
        playerView.UpdateInvincible();
    }

    public void KillPlayer()
    {
        Time.timeScale = 0f;
        Debug.Log("kill player initiated");
        FindObjectOfType<SendToGoogle>().RecordEndTime();

        UIGameManager.Instance.Open<DeathPanel>();
    }
}


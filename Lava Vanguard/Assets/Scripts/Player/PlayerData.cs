using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public struct PlayerData
{
    public float speed;
    public float jumpForce;
    public float jumpAirTime;
    public float jumpAirForce;
    public int healthLimit;
    public float invincibleTime;
    public int healthUpValue;

    public float speedUpValue;
    public float speedLimit;

    public int health;
    public int currentHealthLimit;
    public int coin;

    public PlayerData(float jumpForce, float jumpAirTime, float jumpAirForce, int healthLimit, float invincibleTime, int healthUpValue,float speed, float speedUpValue,float speedLimit, int coin)
    {
        this.jumpForce = jumpForce;
        this.jumpAirTime = jumpAirTime;
        this.jumpAirForce = jumpAirForce;
        this.healthLimit = healthLimit;
        this.invincibleTime = invincibleTime;
        this.healthUpValue = healthUpValue;
        this.coin = coin;
        this.speed = speed;
        this.speedUpValue = speedUpValue;
        this.speedLimit = speedLimit;

        this.health = healthLimit;  
        this.currentHealthLimit = healthLimit;
    }

    public static PlayerData DefaultData = new PlayerData(
        jumpForce: 5.5f,
        jumpAirTime: 0.3f,
        jumpAirForce: 5.5f,
        healthLimit: 10,
        invincibleTime: 1f,
        healthUpValue: 2,
        speed: 5f,
        speedUpValue: 1f,
        speedLimit: 10f,
        coin: 10
    );
}


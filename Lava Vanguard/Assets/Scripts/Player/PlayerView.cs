using Async;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerView : MonoBehaviour
{
    [HideInInspector]
    public PlayerData playerData;
    private Rigidbody2D rb;
    public bool isGround = false;
    private bool isJumping = false;
    private float jumpTempTime = 0.0f;
    
    private float invincibleTempTime = 0.0f;
    //GoD! JUst temP CoDe
    private Collider2D currentPlatformCollider;

    public int GetHP()
    {
        return playerData.health;
    }

    public int GetHPLimit()
    {
        return playerData.currentHealthLimit;
    }

    public int GetCoin()
    {
        return playerData.coin;
    }
    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }
    public void Init(bool isContinue)
    {
        playerData = PlayerData.DefaultData;

        if (isContinue)
        {
            playerData.health = GameDataManager.SavedLevelData.Health;
            playerData.coin = GameDataManager.SavedLevelData.Coin;
        }
        else
        {
            playerData.health = GameDataManager.LevelData.Health;
            playerData.coin = GameDataManager.LevelData.Coin;
        }
        UIGameManager.Instance.UpdateCoin();
        UIGameManager.Instance.UpdateHp();


        invincibleTempTime = 0.0f;
        playerData.health = playerData.healthLimit;
        playerData.currentHealthLimit = playerData.healthLimit;
        rb = GetComponent<Rigidbody2D>();
        UIGameManager.Instance.UpdateHp();
        UIGameManager.Instance.UpdateCoin();

        gameObject.SetActive(true);
    }

    public void MoveLeft()
    {
        rb.velocity = new Vector2(-playerData.speed, rb.velocity.y);
        if (transform.localScale.x > 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void MoveRight()
    {
        rb.velocity = new Vector2(playerData.speed, rb.velocity.y);
        if (transform.localScale.x < 0)
        {
            transform.localScale = new Vector3(-transform.localScale.x, transform.localScale.y, transform.localScale.z);
        }
    }

    public void MoveStop()
    {
        rb.velocity = new Vector2(0, rb.velocity.y);
    }

    public void JumpStart()
    {
        if (isGround)
        {
            isJumping = true;
            jumpTempTime = playerData.jumpAirTime;
            rb.velocity = new Vector2(rb.velocity.x, playerData.jumpForce);
            isGround = false;
        }
    }

    public void MoveDown()
    {
        if (isGround&&currentPlatformCollider != null && !currentPlatformCollider.CompareTag("LongPlatform"))
        {
            isGround = false;
            StartCoroutine(DisableCollisionTemporarily(currentPlatformCollider));
        }
    }
    private IEnumerator DisableCollisionTemporarily(Collider2D platformCollider)
    {
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platformCollider, true);
        yield return new WaitForSeconds(0.4f);
        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), platformCollider, false);
    }

    public void JumpMaintain()
    {
        if (isJumping && jumpTempTime > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, playerData.jumpAirForce);
            jumpTempTime -= Time.deltaTime;
        }
    }

    public void JumpStop()
    {
        isJumping = false;
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGround = true;
            currentPlatformCollider = collision.collider;
        }
        else if (collision.gameObject.CompareTag("LongPlatform"))
        {
            isGround = true;
            currentPlatformCollider = collision.collider;
            if (collision.GetContact(0).normal.y > 0.5f && LevelManager.Instance.waveEnded)
            {
                LevelManager.Instance.enteredNext = true;
            }
        }
    }

    public void UpdateHealth(int damage, bool mustKilled = false)
    {
        if (mustKilled)
        {
            playerData.health = 0;
            PlayerManager.Instance.KillPlayer();
            return;
        }
        if (invincibleTempTime > 0)
        {
            return;
        }
        HitEffect();
        invincibleTempTime = playerData.invincibleTime;
        playerData.health -= damage;
        if (playerData.health <= 0)
        {
            if (SlotManager.Instance.CheckANKH())
            {
                playerData.health = playerData.currentHealthLimit;
            }
            else
            {
                playerData.health = 0;
                PlayerManager.Instance.KillPlayer();
            }
        }
        UIGameManager.Instance.UpdateHp();
    }
    public void UpdateInvincible()
    {
        if (invincibleTempTime > 0)
        {
            invincibleTempTime -= Time.deltaTime;
        }
    }

    public void HitEffect()
    {
        StartCoroutine(ChangeColorTemporarily(Color.red, 0.05f)); // Change to desired color and duration
        //cameraController.CameraShake(0.2f, 0.8f, 1f);
        CameraController.Instance.CameraShake(0.25f, 0.3f, 10);
    }

    private IEnumerator ChangeColorTemporarily(Color color, float duration)
    {
        SpriteRenderer spriteRenderer = GetComponentInChildren<SpriteRenderer>();
        if (spriteRenderer != null)
        {
            Color originalColor = spriteRenderer.color;
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = originalColor;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = originalColor;
        }
    }

    public void ResetSpeed()
    {
        playerData.speed = PlayerData.DefaultData.speed;
    }

    public void SpeedUp()
    {
        playerData.speed += playerData.speedUpValue;
        if (playerData.speed > playerData.speedLimit)
        {
            playerData.speed = playerData.speedLimit;
        }
    }

    public void ResetHealthLimit()
    {
        playerData.currentHealthLimit = playerData.healthLimit;
        UIGameManager.Instance.UpdateHp();
    }

    public void HealthUp(int currentHealth)
    {
        playerData.currentHealthLimit += playerData.healthUpValue;
        playerData.health = currentHealth;
        if(playerData.health > playerData.currentHealthLimit)
        {
            playerData.health=playerData.currentHealthLimit;
        }
        UIGameManager.Instance.UpdateHp();
    }

    public void GainCoin(int coin)
    {
        playerData.coin += coin;
        UIGameManager.Instance.UpdateCoin();
    }

    public void RestoreHealth()
    {
        playerData.health = playerData.currentHealthLimit;
        UIGameManager.Instance.UpdateHp();
    }
}

using System.Collections;
using UnityEngine;

public abstract class EnemyView : MonoBehaviour
{
    public EnemyData enemyData;
    // Minimum spawn distance between enemy and player
    protected float SpawnDistance = 1.2f;
    public GameObject deathEffect;
    public SpriteRenderer spriteRenderer;
    protected Color originalColor;
    protected int level;
    public virtual void Init(string ID,int level)
    {
        this.level = level;
        originalColor = ColorCenter.CardColors["Enemy" + Mathf.Min(Mathf.Max(level, 1), 9)];

        spriteRenderer.color = originalColor;
        enemyData = GameDataManager.EnemyData[ID];
        enemyData.Health += (enemyData.Health)/3*(level-1);
        enemyData.MaxHealth = enemyData.Health;
        //Debug.Log("Enemy " + ID + " " + level);
        transform.position = GetSpawnPosition();
    }
    public virtual void Init(string ID, Vector3 position)
    {
        enemyData = GameDataManager.EnemyData[ID];
        transform.position = position;
    }

    protected abstract void Approching();
    protected abstract Vector3 GetSpawnPosition();

    // How enemy been hit
    public virtual bool TakeHit(int bulletAttack)
    {
        enemyData.Health -= bulletAttack;
        if (enemyData.Health <= 0)
        {
            EnemyManager.Instance.enemyKilled++;
            enemyData.Health = 0;
            StartCoroutine(DeathEffect());
            PlayerManager.Instance.playerView.GainCoin(enemyData.Coin);
            UIGameManager.Instance.UpdateCoin();
            EnemyManager.Instance.enemyViews.Remove(this);
            Destroy(gameObject);
            CameraController.Instance.CameraShake(0.25f, 0.3f, 10);
            return true;
        }
        else
        {
            CameraController.Instance.CameraShake(0.25f, 0.3f, 4);
            HitEffect();
            return false;
        }
    }

    // What will happen when enemy encountered player
    protected virtual void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerManager.Instance.GetHurt(enemyData.Attack);
        }
    }

    public virtual void OnChildTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerManager.Instance.GetHurt(enemyData.Attack);
        }
    }

    private void Update()
    {
        Approching();
    }

    protected IEnumerator DeathEffect()
    {
        var e = Instantiate(deathEffect, transform.position, Quaternion.identity);
        //Debug.Log("Destroy");
        Destroy(e, 1.0f);
        yield return new WaitForSeconds(0.5f);
    }

    protected virtual void HitEffect()
    {
        StartCoroutine(ChangeColorTemporarily(Color.red, 0.05f)); // Change to desired color and duration
    }

    protected virtual IEnumerator ChangeColorTemporarily(Color color, float duration)
    {
        if (spriteRenderer != null)
        {
            spriteRenderer.color = color;
            yield return new WaitForSeconds(duration);
            spriteRenderer.color = originalColor;
        }
    }

    public void ForceKill()
    {
        StartCoroutine(DeathEffect());
        EnemyManager.Instance.enemyViews.Remove(this);
        Destroy(gameObject);
    }
}



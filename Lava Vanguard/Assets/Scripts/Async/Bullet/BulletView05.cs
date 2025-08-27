using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 普通
public class BulletView05 : BulletView
{
    private int moreCoin = 1;

    private static readonly Dictionary<int, BulletData> dataByLevel = new Dictionary<int, BulletData>()
    {
         { 1, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 2, moreCoin = 1 } },
         { 2, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 3, moreCoin = 2 } },
         { 3, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 4, moreCoin = 3 } },
         { 4, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 5, moreCoin = 4 } },
         { 5, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 6, moreCoin = 5 } },
         { 6, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 7, moreCoin = 6 } },
         { 7, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 8, moreCoin = 7 } },
         { 8, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 9, moreCoin = 8 } },
         { 9, new BulletData { lifeDistance=8.0f,detectionRange = 8.0f, speed = 15.0f, attack = 10, moreCoin =9 } },
    };

    public static string BulletDescription(int level)
    {
        return "Range: " + dataByLevel[level].detectionRange +
            "\nSpeed: " + dataByLevel[level].speed +
            "\nAttack: " + dataByLevel[level].attack +
            "\nMore Coins Gained: " + dataByLevel[level].moreCoin;
    }

    protected override void SetupBullet(int level)
    {
        lifeDistance = dataByLevel[level].lifeDistance;
        detectionRange = dataByLevel[level].detectionRange;
        speed = dataByLevel[level].speed;
        attack = dataByLevel[level].attack ;
        moreCoin = dataByLevel[level].moreCoin;
        SetFireDirection();
        ApplyInitialRotation();
    }

    private void ApplyInitialRotation()
    {
        // Debug.Log("Fire Direction: " + fireDirection);
        if (hasTarget)
        {
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void MoveBullet()
    {
        transform.position += (Vector3)fireDirection* speed *Time.deltaTime;
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            hasHit = true;
            Async.BulletManager.Instance.bulletHit5++;
            
            TriggerForwarder forwarder = other.GetComponent<TriggerForwarder>();
            EnemyView enemy;
            if (forwarder != null)
            {
                enemy = forwarder.parent;
            }
            else
            {
                enemy = other.GetComponent<EnemyView>();
            }
            bool killed = false;
            if (enemy != null)
            {
                killed = enemy.TakeHit(attack);
            }
            else
            {
                return;
            }
            if (killed)
            {
                PlayerManager.Instance.playerView.GainCoin(moreCoin);//do not hard code
                UIGameManager.Instance.UpdateCoin();
            }
            Destroy(gameObject);
        }
    }
}

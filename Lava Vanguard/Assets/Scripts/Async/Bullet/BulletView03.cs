using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 狙击
public class BulletView03 : BulletView
{


    private static readonly Dictionary<int, BulletData> dataByLevel = new Dictionary<int, BulletData>()
    {
         { 1, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 5} },
         { 2, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 7} },
         { 3, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 9} },
         { 4, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 11} },
         { 5, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 13 } },
         { 6, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 15} },
         { 7, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 17} },
         { 8, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 19} },
         { 9, new BulletData { lifeDistance=20.0f,detectionRange = 20.0f,speed=30.0f, attack = 21} },
    };

    public static string BulletDescription(int level)
    {
        return "Range: " + dataByLevel[level].detectionRange +
            "\nSpeed: " + dataByLevel[level].speed +
            "\nAttack: " + dataByLevel[level].attack;
    }

    protected override void SetupBullet(int level)
    {
        lifeDistance = dataByLevel[level].lifeDistance;
        detectionRange = dataByLevel[level].detectionRange;
        speed = dataByLevel[level].speed;
        attack = dataByLevel[level].attack;
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
            Async.BulletManager.Instance.bulletHit3++;
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
            if (enemy != null)
            {
                enemy.TakeHit(attack);
            }
            Destroy(gameObject);
        }
    }
}

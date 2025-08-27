using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletView06 : BulletView
{
    private float surviveTime;
    private static readonly Dictionary<int, BulletData> dataByLevel = new Dictionary<int, BulletData>()
    {
         { 1, new BulletData { detectionRange = 10f, attack = 1} },
         { 2, new BulletData { detectionRange = 10f, attack = 2} },
         { 3, new BulletData { detectionRange = 10f, attack = 3} },
         { 4, new BulletData { detectionRange = 10f, attack = 4} },
         { 5, new BulletData { detectionRange = 10f, attack = 5} },
         { 6, new BulletData { detectionRange = 10f, attack = 6} },
         { 7, new BulletData { detectionRange = 10f, attack = 7} },
         { 8, new BulletData { detectionRange = 10f, attack = 8} },
         { 9, new BulletData { detectionRange = 10f, attack = 9} },
    };
    public static string BulletDescription(int level)
    {
        return "Range: " + dataByLevel[level].detectionRange + 
            "\nAttack: " + dataByLevel[level].attack;
    }

    protected override void MoveBullet()
    {
        hasHit = false;
        surviveTime -= Time.deltaTime;
        if (surviveTime <= 0)
        {
            Destroy(gameObject);
        }
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            Async.BulletManager.Instance.bulletHit6++;
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
        }
    }
    private void ApplyInitialRotation()
    {
        if (hasTarget)
        {
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }
    protected override void SetupBullet(int level)
    {
        surviveTime = 1f;
        var sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        attack = dataByLevel[level].attack;
        detectionRange = dataByLevel[level].detectionRange;


        SetFireDirection();
        ApplyInitialRotation();
    }
}

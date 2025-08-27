using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;

public class BulletView04 : BulletView
{
    private float surviveTime;
    private float initialSurviveTime;
    private PolygonCollider2D hexCollider;

    private static readonly Dictionary<int, BulletData> dataByLevel = new Dictionary<int, BulletData>()
    {
         { 1, new BulletData { detectionRange = 3.5f, attack = 1} },
         { 2, new BulletData { detectionRange = 4.0f, attack = 2} },
         { 3, new BulletData { detectionRange = 4.5f, attack = 3} },
         { 4, new BulletData { detectionRange = 5.0f, attack = 4} },
         { 5, new BulletData { detectionRange = 5.5f, attack = 5} },
         { 6, new BulletData { detectionRange = 6.0f, attack = 6} },
         { 7, new BulletData { detectionRange = 6.5f, attack = 7} },
         { 8, new BulletData { detectionRange = 7.0f, attack = 8} },
         { 9, new BulletData { detectionRange = 7.5f, attack = 9} },
    };

    public static string BulletDescription(int level)
    {
        return "Range: " + dataByLevel[level].detectionRange +
            "\nAttack: " + dataByLevel[level].attack;
    }

    protected override void SetupBullet(int level)
    {
        var sr = GetComponent<SpriteRenderer>();
        Color c = sr.color;
        c.a = 0.5f;
        sr.color = c;

        attack = dataByLevel[level].attack;
        detectionRange = dataByLevel[level].detectionRange;
        initialSurviveTime = surviveTime = 0.5f;
        hexCollider = GetComponent<PolygonCollider2D>();
        float scaleFactor = detectionRange / 0.7f;
        transform.localScale = Vector3.one * scaleFactor;
    }
    protected override void MoveBullet()
    {
        hasHit = false;
        surviveTime -= Time.deltaTime;
        if(surviveTime <= 0)
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
            Async.BulletManager.Instance.bulletHit4++;
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
}

using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

// 散射
public class BulletView02 : BulletView
{
    protected float splitDistance = 1f;
    protected bool isSplited = false;
    protected float splitAngle = 30f;
    protected int splitCount = 2;
    protected int splitAttack = 2;

    private static readonly Dictionary<int, BulletData> dataByLevel = new Dictionary<int, BulletData>()
    {
         { 1, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 3,splitAttack=1,splitCount=2} },
         { 2, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 4,splitAttack=2,splitCount=3} },
         { 3, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 5,splitAttack=3,splitCount=4} },
         { 4, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 6,splitAttack=4,splitCount=5} },
         { 5, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 7,splitAttack=5,splitCount=6} },
         { 6, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 8,splitAttack=6,splitCount=7} },
         { 7, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 9,splitAttack=7,splitCount=8} },
         { 8, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 10,splitAttack=8,splitCount=9} },
         { 9, new BulletData { lifeDistance=15.0f,detectionRange = 15.0f,speed=10.0f, attack = 11,splitAttack=9,splitCount=10} },
    };

    public static string BulletDescription(int level)
    {
        return "Range: " + dataByLevel[level].detectionRange +
            "\nSpeed: " + dataByLevel[level].speed +
            "\nSplit into: " + dataByLevel[level].splitCount+
            "\nAttack (before split): " + dataByLevel[level].attack+
            "\nAttack (after split): "+dataByLevel[level].splitAttack;
    }


    protected override void SetupBullet(int level)
    {
        lifeDistance = dataByLevel[level].lifeDistance;
        detectionRange = dataByLevel[level].detectionRange;
        speed = dataByLevel[level].speed;
        attack = dataByLevel[level].attack;
        splitAttack = dataByLevel[level].splitAttack;
        splitCount = dataByLevel[level].splitCount;
        if (!isSplited)
        {
            SetFireDirection();
            ApplyInitialRotation();
        }
    }
    
    // Rotate the bullet
    private void ApplyInitialRotation()
    {
        // Debug.Log("Fire Direction: " + fireDirection);
        if (hasTarget)
        {
            float angle = Mathf.Atan2(fireDirection.y, fireDirection.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }
    }

    protected override void Update()
    {
        base.Update();
        // If not splited and distance>splitDistance then split
        if (!isSplited && Vector3.Distance(startPosition, transform.position) >= splitDistance)
        {
            SplitBullet();
        }
    }
    protected override void MoveBullet()
    {
        transform.position += (Vector3)fireDirection * speed * Time.deltaTime;
    }

    // Split the bullet in to different direction and change the size and attack.
    private void SplitBullet()
    {
        Transform container = transform.parent;
        for(int i = 0; i < splitCount; i++)
        {
            GameObject bulletObject = Instantiate(gameObject, transform.position, Quaternion.identity, container);
            BulletView02 bullet = bulletObject.GetComponent<BulletView02>();
            bullet.Init(level);
            bullet.isSplited = true;
            bullet.attack = splitAttack;
            bullet.transform.localScale *= 0.5f;
            float angle = splitAngle*(i - (splitCount - 1) / 2f);
            bullet.fireDirection= Quaternion.Euler(0, 0, angle) * fireDirection;
        }
        Destroy(gameObject);
    }

    protected override void OnTriggerEnter2D(Collider2D other)
    {
        if (hasHit)
        {
            return;
        }
        if (other.CompareTag("Enemy") || other.CompareTag("Boss"))
        {
            //Debug.Log("Split Hit: "+attack);
            hasHit = true;
            Async.BulletManager.Instance.bulletHit2++;
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

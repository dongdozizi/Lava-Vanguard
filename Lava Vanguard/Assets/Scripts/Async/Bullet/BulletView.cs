using System.Collections;
using System.Collections.Generic;
using System.IO.Pipes;
using UnityEngine;

public abstract class BulletView : MonoBehaviour
{
    protected Vector2 startPosition;
    protected Vector2 fireDirection = Vector2.right;
    protected int attack = 1;
    protected bool hasTarget = false;
    protected float detectionRange = 10.0f;
    protected bool hasHit = false;
    public int level = 1;
    public float speed = 1.0f;
    public float lifeDistance = 30.0f;
    public LayerMask enemyLayer;
    


    // Initialize the bullet with their level
    public void Init(int level)
    {
        this.level = level;
        startPosition = transform.position;
        SetupBullet(level);
    }

    //public abstract string BulletDescription(int level);

    protected abstract void SetupBullet(int level);

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Vector3.Distance(startPosition, transform.position) > lifeDistance)
        {
            Destroy(gameObject);
            return;
        }
        MoveBullet();
    }

    public virtual Transform FindClosestEnemy()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        Transform closestEnemy = null;
        float closestDistance = Mathf.Infinity;
        //Debug.Log($"Found {enemies.Length} enemies in range.");

        foreach (Collider2D enemy in enemies)
        {   
            if ((enemy.CompareTag("Enemy")|| enemy.CompareTag("Boss")) && enemy.gameObject.activeInHierarchy)
            {
                Rigidbody2D targetRb = enemy.GetComponent<Rigidbody2D>();
                Vector3 targetVelocity = targetRb != null ? targetRb.velocity : Vector3.zero;

                Vector3 targetPosition = enemy.transform.position;
                float timeToHit = Vector3.Distance(transform.position, targetPosition) / speed;
                // Vector3 predictedPosition = targetPosition + targetVelocity * timeToHit;

                float distance = Vector3.Distance(targetPosition, transform.position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestEnemy = enemy.transform;
                }
            }
        }
        return closestEnemy;
    }

    protected virtual void SetFireDirection()
    {        
        Transform closestEnemy= FindClosestEnemy();
        if (closestEnemy != null)
        {
            fireDirection = ((Vector2)closestEnemy.position - (Vector2)transform.position).normalized;
            hasTarget = true;
        }
        else
        {
            fireDirection = Vector2.right;
            hasTarget = false;
        }
    }
    protected abstract void MoveBullet();

    protected abstract void OnTriggerEnter2D(Collider2D other);

}

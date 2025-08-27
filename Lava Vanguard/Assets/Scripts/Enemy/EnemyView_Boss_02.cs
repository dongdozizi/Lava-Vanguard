using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Timers;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class EnemyView_Boss_02 : EnemyView
{
    // The relative position of the camera
    private Vector3 currentPosition = new Vector3(0, 0, 0);
    public SpriteRenderer[] spriteRenderers;
    private float healthBarAppearTime = 3f;

    [Header("Exclamation Bar")]
    public GameObject exclamationPrefab;
    private GameObject currentExclamation;
    private Vector3 exclamationCurrentPosition;
    private Vector3 exclamationEntrancePosition = new Vector3(0, -4f, 0);
    float exclamationFlashTime = 3f;
    float exclamationFlashInterval = 0.3f;

    private float bulletAttackHealthPercentage = 0.5f;
    
    private bool startAttack = false;

    [Header("Boss Transforms")]
    public Transform headPivot;
    public Transform leftHornPivot;
    public Transform rightHornPivot;
    public Transform leftLeg;
    public Transform rightLeg;
    public Transform body;
    public Transform leftArmPivot;
    public Transform leftArmEnd;
    public Transform rightArmPivot;
    public Transform rightArmEnd;

    private Vector3 entranceStartPosition = new Vector3(0, -10, 0);
    private Vector3 centerPosition = new Vector3(0, 0, 0);

    [Header("Laser Attack")]
    public GameObject littleLasePrefab;
    public GameObject laserPrefab;
    public GameObject windUpEffect;
    private float armRotationSpeed = 60f;
    private float armCooldown = 0.5f;

    public override void Init(string ID,int level)
    {
        this.level = level;
        originalColor = ColorCenter.CardColors["Boss" + Mathf.Min(Mathf.Max(level, 1), 9)];

        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
                sr.color = originalColor;
        }

        enemyData = GameDataManager.EnemyData[ID];
        enemyData.Health = Mathf.RoundToInt(150 * Mathf.Pow(4500f / 150f, (level - 1f) / 8f));
        enemyData.MaxHealth = enemyData.Health;
        transform.position = GetSpawnPosition();

        InitHealthBar();
        
        StartCoroutine(AttackCycle());
    }

    private void InitHealthBar()
    {
        if (UIGameManager.Instance.bossHPBar != null)
        {
            UIGameManager.Instance.bossHPBar.maxValue = enemyData.MaxHealth;
            UIGameManager.Instance.bossHPBar.value = enemyData.Health;
            UIGameManager.Instance.bossHPBar.gameObject.SetActive(false);
        }
        if (UIGameManager.Instance.BossHPLabel != null)
        {
            UIGameManager.Instance.BossHPLabel.text = "Boss "+level;
            UIGameManager.Instance.BossHPLabel.gameObject.SetActive(false);
        }
    }

    private IEnumerator AttackCycle()
    {
        startAttack = false;
        SetTagRecursively(gameObject, "Untagged");
        yield return StartCoroutine(ShowHealthBar());
        yield return StartCoroutine(ShowExclamationMark(exclamationEntrancePosition));
        SetTagRecursively(gameObject, "Boss");
        startAttack = true;
        yield return StartCoroutine(MoveFromToBoss(entranceStartPosition,centerPosition));
        yield return StartCoroutine(LaserAttackRoutine());
        yield return StartCoroutine(HeadShotRoutine());
    }

    private void Start()
    {
    }
    protected override void Approching()
    {
    }
    protected override Vector3 GetSpawnPosition()
    {
        Vector3 cameraPosition = Camera.main.transform.position;
        currentPosition = entranceStartPosition;
        FindObjectOfType<ButtonSound>()?.PlayBossApproachSound();// add special sound for boss!
        return new Vector3(entranceStartPosition.x, cameraPosition.y, 0);
    }

    private IEnumerator ShowExclamationMark(Vector3 spawnPos)
    {
        startAttack=false;

        // Show exclamation mark
        if (exclamationPrefab != null)
        {
            exclamationCurrentPosition = spawnPos;
            currentExclamation = Instantiate(exclamationPrefab);
            //currentExclamation.transform.SetParent(transform,true);
        }

        SpriteRenderer[] exclamationRenderers = currentExclamation.GetComponentsInChildren<SpriteRenderer>();

        // Flash excalamation mark
        float timeElapsed = 0f;
        while (timeElapsed < exclamationFlashTime)
        {
            foreach (var renderer in exclamationRenderers)
                renderer.enabled = !renderer.enabled;

            yield return new WaitForSeconds(exclamationFlashInterval);
            timeElapsed += exclamationFlashInterval;
        }
        if (currentExclamation != null)
        {
            Destroy(currentExclamation);
        }
        yield return null;
        startAttack=true;

    }

    // Move boss from fromPos to toPos
    private IEnumerator MoveFromToBoss(Vector3 fromPos, Vector3 toPos,bool forceMove=false)
    {
        currentPosition = fromPos;

        while (Vector3.Distance(currentPosition, toPos) > 0.01f)
        {
            currentPosition = Vector3.MoveTowards(currentPosition, toPos, enemyData.Speed * Time.deltaTime);
            yield return null;
        }
        transform.position = toPos;
        yield return null;
    }

    private IEnumerator LaserAttackRoutine()
    {
        while (enemyData.Health > enemyData.MaxHealth * bulletAttackHealthPercentage)
        {
            // Move left arm
            StartCoroutine(DoArmAttack(leftArmPivot, leftArmEnd));
            yield return new WaitForSeconds(0.5f);
            // Move right arm
            StartCoroutine(DoArmAttack(rightArmPivot, rightArmEnd));
            yield return new WaitForSeconds(0.5f);
        }
    }

    private IEnumerator DoArmAttack(Transform pivot,Transform end)
    {
        float timer = armCooldown;
        while (timer > 0f)
        {
            Vector3 toPlayer = PlayerManager.Instance.playerView.transform.position - pivot.position;
            float currentAngle = pivot.eulerAngles.z;
            float targetAngle = Mathf.Atan2(toPlayer.y, toPlayer.x) * Mathf.Rad2Deg + 90f;
            float newAngle = Mathf.MoveTowardsAngle(currentAngle, targetAngle, armRotationSpeed * Time.deltaTime);
            pivot.rotation = Quaternion.Euler(0, 0, newAngle);
            yield return null;
            timer -= Time.deltaTime;
        }
        //var wind = Instantiate(windUpEffect,end);
        var littleLaser = Instantiate(littleLasePrefab, end.position, end.rotation *Quaternion.Euler(0, 0, 180f), end);
        //Destroy(wind, 0.4f);
        Destroy(littleLaser, 0.25f);

        yield return new WaitForSeconds(0.25f);
        var laser = Instantiate(laserPrefab, end.position, end.rotation * Quaternion.Euler(0, 0, 180f), end);
        var laserView = laser.GetComponent<EnemyView_Boss_Bullet>();
        laserView.Init("Enemy_Boss_Bullet", laser.transform.position, new Vector3(0,0,0), laser.transform.rotation);
        Destroy(laser, 0.25f);
        yield return new WaitForSeconds(0.25f);
    }

    private IEnumerator MoveFromToPart(Transform obj, Vector3 toPos, float speed,Quaternion toRot,float rotSpeed, bool forceMove = false)
    {
        while (Vector3.Distance(obj.localPosition, toPos) > 0.01f ||
           Quaternion.Angle(obj.rotation, toRot) > 0.5f)
        {
            obj.localPosition = Vector3.MoveTowards(obj.localPosition, toPos, speed * Time.deltaTime);
            obj.rotation = Quaternion.RotateTowards(obj.rotation,toRot,rotSpeed * Time.deltaTime);
            yield return null;
        }
        obj.localPosition = toPos;
        obj.rotation = toRot;
        yield return null;
    }
    private IEnumerator GenerateFlyingEnemy()
    {
        while (true)
        {
            var enemyView = EnemyManager.Instance.GenerateSpecificEnemy(0, level);
            enemyView.transform.position = leftHornPivot.position;
            yield return new WaitForSeconds(0.1f);
            enemyView = EnemyManager.Instance.GenerateSpecificEnemy(0, level);
            enemyView.transform.position = rightHornPivot.position;
            yield return new WaitForSeconds(0.1f);
        }
    }
    private IEnumerator BodyMovement()
    {
        while(true)
        {
            yield return StartCoroutine(MoveFromToPart(leftArmPivot, new Vector3(2, -4, 0), 9.0f, Quaternion.Euler(0, 0, 45f), 90f));
            yield return StartCoroutine(MoveFromToPart(rightArmPivot, new Vector3(-2, -4, 0), 9.0f, Quaternion.Euler(0, 0, -45f), 90f));
            yield return StartCoroutine(MoveFromToPart(leftLeg, new Vector3(2, 4, 0), 9.0f, Quaternion.Euler(0, 0, -45f), 90f));
            yield return StartCoroutine(MoveFromToPart(rightLeg, new Vector3(-2, 4, 0), 9.0f, Quaternion.Euler(0, 0, 45f), 90f));
            Vector3 diffPos=PlayerManager.Instance.playerView.transform.position-transform.position;
            yield return StartCoroutine(MoveFromToPart(body, diffPos, 9.0f, Quaternion.Euler(0, 0, 0f), 90f));

            leftArmPivot.localPosition = new Vector3(-6, 4, 0);
            rightArmPivot.localPosition = new Vector3(6, 4, 0);
            leftLeg.localPosition = new Vector3(-6, -4, 0);
            rightLeg.localPosition = new Vector3(6, -4, 0);
            body.localPosition = new Vector3(0, -5, 0);

            yield return StartCoroutine(MoveFromToPart(leftArmPivot, new Vector3(-5, 3, 0), 3.0f, Quaternion.Euler(0, 0, 45f), 90f));
            yield return StartCoroutine(MoveFromToPart(rightArmPivot, new Vector3(5, 3, 0), 3.0f, Quaternion.Euler(0, 0, -45f), 90f));
            yield return StartCoroutine(MoveFromToPart(leftLeg, new Vector3(-5, -3, 0), 3.0f, Quaternion.Euler(0, 0, -45f), 90f));
            yield return StartCoroutine(MoveFromToPart(rightLeg, new Vector3(5, -3, 0), 3.0f, Quaternion.Euler(0, 0, 45f), 90f));
            yield return StartCoroutine(MoveFromToPart(body, new Vector3(0, -2, 0), 5.0f, Quaternion.Euler(0, 0, 0f), 90f));
        }
    }
    private IEnumerator HeadShotRoutine()
    {
        yield return StartCoroutine(MoveFromToPart(leftArmPivot, new Vector3(-5, 3, 0), 5.0f, Quaternion.Euler(0, 0, 45f),90f));
        yield return StartCoroutine(MoveFromToPart(rightArmPivot, new Vector3(5, 3, 0), 5.0f, Quaternion.Euler(0, 0, -45f),90f));
        yield return StartCoroutine(MoveFromToPart(leftLeg, new Vector3(-5, -3, 0), 5.0f, Quaternion.Euler(0, 0, -45f), 90f));
        yield return StartCoroutine(MoveFromToPart(rightLeg, new Vector3(5, -3, 0), 5.0f, Quaternion.Euler(0, 0, 45f), 90f));
        yield return StartCoroutine(MoveFromToPart(body, new Vector3(0, -2, 0), 5.0f, Quaternion.Euler(0, 0, 0f), 90f));
        yield return StartCoroutine(MoveFromToPart(headPivot, new Vector3(0, 0, 0), 3.0f, Quaternion.Euler(0, 0, 0f), 90f));
        StartCoroutine(GenerateFlyingEnemy());
        StartCoroutine(BodyMovement());
    }
    private IEnumerator ShowHealthBar()
    {
        if (UIGameManager.Instance.bossHPBar != null)
        {
            UIGameManager.Instance.bossHPBar.gameObject.SetActive(true);
        }
        if (UIGameManager.Instance.BossHPLabel != null)
        {
            UIGameManager.Instance.BossHPLabel.gameObject.SetActive(true);
        }

        float elapsed = 0f;

        UIGameManager.Instance.bossHPBar.value = 0;

        while (elapsed < healthBarAppearTime)
        {
            float t = elapsed / healthBarAppearTime;
            UIGameManager.Instance.bossHPBar.value =t* UIGameManager.Instance.bossHPBar.maxValue;
            elapsed += Time.deltaTime;
            yield return null;
        }
        UIGameManager.Instance.bossHPBar.value = UIGameManager.Instance.bossHPBar.maxValue;
    }

    public override bool TakeHit(int bulletAttack)
    {
        if(!startAttack) { return false; }
        enemyData.Health -= bulletAttack;
        UIGameManager.Instance.bossHPBar.value = enemyData.Health;
        if (enemyData.Health <= 0)
        {
            EnemyManager.Instance.enemyKilled++;
            enemyData.Health = 0;
            StartCoroutine(DeathEffect());
            PlayerManager.Instance.playerView.GainCoin(enemyData.Coin);
            UIGameManager.Instance.UpdateCoin();
            if (currentExclamation != null)
            {
                Destroy(currentExclamation);
            }
            Destroy(gameObject);
            if (UIGameManager.Instance.bossHPBar != null)
            {
                UIGameManager.Instance.bossHPBar.gameObject.SetActive(false);
            }
            if (UIGameManager.Instance.BossHPLabel != null)
            {
                UIGameManager.Instance.BossHPLabel.gameObject.SetActive(false);
            }
            return true;
        }
        else
        {
            HitEffect();
            return false;
        }
    }

    private void OnEnable()
    {
        CameraController.OnCameraUpdated += UpdateBossPosition;
    }

    private void OnDisable()
    {
        CameraController.OnCameraUpdated -= UpdateBossPosition;
    }

    private void UpdateBossPosition()
    {
        // Update exclamation bar location
        if (currentExclamation != null)
        {
            Vector3 tmpPosition = CameraController.Instance.virtualCamera.transform.position + exclamationCurrentPosition;
            tmpPosition.z = 0;
            currentExclamation.transform.position = tmpPosition;
        }
        
        // Update the boss position by relative position
        transform.position = new Vector3(currentPosition.x, CameraController.Instance.virtualCamera.transform.position.y + currentPosition.y, 0);
    }

    void SetTagRecursively(GameObject obj, string tagName)
    {
        obj.tag = tagName;
        foreach (Transform child in obj.transform)
        {
            SetTagRecursively(child.gameObject, tagName);
        }
    }

    protected override IEnumerator ChangeColorTemporarily(Color color, float duration)
    {
        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
                sr.color = color;
        }
        yield return new WaitForSeconds(duration);
        foreach (var sr in spriteRenderers)
        {
            if (sr != null)
                sr.color = originalColor;
        }
    }
}

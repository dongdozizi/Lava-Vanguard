using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Async
{
    public class BulletManager : MonoBehaviour
    {
        public static BulletManager Instance {  get; private set; }
        public Transform bulletContainer;
        public Transform bulletContainer2;
        public GameObject[] bulletPrefabs;

        [HideInInspector] public int bulletGenerated1 = 0;
        [HideInInspector] public int bulletGenerated2 = 0;
        [HideInInspector] public int bulletGenerated3 = 0;
        [HideInInspector] public int bulletGenerated4 = 0;
        [HideInInspector] public int bulletGenerated5 = 0;
        [HideInInspector] public int bulletGenerated6 = 0;

        [HideInInspector] public int bulletHit1 = 0;
        [HideInInspector] public int bulletHit2 = 0;
        [HideInInspector] public int bulletHit3 = 0;
        [HideInInspector] public int bulletHit4 = 0;
        [HideInInspector] public int bulletHit5 = 0;
        [HideInInspector] public int bulletHit6 = 0;

        private void Awake()
        {
            Instance = this;
        }

        public void GenerateBullet(CardView cardView)
        {
            Vector3 spawnPos = PlayerManager.Instance.playerView.transform.position;
            int index = cardView.cardRankData.CardID[^1] - '1';

            var b = Instantiate(bulletPrefabs[index], spawnPos, Quaternion.identity, bulletContainer);
            b.GetComponent<SpriteRenderer>().color = cardView.content.color;
            b.GetComponent<BulletView>().Init(cardView.cardRankData.Level);

            if (b.GetComponent<BulletView>().FindClosestEnemy() == null)
            {
                Destroy(b);
                return;
            }

            switch (index)
            {
                case 0:
                    bulletGenerated1++;
                    break;
                case 1:
                    bulletGenerated2++;
                    break;
                case 2:
                    bulletGenerated3++;
                    break;
                case 3:
                    bulletGenerated4++;
                    break;
                case 4:
                    bulletGenerated5++;
                    break;
                case 5:
                    b.transform.parent = bulletContainer2;
                    bulletGenerated6++;
                    break;
                default:
                    break;
            }
        }

        public string getBulletGeneratedData()
        {
            return $"Fired B1:{bulletGenerated1}, B2:{bulletGenerated2}, B3:{bulletGenerated3}, B4:{bulletGenerated4}, B5:{bulletGenerated5} " +
                $"  Hit  B1:{bulletHit1}, B2:{bulletHit2}, B3:{bulletHit3}, B4:{bulletHit4}, B5:{bulletHit5}";
        }
    }
}

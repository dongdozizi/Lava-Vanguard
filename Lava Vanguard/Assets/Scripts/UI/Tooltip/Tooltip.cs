using Async;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip Instance {  get; private set; }
    public GameObject tooltip;
    public TMP_Text title;
    public TMP_Text description;

    public GameObject tooltip2;
    public TMP_Text title2;
    public TMP_Text bulletDetail2;
    public TMP_Text description2;


    private void Awake()
    {
        Instance = this;
    }

    public void ShowTooltip(CardView data)
    {
        //tooltip.SetActive(true);
        if (data.cardSpriteData.Type == "Bullet")
        {
            title.text = data.cardSpriteData.Title + " (Level: " + data.cardRankData.Level + ")";
        }
        else
        {
            title.text = data.cardSpriteData.Title;
        }
        description.text = data.cardSpriteData.Description;
        tooltip.transform.position = Input.mousePosition + new Vector3(0, -150, 0);

        // Tooltip 2
        tooltip2.SetActive(true);
        title2.text= data.cardSpriteData.Title;
        if (data.cardSpriteData.Type == "Bullet")
        {
            string bulletDescription = "Level: " + data.cardRankData.Level + "\n";
            switch (data.cardSpriteData.ID)
            {
                case "Card_Bullet01":
                    bulletDescription += BulletView01.BulletDescription(data.cardRankData.Level);
                    break;
                case "Card_Bullet02":
                    bulletDescription += BulletView02.BulletDescription(data.cardRankData.Level);
                    break;
                case "Card_Bullet03":
                    bulletDescription += BulletView03.BulletDescription(data.cardRankData.Level);
                    break;
                case "Card_Bullet04":
                    bulletDescription += BulletView04.BulletDescription(data.cardRankData.Level);
                    break;
                case "Card_Bullet05":
                    bulletDescription += BulletView05.BulletDescription(data.cardRankData.Level);
                    break;
                case "Card_Bullet06":
                    bulletDescription += BulletView06.BulletDescription(data.cardRankData.Level);
                    break;
            }
            bulletDetail2.text = bulletDescription;
        }
        else
        {
            bulletDetail2.text = "";
        }
        if (data.cardSpriteData.ID == "Card_AddSlot")
        {
            description2.text = string.Format(data.cardSpriteData.Description, UIGameManager.Instance.GetPanel<WeaponPanel>().BuySlotPrice, PlayerManager.Instance.playerView.GetCoin());
        }
        else
        {
            description2.text = data.cardSpriteData.Description;
        }
    }
    public void HideTooltip()
    {
        tooltip.SetActive(false);
        tooltip2.SetActive(false);
    }
}

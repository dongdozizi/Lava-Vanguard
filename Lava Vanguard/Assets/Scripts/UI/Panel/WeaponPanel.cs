using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class WeaponPanel : UIPanel
{
    private readonly int initialPrice = 5;
    private readonly int addPrice = 5;
    public int BuySlotPrice { get => initialPrice + (SlotManager.Instance.currentTotalGrid - 2) * addPrice; }
    
    public override void Init()
    {
        base.Init();
        canOpen = false;
    }
    public override void Close()
    {
        base.Close();
        FindObjectOfType<ButtonSound>()?.PlayPurchaseSound();// sepcial sound effect for close panel
        UIGameManager.Instance.SetFocus(false);
        CameraZoomAndMove.Instance.ResetCamera();
        Tooltip.Instance.HideTooltip();
    }
    public override void Open()
    {
        FindObjectOfType<ButtonSound>()?.PlayPurchaseSound();// sepcial sound effect for open panel
        UIGameManager.Instance.SetRedDot(false);
        UIGameManager.Instance.SetFocus(true);
        CameraZoomAndMove.Instance.ZoomAndMove(base.Open);
    }
    public void BuySlot()
    {
        int currentTotalGird = SlotManager.Instance.currentTotalGrid;
        int price = initialPrice + (currentTotalGird - 2) * addPrice;
        if (PlayerManager.Instance.playerView.GetCoin() >= price)
        {
            PlayerManager.Instance.playerView.GainCoin(-price);
            UIGameManager.Instance.UpdateCoin();
            FindObjectOfType<ButtonSound>()?.PlayPurchaseSound();// sepcial sound effect for buy slot
            SlotManager.Instance.AddSlot();
        }
        if (SlotManager.Instance.currentTotalGrid == SlotManager.TOTAL_GRID)
        {
            SlotManager.Instance.HideBuySlot();
        }
    }
}

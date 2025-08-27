using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Async
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }
        public InventoryView inventoryView;

        private void Awake()
        {
            Instance = this;
        }
        public void Init(bool isContinue)
        {
            inventoryView.Init(isContinue);
        }
    }
}
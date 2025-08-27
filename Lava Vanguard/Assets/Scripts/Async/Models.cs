using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Async
{
    public struct CardSpriteData
    {
        public string ID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Cost {  get; set; }
        public string Type { get; set; }
        public string Background { get; set; }
        public string Outline { get; set; }
        public string Content { get; set; }
        public bool Draggable { get; set; }
        public bool Collectable { get; set; }
    }
    public struct CardRankData
    {
        //Currently ID has no usage
        public string ID { get; set; }
        public string CardID { get; set; }
        public int Level { get; set; }
        public string LinkedSequenceID { get; set; }
        public CardRankData(string ID, string CardID, int Level) : this()
        {
            this.ID = ID;
            this.CardID = CardID;
            this.Level = Level;
        }
        public CardRankData(CardSpriteData data) : this() 
        {
            this.ID = "0";
            this.CardID = data.ID;
            this.Level = 1;
        }
        public static CardRankData AsyncHead
        {
            get => new CardRankData(ID: "0", CardID: "Card_Async2", Level: 1);
        }
        public static CardRankData Empty
        {
            get => new CardRankData(ID: "0", CardID: "Card_Empty", Level: 1);
        }
    }
    public struct SequenceData
    {
        public List<CardRankData> CardDatas { get; set; }
    }
    public struct InventoryData
    {
        public List<CardRankData> CardDatas { get; set; }
    }
    public struct LevelData
    {
        public int Health { get; set;}
        public int Coin { get; set; }
        public int Wave { get; set; }
    }
}

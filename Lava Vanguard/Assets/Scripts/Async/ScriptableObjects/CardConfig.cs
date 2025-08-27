using UnityEngine;

[CreateAssetMenu(fileName = "CardConfig", menuName = "Scriptable Object/CardConfig")]
public class CardConfig : ScriptableObject
{
    public int MainNum = 10;
    public int CardSize = 80;
    public int LeftRightPadding = 10;

    public int MainLength => CardSize * MainNum + LeftRightPadding * 2;
    public int MainHeight => CardSize + LeftRightPadding * 2;
    public int IntervalY => CardSize + LeftRightPadding * 4;
}

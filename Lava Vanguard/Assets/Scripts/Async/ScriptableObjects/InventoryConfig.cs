using UnityEngine;

[CreateAssetMenu(fileName = "InventoryConfig", menuName = "Scriptable Object/InventoryConfig")]
public class InventoryConfig : ScriptableObject
{
    public Vector2 Center;
    public Vector2 Size;

    public bool CheckInside(Vector2 position)
    {
        Vector2 leftBottom = Center - Size / 2;
        Vector2 rightTop = Center + Size / 2;
        if (position.x >= leftBottom.x && position.x <= rightTop.x &&
            position.y >= leftBottom.y && position.y <= rightTop.y) 
            return true;
        else 
            return false;
    }
}

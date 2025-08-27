using UnityEngine;

public class TriggerForwarder : MonoBehaviour
{
    public EnemyView parent;

    private void OnTriggerStay2D(Collider2D collision)
    {
        parent?.OnChildTriggerStay2D(collision);
    }

}

using UnityEngine;

public class EnemyDetector : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        OnEnemySpawn(other, true);
    }

    private void OnTriggerExit(Collider other)
    {
        OnEnemySpawn(other, false);
    }

    private void OnEnemySpawn(Collider collider, bool isEnable)
    {
        if (collider.gameObject.layer == LayerMask.NameToLayer("Spawner"))
        {
            if (isEnable)
            {
                collider.GetComponent<EnemySpawner>().Enable();
            }
            else
            {
                collider.GetComponent<EnemySpawner>().Disable();
            }
        }
    }
}

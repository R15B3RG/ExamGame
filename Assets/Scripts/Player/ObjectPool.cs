using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    public static ObjectPool instance;

    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private int poolSize = 10;

    private void Awake()
    {
        if(instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }

    private void CreateInitialPool()
    {
        for(int i = 0; i < poolSize; i++)
        {
            GameObject newBullet = Instantiate(bulletPrefab);
        }
    }
}

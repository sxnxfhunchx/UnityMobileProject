using UnityEngine;

public class PlayerShooter : MonoBehaviour
{
    public GameObject swordPrefab;
    public Transform firePoint;
    public float range = 100f;

    void Update()
    {
        if (Input.GetMouseButtonDown(0)) 
        {
            Shoot();
        }
    }

    void Shoot()
    {
        RaycastHit hit;
        if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, range))
        {
            if (hit.collider.CompareTag("Enemy"))
            {
                Destroy(hit.collider.gameObject);
                GameManager.Instance.AddScore(10);
            }
            
            SpawnVisualSword(hit.point);
        }
    }

    void SpawnVisualSword(Vector3 targetPoint)
    {
        GameObject sword = Instantiate(swordPrefab, firePoint.position, firePoint.rotation);
    }
}
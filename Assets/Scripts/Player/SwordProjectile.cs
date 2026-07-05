using System.Collections;
using System.Collections.Generic;
using SO;
using UnityEngine;

public class SwordProjectile : MonoBehaviour
{
    [SerializeField] private Transform visualRoot;
    
    [SerializeField] private float speed = 30f;
    [SerializeField] private int damage = 1;
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private string poolTag = "Projectile"; 
    
    private readonly Dictionary<WeaponData, GameObject> visuals = new();
    
    private Coroutine deactivateCoroutine;

    private GameObject currentVisual;
    private bool isReturned;
    
    void OnEnable()
    {
        isReturned = false;
        deactivateCoroutine = StartCoroutine(DeactivateAfterTime());
    }

    void OnDisable()
    {
        if (deactivateCoroutine != null)
        {
            StopCoroutine(deactivateCoroutine);
        }
    }
    
    void Update()
    {
        transform.position += transform.forward * (speed * Time.deltaTime);
    }

    IEnumerator DeactivateAfterTime()
    {
        yield return new WaitForSeconds(lifeTime);
        ReturnToPool();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;
        
        if (!other.TryGetComponent(out EnemyController enemy))
            return;
        
        enemy.TakeDamage(damage); 
        ReturnToPool();  
    }

    void ReturnToPool()
    {
        if (isReturned)
            return;

        isReturned = true;

        if (ObjectPooler.Instance == null)
            return;

        ObjectPooler.Instance.ReturnToPool(poolTag, gameObject);
    }
    
    public void Initialize(WeaponData weapon)
    {
        if (weapon == null)
            return;

        damage = weapon.Damage;
        
        SetVisual(weapon);
    }
    
    private void SetVisual(WeaponData weapon)
    {
        foreach (GameObject visual in visuals.Values)
        {
            visual.SetActive(false);
        }
        
        if (!visuals.TryGetValue(weapon, out GameObject visualInstance))
        {
            visualInstance = Instantiate(weapon.VisualPrefab, visualRoot);

            visualInstance.transform.localPosition = Vector3.zero;
            visualInstance.transform.localRotation = Quaternion.identity;
            visualInstance.transform.localScale = Vector3.one;

            visuals.Add(weapon, visualInstance);
        }

        visualInstance.SetActive(true);
    }
}
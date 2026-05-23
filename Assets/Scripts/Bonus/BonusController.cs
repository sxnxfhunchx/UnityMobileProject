using UnityEngine;

public class BonusController : MonoBehaviour
{
    [SerializeField] private BonusData bonusData;
    [SerializeField] private SoundData soundData; 
    [SerializeField] private string pickupVfxPoolTag = "BonusVFX";
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        PlayPickupSound();
        
        SpawnPickupVFX();

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddBonusScore(bonusData.scoreValue);
        }
        
        ReturnToPool();
    }

    private void ReturnToPool()
    {
        if (ObjectPooler.Instance == null || bonusData == null)
            return;
        
        ObjectPooler.Instance.ReturnToPool(bonusData.poolTag, gameObject);
    }

    private void SpawnPickupVFX()
    {
        if (ObjectPooler.Instance == null)
            return;

        if (string.IsNullOrEmpty(pickupVfxPoolTag))
            return;
        
        Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
        ObjectPooler.Instance.SpawnFromPool(pickupVfxPoolTag, spawnPos, Quaternion.identity); 
    }

    private void PlayPickupSound()
    {
        if (SoundManager.Instance == null || soundData == null)
            return;
        
        AudioClip clip = soundData.GetRandomSound();
        
        if (clip == null)
            return;
        
        SoundManager.Instance.PlaySound(clip, transform.position);
    }
}

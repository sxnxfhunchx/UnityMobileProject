using UnityEngine;

public class BonusController : MonoBehaviour
{
    [SerializeField] private BonusData bonusData;
    [SerializeField] private SoundData soundData; 
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        if (SoundManager.Instance != null && soundData != null)
        {
            AudioClip clip = soundData.GetRandomSound();
            SoundManager.Instance.PlaySound(clip, transform.position);
        }
        
        if (ObjectPooler.Instance != null)
        {
            Vector3 spawnPos = transform.position + Vector3.up * 0.5f;
            ObjectPooler.Instance.SpawnFromPool("BonusVFX", spawnPos, Quaternion.identity);
        }
        
        GameManager.Instance.AddScore(bonusData.scoreValue);
        gameObject.SetActive(false);
    }

    public string GetTag()
    {
        return bonusData.bonusName;
    }
}

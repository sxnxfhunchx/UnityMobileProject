using UnityEngine;

[CreateAssetMenu(fileName = "NewSoundData", menuName = "ScriptableObjects/SoundData")]
public class SoundData : ScriptableObject
{
    public AudioClip[] hitSounds; 

    public AudioClip GetRandomHitSound()
    {
        if (hitSounds.Length == 0) return null;
        int randomIndex = Random.Range(0, hitSounds.Length);
        return hitSounds[randomIndex];
    }
}
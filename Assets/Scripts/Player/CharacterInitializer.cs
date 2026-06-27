using SO;
using UnityEngine;

public class CharacterInitializer : MonoBehaviour
{
    [SerializeField] private Transform modelRoot;
    [SerializeField] private PlayerMovement playerMovement;
    [SerializeField] private PlayerHealth playerHealth;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        CharacterData data = GameManager.Instance.CurrentCharacter;

        if (data == null)
            return;

        ApplyCharacter(data);
    }

    private void ApplyCharacter(CharacterData data)
    {
        foreach (Transform child in modelRoot)
        {
            Destroy(child.gameObject);
        }
        
        GameObject model = Instantiate(
            data.gameplayPrefab,
            modelRoot
        );

        model.transform.localPosition = Vector3.zero;
        model.transform.localRotation = Quaternion.identity;
        
        playerMovement.SetSpeed(data.speed);
        playerHealth.SetMaxHealth(data.health);
    }
}

using UnityEngine;

public class TileMover : MonoBehaviour
{
   [SerializeField] public float speed = 20f;
    [SerializeField] public float tileLength = 30f; 
    [SerializeField] public int totalTiles = 4;    

    void Update()
    {
        transform.Translate(Vector3.back * speed * Time.deltaTime);

        if (transform.position.z <= -tileLength)
        {
            Vector3 newPos = transform.position;
            newPos.z += tileLength * totalTiles;
            transform.position = newPos;
        }
    }
}
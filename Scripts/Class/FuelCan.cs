using UnityEngine;

public class FuelCan : MonoBehaviour
{
    #region MonoBehaviour Callbacks

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") || other.CompareTag("AI"))
        {
            Destroy(GetComponent<BoxCollider>());
            other.GetComponent<Character>().GetFuel();
            Destroy(gameObject);
        }

        if (other.CompareTag("Player"))
        {
            FindObjectOfType<PlayerControl>().PlusSpawner();
        }
    }

    #endregion
}

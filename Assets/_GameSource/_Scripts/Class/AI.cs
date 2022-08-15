using UnityEngine;

public class AI : Character
{
    
#region MonoBehaviour Callbacks

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Bonus"))
        {
            Win();
            GameManager.Instance.GameOver();
            Destroy(this);
        }

        if (other.CompareTag("Sea"))
        {
            _isAlive = false;
            StopPlayer();
            StartCoroutine(Swimming());
            Vector3 _waterFxPos = transform.position - Vector3.up * other.contactOffset;
            GameManager.Instance.CallWaterFx(_waterFxPos);
        }
    }

#endregion
}
using UnityEngine;

public class Jetpack : MonoBehaviour
{
    #region Variables

    [SerializeField] float pushPower;
    [SerializeField] float maxHeight;
    [SerializeField] float fuelCan;
    [SerializeField] GameObject particles;

    Rigidbody rb;
    Transform myParent;
    Character myCharacter;
    PlayerControl player;

    float currentForce;
    float gravity;
    float difference;
    float currentFuelCan;

    bool bonusTime;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        currentForce = pushPower;
        myCharacter = GetComponentInParent<Character>();
        myParent = myCharacter.transform;
        rb = myParent.GetComponent<Rigidbody>();
        gravity = Physics.gravity.y;
        difference = maxHeight - transform.position.y;
        currentFuelCan = 0;
        GameManager.Instance.FillFuelBar(currentFuelCan / fuelCan);
        if(myParent.CompareTag("Player"))
            player = myParent.GetComponent<PlayerControl>();
    }

    private void FixedUpdate()
    {
        if (currentFuelCan >= 0)
        {
            currentFuelCan -= Time.deltaTime;
            if (myParent.CompareTag("Player"))
                GameManager.Instance.FillFuelBar(currentFuelCan / fuelCan);
            if (myParent.position.y >= maxHeight)
                currentForce = gravity;
            else
            {
                if (GameManager.Instance.isGameStarted)
                    currentForce = pushPower * ((maxHeight - transform.position.y) / difference);
            }
            rb.AddForce(Vector3.up * currentForce, ForceMode.Acceleration);
            myCharacter.ControllJetpack(true);
        }
        else
        {
            if (bonusTime)
            {
                player.FinishBonusTime();
                player.PlayFx();
                GameManager.Instance.FinishLevel();
            }
            this.enabled = false;
        }
    }

    private void OnEnable()
    {
        particles.SetActive(true);
        myCharacter.ResetMaterial();
    }

    private void OnDisable()
    {
        particles.SetActive(false);
    }

    #endregion

    #region Other Methods

    public void AddFuel(float value)
    {
        currentFuelCan += value;
        if (currentFuelCan > fuelCan)
            currentFuelCan = fuelCan;
    }

    public float GetFuelValue()
    {
        return currentFuelCan / fuelCan;
    }

    public float GetFuel()
    {
        return currentFuelCan;
    }

    public void BonusTime()
    {
        maxHeight = 250f;
        currentForce = 35f;
        bonusTime = true;
    }

    #endregion
}

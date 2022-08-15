using UnityEngine;
using Cinemachine;
using DG.Tweening;
using TMPro;

public class PlayerControl : Character
{

#region OBJECTS

    [Header("Objects")]
    [Space]
    [SerializeField] ParticleSystem confeti;
    [SerializeField] CinemachineVirtualCamera cm;
    [SerializeField] CinemachineVirtualCamera bonusCamera;
    [SerializeField] TextMeshProUGUI plusText;    

    [Header("Other")]
    [Space]
    private Tween shake;
    private CapsuleCollider collider;
    private Rigidbody rb;

#endregion

#region CALLBACKS

    private void Start()
    {
        collider = GetComponent<CapsuleCollider>();
        rb = GetComponent<Rigidbody>();
    }

    public void PlusSpawner()
    {
        plusText.gameObject.SetActive(true);
        plusText.rectTransform.DOScale(2, .3f).SetLoops(1, LoopType.Yoyo).OnComplete(() =>
        {
            plusText.rectTransform.DOScale(1, .3f);
            plusText.gameObject.SetActive(false);
        });
    }
    
    public void OpenCollider()
    {
        collider.enabled = true;
        rb.isKinematic = false;
        BonusTime();
    }

    private void Lose()
    {
        StartCoroutine(Swimming());
        OutOfCamera();
        GameManager.Instance.GameOver();
    }

    public void OutOfCamera()
    {
        cm.Follow = null;
        cm.LookAt = null;
    }

    private void BonusCamera()
    {
        bonusCamera.Priority = 11;
    }

    public void FinishBonusTime()
    {
        bonusCamera.Follow = null;
        bonusCamera.LookAt = null;
    }

    public void PlayFx()
    {
        confeti.Play();
    }

#endregion

#region COLLISION

    // TRIGGER
    private void OnTriggerEnter(Collider other)
    {
        // COIN & PLAYER 
        if (other.gameObject.CompareTag("Coin"))
        {
            GameManager.Instance.AddCoin(1);
            Destroy(other.gameObject);
        }

        if (other.CompareTag("Bonus"))
        {
            Win();
            GameManager.Instance.ControllPlayers();
            transform.DOMove(GameManager.Instance.waitPoint.position,.5f).SetEase(Ease.Linear).SetUpdate(UpdateType.Fixed).OnComplete(() => {
                Win();
            });
            transform.DORotateQuaternion(GameManager.Instance.waitPoint.rotation,.5f);
            BonusCamera();
            confeti.Play();
            GameManager.Instance.isGameStarted = false;
            rb.isKinematic = true;
            collider.enabled = false;
            BonusLine.Instance.Move();
        }

        if (other.CompareTag("Sea") && _isAlive)
        {
            _isAlive = false;
            GameManager.Instance.RemovePlayer(this);
            crown.SetActive(false);
            Vector3 _waterFxPos = transform.position - Vector3.up * other.contactOffset;
            GameManager.Instance.CallWaterFx(_waterFxPos);
            Lose();
        }
    }

#endregion

}
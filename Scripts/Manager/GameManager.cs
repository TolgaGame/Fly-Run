using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{

#region OBJECTS - VARIABLES etc.

    [Header("Objects")]
    [Space]
    public GameObject coinFx;
    public PhysicMaterial slideMaterial;
    [HideInInspector] public Transform finish;
    [HideInInspector] public Transform waitPoint;
    [SerializeField] private ParticleSystem _waterFx;
    private Character _currentBestPlayer;
    private Character _player;


    [Header("Lists")]
    [Space]
    public List<Character> players;
    [SerializeField] List<string> AINames;
    [SerializeField] List<Transform> _spawnPoints;
    [SerializeField] AudioSource[] sound;

    [Header("UI Objects")]
    [Space]
    [SerializeField] GameObject finishPanel;
    [SerializeField] GameObject gameOverPanel;
    [SerializeField] GameObject menuPanel;
    [SerializeField] GameObject gamePanel;
    [SerializeField] Image fuelBar;
    [SerializeField] InputField nameField;
    [SerializeField] Text levelText;
    [SerializeField] Text coinText;
    [SerializeField] Text counterText;
    [SerializeField] Text playerRankText;
    [SerializeField] Text rankText;
    [SerializeField] Text winCoinText;

    [Header(("Variables"))]
    [Space]
    [HideInInspector] public bool isGameStarted;
    [HideInInspector] public int coin;
    public int level;
    private int bonusCoin;
    private string _playerName;
    [HideInInspector] public string message;


#endregion

#region CALLBACKS

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        GetDatas();
    }

    private void Start()
    {
        LevelGenerate();
        finish = GameObject.FindGameObjectWithTag("Bonus").transform;
        waitPoint = GameObject.FindGameObjectWithTag("Wait_Point").transform;
        UpdateName();
        _player = FindObjectOfType<PlayerControl>();
    }

    private void Update()
    {
        if (!isGameStarted)
        {
            if (nameField.text == "")
                _player.GetName("Player");
            else
                _player.GetName(nameField.text);
        }
        else
            CalculatingRank();
    }

#endregion

#region GAME STATES

    public void FinishLevel()
    {
        isGameStarted = false;
        sound[0].Play();
        StartCoroutine(FinishPanel());
        LevelUp(); 
    }
   
    public void GameOver()
    {
        if (_player._isAlive)
            _player.Lost();
        isGameStarted = false;
        sound[1].Play();
        StartCoroutine(OverPanel());
    }

    IEnumerator FinishPanel()
    {
        yield return new WaitForSeconds(3f);
        gamePanel.SetActive(false);
        finishPanel.SetActive(true);
        AddCoin(bonusCoin);
        winCoinText.text = "+ " + bonusCoin.ToString();       
    }

    IEnumerator OverPanel()
    {
        yield return new WaitForSeconds(1f);
        gamePanel.SetActive(false);
        gameOverPanel.SetActive(true);
        
    }
#endregion

#region SAVE AND LEVEL LOAD

    private void LevelGenerate()
    {
        int i = level - 1;
        // Generate level
        LevelGenerator.Instance.SpawnLevel(i);
        coinText.text = coin.ToString();
        levelText.text = "LEVEL " + level.ToString();
        SkinManager.Instance.ControllMoney();
        SpawnPlayersRandomly();
    }

    private void LevelUp()
    {
        level++;
        PlayerPrefs.SetInt("level", level);
    }

    public void SceneLoad()
    {
        SceneManager.LoadScene(0);
    }

    public void GetDatas()
    {
        // LEVEL
        if (PlayerPrefs.HasKey("level"))
        {
            level = PlayerPrefs.GetInt("level");
        }
        else
        {
            PlayerPrefs.SetInt("level", 1);
            level = 1;
        }

        // GEM
        coin = PlayerPrefs.GetInt("coin",0);

        // SOUND
        if (!PlayerPrefs.HasKey("sound"))
        {
            PlayerPrefs.SetInt("sound", 1);
        }

        // Name
        _playerName = PlayerPrefs.GetString("Player_Name", "Player");
    }

    public void AddCoin(int newCoin)
    {
        sound[2].Play();
        coin += newCoin;
        PlayerPrefs.SetInt("coin", coin);
        UpdateCoinText();
    }

#endregion

#region UI EVENTS

    public void StartButton()
    {
        menuPanel.SetActive(false);
        gamePanel.SetActive(true);
        StartCoroutine(StartCounter());
        _playerName = nameField.text;
        PlayerPrefs.SetString("Player_Name", _playerName); 
    }

    public void RestartButton()
    {
        SceneLoad();      
    }

    void UpdateCoinText()
    {
        coinText.text = coin.ToString();
    }


    #endregion

#region METHODS

    IEnumerator StartCounter()
    {
        StartCoroutine(SpawnPlayers());
        for (int i = 3; i > 0; i--)
        {
            counterText.text = i.ToString();
            counterText.rectTransform.DOScale(1, .5f).OnComplete(delegate
            {
                counterText.rectTransform.DOScale(.5f, .5f);
            });
            yield return new WaitForSeconds(1f);
        }
        StartGame();
        counterText.text = "Run";
        counterText.rectTransform.DOScale(1, .5f);
        yield return new WaitForSeconds(1f);
        counterText.text = "";
        playerRankText.gameObject.SetActive(true);
    }

    IEnumerator SpawnPlayers()
    {
        for (int i = 1; i < players.Count; i++)
        {
            players[i].gameObject.SetActive(true);
            players[i].GetComponent<AIPath>().LoadPath();
            int random = Random.Range(0, AINames.Count);
            players[i].GetName(AINames[random]);
            AINames.RemoveAt(random);
            yield return new WaitForSeconds(.25f);
        }  
    }

    private void StartGame()
    {
        isGameStarted = true;
        foreach (Character ch in players)
            ch.StartRunning();
    }

    public void FillFuelBar(float value)
    {
        fuelBar.DOFillAmount(value, .25f);
    }

    public void SetBonus(int multiplier)
    {
        bonusCoin = multiplier;
    }
    
    public void CallWaterFx(Vector3 _pos)
    {
        _waterFx.transform.position = _pos;
        _waterFx.Play();
    }

    public void RemovePlayer(Character ch)
    {
        players.Remove(ch);
    }

    public void CalculatingRank()
    {
        players = players.OrderBy(x => x.distance).ToList();
        if (_currentBestPlayer != null && _currentBestPlayer != players[0])
            _currentBestPlayer.crown.SetActive(false);
        _currentBestPlayer = players[0];
        _currentBestPlayer.crown.SetActive(true);
        SortPlayers();
    }

    private void SortPlayers()
    {
        int rank = players.IndexOf(_player) + 1;
        playerRankText.text = rank.ToString();
        if (rank == 1)
            rankText.text = "st";
        else if(rank == 2)
            rankText.text = "nd";
        else if (rank == 3)
            rankText.text = "rd";
        else
            rankText.text = "th";
    }

    private void UpdateName()
    {
        nameField.text = _playerName;
        players[0].GetName(_playerName);
    }

    private void SpawnPlayersRandomly()
    {
        int startPoint = 0;
        if (level <= 5)
        {
            players[0].transform.position = _spawnPoints[0].position;
            _spawnPoints.RemoveAt(0);
            startPoint++;
        }
        for (int i = startPoint; i < players.Count; i++)
        {
            int random = Random.Range(0, _spawnPoints.Count);
            players[i].transform.position = _spawnPoints[random].position;
            _spawnPoints.RemoveAt(random);
        }
    }

    public void ControllPlayers()
    {
        for (int i = 0; i < players.Count; i++)
        {
            if (!players[i]._isWin)
                players[i].Lost();
        }
    }

#endregion

}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkinManager : Singleton<SkinManager>
{

#region OBJECTS - VARIABLES etc.

    public SkinData skinData;

    [Header(("Variables"))]
    [Space]
    [SerializeField] private SkinnedMeshRenderer _renderer;
    [SerializeField] private Material[] _materials;
    [SerializeField] private Skin[] _skins;
    [SerializeField] private Button _buyButton;

    [SerializeField] private Text _costText;

    [SerializeField] private Color selectedColor;
    [SerializeField] private Color _defaultColor;
    [SerializeField] private Color _frameColor;

    [SerializeField] private Transform _coinText;
    [SerializeField] private Transform _menuPanel;
    [SerializeField] private Transform _shopPanel;

    [SerializeField] private GameObject _notText;
    private List<Image> _lockedFrames;
    private Image _lastCard;
    private Image _selectedCard;
    [SerializeField] private Image _warningIcon;

    [Header(("Variables"))]
    [Space]
    private int _skinIndex;
    private int _currentRandomNumber;
    private int _skinCost;
    
    private bool _haveMoney;
    private bool _allSkinsOpened;
    
    #endregion

#region CALLBACKS

    private void OnEnable()
    {
        _lockedFrames = new List<Image>();
        GetData();
    }

    private void Start()
    {
        ChangeSkin(skinData.skinIndex);
    }

#endregion

#region METHODS

    public void ChangeSkin(int _targetIndex)
    {
        _skinIndex = _targetIndex;
        skinData.skinIndex = _skinIndex;
        _renderer.material = _materials[_skinIndex];
        _skins[_skinIndex].Select();
    }

    public void BuySkin(int _cost)
    {
        _skinCost += 200;
        PlayerPrefs.SetInt("SkinCost", _skinCost);
        GameManager.Instance.AddCoin(-_cost);
        UpdateCostText();
    }

    public void Select(Image _selected)
    {
        if (_selectedCard != null)
            _selectedCard.color = _defaultColor;
        _selectedCard = _selected;
        _selectedCard.color = selectedColor;
    }

    public void ControllMoney()
    {
        bool _haveMoney = true;

        ControllSkins();
        if (GameManager.Instance.coin < _skinCost || _allSkinsOpened)
            _haveMoney = false;

        _buyButton.gameObject.SetActive(!_allSkinsOpened);
        _buyButton.interactable = _haveMoney;
        _warningIcon.enabled = _haveMoney;      
    }

    public void BuyButton()
    {
        ControllFrames();
        if (_buyButton.interactable)
        {
            BuySkin(_skinCost);
            StartCoroutine(OpenCard());
        }
    }

    private IEnumerator OpenCard()
    {
        _buyButton.interactable = false;
        _warningIcon.enabled = false;
        int _randomTimes = 8;
        if (_lockedFrames.Count == 1)
            _randomTimes = 1;
        for (int i = 0; i < _randomTimes; i++)
        {
            int _random = GetRandom();
            if (_lastCard != null && _lastCard != _selectedCard)
                _lastCard.color = _defaultColor;
            _lastCard = _lockedFrames[_random];
            _lastCard.color = _frameColor;
            yield return new WaitForSeconds(.25f);
        }
        _lastCard.GetComponentInChildren<Skin>().UnlockSkin();
        _lockedFrames.Remove(_lastCard);
        ControllMoney();
        if(_haveMoney)
            ControllFrames();
    }

    public void AddFrame(Image _frame)
    {
        _lockedFrames.Add(_frame);
    }

    public void PanelControll(bool _openPanel)
    {
        if (_openPanel)
            _coinText.SetParent(_shopPanel);
        else
            _coinText.SetParent(_menuPanel);
    }

    private int GetRandom()
    {
        int random = Random.Range(0, _lockedFrames.Count);

        if (_currentRandomNumber == random && _lockedFrames.Count > 1)
            _currentRandomNumber = GetRandom();
        else
            _currentRandomNumber = random;

        return _currentRandomNumber;
    }

    private void ControllFrames()
    {
        bool _allSkinsOpen = false;

        if (_lockedFrames.Count == 0)
            _allSkinsOpen = true;

        _buyButton.gameObject.SetActive(!_allSkinsOpen);
        _buyButton.interactable = !_allSkinsOpen;
        _warningIcon.enabled = !_allSkinsOpen;
    }

    private void GetData()
    {
        _skinCost = PlayerPrefs.GetInt("SkinCost", 300);
        UpdateCostText();
    }

    private void UpdateCostText()
    {
        if (GameManager.Instance.coin < _skinCost)
        {
            _costText.text = _skinCost.ToString();
            _notText.SetActive(true);
        }
        else
        {
            _costText.text = _skinCost.ToString();
            _notText.SetActive(false);
        }
        if (_allSkinsOpened)
            _buyButton.gameObject.SetActive(false);
    }

    private void ControllSkins()
    {
        _allSkinsOpened = true;
        for (int i = 0; i < _skins.Length; i++)
        {
            _skins[i].ControllSkin();
            if (!_skins[i]._isUnlocked)
            {
                _allSkinsOpened = false;
                break;
            }
        }
    }

#endregion
}

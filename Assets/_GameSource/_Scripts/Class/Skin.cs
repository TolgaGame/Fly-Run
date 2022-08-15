using UnityEngine;
using UnityEngine.UI;

public class Skin : MonoBehaviour
{
    #region Variables

    public bool _isUnlocked;

    [SerializeField] private int _myIndex;
    [SerializeField] private Sprite _skinSprite;
    [SerializeField] private Image _myFrame;
    [SerializeField] private Button _myButton;

    private Image _skinImage;

    #endregion

    #region MonoBehaviour Callbacks

    private void Awake()
    {
        _skinImage = GetComponent<Image>();
        ControllSkin();
    }

    private void Start()
    {
        if (!_isUnlocked)
            SkinManager.Instance.AddFrame(_myFrame);
        else
            _skinImage.sprite = _skinSprite;
    }

    #endregion

    #region Other Methods

    public void OnClick()
    {
        if (_isUnlocked)
        {
            SkinManager.Instance.ChangeSkin(_myIndex);
            Select();
        }
    }

    public void Select()
    {
        SkinManager.Instance.Select(_myFrame);
    }

    public void UnlockSkin()
    {
        _isUnlocked = true;
        _myButton.interactable = _isUnlocked;
        PlayerPrefs.SetInt(_myIndex.ToString(), 1);
        _skinImage.sprite = _skinSprite;

    }

    public void ControllSkin()
    {
        if(!_isUnlocked)
            _isUnlocked = PlayerPrefs.GetInt(_myIndex.ToString()) == 1 ? true : false;
        _myButton.interactable = _isUnlocked;
    }

    #endregion
}

using UnityEngine;
using DG.Tweening;

public class BonusLine : Singleton<BonusLine>
{
#region Variables

    [HideInInspector] public bool isReady;

#endregion

#region Other Methods

    public void Move()
    {
        transform.DOMoveY(46, 3f).OnComplete(delegate {
            isReady = true;
            PlayerControl _player = FindObjectOfType<PlayerControl>();
            _player.OpenCollider();
        });
    }

#endregion
}

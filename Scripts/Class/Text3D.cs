using UnityEngine;

public class Text3D : MonoBehaviour
{
    #region Variables

    Transform lookingTarget;

    #endregion

    #region MonoBehaviour Callbacks
    private void Start()
    {
        lookingTarget = Camera.main.transform;
    }

    private void LateUpdate()
    {
        transform.LookAt(lookingTarget);
    }

    #endregion

    #region Other Methods

    public void SetRandomName(string name)
    {
        transform.GetComponentInChildren<TextMesh>().text = name;
    }

    #endregion
}

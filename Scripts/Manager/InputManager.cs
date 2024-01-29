using UnityEngine;

public class InputManager : MonoBehaviour
{

#region Variables

    [SerializeField] Character player;
    [SerializeField] float smoothness;

    [Header(("Variables"))]
    [Space]
    private float startX;
    private float lastX;
    private float distance;
    private float rotateValue;
    private bool _isFirstClick;

#endregion

#region MonoBehaviour Callbacks

    private void Update()
    {
        if (GameManager.Instance.isGameStarted)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startX = Input.mousePosition.x;
                _isFirstClick = true;
            }
            else if (Input.GetMouseButton(0) && _isFirstClick)
            {
                lastX = Input.mousePosition.x;

                distance = lastX - startX;
                rotateValue += (distance / Screen.width) * smoothness;

                // Player movement
                if(Mathf.Abs(distance) > 0)
                    player.Turning(rotateValue);

                startX = lastX;
            }
        }
    }
#endregion

}
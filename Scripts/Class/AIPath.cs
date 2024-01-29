using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AIPath : MonoBehaviour
{

#region Variables

    [SerializeField] string targetTag;
    List<Transform> pathPoints;
    Transform path;
    Transform currentPoint;
    int currentIndex;

#endregion

 #region MonoBehaviour Callbacks

    private void Update()
    {
        var lookPos = currentPoint.position - transform.position;
        lookPos.y = 0;
        var rotation = Quaternion.LookRotation(lookPos);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, Time.deltaTime * 3f);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag(targetTag))
        {
            Destroy(other.gameObject.GetComponent<SphereCollider>());
            NextPoint();
        }
    }

#endregion

#region Other Methods

    public void NextPoint()
    {
        if (currentIndex == pathPoints.Count - 1)
        {
            Destroy(this);
        }
        else
        {
            currentIndex++;
            currentPoint = pathPoints[currentIndex];
        }
    }

    public void LoadPath()
    {
        path = GameObject.Find(gameObject.name + "_Path").transform;
        pathPoints = path.GetComponentsInChildren<Transform>().ToList();
        pathPoints.RemoveAt(0);
        currentIndex = -1;
        NextPoint();
    }

#endregion

}

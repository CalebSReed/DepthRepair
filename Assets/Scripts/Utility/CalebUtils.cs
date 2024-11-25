using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CalebUtils
{
    class CalebUtility
    {
        public Vector3 RandomPositionInRadius(Vector3 oldPos, int innerRadius, int outerRadius)
        {
            Vector3 newPos = oldPos;
            while (Vector3.Distance(newPos, oldPos) < innerRadius)
            {
                Vector3 randomPosition = Random.insideUnitSphere * outerRadius;
                randomPosition = new Vector3(randomPosition.x, 0, randomPosition.z);
                newPos = randomPosition + oldPos;
            }
            return newPos;
        }

        public Vector3 MoveAway(Vector3 current, Vector3 target, float maxDistanceDelta)
        {
            Vector3 a = target - current;
            float magnitude = a.magnitude;
            if (magnitude <= maxDistanceDelta || magnitude == 0f)
            {
                return target;
            }
            Vector3 newVal = current - a / magnitude * maxDistanceDelta;
            newVal = new Vector3(newVal.x, current.y, newVal.z);
            return newVal;
        }

        public List<GameObject> FindChildrenWithTag(Transform parent, string tag)
        {
            List<GameObject> _objectList = new List<GameObject>();
            foreach (Transform child in parent)
            {
                if (child.gameObject.CompareTag(tag))
                {
                    _objectList.Add(child.gameObject);
                }
            }
            return _objectList;
        }

        public void LookAt2D(Transform t, Vector3 worldPosition)
        {
            t.rotation = Quaternion.identity;
            t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - worldPosition.y, t.position.x - worldPosition.x) * 180 / Mathf.PI) - 180f);
        }
        public void LookAt2D(Transform t, Transform target)
        {
            t.rotation = Quaternion.identity;
            t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - target.position.y, t.position.x - target.position.x) * 180 / Mathf.PI) - 180f);
        }
        public void LookAwayFrom2D(Transform t, Vector3 worldPosition)
        {
            t.rotation = Quaternion.identity;
            t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - worldPosition.y, t.position.x - worldPosition.x) * 180 / Mathf.PI));
        }
        public void LookAwayFrom2D(Transform t, Transform target)
        {
            t.rotation = Quaternion.identity;
            t.Rotate(Vector3.forward, (Mathf.Atan2(t.position.y - target.position.y, t.position.x - target.position.x) * 180 / Mathf.PI));
        }

        public void RandomDirForceNoYAxis3D(Rigidbody rb, float force)
        {
            Vector3 direction = new Vector3((float)Random.Range(-1000, 1000), 0, (float)Random.Range(-1000, 1000));
            rb.AddForce(direction * force);
        }

        public GameObject GetParentOfTriggerCollider(Collider collider)//we always check for triggers and all of them are organized to be the child of their base gameobject (where their scripts are)
        {
            if (collider.transform.parent == null)
            {
                return null;
            }
            return collider.transform.parent.gameObject;
        }
    }
}

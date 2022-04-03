using System.Collections;
using UnityEngine;

namespace Assets.Scripts.UI
{
    public class WorldUILookAtCamera : MonoBehaviour
    {
        private static Camera cam;

        private void Start()
        {
            if (cam == null) { cam = Camera.main; }

            GetComponent<Canvas>().worldCamera = cam;
        }

        private void LateUpdate()
        {
            if(cam == null) { cam = Camera.main; }

            Vector3 direction = transform.position - cam.transform.position;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = lookRotation;
        }
    }
}
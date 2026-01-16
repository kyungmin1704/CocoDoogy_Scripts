using UnityEngine;
using UnityEngine.UI;

namespace CocoDoogy.Test
{
    public class Spinner : MonoBehaviour
    {
        [SerializeField] private Image spinner;
        private float speed = -100f;
        private void Update()
        {
            spinner.rectTransform.Rotate(0, 0, speed * Time.deltaTime, Space.Self);
        }
    }
}

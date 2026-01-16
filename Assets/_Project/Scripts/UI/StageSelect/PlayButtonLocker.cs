using System.Collections;
using UnityEngine;

namespace CocoDoogy.UI.StageSelect
{
    public class PlayButtonLocker: MonoBehaviour
    {
        private CommonButton button;


        IEnumerator Start()
        {
            button = GetComponent<CommonButton>();
            button.interactable = false;
            
            yield return new WaitForSeconds(1f);
            
            button.interactable = true;
        }
    }
}
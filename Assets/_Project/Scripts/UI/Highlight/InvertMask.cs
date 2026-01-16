using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Rendering;
using System.Collections;

namespace CocoDoogy.UI
{
    public class InvertMask : Image
    {
        protected override void Start()
        {
            base.Start();
            StartCoroutine(Fix());
        }

        private IEnumerator Fix()
        {
            yield return null;
            maskable = false;
            maskable = true;
        }


        public override Material materialForRendering
        {
            get
            {
                Material result = new Material(base.materialForRendering);
                result.SetInt("_StencilComp", (int)CompareFunction.NotEqual);
                return result;
            }
        }
    }
}
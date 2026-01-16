using CocoDoogy.CameraSwiper;
using CocoDoogy.Core;
using System.Collections.Generic;
using UnityEngine;

namespace CocoDoogy.LobbyObject
{
    // TEST 코드입니다.
    public class ThemaUpdateTest : MonoBehaviour
    {
        private int index;

        [Header("테마별 오브젝트들")]
        public GameObject[] forestObjects;
        public GameObject[] oasisObjects;
        public GameObject[] iceObjects;
        public GameObject[] desertObjects;

        // 현재 테마에 따라 사용할 오브젝트 리스트
        private List<GameObject> currentObjects;

        private void Awake()
        {
            // 리스트 생성 (null 방지)
            currentObjects = new List<GameObject>();
        }

        private void Start()
        {
            index = 0;

            PageCameraSwiper.OnEndPageChanged += ChangeTheme;

            // InitObjects(Theme.Forest);
            // 전부 비활성화
            SetAllInactive();
        }

        private void Update()
        {
            // Space를 누를 때마다 오브젝트 하나씩 활성화
            if (Input.GetKeyDown(KeyCode.Space))
            {
                if (index < currentObjects.Count)
                {
                    currentObjects[index].SetActive(true);
                    index++;
                }
            }
        }

        /// <summary>
        /// 현재 테마의 오브젝트 배열을 currentObjects에 세팅
        /// </summary>
        void InitObjects(Theme theme)
        {
            // 기존 리스트 비우기
            currentObjects.Clear();

            // 테마에 따라 배열 선택 후 리스트에 추가
            switch (theme)
            {
                case Theme.Forest:
                    if (forestObjects != null)
                        currentObjects.AddRange(forestObjects);
                    break;

                case Theme.Water:
                    if (oasisObjects != null)
                        currentObjects.AddRange(oasisObjects);
                    break;

                case Theme.Snow:
                    if (iceObjects != null)
                        currentObjects.AddRange(iceObjects);
                    break;

                case Theme.Sand:
                    if (desertObjects != null)
                        currentObjects.AddRange(desertObjects);
                    break;
            }

            // 인덱스 초기화
            index = 0;
        }

        /// <summary>
        /// currentObjects에 들어있는 애들 전부 비활성화
        /// </summary>
        void SetAllInactive()
        {
            foreach (var obj in currentObjects)
            {
                if (obj != null)
                    obj.SetActive(false);
            }
        }

        /// <summary>
        /// 외부(카메라 스와이퍼 등)에서 테마 변경 호출용
        /// </summary>
        public void ChangeTheme(Theme theme)
        {
            InitObjects(theme);
            SetAllInactive();
        }
    }
}

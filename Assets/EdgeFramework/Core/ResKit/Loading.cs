using EdgeFramework;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EdgeFramework
{
    public class Loading : MonoBehaviour
    {
        private Slider slider;
        private Text text;

        private void Awake()
        {
            slider = transform.Find("progress/Slider").GetComponent<Slider>();
            text = transform.Find("progress/progressTxtValue").GetComponent<Text>();
         
        }
        private void Update()
        {
            slider.value = GameMapManager.LoadingProgress / 100.0f;
            text.text = string.Format("{0}%", GameMapManager.LoadingProgress);
        }

    }
}
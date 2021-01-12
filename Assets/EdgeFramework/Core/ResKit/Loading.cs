/****************************************************
	文件：Loading.cs
	Author：JaydenWood
	E-Mail: w_style047@163.com
	GitHub: https://github.com/git-Jayden/EdgeFramework.git
	Blog: https://www.jianshu.com/u/9131c2f30f1b
	Date：2021/01/11 17:00   	
	Features：
*****************************************************/
using UnityEngine;
using UnityEngine.UI;

namespace EdgeFramework.Res
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
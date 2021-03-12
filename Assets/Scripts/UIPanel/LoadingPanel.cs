using EdgeFramework.UI;
using UnityEngine;
using UnityEngine.UI;

public class LoadingPanel : MonoBehaviour
{
    [HideInInspector]
    public  Slider slider;
    [HideInInspector]
    public  Text text;

    private void Awake()
    {
      
        slider = transform.Find("BG/Slider").GetComponent<Slider>();
        text = transform.Find("BG/Text").GetComponent<Text>();
    }


}

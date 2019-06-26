using BindingsRx.Bindings;
using UnityEngine;
using UnityEngine.UI;

namespace Ls_Mobile.Example
{
    public class BindingsRxExample : MonoBehaviour
    {
        InputField mInputField;
        Text mText;

        void Start()
        {
            mInputField = transform.Find("InputField").GetComponent<InputField>();
            mText = transform.Find("Text").GetComponent<Text>();

            mInputField.BindTextTo(() => mText.text, text => mText.text = text);
        }
    }
}
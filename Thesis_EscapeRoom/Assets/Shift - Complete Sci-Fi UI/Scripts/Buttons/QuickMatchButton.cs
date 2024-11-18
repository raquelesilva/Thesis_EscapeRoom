using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace Michsky.UI.Shift
{
    public class QuickMatchButton : MonoBehaviour
    {
        [Header("Text")]
        public bool useCustomText = false;
        public string buttonTitle = "My Title";

        [Header("Image")]
        public bool useCustomImage = false;
        public Sprite backgroundImage;

        TextMeshProUGUI titleText;
        Image image1;

        void Start()
        {
            if (useCustomText == false)
            {
                Transform title = gameObject.transform.Find("Content/Title");
                if (title != null)
                {
                    titleText = title.GetComponent<TextMeshProUGUI>();
                    titleText.text = buttonTitle;
                }
            }

            if (useCustomImage == false)
            {
                Transform background = gameObject.transform.Find("Content/Background");
                if (background != null)
                {
                    image1 = background.GetComponent<Image>();
                    image1.sprite = backgroundImage;
                }
            }
        }
    }
}
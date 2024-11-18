using TMPro;
using UnityEngine;

public class LinkToMail : MonoBehaviour
{
    private readonly string mail = "escaperoom.contact@kendirstudios.pt";
    [SerializeField, Tooltip("The UI GameObject having the TextMesh Pro component.")]
    private TMP_Text text;

    public void OpenLink()
    {
        // First, get the index of the link clicked. Each of the links in the text has its own index.
        int linkIndex = TMP_TextUtilities.FindIntersectingLink(text, Input.mousePosition, null);

        if (linkIndex < 0)
        {
            return;
        }

        // As the order of the links can vary easily (e.g. because of multi-language support),
        // you need to get the ID assigned to the links instead of using the index as a base for our decisions.
        // you need the LinkInfo array from the textInfo member of the TextMesh Pro object for that.
        string linkId = text.textInfo.linkInfo[linkIndex].GetLinkID();

        Debug.Log(linkId);

        // Now finally you have the ID in hand to decide what to do. Don't forget,
        // you don't need to make it act like an actual link, instead of opening a web page,
        // any kind of functions can be called.
        string url = linkId switch
        {
            "mail" => mail
        };

        Debug.Log($"URL clicked: linkInfo[{linkIndex}].id={linkId}   ==>   url={url}");

        // Let's see that web page!
        Application.OpenURL(url);
    }
}
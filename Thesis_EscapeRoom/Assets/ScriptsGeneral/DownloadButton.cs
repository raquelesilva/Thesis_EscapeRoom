using System.Runtime.InteropServices;
using UnityEngine;

public class DownloadButton : MonoBehaviour
{
    public void OnDownloadButtonClick(string fileName)
    {
        var downloadLink = Application.streamingAssetsPath + "/" + "Downloadables/" + fileName;
        print(downloadLink);
        print(fileName);
        DownloadFile(downloadLink, fileName);

        //Implementar script de download de ficheiros em javascript
    }
        [DllImport("__Internal")]
        private static extern void DownloadFile(string downloadLink, string fileName);
}
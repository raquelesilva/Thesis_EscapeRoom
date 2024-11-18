using PathCreation;
using PathCreation.Kendir;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class EndTrigger : MonoBehaviour
{
    [SerializeField] private List<string> messages = new()
    {
        "Uh? O quê? Isto.. Isto não pode estar bem...",
        "A realidade ficou assim tão chafurdada?",
        "Estou... preso...",
        "O universo como conhecemos não pode ser salvo...",
        "Bem... Ao menos temos bolo!..."
    };

    private NotificationManager notificationManager;
    private Color32 colorRight;

    [SerializeField] private PathCreatorFromTransforms path;
    [SerializeField] private PathFollower follower;

    [SerializeField] private Animator blackOut;
    [SerializeField] private TextMeshProUGUI endGameMessage;
    [SerializeField, TextArea(6, 6)] private string demoFinalText;
    [SerializeField, TextArea(6, 6)] private string finalText;

    private void Start()
    {
        notificationManager = NotificationManager.instance;
        colorRight = notificationManager.GetColorRight();
    }

    public void OnReachEnd()
    {
        FindAnyObjectByType<FirstPersonController>().SetPlayerState(3);
        path.UpdatePath();
        follower.FollowPath(path);
    }

    public void TriggerMessage()
    {
        if (messages.Count > 0)
        {
            notificationManager.SetMessage(messages[0], colorRight);
            messages.RemoveAt(0);

            if (messages.Count == 0)
            {
                StartCoroutine(OpenEndGameMessage());
            }
        }
    }

    private IEnumerator OpenEndGameMessage()
    {
        blackOut.enabled = true;

        yield return new WaitForSeconds(2f);

        endGameMessage.transform.parent.gameObject.SetActive(true);

        if (ScenePlayersManager.instance.IsDemo())
        {
            endGameMessage.text = demoFinalText;
        }
        else
        {
            endGameMessage.text = finalText;
        }
    }
}
using CoreSystems.Extensions.Attributes;
using DG.Tweening;
using System.Collections;
using UnityEngine;

public class SurveillanceCamera : MonoBehaviour
{
    [SerializeField] FirstPersonController player;
    [SerializeField] float detectPlayeRange;
    [SerializeField] float smoothSpeed;

    [Header("Debug")]
    [SerializeField, Disable()] private bool canSeePlayer;
    [SerializeField, Disable()] float distanceDisplay;


    private MeshRenderer meshRenderer;

    private void Start()
    {
        player = FirstPersonController.instance;
        meshRenderer = GetComponent<MeshRenderer>();

        InvokeRepeating(nameof(LoopTimer), 1f, 1f);
    }

    private void Update()
    {
        if (canSeePlayer)
        {
            // Calculate the direction to the target
            Vector3 direction = player.transform.position - transform.position;
            // Calculate the rotation needed to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            // Smoothly interpolate between the current rotation and the target rotation
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, smoothSpeed * Time.deltaTime);
        }
    }

    private void LoopTimer()
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        canSeePlayer = distance < detectPlayeRange;

        if (!canSeePlayer)
        {
            if (!isStarted)
            {
                StopAllCoroutines();
                rotateTween.Kill();
                StartCoroutine(RotateCamera());
                isStarted = true;
            }
        }
        else
        {
            rotateTween.Kill();
            StopAllCoroutines();
            isStarted = false;
        }

        distanceDisplay = distance;
    }

    private void OnDrawGizmosSelected()
    {
        if (canSeePlayer)
        {
            Gizmos.color = Color.green;
        }
        else
        {
            Gizmos.color = Color.red;
        }
        Gizmos.DrawWireSphere(transform.position, detectPlayeRange);
    }

    float range = 60;
    bool isStarted;
    Tween rotateTween;

    private IEnumerator RotateCamera()
    {
        rotateTween = gameObject.transform.DOLocalRotate(new Vector3(0f, range, 0f), 10).SetEase(Ease.InOutQuad);
        yield return rotateTween.WaitForCompletion();
        range = range * -1;
        yield return new WaitForSeconds(2f);
        StartCoroutine(RotateCamera());
        yield return null;
    }
}
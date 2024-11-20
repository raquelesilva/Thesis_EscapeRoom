using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

namespace CoreSystems.UI.Animations
{
    public class UIAnimations : MonoBehaviour
    {
        [Header("Animation")]
        [SerializeField] private bool playOnEnable;
        [SerializeField] private GameObject target;
        [SerializeField] private Animations selectedAnimation;
        [SerializeField] private float animationDuration;

        [Header("Events")]
        [SerializeField] private UnityEvent onEndOpenAnimation;
        [SerializeField] private UnityEvent onEndCloseAnimation;

        private bool isAnimating;
        private Vector2 originalLocalPos = new();
        private Vector3 originalScale = new();

        //Private
        private Tween currentTween = null;
        private Tween altCurrentTween = null;


        private void GetDependencies()
        {
            originalLocalPos = target.transform.localPosition;
            if (originalScale != Vector3.zero)
            {
                originalScale = target.transform.localScale;
            }
        }

        private void OnEnable()
        {
            GetDependencies();

            if (playOnEnable)
            {
                PlayAnimation();
            }
        }

        public void PlayAnimation()
        {
            SelectAnimation(false, false);
        }
        public void PlayReversedAnimation(bool turnOff)
        {
            SelectAnimation(true, turnOff);
        }

        [ContextMenu("Play Reversed Animation")]
        public void PlayReversedAnimation()
        {
            SelectAnimation(true, false);
        }

        private void SelectAnimation(bool reversed, bool turnOff)
        {
            GetDependencies();

            currentTween.Kill(true);
            switch (selectedAnimation)
            {
                case Animations.simpleScale:
                    SimpleScale(reversed, turnOff);
                    break;
                case Animations.simpleAlpha:
                    SimpleAlpha(reversed, turnOff);
                    break;
                case Animations.rightSlideFade:
                    SlideFade(reversed, turnOff, selectedAnimation);
                    break;
                case Animations.leftSlideFade:
                    SlideFade(reversed, turnOff, selectedAnimation);
                    break;
            }
        }

        private void SimpleScale(bool reversed, bool turnOff)
        {
            isAnimating = true;

            if (reversed)
            {
                target.transform.localScale = Vector3.one;
                currentTween = target.transform.DOScale(Vector3.zero, animationDuration).SetEase(Ease.InOutCirc).OnComplete(() =>
                {
                    isAnimating = false;
                    onEndCloseAnimation?.Invoke();
                    if (turnOff)
                    {
                        gameObject.SetActive(false);
                    }
                });
            }
            else
            {
                target.transform.localScale = Vector3.zero;
                currentTween = target.transform.DOScale(Vector3.one, animationDuration).SetEase(Ease.InOutCirc).OnComplete(() =>
                {
                    isAnimating = false;
                    onEndOpenAnimation?.Invoke();
                });
            }
        }

        private void SimpleAlpha(bool reversed, bool turnOff)
        {
            isAnimating = true;
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                canvasGroup = target.AddComponent<CanvasGroup>();
            }

            if (!reversed)
            {
                canvasGroup.alpha = 0f;
                currentTween = canvasGroup.DOFade(1, animationDuration).SetEase(Ease.InOutQuint).OnComplete(() =>
                {
                    isAnimating = false;
                    //onEndOpenAnimation?.Invoke();
                    //Debug.Log("END SIMPLE ALPHA");
                });
            }
            else
            {
                canvasGroup.alpha = 1f;
                currentTween = canvasGroup.DOFade(0, animationDuration).SetEase(Ease.InOutQuint).OnComplete(() =>
                {
                    isAnimating = false;
                    onEndCloseAnimation?.Invoke();
                    if (turnOff)
                    {
                        gameObject.SetActive(false);
                    }
                });
            }
        }

        private void SlideFade(bool reversed, bool turnOff, Animations type)
        {
            isAnimating = true;
            CanvasGroup canvasGroup = target.GetComponent<CanvasGroup>();
            RectTransform rectTransform = target.GetComponent<RectTransform>();
            Vector2 targetPos = originalLocalPos;

            int movementIncrement = type switch
            {
                Animations.leftSlideFade => -30,
                Animations.rightSlideFade => 30,
                _ => 30
            };

            if (canvasGroup == null)
            {
                canvasGroup = target.AddComponent<CanvasGroup>();
            }

            if (!reversed)
            {
                canvasGroup.alpha = 0f;
                rectTransform.localPosition = rectTransform.localPosition + new Vector3(movementIncrement, 0);

                currentTween = rectTransform.DOLocalMoveX(targetPos.x, animationDuration * 0.7f).SetEase(Ease.OutCubic);
                currentTween = canvasGroup.DOFade(1, animationDuration).SetEase(Ease.InOutQuint).OnComplete(() =>
                {
                    isAnimating = false;
                    onEndOpenAnimation?.Invoke();
                    Debug.Log("END SLIDE FADE");
                    rectTransform.localPosition = originalLocalPos;
                });
            }
            else
            {
                canvasGroup.alpha = 1f;

                rectTransform.localPosition = originalLocalPos;

                targetPos = originalLocalPos - new Vector2(movementIncrement, 0);

                rectTransform.DOLocalMoveX(targetPos.x, animationDuration).SetEase(Ease.OutCubic);
                currentTween = canvasGroup.DOFade(0, animationDuration).SetEase(Ease.InOutQuint).OnComplete(() =>
                {
                    isAnimating = false;
                    onEndCloseAnimation?.Invoke();
                    if (turnOff)
                    {
                        gameObject.SetActive(false);
                    }
                });

            }
        }
    }
    enum Animations
    {
        simpleScale,
        simpleAlpha,
        leftSlideFade,
        rightSlideFade
    }

}

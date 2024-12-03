using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CoreSystems.Managers
{
	public class NotificationManager : MonoBehaviour
	{
		// ### A dura��o total da anima��o quando este abre/fecha
		[SerializeField] float duration = 1f;

		// ### Tipo de fun��o que ter� um efeito especifico na anima��o de Easing
		[SerializeField] EaseFunctions.Ease easeFunction;

		// ### Refer�ncia do objeto da janela
		[SerializeField] Transform window = null;

		// ### Refer�ncia do objeto que ter� mensagem
		[SerializeField] TextMeshProUGUI message = null;

		[SerializeField] Color32 colorRight = Color.white;
		[SerializeField] Color32 colorWrong = Color.white;

		public static NotificationManager instance;

        private void Awake()
        {
            instance = this;

            window.transform.localScale = new Vector3(0, 0, 0);
        }

        /// <summary>
        /// Define a mensagem, a cor da mensagem e abre a janela da notifica��o
        /// </summary>
        public void SetMessage(string text, Color32 messageColor, float duration=1f)
		{
			this.duration = duration;
            window.gameObject.SetActive(false);
            StopCoroutine(nameof(StartMessage));
			window.gameObject.SetActive(true);
			window.localScale = Vector3.zero;

			message.text = text;
			message.color = messageColor;
			var img = window.gameObject.GetComponent<Image>();
			if(img == null)
			{
				Debug.Log("O componente Image est� em falta no Notification Manager.");
				return;
			}

			//img.color = messageColor;

			StartCoroutine(nameof(StartMessage));
		}

		/// <summary>
		/// Retorna a dura��o definida
		/// </summary>
		public float GetDuration() 
		{ 
			return duration;
		}

		public Color32 GetColorRight()
		{
			return colorRight;
		}

		public Color32 GetColorWrong()
		{
			return colorWrong;
		}

		/// <summary>
		/// Cont�m o processo de anima��o da janela
		/// </summary>
		private IEnumerator StartMessage()
		{
			float currentDuration = 0f;
			float currentPercentage = 0f;

			while(currentPercentage < 1f)
			{
				currentDuration += Time.deltaTime;

				currentPercentage = currentDuration / duration;

				var scaleMode = EaseFunctions.GetEasingFunction(easeFunction);

				float value = scaleMode(0f, 1f, currentPercentage);

				window.localScale = new Vector3(value, value, value);

				yield return null;
			}

			yield return new WaitForSeconds(5);

			StartCoroutine(CloseMessage());
		}

		public void DisableWindow()
		{
			window.gameObject.SetActive(false);
		}

        private IEnumerator CloseMessage()
        {
            float currentDuration = 0f;
            float currentPercentage = 0f;

            while (currentPercentage < 1f)
            {
                currentDuration += Time.deltaTime;

                currentPercentage = currentDuration / duration;

                var scaleMode = EaseFunctions.GetEasingFunction(easeFunction);

                float value = scaleMode(1f, 0f, currentPercentage);

                window.localScale = new Vector3(value, value, value);

                yield return null;
            }

			window.transform.localScale = new Vector3(0, 0, 0);
        }
    }
}
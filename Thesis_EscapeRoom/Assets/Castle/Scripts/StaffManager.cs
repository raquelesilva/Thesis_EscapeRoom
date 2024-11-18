using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using DG.Tweening;

public class StaffManager : MonoBehaviour
{
    bool castSpell;
    int chandeliersLightened = 0;

    [SerializeField] GameObject spellPrefab;
    [SerializeField] List<GameObject> dots = new();

    [SerializeField] GameObject cinematicCamera;

    public static StaffManager instance;

    private void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        if (castSpell)
        {
            castSpell = false;
            CastSpell();
        }
    }

    public void StaffSpell(InputAction.CallbackContext context)
    {
        castSpell = context.performed;
    }

    public void CastSpell()
    {
        GameObject spell = Instantiate(spellPrefab);
        StartCoroutine(Spell(spell));
    }

    IEnumerator Spell(GameObject spell)
    {
        float timer = 0f;
        float timeToAnimate = 5;
        spell.transform.position = this.transform.position;
        spell.transform.rotation = Camera.main.transform.rotation;
        while (timer < timeToAnimate)
        {
            spell.transform.position += spell.transform.forward * .1f;
            timer += Time.deltaTime * 2;
            yield return null;
        }
        Destroy(spell);
    }

    public void CheckChandelier()
    {
        chandeliersLightened++;

        if (chandeliersLightened == 2)
        {
            cinematicCamera.SetActive(true);

            StartCoroutine(MoveDots());
        }
    }

    IEnumerator MoveDots()
    {
        for (int i = 0; i < dots.Count; i++)
        {
            yield return new WaitForSeconds(.7f);
            dots[i].SetActive(true);
            //dots[i].transform.DOLocalMoveY(0.08f, 1f);

            if (i < 3)
            {
                dots[i].transform.DOBlendableLocalMoveBy(Vector3.up * (-0.08f), 1f);
            }
            else
            {
                dots[i].transform.DOBlendableLocalMoveBy(Vector3.up * 0.08f, 1f);
            }
        }
    }
}
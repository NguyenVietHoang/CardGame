using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class CardModel : MonoBehaviour
{
    public RuleControl rule;

    public TextMeshProUGUI atkValue;
    public TextMeshProUGUI defValue;
    public TextMeshProUGUI cardDetail;
    public SpriteRenderer photo;

    public Animator anim;
    

    public EventTrigger cardEvt;
    //Moving Card Parameter
    Vector3 orgPos;
    Vector3 targetPos;
    Vector3 currScale;
    Vector3 targetScale;
    float step;
    bool onMove;

    //Card Control Parameter
    [HideInInspector]
    public bool onLockCard;
    [HideInInspector]
    public int teamID;//Id = 0: None, id = 1: Player, id = -1: Bot
    public int currAtk { get; private set; }
    public int curDef { get; private set; }

    private void Awake()
    {
        EventTrigger.Entry entry = new EventTrigger.Entry
        {
            eventID = EventTriggerType.PointerUp
        };
        entry.callback.AddListener((data) => { OnTriggerDownEvt((PointerEventData)data); });
        cardEvt.triggers.Add(entry);

        ToFaceDown();
        onMove = false;

        //MoveCard(new Vector3(0,0.5f,0), new Vector3(0.5f, 0.5f, 0.5f));
    }

    private void Update()
    {
        if(onMove)
        {
            if(step < 1)
            {
                Vector3 newPos = Vector3.Lerp(orgPos, targetPos, step);
                Vector3 newScale = Vector3.Lerp(currScale, targetScale, step);

                gameObject.transform.position = newPos;
                gameObject.transform.localScale = newScale;

                step += Time.deltaTime;
            }
            else
            {
                gameObject.transform.position = targetPos;
                gameObject.transform.localScale = targetScale;
                onMove = false;
                step = 0;
            }
        }
    }
    //Set the Card UI with the CardObject data
    public void SetCardData(CardObject obj)
    {
        atkValue.text = obj.ATK.ToString();
        defValue.text = obj.DEF.ToString();
        cardDetail.text = obj.Detail;
        photo.sprite = obj.Photo;

        curDef = obj.DEF;
        currAtk = obj.ATK;
    }

    //Update Data and UI while have dmg
    public void TakeDmg(int dmg)
    {
        if(curDef > dmg)
        {
            curDef -= dmg;
            defValue.text = curDef.ToString();
            //ToAttacked();
            StartCoroutine(CardBeingAttacked());
        }
        else
        {
            curDef = 0;
            //ToDeath();
            StartCoroutine(CardBeingAttacked());
        }
    }

    public void OnTriggerDownEvt(PointerEventData eventData)
    {
        if(!onLockCard)
        {
            Debug.Log("On Trigger down");
            rule.onCardClicked?.Invoke(this);
        }
    }

    public void MoveCard(Vector3 _targetPos, Vector3 scale)
    {
        orgPos = gameObject.transform.position;
        targetPos = _targetPos;
        //Debug.Log(orgPos + "_" + targetPos);
        currScale = gameObject.transform.localScale;
        targetScale = scale;
        onMove = true;
        step = 0;
    }

    #region Card Animation Control
    public void ToFaceDown()
    {
        anim.SetTrigger("ToFaceDown");
    }

    public void ToFaceUp()
    {
        anim.SetTrigger("ToFaceUp");
    }

    public void ToAttacked()
    {
        anim.SetTrigger("ToAttacked");
    }

    public void ToDeath()
    {
        anim.SetTrigger("ToDeath");
    }

    IEnumerator CardBeingAttacked()
    {
        ToAttacked();
        yield return new WaitForSeconds(0.8f);
        if (curDef != 0)
            ToFaceUp();
        else
            ToDeath();
    }
    #endregion
}

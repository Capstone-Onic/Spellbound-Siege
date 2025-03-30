using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Spellbound;

public class CardDisplay : MonoBehaviour
{

    public Card cardData;

    public Image cardImage;
    public TMP_Text nameText;
    public TMP_Text costText;
    public TMP_Text damageText;
    public Image[] typeImages;

    private Color[] cardColors = {
        Color.red, //fire
        Color.blue, //water
        Color.cyan, //ice
        Color.gray, //earth
        Color.black, //dark
        Color.yellow //light
    };

    private Color[] typeColors = {
        Color.red, //fire
        Color.blue, //water
        Color.cyan, //ice
        Color.gray, //earth
        Color.black, //dark
        Color.yellow //light
    };

    void Start()
    {
        UpdateCardDisplay();
    }

    public void UpdateCardDisplay()
    {
        //Update the main card image color based on the firt card type
        cardImage.color = cardColors[(int)cardData.cardType[0]];

        nameText.text = cardData.cardName;
        costText.text = cardData.cost.ToString();
        damageText.text = $"{cardData.damageMin} - {cardData.damageMax}";

        //Update type images
        for(int i=0; i<typeImages.Length; i++) //타입이 여러개일 경우 타입 이미지 여러개를 표시
        {
            if (i<cardData.cardType.Count) {
                typeImages[i].gameObject.SetActive(true);
                typeImages[i].color = typeColors[(int)cardData.cardType[i]];
            }
            else {
                typeImages[i].gameObject.SetActive(false);
            }
        }
    }
}
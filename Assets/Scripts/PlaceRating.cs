using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlaceRating : MonoBehaviour
{
   public TextMeshProUGUI Name;
   public TextMeshProUGUI Place;
   public TextMeshProUGUI Score;
   public Image Cup;
    public GameObject Stroke;

   public void Init(string nickname, string place, string score, Color cup)
   {
      Name.text = nickname;
      Place.text = place;
      Score.text = score;
        if (nickname == "YOU!")
        {
            Stroke.SetActive(true);
        }
        if (cup == Color.white) return;
      Cup.gameObject.SetActive(true);
      Cup.color = cup;
       
   }
}

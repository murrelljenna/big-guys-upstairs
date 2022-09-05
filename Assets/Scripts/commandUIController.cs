using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

using UnityEngine.UI;
using UnityEngine.Events;

public class commandUIController : MonoBehaviour
{
    List<GameObject> cards = new List<GameObject>();
    private int count;

    // Start is called before the first frame update
    void Start()
    {
        cards = GameObject.FindGameObjectsWithTag("unitCard").OfType<GameObject>().ToList();
        cards.Sort((x, y) => x.name.CompareTo(y.name));
        cards.ForEach(card => {
            card.SetActive(false);
        });
    }

    public void addCard(Unit unit) {
        foreach (GameObject card in cards) {
            if (card.activeSelf == false) {
                setCard(card, unit);
                count++;
                this.transform.Find("UnitCount").Find("Selected_Count").gameObject.GetComponent<Text>().text = count.ToString();
                break;
            }
        }
    }

    private void setCard(GameObject card, Unit unit) {
        card.SetActive(true);

        card.GetComponent<syncHealth>().unit = unit;

        Transform[] trans = card.GetComponentsInChildren<Transform>(true);
        foreach (Transform t in trans) {
            if (t.gameObject.name == "Militia" || t.gameObject.name == "Light Infantry" || t.gameObject.name == "Archer") {
                t.gameObject.SetActive(false);
            }
        }

        card.transform.Find(unit.prefabName).gameObject.SetActive(true);
    }

    public void removeCard(Unit unit) {
        bool cardFound = false;
        GameObject lastCard = GameObject.Find("you're not so fucking smart compiler");

        foreach (GameObject card in cards) {
            if (card.gameObject.activeSelf == true && cardFound == true && lastCard != null) {
                setCard(lastCard, card.GetComponent<syncHealth>().unit);
                card.SetActive(false);
            } else if (card.activeSelf == true && card.GetComponent<syncHealth>().unit.id == unit.id) {
                card.transform.Find("Militia").gameObject.SetActive(true);
                card.transform.Find("Light Infantry").gameObject.SetActive(true);
                card.transform.Find("Archer").gameObject.SetActive(true);

                card.SetActive(false);
                count--;
                this.transform.Find("UnitCount").Find("Selected_Count").gameObject.GetComponent<Text>().text = count.ToString();
                cardFound = true;         
            }

            lastCard = card;
        }
    }

    public void clearCards() {
        cards.ForEach(card => {
            count = 0;
            this.transform.Find("UnitCount").Find("Selected_Count").gameObject.GetComponent<Text>().text = count.ToString();

            card.transform.Find("Militia").gameObject.SetActive(true);
            card.transform.Find("Light Infantry").gameObject.SetActive(true);
            card.transform.Find("Archer").gameObject.SetActive(true);

            card.SetActive(false);
        });
    }
}

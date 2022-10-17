using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using game.assets.ai;
using UnityEngine.UI;
using UnityEngine.Events;
using static game.assets.utilities.GameUtils;

namespace game.assets.ui
{
    public class CommandUIController : MonoBehaviour
    {
        List<GameObject> cards = new List<GameObject>();
        private int count;
        void Start()
        {
            cards = GameObject.FindGameObjectsWithTag("unitCard").OfType<GameObject>().ToList();
            cards.Sort((x, y) => x.name.CompareTo(y.name));
            cards.ForEach(card =>
            {
                card.SetActive(false);
            });
        }

        public void addCard(Health unit)
        {
            foreach (GameObject card in cards)
            {
                if (card.activeSelf == false)
                {
                    setCard(card, unit);
                    count++;
                    this.transform.Find("UnitCount").Find("Selected_Count").gameObject.GetComponent<Text>().text = count.ToString();
                    unit.onZeroHP.AddListener(removeCard);
                    break;
                }
            }
        }

        private void setCard(GameObject card, Health unit)
        {
            card.SetActive(true);

            card.GetComponent<CommandUICardController>().setUnit(unit);

            Transform[] trans = card.GetComponentsInChildren<Transform>(true);
            foreach (Transform t in trans)
            {
                if (t.gameObject.name == "Worker" || t.gameObject.name == "Light Infantry" || t.gameObject.name == "Archer")
                {
                    t.gameObject.SetActive(false);
                }
            }

            card.transform.Find(normalizePrefabName(unit.gameObject.name)).gameObject.SetActive(true);
        }

        public void removeCard(Health unit)
        {
            bool cardFound = false;
            GameObject lastCard = GameObject.Find("you're not so fucking smart compiler");

            foreach (GameObject card in cards)
            {
                if (card.gameObject.activeSelf == true && cardFound == true && lastCard != null)
                {
                    setCard(lastCard, card.GetComponent<CommandUICardController>().getUnit());
                    card.SetActive(false);
                }
                else if (card.activeSelf == true && card.GetComponent<CommandUICardController>().getUnit() == unit)
                {
                    card.transform.Find("Worker").gameObject.SetActive(true);
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

        public void clearCards()
        {
            cards.ForEach(card =>
            {
                count = 0;
                this.transform.Find("UnitCount").Find("Selected_Count").gameObject.GetComponent<Text>().text = count.ToString();

                card.transform.Find("Worker").gameObject.SetActive(true);
                card.transform.Find("Light Infantry").gameObject.SetActive(true);
                card.transform.Find("Archer").gameObject.SetActive(true);

                card.SetActive(false);
            });
        }
    }
}

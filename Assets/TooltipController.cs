using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TooltipController : MonoBehaviour
{
	private GameObject lackResources;
	private GameObject cityRadius;
	private GameObject insideTown;
    private GameObject buildingBlocked;

	private IEnumerator currentRoutine;

    // Start is called before the first frame update
    void Start()
    {
        lackResources = this.transform.Find("LackResources").gameObject;
        cityRadius = this.transform.Find("CityRadius").gameObject;
        insideTown = this.transform.Find("insideTown").gameObject;
        buildingBlocked = this.transform.Find("buildingBlocked").gameObject;

        clearUI();
    }

    public void flashLackResources() {
    	clearUI();
    	currentRoutine = flashUI(lackResources);
    	StartCoroutine(currentRoutine);
    }

    public void flashCityRadius() {
    	clearUI();
    	currentRoutine = flashUI(cityRadius);
    	StartCoroutine(currentRoutine);
    }

    public void flashInsideTown() {
    	clearUI();
    	currentRoutine = flashUI(insideTown);
    	StartCoroutine(currentRoutine);
    }

    public void flashBuildingBlocked() {
        clearUI();
        currentRoutine = flashUI(buildingBlocked);
        StartCoroutine(currentRoutine);
    }

    private IEnumerator flashUI(GameObject tooltip, float offset = 2f) {
        tooltip.SetActive(true);

        yield return new WaitForSeconds(offset);

        tooltip.SetActive(false);
    }

    private void clearUI() {
    	if (currentRoutine != null) {
    		StopCoroutine(currentRoutine);
    	}
    	
    	lackResources.SetActive(false);
        cityRadius.SetActive(false);
        insideTown.SetActive(false);
        buildingBlocked.SetActive(false);
    }
}

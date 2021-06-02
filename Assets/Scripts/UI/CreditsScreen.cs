using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CreditsScreen : MonoBehaviour
{
    [SerializeField] GameObject creditsPrefab;
    [SerializeField] Credits gameCredits;
    [SerializeField] Transform startingLoc;
    [SerializeField] Transform creditsHolder;
    [SerializeField] float delayBetweenSections;
    [SerializeField] float delayBetweenEntries;
    [SerializeField] float delayBetweenFirstEntry;
    [SerializeField] float creditsLiveTime;
    
    // Start is called before the first frame update
    IEnumerator Start()
    {
        yield return StartCoroutine(SpawnCredits());

        SceneManager.LoadScene("Main Menu");
    }

    IEnumerator SpawnCredits()
    {
        foreach(Credits.CreditSection section in gameCredits.GetSections())
        {
            GameObject go = Instantiate(creditsPrefab, startingLoc.position, Quaternion.identity, creditsHolder);
            Destroy(go, creditsLiveTime);
            go.GetComponent<CreditsEntry>().Setup(true, section.sectionHeader, "");
            yield return new WaitForSeconds(delayBetweenFirstEntry);

            foreach(Credits.CreditEntry entry in section.authors)
            {
                go = Instantiate(creditsPrefab, startingLoc.position, Quaternion.identity, creditsHolder);
                go.GetComponent<CreditsEntry>().Setup(false, entry.author, entry.website);
                Destroy(go, creditsLiveTime);
                yield return new WaitForSeconds(delayBetweenEntries);
            }

            yield return new WaitForSeconds(delayBetweenSections);
        }

        yield return new WaitForSeconds(2);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene("Main Menu");
    }
}

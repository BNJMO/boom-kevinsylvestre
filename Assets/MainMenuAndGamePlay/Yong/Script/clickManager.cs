using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class clickManager : MonoBehaviour
{

    public static clickManager instance;
    public AudioSource audioSource;
    public AudioClip buttonAudioClip;

    public GameObject FirstMenu;
    public GameObject Parametres;
    public GameObject Boutique;
    public GameObject Classement;
    public GameObject Creeuneequipe;
    public GameObject Creeuneequipe2;
    public GameObject Personnaliser;
    public GameObject Diamands;
    public GameObject Apparence;

    public GameObject SonoresYes;
    public GameObject SonoresNo;
    public GameObject MusiqueYes;
    public GameObject MusiqueNo;

    public GameObject Language;

    public GameObject Personnaliser2;
    public GameObject Explosif;
    public GameObject Equipement;

    public GameObject ChangeC;

    public GameObject buttonTaille;
    public GameObject buttonColor;

    public GameObject Abonnement;
    public GameObject Monabonnement;
    public GameObject Profile;

    public GameObject ChangeName;

    public GameObject name;

    public GameObject nameAff ;
    public GameObject trophAff;
    public GameObject bombAff;

    private string input;

    private string realName = "Player00";

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        nameAff.GetComponent<TextMeshProUGUI>().text= "Player00";
        trophAff.GetComponent<TextMeshProUGUI>().text = "0";
        bombAff.GetComponent<TextMeshProUGUI>().text = "10";
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void boutique()
    {
        Boutique.SetActive(true);
        Diamands.SetActive(false);
        Apparence.SetActive(false);
        FirstMenu.SetActive(false);
        ButtonSound();
    }

    public void classement()
    {
        Classement.SetActive(true);
        FirstMenu.SetActive(false);
        ButtonSound();
    }

    public void creeuneequipe()
    {
        Creeuneequipe.SetActive(true);
        FirstMenu.SetActive(false);
        ButtonSound();
    }
    
    public void creeuneequipeexit()
    {
        Creeuneequipe.SetActive(false);
        FirstMenu.SetActive(true);
        ButtonSound();
    }

    public void jouer()
    {
        ButtonSound();
    }

    public void personnaliser()
    {
        Personnaliser.SetActive(true);
        FirstMenu.SetActive(false);
        ButtonSound();
    }
    public void parametres()
    {
        Parametres.SetActive(true);
        FirstMenu.SetActive(false);
        ButtonSound();
    }

    public void diamand()
    {
        Boutique.SetActive(false);
        Diamands.SetActive(true);
        Apparence.SetActive(false);
        ButtonSound();
    }
    public void apparence()
    {
        Boutique.SetActive(false);
        Diamands.SetActive(false);
        Apparence.SetActive(true);
        ButtonSound();
    }

    public void language()
    {
        Language.SetActive(true);
        Parametres.SetActive(false);
        ButtonSound();
    }

    public void Returnlanguage()
    {
        Language.SetActive(false);
        Parametres.SetActive(true);
        ButtonSound();
    }

    public void Sonores()
    {
        if (SonoresYes.active)
        {
            SonoresYes.SetActive(false);
            SonoresNo.SetActive(true);
            ///////////////////////////////////
        }
        else
        {
            SonoresNo.SetActive(false);
            SonoresYes.SetActive(true);
            ///////////////////////////////////
        }
        ButtonSound();
    }
    public void Musique()
    {
        if (MusiqueYes.active)
        {
            MusiqueYes.SetActive(false);
            MusiqueNo.SetActive(true);
            ///////////////////////////////////
        }
        else
        {
            MusiqueYes.SetActive(true);
            MusiqueNo.SetActive(false);
            ///////////////////////////////////
        }
        ButtonSound();

    }

    public void personnaliser2()
    {
        Personnaliser2.SetActive(true);
        Explosif.SetActive(false);
        Equipement.SetActive(false);
        ButtonSound();
    }

    public void explosif()
    {
        Personnaliser2.SetActive(false);
        Explosif.SetActive(true);
        Equipement.SetActive(false);
        ButtonSound();
    }

    public void equipment()
    {
        Personnaliser2.SetActive(false);
        Explosif.SetActive(false);
        Equipement.SetActive(true);
        ButtonSound();
    }

    public void changec()
    {
        ChangeC.SetActive(true);
        Parametres.SetActive(false);
        ButtonSound();

    }

    public void changecReturn()
    {
        ChangeC.SetActive(false);
        Parametres.SetActive(true);
        Abonnement.SetActive(false);
        ButtonSound();

    }

    public void abonnement()
    {
        Abonnement.SetActive(true);
        Parametres.SetActive(false);
        ButtonSound();

    }

    public void odns()
    {
        Boutique.SetActive(true);
        Personnaliser.SetActive(false);
        ButtonSound();

    }

    public void profile()
    {
        Profile.SetActive(true);
        
        FirstMenu.SetActive(false);
        ButtonSound();

    }

    public void changeName()
    {
        ChangeName.SetActive(true);
        Profile.SetActive(false);
        ButtonSound();

        Debug.Log("fffff");

      
       

    }

    public void X()
    {
        ChangeName.SetActive(false);
        Profile.SetActive(true);
        ButtonSound();





    }

    public void namechange()
    {
        realName = name.GetComponent<TMP_InputField>().text;
        nameAff.GetComponent<TextMeshProUGUI>().text = realName;
        ChangeName.SetActive(false);
        Profile.SetActive(true);
        ButtonSound();


    }

    public void TAILLE()
    {
        Debug.Log("yejbed0");

        while (Input.GetMouseButtonDown(0))
        {
            var MPX = Input.mousePosition.x;
            buttonTaille.transform.position = new Vector3(buttonTaille.transform.position.x+MPX, 0, 0);
            Debug.Log("yejbed");
        }
      
       

    }

    public void COLOR()
    {
        while (Input.GetMouseButtonDown(0))
        {
            
        }



    }

    public void creeuneequipe2()
    {
        Creeuneequipe.SetActive(false);
        Creeuneequipe2.SetActive(true);
        ButtonSound();
    }



    public void Return()
    {
        Diamands.SetActive(false);
        Apparence.SetActive(false);
        Parametres.SetActive(false);
        Personnaliser.SetActive(false);
        Creeuneequipe.SetActive(false);
        Creeuneequipe2.SetActive(false);
        Classement.SetActive(false);
        Boutique.SetActive(false);
        FirstMenu.SetActive(true);
        Explosif.SetActive(false);
        Equipement.SetActive(false);
        Profile.SetActive(false);
        Monabonnement.SetActive(false);



        ButtonSound();
    }

    public void monabonnement()
    {
        Monabonnement.SetActive(true);
        ButtonSound();
    }

    void ButtonSound()
    {
        audioSource.PlayOneShot(buttonAudioClip);
    }
}

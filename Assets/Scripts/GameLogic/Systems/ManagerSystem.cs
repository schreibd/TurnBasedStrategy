﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ManagerSystem : MonoBehaviour {

    

    public int p1UnitCap = 6;
    public int p2UnitCap = 5;

    public List<GameObject> unitListP1;
    public List<GameObject> unitListP2;

    
    public GameObject activeUnitMarker;

    CameraRotationScript cam;
    public int rounds;             //Spiegelt Rundenzahl wieder
    private bool isPlayer1;         //Spieler1 an der Reihe
    GameObject player1;
    GameObject player2;
    public GameObject selectedFigurine;    //Aktuell ausgewählte Spielfigur
    int roundHalf;  //1 wenn Spieler1 seinen Turn beendet, 2 wenn Spieler2 seinen Turn beendet;

    //Verweise auf andere System
    private ShootingSystem shootingSys;
    public GameObject unit;
    public GameObject uiManager;
    public GameObject map;
    public GameObject plane;

    public AudioSource endTurnSound;

   	// Use this for initialization
	void Start () {

        rounds = 0;
        isPlayer1 = true;
        player1 = GameObject.Find("Player1");
        player2 = GameObject.Find("Player2");
        player2.GetComponent<inputSystem>().enabled = false;
        cam = GameObject.Find("Main Camera").GetComponent<CameraRotationScript>();
        shootingSys = (ShootingSystem)this.gameObject.GetComponent(typeof(ShootingSystem));

        plane = GameObject.Find("Plane");

        
    }
	
	// Update is called once per frame
	void Update () {

        activeUnitMark();

	}

    public void loadUI()
    {
        Instantiate(uiManager);
        selectedFigurine = unitListP1[0];
        isPlayer1 = true;
    }


    public void shoot(GameObject attacker, GameObject target)
    {
        if(shootingSys.shoot(attacker, target))
        {
            Debug.Log("Getroffen");
        }
        else
        {
            Debug.Log("Nicht getroffen");
        }
    }
    //Runde wird inkrementiert && AP werden wieder aufgefüllt
    void nextRound()
    {
        player1.GetComponent<PlayerComponent>().regenerateAP(); //Füllt AP von Spieler1 wieder auf
        player2.GetComponent<PlayerComponent>().regenerateAP(); //Füllt AP von Spieler2 wieder auf
        rounds++;
        Debug.Log("Runde: " + rounds);
    }

    //Liefer true, wenn Spieler1 am Zug
    public bool getPlayerTurn()
    {
        return isPlayer1;
    }

    //Legt fest, welcher Spieler am Zug ist
    public void setPlayerTurn()
    {
        endTurnSound.Play();
        roundHalf++;
        if(roundHalf == 2)
        {
            nextRound();
            roundHalf = 0;
        }
        isPlayer1 = !isPlayer1;
        if(isPlayer1) //wenn Spieler 1 dran ist
        {
            //To-Do: Mit UI verknüpfen 
            Debug.Log("Spieler1 ist am Zug");
            setSelectedFigurine(unitListP1[0]);               //Wählt das erste Child von Spieler2
            cam.setNewTarget(selectedFigurine);                 //Gibt der Kamera ein neues Target
            player1.GetComponent<inputSystem>().enabled = true;                         //Aktiviere InputSys von Spieler1
            player2.GetComponent<inputSystem>().enabled = false;
        }
        else
        {
            //To-Do: Mit UI verknüpfen 
            setSelectedFigurine(unitListP2[0]);             //Wählt das erste Child von Spieler2
            cam.setNewTarget(selectedFigurine);              //Gibt der Kamera ein neues Target
            player1.GetComponent<inputSystem>().enabled = false;
            player2.GetComponent<inputSystem>().enabled = true;                      //Aktiviere InputSys von Spieler2
        }

        plane.GetComponent<DijkstraSystem>().resetDijkstra();

    }

    public void setSelectedFigurine(GameObject selected)
    {
        selectedFigurine = selected;
    }

    public GameObject getSelectedFigurine()
    {
        return selectedFigurine;
    }


    public void addUnit(int team)
    {

        //erzeuge einheit
        GameObject tmp = Instantiate(unit);

        if (team == 1)
        {
            tmp.transform.SetParent(player1.transform);
            unitListP1.Add( tmp );
            tmp.tag = "FigurSpieler1";
            tmp.GetComponent<AttributeComponent>().team = 1;
        }
        else if (team == 2)
        {
            tmp.transform.SetParent(player2.transform);
            unitListP2.Add(tmp );
            tmp.tag = "FigurSpieler2";
            tmp.GetComponent<AttributeComponent>().team = 2;
        }

        
        placeUnit(team, tmp);
        
    }


    public void activeUnitMark()
    {
        if (selectedFigurine)
        {
            activeUnitMarker.transform.position = selectedFigurine.transform.position;
            activeUnitMarker.transform.rotation = selectedFigurine.transform.rotation;
        }
        

    }


    public void placeUnit(int team, GameObject unit)
    {

        Vector2 posi = new Vector2(0,0);
        
        float sizeX = map.GetComponent<BattlefieldCreater>().mapSizeX;
        float sizeZ = map.GetComponent<BattlefieldCreater>().mapSizeZ;

        if (team == 1)
        {
            posi = map.GetComponent<BattlefieldCreater>().startPostionsP1[unitListP1.Count];           
        }
        else if (team == 2)
        {
            posi = map.GetComponent<BattlefieldCreater>().startPostionsP2[unitListP1.Count];
        }

        //setze map Coordinaten
        unit.GetComponent<ObjectSetter>().x = (int) posi.x;
        unit.GetComponent<ObjectSetter>().z = (int) posi.y;
        unit.GetComponent<AttributeComponent>().setCurrentCell((int)posi.x, (int)posi.y);



        //setze welt coordinaten
        float xBase = map.transform.position.x -map.transform.localScale.x *10 /2;
        float yBase = map.transform.position.y;
        float zBase = 0;


        unit.transform.position = new Vector3(xBase + (int)posi.x + 0.5f, yBase + 0.52f, zBase - (int)posi.y - 0.5f );
        // tmp.transform.position = new Vector3(x,1.0f,z)



    }

    public void removeUnit(GameObject unit, int team)
    {

        //spiel zuende
        if(unitListP1.Count <= 1){
            Debug.Log("Spieler 2 gewinnt");
            ende();
        }else if(unitListP2.Count <= 1){
            Debug.Log("Spieler 1 gewinnt");
            ende();
        }



        if (team == 1)
        {
            //sterbende Figur ist aktuelle figur
            if (unit == selectedFigurine)
            {
                selectedFigurine = unitListP1[0];
                activeUnitMark();
            }

            unitListP1.Remove(unit);
        }
        else if (team == 2)
        {
            //sterbende Figur ist aktuelle figur
            if (unit == selectedFigurine)
            {
                selectedFigurine = unitListP2[0];
                activeUnitMark();
            }
            unitListP2.Remove(unit);
        }




    }


    public void ende()
    {
        Debug.Log("Ende");
    }

}

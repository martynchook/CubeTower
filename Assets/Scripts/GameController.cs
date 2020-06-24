﻿using UnityEngine;
using UnityEngine.EventSystems; 
using System;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

    private CubePos nowCube = new CubePos(0, 1, 0); // координаты последнего установленного куба
    public float cubeChangePlaceSpeed = 0.5f; // скорость смены позиций устанавливаемого куба
    public Transform cubeToPlace; // ссылка на объект cubeToPlace
    private float camMoveToYPosition, camMoveSpeed = 2f;

    public GameObject cubeToCreate, allCubes, vfx;
    public GameObject[] canvasStartPage;
    private Rigidbody allCubesRb;
    private bool isLose, firstCube;
    private Coroutine showCubePlace;
    private int prevCountMaxHorizontal;
    public Color [] bgColor;
    private Color toCameraColor;

    private List<Vector3> allCubesPosition = new List<Vector3> { // список занятых позиций (не пригодных для установки куба)
        new Vector3 (0, 1, 0),
        new Vector3 (0, 0, 0),
        new Vector3 (1, 0, 0),
        new Vector3 (-1, 0, 0), 
        new Vector3 (0, 0, 1),
        new Vector3 (0, 0, -1), 
        new Vector3 (1, 0, 1),
        new Vector3 (1, 0, -1),
        new Vector3 (-1, 0, 1),
        new Vector3 (-1, 0, -1),
    };

    IEnumerator ShowCubePlace() { // куратина меняющяя позицию устанавливаемого куба
        while(true) {
            SpawnPosition ();
            yield return new WaitForSeconds(cubeChangePlaceSpeed);
        }
    }
    
    private void SpawnPosition () { // метод проверяет все позиции вокруг текщего куда и добавляет свободные в лист
        List <Vector3> positions = new List <Vector3>(); // списк свободных позиций

        if(IsPositionEmpty(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z)) && nowCube.x + 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x + 1, nowCube.y, nowCube.z));

        if(IsPositionEmpty(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z)) && nowCube.x - 1 != cubeToPlace.position.x)
            positions.Add(new Vector3(nowCube.x - 1, nowCube.y, nowCube.z));
            
        if(IsPositionEmpty(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z)) && nowCube.y + 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y + 1, nowCube.z));

        if(IsPositionEmpty(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z)) && nowCube.y - 1 != cubeToPlace.position.y)
            positions.Add(new Vector3(nowCube.x, nowCube.y - 1, nowCube.z));

        if(IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1)) && nowCube.z + 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z + 1));

        if(IsPositionEmpty(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1)) && nowCube.z - 1 != cubeToPlace.position.z)
            positions.Add(new Vector3(nowCube.x, nowCube.y, nowCube.z - 1));

        if(positions.Count > 1) {
            cubeToPlace.position = positions[UnityEngine.Random.Range(0, positions.Count)];
        } else if (positions.Count == 0) {
            isLose = true;
        } else {
            cubeToPlace.position = positions[0];
        }
       
    }

    private bool IsPositionEmpty (Vector3 targetPos) { // проверка пустая ли позиция по передаваемому вектору (сравниваем со списком занятых позиций)
        if (targetPos.y == 0) {
            return false;
        }
        foreach (Vector3 pos in allCubesPosition) {
             if(pos.x == targetPos.x && pos.y == targetPos.y && pos.z == targetPos.z) {
                return false;
            }
        }
        return true;
    }

    private void MoveCameraChangeBg () {
        int max_X = 0, max_Y = 0, max_Z = 0, maxHor;
        foreach (Vector3 pos in allCubesPosition) {
            if (Mathf.Abs(Convert.ToInt32(pos.x))> max_X) {
                max_X =Convert.ToInt32(pos.x);
            }
             if (Convert.ToInt32(pos.y)> max_Y) {
                max_Y =Convert.ToInt32(pos.y);
            }
             if (Mathf.Abs(Convert.ToInt32(pos.z))> max_Z) {
                max_Z =Convert.ToInt32(pos.z);
            }
            
            camMoveToYPosition = 5.9f + nowCube.y - 1f;

            maxHor = max_X > max_Z ? max_X : max_Z;
            if(maxHor % 3 == 0 && prevCountMaxHorizontal != maxHor) {
                mainCam.localPosition -= new Vector3(0, 0, 2.5f);
                prevCountMaxHorizontal = maxHor;
            }

            if (max_Y >= 15) {
               toCameraColor = bgColor[2];
            } else if (max_Y >= 10) {
              toCameraColor = bgColor[1];
            } else if (max_Y >= 5) {
               toCameraColor = bgColor[0];
            }
        }
    }
    

    private Transform mainCam;

    private void Start() {
        toCameraColor = Camera.main.backgroundColor;
        mainCam = Camera.main.transform;
        camMoveToYPosition = 5.9f + nowCube.y - 1f; 

        allCubesRb = allCubes.GetComponent<Rigidbody>();
        showCubePlace = StartCoroutine(ShowCubePlace());
    }

    private void Update() {  
        if ((Input.GetMouseButtonDown(0) || Input.touchCount > 0) && allCubes!=null && cubeToPlace != null && !EventSystem.current.IsPointerOverGameObject()) {
            #if !UNITY_EDITOR
                if (Input.GetTouch(0).phase != TouchPhase.Began)
                return;
            #endif

            if(!firstCube) {
                firstCube = true;
                foreach(GameObject obj in canvasStartPage) {
                    Destroy(obj);
                }
            }

            GameObject newCube = Instantiate (cubeToCreate, cubeToPlace.position, Quaternion.identity) as GameObject;
            newCube.transform.SetParent(allCubes.transform);
            nowCube.setVector(cubeToPlace.position);
            allCubesPosition.Add(nowCube.GetVector());
            
            if (PlayerPrefs.GetString("music") != "No") {
                GetComponent<AudioSource>().Play();
            }

            GameObject newVfx = Instantiate(vfx,  newCube.transform.position, Quaternion.identity) as GameObject;
            Destroy(newVfx, 1.5f);

            allCubesRb.isKinematic = true;
            allCubesRb.isKinematic = false;

            SpawnPosition();
            MoveCameraChangeBg();
        }

        if (!isLose && allCubesRb.velocity.magnitude > 0.1f) {
            Destroy(cubeToPlace.gameObject);
            isLose = true;
            StopCoroutine(showCubePlace);
        }
        mainCam.localPosition = Vector3.MoveTowards( mainCam.localPosition, new Vector3(mainCam.localPosition.x, camMoveToYPosition, mainCam.localPosition.z), camMoveSpeed * Time.deltaTime );
        
        if (Camera.main.backgroundColor != toCameraColor) {
            Camera.main.backgroundColor = Color.Lerp(Camera.main.backgroundColor, toCameraColor, Time.deltaTime / 1.5f);
        } 
    }

}

/*==============================================================*/

struct CubePos { // структура для хранение координат объекта

    public int x, y, z;

    public CubePos ( int x, int y, int z) {
        this.x = x;
        this.y = y;
        this.z = z;
    }

    public Vector3 GetVector () {
        return new Vector3(x, y, z);
    }

    public void setVector(Vector3 pos) {
        x = Convert.ToInt32(pos.x);
        y = Convert.ToInt32(pos.y);
        z = Convert.ToInt32(pos.z);
    }
    
}
 
 /*==============================================================*/
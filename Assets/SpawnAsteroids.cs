using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnAsteroids : MonoBehaviour
{

  public GameObject Asteroid;
  // Start is called before the first frame update
  void Start(){
    for(int i=0;i<300;i++){
      Vector3 position = new Vector3(Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f), Random.Range(-100.0f, 100.0f));
      GameObject ast=Instantiate(Asteroid, position, Quaternion.identity);
      float scale=Random.Range(3.0f, 10.0f);
      ast.transform.localScale = new Vector3(scale, scale, scale);
    }
  }

  // Update is called once per frame
  void Update(){
        
  }
}

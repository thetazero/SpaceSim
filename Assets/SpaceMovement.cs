using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceMovement : MonoBehaviour
{
  // Start is called before the first frame update
  float speed = 0.0f;
  float accel = 0.01f;
  float forward_max_speed = 1.0f;
  float backward_max_speed = 0.1f;
  float damp = 0.0002f;
  public GameObject HUD;
  public GameObject circleSprite;
  private int hudCircles = 100;
  public GameObject[] hudCircleObjects;
  public Transform camTransform;
  public Camera cam;

  float rollVelo = 0.0f;
  float rollAccel = 0.1f;
  float rollDamp = 0.07f;
  float rollMaxVelo = 0.4f;

  void Start()
  {
    Cursor.lockState = CursorLockMode.Confined;
    Cursor.visible = true;
    hudCircleObjects = new GameObject[hudCircles];
    for (int i = 0; i < hudCircles; i++)
    {
      var x = Instantiate(circleSprite, HUD.transform);
      hudCircleObjects[i] = x;
    }
  }

  // Update is called once per frame
  void FixedUpdate()
  {
    if (Input.GetKey(KeyCode.W))
    {
      speed = clamp(KEAdd(speed, accel), forward_max_speed);
      // Mathf.Min(accel * Time.deltaTime + speed * speed, max_speed * max_speed);
      // speed = Mathf.Sqrt(speed);
    }
    else if (Input.GetKey(KeyCode.S))
    {
      // speed = Mathf.Max(-accel * Time.deltaTime - speed * speed, -max_speed * max_speed);
      // speed = Mathf.Sqrt(-speed);

      speed = Mathf.Clamp(KEAdd(speed, -accel), -backward_max_speed, forward_max_speed);
    }
    else
    {
      speed = dampen(speed, damp);
    }

    if (Input.GetKey(KeyCode.Q))
    {
      rollVelo = KEAdd(rollVelo, rollAccel);
    }
    else if (Input.GetKey(KeyCode.E))
    {
      rollVelo = KEAdd(rollVelo, -rollAccel);
    }
    else
    {
      rollVelo = dampen(rollVelo, rollDamp);
    }
    rollVelo = Mathf.Clamp(rollVelo, -rollMaxVelo, rollMaxVelo);
    setHUDCircles();
    Vector2 mouseDelta = getMouseDelta();
    float yangle = mouseDelta.y;
    float xangle = mouseDelta.x;
    Vector3 rot = new Vector3(-yangle, rollVelo, -xangle) * 3.0f;
    if (Mathf.Abs(rot.magnitude) > 0.01f)
    {
      transform.Rotate(new Vector3(-yangle, rollVelo, -xangle) * 3.0f, Space.Self);//up & down
    }
    float dtheta = Mathf.Sqrt(yangle * yangle + xangle * xangle);
    speed = dampen(speed, speed * mouseDelta.magnitude / 150.0f);//damp due to rotation
    if (speed > 0)
    {
      cam.fieldOfView = (60.0f + 20.0f * speed / forward_max_speed);
    }
    else
    {
      cam.fieldOfView = (60.0f + 20.0f * -speed / forward_max_speed);
    }

    transform.position += transform.up * speed;
  }

  void setHUDCircles()
  {
    Vector3 Start = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    Vector3 End = Input.mousePosition;

    int spriteGap = 24;
    int showNum = (int)Mathf.Min(hudCircleObjects.Length, Vector3.Distance(Start, End) / spriteGap);
    for (int i = 0; i < showNum; i++)
    {
      hudCircleObjects[i].transform.position = Start + i * Vector3.Normalize(End - Start) * spriteGap;
      var img = hudCircleObjects[i].GetComponent<RawImage>();
      var color = img.color;
      color.a = 0.3f + 0.5f * (float)(showNum - i) / (float)(showNum);
      img.color = color;
    }
    for (int i = showNum; i < hudCircleObjects.Length; i++)
    {
      hudCircleObjects[i].transform.position = Start;
    }
  }

  float KEAdd(float val, float amount)
  {
    if (val > 0)
    {
      return keepSignSqrt(val * val + amount);
    }
    return keepSignSqrt(-val * val + amount);
  }

  float keepSignSqrt(float val)
  {
    if (val < 0)
    {
      return -Mathf.Sqrt(-val);
    }
    return Mathf.Sqrt(val);
  }

  Vector2 getMouseDelta()
  {
    float w = Screen.width;
    float h = Screen.height;
    return new Vector2((Input.mousePosition.x - w / 2) / w, (Input.mousePosition.y - h / 2) / h);
  }

  float dampen(float val, float k)
  {

    if (val < 0)
    {
      val += k;
    }
    else
    {
      val -= k;
    }
    if (Mathf.Abs(val) <= k)
    {
      return 0.0f;
    }
    return val;
  }

  float clamp(float val, float lim)
  {
    if (val < 0)
    {
      return Mathf.Max(val, -lim);
    }
    else
    {
      return Mathf.Min(val, lim);
    }
  }
}

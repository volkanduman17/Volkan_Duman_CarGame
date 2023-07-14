using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class Car : MonoBehaviour
{
    public float speed;
    public float turnSpeed;
    private Rigidbody rb;

    private Vector3 startPos;

    private bool isCompleted;

    private float recordInterval = 0.05f; // Ardýþýk dönüþler arasýndaki kayýt aralýðý
    private float lastRecordTime; // Son kaydýn yapýldýðý zama

    [SerializeField] private List<Vector3> path = new List<Vector3>(); // Arabanýn izlediði yolun saklandýðý liste

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        startPos = transform.position;
    }

    



    private void Update()
    {
        if (isCompleted)
            return;

        
            Turn();     
            Move();
    }

        int currentIndex;
        Vector3 currentTarget;


    public void ReplayRoad()
    {
        transform.DOKill();

        if (!isCompleted)
        {
            return;
        }
        Sequence sequence = DOTween.Sequence();
        float totalTime = 5f;
        for (int i = 0; i < path.Count-5; i++)
        {
            Vector3 item = path[i];
            sequence.Append(transform.DOMove(item, totalTime/path.Count)).Join(transform.DOLookAt(item, totalTime/path.Count));
        }

    }


    private void Turn()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            if (touch.position.x > Screen.width / 2)
            {
               
                    Debug.Log("Saða döndün");
                    transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
                    rb.angularVelocity = Vector3.zero;                   
                
            }
            else if (touch.position.x < Screen.width / 2)
            {
                
                    Debug.Log("Sola döndün");
                    transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
                    rb.angularVelocity = Vector3.zero;

                    
                
            }
        }
    }

    private void Move()
    {
        Vector3 forwardForce = transform.forward * speed * Time.deltaTime;
        forwardForce.y = 0f; // Y eksenindeki kuvveti sýfýrlayarak düz bir hareket saðlar
        rb.AddForce(forwardForce, ForceMode.VelocityChange);
        // Sadece kaydýn yapýldýðý zamaný güncelleyin

    if (Time.time > lastRecordTime + recordInterval)
    {
            lastRecordTime = Time.time;

            // Sadece dönüþlerde RecordPath fonksiyonunu çaðýrýn
            RecordPath(transform.position);
    }
    }

    public void SetTargetPoint(Transform target)
    {
        Vector3 targetPosition = target.position;
        // Hedefe doðru yönelmek için dönüþ rotasyonunu hesaplayýn
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        // Arabayý hedefe doðru yöneltin
        transform.rotation = targetRotation;
    }

    public void RecordPath(Vector3 position)
    {
        // Arabanýn izlediði yolu kaydetmek için path listesine pozisyon ekleyin
        path.Add(transform.position);
    }

    public void ResetPath()
    {
        // Yeni bir araba spawn olduðunda path listesini sýfýrlayýn
        path.Clear();
        path.Add(transform.position); // Arabanýn þu anki konumunu path listesine ekleyin
    }

    public List<Vector3> GetPath()
    {
        // Arabanýn izlediði yolu döndürmek için kullanýlýr
        return path;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Point") && other.GetComponent<MeshRenderer>().enabled && isCompleted==false)
        {
            
            isCompleted = true;
            ReplayRoad();
            currentIndex = 0;
            transform.position = startPos;
            // Hedefe ulaþýldýðýnda GameManager'a bildir
            GameManager.Instance.CarReachedTarget(this);
        }
        else if ((other.CompareTag("Car") || other.CompareTag ("Obstacle")) && isCompleted == false)
        {
            GameManager.Instance.HandleCarCrash();
            transform.position = startPos;
            path.Clear();
        }
    }
}
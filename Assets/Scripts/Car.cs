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

    private float recordInterval = 0.05f; // Ard���k d�n��ler aras�ndaki kay�t aral���
    private float lastRecordTime; // Son kayd�n yap�ld��� zama

    [SerializeField] private List<Vector3> path = new List<Vector3>(); // Araban�n izledi�i yolun sakland��� liste

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
               
                    Debug.Log("Sa�a d�nd�n");
                    transform.Rotate(Vector3.up, turnSpeed * Time.deltaTime);
                    rb.angularVelocity = Vector3.zero;                   
                
            }
            else if (touch.position.x < Screen.width / 2)
            {
                
                    Debug.Log("Sola d�nd�n");
                    transform.Rotate(Vector3.up, -turnSpeed * Time.deltaTime);
                    rb.angularVelocity = Vector3.zero;

                    
                
            }
        }
    }

    private void Move()
    {
        Vector3 forwardForce = transform.forward * speed * Time.deltaTime;
        forwardForce.y = 0f; // Y eksenindeki kuvveti s�f�rlayarak d�z bir hareket sa�lar
        rb.AddForce(forwardForce, ForceMode.VelocityChange);
        // Sadece kayd�n yap�ld��� zaman� g�ncelleyin

    if (Time.time > lastRecordTime + recordInterval)
    {
            lastRecordTime = Time.time;

            // Sadece d�n��lerde RecordPath fonksiyonunu �a��r�n
            RecordPath(transform.position);
    }
    }

    public void SetTargetPoint(Transform target)
    {
        Vector3 targetPosition = target.position;
        // Hedefe do�ru y�nelmek i�in d�n�� rotasyonunu hesaplay�n
        Quaternion targetRotation = Quaternion.LookRotation(targetPosition - transform.position);
        // Arabay� hedefe do�ru y�neltin
        transform.rotation = targetRotation;
    }

    public void RecordPath(Vector3 position)
    {
        // Araban�n izledi�i yolu kaydetmek i�in path listesine pozisyon ekleyin
        path.Add(transform.position);
    }

    public void ResetPath()
    {
        // Yeni bir araba spawn oldu�unda path listesini s�f�rlay�n
        path.Clear();
        path.Add(transform.position); // Araban�n �u anki konumunu path listesine ekleyin
    }

    public List<Vector3> GetPath()
    {
        // Araban�n izledi�i yolu d�nd�rmek i�in kullan�l�r
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
            // Hedefe ula��ld���nda GameManager'a bildir
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
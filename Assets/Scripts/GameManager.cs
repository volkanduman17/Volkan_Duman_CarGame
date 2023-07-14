using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public Car carPrefab; // Araba prefab'i
    public List<Transform> spawnPoints; // Arabalarýn spawn noktalarý
    public List<Transform> targetPoints; // Arabalarýn hedef noktalarý

    public List<Car> cars = new List<Car>(); // Tüm arabalarýn listesi
    private int currentCarIndex = 0; // Þu anki arabanýn index'i

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        SpawnCar();
    }

    IEnumerator WaitBeforeCurrentCar()
    {
        yield return new WaitForSeconds(2f);
    }


    public void HandleCarCrash()
    {
        WaitBeforeCurrentCar() ;
        foreach (var item in cars)
        {
            item.ReplayRoad();
        }
    }

    private void SpawnCar()
    {
        if (currentCarIndex < spawnPoints.Count)
        {
            // Arabayý spawn edin ve baþlangýç noktasýna yerleþtirin
            Car newCar = Instantiate(carPrefab, spawnPoints[currentCarIndex].position, spawnPoints[currentCarIndex].rotation);
            cars.Add(newCar);

            // Arabanýn hedefini ayarlayýn
            foreach (var item in targetPoints)
            {
                item.GetComponent<MeshRenderer>().enabled = false;
                if (item==targetPoints[currentCarIndex])
                {
                    item.GetComponent<MeshRenderer>().enabled = true;
                }
            }
            newCar.SetTargetPoint(targetPoints[currentCarIndex]);

            // Daha önceki arabalarýn yolu takip etmesi için path listesini güncelleyin
            for (int i = 0; i < cars.Count  ; i++)
            {
                cars[i].RecordPath(cars[i].transform.position);
            }

            currentCarIndex++;
        }
    }

    public void CarReachedTarget(Car car)
    {
        if (currentCarIndex==8)
        {
            // Son arabaysa ve hedefe ulaþmýþsa oyunu bitir
            EndGame();
        }
        else
        {
            StartCoroutine(SpawnNewCar());
            HandleCarCrash();
        }
    }

    private IEnumerator SpawnNewCar()
    {
        yield return new WaitForSeconds(0.5f); 
        SpawnCar();
    }

    private void EndGame()
    {
        Debug.Log("Oyun bitti!");

        // Arabalarý sýfýrla ve yeniden spawn et
        foreach (Car car in cars)
        {
            car.ResetPath();
            Destroy(car.gameObject);
        }
        cars.Clear();

        currentCarIndex = 0;

    }
}
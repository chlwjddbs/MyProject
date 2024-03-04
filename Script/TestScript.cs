using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TestScript : MonoBehaviour
{
    public List<GameObject> PuzzleDoorSet_1;
    private bool trigger = true;
    public TextMeshProUGUI test;
    public float testf;
    private Vector3 a;
    private Vector3 a1;
    private Vector3 a2;
    private Vector3 a3;
    private Vector3 a4;

    public int test_a;
    public int test_b;
    public int test_c;

    public List<int> testList = new List<int>();
    public int[] testArray;
    // Start is called before the first frame update
    void Start()
    {
        /*
        a = new Vector3(0, 0, 0);
        a1 = new Vector3(1, 0, 1);
        a2 = new Vector3(1, 0, -1);
        a3 = new Vector3(-1, 0, -1);
        a4 = new Vector3(-1, 0, 1);

        Debug.Log((a1 - a).normalized);
        Debug.Log((a2 - a).normalized);
        Debug.Log((a3 - a).normalized);
        Debug.Log((a4 - a).normalized);
        */
        Num();
        Nums();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            Cursor.visible = false;
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            Cursor.visible = true;
        }

        if (Input.GetKeyDown(KeyCode.A))
        {
            /*
            testList = new List<int>(8);
            testList.Add(2);
            testList[1] = 5;
            testList[9] = 3;
            */
            OptionalParameterTest(test_a);
            OptionalParameterTest(test_a, test_b);
            OptionalParameterTest(test_a, test_b, test_c);
            OptionalParameterTest(test_a, test:test_c);
        }
    }

    public void OptionalParameterTest(int _a,int _b = 1, int test = 0)
    {
        Debug.Log(_a + " " + _b + " " + test);
    }

    public void Test2(int _a, int _b = 1, int _c = 0)
    {
        Debug.Log(_a + " " + _b + " " + _c);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("╬Нец");
        }
    }

    private void Num()
    {      
        for (int i = 0; i < 6; i++)
        {
            int ran = Random.Range(0, 10);
            Debug.Log(ran);

        }
    }

    private void Nums()
    {
        List<int> nums = new List<int>(5);

        for (int i = 0; i < 6; i++)
        {
            int ran = Random.Range(1, 46);
            if (nums.Contains(ran))
            {
                i--;
            }
            else
            {
                nums.Add(ran);
            }
        }
        nums.Sort();
        Debug.Log(nums[0] + "," + nums[1]+ ","+ nums[2]+ "," + nums[3]+ "," + nums[4]+ "," + nums[5]);
    }
}

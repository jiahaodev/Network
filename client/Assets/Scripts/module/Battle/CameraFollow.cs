using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    //����ʸ��
    public Vector3 distance = new Vector3(0, 15, -22);
    //���
    new public Camera camera;
    //ƫ��ֵ
    public Vector3 offset = new Vector3(0, 8f, 0);
    //����ƶ��ٶ�
    public float speed = 6f;
    //������С����
    public float minDistanceZ = -35f;
    public float maxDistanceZ = -10f;
    //����仯�ٶ�
    public float zoomSpeed = 2f;

    //��ʼ��
    private void Start()
    {
        //Ĭ��Ϊ�����
        camera = Camera.main;
        //����ĳ�ʼλ��
        Vector3 pos = transform.position;
        Vector3 forward = transform.forward;
        Vector3 initPos = pos - 30 * forward + Vector3.up * 10;
        camera.transform.position = initPos;
    }

    //��������
    void Zoom()
    {
        float axis = Input.GetAxis("Mouse ScrollWheel");
        distance.z += axis * zoomSpeed;
        distance.z = Mathf.Clamp(distance.z, minDistanceZ, maxDistanceZ);
    }

    //�����Ƕ�
    void Rotate()
    {
        if (!Input.GetMouseButton(1))
        {//�Ҽ�
            return;
        }
        float axis = Input.GetAxis("Mouse X");
        distance.x += 2 * axis;
        distance.x = Mathf.Clamp(distance.x, -20, 20);
    }

    //������� update֮����
    void LateUpdate()
    {
        //̹��λ��
        Vector3 pos = transform.position;
        //̹�˷���
        Vector3 forward = transform.forward;
        Vector3 rigiht = transform.right;
        //���Ŀ��λ��
        Vector3 targetPos = pos;
        targetPos = pos + forward * distance.z + rigiht * distance.x;
        targetPos.y += distance.y;
        //���λ��
        Vector3 cameraPos = camera.transform.position;
        cameraPos = Vector3.MoveTowards(cameraPos, targetPos, Time.deltaTime * speed);
        camera.transform.position = cameraPos;
        //��׼̹��
        Camera.main.transform.LookAt(pos + offset);
        //��������
        Zoom();
        //�����Ƕ�
        Rotate();
    }

}